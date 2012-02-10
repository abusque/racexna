using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class Tile : BasePrimitive
    {
        const float CIRCLE_IN_DEGREES = 360;

        public Tile(RacingGame raceGame, Vector3 origin, Vector3 size, String textureName)
         : base(raceGame)
      {
         Origin = origin;
         Size = size;
         TextureName = textureName;
         TextureData = RaceGame.TextureMgr.Find(TextureName);
      }

       public override void Initialize()
       {
           PointsNb = (ColumnsNb + 1) * (RowsNb + 1);
           TrianglesPerStrip = ColumnsNb * 2;
           NbVertices = (TrianglesPerStrip + 2) * RowsNb;
           VerticesPoints = new Vector3[ColumnsNb + 1, RowsNb + 1];
           TexturePts = new Vector2[ColumnsNb + 1, RowsNb + 1];
           Vertices = new VertexPositionTexture[NbVertices];

           CreatePointsArray();
           InitializeVertices();

           base.Initialize();
        }

       private void CreatePointsArray()
       {
           for (int i = 0; i <= ColumnsNb; ++i)
           {
               for (int j = 0; j <= RowsNb; ++j)
               {
                   VerticesPoints[i, j] = new Vector3(Origin.X + (i * Delta.X), Origin.Y + (j * Delta.Y), Origin.Z + (j * Delta.Z));
                   TexturePts[i, j] = new Vector2(i * DeltaTexture.X, 1 - j * DeltaTexture.Y);
               }
           }
       }
    }
}
