//Class based on ChaseCamera Sample from Microsoft
using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public class SpringCamera : Camera
    {
        public const float STIFFNESS = 4000.0f;
        public const float DAMPING = 600.0f;
        public const float MASS = 50.0f;

        #region Properties

        public Vector3 DesiredPositionOffset { get; private set; }

        public Vector3 DesiredPosition
        {
            get
            {
                UpdateWorldPositions();
                return desiredPosition;
            }
            private set
            {
                desiredPosition = value;
            }
        }
        private Vector3 desiredPosition;

        public Vector3 LookAtOffset { get; private set; }

        public Vector3 LookAt
        {
            get
            {
                UpdateWorldPositions();
                return lookAt;
            }
            private set
            {
                lookAt = value;
            }
        }
        private Vector3 lookAt;

        public Vector3 Velocity { get; private set; }

        private Vehicle TargetVehicle { get; set; }

        #endregion Properties

        public SpringCamera(Vehicle targetVehicle, Vector3 desiredPositionOffset, Vector3 lookAtOffset)
            : base(targetVehicle.RaceGame)
        {
            TargetVehicle = targetVehicle;
            DesiredPositionOffset = desiredPositionOffset;
            LookAtOffset = lookAtOffset;
        }

        private void UpdateWorldPositions()
        {
            Matrix transform = Matrix.Identity;
            transform.Forward = TargetVehicle.GetWorld().Forward;
            transform.Up = TargetVehicle.GetWorld().Up;
            transform.Right = Vector3.Cross(TargetVehicle.GetWorld().Up, TargetVehicle.GetWorld().Forward);

            desiredPosition = TargetVehicle.Position + Vector3.TransformNormal(DesiredPositionOffset, transform);
            lookAt = TargetVehicle.Position + Vector3.TransformNormal(LookAtOffset, transform);
        }

        private void UpdateMatrices()
        {
            View = Matrix.CreateLookAt(this.Position, this.LookAt, TargetVehicle.GetWorld().Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(FovAngle, AspectRatio, NearPlaneDistance, FarPlaneDistance);
        }

        public void Reset()
        {
            UpdateWorldPositions();

            Velocity = Vector3.Zero;
            Position = DesiredPosition;

            UpdateMatrices();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateWorldPositions();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 stretch = Position - DesiredPosition;
            Vector3 force = -STIFFNESS * stretch - DAMPING * Velocity;

            Vector3 acceleration = force / MASS;
            Velocity += acceleration * elapsed;

            Position += Velocity * elapsed;

            UpdateMatrices();
        }
    }
}