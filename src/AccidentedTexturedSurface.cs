using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class AccidentedTexturedSurface : BasePrimitive
    {
        const float CIRCLE_IN_DEGREES = 360;
        const float WAVELENGTH = 16;

        const int SEED = 5;
        const float MARGIN = 1.0f;
        const float MAX_DIST = 3.0f;

        VertexPositionTexture[] Vertices { get; set; }
        protected Vector3[,] VerticesPoints { get; set; }
        protected Vector3 Origin { get; set; }
        Vector2 Size { get; set; }
        Vector2 Dimension { get; set; }
        protected Vector2 Delta { get; set; }
        protected int ColumnsNb { get; set; }
        protected int RowsNb { get; set; }
        int PointsNb { get; set; }
        int TrianglesPerStrip { get; set; }
        protected String TextureName { get; private set; }
        Texture2D Texture { get; set; }
        Vector2[,] TexturePts { get; set; }
        Vector2[,] TextutreRepeatedPts { get; set; }
        Vector2 DeltaTexture { get; set; }
        bool IsTextureRepeated { get; set; }

        float CurrentHeight { get; set; }
        float MaxHeight { get; set; }
        float MinHeight { get; set; }
        Random RandomGenerator { get; set; }

        public AccidentedTexturedSurface(RacingGame raceGame, Vector3 origin, Vector2 size, Vector2 dimension, String textureName, bool isTextureRepeated)
            : base(raceGame)
        {
            Origin = origin;
            Size = size;
            Dimension = dimension;
            TextureName = textureName;
            Texture = RaceGame.TextureMgr.Find(TextureName);
            ColumnsNb = (int)Dimension.X;
            RowsNb = (int)Dimension.Y;
            DeltaTexture = new Vector2(1.0f / ColumnsNb, 1.0f / RowsNb);
            IsTextureRepeated = isTextureRepeated;
        }

        public override void Initialize()
        {
            PointsNb = (ColumnsNb + 1) * (RowsNb + 1);
            TrianglesPerStrip = ColumnsNb * 2;
            NbVertices = (TrianglesPerStrip + 2) * RowsNb;
            Delta = new Vector2(Size.X / ColumnsNb, Size.Y / RowsNb);
            VerticesPoints = new Vector3[ColumnsNb + 1, RowsNb + 1];
            TexturePts = new Vector2[ColumnsNb + 1, RowsNb + 1];
            TextutreRepeatedPts = new Vector2[2, 2];
            Vertices = new VertexPositionTexture[NbVertices];

            RandomGenerator = new Random(SEED);
            CurrentHeight = Origin.Y;
            MaxHeight = CurrentHeight + MAX_DIST;
            MinHeight = CurrentHeight - MAX_DIST;

            CreatePointsArray();
            InitializeVertices();
            //TestModifier();
            SetHeights();

            base.Initialize();
        }

        private void CreatePointsArray()
        {
            for (int i = 0; i <= ColumnsNb; ++i)
            {
                for (int j = 0; j <= RowsNb; ++j)
                {
                    VerticesPoints[i, j] = new Vector3(Origin.X + (i * Delta.X), Origin.Y, Origin.Z + (j * Delta.Y));
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
                    if (RandomGenerator.Next(-2,3) > 0)
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
                    VerticesPoints[i, j] = new Vector3(VerticesPoints[i, j].X, CurrentHeight, VerticesPoints[i, j].Z);
                }
            }
        }

        private void TestModifier()
        {
            VerticesPoints[0, 0] = new Vector3(VerticesPoints[0, 0].X, VerticesPoints[0, 0].Y + 10, VerticesPoints[0, 0].Z);
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