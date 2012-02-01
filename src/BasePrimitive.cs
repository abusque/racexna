using Microsoft.Xna.Framework;


namespace RaceXNA
{
   public abstract class BasePrimitive : Microsoft.Xna.Framework.DrawableGameComponent
   {
      public RacingGame RaceGame { get; protected set; }
      protected int NbVertices { get; set; }
      protected int NbTriangles { get; set; }
      protected Matrix World { get; set; }

      protected BasePrimitive(RacingGame raceGame)
          : base(raceGame)
      {
         RaceGame = raceGame;
         World = Matrix.Identity;
      }

      public override void Initialize()
      {
         InitializeVertices();
         base.Initialize();
      }
      public override void Update(GameTime gameTime)
      {
         base.Update(gameTime);
      }
     
      protected  abstract void InitializeVertices();

      public override void Draw(GameTime gameTime)
      {
         base.Draw(gameTime);
      }

   }
}