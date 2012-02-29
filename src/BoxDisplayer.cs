using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RaceXNA
{
    public class BoxDisplayer : DrawableGameComponent
    {
        const int VERTICES = 10;
        const int TRIANGLES = 8;

        public Vector3[] Corners { get; private set; }
        public RacingGame RaceGame { get; private set; }
        public VertexPositionColor[] Vertices { get; private set; }
        public Matrix World { get; private set; }
        public Color DefaultColor { get; private set; }

        public BoxDisplayer(RacingGame raceGame, Vector3[] corners, Matrix world)
            :base(raceGame)
        {
            RaceGame = raceGame;
            Corners = corners;
            World = world;
            DefaultColor = Color.Red;
            Vertices = new VertexPositionColor[VERTICES];
            SetVertices();
        }

        public void SetVertices()
        {

            Vertices[0] = new VertexPositionColor(Corners[3], DefaultColor);//A                       D-----------F      
            Vertices[1] = new VertexPositionColor(Corners[0], DefaultColor);//B                      /.          /.                 
            Vertices[2] = new VertexPositionColor(Corners[7], DefaultColor);//C                     / .         / .       
            Vertices[3] = new VertexPositionColor(Corners[4], DefaultColor);//D                    B---------- H  .       
            Vertices[4] = new VertexPositionColor(Corners[6], DefaultColor);//E                    |  .        |  .               
            Vertices[5] = new VertexPositionColor(Corners[5], DefaultColor);//F                    |  C........|. E                 
            Vertices[6] = new VertexPositionColor(Corners[2], DefaultColor);//G                    | /         | /       
            Vertices[7] = new VertexPositionColor(Corners[1], DefaultColor);//H                    |/          |/                             
            Vertices[8] = new VertexPositionColor(Corners[3], DefaultColor);//A                    A-----------G
            Vertices[9] = new VertexPositionColor(Corners[0], DefaultColor);//B
        }

        public void SetCorners(Vector3[] corners)
        {
            Corners = corners;
        }

        public void SetWorld(Matrix world)
        {
            World = world;
        }

        public override void Draw(GameTime gameTime)
        {
            CullMode previousCullMode = RaceGame.GraphicsDevice.RenderState.CullMode;
            RaceGame.GraphicsDevice.RenderState.CullMode = CullMode.None;
            FillMode previousFillMode = RaceGame.GraphicsDevice.RenderState.FillMode;
            RaceGame.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            RaceGame.GraphicsDevice.VertexDeclaration = new VertexDeclaration(RaceGame.GraphicsDevice, VertexPositionColor.VertexElements);
            SetVertices();
            RaceGame.ModelDisplayer.Effect3D.World = World;
            RaceGame.ModelDisplayer.Effect3D.VertexColorEnabled = true;
            RaceGame.ModelDisplayer.Effect3D.Begin();
            Visible = true;
            foreach (EffectPass effectPass in RaceGame.ModelDisplayer.Effect3D.CurrentTechnique.Passes)
            {
                effectPass.Begin();
                RaceGame.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip,
                                                                           Vertices, 0, TRIANGLES);
                effectPass.End();
            }
            RaceGame.ModelDisplayer.Effect3D.End();
            RaceGame.ModelDisplayer.Effect3D.VertexColorEnabled = false;
            RaceGame.GraphicsDevice.RenderState.FillMode = previousFillMode;
            RaceGame.GraphicsDevice.RenderState.CullMode = previousCullMode;
            base.Draw(gameTime);
        }
    }
}