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
      private string NomMod�le { get; set; }
      protected Model Mod�le { get; private set; }
      protected Matrix Monde;
      public Vector3 Position { get; protected set; }
      public float �chelle { get; protected set; }
      public Vector3 Rotation { get; protected set; }

      List<BoundingBox> ListeDesBo�tes { get; set; }

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

      public ObjetDeBasePhysique(Atelier jeu, String nomMod�le, Vector3 positionInitiale, float �chelleInitiale, Vector3 rotationInitiale)
         : base(jeu)
      {
         Jeu = jeu;
         NomMod�le = nomMod�le;
         Position = positionInitiale;
         �chelle = �chelleInitiale;
         Rotation = rotationInitiale;
      }

      public override void Initialize()
      {
         Mod�le = Jeu.ModelsMgr.Find(NomMod�le);
         Angle = 0;
         Pause = true;
         Monde = Matrix.Identity * Matrix.CreateScale(�chelle);
         Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
         Monde.Translation = Position;
         Cr�erListeDesB�ites();
         VisualiserSph�reDeCollision();
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         G�rerClavier();
         if (!Pause)
         {
            Monde = Matrix.Identity * Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Angle, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
         }
         base.Update(gameTime);
      }

      private void G�rerClavier()
      {
         if (Jeu.InputMgr.EstNouvelleTouche(Keys.Space))
         {
            Pause = !Pause;
         }
      }

      public override void Draw(GameTime gameTime)
      {
         //Jeu.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
         // Au cas o� le mod�le se composerait de plusieurs morceaux
         Matrix[] transformations = new Matrix[Mod�le.Bones.Count];
         Mod�le.CopyAbsoluteBoneTransformsTo(transformations);
         foreach (ModelMesh maille in Mod�le.Meshes)
         {
            Matrix mondeLocal = transformations[maille.ParentBone.Index] * GetMonde();
            foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
            {
               BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
               effet.EnableDefaultLighting();
               effet.Projection = Jeu.Cam�raJeu.Projection;
               effet.View = Jeu.Cam�raJeu.Vue;
               effet.World = mondeLocal;
            }
            maille.Draw();
         }
         base.Draw(gameTime);
      }

      public void VisualiserSph�reDeCollision()
      {
         foreach (ModelMesh maille in Mod�le.Meshes)
         {
            BoundingSphere sph�reDeCollision = maille.BoundingSphere;
            sph�reDeCollision = sph�reDeCollision.Transform(Monde);
            Sph�reDeCollision Sph�re = new Sph�reDeCollision(Jeu, sph�reDeCollision.Center, sph�reDeCollision.Radius, 16, 16, "rouge");
            Sph�re.Visible = false;
            Jeu.Components.Add(Sph�re);
         }
      }

      public virtual Matrix GetMonde()
      {
         return Monde;
      }

      public bool EstEnCollision(BoundingSphere sph�reCollision)
      {
         bool collision = false;
         for (int i = 0; i < Mod�le.Meshes.Count; ++i)
         {
            BoundingSphere sph�reCollisionDuMaillage = Mod�le.Meshes[i].BoundingSphere;
            if (sph�reCollisionDuMaillage.Transform(Monde).Intersects(sph�reCollision))
            {
               collision = true;
               break;
            }
         }
         return collision;
      }

      public bool EstEnCollision(BoundingBox bo�teCollision)
      {
         bool collision = false;
         for (int i = 0; i < ListeDesBo�tes.Count; ++i)
         {
            BoundingBox bo�teDeCollisionDuMaillage = ListeDesBo�tes[i];
            Vector3[] listeDesCoins = bo�teDeCollisionDuMaillage.GetCorners();
            Matrix mondeLocal = GetMonde();
            Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);// je transforme les 8 coins de la bo�te par la matrice Monde "Object oriented Bounding Box"
            bo�teDeCollisionDuMaillage = BoundingBox.CreateFromPoints(listeDesCoins);
            if (bo�teDeCollisionDuMaillage.Intersects(bo�teCollision))
            {
               collision = true;
               break;
            }
         }
         return collision;
      }

      private void Cr�erListeDesB�ites()
      {
         ListeDesBo�tes = new List<BoundingBox>();
         foreach (ModelMesh maillage in Mod�le.Meshes)
         {
            BoundingBox bo�teDeCollision = CalculerBoundingBox(maillage); //Cr�ation d'une bo�te de collision "Axis Aligned Bounding Box"
            Bo�teCollision bo�te = new Bo�teCollision(Jeu, this, bo�teDeCollision, Color.Blue);
            bo�te.Visible = false;
            Jeu.Components.Add(bo�te);
            ListeDesBo�tes.Add(bo�teDeCollision);
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
