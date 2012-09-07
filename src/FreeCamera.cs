using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RaceXNA
{
   public class FreeCamera : StationaryCamera
   {
      const float DEFAULT_SPEED = 3.0f;
      const float DEFAULT_ROTATION = 1.0f;
      const float RADIUS = 0.1f;
      private float Yaw { get; set; }
      private float Pitch { get; set; }
      private Matrix CameraRotation { get; set; }

      public FreeCamera(RacingGame raceGame, Vector3 position, Vector3 target, Vector3 orientation)
         : base(raceGame, position, target, orientation)
      {
          Yaw = 0;
          Pitch = 0;

          CameraRotation = Matrix.Identity;
      }

      public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
      {
         HandleRotation();
         HandleMovement();
         CreateView();

         base.Update(gameTime);
      }

      private void HandleMovement()
      {
         if (RaceGame.InputMgr.IsKeyDown(Keys.W))
              MoveCamera(CameraRotation.Forward);
         if (RaceGame.InputMgr.IsKeyDown(Keys.S))
             MoveCamera(CameraRotation.Backward);
         if (RaceGame.InputMgr.IsKeyDown(Keys.A))
             MoveCamera(CameraRotation.Left);
         if (RaceGame.InputMgr.IsKeyDown(Keys.D))
             MoveCamera(CameraRotation.Right);
      }

      private void HandleRotation()
      {
          float rotation = 0;

          if (RaceGame.FpsHandler.FpsValue > 0)
              rotation = DEFAULT_ROTATION / RaceGame.FpsHandler.FpsValue;
          if (RaceGame.InputMgr.IsKeyDown(Keys.Up))
              Pitch += rotation;
          if (RaceGame.InputMgr.IsKeyDown(Keys.Down))
              Pitch -= rotation;
          if (RaceGame.InputMgr.IsKeyDown(Keys.Left))
              Yaw += rotation;
          if (RaceGame.InputMgr.IsKeyDown(Keys.Right))
              Yaw -= rotation;
      }


      private void MoveCamera(Vector3 movement)
      {
          float speed = 0;
          Vector3 newPosition;
          BoundingSphere currentBoundingSphere;

          if (RaceGame.FpsHandler.FpsValue > 0)
              speed = DEFAULT_SPEED / RaceGame.FpsHandler.FpsValue;

          newPosition = Position + speed * movement;
          currentBoundingSphere = new BoundingSphere(newPosition, RADIUS);
          
          Position = newPosition;
      }

      public override void CreateView()
      {
          CameraRotation.Forward.Normalize();
          CameraRotation.Up.Normalize();
          CameraRotation.Right.Normalize();

          CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Up, Yaw);
          CameraRotation *= Matrix.CreateFromAxisAngle(CameraRotation.Right, Pitch);

          Yaw = 0.0f;
          Pitch = 0.0f;

          Target = Position + CameraRotation.Forward;

          View = Matrix.CreateLookAt(Position, Target, CameraRotation.Up);
      }
   }
}
