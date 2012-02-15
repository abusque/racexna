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
    public class Terrain : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector2 Dimension { get; set; }
        private Vector3 Origin { get; set; }
        private Vector2 Size { get; set; }
        private Vector2 TileSize { get; set; }
        private TerrainTile[,] TerrainTiles { get; set; }

        public Terrain(Game game, Vector3 origin, Vector2 size, Vector2 dimension)
            : base(game)
        {
            Origin = origin;
            Size = size;
            Dimension = dimension;
            TileSize = new Vector2(Size.X / Dimension.X, Size.Y / Dimension.Y);
        }

        public override void Initialize()
        {
            TerrainTiles = new TerrainTile[(int)Dimension.X, (int)Dimension.Y];

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (TerrainTile terrainTile in TerrainTiles)
            {
                terrainTile.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}