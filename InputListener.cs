using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Coursework
{
    class InputListener
    {
        // Current and previous keyboard states
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }

        // List of keys to check for
        public HashSet<Keys> KeyList;

        //Keyboard event handlers
        //key is down
        public event EventHandler<KeyboardEventArgs> OnKeyDown = delegate { };
        //key was up and is now down
        public event EventHandler<KeyboardEventArgs> OnKeyPressed = delegate { };
        //key was down and is now up
        public event EventHandler<KeyboardEventArgs> OnKeyUp = delegate { };

        public InputListener()
        {
            CurrentKeyboardState = Keyboard.GetState();
            PrevKeyboardState = CurrentKeyboardState;
            KeyList = new HashSet<Keys>();
        }
        public void AddKey(Keys key)
        {
            KeyList.Add(key);
        }
        public void Update()
        {
            PrevKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            FireKeyboardEvents();
        }
        private void FireKeyboardEvents()
        {
            // Check through each key in the key list
            foreach (Keys key in KeyList)
            {
                // Is the key currently down?
                if (CurrentKeyboardState.IsKeyDown(key))
                {
                    // Fire the OnKeyDown event
                    if (OnKeyDown != null)
                        OnKeyDown(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }
                // Has the key been released? (Was down and is now up)
                if (PrevKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key))
                {
                    // Fire the OnKeyUp event
                    if (OnKeyUp != null)
                        OnKeyUp(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }
                // Has the key been pressed? (was up and is now down)
                if (PrevKeyboardState.IsKeyUp(key) && CurrentKeyboardState.IsKeyDown(key))
                {
                    // Fire the OnKeyUp event
                    if (OnKeyPressed != null)
                        OnKeyPressed(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }
            }
        }
    }
}
