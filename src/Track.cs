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
    public class Track : Microsoft.Xna.Framework.GameComponent
    {
        RacingGame RaceGame { get; set; }
        public Terrain Ground { get; private set; }
        public List<BaseObject> Obstacles { get; private set; }
        bool HeightSet { get; set; }

        public Track(RacingGame game, Terrain ground)
            : base(game)
        {
            RaceGame = game;
            Ground = ground;
        }

        public override void Initialize()
        {
            Obstacles = new List<BaseObject>();
            RaceGame.Components.Add(Ground);

            SetObstacles();

            base.Initialize();
        }

        private void SetObstacles()
        {
            Obstacles.Add(new BaseObject(RaceGame, "L200-FBX", new Vector3(30, 0, -60), 0.01f, 0.0f));
            Obstacles.Add(new BaseObject(RaceGame, "L200-FBX", new Vector3(0, 0, -60), 0.01f, 0.0f));

            for (int i = 0; i < Obstacles.Count; ++i)
            {
                RaceGame.Components.Add(Obstacles[i]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!HeightSet)
            {
                foreach (BaseObject obstacle in Obstacles)
                    obstacle.SetInitialHeight(Ground);

                HeightSet = true;
            }

            base.Update(gameTime);
        }
    }
}