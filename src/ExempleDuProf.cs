using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace AtelierXNA
{
   public class ObjetDeBasePhysique : Microsoft.Xna.Framework.DrawableGameComponent
   {
      protected Atelier Jeu { get; private set; }
      private string NomModèle { get; set; }
      protected Model Modèle { get; private set; }
      protected Matrix Monde;
      public Vector3 Position { get; protected set; }
      public float Échelle { get; protected set; }
      public Vector3 Rotation { get; protected set; }

      List<BoundingBox> ListeDesBoîtes { get; set; }

      float angle_;
      public float Angle
      {
         get
         {
            if (Jeu.GestionFPS.ValFPS > 0)
            {
               angle_ += (2 * MathHelper.Pi) / (Jeu.GestionFPS.ValFPS*4);
            }
            else
            {
               angle_ = 0;
            }
            return angle_;
         }
         set { angle_ = value; }
      }

      bool pause_;
      bool Pause
      {
         get { return pause_; }
         set
         {
            pause_ = value;
         }
      }

      public ObjetDeBasePhysique(Atelier jeu, String nomModèle, Vector3 positionInitiale, float échelleInitiale, Vector3 rotationInitiale)
         : base(jeu)
      {
         Jeu = jeu;
         NomModèle = nomModèle;
         Position = positionInitiale;
         Échelle = échelleInitiale;
         Rotation = rotationInitiale;
      }

      public override void Initialize()
      {
         Modèle = Jeu.ModelsMgr.Find(NomModèle);
         Angle = 0;
         Pause = true;
         Monde = Matrix.Identity * Matrix.CreateScale(Échelle);
         Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
         Monde.Translation = Position;
         CréerListeDesBôites();
         VisualiserSphèreDeCollision();
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         GérerClavier();
         if (!Pause)
         {
            Monde = Matrix.Identity * Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Angle, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
         }
         base.Update(gameTime);
      }

      private void GérerClavier()
      {
         if (Jeu.InputMgr.EstNouvelleTouche(Keys.Space))
         {
            Pause = !Pause;
         }
      }

      public override void Draw(GameTime gameTime)
      {
         //Jeu.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
         // Au cas où le modèle se composerait de plusieurs morceaux
         Matrix[] transformations = new Matrix[Modèle.Bones.Count];
         Modèle.CopyAbsoluteBoneTransformsTo(transformations);
         foreach (ModelMesh maille in Modèle.Meshes)
         {
            Matrix mondeLocal = transformations[maille.ParentBone.Index] * GetMonde();
            foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
            {
               BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
               effet.EnableDefaultLighting();
               effet.Projection = Jeu.CaméraJeu.Projection;
               effet.View = Jeu.CaméraJeu.Vue;
               effet.World = mondeLocal;
            }
            maille.Draw();
         }
         base.Draw(gameTime);
      }

      public void VisualiserSphèreDeCollision()
      {
         foreach (ModelMesh maille in Modèle.Meshes)
         {
            BoundingSphere sphèreDeCollision = maille.BoundingSphere;
            sphèreDeCollision = sphèreDeCollision.Transform(Monde);
            SphèreDeCollision Sphère = new SphèreDeCollision(Jeu, sphèreDeCollision.Center, sphèreDeCollision.Radius, 16, 16, "rouge");
            Sphère.Visible = false;
            Jeu.Components.Add(Sphère);
         }
      }

      public virtual Matrix GetMonde()
      {
         return Monde;
      }

      public bool EstEnCollision(BoundingSphere sphèreCollision)
      {
         bool collision = false;
         for (int i = 0; i < Modèle.Meshes.Count; ++i)
         {
            BoundingSphere sphèreCollisionDuMaillage = Modèle.Meshes[i].BoundingSphere;
            if (sphèreCollisionDuMaillage.Transform(Monde).Intersects(sphèreCollision))
            {
               collision = true;
               break;
            }
         }
         return collision;
      }

      public bool EstEnCollision(BoundingBox boîteCollision)
      {
         bool collision = false;
         for (int i = 0; i < ListeDesBoîtes.Count; ++i)
         {
            BoundingBox boîteDeCollisionDuMaillage = ListeDesBoîtes[i];
            Vector3[] listeDesCoins = boîteDeCollisionDuMaillage.GetCorners();
            Matrix mondeLocal = GetMonde();
            Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);// je transforme les 8 coins de la boîte par la matrice Monde "Object oriented Bounding Box"
            boîteDeCollisionDuMaillage = BoundingBox.CreateFromPoints(listeDesCoins);
            if (boîteDeCollisionDuMaillage.Intersects(boîteCollision))
            {
               collision = true;
               break;
            }
         }
         return collision;
      }

      private void CréerListeDesBôites()
      {
         ListeDesBoîtes = new List<BoundingBox>();
         foreach (ModelMesh maillage in Modèle.Meshes)
         {
            BoundingBox boîteDeCollision = CalculerBoundingBox(maillage); //Création d'une boîte de collision "Axis Aligned Bounding Box"
            BoîteCollision boîte = new BoîteCollision(Jeu, this, boîteDeCollision, Color.Blue);
            boîte.Visible = false;
            Jeu.Components.Add(boîte);
            ListeDesBoîtes.Add(boîteDeCollision);
         }
      }

      private BoundingBox CalculerBoundingBox(ModelMesh maillage)
      {
         int tailleBuffer = maillage.VertexBuffer.SizeInBytes; // Taille du liste des sommets (en byte)
         int tailleSommet = VertexPositionNormalTexture.SizeInBytes; // Taille d'un sommet (en byte)
         int nbSommets = tailleBuffer / tailleSommet;
         VertexPositionNormalTexture[] SommetsDuMaillage = new VertexPositionNormalTexture[nbSommets];
         maillage.VertexBuffer.GetData<VertexPositionNormalTexture>(SommetsDuMaillage);

         Vector3[] positionSommets = new Vector3[SommetsDuMaillage.Length];

         for (int index = 0; index < positionSommets.Length; index++)
         {
            positionSommets[index] = SommetsDuMaillage[index].Position;
         }

         return BoundingBox.CreateFromPoints(positionSommets);
      }
   }
}
