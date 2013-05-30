using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobsXNALibrary {
	public class GameObject:IDisposable {
		protected static Dictionary<Texture2D,Color[]> SavedTextureData=new Dictionary<Texture2D,Color[]>();
		protected Random rand=new Random();
		private static int _ID=0;
		private int _id;
		private Matrix _transform;
		private bool _scaled;
		protected Color[] _textureData;
		protected Vector2 _pos,_origin;
		protected float _rotation,_scale;
		protected internal Texture2D _texture;
		protected int _width,_height;
		protected Rectangle _rect,_boundingRect,_drawRect;
		protected internal Rectangle _srcRect;
		protected SpriteEffects _spriteEffect;
		protected Animation _animation;
		public bool CanRemove=true;
		public Color Colour;
		public bool ToRemove=false;

		/// <summary>
		/// GameObject basic constructor
		/// </summary>
		/// <param name="texturename">Name of texture in the Textures static class</param>
		public GameObject(string texturename) {
			_id=_ID;
			_ID++;
			_pos=Vector2.Zero;
			_rotation=0;
			Colour=Color.White;
			ChangeTexture(texturename);
			_srcRect=new Rectangle(0,0,_texture.Width,_texture.Height);
			_spriteEffect=SpriteEffects.None;
			_animation=null;
			_scale=1;
			_transform=new Matrix();
			_scaled=false;
			UpdateRect();
		}

		/// <summary>
		/// Animated GameObject constructor
		/// </summary>
		/// <param name="texturename">Name of texture in the Textures static class</param>
		/// <param name="width">Frame/object width</param>
		/// <param name="height">Frame/object height</param>
		public GameObject(string texturename,int width,int height):this(texturename) {
			_width=width;
			_height=height;
			_animation=new Animation(this,width,height);
			UpdateRect();
		}

		/// <summary>
		/// Runs a cleanup of the object
		/// </summary>
		protected virtual void CleanUp() {
			if (_animation!=null) {
				_animation.Remove();
				_animation=null;
			}
		}

		/// <summary>
		/// Descructor
		/// </summary>
		~GameObject() {
			CleanUp();
		}

		/// <summary>
		/// Dispose of the GameObject
		/// </summary>
		public void Dispose() {
			CleanUp();
		}

		/// <summary>
		/// Change the texture of the object
		/// </summary>
		/// <param name="texturename">Name of texture in the Textures static class</param>
		protected void ChangeTexture(string texturename) {
			_texture=Textures.Get(texturename);
			_width=_texture.Width;
			_height=_texture.Height;
			if (SavedTextureData.ContainsKey(_texture))
				_textureData=SavedTextureData[_texture];
			else {
				_textureData=new Color[_width*_height];
				_texture.GetData(_textureData);
				SavedTextureData[_texture]=_textureData;
			}
		}

		/// <summary>
		/// Update the rectangles
		/// </summary>
		protected virtual void UpdateRect() {
			_origin=new Vector2((float)_width/2,(float)_height/2);
			_rect.X=(int)(_pos.X-_origin.X);
			_rect.Y=(int)(_pos.Y-_origin.Y);
			_rect.Width=_width;
			_rect.Height=_height;
			_drawRect=_rect;
			_drawRect.X=(int)_pos.X;
			_drawRect.Y=(int)_pos.Y;
			_origin=new Vector2(_srcRect.Width/2,_srcRect.Height/2);
			UpdateBoundingRect();
		}

		/// <summary>
		/// Update the bounding rectangles
		/// </summary>
		public void UpdateBoundingRect() {
			if (_rotation==0&&_scale==1) {
				_boundingRect=_rect;
				return;
			}
			_transform=Matrix.CreateTranslation(-_pos.X,-_pos.Y,0f)*Matrix.CreateScale(1/_scale)*Matrix.CreateRotationZ(_rotation)*Matrix.CreateScale(_scale)*Matrix.CreateTranslation(_pos.X,_pos.Y,0);
			Vector2 leftTop=new Vector2(_rect.Left,_rect.Top);
			Vector2 rightTop=new Vector2(_rect.Right,_rect.Top);
			Vector2 leftBottom=new Vector2(_rect.Left,_rect.Bottom);
			Vector2 rightBottom=new Vector2(_rect.Right,_rect.Bottom);
			Vector2.Transform(ref leftTop,ref _transform,out leftTop);
			Vector2.Transform(ref rightTop,ref _transform,out rightTop);
			Vector2.Transform(ref leftBottom,ref _transform,out leftBottom);
			Vector2.Transform(ref rightBottom,ref _transform,out rightBottom);
			Vector2 min=Vector2.Min(Vector2.Min(leftTop,rightTop),Vector2.Min(leftBottom,rightBottom));
			Vector2 max=Vector2.Max(Vector2.Max(leftTop,rightTop),Vector2.Max(leftBottom,rightBottom));
			_boundingRect.X=(int)(min.X);
			_boundingRect.Y=(int)(min.Y);
			_boundingRect.Width=(int)((max.X-min.X));
			_boundingRect.Height=(int)((max.Y-min.Y));
		}

		/// <summary>
		/// Draw the GameObject
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to use for drawing</param>
		/// <param name="alpha">GameObject draw transparency</param>
		public virtual void Draw(SpriteBatch spriteBatch, float alpha) {
			if (alpha<=0)
				return;
			alpha=MathHelper.Clamp(alpha,0,1);
			spriteBatch.Draw(_texture,_drawRect,_srcRect,Colour*alpha,_rotation,_origin,_spriteEffect,0);
		}

		/// <summary>
		/// Draw the GameObject
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to use for drawing</param>
		public virtual void Draw(SpriteBatch spriteBatch) {
			spriteBatch.Draw(_texture,_drawRect,_srcRect,Colour,_rotation,_origin,_spriteEffect,0);
		}

		/// <summary>
		/// Get the scale for the GameObject
		/// </summary>
		/// <returns>GameObject scale</returns>
		public float Scale() {
			return _scale;
		}

		/// <summary>
		/// Scale the GameObject
		/// </summary>
		/// <param name="amount">Scale factor</param>
		public void Scale(float amount) {
			_scale=amount;
			_height=(int)((float)_height*amount);
			_width=(int)((float)_width*amount);
			UpdateRect();
			_scaled=true;
		}

		/// <summary>
		/// Scale the height GameObject
		/// </summary>
		/// <param name="amount">Scale factor</param>
		public void ScaleX(float amount) {
			_width=(int)((float)_width*amount);
			UpdateRect();
			_scaled=true;
		}

		/// <summary>
		/// Scale the width GameObject
		/// </summary>
		/// <param name="amount">Scale factor</param>
		public void ScaleY(float amount) {
			_height=(int)((float)_height*amount);
			UpdateRect();
			_scaled=true;
		}

		/// <summary>
		/// Scale the GameObject rounding up to the nearest whole number
		/// </summary>
		/// <param name="amount">Scale factor</param>
		public void ScaleUp(float amount) {
			_scale=amount;
			_height=(int)Math.Ceiling((float)_height*amount);
			_width=(int)Math.Ceiling((float)_width*amount);
			UpdateRect();
			_scaled=true;
		}

		/// <summary>
		/// Scale the GameObject rounding down to the nearest whole number
		/// </summary>
		/// <param name="amount">Scale factor</param>
		public void ScaleDown(float amount) {
			_scale=amount;
			_height=(int)Math.Floor((float)_height*amount);
			_width=(int)Math.Floor((float)_width*amount);
			UpdateRect();
			_scaled=true;
		}

		/// <summary>
		/// Set the size of the GameObject
		/// </summary>
		/// <param name="width">Width to set</param>
		/// <param name="height">Height to set</param>
		public void SetSize(int width,int height) {
			_height=height;
			_width=width;
			UpdateRect();
			_scaled=true;
		}

		/// <summary>
		/// Set the colour of the GameObject
		/// </summary>
		/// <param name="colour">Colour to use</param>
		public void SetColour(Color colour) {
			Colour=colour;
		}

		/// <summary>
		/// Move the GameObject
		/// </summary>
		/// <param name="amount">Vector to move the object by</param>
		public void Move(Vector2 amount) {
			//_lastPos=_pos;
			Position+=amount;
			//UpdateRect();
		}

		/// <summary>
		/// Move the GameObject along the X axis
		/// </summary>
		/// <param name="amount">Move value</param>
		public void MoveX(float amount) {
			_pos.X+=amount;
			UpdateRect();
		}

		/// <summary>
		/// Move the GameObject along the Y axis
		/// </summary>
		/// <param name="amount">Move value</param>
		public void MoveY(float amount) {
			_pos.Y+=amount;
			UpdateRect();
		}

		/// <summary>
		/// Clips the rotation of the GameObject to within 0 and 2Pi
		/// </summary>
		void ClipRotation() {
			if (_rotation>MathHelper.TwoPi) {
				_rotation-=MathHelper.TwoPi;
			} else if (_rotation<0) {
				_rotation+=MathHelper.TwoPi;
			}
		}

		/// <summary>
		/// Rotate the game object
		/// </summary>
		/// <param name="amount">Degress of rotation</param>
		public void Rotate(float amount) {
			Rotation+=MathHelper.ToRadians(amount);
			//ClipRotation();
			//UpdateBoundingRect();
		}

		/// <summary>
		/// Is the GameObject allowed to collide
		/// </summary>
		/// <param name="o">Query object to check</param>
		/// <returns>Returns if the objects can collide or not</returns>
		public virtual bool CanCollide(GameObject o) {
			return false;
		}

		/// <summary>
		/// Do the collide action
		/// </summary>
		/// <param name="go">GameObject that is being collided with</param>
		/// <returns>Returns true if should remove, false if not</returns>
		public virtual bool DoCollide(GameObject go) {
			return false;
		}

		/// <summary>
		/// Update the GameObject
		/// </summary>
		[Obsolete("Update() is now considered insufficient, use Update(GameTime) instead",true)]
		public virtual void Update() {
		}

		/// <summary>
		/// Update the GameObject using GameTime
		/// </summary>
		public virtual void Update(GameTime gameTime) {
		}

		/// <summary>
		/// What to do when the GameObject is being destroyed
		/// </summary>
		public virtual void OnDestroy() {
		}

		/// <summary>
		/// Gets the radius of the GameObject
		/// </summary>
		/// <returns></returns>
		public int GetRadius() {
			return _rect.Width/2;
		}

		/// <summary>
		/// Unique GameObject identifier
		/// </summary>
		public int ID {
			get {
				return _id;
			}
		}

		/// <summary>
		/// GameObject world position
		/// </summary>
		public Vector2 Position {
			get {
				return _pos;
			}
			set {
				_pos=value;
				UpdateRect();
			}
		}

		/// <summary>
		/// GameObject rotation
		/// </summary>
		public virtual float Rotation {
			get {
				return _rotation;
			}
			set {
				_rotation=value;
				ClipRotation();
				UpdateBoundingRect();
			}
		}

		/// <summary>
		/// GameObject containing rectangle - do not use for collisions with rotation and scaling
		/// </summary>
		public Rectangle Rect {
			get {
				return _rect;
			}
		}

		/// <summary>
		/// GameObject bounding rectangle - use for collisions with rotation and scaling
		/// </summary>
		public Rectangle BoundingRect {
			get {
				return _boundingRect;
			}
		}

		/// <summary>
		/// GameObject's source rectangle
		/// </summary>
		public Rectangle SourceRect {
			get {
				return _srcRect;
			}
		}

		/// <summary>
		/// Texture data for this GameObject
		/// </summary>
		public Color[] TextureData {
			get {
				return _textureData;
			}
		}

		/// <summary>
		/// Bounding rectangle transformation information
		/// </summary>
		public Matrix Transform {
			get {
				return _transform;
			}
		}

		/// <summary>
		/// Is this GameObject scaled?
		/// </summary>
		public bool Scaled {
			get {
				return _scaled;
			}
		}


		/// <summary>
		/// Origin for the GameObject
		/// </summary>
		public Vector2 Origin {
			get {
				return _origin;
			}
		}

		/// <summary>
		/// Animation object for managing this GameObject's animations
		/// </summary>
		public Animation Anim {
			get {
				return _animation;
			}
		}
	}
}
