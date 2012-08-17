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
        const float BRAKE_ACCEL = 60.0f;
        const float MAX_SPEED = 120.0f;
        const float MIN_SPEED = -20.0f;
        const float BASE_ACCEL = 7.0f;
        const float FRICTION = 2.5f;

        const float MAX_ROT = 0.75f;
        const float ROT_COEFF = 0.15f; //Valeur positive, sinon rot. inverse

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
        public BoundingBox CarBoundingBox { get; private set; }
        public bool IsCollision { get; set; }

        public Vehicle(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
            : base(raceGame, modelName, initPos, initScale, initRot)
        {
            Acceleration = 0;
            Speed = 0;
            Camera = new ChasingCamera(this);
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

            CreateSpheres();
            CreateBoxes();

            HandleCollision();

            #region ProgrammerHelper
            if (RaceGame.InputMgr.ControllerState.IsButtonDown(Buttons.B))
            {
                Speed = 0; Acceleration = 0;
            }
            if (RaceGame.InputMgr.ControllerState.IsButtonDown(Buttons.X))
            {
                if (Speed >=0)
                    Rotation = new Vector3(Rotation.X, Rotation.Y + MAX_ROT * -RaceGame.InputMgr.ControllerState.ThumbSticks.Left.X * 1.5f / RaceGame.FpsHandler.FpsValue, Rotation.Z);
                else
                    Rotation = new Vector3(Rotation.X, Rotation.Y - MAX_ROT * -RaceGame.InputMgr.ControllerState.ThumbSticks.Left.X * 1.5f / RaceGame.FpsHandler.FpsValue, Rotation.Z);
            }
            #endregion

            base.Update(gameTime);
        }

        private void HandleAcceleration()
        {
            float rightTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Right;
            float leftTriggerValue = RaceGame.InputMgr.ControllerState.Triggers.Left;

            if (leftTriggerValue > 0)
            {
                if (Speed <= 0)
                    Acceleration = leftTriggerValue * MIN_ACCEL;
                else
                    Acceleration = -BRAKE_ACCEL;
            }
            else if (rightTriggerValue > 0)
            {
                if (Speed >= 0)
                    Acceleration = rightTriggerValue * MAX_ACCEL;
                else
                    Acceleration = BRAKE_ACCEL;
            }
            else
            {
                Acceleration = 0;
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

            Yaw = MAX_ROT * leftThumbStickHorizontalValue * ROT_COEFF * (float)(Math.Sqrt(Math.Abs(Speed))) / RaceGame.FpsHandler.FpsValue;
            
            Rotation = new Vector3(Rotation.X, Rotation.Y + Yaw, Rotation.Z);
        }

        private void Move()
        {
            Vector3 forward = World.Forward;
            forward.Normalize();

            Position -= forward * Speed / RaceGame.FpsHandler.FpsValue; //A changer pour += lorsque l'orientation du modele sera la bonne
        }

        private void HandleCollision()
        {
            for (int i = 0; i < RaceGame.GameTrack.Obstacles.Count; ++i)
            {
                if (CheckCollision(RaceGame.GameTrack.Obstacles[i]))
                {
                    Vector3 VectorCollision = new Vector3(Position.X - RaceGame.GameTrack.Obstacles[i].Position.X,
                                                          Position.Y - RaceGame.GameTrack.Obstacles[i].Position.Y,
                                                          Position.Z - RaceGame.GameTrack.Obstacles[i].Position.Z);
                    VectorCollision.Normalize();
                    VectorCollision /= RaceGame.FpsHandler.FpsValue;
                    Position += VectorCollision;
                    Speed = 0;
                }
            }
        }

        private bool CheckCollision(BaseObject oneObstacle)
        {
            BigSphere = new BoundingSphere(Position, BigSphere.Radius);
            BoundingBox box;

            if (BigSphere.Intersects(oneObstacle.BigSphere))
            {
                for (int i = 0; i < Boxes.Count; ++i)
                {
                    box = Boxes[i];
                    Vector3[] corners = box.GetCorners();
                    Matrix localWorld = GetWorld();
                    Vector3.Transform(corners, ref localWorld, corners);
                    box = BoundingBox.CreateFromPoints(corners);
                    VisibleBoxes[i].SetCorners(corners);
                    VisibleBoxes[i].SetWorld(World);

                    for (int j = 0; j < oneObstacle.GetBoxesCount(); ++j)
                    {
                        if (box.Intersects(oneObstacle.GetBox(j)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}