using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class Terrain : TexturedSurface
    {
        
        const string ASPHALT = "asphalt1";
        const float ASPHALT_FRICTION = 0.05f;
        const string GRASS = "grass1";
        const float GRASS_FRICTION = 2 * ASPHALT_FRICTION;
        const string SAND = "sand1";
        const float SAND_FRICTION = 4 * ASPHALT_FRICTION;
        public float FrictionValue { get; private set; }
        public string TerrainType { get; private set; }

        public Terrain(RacingGame raceGame, Vector3 origine, Vector3 size, Vector2 dimension, String textureName, bool isTextureRepeated)
            :base(raceGame,origine,size,dimension,textureName,isTextureRepeated)
        {
            TerrainType = textureName;
            DeterminateFrictionValue();
        }

        private void DeterminateFrictionValue()
        {
            switch (TextureName)
            {
                case ASPHALT:
                    FrictionValue = ASPHALT_FRICTION;
                    break;
                case GRASS:
                    FrictionValue = GRASS_FRICTION;
                    break;
                case SAND:
                    FrictionValue = SAND_FRICTION;
                    break;
            }
        }
    }
}