using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class TerrainTile : Tile
    {
        public enum TerrainTypes { Asphalt, Grass, Sand };

        const string ASPHALT_FILENAME = "asphalt1";
        const string GRASS_FILENAME =  "grass1";
        const string SAND_FILENAME = "sand1";

        const float ASPHALT_FRICTION = 0.05f;
        const float GRASS_FRICTION = 0.1f;
        const float SAND_FRICTION = 0.2f;

        public float FrictionCoeff { get; private set; }
        public TerrainTypes TerrainType { get; private set; }

        public TerrainTile(RacingGame raceGame, Vector3 origin, Vector2 size, TerrainTypes terrainType)
            :base(raceGame, origin, size, GetTextureName(terrainType))
        {
            TerrainType = terrainType;
            SetFrictionCoeff();
        }

        private void SetFrictionCoeff()
        {
            switch (TerrainType)
            {
                case TerrainTypes.Asphalt:
                    FrictionCoeff = ASPHALT_FRICTION;
                    break;
                case TerrainTypes.Grass:
                    FrictionCoeff = GRASS_FRICTION;
                    break;
                case TerrainTypes.Sand:
                    FrictionCoeff = SAND_FRICTION;
                    break;
                default:
                    FrictionCoeff = ASPHALT_FRICTION;
                    break;
            }
        }

        static public string GetTextureName(TerrainTypes terrainType)
        {
            switch (terrainType)
            {
                case TerrainTypes.Asphalt:
                    return ASPHALT_FILENAME;
                case TerrainTypes.Grass:
                    return GRASS_FILENAME;
                case TerrainTypes.Sand:
                    return SAND_FILENAME;
                default:
                    return ASPHALT_FILENAME;

            }
        }
    }
}