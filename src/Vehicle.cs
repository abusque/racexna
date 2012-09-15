using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
    public class Vehicle : BaseObject
    {
        #region Constants
        const float MAX_ACCEL = 20.0f;
        const float MIN_ACCEL = -15.0f;
        const float MAX_SPEED = 80.0f;
        const float MIN_SPEED = -20.0f;
        const float BASE_ACCEL = 7.0f;
        const float FRICTION = 9.0f;
        const float MIN_CRASH_SPEED = 15.0f;
        const float MIN_BRAKE_SPEED = 15.0f;
        const float ROT_COEFF = 0.075f;
        const float DELTA_ROT = 5.0f;
        const float MAX_YAW = 1.8f;
        const float SMALL_RADIUS_FACTOR = 0.85f;
        #endregion Constants

        #region Properties
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
                if (value > MAX_YAW)
                {
                    yaw = MAX_YAW;
                }
                else if (value < -MAX_YAW)
                {
                    yaw = -MAX_YAW;
                }
                else
                    yaw = value;
            }
        }

        public SpringCamera Camera { get; private set; }
        public BoundingBox CarBoundingBox { get; private set; }
        public bool IsCollision { get; set; }
        public float PrevRot { get; set; }
        public BoundingSphere[] CollisionSpheres { get; set; }
        public SoundEffect CrashSound { get; private set; }
        public SoundEffect EngineSound { get; private set; }
        public SoundEffect BrakeSound { get; private set; }

        #endregion Properties

        public Vehicle(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, Vector3 initRot)
            : base(raceGame, modelName, initPos, initScale, initRot)
        {
            Acceleration = 0;
            Speed = 0;
            PrevRot = 0;
            Camera = new SpringCamera(this, new Vector3(0, 400, -1000), new Vector3(0, 300, 0));
            IsCollision = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            CollisionSpheres = new BoundingSphere[3];
            CreateCollisionSpheres();
            CrashSound = RaceGame.SfxMgr.Find("Sounds/crash");
            BrakeSound = RaceGame.SfxMgr.Find("Sound/crash");
        }
        
        private void CreateCollisionSpheres()
        {
            float radius = BigSphere.Radius / 2;
            radius *= SMALL_RADIUS_FACTOR;

            Vector3 forw = World.Forward;
            forw = Vector3.Normalize(forw);

            CollisionSpheres[0] = new BoundingSphere(Position + forw * radius, radius);
            CollisionSpheres[1] = new BoundingSphere(Position, radius);
            CollisionSpheres[2] = new BoundingSphere(Position - forw * radius, radius);
        }

        public override void Update(GameTime gameTime)
        {

            HandleAcceleration();

            HandleRotation();

            Move();

            HandleCollision();

            #region ProgrammerHelper
            if (RaceGame.InputMgr.ControllerState.IsButtonDown(Buttons.B))
            {
                Speed = 0; Acceleration = 0;
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
                if (Speed > MIN_BRAKE_SPEED)
                    BrakeSound.Play();

                Acceleration = leftTriggerValue * MIN_ACCEL;
            }
            else if (rightTriggerValue > 0)
            {
                if (Speed < -MIN_BRAKE_SPEED)
                    BrakeSound.Play();

                Acceleration = rightTriggerValue * MAX_ACCEL;
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
            float targetRot = -RaceGame.InputMgr.ControllerState.ThumbSticks.Left.X;
            float attenuatedRot;

            if (targetRot < PrevRot)
            {
                attenuatedRot = PrevRot - DELTA_ROT / RaceGame.FpsHandler.FpsValue;
                if (attenuatedRot < targetRot)
                    attenuatedRot = targetRot;
            }
            else if (targetRot > PrevRot)
            {
                attenuatedRot = PrevRot + DELTA_ROT / RaceGame.FpsHandler.FpsValue;
                if (attenuatedRot > targetRot)
                    attenuatedRot = targetRot;
            }
            else
            {
                attenuatedRot = targetRot;
            }

            PrevRot = attenuatedRot;

            Yaw = ROT_COEFF * attenuatedRot * (float)(Math.Abs(Speed));

            if (Speed >= 0.01f || Speed <= -0.01f)
                Rotation = new Vector3(Rotation.X, Rotation.Y + Yaw / RaceGame.FpsHandler.FpsValue, Rotation.Z);

            Orientation = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z);
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
                if (IsObstacleCollision(RaceGame.GameTrack.Obstacles[i]))
                {
                    if (Speed > MIN_CRASH_SPEED)
                        CrashSound.Play();

                    Vector3 VectorCollision = new Vector3(Position.X - RaceGame.GameTrack.Obstacles[i].Position.X,
                                                          Position.Y - RaceGame.GameTrack.Obstacles[i].Position.Y,
                                                          Position.Z - RaceGame.GameTrack.Obstacles[i].Position.Z);
                    VectorCollision.Normalize();
                    VectorCollision /= RaceGame.FpsHandler.FpsValue;
                    Position += VectorCollision;
                    Speed = 0;
                }
                else if (RaceGame.GameTrack.Ground.IsOnHeightmap(Position))
                {
                    CheckTerrainCollision(Position);
                }
            }
        }

        private bool IsObstacleCollision(BaseObject oneObstacle)
        {
            BigSphere = new BoundingSphere(Position, BigSphere.Radius);

            CreateCollisionSpheres();
            if (BigSphere.Intersects(oneObstacle.BigSphere))
            {
                for (int i = 0; i < CollisionSpheres.Length; ++i)
                {
                    for (int j = 0; j < oneObstacle.GetSpheresLength(); ++j)
                    {
                        if (CollisionSpheres[i].Intersects(oneObstacle.GetBox(j)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void CheckTerrainCollision(Vector3 newPos)
        {
            Vector3 normal;

            RaceGame.GameTrack.Ground.GetHeightAndNormal(newPos,
                out newPos.Y, out normal);

            Orientation.Up = normal;

            Orientation.Right = Vector3.Cross(Orientation.Forward, Orientation.Up);
            Orientation.Right = Vector3.Normalize(Orientation.Right);

            Orientation.Forward = Vector3.Cross(Orientation.Up, Orientation.Right);
            Orientation.Forward = Vector3.Normalize(Orientation.Forward);

            Position = newPos;
        }
    }
}