using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace RaceXNA
{
    public class RacingGame : Microsoft.Xna.Framework.Game
    {
        const float FPS_INTERVAL = 1.0f;

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public InputManager InputMgr { get; private set; }
        public ResourceManager<Texture2D> TextureMgr { get; private set; }
        public ResourceManager<SpriteFont> FontMgr { get; private set; }
        public ResourceManager<Model> ModelMgr { get; private set; }
        public FpsCounter FpsHandler { get; private set; }
        public FpsDisplay FpsDisplayer { get; private set; }
        public FreeCamera GameCamera { get; private set; }
        public BaseObject Car { get; private set; }
        public TexturedSurface GrassGround { get; private set; }
        public ModelDisplay ModelDisplayer { get; private set; }


        public RacingGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = false;
            graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            FontMgr = new ResourceManager<SpriteFont>(this);
            TextureMgr = new ResourceManager<Texture2D>(this);
            ModelMgr = new ResourceManager<Model>(this);

            LoadAssets();

            FpsHandler = new FpsCounter(this, FPS_INTERVAL);
            FpsDisplayer = new FpsDisplay(this, "Pericles20");
            InputMgr = new InputManager(this);
            ModelDisplayer = new ModelDisplay(this);
            GameCamera = new FreeCamera(this, Vector3.Zero, Vector3.Zero, Vector3.Up);

            Car = new BaseObject(this, "L200-FBX", new Vector3(0, 0, -2), 0.01f, Vector3.Zero);
            GrassGround = new TexturedSurface(this, new Vector3(0, 0, -10), new Vector3(10,0,-10), new Vector2(20, 20), "grass1", true);

            Components.Add(FpsHandler);
            Components.Add(InputMgr);
            Components.Add(GameCamera);
            Components.Add(GrassGround);
            Components.Add(ModelDisplayer);
            Components.Add(Car);

            //Laisser FpsDisplayer a la fin de la liste pour eviter les problemes d'affichage
            Components.Add(FpsDisplayer);

            base.Initialize();
        }

        private void LoadAssets()
        {
            FontMgr.Add("Fonts/Pericles20");
            ModelMgr.Add("Models/L200-FBX");
            TextureMgr.Add("Textures/grass1");
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void EndDraw()
        {
            spriteBatch.End();

            base.EndDraw();
        }
    }
}
