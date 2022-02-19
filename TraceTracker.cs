using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000173 RID: 371
	internal class TraceTracker
	{
		// Token: 0x0600094C RID: 2380 RVA: 0x0009A6E4 File Offset: 0x000988E4
		public TraceTracker(OS _os)
		{
			this.os = _os;
			this.timerColor = new Color(170, 0, 0);
			this.timer = 0f;
			this.active = false;
			if (TraceTracker.font == null)
			{
				TraceTracker.font = this.os.content.Load<SpriteFont>("Kremlin");
				TraceTracker.font.Spacing = 11f;
				TraceTracker.beep = this.os.content.Load<SoundEffect>("SFX/beep");
			}
			this.drawtext = "TRACE :";
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0009A79C File Offset: 0x0009899C
		public void Update(float t)
		{
			bool flag = false;
			this.timeSinceFreezeRequest += t;
			if (this.timeSinceFreezeRequest < 0f)
			{
				this.timeSinceFreezeRequest = 1f;
			}
			if (this.timeSinceFreezeRequest < 0.2f)
			{
				flag = true;
			}
			if (this.active)
			{
				if (this.os.connectedComp == null || !this.os.connectedComp.ip.Equals(this.target.ip))
				{
					this.active = false;
					if (this.timer < 0.5f)
					{
						AchievementsManager.Unlock("trace_close", false);
					}
					if (this.os.connectedComp == null)
					{
						Console.WriteLine("Trace Ended - connection null");
					}
					else
					{
						Console.WriteLine("Trace Ended - connection changed - was " + this.target.ip + " now is " + this.os.connectedComp.ip);
					}
				}
				else if (!flag)
				{
					float num = Settings.AllTraceTimeSlowed ? 0.055f : 1f;
					this.timer -= t * this.trackSpeedFactor * num;
					if (this.timer <= 0f)
					{
						this.timer = 0f;
						this.active = false;
						this.os.timerExpired();
					}
				}
				float num2 = this.timer / this.startingTimer * 100f;
				float num3 = (num2 < 45f) ? ((num2 < 15f) ? 1f : 5f) : 10f;
				if (num2 % num3 > this.lastFrameTime % num3)
				{
					TraceTracker.beep.Play(0.5f, 0f, 0f);
					this.os.warningFlash();
				}
				this.lastFrameTime = num2;
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0009A9B0 File Offset: 0x00098BB0
		public void start(float t)
		{
			if (!this.active)
			{
				this.trackSpeedFactor = 1f;
				this.startingTimer = t;
				this.timer = t;
				this.active = true;
				this.os.warningFlash();
				this.target = ((this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp);
				Console.WriteLine("Warning flash");
			}
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x0009AA2D File Offset: 0x00098C2D
		public void stop()
		{
			this.active = false;
			this.trackSpeedFactor = 1f;
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0009AA44 File Offset: 0x00098C44
		public void Draw(SpriteBatch sb)
		{
			if (this.active)
			{
				string text = (this.timer / this.startingTimer * 100f).ToString("00.00");
				Vector2 vector = TraceTracker.font.MeasureString(text);
				Vector2 position = new Vector2(10f, (float)sb.GraphicsDevice.Viewport.Height - vector.Y);
				sb.DrawString(TraceTracker.font, text, position, this.timerColor);
				position.Y -= 25f;
				sb.DrawString(TraceTracker.font, this.drawtext, position, this.timerColor, 0f, Vector2.Zero, new Vector2(0.3f), SpriteEffects.None, 0.5f);
			}
		}

		// Token: 0x04000ADF RID: 2783
		private OS os;

		// Token: 0x04000AE0 RID: 2784
		private float timer;

		// Token: 0x04000AE1 RID: 2785
		private float lastFrameTime;

		// Token: 0x04000AE2 RID: 2786
		private float startingTimer;

		// Token: 0x04000AE3 RID: 2787
		public bool active;

		// Token: 0x04000AE4 RID: 2788
		public float timeSinceFreezeRequest = 0f;

		// Token: 0x04000AE5 RID: 2789
		public float trackSpeedFactor = 1f;

		// Token: 0x04000AE6 RID: 2790
		private static SpriteFont font;

		// Token: 0x04000AE7 RID: 2791
		private static SoundEffect beep;

		// Token: 0x04000AE8 RID: 2792
		private Computer target;

		// Token: 0x04000AE9 RID: 2793
		private string drawtext;

		// Token: 0x04000AEA RID: 2794
		private Color timerColor;
	}
}
