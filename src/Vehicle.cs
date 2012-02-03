using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
    public class Vehicle : BaseObject
    {
        const float BASE_ACCEL = 1.0f;

        public Vector3 Acceleration { get; private set; }
        public Vector3 Velocity { get; private set; }
        public ChasingCamera Camera { get; private set; }

        public Vehicle(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
            : base(raceGame, modelName, initPos, initScale, initRot)
        {
            Acceleration = Vector3.Zero;
            Velocity = Vector3.Zero;
            Camera = new ChasingCamera(this);
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();
            Velocity += Acceleration * gameTime.ElapsedGameTime.Seconds;
            Position += Velocity * gameTime.ElapsedGameTime.Seconds;

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            Vector3 forward = World.Forward;
            forward.Normalize();
            float rightTriggerValue = RaceGame.InputMgr.CurrentControllerState.Triggers.Right;


            if (rightTriggerValue > 0.0f)
            {
                Acceleration += forward * BASE_ACCEL * rightTriggerValue;
            }
        }
    }
}