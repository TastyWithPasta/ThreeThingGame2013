using Microsoft.Xna.Framework;
using System;

namespace RobsXNALibrary.Particles {
	public class FlameThrower:IParticle {
		public static string TexureName="";
		Vector2 _dir;
		float _speed=0;

		public FlameThrower(Vector2 pos, Vector2 dir, float speed) {
			if (TexureName=="")
				throw new Exception("Please set the TextureName field of the FlameThrower class.");
			Texture=Textures.Get(TexureName);
			Position=pos;
			_speed=speed;
			dir.Normalize();
			Angle=Helper.VecToAng(dir);
			_dir=dir;
			_dir.Y+=rand.Next(-1000,1000)/8000f;
			TTL=60;
			Colour=Color.Orange;
			Scale=0.5f;
		}

		public override void Update(GameTime gameTime) {
			if (TTL<30)
				Alpha=(float)TTL/30f;
			Position+=_dir*_speed*(float)gameTime.ElapsedGameTime.TotalSeconds*60f;
		}
	}
}
