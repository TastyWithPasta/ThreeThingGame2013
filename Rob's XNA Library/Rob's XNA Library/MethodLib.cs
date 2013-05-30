using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobsXNALibrary {
	public static class MethodLib {
		public static Vector2 Vec2Rotate(Vector2 vec,float angle) {
			float s=(float)Math.Sin(MathHelper.ToRadians(angle));
			float c=(float)Math.Cos(MathHelper.ToRadians(angle));
			float x=vec.X*c-vec.Y*s;
			float y=vec.X*s+vec.Y*c;
			return new Vector2(x,y);
		}

		public static Rectangle CreateRectangleFromVector2(Vector2 vector) {
			return new Rectangle((int)vector.X,(int)vector.Y,1,1);
		}

		public static Point Vector2ToPoint(Vector2 vector) {
			return new Point((int)vector.X,(int)vector.Y);
		}

		public static Vector2 GetPositionFromRectangle(Rectangle rectangle) {
			return new Vector2(rectangle.X,rectangle.Y);
		}


	}
}
