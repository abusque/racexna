using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaceXNA
{
   public class TexturedSurface : BasePrimitive
   {
      const float CIRCLE_IN_DEGREES = 360;
      const float WAVELENGTH = 16;
      VertexPositionTexture[] Vertices { get; set; }
      protected Vector3[,] VerticesPoints { get; set; }
      protected Vector3 Origine { get; set; }
      Vector2 Size { get; set; }
      Vector2 Dimension { get; set; }
      protected Vector2 Delta { get; set; }
      protected int ColumnsNb { get; set; }
      protected int RowsNb { get; set; }
      int PointsNb { get; set; }
      int TrianglesNbPerStrip { get; set; }
      String TextureName { get; set; }
      Texture2D ImageTexture { get; set; }
      Vector2[,] TexturePts { get; set; }
      Vector2[,] TextutreRepeatedPts { get; set; }
      Vector2 DeltaTexture { get; set; }
      bool IsTextureRepeated { get; set; }
      
      
      public TexturedSurface(RacingGame raceGame, Vector3 origine, Vector2 size, Vector2 dimension, String textureName, bool isTextureRepeated)
         : base(raceGame)
      {
         Origine = origine;
         Size = size;
         Dimension = dimension;
         TextureName = textureName;
         ImageTexture = RaceGame.TextureMgr.Find(TextureName);
         ColumnsNb = (int)Dimension.X;
         RowsNb = (int)Dimension.Y;
         DeltaTexture = new Vector2(1.0f / ColumnsNb, 1.0f / RowsNb);
         IsTextureRepeated = isTextureRepeated;
      }

      public override void Initialize()
      {
         PointsNb = (ColumnsNb + 1) * (RowsNb + 1);
         TrianglesNbPerStrip = ColumnsNb * 2;
         NbVertices = (TrianglesNbPerStrip + 2) * RowsNb;
         Delta = new Vector2(Size.X / ColumnsNb, Size.Y / RowsNb);

         VerticesPoints = new Vector3[ColumnsNb + 1, RowsNb + 1];
         TexturePts = new Vector2[ColumnsNb + 1, RowsNb + 1];
         TextutreRepeatedPts = new Vector2[2, 2];
         Vertices = new VertexPositionTexture[NbVertices];
         
         CréerTableauPoints();
         InitializeVertices();

         base.Initialize();
      }

      private void CréerTableauPoints()
      {
         for (int i = 0; i <= ColumnsNb; ++i)
         {
            for (int j = 0; j <= RowsNb; ++j)
            {
               VerticesPoints[i, j] = new Vector3(Origine.X + (i * Delta.X), Origine.Y + (j * Delta.Y), Origine.Z);
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
         int NoSommet = -1;
         for (int j = 0; j < RowsNb; ++j)
         {
            if (IsTextureRepeated)
            {
                for (int i = 0; i <= ColumnsNb; ++i)
                {
                    Vertices[++NoSommet] = new VertexPositionTexture(VerticesPoints[i, j], TextutreRepeatedPts[i % 2, j % 2]);
                    Vertices[++NoSommet] = new VertexPositionTexture(VerticesPoints[i, j + 1], TextutreRepeatedPts[(i+1) % 2, (j+1) % 2]);
                }
            }
            else
            {
                for (int i = 0; i <= ColumnsNb; ++i)
                {
                    Vertices[++NoSommet] = new VertexPositionTexture(VerticesPoints[i, j], TexturePts[i, j]);
                    Vertices[++NoSommet] = new VertexPositionTexture(VerticesPoints[i, j + 1], TexturePts[i, j + 1]);
                }
            }
         }
      }

      public override void Draw(GameTime gameTime)
      {
         RaceGame.GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);
         RaceGame.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
         //BasicEffect effetAffichage = new BasicEffect(Jeu.GraphicsDevice, null);
         BasicEffect displayEffect = RaceGame.ModelDisplayer.Effect3D;
         displayEffect.World = World;
         displayEffect.View = RaceGame.GameCamera.View;
         displayEffect.Projection = RaceGame.GameCamera.Projection;
         displayEffect.TextureEnabled = true;
         displayEffect.Texture = ImageTexture;
         displayEffect.Begin();
         foreach (EffectPass effectPass in displayEffect.CurrentTechnique.Passes)
         {
            effectPass.Begin();
            for (int noStrip = 0; noStrip < RowsNb; ++noStrip)
            {
               RaceGame.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Vertices, (TrianglesNbPerStrip + 2) * noStrip, TrianglesNbPerStrip);
            }
            effectPass.End();
         }
         displayEffect.End();
         displayEffect.TextureEnabled = false;
         base.Draw(gameTime);
      }
   }
}
