using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RobsXNALibrary {
	public class RectangleCalc {
		public enum RectSide {
			Top,
			Bottom,
			Left,
			Right,
			None
		}

		public static RectSide CalcSide(Rectangle rect,Rectangle rect2) {
			Vector2 Centre=new Vector2(rect.Center.X,rect.Center.Y);
			Vector2 pos=new Vector2(rect2.Center.X,rect2.Center.Y);
			if (pos.Y<(Centre.Y-(rect.Height/2)))
				return RectSide.Top;
			if (pos.Y>(Centre.Y+(rect.Height/2)))
				return RectSide.Bottom;
			if (pos.X>(Centre.X-(rect.Width/2)))
				return RectSide.Right;
			if (pos.X<(Centre.X-(rect.Width/2)))
				return RectSide.Left;
			return RectSide.None;
		}
	}
}
