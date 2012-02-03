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
      public Model Model { get; private set; }
      protected Matrix World;
      public Vector3 Position { get; protected set; }
      public float Scale { get; protected set; }
      public Vector3 Rotation { get; protected set; }
      private BoundingSphere[] Spheres { get; set; }

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
         Spheres = new BoundingSphere[Model.Meshes.Count];
         float radius;
         Vector3 center;
         Matrix[] transformations = new Matrix[Model.Bones.Count];
         Model.CopyAbsoluteBoneTransformsTo(transformations);
         Matrix localTrans;

         for (int i = 0; i < Spheres.Length; ++i)
         {
            localTrans =  transformations[Model.Meshes[i].ParentBone.Index] * World;
            center = Vector3.Transform(Model.Meshes[i].BoundingSphere.Center, localTrans);
            radius = Scale * Model.Meshes[i].BoundingSphere.Radius;
            Spheres[i] = new BoundingSphere(center, radius);
         }
      }

      public BoundingSphere GetSphere(int i)
      {
         return Spheres[i];
      }

      public override void Initialize()
      {
         Model = RaceGame.ModelMgr.Find(ModelName);
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
         Matrix[] transformations = new Matrix[Model.Bones.Count];
         Model.CopyAbsoluteBoneTransformsTo(transformations);

         foreach (ModelMesh mesh in Model.Meshes)
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
