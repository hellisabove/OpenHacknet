using System;
using Hacknet.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000B3 RID: 179
	internal class TrailLoadingSpinnerEffect
	{
		// Token: 0x0600039B RID: 923 RVA: 0x0003750B File Offset: 0x0003570B
		public TrailLoadingSpinnerEffect(OS operatingSystem)
		{
			this.circle = TextureBank.load("Circle", operatingSystem.content);
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0003752C File Offset: 0x0003572C
		public void Draw(Rectangle bounds, SpriteBatch spriteBatch, float totalTime, float timeRemaining, float extraTime = 0f, Color? color = null)
		{
			Color c = (color != null) ? color.Value : OS.currentInstance.highlightColor;
			Rectangle destinationRectangle = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 3);
			bool flag = false;
			if (this.target == null)
			{
				this.target = new RenderTarget2D(spriteBatch.GraphicsDevice, destinationRectangle.Width, destinationRectangle.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
				this.internalSB = new SpriteBatch(spriteBatch.GraphicsDevice);
				flag = true;
			}
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			spriteBatch.GraphicsDevice.SetRenderTarget(this.target);
			Rectangle dest = new Rectangle(0, 0, destinationRectangle.Width, destinationRectangle.Height);
			this.internalSB.Begin();
			if (flag)
			{
				this.internalSB.GraphicsDevice.Clear(Color.Transparent);
			}
			this.DrawLoading(timeRemaining, totalTime, dest, this.internalSB, c, extraTime);
			this.internalSB.End();
			spriteBatch.GraphicsDevice.SetRenderTarget(currentRenderTarget);
			spriteBatch.Draw(this.target, destinationRectangle, Color.White);
		}

		// Token: 0x0600039D RID: 925 RVA: 0x000376A4 File Offset: 0x000358A4
		public void Draw2(Rectangle bounds, SpriteBatch spriteBatch, float totalTime, float timeRemaining, float extraTime = 0f, float dt = 0f)
		{
			if (this.flyout == null)
			{
				this.flyout = new FlyoutEffect(GuiData.spriteBatch.GraphicsDevice, OS.currentInstance.content, bounds.Width, bounds.Height);
			}
			this.flyout.Draw(dt, bounds, spriteBatch, delegate(SpriteBatch sb, Rectangle dest)
			{
				this.DrawLoading(timeRemaining, totalTime, dest, sb, OS.currentInstance.highlightColor, extraTime);
			});
		}

		// Token: 0x0600039E RID: 926 RVA: 0x00037730 File Offset: 0x00035930
		private void DrawLoading(float timeRemaining, float totalTime, Rectangle dest, SpriteBatch sb, Color c, float timeAdd = 0f)
		{
			float num = 20f;
			float num2 = (float)dest.Width / 2f - (num + 2f);
			Vector2 loaderCentre = new Vector2((float)dest.X + (float)dest.Width / 2f, (float)dest.Y + (float)dest.Height / 2f);
			float num3 = totalTime - timeRemaining + timeAdd;
			float num4 = Math.Min(1f, (num3 + timeAdd) / totalTime);
			Rectangle destinationRectangle = new Rectangle(0, 0, this.target.Width, this.target.Height);
			sb.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray * 0.05f);
			this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, num, num3 * 0.2f, 1f, sb, Utils.AddativeWhite, 10, 1f);
			this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, num + num2 / 2f * num4, num3 * -0.4f, 0.7f, sb, Utils.AddativeWhite * (1f - num4), 10, 0.8f);
			this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, num + num2 * num4, num3 * 0.5f, 0.52f, sb, Utils.AddativeWhite * (1f - num4), 10, 0.8f);
			int num5 = 30;
			for (int i = 0; i < num5; i++)
			{
				float num6 = (float)i / (float)num5;
				this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, num + num2 * num6 * num4, num3 * -0.4f * num6, 2f * num6, sb, c, 6, 0.2f + num6 * 0.2f);
			}
		}

		// Token: 0x0600039F RID: 927 RVA: 0x000378E4 File Offset: 0x00035AE4
		private void DrawLoadingCircle(float timeRemaining, float totalTime, Rectangle dest, Vector2 loaderCentre, float loaderRadius, float baseRotationAdd, float rotationRateRPS, SpriteBatch sb, Color c, int NumberOfCircles = 10, float scaleMod = 1f)
		{
			float num = totalTime - timeRemaining;
			for (int i = 0; i < NumberOfCircles; i++)
			{
				float num2 = (float)i / (float)NumberOfCircles;
				float num3 = 2f;
				float num4 = 1f;
				float num5 = 6.2831855f;
				float num6 = num5 + num4;
				float num7 = num2 * num3;
				if (num > num7)
				{
					float num8 = num / num7 * rotationRateRPS % num6;
					if (num8 >= num5)
					{
						num8 = 0f;
					}
					num8 = num5 * Utils.QuadraticOutCurve(num8 / num5);
					num8 += baseRotationAdd;
					Vector2 vector = loaderCentre + Utils.PolarToCartesian(num8, loaderRadius);
					sb.Draw(this.circle, vector, null, c, 0f, Vector2.Zero, scaleMod * 0.1f * (loaderRadius / 120f), SpriteEffects.None, 0.3f);
					if (Utils.random.NextDouble() < 0.001)
					{
						Vector2 vector2 = loaderCentre + Utils.PolarToCartesian(num8, 20f + Utils.randm(45f));
						sb.Draw(Utils.white, vector, Utils.AddativeWhite);
						Utils.drawLine(sb, vector, vector2, Vector2.Zero, Utils.AddativeWhite * 0.4f, 0.1f);
					}
				}
			}
		}

		// Token: 0x04000426 RID: 1062
		private RenderTarget2D target;

		// Token: 0x04000427 RID: 1063
		private SpriteBatch internalSB;

		// Token: 0x04000428 RID: 1064
		private Texture2D circle;

		// Token: 0x04000429 RID: 1065
		private FlyoutEffect flyout;
	}
}
