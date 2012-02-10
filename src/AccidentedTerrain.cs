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
    public class AccidentedTerrain : Terrain
    {
        const float MARGIN = 5f;
        const float MAX_MARGIN = 0.5f;
        const int SEED = 5;

        float CurrentHeight { get; set; }
        float MaxHeight { get; set; }
        float MinHeight { get; set; }
        Random RandomGenerator { get; set; }

        public AccidentedTerrain(RacingGame raceGame, Vector3 origin, Vector3 size, Vector2 dimension, bool isTextureRepeated, TerrainTypes terrainType)
            :base(raceGame, origin, size, dimension, isTextureRepeated, terrainType)
        {
            //Size = new Vector3(size.X, 0, size.Z);
            RandomGenerator = new Random(SEED);
            MaxHeight = Origin.Y + MAX_MARGIN;
            MinHeight = Origin.Y - MAX_MARGIN;
            CurrentHeight = Origin.Y;
            
        }

        public override void Initialize()
        {
            PointsNb = (ColumnsNb + 1) * (RowsNb + 1);
            TrianglesPerStrip = ColumnsNb * 2;
            NbVertices = (TrianglesPerStrip + 2) * RowsNb;
            Delta = new Vector3(Size.X / ColumnsNb, Size.Y / RowsNb, Size.Z / RowsNb);
            VerticesPoints = new Vector3[ColumnsNb + 1, RowsNb + 1];
            TexturePts = new Vector2[ColumnsNb + 1, RowsNb + 1];
            TextutreRepeatedPts = new Vector2[2, 2];
            Vertices = new VertexPositionTexture[NbVertices];

            CreatePointsArrayX();
            SetHeights();
            InitializeVertices();

            base.Initialize();
        }

        protected void CreatePointsArrayX()
        {

            for (int i = 0; i <= ColumnsNb; ++i)
            {
                for (int j = 0; j <= RowsNb; ++j)
                {
                    VerticesPoints[i, j] = new Vector3(Origin.X + (i * Delta.X), Origin.Y + (j * Delta.Y), Origin.Z + (j * Delta.Z));
                    TexturePts[i, j] = new Vector2(i * DeltaTexture.X, 1 - j * DeltaTexture.Y);
                }
            }

            for (int i = 0; i <= 1; ++i)
            {
                for (int j = 0; j <= 1; ++j)
                {
                    TextutreRepeatedPts[i, j] = new Vector2(i, j);
                }
            }
        }

        protected override void InitializeVertices()
        {
            int VertexNb = -1;

            for (int j = 0; j < RowsNb; ++j)
            {
                if (IsTextureRepeated)
                {
                    for (int i = 0; i <= ColumnsNb; ++i)
                    {
                        Vertices[++VertexNb] = new VertexPositionTexture(VerticesPoints[i, j], TextutreRepeatedPts[i % 2, j % 2]);
                        Vertices[++VertexNb] = new VertexPositionTexture(VerticesPoints[i, j + 1], TextutreRepeatedPts[i % 2, (j + 1) % 2]);
                    }
                }
                else
                {
                    for (int i = 0; i <= ColumnsNb; ++i)
                    {
                        Vertices[++VertexNb] = new VertexPositionTexture(VerticesPoints[i, j], TexturePts[i, j]);
                        Vertices[++VertexNb] = new VertexPositionTexture(VerticesPoints[i, j + 1], TexturePts[i, j + 1]);
                    }
                }
            }

        }

        private void SetHeights()
        {
            for (int i = 0; i < ColumnsNb; ++i)
            {
                for (int j = 0; j < RowsNb; ++j)
                {
                    if (RandomGenerator.Next() > 0)
                    {
                        CurrentHeight += (float)RandomGenerator.NextDouble() * MARGIN;
                    }
                    else
                    {
                        CurrentHeight -= (float)RandomGenerator.NextDouble() * MARGIN;
                    }
                    if (CurrentHeight >= MaxHeight)
                    {
                        CurrentHeight = MaxHeight;
                    }
                    else if (CurrentHeight <= MinHeight)
                    {
                        CurrentHeight = MinHeight;
                    }
                    VerticesPoints[i,j] = new Vector3(VerticesPoints[i, j].X, 2, VerticesPoints[i, j].Z);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            RaceGame.GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);
            RaceGame.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            BasicEffect displayEffect = RaceGame.ModelDisplayer.Effect3D;
            displayEffect.World = World;
            displayEffect.View = RaceGame.GameCamera.View;
            displayEffect.Projection = RaceGame.GameCamera.Projection;
            displayEffect.TextureEnabled = true;
            displayEffect.Texture = Texture;
            displayEffect.Begin();

            foreach (EffectPass effectPass in displayEffect.CurrentTechnique.Passes)
            {
                effectPass.Begin();

                for (int noStrip = 0; noStrip < RowsNb; ++noStrip)
                {
                    RaceGame.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Vertices, (TrianglesPerStrip + 2) * noStrip, TrianglesPerStrip);
                }

                effectPass.End();
            }

            displayEffect.End();
            displayEffect.TextureEnabled = false;

            base.Draw(gameTime);
        }
    }
}