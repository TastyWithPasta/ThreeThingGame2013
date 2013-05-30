using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace RobsXNALibrary {
	public static class Textures {
		private static Dictionary<string,Texture2D> _textures=new Dictionary<string,Texture2D>();

		public static void Load(ContentManager Content) {
			_textures["pixel"]=Content.Load<Texture2D>("pixel");
		}

		public static void Add(string name,Texture2D texture) {
			_textures[name.ToLower()]=texture;
		}

		public static Texture2D Get(string name) {
			return _textures[name.ToLower()];
		}
	}

	public static class Sounds {
		private static Dictionary<string,SoundEffect> _sounds=new Dictionary<string,SoundEffect>();
		public static void Load(ContentManager Content) {
		}
		public static void Add(string name,SoundEffect sound) {
			_sounds[name.ToLower()]=sound;
		}
		public static SoundEffect Get(string name) {
			return _sounds[name.ToLower()];
		}
	}

	public static class Fonts {
		private static Dictionary<string,SpriteFont> _fonts=new Dictionary<string,SpriteFont>();

		public static void Load(ContentManager Content) {
			_fonts["pixel"]=Content.Load<SpriteFont>("pixel");
		}

		public static void Add(string name,SpriteFont font) {
			_fonts[name.ToLower()]=font;
		}

		public static SpriteFont Get(string name) {
			return _fonts[name.ToLower()];
		}
	}
}
