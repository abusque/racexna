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
    public class FpsCounter : Microsoft.Xna.Framework.GameComponent
    {
        public float FpsValue { get; private set; }
        float Interval { get; set; }
        float ElapsedTime { get; set; }
        int Frames { get; set; }
        Game RaceGame { get; set; }

        public FpsCounter(Game game, float interval)
            : base(game)
        {
            RaceGame = game;
            Interval = interval;
        }

        public override void Initialize()
        {
            FpsValue = 6000;
            ElapsedTime = 0;
            Frames = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime;

            if (RaceGame.IsFixedTimeStep)
            {
                elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                elapsedTime = (float)gameTime.ElapsedRealTime.TotalSeconds;
            }

            ++Frames;
            ElapsedTime += elapsedTime;

            if (ElapsedTime > Interval)
            {
                FpsValue = Frames / ElapsedTime;
                Frames = 0;
                ElapsedTime -= Interval;
            }

            base.Update(gameTime);
        }
    }
}

