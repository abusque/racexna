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

    public class Chronometer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Constants
        const int MILLISECONDS_RIGHT_MARGIN = 25;
        const int SECONDS_RIGHT_MARGIN = 60;
        const int MINUTES_RIGHT_MARGIN = 125;
        #endregion Constants

        #region Properties
        RacingGame RaceGame { get; set; }

        Vector2 MillisecondsBottomRightPosition { get; set; }
        Vector2 SecondsBottomRightPosition { get; set; }
        Vector2 MinutesBottomRightPosition { get; set; }

        Vector2 StringMilisecondsPosition { get; set; }
        Vector2 StringSecondsPosition { get; set; }
        Vector2 StringMinutesPosition { get; set; }

        string StringMilliseconds { get; set; }
        string StringSeconds { get; set; }
        string StringMinutes { get; set; }

        Vector2 DimensionMilliseconds { get; set; }
        Vector2 DimensionSeconds { get; set; }
        Vector2 DimensionMinutes { get; set; }

        SpriteFont FontDisplay { get; set; }
        string FontName { get; set; }
        Color DisplayColor { get; set; }

        float millisecondsValue;
        float MillisecondsValue
        {
            get
            {
                return millisecondsValue;
            }
            set
            {
                millisecondsValue = value;
                if (millisecondsValue >= 100)
                {
                    millisecondsValue -= 100;
                    ++SecondsValue;
                }
            }
        }

        float secondsValue;
        float SecondsValue
        {
            get
            {
                return secondsValue;
            }
            set
            {
                secondsValue = value;
                if (secondsValue >= 60)
                {
                    secondsValue -= 60;
                    ++MinutesValue;
                }

            }
        }

        float MinutesValue { get; set; }

        float BottomMargin { get; set; }
        #endregion Properties

        public Chronometer(RacingGame game, string fontName)
            : base(game)
        {
            RaceGame = game;
            FontName = fontName;
        }

        public override void Initialize()
        {
            BottomMargin = RaceGame.Window.ClientBounds.Height - 50;

            MillisecondsBottomRightPosition = new Vector2(RaceGame.Window.ClientBounds.Width - MILLISECONDS_RIGHT_MARGIN,
                                                         RaceGame.Window.ClientBounds.Height - BottomMargin);
            SecondsBottomRightPosition = new Vector2(RaceGame.Window.ClientBounds.Width - SECONDS_RIGHT_MARGIN,
                                                     RaceGame.Window.ClientBounds.Height - BottomMargin);
            MinutesBottomRightPosition = new Vector2(RaceGame.Window.ClientBounds.Width - MINUTES_RIGHT_MARGIN,
                                                     RaceGame.Window.ClientBounds.Height - BottomMargin);

            DisplayColor = Color.Chartreuse;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            FontDisplay = RaceGame.FontMgr.Find(FontName);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (RaceGame.FpsHandler.FpsValue != 0)
            {
                float temporarySecondsValue = (float)gameTime.ElapsedGameTime.TotalSeconds;
                MillisecondsValue += temporarySecondsValue * 100;

                StringMilliseconds = MillisecondsValue.ToString("00");
                DimensionMilliseconds = FontDisplay.MeasureString(StringMilliseconds);
                StringMilisecondsPosition = MillisecondsBottomRightPosition - DimensionMilliseconds;

                StringSeconds = SecondsValue.ToString("00") + " : ";
                DimensionSeconds = FontDisplay.MeasureString(StringSeconds);
                StringSecondsPosition = SecondsBottomRightPosition - DimensionSeconds;

                StringMinutes = MinutesValue.ToString("00") + " : ";
                DimensionMinutes = FontDisplay.MeasureString(StringMinutes);
                StringMinutesPosition = MinutesBottomRightPosition - DimensionMinutes;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            FillMode previousFillMode = RaceGame.GraphicsDevice.RenderState.FillMode;

            RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            RaceGame.spriteBatch.DrawString(FontDisplay, StringMilliseconds, StringMilisecondsPosition, DisplayColor, 0,
                                            Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            RaceGame.spriteBatch.DrawString(FontDisplay, StringSeconds, StringSecondsPosition, DisplayColor, 0,
                                            Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            RaceGame.spriteBatch.DrawString(FontDisplay, StringMinutes, StringMinutesPosition, DisplayColor, 0,
                                            Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            RaceGame.GraphicsDevice.RenderState.FillMode = previousFillMode;

            base.Draw(gameTime);
        }
    }
}