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
         if (RaceGame.InputMgr.ÉtatClavier.IsKeyDown(Keys.W))
              MoveCamera(CameraRotation.Forward);
         if (RaceGame.InputMgr.ÉtatClavier.IsKeyDown(Keys.S))
             MoveCamera(CameraRotation.Backward);
         if (RaceGame.InputMgr.ÉtatClavier.IsKeyDown(Keys.A))
             MoveCamera(CameraRotation.Left);
         if (RaceGame.InputMgr.ÉtatClavier.IsKeyDown(Keys.D))
             MoveCamera(CameraRotation.Right);
      }

      private void HandleRotation()
      {
          float rotation = 0;

          if (RaceGame.GestionFPS.ValFPS > 0)
              rotation = DEFAULT_ROTATION / RaceGame.GestionFPS.ValFPS;

          if (RaceGame.InputMgr.ÉtatClavier.IsKeyDown(Keys.Up))
              Pitch += rotation;
          if (RaceGame.InputMgr.ÉtatClavier.IsKeyDown(Keys.Down))
              Pitch -= rotation;
          if (RaceGame.InputMgr.ÉtatClavier.IsKeyDown(Keys.Left))
              Yaw += rotation;
          if (RaceGame.InputMgr.ÉtatClavier.IsKeyDown(Keys.Right))
              Yaw -= rotation;
      }


      private void MoveCamera(Vector3 movement)
      {
          float speed = 0;
          Vector3 newPosition;
          BoundingSphere currentBoundingSphere;

          if (RaceGame.GestionFPS.ValFPS > 0)
              speed = DEFAULT_SPEED / RaceGame.GestionFPS.ValFPS;

          newPosition = Position + speed * movement;
          currentBoundingSphere = new BoundingSphere(newPosition, RADIUS);

          if (currentBoundingSphere.Intersects(RaceGame.Phare.SphereEnglobante))
          {
              if (!IsCollision(newPosition, RaceGame.Phare) && !EstCollisionEau(newPosition))
                Position = newPosition;
          }
          else if(!EstCollisionEau(newPosition))
          {
             Position = newPosition;
          }
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

      private bool IsCollision(Vector3 pos, BaseObject obj)
      {
         BoundingSphere sphereCamera = new BoundingSphere(pos, RADIUS);

         for (int i = 0; i < obj.ModelData.Meshes.Count; ++i)
            if(sphereCamera.Intersects(obj.GetSphere(i)))
               return true;

         return false;
      }

      //private bool EstCollisionEau(Vector3 pos)
      //{
      //    return pos.Y <= Atelier.NIVEAU_EAU + RADIUS + SurfaceOndulee.AMPLITUDE_TOTALE;
      //}
      
   }
}
