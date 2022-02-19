using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x020000EE RID: 238
	public abstract class GameScreen
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060004F7 RID: 1271 RVA: 0x0004EC74 File Offset: 0x0004CE74
		// (set) Token: 0x060004F8 RID: 1272 RVA: 0x0004EC8C File Offset: 0x0004CE8C
		public bool IsPopup
		{
			get
			{
				return this.isPopup;
			}
			protected set
			{
				this.isPopup = value;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x0004EC98 File Offset: 0x0004CE98
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x0004ECB0 File Offset: 0x0004CEB0
		public TimeSpan TransitionOnTime
		{
			get
			{
				return this.transitionOnTime;
			}
			protected set
			{
				this.transitionOnTime = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x0004ECBC File Offset: 0x0004CEBC
		// (set) Token: 0x060004FC RID: 1276 RVA: 0x0004ECD4 File Offset: 0x0004CED4
		public TimeSpan TransitionOffTime
		{
			get
			{
				return this.transitionOffTime;
			}
			protected set
			{
				this.transitionOffTime = value;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x0004ECE0 File Offset: 0x0004CEE0
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x0004ECF8 File Offset: 0x0004CEF8
		public float TransitionPosition
		{
			get
			{
				return this.transitionPosition;
			}
			protected set
			{
				this.transitionPosition = value;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x0004ED04 File Offset: 0x0004CF04
		public byte TransitionAlpha
		{
			get
			{
				return (byte)(255f - this.TransitionPosition * 255f);
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000500 RID: 1280 RVA: 0x0004ED2C File Offset: 0x0004CF2C
		// (set) Token: 0x06000501 RID: 1281 RVA: 0x0004ED44 File Offset: 0x0004CF44
		public ScreenState ScreenState
		{
			get
			{
				return this.screenState;
			}
			protected set
			{
				this.screenState = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000502 RID: 1282 RVA: 0x0004ED50 File Offset: 0x0004CF50
		// (set) Token: 0x06000503 RID: 1283 RVA: 0x0004ED68 File Offset: 0x0004CF68
		public bool IsExiting
		{
			get
			{
				return this.isExiting;
			}
			protected internal set
			{
				this.isExiting = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x0004ED74 File Offset: 0x0004CF74
		public bool IsActive
		{
			get
			{
				return !this.otherScreenHasFocus && (this.screenState == ScreenState.TransitionOn || this.screenState == ScreenState.Active);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000505 RID: 1285 RVA: 0x0004EDA8 File Offset: 0x0004CFA8
		// (set) Token: 0x06000506 RID: 1286 RVA: 0x0004EDC0 File Offset: 0x0004CFC0
		public ScreenManager ScreenManager
		{
			get
			{
				return this.screenManager;
			}
			internal set
			{
				this.screenManager = value;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000507 RID: 1287 RVA: 0x0004EDCC File Offset: 0x0004CFCC
		// (set) Token: 0x06000508 RID: 1288 RVA: 0x0004EDE4 File Offset: 0x0004CFE4
		public PlayerIndex? ControllingPlayer
		{
			get
			{
				return this.controllingPlayer;
			}
			internal set
			{
				this.controllingPlayer = value;
			}
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x0004EDEE File Offset: 0x0004CFEE
		public virtual void LoadContent()
		{
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x0004EDF1 File Offset: 0x0004CFF1
		public virtual void UnloadContent()
		{
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x0004EDF4 File Offset: 0x0004CFF4
		public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			this.otherScreenHasFocus = otherScreenHasFocus;
			if (this.isExiting)
			{
				this.screenState = ScreenState.TransitionOff;
				if (!this.UpdateTransition(gameTime, this.transitionOffTime, 1))
				{
					this.ScreenManager.RemoveScreen(this);
				}
			}
			else if (coveredByOtherScreen)
			{
				if (this.UpdateTransition(gameTime, this.transitionOffTime, 1))
				{
					this.screenState = ScreenState.TransitionOff;
				}
				else
				{
					this.screenState = ScreenState.Hidden;
				}
			}
			else if (this.UpdateTransition(gameTime, this.transitionOnTime, -1))
			{
				this.screenState = ScreenState.TransitionOn;
			}
			else
			{
				this.screenState = ScreenState.Active;
			}
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x0004EEA4 File Offset: 0x0004D0A4
		private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
		{
			float num;
			if (time == TimeSpan.Zero)
			{
				num = 1f;
			}
			else
			{
				num = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
			}
			this.transitionPosition += num * (float)direction;
			bool result;
			if ((direction < 0 && this.transitionPosition <= 0f) || (direction > 0 && this.transitionPosition >= 1f))
			{
				this.transitionPosition = MathHelper.Clamp(this.transitionPosition, 0f, 1f);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x0004EF48 File Offset: 0x0004D148
		public virtual void HandleInput(InputState input)
		{
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x0004EF4B File Offset: 0x0004D14B
		public virtual void Draw(GameTime gameTime)
		{
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0004EF50 File Offset: 0x0004D150
		public void ExitScreen()
		{
			if (this.TransitionOffTime == TimeSpan.Zero)
			{
				this.ScreenManager.RemoveScreen(this);
			}
			else
			{
				this.isExiting = true;
			}
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0004EF8F File Offset: 0x0004D18F
		public virtual void inputMethodChanged(bool usingGamePad)
		{
		}

		// Token: 0x0400059E RID: 1438
		private bool isPopup = false;

		// Token: 0x0400059F RID: 1439
		private TimeSpan transitionOnTime = TimeSpan.Zero;

		// Token: 0x040005A0 RID: 1440
		private TimeSpan transitionOffTime = TimeSpan.Zero;

		// Token: 0x040005A1 RID: 1441
		private float transitionPosition = 1f;

		// Token: 0x040005A2 RID: 1442
		private ScreenState screenState = ScreenState.TransitionOn;

		// Token: 0x040005A3 RID: 1443
		private bool isExiting = false;

		// Token: 0x040005A4 RID: 1444
		private bool otherScreenHasFocus;

		// Token: 0x040005A5 RID: 1445
		private ScreenManager screenManager;

		// Token: 0x040005A6 RID: 1446
		private PlayerIndex? controllingPlayer;
	}
}
