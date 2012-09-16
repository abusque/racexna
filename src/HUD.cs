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
    public class HUD : Microsoft.Xna.Framework.GameComponent
    {
        RacingGame RaceGame { get; set; }

        AccelerationDisplay AccelerationDisplayer { get; set; }
        SpeedDisplay SpeedDisplayer { get; set; }
        FpsDisplay FpsDisplayer { get; set; }
        Chronometer GameChronometer { get; set; }
        Odometer GameOdometer { get; set; }

        public bool IsProgramerDisplay { get; set; }

        public HUD(RacingGame game)
            : base(game)
        {
            RaceGame = game;
        }

        public override void Initialize()
        {
            IsProgramerDisplay = false;

            AccelerationDisplayer = new AccelerationDisplay(RaceGame, "Pericles20");
            SpeedDisplayer = new SpeedDisplay(RaceGame, "Pericles20");
            FpsDisplayer = new FpsDisplay(RaceGame, "Pericles20");
            GameChronometer = new Chronometer(RaceGame, "Pericles20");
            GameOdometer = new Odometer(RaceGame, "Odometer", "NeedleMap",6,6);

            RaceGame.Components.Add(AccelerationDisplayer);
            RaceGame.Components.Add(SpeedDisplayer);
            RaceGame.Components.Add(FpsDisplayer);
            RaceGame.Components.Add(GameChronometer);
            RaceGame.Components.Add(GameOdometer);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (RaceGame.InputMgr.IsNewKey(Keys.F1))
                IsProgramerDisplay = !IsProgramerDisplay;
            base.Update(gameTime);
        }

        public void ToggleChronometer()
        {
            GameChronometer.Enabled = !GameChronometer.Enabled;
        }
    }
}