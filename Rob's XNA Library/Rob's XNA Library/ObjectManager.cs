using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobsXNALibrary {
	public class ObjectManager {
		private List<Rectangle> _bounds=new List<Rectangle>();
		public GameObject[] ObjectList;
		private bool _clear;
		Dictionary<Rectangle,List<int>> Collides=new Dictionary<Rectangle,List<int>>();

		/// <summary>
		/// ObjectManager constructor
		/// </summary>
		/// <param name="posx">X position to start the collisions from</param>
		/// <param name="posy">Y position to start the collisions from</param>
		/// <param name="xcount">Horizontal rectangle count</param>
		/// <param name="ycount">Vertical rectangle count</param>
		/// <param name="width">Field width</param>
		/// <param name="height">Field Height</param>
		/// <param name="maxObjects">Maximum amount of objects allowed</param>
		public ObjectManager(float posx,float posy,int xcount,int ycount,int width,int height, int maxObjects) {
			_clear=false;
			int regionwidth=width/xcount;
			int regionheight=height/ycount;
			ObjectList=new GameObject[maxObjects];
			for (int i=0;i<ObjectList.Length;i++)
				ObjectList[i]=null;
			_bounds=new List<Rectangle>();
			Vector2 width2=new Vector2(regionwidth,regionheight);
			for (int x=0;x<xcount;x++)
				for (int y=0;y<ycount;y++) {
					Vector2 pos=new Vector2(x,y)*width2;
					_bounds.Add(new Rectangle((int)(posx+pos.X),(int)(posy+pos.Y),regionwidth,regionheight));
				}
			foreach (Rectangle r in _bounds)
				Collides[r]=new List<int>();
		}

		/// <summary>
		/// ObjectManager constructor
		/// </summary>
		/// <param name="collisionField">Area to cover</param>
		/// <param name="xcount">Horizontal rectangle count</param>
		/// <param name="ycount">Vertical rectangle count</param>
		/// <param name="maxObjects">Maximum amount of objects allowed</param>
		public ObjectManager(Rectangle collisionField,int xCount,int yCount,int maxObjects)
			:this(collisionField.X,collisionField.Y,xCount,yCount,collisionField.Width,collisionField.Height,maxObjects) {
		}

		/// <summary>
		/// Adds a GameObject to the object list.
		/// </summary>
		/// <param name="obj">GameObject to add</param>
		public void AddObject(GameObject obj) {
			for (int i=0;i<ObjectList.Length;i++)
				if (ObjectList[i]==null) {
					ObjectList[i]=obj;
					break;
				}
		}

		/// <summary>
		/// Flags the object list for clearing
		/// </summary>
		public void ClearObjects() {
			_clear=true;
		}

		private void DoClear() {
			for (int i=0;i<ObjectList.Length;i++)
				if (ObjectList[i]!=null&&ObjectList[i].CanRemove)
					ObjectList[i]=null;
			_clear=false;
		}

		[Obsolete("Update() is now considered insufficient, use Update(GameTime) instead",true)]
		public void Update() {
		}

		/// <summary>
		/// Updates all GameObjects and detects any collisions - compulsory for ObjectManager to work
		/// </summary>
		public void Update(GameTime gameTime) {
			for (int i=0;i<ObjectList.Length;i++)
				if (ObjectList[i]!=null) {
					ObjectList[i].Update(gameTime);
					if (ObjectList[i].ToRemove) {
						ObjectList[i].Dispose();
						ObjectList[i]=null;
					}
				}
			foreach (Rectangle r in _bounds) {
				Collides[r].Clear();
				for (int i=0;i<ObjectList.Length;i++)
					if (ObjectList[i]!=null&&r.Intersects(ObjectList[i].Rect))
						Collides[r].Add(i);
			}
			foreach (KeyValuePair<Rectangle,List<int>> kvp in Collides)
				foreach (int i in kvp.Value) {
					if (ObjectList[i]==null)
						continue;
					if (!ObjectList[i].ToRemove) {
						foreach (int i2 in kvp.Value) {
							if (ObjectList[i]==null)
								break;
							if (ObjectList[i2]==null||ObjectList[i]==ObjectList[i2])
								continue;
							if (!_clear) {
								if (!ObjectList[i2].ToRemove) {
									if (ObjectList[i].CanCollide(ObjectList[i2]))
										if (ObjectList[i].BoundingRect.Intersects(ObjectList[i2].BoundingRect))
											if (ObjectList[i2].Scaled||ObjectList[i].Scaled||PixelPerfect(ObjectList[i],ObjectList[i2])) {
												bool remove1=false;
												bool remove2=false;
												if (ObjectList[i].DoCollide(ObjectList[i2]))
													if (ObjectList[i].CanRemove)
														//ObjectList[i]=null;
														remove1=true;
												if (ObjectList[i2].DoCollide(ObjectList[i]))
													if (ObjectList[i2].CanRemove)
														//ObjectList[i2]=null;
														remove2=true;
												if (remove1) {
													ObjectList[i].Dispose();
													ObjectList[i]=null;
												}
												if (remove2) {
													ObjectList[i2].Dispose();
													ObjectList[i2]=null;
												}
											}
								} else {
									ObjectList[i2].Dispose();
									ObjectList[i2]=null;
								}
							} else {
								DoClear();
								return;
							}
						}
					} else {
						ObjectList[i].Dispose();
						ObjectList[i]=null;
					}
				}
		}

		private bool PixelPerfectSimple(Rectangle rectangleA,Color[] dataA,Rectangle rectangleB,Color[] dataB) {// Stolen from http://create.msdn.com/en-US/education/catalog/tutorial/collision_2d_perpixel
			int top=Math.Max(rectangleA.Top,rectangleB.Top);
			int bottom=Math.Min(rectangleA.Bottom,rectangleB.Bottom);
			int left=Math.Max(rectangleA.Left,rectangleB.Left);
			int right=Math.Min(rectangleA.Right,rectangleB.Right);
			for (int y=top;y<bottom;y++) {
				for (int x=left;x<right;x++) {
					Color colorA=dataA[(x-rectangleA.Left)+(y-rectangleA.Top)*rectangleA.Width];
					Color colorB=dataB[(x-rectangleB.Left)+(y-rectangleB.Top)*rectangleB.Width];
					if (colorA.A>0&&colorB.A>0)
						return true;
				}
			}
			return false;
		}

		private bool PixelPerfect(GameObject o1,GameObject o2) {
			if (o1.Rotation==0&&o2.Rotation==0)
				return PixelPerfectSimple(o1.Rect,o1.TextureData,o2.Rect,o2.TextureData);
			Matrix a2b=Matrix.CreateTranslation(-o1.Origin.X,-o1.Origin.Y,0)*
				Matrix.CreateRotationZ(o1.Rotation)*
				Matrix.CreateTranslation(o1.Position.X,o1.Position.Y,0)*
				Matrix.Invert(
					Matrix.CreateTranslation(-o2.Origin.X,-o2.Origin.Y,0)*
					Matrix.CreateRotationZ(o2.Rotation)*
					Matrix.CreateTranslation(o2.Position.X,o2.Position.Y,0)
				);
			Vector2 stepX=Vector2.TransformNormal(Vector2.UnitX,a2b);
			Vector2 stepY=Vector2.TransformNormal(Vector2.UnitY,a2b);
			Vector2 o2y=Vector2.Transform(Vector2.Zero,a2b);
			for (int y1=0;y1<o1.Rect.Height;y1++) {
				Vector2 o2pos=o2y;
				for (int x1=0;x1<o1.Rect.Width;x1++) {
					int x2=(int)Math.Round(o2pos.X);
					int y2=(int)Math.Round(o2pos.Y);
					if (x2>0&&x2<o2.Rect.Width&&y2>0&&y2<o2.Rect.Height)
						if (o1.TextureData[x1+y1*o1.Rect.Width].A>0&&o2.TextureData[x2+y2*o2.Rect.Width].A>0)
							return true;
					o2pos+=stepX;
				}
				o2y+=stepY;
			}
			return false;
		}

		/// <summary>
		/// Draw all objects in the ObjectList
		/// </summary>
		/// <param name="spriteBatch"></param>
		public void Draw(SpriteBatch spriteBatch) {
			for (int i=0;i<ObjectList.Length;i++)
				if (ObjectList[i]!=null)
					ObjectList[i].Draw(spriteBatch);
		}

		/// <summary>
		/// Get an array of GameObject within a radius around a point
		/// </summary>
		/// <param name="point">Location centre from</param>
		/// <param name="radius">Radius to search</param>
		/// <param name="criteria">Case sensitive class to search for. Can be a partial match</param>
		/// <returns>Results of search</returns>
		/*public GameObject[] FindInRadius(Vector2 point,float radius,string criteria) {
			Rectangle r=new Rectangle((int)(point.X-radius),(int)(point.Y-radius),(int)radius,(int)radius);
			List<GameObject> objects=new List<GameObject>();
			foreach (KeyValuePair<Rectangle,List<int>> kvp in Collides)
				if (kvp.Key.Intersects(r))
					foreach (int i in kvp.Value)
						if (ObjectList[i]!=null && ObjectList[i].GetType().ToString().IndexOf(criteria)>-1 && Vector2.Distance(point,ObjectList[i].Position)<=radius)
							objects.Add(ObjectList[i]);
			return objects.ToArray();
		}*/

		/// <summary>
		/// Get an array of GameObject within a radius around a point
		/// </summary>
		/// <param name="point">Location centre from</param>
		/// <param name="radius">Radius to search</param>
		/// <returns>Results of search</returns>
		/*public GameObject[] FindInRadius(Vector2 point,float radius) {
			return FindInRadius(point,radius,"");
		}*/
	}
}