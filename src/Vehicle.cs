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
        const float FRICTION = 2.0f;

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
            Vector3 backward = World.Backward;
            backward.Normalize();

            HandleInput();
            if (Acceleration.Length() > 0)
            {
                Acceleration += backward * FRICTION;
            }

            Velocity += Acceleration / RaceGame.FpsHandler.FpsValue;
            if (Velocity.Z < 0)
            {
                Velocity = Vector3.Zero;
                Acceleration = Vector3.Zero;
            }

            //J'ai fait mes changements dans une nouvelle m�thode temporairement 
            //pour pas que tu sois m�langer quand tu voudras modifier l'acc�l�ration
            HandleRotation();

            Position -= Velocity / RaceGame.FpsHandler.FpsValue;

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            Vector3 forward = World.Forward;
            forward.Normalize();
            float rightTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Right;
            float leftTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Left;
            
            if (rightTriggerValue > 0.0f)
            {
                Acceleration += forward * BASE_ACCEL * rightTriggerValue;
            }
            if (leftTriggerValue > 0.0f)
            {
                Acceleration -= forward * BASE_ACCEL * leftTriggerValue;
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