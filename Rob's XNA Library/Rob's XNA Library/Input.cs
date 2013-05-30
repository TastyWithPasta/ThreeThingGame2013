using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RobsXNALibrary {
	public static class Input {
		private static MouseState OldMousestate,CurrentMouseState=Mouse.GetState();
		private static KeyboardState OldKeyboardState,CurrentKeyboardState=Keyboard.GetState();
		private static string TypedString="";
		private static DateTime LastLeftClickTime=DateTime.Now;
		public static int LeftClicks=0;
		private static DateTime LastRightClickTime=DateTime.Now;
		public static int RightClicks=0;
		private static DateTime LastMiddleClickTime=DateTime.Now;
		public static int MiddleClicks=0;

		public static void Update() {
			OldMousestate=CurrentMouseState;
			CurrentMouseState=Mouse.GetState();
			OldKeyboardState=CurrentKeyboardState;
			CurrentKeyboardState=Keyboard.GetState();
			foreach (Keys key in PressedKeys) {
			//if (PressedKeys.Length>0){
			//	Keys key=PressedKeys[0];
				string typed="";
				switch (key) {
					case Keys.Space:
						typed+=" ";
						break;
					case Keys.OemPeriod:
						typed+=".";
						break;
					case Keys.OemComma:
						typed+=",";
						break;
					case Keys.Back:
						if (TypedString.Length>0)
							TypedString=TypedString.Remove(TypedString.Length-1,1);
						break;
					case Keys.Enter:
						break;
					case Keys.LeftShift:
						break;
					case Keys.RightShift:
						break;
					default:
						string str=key.ToString();
						switch (str.Length) {
							case 1:
								typed+=str;
								break;
							case 2:
								if (str[0]=='D')
									typed+=str[1];
								break;
						}
						break;
				}
				if (!(KeyDown(Keys.LeftShift)||KeyDown(Keys.RightShift)))
					typed=typed.ToLower();
				TypedString+=typed;
			}
			if ((DateTime.Now-LastLeftClickTime).Milliseconds>500||LeftClicks>=2)
				LeftClicks=0;
			if (LeftClicked) {
				LastLeftClickTime=DateTime.Now;
				LeftClicks++;
			}
			if ((DateTime.Now-LastRightClickTime).Milliseconds>500||RightClicks>=2)
				RightClicks=0;
			if (RightClicked) {
				LastRightClickTime=DateTime.Now;
				RightClicks++;
			}
			if ((DateTime.Now-LastMiddleClickTime).Milliseconds>500||MiddleClicks>=2)
				MiddleClicks=0;
			if (MiddleClicked) {
				LastMiddleClickTime=DateTime.Now;
				MiddleClicks++;
			}
		}

		public static bool LeftClicked {
			get {
				return (OldMousestate.LeftButton==ButtonState.Pressed&&CurrentMouseState.LeftButton==ButtonState.Released);
			}
		}

		public static bool DoubleLeftClicked {
			get {
				return (LeftClicks>=2);
			}
		}

		public static bool RightClicked {
			get {
				return (OldMousestate.RightButton==ButtonState.Pressed&&CurrentMouseState.RightButton==ButtonState.Released);
			}
		}

		public static bool DoubleRightClicked {
			get {
				return (RightClicks>=2);
			}
		}

		public static bool MiddleClicked {
			get {
				return (OldMousestate.MiddleButton==ButtonState.Pressed&&CurrentMouseState.MiddleButton==ButtonState.Released);
			}
		}

		public static bool DoubleMiddleClicked {
			get {
				return (MiddleClicks>=2);
			}
		}

		public static bool LeftDown{
			get {
				return (CurrentMouseState.LeftButton==ButtonState.Pressed);
			}
		}

		public static bool RightDown {
			get {
				return (CurrentMouseState.RightButton==ButtonState.Pressed);
			}
		}

		public static bool MiddleDown {
			get {
				return (CurrentMouseState.MiddleButton==ButtonState.Pressed);
			}
		}

		public static bool WasLeftDown {
			get {
				return (OldMousestate.LeftButton==ButtonState.Released&&CurrentMouseState.LeftButton==ButtonState.Pressed);
			}
		}

		public static bool WasRightDown {
			get {
				return (OldMousestate.RightButton==ButtonState.Released&&CurrentMouseState.RightButton==ButtonState.Pressed);
			}
		}

		public static bool WasMiddleDown {
			get {
				return (OldMousestate.MiddleButton==ButtonState.Released&&CurrentMouseState.MiddleButton==ButtonState.Pressed);
			}
		}

		public static int MouseWheel {
			get {
				return (CurrentMouseState.ScrollWheelValue-OldMousestate.ScrollWheelValue)/120;
			}
		}

		public static int MouseX {
			get {
				//return (int)(((CurrentMouseState.X-(game.GraphicsDevice.Viewport.Width/2))+(game.cam.Pos.X*(2*game.cam.Zoom)))/game.cam.Zoom);
				return CurrentMouseState.X;
			}
		}

		public static int MouseY {
			get {
				//return (int)(((CurrentMouseState.Y-(game.GraphicsDevice.Viewport.Height/2))+(game.cam.Pos.Y*(2*game.cam.Zoom)))/game.cam.Zoom);
				return CurrentMouseState.Y;
			}
		}

		public static Vector2 MousePos {
			get{
				return new Vector2(MouseX,MouseY);
			}
		}

		public static Rectangle MouseRect {
			get {
				return new Rectangle(MouseX,MouseY,1,1);
			}
		}

		public static Point MousePoint {
			get {
				return new Point(MouseX,MouseY);
			}
		}

		public static bool KeyDown(Keys key) {
			return CurrentKeyboardState.IsKeyDown(key);
		}

		public static bool KeyUp(Keys key) {
			return CurrentKeyboardState.IsKeyUp(key);
		}

		public static bool KeyPressed(Keys key) {
			return (CurrentKeyboardState.IsKeyDown(key)&&OldKeyboardState.IsKeyUp(key));
		}

		public static Keys[] PressedKeys {
			get {
				if (CurrentKeyboardState==OldKeyboardState)
					return new Keys[0];
				return Keyboard.GetState().GetPressedKeys();
			}
		}

		public static string PressedKeysString {
			get {
				string ret="";
				foreach (Keys key in PressedKeys)
					ret+=key.ToString();
				return ret;
			}
		}

		public static string GetTypedString() {
			return TypedString;
		}

		public static void ClearTypedString() {
			TypedString="";
		}

		public static bool MouseMovedSinceLastFrame()
		{
			return (OldMousestate.X != CurrentMouseState.X) ||
				(OldMousestate.Y != CurrentMouseState.Y);
		}

		public static bool KeyReleased(Keys key)
		{
			return (CurrentKeyboardState.IsKeyUp(key) && OldKeyboardState.IsKeyDown(key));
		}
	}
}