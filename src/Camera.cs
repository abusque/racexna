using Microsoft.Xna.Framework;

namespace RaceXNA
{
    public abstract class Camera : Microsoft.Xna.Framework.GameComponent
    {
        protected const float NEAR_PLANE_DISTANCE = 0.1f;
        protected const float FAR_PLANE_DISTANCE = 1000;
        protected const float FOV = MathHelper.PiOver4; //45 degrees

        protected RacingGame RaceGame { get; set; }
        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }

        public Vector3 Position { get; protected set; }
        public Vector3 Target { get; protected set; }
        public Vector3 VerticalOrientation { get; protected set; }

        protected float FovAngle { get; set; }
        protected float AspectRatio { get; set; }
        protected float NearPlaneDistance { get; set; }
        protected float FarPlaneDistance { get; set; }

        public Camera(RacingGame raceGame)
            : base(raceGame)
        {
            RaceGame = raceGame;
            float ratio = RaceGame.GraphicsDevice.Viewport.AspectRatio;
            CreateViewpoint(FOV, ratio, NEAR_PLANE_DISTANCE, FAR_PLANE_DISTANCE);
        }

        public virtual void CreateView()
        {
            View = Matrix.CreateLookAt(Position, Target, VerticalOrientation);
        }

        protected void CreateProjection()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(FovAngle, AspectRatio, NearPlaneDistance, FarPlaneDistance);
        }

        protected virtual void CreateViewpoint(float fovAngle, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            //Initialisation des propriétés de la matrice de projection (volume de visualisation)
            FovAngle = fovAngle;
            AspectRatio = aspectRatio;
            NearPlaneDistance = nearPlaneDistance;
            FarPlaneDistance = farPlaneDistance;
        }
    }
}
