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
        private RacingGame RaceGame;
        private Vector2 Dimension { get; set; }
        private Vector3 Origin { get; set; }
        private Vector2 Size { get; set; }
        private Vector2 TileSize { get; set; }
        private TerrainTile[,] TerrainTiles { get; set; }
        private Texture2D MapTexture { get; set; }

        public Terrain(RacingGame raceGame, Vector3 origin, Texture2D mapTexture)
            : base(raceGame)
        {
            RaceGame = raceGame;
            Origin = origin;
            MapTexture = mapTexture;
            Size = new Vector2(MapTexture.Width, MapTexture.Height);
            Dimension = new Vector2(MapTexture.Width, MapTexture.Height);
            TileSize = new Vector2(Size.X / Dimension.X, Size.Y / Dimension.Y);
        }

        public override void Initialize()
        {
            TerrainTiles = new TerrainTile[(int)Dimension.X, (int)Dimension.Y];
            CreateFromTexture();

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Size.X; ++i)
            {
                for (int j = 0; j < Size.Y; ++j)
                {
                    TerrainTiles[i, j].Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        private void CreateFromTexture()
        {
            Color[][] pixelColors = new Color[MapTexture.Width][];
            for(int i = 0; i < MapTexture.Width; ++i)
                pixelColors[i] = new Color[MapTexture.Height];
            Rectangle sourceRectangle;

            for (int i = 0; i < MapTexture.Width; ++i)
            {
                for (int j = 0; j < MapTexture.Height; ++j)
                {
                    sourceRectangle = new Rectangle(i, j, 1, 1);
                    MapTexture.GetData<Color>(0, sourceRectangle, pixelColors[i], j, 1);
                }
            }

            for (int i = 0; i < MapTexture.Width; ++i)
            {
                for (int j = 0; j < MapTexture.Height; ++j)
                {
                    if(pixelColors[i][j] == Color.Black)
                    {
                        TerrainTiles[i, j] = new TerrainTile(RaceGame, new Vector3(Origin.X + i * TileSize.X, Origin.Y + 0, Origin.Z + j * TileSize.Y),
                            new Vector2(TileSize.X, TileSize.Y), TerrainTile.TerrainTypes.Asphalt);
                    }
                    else if(pixelColors[i][j] == Color.White)
                    {
                        TerrainTiles[i, j] = new TerrainTile(RaceGame, new Vector3(Origin.X + i * TileSize.X, Origin.Y + 0, Origin.Z + j * TileSize.Y),
                            new Vector2(TileSize.X, TileSize.Y), TerrainTile.TerrainTypes.Sand);
                    }
                    else if(pixelColors[i][j] == Color.Green)
                    {
                        TerrainTiles[i, j] = new TerrainTile(RaceGame, new Vector3(Origin.X + i * TileSize.X, Origin.Y + 0, Origin.Z + j * TileSize.Y),
                            new Vector2(TileSize.X, TileSize.Y), TerrainTile.TerrainTypes.Grass);
                    }
                    else
                    {
                        TerrainTiles[i, j] = new TerrainTile(RaceGame, new Vector3(Origin.X + i * TileSize.X, Origin.Y + 0, Origin.Z + j * TileSize.Y),
                            new Vector2(TileSize.X, TileSize.Y), TerrainTile.TerrainTypes.Asphalt);
                    }

                    TerrainTiles[i, j].Initialize();
                }
            }
        }
    }
}