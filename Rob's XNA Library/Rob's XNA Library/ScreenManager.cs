using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RobsXNALibrary {
	public class ScreenManager:DrawableGameComponent {
		const byte DEFAULT_FADE_AMOUNT=5;
		private Game _game;
		private Dictionary<string,int> _screenRef=new Dictionary<string,int>();
		private List<GameScreen> _screens=new List<GameScreen>();
		private SpriteBatch spriteBatch;
		private GameScreen _updateSkip=null;
		private Texture2D pixel;
		private byte _blackAlpha=0;
		private byte _fadeAmount=DEFAULT_FADE_AMOUNT;
		private int _fadeDir=1;
		private Color _fadeColour;
		private Rectangle _viewRect;
		private Timer _fadeTimer;
		private string _nextScreen;
		private bool _contentLoaded=false;
		//private GameScreen _lastScreen;

		/// <summary>
		/// ScreenManager constructor. Automatically adds itself to game components
		/// </summary>
		/// <param name="game">Parent game of this ScreenManager</param>
		public ScreenManager(Game game):base(game) {
			_game=game;
			_game.Components.Add(this);
			_fadeColour=new Color(0,0,0,_blackAlpha);
			_fadeTimer=new Timer(25,0,UpdateFade);
			_fadeTimer.Enabled=false;
			Timers.Add(_fadeTimer);
		}

		protected override void LoadContent() {
			spriteBatch=new SpriteBatch(_game.GraphicsDevice);
			_viewRect=new Rectangle(0,0,_game.GraphicsDevice.Viewport.Width,_game.GraphicsDevice.Viewport.Height);
			//pixel=Content.Load<Texture2D>("Textures\\pixel");
			pixel=new Texture2D(GraphicsDevice,1,1);
			pixel.SetData<Color>(new Color[1]{new Color(255,255,255,255)});
			foreach (GameScreen s in _screens){
				s.spriteBatch=spriteBatch;
				s.LoadContent();
			}
			_contentLoaded=true;
			base.LoadContent();
		}

		private void FadeToBlack() {
			//if (_fadeTimer.Enabled)
			//	return;
			//_fadeToBlack=true;
			_blackAlpha=0;
			_fadeColour.A=_blackAlpha;
			_fadeDir=1;
			_fadeTimer.Enabled=true;
		}

		private void UpdateFade() {
			if (_fadeDir>0) {
				if (_blackAlpha>=255) {
					_fadeDir=-1;
					SwitchToScreen(_nextScreen);
				}
			} else
				if (_blackAlpha<=0)
					_fadeTimer.Enabled=false;
			//_blackAlpha+=(byte)(_fadeAmount*_fadeDir);
			_blackAlpha=(byte)MathHelper.Clamp(_blackAlpha+(_fadeAmount*_fadeDir),0,255);
			_fadeColour.A=_blackAlpha;
		}

		public override void Update(GameTime gameTime) {
			if (!_game.IsActive)
				return;
			Input.Update();
			Timers.Update(gameTime.ElapsedGameTime);
			foreach (GameScreen s in _screens) {
				if (s==_updateSkip)
					continue;
				s.UpdateAlways(gameTime);
				/*if (s.Enabled)
					s.Update(gameTime);*/
			}
			int i=_screens.Count-1;
			while (!_screens[i].Enabled&&i>0){
				i--;
			}
			_screens[i].Update(gameTime);
			_updateSkip=null;
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
			if (!_game.IsActive)
				return;
			foreach (GameScreen s in _screens)
				if (s.Enabled)
					s.Draw(gameTime);
			if (_fadeTimer.Enabled){
				spriteBatch.Begin();
				spriteBatch.Draw(pixel,_viewRect,_fadeColour);
				spriteBatch.End();
			}
			base.Draw(gameTime);
		}

		/// <summary>
		/// Add a new screen
		/// </summary>
		/// <param name="screen">GameScreen to add</param>
		/// <param name="name">Name for the screen</param>
		/// <param name="enable">Enable the screen</param>
		public void AddScreen(GameScreen screen,string name,bool enable) {
			screen.spriteBatch=spriteBatch;
			screen.Manager=this;
			_screens.Add(screen);
			_screenRef[name]=_screens.Count-1;
			screen.Initialise();
			if (_contentLoaded)
				screen.LoadContent();
			if (!enable)
				screen.Disable();
		}

		/// <summary>
		/// Add a new screen
		/// </summary>
		/// <param name="screen">GameScreen to add</param>
		/// <param name="name">Name for the screen</param>
		public void AddScreen(GameScreen screen,string name) {
			AddScreen(screen,name,false);
		}

		/// <summary>
		/// Does a screen exist?
		/// </summary>
		/// <param name="name">Screen name</param>
		/// <returns>True if it does, false if it doesn't</returns>
		public bool ScreenExists(string name) {
			return _screenRef.ContainsKey(name);
		}

		/// <summary>
		/// Get a screen from its name
		/// </summary>
		/// <param name="name">Name of the screen</param>
		/// <returns>Associated GameScreen</returns>
		public GameScreen GetScreen(string name) {
			return _screens[_screenRef[name]];
		}

		/// <summary>
		/// Enable a GameScreen
		/// </summary>
		/// <param name="name">Screen name</param>
		public void EnableScreen(string name) {
			_updateSkip=_screens[_screenRef[name]];
			_screens[_screenRef[name]].Enable();
		}

		/// <summary>
		/// Disable a GameScreen
		/// </summary>
		/// <param name="name">Screen name</param>
		public void DisableScreen(string name) {
			_screens[_screenRef[name]].Disable();
			//if (_screenRef[name]>0)
				//((GameScreen)_screens[_screenRef[name]-1]).Enable();
		}

		/// <summary>
		/// Switch to a GameScreen
		/// </summary>
		/// <param name="name">Screen name</param>
		public void SwitchToScreen(string name) {
			foreach (KeyValuePair<string,int> s in _screenRef)
				if (!_screens[s.Value].Sticky&&s.Key!=name&&_screens[s.Value].Enabled)
					DisableScreen(s.Key);
			EnableScreen(name);
		}

		/// <summary>
		/// Fade through black to a screen
		/// </summary>
		/// <param name="name">Screen name</param>
		public void FadeToScreen(string name) {
			if (_fadeTimer.Enabled) {
			//	SwitchToScreen(name);//Do this to avoid fade glitchy things;
			//	return;
			}
			_nextScreen=name;
			FadeToBlack();
		}

		/// <summary>
		/// Fade through black to a screen, with custom speed
		/// </summary>
		/// <param name="name">Screen name</param>
		/// <param name="fadeamount">Fade speed</param>
		public void FadeToScreen(string name,byte fadeamount) {
			if (_fadeTimer.Enabled) {
			//	SwitchToScreen(name);//Do this to avoid fade glitchy things;
				//return;
			}
			_fadeAmount=fadeamount;
			FadeToScreen(name);
		}

		/// <summary>
		/// Parent Game ContentManager
		/// </summary>
		public ContentManager Content {
			get {
				return _game.Content;
			}
		}

		/// <summary>
		/// Parent Game Window
		/// </summary>
		public GameWindow Window {
			get {
				return _game.Window;
			}
		}

		/// <summary>
		/// Parent Game GraphicsDevice
		/// </summary>
		public new GraphicsDevice GraphicsDevice {
			get {
				return _game.GraphicsDevice;
			}
		}

		/// <summary>
		/// Parent Game
		/// </summary>
		public new Game Game {
			get {
				return _game;
			}
		}

		/// <summary>
		/// View rectangle
		/// </summary>
		public Rectangle ViewRect {
			get {
				return _viewRect;
			}
		}
	}
}
