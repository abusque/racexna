using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
    public class Vehicle : BaseObject
    {
        const float MAX_ACCEL = 30.0f;
        const float MIN_ACCEL = -20.0f;
        const float MAX_SPEED = 80.0f;
        const float MIN_SPEED = -40.0f;
        const float BASE_ACCEL = 7.0f;
        const float FRICTION = 2.5f;

        const float BASE_ROT = 0.75f;

        public enum Gears { Neutral, Forward, Reverse };

        public float Acceleration { get; private set; }
        public float Speed { get; private set; }
        public ChasingCamera Camera { get; private set; }
        public Gears GearState { get; private set; }

        public Vehicle(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
            : base(raceGame, modelName, initPos, initScale, initRot)
        {
            Acceleration = 0;
            Speed = 0;
            Camera = new ChasingCamera(this);
            GearState = Gears.Neutral; 
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

            if (leftTriggerValue > 0.0f)
            {
                if (GearState == Gears.Forward)
                    Acceleration = 0;

                Acceleration -= BASE_ACCEL * leftTriggerValue / RaceGame.FpsHandler.FpsValue;
                GearState = Gears.Reverse;

            }
            else if (rightTriggerValue > 0.0f)
            {
                if (GearState == Gears.Reverse)
                    Acceleration = 0;

                Acceleration += BASE_ACCEL * rightTriggerValue / RaceGame.FpsHandler.FpsValue;
                GearState = Gears.Forward;
            }

            if (leftTriggerValue == 0 && rightTriggerValue == 0)
            {
                Acceleration = 0.0f;
                GearState = Gears.Neutral;
            }
            


            if (Acceleration >= MAX_ACCEL)
                Acceleration = MAX_ACCEL;
            else if (Acceleration <= MIN_ACCEL)
                Acceleration = MIN_ACCEL;

            Speed += Acceleration / RaceGame.FpsHandler.FpsValue;

            if (Speed >= MAX_SPEED)
                Speed = MAX_SPEED;
            else if (Speed <= MIN_SPEED)
                Speed = MIN_SPEED;

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

            Position -= forward * Speed / RaceGame.FpsHandler.FpsValue; //A changer pour += lorsque l'orientation du modele sera la bonne
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