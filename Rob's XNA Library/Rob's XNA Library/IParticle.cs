using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RobsXNALibrary {
	public abstract class IParticle {
		protected static Random rand=new Random();
		public Vector2 Position=Vector2.Zero;
		public Texture2D Texture=null;
		public float Angle=0;
		public int TTL=1000;
		public Color Colour=Color.White;
		public float Alpha=1;
		public float Scale=1;
		protected internal double Life=0;

		public abstract void Update(GameTime gameTime);
	}
}
