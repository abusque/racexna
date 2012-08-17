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
        Terrain Ground { get; set; }
        public List<BaseObject> Obstacles { get; private set; }

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
            Obstacles.Add(new BaseObject(RaceGame, "L200-FBX", new Vector3(30, 0, -60), 0.01f, Vector3.Zero));
            Obstacles.Add(new BaseObject(RaceGame, "L200-FBX", new Vector3(0, 0, -60), 0.01f, Vector3.Zero));
            //Obstacles.Add(new BaseObject(RaceGame, "L200-FBX", new Vector3(20, 0, -40), 0.01f, Vector3.Zero));
            //Obstacles.Add(new BaseObject(RaceGame, "L200-FBX", new Vector3(0, 0, -30), 0.01f, Vector3.Zero));
            //Obstacles.Add(new BaseObject(RaceGame, "L200-FBX", new Vector3(60, 0, -90), 0.01f, Vector3.Zero));
            for (int i = 0; i < Obstacles.Count; ++i)
            {
                RaceGame.Components.Add(Obstacles[i]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}