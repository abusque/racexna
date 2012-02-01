using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaceXNA
{
   class TexturedSurface : BaseObject
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
      Vector2 DeltaTexture { get; set; }

      public TexturedSurface(Atelier jeu, Vector3 origine, Vector2 size, Vector2 dimension, String textureName)
         : base(jeu)
      {
         Origine = origine;
         Size = size;
         Dimension = dimension;
         TextureName = textureName;
         ImageTexture = Jeu.TexturesMgr.Find(TextureName);
         ColumnsNb = (int)Dimension.X;
         RowsNb = (int)Dimension.Y;
         DeltaTexture = new Vector2(1.0f / ColumnsNb, 1.0f / RowsNb);
      }

      public override void Initialize()
      {
         PointsNb = (ColumnsNb + 1) * (RowsNb + 1);
         TrianglesNbPerStrip = ColumnsNb * 2;
         NbSommets = (TrianglesNbPerStrip + 2) * RowsNb;
         Delta = new Vector2(Size.X / ColumnsNb, Size.Y / RowsNb);

         VerticesPoints = new Vector3[ColumnsNb + 1, RowsNb + 1];
         TexturePts = new Vector2[ColumnsNb + 1, RowsNb + 1];
         Vertices = new VertexPositionTexture[NbSommets];

         CréerTableauPoints();
         InitialiserSommets();

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
      }

      protected override void InitialiserSommets()
      {
         int NoSommet = -1;
         for (int j = 0; j < RowsNb; ++j)
         {
            for (int i = 0; i <= ColumnsNb; ++i)
            {
               Vertices[++NoSommet] = new VertexPositionTexture(VerticesPoints[i, j], TexturePts[i,j]);
               Vertices[++NoSommet] = new VertexPositionTexture(VerticesPoints[i, j + 1], TexturePts[i, j + 1]);
            }
         }
      }

      public override void Draw(GameTime gameTime)
      {
         Jeu.GraphicsDevice.VertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);
         Jeu.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
         //BasicEffect effetAffichage = new BasicEffect(Jeu.GraphicsDevice, null);
         BasicEffect effetAffichage = Jeu.Affichage3D.Effet3D;
         effetAffichage.World = Monde;
         effetAffichage.View = Jeu.CaméraJeu.Vue;
         effetAffichage.Projection = Jeu.CaméraJeu.Projection;
         effetAffichage.TextureEnabled = true;
         effetAffichage.Texture = ImageTexture;
         effetAffichage.Begin();
         foreach (EffectPass passeEffet in effetAffichage.CurrentTechnique.Passes)
         {
            passeEffet.Begin();
            for (int noStrip = 0; noStrip < RowsNb; ++noStrip)
            {
               Jeu.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Vertices, (TrianglesNbPerStrip + 2) * noStrip, TrianglesNbPerStrip);
            }
            passeEffet.End();
         }
         effetAffichage.End();
         effetAffichage.TextureEnabled = false;
         base.Draw(gameTime);
      }
   }
}
