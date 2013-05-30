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
		private Vector2 _pos,_target;

		public MenuItem(string text,Rectangle rect,OnClick click) {
			_text=text;
			_rect=rect;
			_click=click;
			_pos=new Vector2(_rect.X,_rect.Y);
			_target=_pos;
		}
		
		public MenuItem(string text, Rectangle rect, OnClick click, Vector2 target):this(text,rect,click){
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
	}
	public class Menu {
		public bool Animated=false;
		public delegate void AnimateMethod(MenuItem m);
		public AnimateMethod Animate;
		public delegate void AddMethod();
		public AddMethod OnAdd;
		public string ClickSound="";
		private List<MenuItem> _items=new List<MenuItem>();
		private MenuItem _hoverItem;
		private SpriteFont _font;
		private Color _idleColour,_hoverColour,_clickColour;
		private int _x,_y;

		public Menu(SpriteFont font,int x,int y,Color idlecolour,Color hovercolour,Color clickcolour) {
			_font=font;
			_x=x;
			_y=y;
			_idleColour=idlecolour;
			_hoverColour=hovercolour;
			_clickColour=clickcolour;
		}

		public void Update() {
			_hoverItem=null;
			if (Animated&&Animate!=null)
				foreach (MenuItem m in _items)
					Animate(m);
			while (TouchPanel.IsGestureAvailable) {
				GestureSample g=TouchPanel.ReadGesture();
				switch (g.GestureType) {
					case GestureType.Tap:
						foreach (MenuItem m in _items)
							if (m.Rect.Contains(Input.MousePoint)) {
								_hoverItem=m;
								if (Input.LeftClicked) {
									m.Click();
									if (ClickSound!="")
										Sounds.Get(ClickSound).Play();
								}
								return;
							}
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
				if (m==_hoverItem)
					if (Input.LeftClicked)
						colour=_clickColour;
					else
						colour=_hoverColour;
				spriteBatch.DrawString(_font,m.Text,m.Position,colour*alpha);
			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			Draw(spriteBatch,1);
		}

		public void AddItem(string text,MenuItem.OnClick click){
			_items.Add(new MenuItem(text,new Rectangle(_x,_y+(int)(_items.Count*_font.MeasureString(text).Y),(int)_font.MeasureString(text).X,(int)_font.MeasureString(text).Y),click));
			if (OnAdd!=null)
				OnAdd();
		}

		public MenuItem[] Items {
			get {
				return _items.ToArray();
			}
		}
	}
}
