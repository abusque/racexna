using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
    public class Vehicle : BaseObject
    {
        const float BASE_ACCEL = 0.25f;
        const float FRICTION = 0.05f;

        const float BASE_ROT = 0.75f;

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
            //J'ai fait mes changements dans une nouvelle méthode temporairement 
            //pour pas que tu sois mélanger quand tu voudras modifier l'accélération
            HandleRotation();

            Vector3 forward = World.Forward;
            forward.Normalize();

            HandleInput();
            Acceleration -= forward * FRICTION;

            Velocity += Acceleration / RaceGame.FpsHandler.FpsValue;
            if (Velocity.Z < 0)
                Velocity = Vector3.Zero;

            Position -= Velocity / RaceGame.FpsHandler.FpsValue;

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            Vector3 forward = World.Forward;
            forward.Normalize();
            float rightTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Right;
            
            if (rightTriggerValue > 0.0f)
            {
                Acceleration += forward * BASE_ACCEL * rightTriggerValue;
            }
        }

        private void HandleRotation()
        {
            float leftThumbStickHorizontalValue = -RaceGame.InputMgr.ControllerState.ThumbSticks.Left.X;
            float yaw;

            yaw = BASE_ROT * leftThumbStickHorizontalValue / RaceGame.FpsHandler.FpsValue;

            Rotation = new Vector3(Rotation.X, Rotation.Y + yaw, Rotation.Z);
        }
    }
}