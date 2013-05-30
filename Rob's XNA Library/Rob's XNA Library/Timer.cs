/*
 * Made by Robert Marshall
 * 
 * Feel free to use, but please keep this notice.
 * 
 * http://robware.co.uk
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobsXNALibrary {
	public class Timer {
		private static int _ID=0;
		private int ID=0;
		public delegate void OnTick();
		public OnTick Tick;
		public int Interval;
		public double Elapsed;//=new TimeSpan(0,0,0);
		public int Reps;
		public bool _Enabled=true;
		public object Parent=null;

		/// <summary>
		/// Timer constructor
		/// </summary>
		/// <param name="Interval">Timer's interval</param>
		/// <param name="Reps">Number of times to repeat this timer before removal</param>
		/// <param name="Tick">Method to execute each tick</param>
		public Timer(int Interval,int Reps,OnTick Tick) {
			_ID++;
			ID=_ID;
			this.Interval=Interval;
			this.Tick=Tick;
			this.Reps=Reps;
		}

		/// <summary>
		/// Timer constructor
		/// </summary>
		/// <param name="Interval">Timer's interval</param>
		/// <param name="Reps">Number of times to repeat this timer before removal</param>
		/// <param name="Tick">Method to execute each tick</param>
		/// <param name="Parent">Parent object - useful for posession checking and debugging</param>
		public Timer(int Interval, int Reps, OnTick Tick, object Parent):this(Interval,Reps,Tick){
			this.Parent=Parent;
		}

		/// <summary>
		/// Reset the timer
		/// </summary>
		public void Reset() {
			Elapsed=0;
		}

		/// <summary>
		/// Is the timer enabled?
		/// </summary>
		public bool Enabled {//Made in to property to enable use of the stack tracer to find where it's being changed.
			get{
				return _Enabled;
			}
			set {
				//System.Diagnostics.Debug.Assert(!value);
				_Enabled=value;
			}
		}

		/// <summary>
		/// Unique timer ID
		/// </summary>
		public int TimerID {
			get {
				return ID;
			}
		}
	}

	public static class Timers {
		private static Timer[] _timers;
		private static bool _paused=false;
		private static Dictionary<int,int> _timersByID=new Dictionary<int,int>();

		/// <summary>
		/// Set up the timers
		/// </summary>
		/// <param name="MAX_TIMERS">Maximum timer allowance</param>
		public static void Initialise(int MAX_TIMERS) {
			_timers=new Timer[MAX_TIMERS];
			for (int i=0;i<MAX_TIMERS;i++)
				_timers[i]=null;
		}

		/// <summary>
		/// Update each timer
		/// </summary>
		/// <param name="elapsedGameTime">Game time elapsed since the last update</param>
		public static void Update(TimeSpan elapsedGameTime) {
			if (!_paused) {
				for (int i=0;i<_timers.Length;i++) {
					if (_timers[i]==null)
						continue;
					if (_timers[i].Enabled) {
						_timers[i].Elapsed+=elapsedGameTime.TotalMilliseconds;//.Milliseconds;
						if (_timers[i].Elapsed>=_timers[i].Interval) {
							_timers[i].Elapsed=0;
							_timers[i].Tick();
							if (_timers[i]==null)
								continue;
							if (_timers[i].Reps>=0) {
								_timers[i].Reps--;
								if (_timers[i].Reps==0)
									Remove(_timers[i].TimerID);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Add a timer
		/// </summary>
		/// <param name="Interval">Timer's interval</param>
		/// <param name="Reps">Number of times to repeat this timer before removal</param>
		/// <param name="Tick">Method to execute each tick</param>
		/// <returns>New timer ID</returns>
		public static int Add(int Interval,int Reps,Timer.OnTick Tick){
			return Add(new Timer(Interval,Reps,Tick));
		}

		/// <summary>
		/// Add a timer
		/// </summary>
		/// <param name="Interval">Timer's interval</param>
		/// <param name="Reps">Number of times to repeat this timer before removal</param>
		/// <param name="Tick">Method to execute each tick</param>
		/// <param name="obj">Parent object - useful for posession checking and debugging</param>
		/// <returns>New timer ID</returns>
		public static int Add(int Interval,int Reps,Timer.OnTick Tick, object obj) {
			return Add(new Timer(Interval,Reps,Tick,obj));
		}

		/// <summary>
		/// Add a timer
		/// </summary>
		/// <param name="timer">Timer to add</param>
		/// <returns>New timer ID</returns>
		public static int Add(Timer timer) {
			int i;
			for (i=0;i<_timers.Length;i++)
				if (_timers[i]==null) {
					_timers[i]=timer;
					break;
				}
			System.Diagnostics.Debug.Assert(i!=_timers.Length);
			if (i<_timers.Length) {
				_timersByID[timer.TimerID]=i;
				return timer.TimerID;
			} else
				return -1;
		}

		/// <summary>
		/// Remove a timer
		/// </summary>
		/// <param name="id">Timer ID</param>
		public static void Remove(int id) {
			if (_timersByID.ContainsKey(id)&&_timersByID[id]!=null) {
				_timers[_timersByID[id]]=null;
			} else
				for (int i=0;i<_timers.Length;i++) {
					if (_timers[i]==null)
						continue;
					if (_timers[i].TimerID==id) {
						_timers[i]=null;
						return;
					}
				}
				_timersByID.Remove(id);
		}

		/// <summary>
		/// Remove a timer
		/// </summary>
		/// <param name="timer">Timer to remove</param>
		public static void Remove(Timer timer) {
			for (int i=0;i<_timers.Length;i++)
				if (_timers[i]==timer) {
					if (_timersByID.ContainsKey(_timers[i].TimerID))
						_timersByID.Remove(_timers[i].TimerID);
					_timers[i]=null;
					return;
				}
		}

		/// <summary>
		/// Get a timer by ID
		/// </summary>
		/// <param name="id">ID of timer</param>
		/// <returns>Timer</returns>
		public static Timer Get(int id) {
			if (_timersByID.ContainsKey(id))
				return _timers[_timersByID[id]];
			else
				for (int i=0;i<_timers.Length;i++) {
					if (_timers[i]==null)
						continue;
					if (_timers[i].TimerID==id)
						return _timers[i];
				}
			return null;
		}

		/// <summary>
		/// Get all the timers
		/// </summary>
		/// <returns>Array of timers</returns>
		public static Timer[] GetAll() {
			return _timers;
		}

		/// <summary>
		/// Enable or disable a timer
		/// </summary>
		/// <param name="index">Timer ID</param>
		/// <param name="enabled">Is it to be enabled?</param>
		public static void Enabled(int index,bool enabled) {
			_timers[index].Enabled=enabled;
		}

		/// <summary>
		/// Pause a timer
		/// </summary>
		public static void Pause() {
			_paused=true;
		}

		/// <summary>
		/// Resume a timer
		/// </summary>
		public static void Resume() {
			_paused=false;
		}
	}
}
