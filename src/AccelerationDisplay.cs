using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace RaceXNA
{

   public class AccelerationDisplay : Microsoft.Xna.Framework.DrawableGameComponent
   {
      const int BOTTOM_MARGIN = 10;
      const int RIGHT_MARGIN = 205;

      RacingGame RaceGame { get; set; }
      Vector2 BottomRightPosition { get; set; }
      Vector2 StringPosition { get; set; }
      string StringAcceleration { get; set; }
      Vector2 Dimention { get; set; }
      SpriteFont FontDisplay { get; set; }
      float AccelerationValue { get; set; }
      string FontName { get; set; }

      public AccelerationDisplay(RacingGame game, string fontName)
         : base(game)
      {
         RaceGame = game;
         FontName = fontName;
      }

      public override void Initialize()
      {
         BottomRightPosition = new Vector2(RaceGame.Window.ClientBounds.Width - RIGHT_MARGIN,
                                           RaceGame.Window.ClientBounds.Height - BOTTOM_MARGIN);
         AccelerationValue = -1;
         base.Initialize();
      }

      protected override void LoadContent()
      {
         FontDisplay = RaceGame.FontMgr.Find(FontName);
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
         if (RaceGame.Car.Acceleration != AccelerationValue)
         {
            StringAcceleration = RaceGame.Car.Acceleration.ToString("0");
            Dimention = FontDisplay.MeasureString(StringAcceleration);
            StringPosition = BottomRightPosition - Dimention;
            AccelerationValue = RaceGame.Car.Acceleration;
         }
         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         FillMode previousFillMode = RaceGame.GraphicsDevice.RenderState.FillMode;
         RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
         RaceGame.spriteBatch.DrawString(FontDisplay, StringAcceleration, StringPosition, Color.Tomato, 0,
                                      Vector2.Zero, 1.0f, SpriteEffects.None, 0);
         RaceGame.GraphicsDevice.RenderState.FillMode = previousFillMode;

         base.Draw(gameTime);
      }
   }
}