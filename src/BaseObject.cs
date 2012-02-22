using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
   public class BaseObject : Microsoft.Xna.Framework.DrawableGameComponent
   {
       const float RADIUS = 2f;
       const float RADIUS_MODIFICATOR = 0.65f;
       public BoundingSphere TestSphere { get; private set; }

       public RacingGame RaceGame { get; private set; }
       private string ModelName { get; set; }
       public Model ModelData { get; private set; }
       protected Matrix World;
       public Vector3 Position { get; protected set; }
       public float Scale { get; protected set; }
       public Vector3 Rotation { get; protected set; }
       protected BoundingSphere[] Spheres { get; set; }
       public BoundingSphere BigSphere { get; private set; }
       protected BoundingBox[] Boxes { get; set; }
       public BoundingBox BigBox { get; private set; }


       public BaseObject(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
          : base(raceGame)
       {
          RaceGame = raceGame;
          ModelName = modelName;
          Position = initPos;
          Scale = initScale;
          Rotation = initRot;

          World = Matrix.Identity * Matrix.CreateScale(Scale);
          World *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
          World.Translation = Position;

          TestSphere = new BoundingSphere(Position, RADIUS);
       }

      private void CreateSpheres()
      {
         Spheres = new BoundingSphere[ModelData.Meshes.Count];
         float radius;
         Vector3 center;
         Matrix[] transformations = new Matrix[ModelData.Bones.Count];
         ModelData.CopyAbsoluteBoneTransformsTo(transformations);
         Matrix localTrans;

         for (int i = 0; i < Spheres.Length; ++i)
         {
            localTrans =  transformations[ModelData.Meshes[i].ParentBone.Index] * World;
            center = Vector3.Transform(ModelData.Meshes[i].BoundingSphere.Center, localTrans);
            radius = Scale * RADIUS_MODIFICATOR * ModelData.Meshes[i].BoundingSphere.Radius;
            Spheres[i] = new BoundingSphere(center, radius);
         }

         CreateBigSphere();
      }

      protected void CreateBigSphere()
      {
          BigSphere = BoundingSphere.CreateMerged(Spheres[0], Spheres[1]);
          for (int i = 2; i < Spheres.Length; ++i)
          {
              BigSphere = BoundingSphere.CreateMerged(BigSphere, Spheres[i]);
          }
      }

      private void CreateBoxes()
      {
          Boxes = new BoundingBox[ModelData.Meshes.Count];
          for (int i = 0; i < Boxes.Length; ++i)
          {
              Boxes[i] = BoundingBox.CreateFromSphere(Spheres[i]);
          }
          BigBox = BoundingBox.CreateMerged(Boxes[0], Boxes[1]);
          for (int i = 2; i < Boxes.Length; ++i)
          {
              BigBox = BoundingBox.CreateMerged(BigBox, Boxes[i]);
          }
      }

      public BoundingSphere GetSphere(int i)
      {
         return Spheres[i];
      }

      public BoundingBox GetBox(int i)
      {
          return Boxes[i];
      }

      public override void Initialize()
      {
         ModelData = RaceGame.ModelMgr.Find(ModelName);
         CreateSpheres();

         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         World = Matrix.Identity * Matrix.CreateScale(Scale);
         World *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
         World *= Matrix.CreateTranslation(Position);

         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         //Jeu.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
         // Au cas où le modèle se composerait de plusieurs morceaux
         Matrix[] transformations = new Matrix[ModelData.Bones.Count];
         ModelData.CopyAbsoluteBoneTransformsTo(transformations);

         foreach (ModelMesh mesh in ModelData.Meshes)
         {
            Matrix localWorld = transformations[mesh.ParentBone.Index] * GetWorld();
            foreach (ModelMeshPart meshPart in mesh.MeshParts)
            {
               BasicEffect effect = (BasicEffect)meshPart.Effect;
               effect.EnableDefaultLighting();
               effect.Projection = RaceGame.GameCamera.Projection;
               effect.View = RaceGame.GameCamera.View;
               effect.World = localWorld;
            }
            mesh.Draw();
         }
         base.Draw(gameTime);
      }

      //public override void Draw(GameTime gameTime)
      //{
      //   //Jeu.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
      //   // Au cas où le modèle se composerait de plusieurs morceaux
      //   Matrix[] transformations = new Matrix[Modèle.Bones.Count];
      //   Modèle.CopyAbsoluteBoneTransformsTo(transformations);

      //   foreach (ModelMesh maille in Modèle.Meshes)
      //   {
      //      Matrix mondeLocal = transformations[maille.ParentBone.Index] * GetMonde();
      //      foreach (BasicEffect effet in maille.Effects) //foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
      //      {
      //         effet.EnableDefaultLighting();
      //         effet.Projection = Jeu.CaméraJeu.Projection;
      //         effet.View = Jeu.CaméraJeu.Vue;
      //         effet.World = mondeLocal;
      //      }
      //      maille.Draw();
      //   }
      //   base.Draw(gameTime);
      //}

      public virtual Matrix GetWorld()
      {
         return World;
      }
   }
}
