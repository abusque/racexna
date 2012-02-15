using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
   public class BaseObject : Microsoft.Xna.Framework.DrawableGameComponent
   {
       public RacingGame RaceGame { get; private set; }
       private string ModelName { get; set; }
       public Model ModelData { get; private set; }
       protected Matrix World;
       public Vector3 Position { get; protected set; }
       public float Scale { get; protected set; }
       public Vector3 Rotation { get; protected set; }
       private BoundingSphere[] Spheres { get; set; }
       private BoundingBox[] Boxes { get; set; }


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
            radius = Scale * ModelData.Meshes[i].BoundingSphere.Radius;
            Spheres[i] = new BoundingSphere(center, radius);
         }
      }

      private void CreateBoxes()
      {
          Boxes = new BoundingBox[ModelData.Meshes.Count];
          for (int i = 0; i < Boxes.Length; ++i)
          {
              Boxes = BoundingBox.CreateFromSphere(Spheres[i]);
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
         // Au cas o� le mod�le se composerait de plusieurs morceaux
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
      //   // Au cas o� le mod�le se composerait de plusieurs morceaux
      //   Matrix[] transformations = new Matrix[Mod�le.Bones.Count];
      //   Mod�le.CopyAbsoluteBoneTransformsTo(transformations);

      //   foreach (ModelMesh maille in Mod�le.Meshes)
      //   {
      //      Matrix mondeLocal = transformations[maille.ParentBone.Index] * GetMonde();
      //      foreach (BasicEffect effet in maille.Effects) //foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
      //      {
      //         effet.EnableDefaultLighting();
      //         effet.Projection = Jeu.Cam�raJeu.Projection;
      //         effet.View = Jeu.Cam�raJeu.Vue;
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
