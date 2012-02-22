using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
    public class Vehicle : BaseObject
    {
        const float MAX_ACCEL = 20.0f;
        const float MIN_ACCEL = -15.0f;
        const float MAX_SPEED = 120.0f;
        const float MIN_SPEED = -20.0f;
        const float BASE_ACCEL = 7.0f;
        const float FRICTION = 2.5f;

        const float MAX_ROT = 0.75f;
        const float ROT_COEFF = 0.1f; //doit être suppérieur à 0 pour faire sens

        public enum Gears { Neutral, Forward, Reverse };

        public float Acceleration;
        float speed;
        public float Speed 
        {
            get
            {
                return speed;
            }
            private set
            {
                if (value < MAX_SPEED && value > MIN_SPEED)
                    speed = value;
                else if (value >= MAX_SPEED)
                    speed = MAX_SPEED;
                else
                    speed = MIN_SPEED;
            }
        }
        float yaw;
        public float Yaw
        {
            get
            {
                if (Speed > 0)
                    return yaw;
                else if (Speed < 0)
                    return -yaw;
                else
                    return 0;
            }
            set
            {
                yaw = value;
            }
        }
        public ChasingCamera Camera { get; private set; }
        public Gears GearState { get; private set; }
        public BoundingBox ModelBoundingBox { get; private set; }
        public bool IsCollision { get; set; }

        public Vehicle(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
            : base(raceGame, modelName, initPos, initScale, initRot)
        {
            Acceleration = 0;
            Speed = 0;
            Camera = new ChasingCamera(this);
            GearState = Gears.Neutral;
            IsCollision = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            HandleAcceleration();

            HandleRotation();

            Move();

            HandleCollision();

            base.Update(gameTime);
        }

        private void HandleAcceleration()
        {
            float rightTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Right;
            float leftTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Left;

            if (leftTriggerValue > 0)
            {
                Acceleration = leftTriggerValue * MIN_ACCEL;
                GearState = Gears.Reverse;
            }
            else if (rightTriggerValue  > 0)
            {
                Acceleration = rightTriggerValue * MAX_ACCEL;
                GearState = Gears.Forward;
                
            }
            else
            {
                Acceleration = 0;
                GearState = Gears.Neutral;
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

        private void HandleRotation()
        {
            if (Speed < 0.01f && Speed > -0.01f)
                return;

            float leftThumbStickHorizontalValue = -RaceGame.InputMgr.ControllerState.ThumbSticks.Left.X;



            if (RaceGame.InputMgr.ControllerState.IsButtonDown(Buttons.X))
            {
                float yawValue = 0;
                yawValue = MAX_ROT * leftThumbStickHorizontalValue / RaceGame.FpsHandler.FpsValue;
                Rotation = new Vector3(Rotation.X, Rotation.Y + yawValue, Rotation.Z);
            }
            else
            {
                Yaw = MAX_ROT * leftThumbStickHorizontalValue * ROT_COEFF * (float)(Math.Sqrt(Math.Abs(Speed))) / RaceGame.FpsHandler.FpsValue;
                Rotation = new Vector3(Rotation.X, Rotation.Y + Yaw, Rotation.Z);
            }

        }

        private void Move()
        {
            Vector3 forward = World.Forward;
            forward.Normalize();

            Position -= forward * Speed / RaceGame.FpsHandler.FpsValue; //A changer pour += lorsque l'orientation du modele sera la bonne
        }

        private void HandleCollision()
        {
            for (int i = 0; i < ModelData.Meshes.Count; ++i)
            {
                Spheres[i] = new BoundingSphere(Position, Spheres[i].Radius);
            }

            CreateBigSphere();

            IsCollision = false;

            if (BigSphere.Intersects(RaceGame.OneObstacle.BigSphere))
            {
                for (int i = 0; i < ModelData.Meshes.Count; ++i)
                {
                    for (int j = 0; j < RaceGame.OneObstacle.ModelData.Meshes.Count; ++j)
                    {
                        IsCollision = Spheres[i].Intersects(RaceGame.OneObstacle.GetSphere(j));
                    }
                }
            }

            if (IsCollision)
            {
                Vector3 VectorCollision = new Vector3(Position.X - RaceGame.OneObstacle.Position.X,
                                                      Position.Y - RaceGame.OneObstacle.Position.Y,
                                                      Position.Z - RaceGame.OneObstacle.Position.Z);
                VectorCollision.Normalize();
                VectorCollision /= RaceGame.FpsHandler.FpsValue;
                Position += VectorCollision;
                Speed = 0;
            }
        }
    }
}