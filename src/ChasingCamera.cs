using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class ChasingCamera : Camera
    {
        const float HEIGHT = 3.5f;
        const float DISTANCE = 12;

        Vehicle TargetVehicle { get; set; }

        public ChasingCamera(Vehicle targetVehicle)
            : base(targetVehicle.RaceGame)
        {
            TargetVehicle = targetVehicle;

            Position = new Vector3(TargetVehicle.Position.X, TargetVehicle.Position.Y + HEIGHT, TargetVehicle.Position.Z - DISTANCE);
            Target = new Vector3(TargetVehicle.Position.X, TargetVehicle.Position.Y + HEIGHT, TargetVehicle.Position.Z);
            CreateViewpoint(Position, Target, Vector3.Up);
            CreateProjection();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Position = new Vector3(TargetVehicle.Position.X, TargetVehicle.Position.Y + HEIGHT, TargetVehicle.Position.Z - DISTANCE);
            Target = new Vector3(TargetVehicle.Position.X, TargetVehicle.Position.Y + HEIGHT, TargetVehicle.Position.Z);
            CreateViewpoint(Position, Target, Vector3.Up);

            base.Update(gameTime);
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