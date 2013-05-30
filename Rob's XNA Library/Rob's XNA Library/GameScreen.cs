using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RobsXNALibrary {
	public abstract class GameScreen {
		protected static Random rand=new Random();
		protected internal SpriteBatch spriteBatch;
		protected bool _enabled=true;
		private bool _sticky=false;
		private ScreenManager _screenManager;

		public void Enable() {
			_enabled=true;
			OnEnable();
		}

		public void Disable() {
			_enabled=false;
			OnDisable();
		}

		public virtual void Initialise() {
		}

		public virtual void LoadContent() {
		}

		public virtual void Update(GameTime gameTime) {
		}

		public virtual void UpdateAlways(GameTime gameTime) {
		}

		public virtual void Draw(GameTime gameTime) {
		}

		public virtual void OnEnable() {
		}

		public virtual void OnDisable() {
		}

		public bool Enabled {
			get {
				return _enabled;
			}
		}

		public bool Sticky {
			get {
				return _sticky;
			}
			set {
				_sticky=value;
			}
		}

		public ScreenManager Manager {
			get {
				return _screenManager;
			}
			set {
				_screenManager=value;
			}
		}

		public ContentManager Content {
			get {
				return _screenManager.Content;
			}
		}

		public GameWindow Window {
			get {
				return Manager.Game.Window;
			}
		}

		public GraphicsDevice GraphicsDevice {
			get {
				return Manager.Game.GraphicsDevice;
			}
		}

		public Game Game {
			get {
				return Manager.Game;
			}
		}

		public Rectangle ViewRect {
			get {
				return Manager.ViewRect;
			}
		}
	}
}
