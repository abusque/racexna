using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace RaceXNA
{
   public class InputManager : Microsoft.Xna.Framework.GameComponent
   {
      Game RaceGame;
      Keys[] PreviousKeys { get; set; }
      Keys[] CurrentKeys { get; set; }
      public KeyboardState KbState { get; private set; }
      public MouseState PreviousMouseState { get; private set; }
      public MouseState CurrentMouseState { get; private set; }

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
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         PreviousKeys = CurrentKeys;
         KbState = Keyboard.GetState();
         CurrentKeys = KbState.GetPressedKeys();

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

         if (CurrentMouseState.LeftButton == ButtonState.Pressed)
         {
            Vector2 Position = GetMousePos();
         }
      }

      public bool IsKbActive
      {
         get { return CurrentKeys.Length > 0; }
      }

      public bool IsNewKey(Keys key)
      {
         int keyCount = PreviousKeys.Length;
         bool newKey = IsHeld(key);
         int i = 0;

         while (i < keyCount && newKey)
         {
            newKey = PreviousKeys[i] != key;
            ++i;
         }

         return newKey;
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

      public bool IsHeld(Keys touche)
      {
          return KbState.IsKeyDown(touche);
      }
   }
}