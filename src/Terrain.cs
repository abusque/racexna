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
        int Length { get; set; }
        float[,] VerticesHeight { get; set; }
        VertexPositionNormalTexture[] Vertices { get; set; }
        int[] Indices { get; set; }
        Texture2D TerrainTexture { get; set; }
        Vector3[,] Normals { get; set; }
        float TerrainScale { get; set; }
        float HeightFactor { get; set; }

        public Terrain(RacingGame raceGame, Vector3 origin, string colorMapName, string heightMapName, float terrainScale, float heightFactor)
            : base(raceGame)
        {
            RaceGame = raceGame;
            Origin = origin;
            ColorMapName = colorMapName;
            HeightMapName = heightMapName;
            TerrainScale = terrainScale;
            HeightFactor = heightFactor;
        }

        public override void Initialize()
        {
            TerrainTexture = RaceGame.TextureMgr.Find(ColorMapName);
            ReadHeightMap();
            CreateVertices();
            CreateIndices();
            CalculateNormals();

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            RaceGame.ModelDisplayer.Draw(gameTime);

            RaceGame.GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            BasicEffect displayEffect = RaceGame.ModelDisplayer.Effect3D;
            displayEffect.World = Matrix.Identity;
            displayEffect.View = RaceGame.GameCamera.View;
            displayEffect.Projection = RaceGame.GameCamera.Projection;
            displayEffect.EnableDefaultLighting();
            displayEffect.TextureEnabled = true;
            displayEffect.Texture = TerrainTexture;
            displayEffect.Begin();

            foreach (EffectPass effectPass in displayEffect.CurrentTechnique.Passes)
            {
                effectPass.Begin();

                RaceGame.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length,
                    Indices, 0, Indices.Length / INDICES_PER_TRIANGLE);

                effectPass.End();
            }

            displayEffect.End();

            base.Draw(gameTime);
        }

        private void CreateVertices()
        {
            Vertices = new VertexPositionNormalTexture[Width * Length];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Length; ++j)
                {
                    Vertices[i + j * Width].Position = new Vector3(Origin.X + i * TerrainScale, Origin.Y + VerticesHeight[i, j], Origin.Z - j * TerrainScale);
                    Vertices[i + j * Width].TextureCoordinate.X = (float)i / Width;
                    Vertices[i + j * Width].TextureCoordinate.Y = (float)j / Length;
                }
            }
        }

        private void CreateIndices()
        {
            Indices = new int[(Width - 1) * (Length - 1) * INDICES_PER_TILE];

            int counter = 0;
            for (int j = 0; j < Length - 1; ++j)
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
            Length = heightMap.Height;

            Color[] pixelColors = new Color[Width * Length];
            heightMap.GetData(pixelColors);

            VerticesHeight = new float[Width, Length];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Length; ++j)
                {
                    VerticesHeight[i, j] = pixelColors[i + j * Width].R * HeightFactor;
                }
            }
        }

        private void CalculateNormals()
        {
            Normals = new Vector3[Width, Length];

            for (int i = 0; i < Vertices.Length; ++i)
            {
                Vertices[i].Normal = Vector3.Zero;
            }

            for (int i = 0; i < Indices.Length / INDICES_PER_TRIANGLE; ++i)
            {
                int index1 = Indices[i * INDICES_PER_TRIANGLE];
                int index2 = Indices[i * INDICES_PER_TRIANGLE + 1];
                int index3 = Indices[i * INDICES_PER_TRIANGLE + 2];

                Vector3 side1 = Vertices[index1].Position - Vertices[index3].Position;
                Vector3 side2 = Vertices[index1].Position - Vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                Vertices[index1].Normal += normal;
                Vertices[index2].Normal += normal;
                Vertices[index3].Normal += normal;
            }

            for (int i = 0; i < Vertices.Length; ++i)
            {
                Vertices[i].Normal.Normalize();
            }
        }

        public bool IsOnHeightmap(Vector3 carPos)
        {
            Vector3 relativePos = carPos - Origin;

            return (relativePos.X >= 0 && relativePos.X < Width * TerrainScale &&
                relativePos.Z <= 0 && relativePos.Z > (-Length + 1) * TerrainScale);
        }

        public void GetHeightAndNormal(Vector3 position, out float height, out Vector3 normal)
        {
            Vector3 relativePos = position - Origin;

            int left, top;
            left = (int)relativePos.X / (int)TerrainScale;
            top = -((int)relativePos.Z / (int)TerrainScale);

            float xNormalized = (relativePos.X % TerrainScale) / TerrainScale;
            float zNormalized = (-relativePos.Z % TerrainScale) / TerrainScale;

            float topHeight = MathHelper.Lerp(
                Vertices[left + top * Width].Position.Y,
                Vertices[(left + 1) + top * Width].Position.Y,
                xNormalized);

            float bottomHeight = MathHelper.Lerp(
                Vertices[left + (top + 1) * Width].Position.Y,
                Vertices[(left + 1) + (top + 1) * Width].Position.Y,
                xNormalized);

            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);

            Vector3 topNormal = Vector3.Lerp(
                Vertices[left + top * Width].Normal,
                Vertices[(left + 1) + top * Width].Normal,
                xNormalized);

            Vector3 bottomNormal = Vector3.Lerp(
                Vertices[left + (top + 1) * Width].Normal,
                Vertices[(left + 1) + (top + 1) * Width].Normal,
                xNormalized);

            normal = Vector3.Lerp(topNormal, bottomNormal, zNormalized);
            normal.Normalize();
        }

        public void GetHeight(Vector3 position, out float height)
        {
            Vector3 relativePos = position - Origin;

            int left, top;
            left = (int)relativePos.X / (int)TerrainScale;
            top = -((int)relativePos.Z / (int)TerrainScale);

            float xNormalized = (relativePos.X % TerrainScale) / TerrainScale;
            float zNormalized = (-relativePos.Z % TerrainScale) / TerrainScale;

            float topHeight = MathHelper.Lerp(
                Vertices[left + top * Width].Position.Y,
                Vertices[(left + 1) + top * Width].Position.Y,
                xNormalized);

            float bottomHeight = MathHelper.Lerp(
                Vertices[left + (top + 1) * Width].Position.Y,
                Vertices[(left + 1) + (top + 1) * Width].Position.Y,
                xNormalized);

            height = MathHelper.Lerp(topHeight, bottomHeight, zNormalized);
        }
    }
}