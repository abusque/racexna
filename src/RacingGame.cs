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
        public ResourceManager<Song> MusicsMgr { get; private set; }
        public FpsCounter FpsHandler { get; private set; }
        public FpsDisplay FpsDisplayer { get; private set; }
        public HUD HeadsUpDisplay { get; private set; }
        public AccelerationDisplay AccelerationDisplayer { get; private set; }
        public SpeedDisplay SpeedDisplayer { get; private set; }
        public ChasingCamera GameCamera { get; private set; }
        public Vehicle Car { get; private set; }
        public ModelDisplay ModelDisplayer { get; private set; }
        public Terrain Ground { get; private set; }

        public RacingGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = false;
            graphics.IsFullScreen = false;
            //MediaPlayer.IsRepeating = true;
        }

        protected override void Initialize()
        {
            FontMgr = new ResourceManager<SpriteFont>(this);
            TextureMgr = new ResourceManager<Texture2D>(this);
            ModelMgr = new ResourceManager<Model>(this);
            MusicsMgr = new ResourceManager<Song>(this);

            LoadAssets();

            //MediaPlayer.Play(MusicsMgr.Find("KalimariDesert"));

            FpsHandler = new FpsCounter(this, FPS_INTERVAL);
            FpsDisplayer = new FpsDisplay(this, "Pericles20");
            InputMgr = new InputManager(this);
            ModelDisplayer = new ModelDisplay(this);

            HeadsUpDisplay = new HUD(this);

            Car = new Vehicle(this, "L200-FBX", new Vector3(0, 0, -2), 0.01f, new Vector3(0, MathHelper.Pi, 0));
            Ground = new Terrain(this, new Vector3(-20, -15, 0), "Patate", "heightmap");

            GameCamera = Car.Camera;

            Components.Add(FpsHandler);
            Components.Add(InputMgr);
            Components.Add(ModelDisplayer);
            Components.Add(Car); //Mettre GameCamera apres Car pour eviter les problemes
            Components.Add(Ground);
            Components.Add(GameCamera);

            //Laisser FpsDisplayer a la fin de la liste pour eviter les problemes d'affichage
            Components.Add(HeadsUpDisplay);
            Components.Add(FpsDisplayer);

            base.Initialize();
        }

        private void LoadAssets()
        {
            FontMgr.Add("Fonts/Pericles20");
            ModelMgr.Add("Models/L200-FBX");
            ModelMgr.Add("Models/tree");
            TextureMgr.Add("Textures/grass1");
            TextureMgr.Add("Textures/asphalt1");
            TextureMgr.Add("Textures/sand1");
            TextureMgr.Add("Textures/Odometer");
            TextureMgr.Add("Textures/NeedleMap");
            TextureMgr.Add("Textures/heightmap");
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
            if (InputMgr.ControllerState.Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (InputMgr.IsNewKey(Keys.F2))
            {
                graphics.SynchronizeWithVerticalRetrace = !graphics.SynchronizeWithVerticalRetrace;
                IsFixedTimeStep = !IsFixedTimeStep;
            }

            base.Update(gameTime);
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.Clear(Color.Chocolate);
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
