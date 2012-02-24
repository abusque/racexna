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

    public class Odometer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int ODOMETER_LEFT_MARGIN = 0;
        const int ODOMETER_BOTTOM_MARGIN = 0;
        const int NEEDLE_LEFT_MARGIN = 0;
        const int NEEDLE_BOTTOM_MARGIN = 0;
        const float MIN_SPEED = 0;
        const float MAX_SPEED = 120;

        RacingGame RaceGame { get; set; }

        string OdometerTextureName { get; set; }
        Texture2D OdometerTexture { get; set; }
        int OdometerWidth { get; set; }
        int OdometerHeight { get; set; }
        Vector2 OdometerPos { get; set; }
        Rectangle OdometerRectangle { get; set; }

        int NeedleMapNbColumns { get; set; }
        int NeedleMapNbRows { get; set; }
        string NeedleMapTextureName { get; set; }
        Texture2D NeedleMapTexture { get; set; }
        int NeedleMapWidth { get; set; }
        int NeedleWidth { get; set; }
        int NeedleMapHeight { get; set; }
        int NeedleHeight { get; set; }
        Vector2 NeedlePos { get; set; }
        Rectangle NeedleRectangle { get; set; }
        Rectangle[] NeedleSourceRectangles { get; set; }

        int ArraysLenght { get; set; }
        float ActualSpeed { get; set; }
        float[] SpeedValues { get; set; }
        int ActualIndex { get; set; }

        public Odometer(RacingGame raceGame, string odometerTextureName, string needleMapTextureName, int needleMapNbColumns, int needleMapNbRows)
            : base(raceGame)
        {
            RaceGame = raceGame;
            OdometerTextureName = odometerTextureName;
            NeedleMapTextureName = needleMapTextureName;
            NeedleMapNbColumns = needleMapNbColumns;
            NeedleMapNbRows = needleMapNbRows;
        }

        public override void Initialize()
        {
            OdometerTexture = RaceGame.TextureMgr.Find(OdometerTextureName);
            OdometerWidth = OdometerTexture.Width;
            OdometerHeight = OdometerTexture.Height;
            OdometerPos = new Vector2(RaceGame.Window.ClientBounds.Width - ODOMETER_LEFT_MARGIN - OdometerWidth,
                                             RaceGame.Window.ClientBounds.Height - ODOMETER_BOTTOM_MARGIN - OdometerHeight);
            OdometerRectangle = new Rectangle((int)OdometerPos.X, (int)OdometerPos.Y, OdometerWidth, OdometerHeight);

            NeedleMapTexture = RaceGame.TextureMgr.Find(NeedleMapTextureName);
            NeedleMapWidth = NeedleMapTexture.Width;
            NeedleWidth = NeedleMapWidth / NeedleMapNbColumns;
            NeedleMapHeight = NeedleMapTexture.Height;
            NeedleHeight = NeedleMapHeight / NeedleMapNbRows;
            NeedlePos = new Vector2(RaceGame.Window.ClientBounds.Width - NEEDLE_LEFT_MARGIN - NeedleWidth,
                                    RaceGame.Window.ClientBounds.Height - NEEDLE_BOTTOM_MARGIN - NeedleHeight);
            NeedleRectangle = new Rectangle((int)NeedlePos.X, (int)NeedlePos.Y, NeedleWidth, NeedleHeight);
            InitializeArrays();
            base.Initialize();
        }

        public void InitializeArrays()
        {
            ArraysLenght = NeedleMapNbColumns * NeedleMapNbRows;
            NeedleSourceRectangles = new Rectangle[ArraysLenght];
            for (int i = 0; i < NeedleMapNbColumns; ++i)
            {
                for (int j = 0; j < NeedleMapNbRows; ++j)
                {
                    NeedleSourceRectangles[j+i*NeedleMapNbColumns] = new Rectangle(j * NeedleWidth,i *  NeedleHeight, NeedleWidth, NeedleHeight);
                }
            }

            SpeedValues = new float[ArraysLenght];
            for (int i = 0; i < ArraysLenght; ++i)
            {
                SpeedValues[i] = MIN_SPEED + (i + 1) * (MAX_SPEED / ArraysLenght);
            }


        }

        public override void Update(GameTime gameTime)
        {
            HandleOdometer();
            base.Update(gameTime);
        }

        private void HandleOdometer()
        {
            ActualSpeed = (float)Math.Abs(RaceGame.Car.Speed);
            float slightestDistance = MAX_SPEED;
            for (int i = 0; i < ArraysLenght; ++i)
            {
                if (Math.Abs(ActualSpeed - SpeedValues[i]) < slightestDistance)
                {
                    slightestDistance = Math.Abs(ActualSpeed - SpeedValues[i]);
                    ActualIndex = i;
                }
                
            }
        }

        public override void Draw(GameTime gameTime)
        {
            RaceGame.spriteBatch.Draw(OdometerTexture, OdometerRectangle, Color.White);
            RaceGame.spriteBatch.Draw(NeedleMapTexture, NeedleRectangle, NeedleSourceRectangles[ActualIndex], Color.White);
            base.Draw(gameTime);
        }
    }
}