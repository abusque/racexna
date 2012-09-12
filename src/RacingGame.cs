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
        public ResourceManager<Song> MusicMgr { get; private set; }
        public FpsCounter FpsHandler { get; private set; }
        public FpsDisplay FpsDisplayer { get; private set; }
        public HUD HeadsUpDisplay { get; private set; }
        public AccelerationDisplay AccelerationDisplayer { get; private set; }
        public SpeedDisplay SpeedDisplayer { get; private set; }
        public SpringCamera GameCamera { get; private set; }
        //public FreeCamera GameCamera { get; private set; }
        public Vehicle Car { get; private set; }
        public ModelDisplay ModelDisplayer { get; private set; }
        Terrain Ground { get; set; }
        public Track GameTrack { get; private set; }
        public bool Paused { get; private set; }
        public Rectangle PausedRectangle { get; private set; }

        public RacingGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            IsMouseVisible = false;
            graphics.IsFullScreen = false;
            //MediaPlayer.IsRepeating = true;
        }

        protected override void Initialize()
        {
            FontMgr = new ResourceManager<SpriteFont>(this);
            TextureMgr = new ResourceManager<Texture2D>(this);
            ModelMgr = new ResourceManager<Model>(this);
            MusicMgr = new ResourceManager<Song>(this);

            LoadAssets();

            Texture2D pauseTex = TextureMgr.Find("pause");
            PausedRectangle = new Rectangle((Window.ClientBounds.Width - pauseTex.Width) / 2, (Window.ClientBounds.Height - pauseTex.Height) / 2,
                                            pauseTex.Width, pauseTex.Height);

            MediaPlayer.Play(MusicMgr.Find("RenditionFull"));

            FpsHandler = new FpsCounter(this, FPS_INTERVAL);
            FpsDisplayer = new FpsDisplay(this, "Pericles20");
            InputMgr = new InputManager(this);
            ModelDisplayer = new ModelDisplay(this);

            HeadsUpDisplay = new HUD(this);

            Car = new Vehicle(this, "L200-FBX", new Vector3(0, 0, -2), 0.01f, MathHelper.Pi);
            Ground = new Terrain(this, new Vector3(-20, 0, 0), "colormap", "heightmap", 10.0f, 0.5f);
            GameTrack = new Track(this, Ground);

            GameCamera = Car.Camera;
            //GameCamera = new FreeCamera(this, new Vector3(0, 0, 2), Vector3.Zero, Vector3.Zero);

            Components.Add(FpsHandler);
            Components.Add(InputMgr);
            Components.Add(ModelDisplayer);
            Components.Add(Car); //Mettre GameCamera apres Car pour eviter les problemes
            Components.Add(GameTrack);
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
            TextureMgr.Add("Textures/colormap");
            TextureMgr.Add("Textures/flatmap");
            TextureMgr.Add("Textures/pause");
            MusicMgr.Add("Music/RenditionFull");
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            CheckInputs();

            if (Paused)
            {
                InputMgr.Update(gameTime);
                return;
            }

            base.Update(gameTime);
        }

        private void CheckInputs()
        {
            if (InputMgr.ControllerState.Buttons.Start == ButtonState.Pressed && !(InputMgr.PreviousControllerState.Buttons.Start == ButtonState.Pressed))
            {
                Paused = !Paused;

                if (Paused)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Resume();
            }
            if (InputMgr.ControllerState.Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (InputMgr.IsNewKey(Keys.F2))
            {
                graphics.SynchronizeWithVerticalRetrace = !graphics.SynchronizeWithVerticalRetrace;
                IsFixedTimeStep = !IsFixedTimeStep;
            }
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            if(Paused)
                spriteBatch.Draw(TextureMgr.Find("pause"), PausedRectangle, Color.White);

            base.Draw(gameTime);
        }

        protected override void EndDraw()
        {
            spriteBatch.End();

            base.EndDraw();
        }
    }
}
