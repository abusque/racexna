using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class ChasingCamera : Camera
    {
        const float HEIGHT = 1;
        const float DISTANCE = 1;

        Vehicle TargetVehicle { get; set; }

        public ChasingCamera(Vehicle targetVehicle)
            : base(targetVehicle.RaceGame)
        {
            TargetVehicle = targetVehicle;

            Position = new Vector3(TargetVehicle.Position.X, TargetVehicle.Position.Y + HEIGHT, TargetVehicle.Position.Z + DISTANCE);
            Target = TargetVehicle.Position;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Position = new Vector3(TargetVehicle.Position.X, TargetVehicle.Position.Y + HEIGHT, TargetVehicle.Position.Z + DISTANCE);
            Target = TargetVehicle.Position;
            CreateView();

            base.Update(gameTime);
        }

    }
}