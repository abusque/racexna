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

        public enum Gear { Neutral, Forward, Reverse };

        public float Acceleration { get; private set; }
        public float Speed { get; private set; }
        public ChasingCamera Camera { get; private set; }
        public Gear GearState { get; private set; }

        public Vehicle(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
            : base(raceGame, modelName, initPos, initScale, initRot)
        {
            Acceleration = 0;
            Speed = 0;
            Camera = new ChasingCamera(this);
            GearState = Gear.Neutral;
        }

        public override void Update(GameTime gameTime)
        {

            HandleAcceleration();

            HandleRotation();

            Move();

            base.Update(gameTime);
        }

        private void HandleAcceleration()
        {
            float rightTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Right;
            float leftTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Left;
            
            if (rightTriggerValue > 0.0f)
            {
                Acceleration += BASE_ACCEL * rightTriggerValue / RaceGame.FpsHandler.FpsValue;
                GearState = Gear.Forward;
            }
            else if (leftTriggerValue > 0.0f)
            {
                Acceleration -= BASE_ACCEL * leftTriggerValue / RaceGame.FpsHandler.FpsValue;
                GearState = Gear.Reverse;
            }
            else
            {
                Acceleration = 0.0f;
                GearState = Gear.Neutral;
            }

            Speed += Acceleration / RaceGame.FpsHandler.FpsValue;

            if (Speed > 0.0f)
            {
                Speed -= FRICTION / RaceGame.FpsHandler.FpsValue;
                if (Speed < 0.0f)
                    Speed = 0.0f;
            }
            else if (Speed < 0.0f)
            {
                Speed += FRICTION / RaceGame.FpsHandler.FpsValue;
                if (Speed > 0.0f)
                    Speed = 0.0f;
            }
        }

        private void Move()
        {
            Vector3 forward = World.Forward;
            forward.Normalize();

            Position -= forward * Speed / RaceGame.FpsHandler.FpsValue;
        }

        private void HandleRotation()
        {
            if(Speed < 0.01f  && Speed > -0.01f)
                return;

            float leftThumbStickHorizontalValue = -RaceGame.InputMgr.ControllerState.ThumbSticks.Left.X;
            float yaw;

            yaw = BASE_ROT * leftThumbStickHorizontalValue / RaceGame.FpsHandler.FpsValue;

            if (Speed < 0.0f)
                yaw = -yaw;

            Rotation = new Vector3(Rotation.X, Rotation.Y + yaw, Rotation.Z);
        }
    }
}