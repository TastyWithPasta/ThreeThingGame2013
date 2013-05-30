using System;
using Microsoft.Xna.Framework;

namespace RobsXNALibrary {
	public static class Helper {
		private static readonly Vector2 UP=new Vector2(0,-1);

		/// <summary>
		/// Convert a vector to an angle
		/// </summary>
		/// <param name="vec"></param>
		/// <returns></returns>
		public static float VecToAng(Vector2 vec) {
			return -(float)Math.Atan2(vec.X,vec.Y);
		}

		/// <summary>
		/// Convert an angle to a vector
		/// </summary>
		/// <param name="ang"></param>
		/// <returns></returns>
		public static Vector2 AngToVec(float ang) {
			return Vector2.Transform(UP,Matrix.CreateRotationZ(ang));
		}

		/// <summary>
		/// Rotate a vector
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="ang"></param>
		/// <returns></returns>
		public static Vector2 RotateVec(Vector2 vec, float ang) {
			Vector2 retvec=new Vector2();
			retvec.X=vec.X*(float)Math.Cos(ang)-vec.Y*(float)Math.Sin(ang);
			retvec.Y=vec.X*(float)Math.Sin(ang)+vec.Y*(float)Math.Cos(ang);
			return retvec;
		}
	}
}
