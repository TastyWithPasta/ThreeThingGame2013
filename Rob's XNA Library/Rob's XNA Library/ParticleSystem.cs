using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RobsXNALibrary {
	public class ParticleSystem {
		private List<IParticle> _particles=new List<IParticle>();
		
		public bool RelativePositioning=false;
		public Vector2 Position=Vector2.Zero;
		public float Angle=0;

		public ParticleSystem(Vector2 pos,bool relativePositioning) {
			Position=pos;
			RelativePositioning=relativePositioning;
		}

		public void AddParticle(IParticle particle) {
			_particles.Add(particle);
		}

		public void Update(GameTime gameTime) {
			for (int i=0;i<_particles.Count;i++) {
				_particles[i].Life+=gameTime.ElapsedGameTime.TotalMilliseconds;
				_particles[i].Update(gameTime);
				if (_particles[i].Life>=_particles[i].TTL) {
					_particles.RemoveAt(i);
					i--;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			Vector2 drawPos=Vector2.Zero;
			Vector2 origin=Vector2.Zero;
			foreach (IParticle p in _particles){
				drawPos=p.Position;
				origin.X=p.Texture.Width/2;
				origin.Y=p.Texture.Height/2;
				if (RelativePositioning)
					drawPos+=Position;
				spriteBatch.Draw(p.Texture,drawPos,null,p.Colour*p.Alpha,p.Angle+Angle,origin,p.Scale,SpriteEffects.None,0);
			}
		}

		public int ParticleCount {
			get {
				return _particles.Count;
			}
		}
	}
}
