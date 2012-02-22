using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class Tile : BasePrimitive
    {
        const int COLS = 1;
        const int ROWS = 1;
        const int TRIANGLES = 2;
        const int NB_VERTICES = 4;

        VertexPositionTexture[] Vertices { get; set; }
        protected Vector3[,] VerticesPoints { get; set; }
        protected Vector3 Origin { get; set; }
        Vector2 Size { get; set; }
        Vector2 Dimension { get; set; }
        protected String TextureName { get; private set; }
        Texture2D TextureData { get; set; }
        Vector2[,] TexturePts { get; set; }

        public Tile(RacingGame raceGame, Vector3 origin, Vector2 size, String textureName)
         : base(raceGame)
       {
         Origin = origin;
         Size = size;
         TextureName = textureName;
         TextureData = RaceGame.TextureMgr.Find(TextureName);
       }

       public override void Initialize()
       {
           VerticesPoints = new Vector3[COLS + 1, ROWS + 1];
           TexturePts = new Vector2[COLS + 1, ROWS + 1];
           Vertices = new VertexPositionTexture[NB_VERTICES];

           CreatePointsArray();
           InitializeVertices();

           base.Initialize();
        }

       private void CreatePointsArray()
       {
           for (int i = 0; i <= COLS; ++i)
           {
               for (int j = 0; j <= ROWS; ++j)
               {
                   VerticesPoints[i, j] = new Vector3(Origin.X + i * Size.X, Origin.Y, Origin.Z + j * Size.Y);
                   TexturePts[i, j] = new Vector2(i, 1 - j);
               }
           }
       }

       protected override void InitializeVertices()
       {
           int VertexNb = -1;

           for (int j = 0; j < ROWS; ++j)
           {
               for (int i = 0; i <= COLS; ++i)
               {
                   Vertices[++VertexNb] = new VertexPositionTexture(VerticesPoints[i, j], TexturePts[i, j]);
                   Vertices[++VertexNb] = new VertexPositionTexture(VerticesPoints[i, j + 1], TexturePts[i, j + 1]);
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
           displayEffect.Texture = TextureData;
           displayEffect.Begin();

           foreach (EffectPass effectPass in displayEffect.CurrentTechnique.Passes)
           {
               effectPass.Begin();

               for (int stripNb = 0; stripNb < ROWS; ++stripNb)
               {
                   RaceGame.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Vertices, (TRIANGLES + 2) * stripNb, TRIANGLES);
               }

               effectPass.End();
           }

           displayEffect.End();
           displayEffect.TextureEnabled = false;

           base.Draw(gameTime);
       }
    }
}
