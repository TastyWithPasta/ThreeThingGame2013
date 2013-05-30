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
using RobsXNALibrary;

namespace TTG2013 {
	public class Main:Microsoft.Xna.Framework.Game {
		public const int MAX_TIMERS=1000;
		public const int BASE_RES_WIDTH=1200;
		public const int BASE_RES_HEIGHT=720;
		private static float _camScale;
		GraphicsDeviceManager _graphics;
		SpriteBatch spriteBatch;
		ScreenManager sm;
		MainScreen _main;

		public Main() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			sm=new ScreenManager(this);
			Timers.Initialise(MAX_TIMERS);
			//set up the window to be full screen at full res.
			DisplayMode dm=GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			//int x=dm.Width;
			//int y=dm.Height;
			int x=1280;
			int y=720;
			_graphics.PreferredBackBufferWidth=x;
			_graphics.PreferredBackBufferHeight=y;
			_camScale=Math.Min((float)x/(float)BASE_RES_WIDTH, (float)y/(float)BASE_RES_HEIGHT);
			//IsFixedTimeStep=false;
			_graphics.IsFullScreen=false;
			//TargetElapsedTime=new TimeSpan(0,0,0,0,100);
			//_graphics.SynchronizeWithVerticalRetrace=false;
			IsMouseVisible=true;
		}

		protected override void Initialize() {
			base.Initialize();
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Textures.Load(Content);
			sm.AddScreen(new MainScreen(),"main");
		}

		protected override void UnloadContent() {
		}

		protected override void Update(GameTime gameTime) {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();
			Input.Update();
			Timers.Update(gameTime.ElapsedGameTime);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
			base.Draw(gameTime);
		}

		public static float CamScale {
			get {
				return _camScale;
			}
		}

		public int GameWidth {
			get {
				return _graphics.PreferredBackBufferWidth;
			}
		}

		public int GameHeight {
			get {
				return _graphics.PreferredBackBufferHeight;
			}
		}
	}
}
