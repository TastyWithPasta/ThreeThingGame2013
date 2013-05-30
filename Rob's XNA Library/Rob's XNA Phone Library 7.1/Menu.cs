using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace RobsXNALibrary {
	public class MenuItem {
		public delegate void OnClick();
		private OnClick _click;
		private Rectangle _rect;
		private string _text;
		private Texture2D _texture;
		private Vector2 _pos,_target;

		private MenuItem(Rectangle rect,OnClick click) {
			_rect=rect;
			_click=click;
			_pos=new Vector2(_rect.X,_rect.Y);
			_target=_pos;
		}

		public MenuItem(string text,Rectangle rect,OnClick click):this(rect,click) {
			_text=text;
		}
		
		public MenuItem(string text, Rectangle rect, OnClick click, Vector2 target):this(text,rect,click){
			_target=target;
		}

		public MenuItem(Texture2D texture,Rectangle rect,OnClick click):this(rect,click) {
			_texture=texture;
		}
		
		public MenuItem(Texture2D texture, Rectangle rect, OnClick click, Vector2 target):this(texture,rect,click){
			_target=target;
		}

		public void Click() {
			_click();
		}

		public Rectangle Rect {
			get {
				return _rect;
			}
		}

		public string Text {
			get {
				return _text;
			}
		}

		public Vector2 Position {
			get {
				return _pos;
			}
			set {
				_pos=value;
				_rect.X=(int)_pos.X;
				_rect.Y=(int)_pos.Y;
			}
		}

		public Vector2 Target {
			get {
				return _target;
			}
			set {
				_target=value;
			}
		}

		public Texture2D Texture {
			get {
				return _texture;
			}
			set {
				_texture=value;
			}
		}
	}
	public class Menu {
		public enum Alignment {
			Left, Right, Center
		}
		public bool Animated=false;
		public delegate void AnimateMethod(MenuItem m);
		public AnimateMethod Animate;
		public delegate void AddMethod();
		public AddMethod OnAdd;
		public string ClickSound="";
		public int Padding=0;
		public Alignment Align=Alignment.Left;
		private List<MenuItem> _items=new List<MenuItem>();
		private MenuItem _selectedItem;
		private SpriteFont _font;
		private Color _idleColour,_selectColour;
		private int _x,_y,_width;

		public Menu(SpriteFont font,int x,int y,Color idlecolour,Color selectcolour) {
			_font=font;
			_x=x;
			_y=y;
			_idleColour=idlecolour;
			_selectColour=selectcolour;
			_width=0;
		}

		public void Update() {
			if (Animated&&Animate!=null)
				foreach (MenuItem m in _items)
					Animate(m);
			while (TouchPanel.IsGestureAvailable) {
				GestureSample g=TouchPanel.ReadGesture();
				switch (g.GestureType) {
					case GestureType.Tap:
						foreach (MenuItem m in _items) {
							int mod=0;
							switch (Align) {
								case Alignment.Center:
									mod=(m.Rect.Width/2);
									break;
								case Alignment.Right:
									mod=m.Rect.Width;
									break;
							}
							if (m.Rect.Contains((int)g.Position.X+mod,(int)g.Position.Y)) {
								_selectedItem=m;
								m.Click();
								if (ClickSound!="")
									Sounds.Get(ClickSound).Play();
								return;
							}
						}
						_selectedItem=null;
						break;
					case GestureType.Hold:
						break;
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch,float alpha) {
			alpha=MathHelper.Clamp(alpha,0,1);
			foreach (MenuItem m in _items) {
				Color colour=_idleColour;
				if (m==_selectedItem)
					colour=_selectColour;
				Vector2 pos=m.Position;
				switch (Align) {
					case Alignment.Center:
						pos.X-=(m.Rect.Width/2);
						break;
					case Alignment.Right:
						pos.X-=m.Rect.Width;
						break;
				}
				if (_font!=null)
					spriteBatch.DrawString(_font,m.Text,pos,colour*alpha);
				else
					spriteBatch.Draw(m.Texture,pos,colour*alpha);
			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			Draw(spriteBatch,1);
		}

		public void AddItem(string text,MenuItem.OnClick click){
			float x,y1,y2;
			Texture2D texture=null;
			if (_font!=null) {
				x=_font.MeasureString(text).X;
				y2=_font.MeasureString(text).Y;
				y1=_items.Count*(y2+Padding);
			} else {
				texture=Textures.Get(text);
				x=texture.Width;
				y1=0;
				foreach (MenuItem m in _items)
					y1+=m.Texture.Height+Padding;
				y2=texture.Height;
				if (texture.Width>_width)
					_width=texture.Width;
			}
			Rectangle rect=new Rectangle(_x,_y+(int)y1,(int)x,(int)y2);
			MenuItem mi;
			if (_font!=null)
				mi=new MenuItem(text,rect,click);
			else
				mi=new MenuItem(texture,rect,click);
			_items.Add(mi);
			if (OnAdd!=null)
				OnAdd();
		}

		public void Reset() {
			_selectedItem=null;
		}

		public MenuItem[] Items {
			get {
				return _items.ToArray();
			}
		}
	}
}
