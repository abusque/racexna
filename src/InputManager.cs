using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
   public class InputManager : Microsoft.Xna.Framework.GameComponent
   {
      private Game RaceGame;
      public GamePadState PreviousControllerState { get; private set; }
      public GamePadState CurrentControllerState { get; private set; }
      private Keys[] PreviousKeys { get; set; }
      private Keys[] CurrentKeys { get; set; }
      private KeyboardState KbState { get; set; }
      private MouseState PreviousMouseState { get; set; }
      private MouseState CurrentMouseState { get; set; }

      public InputManager(Game game)
         : base(game)
      {
         RaceGame = game;
      }

      public override void Initialize()
      {
         PreviousKeys = new Keys[0];
         CurrentKeys = new Keys[0];
         CurrentMouseState = Mouse.GetState();
         PreviousMouseState = CurrentMouseState;
         CurrentControllerState = GamePad.GetState(PlayerIndex.One);
         PreviousControllerState = CurrentControllerState;
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         PreviousKeys = CurrentKeys;
         KbState = Keyboard.GetState();
         CurrentKeys = KbState.GetPressedKeys();

         PreviousControllerState = CurrentControllerState;
         CurrentControllerState = GamePad.GetState(PlayerIndex.One);

         if (RaceGame.IsMouseVisible)
         {
            UpdateMouseState();
         }

         base.Update(gameTime);
      }

      private void UpdateMouseState()
      {
         PreviousMouseState = CurrentMouseState;
         CurrentMouseState = Mouse.GetState();

         Vector2 Position = GetMousePos();
      }

      public bool IsKbActive
      {
         get { return CurrentKeys.Length > 0; }
      }

      public bool IsNewKey(Keys key)
      {
         int keyCount = PreviousKeys.Length;
         bool newKey = IsKeyDown(key);
         int i = 0;

         while (i < keyCount && newKey)
         {
            newKey = PreviousKeys[i] != key;
            ++i;
         }

         return newKey;
      }

      public bool IsKeyDown(Keys key)
      {
          return KbState.IsKeyDown(key);
      }

      public bool IsNewRightClick()
      {
         return CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;
      }

      public bool IsNewLeftClick()
      {
         return CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
      }

      public Vector2 GetMousePos()
      {
         return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
      }
   }
}