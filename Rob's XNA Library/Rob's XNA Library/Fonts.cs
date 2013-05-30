using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace RobsXNALibrary {
	public static class Fonts {
		private static Dictionary<string,SpriteFont> _fonts=new Dictionary<string,SpriteFont>();

		/// <summary>
		/// Load the SpriteFonts from a ContentManager
		/// </summary>
		/// <param name="content"></param>
		public static void Load(ContentManager content) {
			foreach (string f in Directory.GetFiles(content.RootDirectory+"\\Fonts")) {
				_fonts.Add(Path.GetFileNameWithoutExtension(f),content.Load<SpriteFont>("Fonts\\"+Path.GetFileNameWithoutExtension(f)));
			}
		}

		/// <summary>
		/// Get a font
		/// </summary>
		/// <param name="name">The name of the font to get</param>
		/// <returns>Font associated with that name</returns>
		public static SpriteFont GetFont(string name) {
			return _fonts[name];
		}
	}
}
