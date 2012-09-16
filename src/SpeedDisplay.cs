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

    public class SpeedDisplay : Microsoft.Xna.Framework.DrawableGameComponent
    {
        RacingGame RaceGame { get; set; }
        Vector2 BottomRightPosition { get; set; }
        Vector2 StringPosition { get; set; }
        string StringSpeed { get; set; }
        Vector2 Dimention { get; set; }
        SpriteFont FontDisplay { get; set; }
        float SpeedValue { get; set; }
        string FontName { get; set; }
        float BottomMargin { get; set; }
        float RightMargin { get; set; }

        public SpeedDisplay(RacingGame game, string fontName)
            : base(game)
        {
            RaceGame = game;
            FontName = fontName;
        }

        public override void Initialize()
        {
            BottomMargin = RaceGame.Window.ClientBounds.Height - 100;
            RightMargin = RaceGame.Window.ClientBounds.Width - 80;

            BottomRightPosition = new Vector2(RaceGame.Window.ClientBounds.Width - RightMargin,
                                              RaceGame.Window.ClientBounds.Height - BottomMargin);
            SpeedValue = -1;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            FontDisplay = RaceGame.FontMgr.Find(FontName);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (RaceGame.HeadsUpDisplay.IsProgramerDisplay)
            {
                if (RaceGame.Car.Speed != SpeedValue)
                {
                    StringSpeed = RaceGame.Car.Speed.ToString("0");
                    Dimention = FontDisplay.MeasureString(StringSpeed);
                    StringPosition = BottomRightPosition - Dimention;
                    SpeedValue = RaceGame.Car.Speed;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (RaceGame.HeadsUpDisplay.IsProgramerDisplay)
            {
                FillMode previousFillMode = RaceGame.GraphicsDevice.RenderState.FillMode;
                RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                RaceGame.spriteBatch.DrawString(FontDisplay, StringSpeed, StringPosition, Color.Tomato, 0,
                                             Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                RaceGame.GraphicsDevice.RenderState.FillMode = previousFillMode;
            }
            base.Draw(gameTime);
        }
    }
}