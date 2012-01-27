using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
   public class BaseObject : Microsoft.Xna.Framework.DrawableGameComponent
   {
      protected RacingGame RaceGame { get; private set; }
      private string ModelName { get; set; }
      protected Model ModelData { get; private set; }
      protected Matrix World;
      public Vector3 Position { get; protected set; }
      public float Scale { get; protected set; }
      public Vector3 Rotation { get; protected set; }
      float angle_;
      public float Angle
      {
         get
         {
            if (RaceGame.GestionFPS.ValFPS > 0)
            {
               angle_ += (2 * MathHelper.Pi) / (RaceGame.GestionFPS.ValFPS * 4);
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

      public BaseObject(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
         : base(jeu)
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

      public override void Initialize()
      {
         ModelData = RaceGame.ModelsMgr.Find(ModelName);
         Angle = 0;
         Pause = true;
         //this.Enabled = false;
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         HandleInput();
         if (!Pause)
         {
            World = Matrix.Identity * Matrix.CreateScale(Scale);
            World *= Matrix.CreateFromYawPitchRoll(Angle, Rotation.X, Rotation.Z);
            World *= Matrix.CreateTranslation(Position);
         }
         base.Update(gameTime);
      }

      private void HandleInput()
      {
         if (RaceGame.InputMgr.EstNouvelleTouche(Keys.Space))
         {
            Pause = !Pause;
         }
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
            foreach (ModelMeshPart portionDeMaillage in mesh.MeshParts)
            {
               BasicEffect effect = (BasicEffect)portionDeMaillage.Effect;
               effect.EnableDefaultLighting();
               effect.Projection = RaceGame.CaméraJeu.Projection;
               effect.View = RaceGame.CaméraJeu.Vue;
               effect.World = localWorld;
            }
            mesh.Draw();
         }
         base.Draw(gameTime);
      }

      public virtual Matrix GetWorld()
      {
         return World;
      }
   }
}
