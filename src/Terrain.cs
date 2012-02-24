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
        const int INDICES_PER_TILE = 6;
        const int INDICES_PER_TRIANGLE = 3;

        RacingGame RaceGame { get; set; }
        Vector3 Origin { get; set; }
        string ColorMapName { get; set; }
        string HeightMapName { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        float[,] VerticesHeight { get; set; }
        VertexPositionColor[] Vertices { get; set; }
        int[] Indices { get; set; }

        public Terrain(RacingGame raceGame, Vector3 origin, string colorMapName, string heightMapName)
            : base(raceGame)
        {
            RaceGame = raceGame;
            Origin = origin;
            ColorMapName = colorMapName;
            HeightMapName = heightMapName;
        }

        public override void Initialize()
        {
            ReadHeightMap();
            CreateVertices();
            CreateIndices();

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            RaceGame.GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
            RaceGame.GraphicsDevice.RenderState.CullMode = CullMode.None;
            RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

            BasicEffect displayEffect = RaceGame.ModelDisplayer.Effect3D;
            displayEffect.World = Matrix.Identity;
            displayEffect.View = RaceGame.GameCamera.View;
            displayEffect.Projection = RaceGame.GameCamera.Projection;
            displayEffect.Begin();

            foreach (EffectPass effectPass in displayEffect.CurrentTechnique.Passes)
            {
                effectPass.Begin();

                RaceGame.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length,
                    Indices, 0, Indices.Length / INDICES_PER_TRIANGLE);

                effectPass.End();
            }

            displayEffect.End();

            RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            base.Draw(gameTime);
        }

        private void CreateVertices()
        {
            Vertices = new VertexPositionColor[Width * Height];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    Vertices[i + j * Width].Position = new Vector3(Origin.X + i, Origin.Y + VerticesHeight[i, j], Origin.Z - j);
                    Vertices[i + j * Width].Color = Color.White;
                }
            }
        }

        private void CreateIndices()
        {
            Indices = new int[(Width - 1) * (Height - 1) * INDICES_PER_TILE];

            int counter = 0;
            for (int j = 0; j < Height - 1; ++j)
            {
                for (int i = 0; i < Width - 1; ++i)
                {
                    int lowerLeft = i + j * Width;
                    int lowerRight = (i + 1) + j * Width;
                    int topLeft = i + (j + 1) * Width;
                    int topRight = (i + 1) + (j + 1) * Width;

                    Indices[counter++] = topLeft;
                    Indices[counter++] = lowerRight;
                    Indices[counter++] = lowerLeft;

                    Indices[counter++] = topLeft;
                    Indices[counter++] = topRight;
                    Indices[counter++] = lowerRight;
                }
            }
        }

        private void ReadHeightMap()
        {
            Texture2D heightMap = RaceGame.TextureMgr.Find(HeightMapName);

            Width = heightMap.Width;
            Height = heightMap.Height;

            Color[] pixelColors = new Color[Width * Height];
            heightMap.GetData(pixelColors);

            VerticesHeight = new float[Width, Height];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    VerticesHeight[i, j] = pixelColors[i + j * Width].R / 5.0f; //On divise la valeur du rouge par 5 pour la ramener entre 0 et 51
                }
            }
        }
    }
}