using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobsXNALibrary {
	public class GameCam { //had to rename from Camera thanks to Microsoft.Devices.Camera
		private const float SHAKE_DELAY=1f/60f;
		private float _currentZoom,_targetZoom,_currentRotation,_targetRotation,_scaleX,_scaleY;
		private Vector2 _currentPos,_targetPos,_minBounds,_maxBounds;
		private bool _shake;
		private int _shakeMagnitude,_shakeCountdown,_width,_height;
		private Matrix _matrix;
		private static Random rand;
		private double _lastShake=0f;
		public float MaxZoom;//=1.5f;
		public float MinZoom;//=0.7f;

		/// <summary>
		/// GameCam constructor
		/// </summary>
		/// <param name="width">Viewport's width</param>
		/// <param name="height">Viewport's height</param>
		/// <param name="scalex">Scale of the camera width</param>
		/// <param name="scaley">Scale of the camera height</param>
		public GameCam(int width,int height,float scalex,float scaley) {
			_currentZoom=1f;
			_targetZoom=1f;
			_currentRotation=0f;
			_targetRotation=0f;
			_currentPos=Vector2.Zero;
			_targetPos=Vector2.Zero;
			_shake=false;
			_shakeMagnitude=0;
			_shakeCountdown=0;
			rand=new Random();
			_minBounds=new Vector2(-500,-500);
			_maxBounds=new Vector2(500,500);
			MaxZoom=2.5f;
			MinZoom=0.1f;
			_width=width;
			_height=height;
			_scaleX=scalex;
			_scaleY=scaley;
			ZoomSpeed=5;
			MoveSpeed=10;
			Update(new GameTime());
		}

		/// <summary>
		/// Set up an area to constrin the camera
		/// </summary>
		/// <param name="Top">Lowest Y Value</param>
		/// <param name="Bottom">Highest Y Value</param>
		/// <param name="Left">Lowest X Value</param>
		/// <param name="Right">Highest X Value</param>
		public void SetBounds(float Top,float Bottom,float Left,float Right) {
			_maxBounds.X=Right;
			_maxBounds.Y=Bottom;
			_minBounds.X=Left;
			_minBounds.Y=Top;
		}

		/// <summary>
		/// Set the zoom level for the camera
		/// </summary>
		/// <param name="zoom">Level of zoom</param>
		public void SetZoom(float zoom) {
			if (zoom<MinZoom-0.1f||zoom>MaxZoom+0.1f)
				return;
			_targetZoom=zoom;
		}

		/// <summary>
		/// Relative zoom adjustment
		/// </summary>
		/// <param name="amount">Amount of zoom to use</param>
		public void Zoom(float amount) {
			SetZoom(_targetZoom+amount);
		}

		/// <summary>
		/// Current zoom level
		/// </summary>
		public float zoom {
			get {
				return _targetZoom;
			}
		}

		/// <summary>
		/// Set cameras view target
		/// </summary>
		/// <param name="pos">Target position</param>
		/// <param name="immediate">True for immediate move, or false for smooth move</param>
		public void SetPos(Vector2 pos, bool immediate) {
			_targetPos=pos;
			_targetPos.X=MathHelper.Clamp(_targetPos.X,_minBounds.X,_maxBounds.X);
			_targetPos.Y=MathHelper.Clamp(_targetPos.Y,_minBounds.Y,_maxBounds.Y);
			if (immediate)
				_currentPos=_targetPos;
		}

		/// <summary>
		/// Set cameras view target and smoothly moves there
		/// </summary>
		/// <param name="pos">Target position</param>
		public void SetPos(Vector2 pos) {
			SetPos(pos,false);
		}

		/// <summary>
		/// Move the camera
		/// </summary>
		/// <param name="amount">Move amount</param>
		public void Move(Vector2 amount) {
			SetPos(_targetPos+amount);
		}

		/// <summary>
		/// Current camera view target
		/// </summary>
		public Vector2 Position {
			get {
				return _targetPos;
			}
		}


		/// <summary>
		/// Actual camera view position
		/// </summary>
		public Vector2 CurPosition {
			get {
				return _currentPos;
			}
		}

		/// <summary>
		/// Set the rotation of the camera in degrees
		/// </summary>
		/// <param name="rotation">Rotation value</param>
		public void SetRotationDeg(float rotation) {
			_targetRotation=MathHelper.ToRadians(rotation);
		}

		/// <summary>
		/// Set the rotation of the camera in radians
		/// </summary>
		/// <param name="rotation">Rotation value</param>
		public void SetRotation(float rotation) {
			_targetRotation=rotation;
		}

		/// <summary>
		/// Rotate the camera using degrees
		/// </summary>
		/// <param name="amount">Rotate amount</param>
		public void Rotate(float amount) {
			_targetRotation+=MathHelper.ToRadians(amount);
		}

		/// <summary>
		/// Clips the camera rotation value between 0 and 2Pi
		/// </summary>
		void ClipRotation() {
			if (_targetRotation>Math.PI) {
				_targetRotation-=MathHelper.TwoPi;
			} else if (_targetRotation<-Math.PI) {
				_targetRotation+=MathHelper.TwoPi;
			}
			if (Math.Abs(_currentRotation-_targetRotation)>Math.PI) {
				if (_currentRotation>_targetRotation) {
					_currentRotation-=MathHelper.TwoPi;
				} else {
					_currentRotation+=MathHelper.TwoPi;
				}
			}
		}

		/// <summary>
		/// Cameras rotation
		/// </summary>
		public float Rotation {
			get {
				return _targetRotation;
			}
		}

		/// <summary>
		/// Shake the camera
		/// </summary>
		/// <param name="Magnitude">Magnitude of each shake</param>
		/// <param name="Shakes">Frames to shake for</param>
		public void Shake(int Magnitude,int Shakes) {
			if (Magnitude>_shakeMagnitude||!_shake)
				_shakeMagnitude=Magnitude;
			if (Shakes>_shakeCountdown)
				_shakeCountdown=Shakes;
			_shake=_shakeCountdown>0&&_shakeMagnitude>0;
		}

		/// <summary>
		/// Update the camera
		/// </summary>
		public void Update(GameTime gameTime) {
			ClipRotation();
			if (_targetZoom!=_currentZoom)
				_currentZoom+=((_targetZoom-_currentZoom)/ZoomSpeed)*(float)gameTime.ElapsedGameTime.TotalSeconds*60f;
			if (_targetPos!=_currentPos)
				_currentPos+=((_targetPos-_currentPos)/MoveSpeed)*(float)gameTime.ElapsedGameTime.TotalSeconds*60f;
			if (_targetRotation!=_currentRotation)
				_currentRotation+=((_targetRotation-_currentRotation)/5);
			int ShakeModX=0;
			int ShakeModY=0;
			_lastShake+=gameTime.ElapsedGameTime.TotalSeconds;
			if (_shake && _lastShake>=SHAKE_DELAY) {
				ShakeModX=rand.Next(-_shakeMagnitude,_shakeMagnitude);
				ShakeModY=rand.Next(-_shakeMagnitude,_shakeMagnitude);
				_shakeCountdown--;
				_shake=_shakeCountdown>0;
			}

			_matrix=(
					Matrix.CreateTranslation(
					new Vector3(-_currentPos.X+ShakeModX,-_currentPos.Y+ShakeModY,0))*
					Matrix.CreateRotationZ(_currentRotation)*
					Matrix.CreateScale(new Vector3(_currentZoom,_currentZoom,1))*
					Matrix.CreateTranslation((_width/_scaleX)/2,(_height/_scaleY)/2,0)
				)*Matrix.CreateScale(_scaleX,_scaleY,1f);
		}

		/// <summary>
		/// View matrix of the camera
		/// </summary>
		public Matrix CamMatrix {
			get {
				return _matrix;
			}
		}

		/// <summary>
		/// Translated mouse position
		/// </summary>
		public Vector2 MousePos {
			get {
				return Vector2.Transform(Input.MousePos,Matrix.Invert(_matrix));
			}
		}

		public float ZoomSpeed {
			get;
			set;
		}

		public float MoveSpeed {
			get;
			set;
		}
	}
}
