using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RobsXNALibrary {
	public static class Textures {
		private static Dictionary<string,Texture2D> _textures=new Dictionary<string,Texture2D>();

		/// <summary>
		/// Load the textures from a ContentManager
		/// </summary>
		/// <param name="Content">ContentManager to use</param>
		public static void Load(ContentManager Content) {
			foreach (string f in Directory.GetFiles(Content.RootDirectory + "\\Textures"))
				Textures.Add(Path.GetFileNameWithoutExtension(f).ToLower(),Content.Load<Texture2D>("Textures\\" + Path.GetFileNameWithoutExtension(f)));
		}

		/// <summary>
		/// Add a texture to the dictionary
		/// </summary>
		/// <param name="name">Name to use</param>
		/// <param name="texture">Texture2D to use</param>
		public static void Add(string name,Texture2D texture) {
			_textures[name.ToLower()]=texture;
		}

		/// <summary>
		/// Get a texture from the dictionary
		/// </summary>
		/// <param name="name">Name of the texture</param>
		/// <returns>Texture associated with that name</returns>
		public static Texture2D Get(string name) {
			return _textures[name.ToLower()];
		}
	}
}
