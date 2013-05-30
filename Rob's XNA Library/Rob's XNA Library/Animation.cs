using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RobsXNALibrary {
	internal class AnimationStage {
		int _startFrame,_endFrame,_interval;
		bool _loop,_nextStageAfterComplete,_destroyAfterComplete;

		public AnimationStage(int startFrame,int endFrame,bool loop,int interval,bool nextStageAfterComplete) {
			_startFrame=startFrame;
			_endFrame=endFrame;
			_loop=loop;
			_interval=interval;
			_nextStageAfterComplete=nextStageAfterComplete;
		}

		public AnimationStage(int startFrame,int endFrame,bool loop,int interval,bool nextStageAfterComplete, bool destroyAfterComplete):this(startFrame, endFrame, loop, interval, nextStageAfterComplete){
			_destroyAfterComplete=destroyAfterComplete;
		}

		public int StartFrame {
			get {
				return _startFrame;
			}
		}

		public int EndFrame {
			get {
				return _endFrame;
			}
		}

		public bool Loop {
			get {
				return _loop;
			}
		}

		public int Interval {
			get {
				return _interval;
			}
		}

		public bool NextStageAfterComplete {
			get {
				return _nextStageAfterComplete;
			}
		}

		public bool DestroyAfterComplete {
			get {
				return _destroyAfterComplete;
			}
			set {
				_destroyAfterComplete=value;
			}
		}

		public int FrameCount {
			get {
				return EndFrame-StartFrame;
			}
		}
	}
	public class Animation {
		private const int DEFAULT_FRAME_DELAY=50;
		Texture2D _texture;
		Rectangle _textureRect,_displayRect;
		private int _cols,_rows,_totalFrames,_currentFrame,_currentStageIndex,_timerIndex;
		List<AnimationStage> _stages=new List<AnimationStage>();
		Dictionary<string,int> _stageNames=new Dictionary<string,int>();
		AnimationStage _currentStage;
		GameObject _parent;
		Timer _timer;
		bool _useTimer;
		double _tickElapsed=0;

		public Animation(GameObject parent,int width,int height,bool useTimer) {
			_texture=parent._texture;
			_textureRect=new Rectangle(0,0,_texture.Width,_texture.Height);
			_displayRect=new Rectangle(0,0,width,height);
			_cols=_texture.Width/width;
			_rows=_texture.Height/height;
			_totalFrames=_cols*_rows;
			_parent=parent;
			_currentStage=new AnimationStage(0,_totalFrames-1,true,0,false);
			_parent._srcRect=_displayRect;
			_currentStageIndex=0;
			_useTimer=useTimer;
			if (_useTimer){
				_timer=new Timer(DEFAULT_FRAME_DELAY,0,Animate,this);
				_timerIndex=Timers.Add(_timer);
			}
		}

		public Animation(GameObject parent,int width,int height):this(parent,width,height,true){}

		public void Remove() {
			Timers.Remove(_timerIndex);
			_parent=null;
		}

		public void Pause() {
			_timer.Enabled=false;
		}

		public void Play() {
			_timer.Enabled=true;
		}

		public void Stop() {
			_currentFrame=_currentStage.StartFrame;
			_timer.Enabled=false;
		}

		public void SetStageName(string name,int stage) {
			_stageNames[name]=stage;
		}

		public int AddStage(int startFrame,int endFrame,bool loop,int interval,bool nextStageAfterComplete,bool destroyAfterComplete) {
			_stages.Add(new AnimationStage(startFrame,endFrame,loop,interval,nextStageAfterComplete,destroyAfterComplete));
			if (_stages.Count==1) {
				_currentStage=_stages[0];
				_currentFrame=_currentStage.StartFrame;
				if (_useTimer)
					_timer.Interval=_currentStage.Interval;
			}
			return _stages.Count-1;
		}

		public int AddStage(int startFrame,int endFrame,bool loop,int interval,bool nextStageAfterComplete) {
			return AddStage(startFrame,endFrame,loop,interval,nextStageAfterComplete,false);
		}

		public int AddStage(string name,int startFrame,int endFrame,bool loop,int interval,bool nextStageAfterComplete,bool destroyAfterComplete) {
			int i=AddStage(startFrame,endFrame,loop,interval,nextStageAfterComplete,destroyAfterComplete);
			SetStageName(name,i);
			return i;
		}

		public int AddStage(string name,int startFrame,int endFrame,bool loop,int interval,bool nextStageAfterComplete) {
			return AddStage(name,startFrame,endFrame,loop,interval,nextStageAfterComplete,false);
		}

		public int GetStageIndex(string name) {
			return _stageNames[name];
		}

		private void SetupStage(bool animate) {
			_currentStage=_stages[_currentStageIndex];
			_currentFrame=_currentStage.StartFrame;
			if (_useTimer)
				_timer.Interval=_currentStage.Interval;
			if (animate)
				Animate();
		}

		public void NextStage() {
			_currentStageIndex++;
			SetupStage(true);
		}

		public void SwitchToStage(int index) {
			_currentStageIndex=index;
			int framediff=_currentFrame-_currentStage.StartFrame;
			SetupStage(false);
			_currentFrame=_currentStage.StartFrame+framediff;
		}

		public void SwitchToStage(string name) {
			SwitchToStage(_stageNames[name]);
		}

		public void SetStage(int index) {
			_currentStageIndex=index;
			SetupStage(true);
		}

		public void SetStage(string name) {
			SetStage(_stageNames[name]);
		}

		public void Animate() {
			if (_currentFrame>_currentStage.EndFrame)
				if (_currentStage.Loop)
					_currentFrame=_currentStage.StartFrame;
				else if (_currentStage.NextStageAfterComplete)
					NextStage();
				else if (_currentStage.DestroyAfterComplete)
					Timers.Remove(_timer);
				else
					return;
			_displayRect.X=_displayRect.Width*(_currentFrame%_cols);
			_displayRect.Y=_displayRect.Height*(_currentFrame/_cols);
			_parent._srcRect=_displayRect;
			_currentFrame++;
		}

		public void Tick(GameTime gameTime) {
			if (_useTimer)
				return;
			_tickElapsed+=gameTime.ElapsedGameTime.TotalMilliseconds;
			if (_tickElapsed>=_currentStage.Interval) {
				_tickElapsed=0;
				Animate();
			}
		}

		public int CurrentFrame {
			get {
				return _currentFrame;
			}
		}

		public int CurrentStageIndex {
			get {
				return _currentStageIndex;
			}
		}

		public bool IsFinished
		{
			get
			{
				if (_currentFrame > _currentStage.EndFrame)
				{
					return true;
				}
				return false;
			}
		}
	}
}
