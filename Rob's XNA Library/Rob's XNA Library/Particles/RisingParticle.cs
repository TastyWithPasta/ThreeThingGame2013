using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace RobsXNALibrary.Particles {
	public class RisingParticle:IParticle {
		public static GraphicsDevice GraphicsDevice=null;
		Vector2 _dir;
		float _speed=0;

		public RisingParticle() {
			if (GraphicsDevice==null)
				throw new Exception("Please set the GraphicsDevice field of the Smoke class.");
			Texture=new Texture2D(GraphicsDevice,1,1);
			Texture.SetData<Color>(new Color[1]{new Color(255,255,255,255)});
			Position=Vector2.Zero;
			Position.X+=rand.Next(-10,10);
			Position.Y+=rand.Next(-10,10);
			_dir=new Vector2(0,-1);
			_speed=(float)rand.NextDouble()*2;
			TTL=rand.Next(60,180);
		}

		public RisingParticle(Vector2 pos):this(){
			Position=pos;
		}

		public override void Update(GameTime gameTime) {
			Position+=_dir*_speed*(float)gameTime.ElapsedGameTime.TotalSeconds*60f;
		}
	}
}
