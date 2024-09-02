using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Nez.ECS.Headless
{
	public class CoreHeadless
	{
		public static ContentManagerHeadless Content;

		public static GameServiceContainer Services;

		public static CoreHeadless Instance => _instance;
		internal static CoreHeadless _instance;

		FastList<GlobalManager> _globalManagers = new FastList<GlobalManager>();

		public TimeSpan InactiveSleepTime
		{
			get { return _inactiveSleepTime; }
			set
			{
				if (value < TimeSpan.Zero)
					throw new ArgumentOutOfRangeException("The time must be positive.");

				_inactiveSleepTime = value;
			}
		}

		/// <summary>
		/// The maximum amount of time we will frameskip over and only perform Update calls with no Draw calls.
		/// MonoGame extension.
		/// </summary>
		public TimeSpan MaxElapsedTime
		{
			get { return _maxElapsedTime; }
			set
			{
				if (value < TimeSpan.Zero)
					throw new ArgumentOutOfRangeException(
						"The time must be positive.");

				if (value < _targetElapsedTime)
					throw new ArgumentOutOfRangeException(
						"The time must be at least TargetElapsedTime");

				_maxElapsedTime = value;
			}
		}

		/// <summary>
		/// The time between frames when running with a fixed time step. <seealso cref="IsFixedTimeStep"/>
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Target elapsed time must be strictly larger than zero.</exception>
		public TimeSpan TargetElapsedTime
		{
			get { return _targetElapsedTime; }
			set
			{
				if (value != _targetElapsedTime)
				{
					_targetElapsedTime = value;
				}
			}
		}

		/// <summary>
		/// Indicates if this game is running with a fixed time between frames.
		/// 
		/// When set to <code>true</code> the target time between frames is
		/// given by <see cref="TargetElapsedTime"/>.
		/// </summary>
		public bool IsFixedTimeStep
		{
			get { return _isFixedTimeStep; }
			set { _isFixedTimeStep = value; }
		}

		private TimeSpan _accumulatedElapsedTime;
		private readonly GameTime _gameTime = new GameTime();
		private Stopwatch _gameTimer;
		private long _previousTicks = 0;
		private int _updateFrameLag;

		private bool _initialized = false;
		private bool _isFixedTimeStep = true;

		private TimeSpan _targetElapsedTime = TimeSpan.FromTicks(166667); // 60fps
		private TimeSpan _inactiveSleepTime = TimeSpan.FromSeconds(0.02);

		private TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500);

		public CoreHeadless()
		{
			_instance = this;
			Services = new GameServiceContainer();
			Content = new ContentManagerHeadless(Services);
		}

		//public virtual void Initialize() { }

		public virtual void Tick()
		{
		// todo: run game loop.
		// update time.
		// call Update().
		// NOTE: This code is very sensitive and can break very badly
		// with even what looks like a safe change.  Be sure to test 
		// any change fully in both the fixed and variable timestep 
		// modes across multiple devices and platforms.

		RetryTick:

			if ((InactiveSleepTime.TotalMilliseconds >= 1.0))
			{
#if WINDOWS_UAP
                lock (_locker)
                    System.Threading.Monitor.Wait(_locker, (int)InactiveSleepTime.TotalMilliseconds);
#else
				System.Threading.Thread.Sleep((int)InactiveSleepTime.TotalMilliseconds);
#endif
			}

			// Advance the accumulated elapsed time.
			if (_gameTimer == null)
			{
				_gameTimer = new Stopwatch();
				_gameTimer.Start();
			}
			var currentTicks = _gameTimer.Elapsed.Ticks;
			_accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - _previousTicks);
			_previousTicks = currentTicks;

			if (IsFixedTimeStep && _accumulatedElapsedTime < TargetElapsedTime)
			{
				// Sleep for as long as possible without overshooting the update time
				var sleepTime = (TargetElapsedTime - _accumulatedElapsedTime).TotalMilliseconds;
				// We only have a precision timer on Windows, so other platforms may still overshoot
#if WINDOWS && !DESKTOPGL
                MonoGame.Framework.Utilities.TimerHelper.SleepForNoMoreThan(sleepTime);
#elif WINDOWS_UAP
                lock (_locker)
                    if (sleepTime >= 2.0)
                        System.Threading.Monitor.Wait(_locker, 1);
#elif DESKTOPGL || ANDROID || IOS
                if (sleepTime >= 2.0)
                    System.Threading.Thread.Sleep(1);
#endif
				// Keep looping until it's time to perform the next update
				goto RetryTick;
			}

			// Do not allow any update to take longer than our maximum.
			if (_accumulatedElapsedTime > _maxElapsedTime)
				_accumulatedElapsedTime = _maxElapsedTime;

			if (IsFixedTimeStep)
			{
				_gameTime.ElapsedGameTime = TargetElapsedTime;
				var stepCount = 0;

				// Perform as many full fixed length time steps as we can.
				while (_accumulatedElapsedTime >= TargetElapsedTime)
				{
					_gameTime.TotalGameTime += TargetElapsedTime;
					_accumulatedElapsedTime -= TargetElapsedTime;
					++stepCount;

					Time.Update((float)_gameTime.ElapsedGameTime.TotalSeconds);
					Update(Time.DeltaTime);
				}

				//Every update after the first accumulates lag
				_updateFrameLag += Math.Max(0, stepCount - 1);

				//If we think we are running slowly, wait until the lag clears before resetting it
				if (_gameTime.IsRunningSlowly)
				{
					if (_updateFrameLag == 0)
						_gameTime.IsRunningSlowly = false;
				}
				else if (_updateFrameLag >= 5)
				{
					//If we lag more than 5 frames, start thinking we are running slowly
					_gameTime.IsRunningSlowly = true;
				}

				//Every time we just do one update and one draw, then we are not running slowly, so decrease the lag
				if (stepCount == 1 && _updateFrameLag > 0)
					_updateFrameLag--;

				// Draw needs to know the total elapsed time
				// that occured for the fixed length updates.
				_gameTime.ElapsedGameTime = TimeSpan.FromTicks(TargetElapsedTime.Ticks * stepCount);
			}
			else
			{
				// Perform a single variable length update.
				_gameTime.ElapsedGameTime = _accumulatedElapsedTime;
				_gameTime.TotalGameTime += _accumulatedElapsedTime;
				_accumulatedElapsedTime = TimeSpan.Zero;

				Time.Update((float)_gameTime.ElapsedGameTime.TotalSeconds);
				Update(Time.DeltaTime);
			}
		}

		public virtual void Update(float deltaTime)
		{
			// todo: update engine processes.
		}

		#region Global Managers

		/// <summary>
		/// adds a global manager object that will have its update method called each frame before Scene.update is called
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <param name="manager">Manager.</param>
		public static void RegisterGlobalManager(GlobalManager manager)
		{
			_instance._globalManagers.Add(manager);
			manager.Enabled = true;
		}

		/// <summary>
		/// removes the global manager object
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <param name="manager">Manager.</param>
		public static void UnregisterGlobalManager(GlobalManager manager)
		{
			_instance._globalManagers.Remove(manager);
			manager.Enabled = false;
		}

		/// <summary>
		/// gets the global manager of type T
		/// </summary>
		/// <returns>The global manager.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetGlobalManager<T>() where T : GlobalManager
		{
			for (var i = 0; i < _instance._globalManagers.Length; i++)
			{
				if (_instance._globalManagers.Buffer[i] is T)
					return _instance._globalManagers.Buffer[i] as T;
			}

			return null;
		}

		#endregion
	}
}
