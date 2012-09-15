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
            string modelName = "tree stomp sculpture";
            float modelScale = 0.02f;
            Vector3 initRot = new Vector3(-MathHelper.PiOver2, 0, 0);
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(29, 0, -8) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(63, 0, -18) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(101, 0, -13) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(101, 0, -19) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(111, 0, -20) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(94, 0, -31) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(15, 0, -36) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(111, 0, -43) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(50, 0, -50) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(106, 0, -53) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(57, 0, -55) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(98, 0, -62) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(75, 0, -68) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(28, 0, -69) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(42, 0, -69) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(81, 0, -78) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(13, 0, -85) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(103, 0, -89) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(73, 0, -90) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(49, 0, -92) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(37, 0, -101) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(106, 0, -53) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(120, 0, -121) * Ground.TerrainScale, modelScale, initRot));
            Obstacles.Add(new BaseObject(RaceGame, modelName, Ground.Origin + new Vector3(24, 0, -122) * Ground.TerrainScale, modelScale, initRot));

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