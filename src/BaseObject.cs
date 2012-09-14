using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace RaceXNA
{
   public class BaseObject : Microsoft.Xna.Framework.DrawableGameComponent
   {
       const float RADIUS_MODIFICATOR = 0.75f;

       public RacingGame RaceGame { get; private set; }
       private string ModelName { get; set; }
       public Model ModelData { get; private set; }
       protected Matrix World;
       protected Matrix Orientation;
       public Vector3 Position { get; protected set; }
       public float Scale { get; protected set; }
       public float Rotation { get; protected set; }
       protected BoundingSphere[] Spheres { get; set; }
       public BoundingSphere BigSphere { get; protected set; }
       protected List<BoundingBox> Boxes { get; set; }
       public BoundingSphere SphereTest { get; protected set; }

       public BaseObject(RacingGame raceGame, String modelName, Vector3 initPos, float initScale, float initRot)
          : base(raceGame)
       {
          RaceGame = raceGame;
          ModelName = modelName;
          Position = initPos;
          Scale = initScale;
          Rotation = initRot;
       }

       public override void Initialize()
       {
           Boxes = new List<BoundingBox>();

           ModelData = RaceGame.ModelMgr.Find(ModelName);

           Orientation = Matrix.Identity;

           World = Matrix.CreateScale(Scale) * Orientation * Matrix.CreateTranslation(Position);

           CreateSpheres();

           base.Initialize();
       }

      protected void CreateSpheres()
      {
         Spheres = new BoundingSphere[ModelData.Meshes.Count];
         float radius;
         Vector3 center;
         Matrix[] transformations = new Matrix[ModelData.Bones.Count];
         ModelData.CopyAbsoluteBoneTransformsTo(transformations);
         Matrix localTrans;

         for (int i = 0; i < Spheres.Length; ++i)
         {
            localTrans =  transformations[ModelData.Meshes[i].ParentBone.Index] * World;
            center = Vector3.Transform(ModelData.Meshes[i].BoundingSphere.Center, localTrans);
            radius = Scale * RADIUS_MODIFICATOR * ModelData.Meshes[i].BoundingSphere.Radius;
            Spheres[i] = new BoundingSphere(center, radius);
         }

         CreateBigSphere();
      }

      private void CreateBigSphere()
      {
          if (ModelData.Meshes.Count > 1)
          {
              BigSphere = BoundingSphere.CreateMerged(Spheres[0], Spheres[1]);
              for (int i = 2; i < Spheres.Length; ++i)
                  BigSphere = BoundingSphere.CreateMerged(BigSphere, Spheres[i]);
          }
          else
          {
              BigSphere = Spheres[0];
          }
      }

      protected void CreateBoxes()
      {
          foreach(ModelMesh mesh in ModelData.Meshes)
          {
              BoundingBox box = CreateNewBox(mesh);

              Boxes.Add(box);
          }
      }

      protected BoundingBox CreateNewBox(ModelMesh mesh)
      {
          int lengthBuffer = mesh.VertexBuffer.SizeInBytes;
          int lengthVertex = VertexPositionNormalTexture.SizeInBytes;
          int verticesNb = lengthBuffer / lengthVertex;
          VertexPositionNormalTexture[] meshVertices = new VertexPositionNormalTexture[verticesNb];
          mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(meshVertices);

          Vector3[] verticesPosition = new Vector3[meshVertices.Length];

          for (int i = 0; i < verticesPosition.Length; i++)
          {
              verticesPosition[i] = meshVertices[i].Position;
              verticesPosition[i] = Vector3.Transform(verticesPosition[i], World);
          }

          return BoundingBox.CreateFromPoints(verticesPosition);
      }

      public BoundingSphere GetSphere(int i)
      {
         return Spheres[i];
      }

      public BoundingBox GetBox(int i)
      {
          return Boxes[i];
      }

      public int GetBoxesCount()
      {
          return Boxes.Count;
      }

      public int GetSpheresLength()
      {
          return Spheres.Length;
      }

      public override void Update(GameTime gameTime)
      {
          World = Matrix.CreateScale(Scale) * Orientation * Matrix.CreateTranslation(Position);

          base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         RaceGame.ModelDisplayer.Draw(gameTime);

         Matrix[] transformations = new Matrix[ModelData.Bones.Count];
         ModelData.CopyAbsoluteBoneTransformsTo(transformations);

         foreach (ModelMesh mesh in ModelData.Meshes)
         {
            Matrix localWorld = transformations[mesh.ParentBone.Index] * GetWorld();
            foreach (ModelMeshPart meshPart in mesh.MeshParts)
            {
               BasicEffect effect = (BasicEffect)meshPart.Effect;
               effect.EnableDefaultLighting();
               effect.Projection = RaceGame.GameCamera.Projection;
               effect.View = RaceGame.GameCamera.View;
               effect.World = localWorld;
            }
            mesh.Draw();
         }

         base.Draw(gameTime);
      }

      public virtual Matrix GetWorld()
      {
         return World;
      }

      public virtual Matrix GetWorldNoScale()
      {
          return World = Orientation * Matrix.CreateTranslation(Position);
      }

      public void SetInitialHeight(Terrain terrain)
      {
          Vector3 newPos = new Vector3(Position.X, Position.Y, Position.Z);

          terrain.GetHeight(newPos, out newPos.Y);
          Position = newPos;

          World = Matrix.CreateScale(Scale) * Orientation * Matrix.CreateTranslation(Position);

          CreateSpheres();
          CreateBoxes();
      }
   }
}
