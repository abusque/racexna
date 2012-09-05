using Microsoft.Xna.Framework;

namespace RaceXNA
{
   public class StationaryCamera : Camera
   {
      public StationaryCamera(RacingGame raceGame, Vector3 position, Vector3 target, Vector3 orientation)
         : base(raceGame)
      {
         //Initialisation des propriétés de la matrice de vue (point de vue)
         CreateViewpoint(position, target, orientation);
         CreateProjection();
      }

      public virtual void CreateViewpoint(Vector3 position, Vector3 target, Vector3 orientation)
      {
         //Initialisation des propriétés de la matrice de vue (point de vue)
         Position = position;
         Target = target;
         VerticalOrientation = orientation;
         //Création de la matrice de vue (point de vue)
         CreateView();
      }
   }
}
