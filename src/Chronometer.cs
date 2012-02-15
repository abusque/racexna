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
       const int BOTTOM_MARGIN = 550;
       const int MILISECONDS_RIGHT_MARGIN = 25;
       const int SECONDS_RIGHT_MARGIN = 70;
       const int MINUTES_RIGHT_MARGIN = 155;

       RacingGame RaceGame { get; set; }
       Vector2 MilisecondsBottomRightPosition { get; set; }
       Vector2 SecondsBottomRightPosition { get; set; }
       Vector2 MinutesBottomRightPosition { get; set; }
       Vector2 StringMilisecondsPosition { get; set; }
       Vector2 StringSecondsPosition { get; set; }
       Vector2 StringMinutesPosition { get; set; }
       string StringMiliseconds { get; set; }
       string StringSeconds { get; set; }
       string StringMinutes { get; set; }
       Vector2 DimentionMiliseconds { get; set; }
       Vector2 DimentionSeconds { get; set; }
       Vector2 DimentionMinutes { get; set; }
       SpriteFont FontDisplay { get; set; }
       string FontName { get; set; }
       float milisecondsValue;
       float MilisecondsValue 
       {
           get
           {
               return milisecondsValue;
           }
           set
           {
               milisecondsValue = value;
               if (milisecondsValue >= 100)
               {
                   milisecondsValue -= 100;
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
       Color DisplayColor { get; set; }

       public Chronometer(RacingGame game, string fontName)
          : base(game)
       {
           RaceGame = game;
           FontName = fontName;
       }

       public override void Initialize()
       {
           MilisecondsBottomRightPosition = new Vector2(RaceGame.Window.ClientBounds.Width - MILISECONDS_RIGHT_MARGIN,
                                                        RaceGame.Window.ClientBounds.Height - BOTTOM_MARGIN);
           SecondsBottomRightPosition = new Vector2(RaceGame.Window.ClientBounds.Width - SECONDS_RIGHT_MARGIN,
                                                    RaceGame.Window.ClientBounds.Height - BOTTOM_MARGIN);
           MinutesBottomRightPosition = new Vector2(RaceGame.Window.ClientBounds.Width - MINUTES_RIGHT_MARGIN,
                                                    RaceGame.Window.ClientBounds.Height - BOTTOM_MARGIN);
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
               //float tempSecondsValue = (float)gameTime.ElapsedGameTime.TotalSeconds;
               float temporarySecondsValue = (float)gameTime.ElapsedGameTime.TotalSeconds;
               MilisecondsValue += temporarySecondsValue * 100;
               //MilisecondsValue += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               StringMiliseconds = MilisecondsValue.ToString("00");
               DimentionMiliseconds = FontDisplay.MeasureString(StringMiliseconds);
               StringMilisecondsPosition = MilisecondsBottomRightPosition - DimentionMiliseconds;

               StringSeconds = SecondsValue.ToString("00") + " : ";
               DimentionSeconds = FontDisplay.MeasureString(StringSeconds);
               StringSecondsPosition = SecondsBottomRightPosition - DimentionSeconds;

               StringMinutes = MinutesValue.ToString("00") + " : ";
               DimentionMinutes = FontDisplay.MeasureString(StringMinutes);
               StringMinutesPosition = MinutesBottomRightPosition - DimentionMinutes;
           }

           base.Update(gameTime);
       }

       public override void Draw(GameTime gameTime)
       {
           FillMode previousFillMode = RaceGame.GraphicsDevice.RenderState.FillMode;
           RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
           RaceGame.spriteBatch.DrawString(FontDisplay, StringMiliseconds, StringMilisecondsPosition, DisplayColor, 0,
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