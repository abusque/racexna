using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
   public class ModelDisplay : Microsoft.Xna.Framework.DrawableGameComponent
   {
      RacingGame RaceGame { get; set; }
      public BasicEffect Effect3D { get; private set; }

      bool isWireframe_;
      bool IsWireframe
      {
         get { return isWireframe_; }
         set
         {
            isWireframe_ = value;
            if (isWireframe_)
            {
               RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            }
            else
            {
               RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            }
         }
      }

      public ModelDisplay(RacingGame raceGame)
         : base(raceGame)
      {
         RaceGame = raceGame;
      }

      public override void Initialize()
      {
         Effect3D = new BasicEffect(RaceGame.GraphicsDevice, null);
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         HandleInput();
         base.Update(gameTime);
      }
     

      public override void Draw(GameTime gameTime)
      {
         RaceGame.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
         RaceGame.GraphicsDevice.RenderState.DepthBufferEnable = true;
         Effect3D.View = RaceGame.CaméraJeu.Vue;
         Effect3D.Projection = RaceGame.CaméraJeu.Projection;
         //Effet3D.EnableDefaultLighting();
         base.Draw(gameTime);
      }

      private void HandleInput()
      {
         if (RaceGame.InputMgr.EstNouvelleTouche(Keys.F))
         {
            IsWireframe = !IsWireframe;
         }
      }
   }
}