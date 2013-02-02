using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace RetroSeriesTennis
{
    public static class Input
    {
        public static List<GestureSample> Gestures;

        static Input()
        {
            Gestures = new List<GestureSample>();
        }

        public static String GetKeyEntryMenu()
        {
            
            Gestures.Clear();
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                return "Enter";
            }

            else

            foreach (GestureSample gesture in Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    return "Enter";
                }
            }
             return "N/A";
        }

        public static String isGamePaused()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.P))
            {
                return "Pause";
            }

            else if (keyboardState.IsKeyDown(Keys.U))
            {
                return "Unpause";
            }
            else return "N/A";
        }
        public static Vector2 GetKeyboardInputDirection(PlayerIndex playerIndex)
        {
            Vector2 direction = Vector2.Zero;
            KeyboardState keyboardState = Keyboard.GetState(playerIndex);
            if (playerIndex == PlayerIndex.One)
            {
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    direction.Y += -1;
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    direction.Y += 1;
                }
                
            }

            if (playerIndex == PlayerIndex.Two)
            {
                if (keyboardState.IsKeyDown(Keys.Up))
                    direction.Y += -1;
                if (keyboardState.IsKeyDown(Keys.Down))
                    direction.Y += 1;
            }
            return direction;
        }

        public static void ProcessTouchInput(out Vector2 paddle1Velocity, out Vector2 paddle2Velocity)
        {
            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
            paddle1Velocity = Vector2.Zero;
            paddle2Velocity = Vector2.Zero;

            foreach (GestureSample gestureSample in Gestures)
            {
                if (gestureSample.GestureType == GestureType.FreeDrag)
                {
                    if (gestureSample.Position.X >= 0 && gestureSample.Position.X <= Game1.ScreenWidth / 2)
                    {
                        paddle1Velocity.Y += gestureSample.Delta.Y;
                    }
                    if (gestureSample.Position.X >= Game1.ScreenWidth / 2 && gestureSample.Position.X <= Game1.ScreenWidth)
                    {
                        paddle2Velocity.Y += gestureSample.Delta.Y;
                    }
                }

            }
        }
    }
}
