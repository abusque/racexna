using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
    public class Vehicle : BaseObject
    {
        const float MAX_ACCEL = 100.0f;
        const float BASE_ACCEL = 7.0f;
        const float FRICTION = 2.5f;

        const float BASE_ROT = 0.75f;

        public Vector3 Acceleration { get; private set; }
        public Vector3 Velocity { get; private set; }
        public ChasingCamera Camera { get; private set; }
        public bool GoingForward { get; private set; }

        public Vehicle(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
            : base(raceGame, modelName, initPos, initScale, initRot)
        {
            Acceleration = Vector3.Zero;
            Velocity = Vector3.Zero;
            Camera = new ChasingCamera(this);
            GoingForward = true;
        }

        public override void Update(GameTime gameTime)
        {

            HandleAcceleration();



            HandleRotation();

            Position -= Velocity / RaceGame.FpsHandler.FpsValue;

            base.Update(gameTime);
        }

        private void HandleAcceleration()
        {
            Vector3 forward = World.Forward;
            forward.Normalize();
            float rightTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Right;
            float leftTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Left;
            
            if (rightTriggerValue > 0.0f)
            {
                Acceleration += forward * BASE_ACCEL * rightTriggerValue;
                GoingForward = true;
            }
            else if (leftTriggerValue > 0.0f)
            {
                Acceleration -= forward * BASE_ACCEL * leftTriggerValue;
                GoingForward = false;
            }

            if (Acceleration.Length() > 0.0f)
            {
                if (GoingForward)
                    Acceleration -= forward * FRICTION;
                else
                    Acceleration += forward * FRICTION;
            }

            Velocity += Acceleration / RaceGame.FpsHandler.FpsValue;
            if (GoingForward && Vector3.Dot(forward, Velocity) < 0)
            {
                Velocity = Vector3.Zero;
                Acceleration = Vector3.Zero;
            }
            else if(!GoingForward && Vector3.Dot(forward, Velocity) > 0)
            {
                Velocity = Vector3.Zero;
                Acceleration = Vector3.Zero;
            }
        }

        private void HandleRotation()
        {
            if(Velocity.Length() < 0.5f)
                return;

            float leftThumbStickHorizontalValue = -RaceGame.InputMgr.ControllerState.ThumbSticks.Left.X;
            float yaw;

            yaw = BASE_ROT * leftThumbStickHorizontalValue / RaceGame.FpsHandler.FpsValue;

            if (!GoingForward)
                yaw = -yaw;

            Rotation = new Vector3(Rotation.X, Rotation.Y + yaw, Rotation.Z);
        }
    }
}