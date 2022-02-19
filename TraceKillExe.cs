using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000CA RID: 202
	internal class TraceKillExe : ExeModule
	{
		// Token: 0x06000415 RID: 1045 RVA: 0x00040ED0 File Offset: 0x0003F0D0
		public TraceKillExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "TraceKill";
			this.ramCost = 600;
			this.IdentifierName = "TraceKill";
			this.targetIP = this.os.thisComputer.ip;
			this.effectSB = new SpriteBatch(GuiData.spriteBatch.GraphicsDevice);
			this.circle = this.os.content.Load<Texture2D>("Circle");
			this.traceKillSound = this.os.content.Load<SoundEffect>("SFX/TraceKill");
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00040FE0 File Offset: 0x0003F1E0
		public override void Update(float t)
		{
			base.Update(t);
			this.UpdateEffect(t);
			bool flag = this.os.connectedComp != null && this.os.connectedComp.idName == "dGibson";
			if (!this.isExiting && !flag)
			{
				if (this.os.traceTracker.timeSinceFreezeRequest > 0.25f)
				{
					this.traceKillSound.Play();
				}
				this.os.traceTracker.timeSinceFreezeRequest = 0f;
			}
			if (this.BotBarcode != null)
			{
				this.BotBarcode.Update(t * (this.os.traceTracker.active ? 3f : 2f));
			}
			if (this.os.traceTracker.active)
			{
				this.traceActivityTimer += t;
			}
			else
			{
				this.traceActivityTimer = 0f;
			}
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x000410E8 File Offset: 0x0003F2E8
		private void UpdateEffect(float t)
		{
			float num = 1f;
			if (this.os.traceTracker.active)
			{
				num = 1.6f;
			}
			this.timer += t * num;
			Vector2 vector = new Vector2((float)Math.Sin((double)this.timer * 2.0), (float)Math.Sin((double)this.timer)) * 0.4f + new Vector2(0.5f);
			if (this.isOnFocusPoint && this.focusPointTransitionTime >= 1.2f)
			{
				this.EffectFocus = this.focusPointLocation;
				this.timeOnFocusPoint += t;
				if (!this.hasDoneBurstForThisFocusPoint)
				{
					Vector2 location = this.focusPointLocation;
					TraceKillExe.PointImpactEffect item = new TraceKillExe.PointImpactEffect(location, this.os);
					this.ImpactEffects.Add(item);
					this.hasDoneBurstForThisFocusPoint = true;
				}
				if (this.timeOnFocusPoint >= this.focusPointIdleTime)
				{
					this.isOnFocusPoint = false;
					this.focusPointTransitionTime = 1.2f;
					this.timeTillNextFocusPoint = Utils.randm(4f);
				}
			}
			else if ((this.isOnFocusPoint && this.focusPointTransitionTime < 1.2f) || (!this.isOnFocusPoint && this.focusPointTransitionTime > 0f))
			{
				this.focusPointTransitionTime += t * (this.isOnFocusPoint ? 1f : -1f);
				float point = this.focusPointTransitionTime / 1.2f;
				this.EffectFocus = Vector2.Lerp(vector, this.focusPointLocation, Utils.QuadraticOutCurve(point));
			}
			else
			{
				this.EffectFocus = vector;
				this.timeTillNextFocusPoint -= t;
				if (this.timeTillNextFocusPoint <= 0f)
				{
					this.isOnFocusPoint = true;
					this.focusPointTransitionTime = 0f;
					this.timeOnFocusPoint = 0f;
					this.hasDoneBurstForThisFocusPoint = false;
					this.focusPointIdleTime = Utils.randm(0.1f);
					this.focusPointLocation = new Vector2(Utils.randm(1f), Utils.randm(1f));
				}
			}
			for (int i = 0; i < this.ImpactEffects.Count; i++)
			{
				TraceKillExe.PointImpactEffect value = this.ImpactEffects[i];
				value.timeEnabled += t;
				if (value.timeEnabled > 3f)
				{
					this.ImpactEffects.RemoveAt(i);
					i--;
				}
				else
				{
					this.ImpactEffects[i] = value;
				}
			}
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x000413AC File Offset: 0x0003F5AC
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			this.DrawEffect(t);
			if (this.bounds.Height > 100)
			{
				Rectangle dest = new Rectangle(this.bounds.X + 1, this.bounds.Y + 60, (int)((double)this.bounds.Width * 0.7), 40);
				TextItem.doRightAlignedBackingLabelFill(dest, this.os.traceTracker.active ? "SUPPRESSION ACTIVE" : "        SCANNING...", GuiData.titlefont, TraceKillExe.BoatBarColor, this.os.traceTracker.active ? Color.Lerp(Color.Red, this.os.brightLockedColor, Utils.rand(1f)) : (Color.White * 0.8f));
				if (!this.os.traceTracker.active)
				{
					dest.Y += dest.Height + 2;
					dest.Height = 16;
					TextItem.doRightAlignedBackingLabel(dest, "TraceKill v0.8011", GuiData.detailfont, TraceKillExe.BoatBarColor, Color.DarkGray);
				}
			}
			if (this.bounds.Height > 40)
			{
				if (this.BotBarcode == null)
				{
					this.BotBarcode = new BarcodeEffect(this.bounds.Width - 2, true, false);
				}
				int num = 12;
				int num2 = 22;
				this.BotBarcode.Draw(this.bounds.X + 1, this.bounds.Y + this.bounds.Height - (num + num2), this.bounds.Width - 2, num, this.spriteBatch, new Color?(TraceKillExe.BoatBarColor));
				Rectangle destinationRectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height - num2 - 1, this.bounds.Width - 2, num2);
				this.spriteBatch.Draw(Utils.white, destinationRectangle, TraceKillExe.BoatBarColor);
				int num3 = 130;
				if (Button.doButton(34004301, destinationRectangle.X + destinationRectangle.Width - num3 - 2, destinationRectangle.Y + 2, num3, destinationRectangle.Height - 4, "EXIT", new Color?(this.os.brightLockedColor)))
				{
					this.isExiting = true;
				}
				Rectangle dest2 = new Rectangle(destinationRectangle.X + 2, destinationRectangle.Y + 5, destinationRectangle.Width - num3 - 4, destinationRectangle.Height / 2);
				TextItem.doRightAlignedBackingLabel(dest2, this.traceActivityTimer.ToString("0.000000"), GuiData.detailfont, Color.Transparent, Color.White * 0.8f);
				dest2.Y += dest2.Height - 2;
				TextItem.doRightAlignedBackingLabel(dest2, (this.os.connectedComp != null) ? this.os.connectedComp.ip : "UNKNOWN", GuiData.detailfont, Color.Transparent, Color.White * 0.8f);
			}
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00041714 File Offset: 0x0003F914
		private void DrawEffect(float t)
		{
			int num = 16;
			Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + num, this.bounds.Width - 2, this.bounds.Height - (num + 2));
			if (this.effectTarget == null || this.effectTarget.Width != rectangle.Width || this.effectTarget.Height < rectangle.Height)
			{
				if (this.effectTarget != null)
				{
					this.effectTarget.Dispose();
				}
				this.effectTarget = new RenderTarget2D(this.spriteBatch.GraphicsDevice, rectangle.Width, rectangle.Height, false, SurfaceFormat.Rgba64, DepthFormat.None, 4, RenderTargetUsage.PlatformContents);
			}
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			this.spriteBatch.GraphicsDevice.SetRenderTarget(this.effectTarget);
			this.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
			this.effectSB.Begin();
			Rectangle dest = rectangle;
			dest.X = 0;
			dest.Y = 0;
			this.DrawEffectFill(this.effectSB, dest);
			this.DrawImpactEffects(this.effectSB, dest);
			this.effectSB.End();
			this.spriteBatch.GraphicsDevice.SetRenderTarget(currentRenderTarget);
			Rectangle value = rectangle;
			value.X = (value.Y = 0);
			this.spriteBatch.Draw(this.effectTarget, rectangle, new Rectangle?(value), Color.White);
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x000418A8 File Offset: 0x0003FAA8
		private void DrawImpactEffects(SpriteBatch sb, Rectangle dest)
		{
			Color value = this.os.traceTracker.active ? Color.Red : Utils.AddativeWhite;
			for (int i = 0; i < this.ImpactEffects.Count; i++)
			{
				TraceKillExe.PointImpactEffect pointImpactEffect = this.ImpactEffects[i];
				Vector2 vector = new Vector2((float)dest.X + pointImpactEffect.location.X * (float)dest.Width, (float)dest.Y + pointImpactEffect.location.Y * (float)dest.Height);
				if (pointImpactEffect.timeEnabled <= 1f)
				{
					float num = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(pointImpactEffect.timeEnabled / 1f));
					pointImpactEffect.cne.color = value * num;
					pointImpactEffect.cne.ScaleFactor = num;
					sb.Draw(this.circle, vector, null, value * (1f - num), 0f, new Vector2((float)(this.circle.Width / 2), (float)(this.circle.Height / 2)), num / (float)this.circle.Width * 30f, SpriteEffects.None, 0.7f);
				}
				else
				{
					float num2 = Utils.QuadraticOutCurve((pointImpactEffect.timeEnabled - 1f) / 2f);
					pointImpactEffect.cne.color = value * (1f - num2);
					pointImpactEffect.cne.ScaleFactor = 1f;
				}
				pointImpactEffect.cne.draw(sb, vector);
			}
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x00041ACC File Offset: 0x0003FCCC
		private void DrawEffectFill(SpriteBatch sb, Rectangle dest)
		{
			sb.Draw(Utils.white, dest, this.os.indentBackgroundColor * 0.8f);
			float num = 10f;
			float num2 = 2.5f;
			float lineThickness = 2f;
			float num3 = 0.08f;
			Color color = this.os.traceTracker.active ? this.os.lockedColor : (Utils.VeryDarkGray * 0.4f);
			Color value = this.os.traceTracker.active ? this.os.brightLockedColor : Color.Gray;
			Vector2 targetPos = this.EffectFocus * new Vector2((float)dest.Width, (float)dest.Height);
			List<Action> list = new List<Action>();
			int num4 = 0;
			Vector2 vector = new Vector2((float)dest.X - num / 2f, (float)dest.Y - num);
			while (vector.Y + lineThickness < (float)(dest.Y + dest.Height) + num)
			{
				vector.X = (float)dest.X - num / 2f;
				while (vector.X + lineThickness < (float)(dest.X + dest.Width) + num)
				{
					Vector2 pos = vector;
					float amount = 1f - Utils.QuadraticOutCurve(Vector2.Distance(pos, targetPos) / (float)dest.Height);
					Color highlightColor = Color.Lerp(color, value, amount);
					float length = Math.Min(num * 1.1f, Vector2.Distance(vector, targetPos));
					this.DrawTracerLine(sb, pos, lineThickness, targetPos, length, color, highlightColor);
					list.Add(delegate
					{
						this.DrawTracerLineShadow(sb, pos, lineThickness, targetPos, length, Color.Black * 0.6f);
					});
					vector.X += num + lineThickness / 2f;
				}
				num4++;
				num += num2;
				lineThickness += num3;
				vector.Y += num;
				if (num <= 0f)
				{
					break;
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i]();
			}
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x00041D90 File Offset: 0x0003FF90
		private void DrawTracerLine(SpriteBatch sb, Vector2 pos, float thickness, Vector2 target, float length, Color baseColor, Color highlightColor)
		{
			float rotation = (float)Math.Atan2((double)(target.Y - pos.Y), (double)(target.X - pos.X));
			Rectangle destinationRectangle = new Rectangle((int)pos.X, (int)pos.Y, (int)length, (int)thickness);
			sb.Draw(Utils.white, destinationRectangle, null, baseColor, rotation, Vector2.Zero, SpriteEffects.None, 0.7f);
			sb.Draw(Utils.gradientLeftRight, destinationRectangle, null, highlightColor, rotation, Vector2.Zero, SpriteEffects.None, 0.7f);
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00041E2C File Offset: 0x0004002C
		private void DrawTracerLineShadow(SpriteBatch sb, Vector2 pos, float thickness, Vector2 target, float length, Color color)
		{
			float rotation = (float)Math.Atan2((double)(target.Y - pos.Y), (double)(target.X - pos.X));
			sb.Draw(Utils.gradient, pos, null, color, rotation, Vector2.Zero, new Vector2(length, (float)((int)thickness)), SpriteEffects.None, 0.6f);
			Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, (int)length, (int)thickness);
		}

		// Token: 0x040004EF RID: 1263
		private const float TIME_BETWEEN_FOCUS_POINTS = 4f;

		// Token: 0x040004F0 RID: 1264
		private const float FOCUS_POINT_TRANSITION_TIME = 1.2f;

		// Token: 0x040004F1 RID: 1265
		private const float MAX_FOCUS_POINT_IDLE_TIME = 0.1f;

		// Token: 0x040004F2 RID: 1266
		private static Color BoatBarColor = new Color(0, 0, 0, 200);

		// Token: 0x040004F3 RID: 1267
		private RenderTarget2D effectTarget;

		// Token: 0x040004F4 RID: 1268
		private SpriteBatch effectSB;

		// Token: 0x040004F5 RID: 1269
		private SoundEffect traceKillSound;

		// Token: 0x040004F6 RID: 1270
		private Vector2 EffectFocus = new Vector2(0.5f);

		// Token: 0x040004F7 RID: 1271
		private float timer = 0f;

		// Token: 0x040004F8 RID: 1272
		private Vector2 focusPointLocation = Vector2.Zero;

		// Token: 0x040004F9 RID: 1273
		private float timeTillNextFocusPoint = 1f;

		// Token: 0x040004FA RID: 1274
		private bool isOnFocusPoint = false;

		// Token: 0x040004FB RID: 1275
		private float focusPointTransitionTime = 0f;

		// Token: 0x040004FC RID: 1276
		private float timeOnFocusPoint = 0f;

		// Token: 0x040004FD RID: 1277
		private float focusPointIdleTime = 0f;

		// Token: 0x040004FE RID: 1278
		private bool hasDoneBurstForThisFocusPoint = false;

		// Token: 0x040004FF RID: 1279
		private List<TraceKillExe.PointImpactEffect> ImpactEffects = new List<TraceKillExe.PointImpactEffect>();

		// Token: 0x04000500 RID: 1280
		private Texture2D circle;

		// Token: 0x04000501 RID: 1281
		private BarcodeEffect BotBarcode;

		// Token: 0x04000502 RID: 1282
		private float traceActivityTimer = 0f;

		// Token: 0x020000CB RID: 203
		public struct PointImpactEffect
		{
			// Token: 0x0600041F RID: 1055 RVA: 0x00041EC0 File Offset: 0x000400C0
			public PointImpactEffect(Vector2 location, OS os)
			{
				this.cne = new ConnectedNodeEffect(os, true);
				this.timeEnabled = 0f;
				this.location = location;
				this.scaleModifier = 1f;
				this.HasHighlightCircle = true;
			}

			// Token: 0x04000503 RID: 1283
			public const float TransInTime = 1f;

			// Token: 0x04000504 RID: 1284
			public const float TransOutTime = 2f;

			// Token: 0x04000505 RID: 1285
			public ConnectedNodeEffect cne;

			// Token: 0x04000506 RID: 1286
			public float timeEnabled;

			// Token: 0x04000507 RID: 1287
			public Vector2 location;

			// Token: 0x04000508 RID: 1288
			public float scaleModifier;

			// Token: 0x04000509 RID: 1289
			public bool HasHighlightCircle;
		}
	}
}
