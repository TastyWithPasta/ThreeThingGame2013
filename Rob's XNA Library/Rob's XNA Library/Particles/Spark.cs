using Microsoft.Xna.Framework;
using System;

namespace RobsXNALibrary.Particles {
	public class Spark:IParticle {
		public static string TexureName="";
		Vector2 _dir;
		float _speed=0;
		float _xMod=0;

		public Spark(Vector2 pos, float angle, int ttl) {
			if (TexureName=="")
				throw new Exception("Please set the TextureName field of the Spark class.");
			Texture=Textures.Get(TexureName);
			Position=pos;
			_speed=1;
			TTL=ttl;
			Angle=angle;
			_dir=Helper.AngToVec(Angle)*10;
			_xMod=rand.Next(-100,100)/100f;
		}

		public override void Update(GameTime gameTime) {
			if (TTL<15)
				Alpha=(float)TTL/15f;
			_dir.Y+=1f;
			_dir.X+=_xMod;
			if (_dir!=Vector2.Zero)
				_dir.Normalize();
			Position+=_dir*_speed;
			_speed+=0.1f;
		}
	}
}
