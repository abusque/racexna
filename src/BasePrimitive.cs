using Microsoft.Xna.Framework;


namespace RaceXNA
{
   public abstract class PrimitiveDeBase : Microsoft.Xna.Framework.DrawableGameComponent
   {
      public RacingGame RaceGame { get; protected set; }
      protected int NbOfSubmits { get; set; }
      protected int NbOfTriangles { get; set; }
      protected Matrix World { get; set; }

      protected PrimitiveDeBase(RacingGame raceGame)
          : base(raceGame)
      {
         RaceGame = raceGame;
         World = Matrix.Identity;
      }

      public override void Initialize()
      {
         InitializeSubmits();
         base.Initialize();
      }
      public override void Update(GameTime gameTime)
      {
         base.Update(gameTime);
      }
     
      protected  abstract void InitializeSubmits();

      public override void Draw(GameTime gameTime)
      {
         base.Draw(gameTime);
      }

   }
}