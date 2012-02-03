using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class ChasingCamera : Camera
    {
        const Vector3 DEFAULT_ORIENTATION = Vector3.Up;
        const float HEIGHT = 1;
        const float DISTANCE = 10;


        public ChasingCamera(RacingGame raceGame)
            : base(raceGame)
        {
            Position = new Vector3(RaceGame.Car.Position.X, RaceGame.Car.Position.Y + HEIGHT, RaceGame.Car.Position.Z - DISTANCE);
            Target = RaceGame.Car.Position;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Position = new Vector3(RaceGame.Car.Position.X, RaceGame.Car.Position.Y + HEIGHT, RaceGame.Car.Position.Z - DISTANCE);
            Target = RaceGame.Car.Position;
            CreateView();

            base.Update(gameTime);
        }

    }
}