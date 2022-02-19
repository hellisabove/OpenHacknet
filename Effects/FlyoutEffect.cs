using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x02000191 RID: 401
	public class FlyoutEffect : IDisposable
	{
		// Token: 0x06000A22 RID: 2594 RVA: 0x000A19E0 File Offset: 0x0009FBE0
		public FlyoutEffect(GraphicsDevice gd, ContentManager c, int width, int height)
		{
			if (FlyoutEffect.PooledTarget != null && FlyoutEffect.PooledTarget.Width == width && FlyoutEffect.PooledTarget.Height == height)
			{
				this.DrawTarget = FlyoutEffect.PooledTarget;
				this.BackTarget = FlyoutEffect.PooledBackTarget;
				FlyoutEffect.PooledTarget = null;
			}
			else
			{
				this.DrawTarget = new RenderTarget2D(gd, width, height);
				this.BackTarget = new RenderTarget2D(gd, width, height);
			}
			this.innerBatch = new SpriteBatch(gd);
			this.UsedSprite = c.Load<Texture2D>("CircleOutlineLarge");
			this.FlashSprite = c.Load<Texture2D>("EffectFiles/PointClicker/Star");
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x000A1AC8 File Offset: 0x0009FCC8
		public void Draw(float dt, Rectangle dest, SpriteBatch sb, float cornerIn, int spiralElements, float spiralRadius, Color color, bool drawFlashFromMiddle, bool flashBackground)
		{
			this.elapsedTime += dt;
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			sb.GraphicsDevice.SetRenderTarget(this.DrawTarget);
			sb.GraphicsDevice.Clear(Color.Transparent);
			Rectangle rectangle = new Rectangle(0, 0, this.DrawTarget.Width, this.DrawTarget.Height);
			Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, 2);
			this.rotation += dt * 3f;
			this.offset = new Vector2((float)(Math.Cos((double)(this.elapsedTime * 2f)) * 40.0), -1f * (float)(Math.Cos((double)(this.elapsedTime * 8f)) * 40.0));
			Vector2 vector = new Vector2((float)(this.DrawTarget.Width / 2), (float)(this.DrawTarget.Height / 2));
			this.innerBatch.Begin();
			destinationRectangle.X = this.DrawTarget.Width / 2;
			destinationRectangle.Y = this.DrawTarget.Height / 2;
			this.innerBatch.Draw(this.BackTarget, destinationRectangle, null, new Color(250, 250, 250), 0f, vector, SpriteEffects.None, 0.5f);
			if (flashBackground)
			{
				GridEffect.DrawGridBackground(rectangle, this.innerBatch, 4, Utils.AddativeWhite);
			}
			for (int i = 0; i < spiralElements; i++)
			{
				Vector2 value = Utils.PolarToCartesian((float)i / (float)spiralElements * 6.2831855f + this.rotation, spiralRadius);
				this.innerBatch.Draw(this.UsedSprite, value + new Vector2((float)(this.DrawTarget.Width / 2), (float)(this.DrawTarget.Height / 2)), null, flashBackground ? Utils.AddativeWhite : color, this.rotation, this.UsedSprite.GetCentreOrigin(), Vector2.One * 0.1f, SpriteEffects.None, 0.4f);
			}
			if (drawFlashFromMiddle)
			{
				int num = 4;
				float num2 = (float)dest.Width - 50f;
				for (int i = 0; i < num; i++)
				{
					this.innerBatch.Draw(this.FlashSprite, vector - new Vector2(3f), null, Color.Lerp(color, Utils.AddativeWhite, 0.5f) * 0.4f, 0f, this.FlashSprite.GetCentreOrigin(), Vector2.One * (2.5f + Utils.randm(3f)), SpriteEffects.None, 0.8f);
				}
			}
			this.innerBatch.End();
			sb.GraphicsDevice.SetRenderTarget(currentRenderTarget);
			sb.Draw(this.DrawTarget, dest, Color.White);
			RenderTarget2D backTarget = this.BackTarget;
			this.BackTarget = this.DrawTarget;
			this.DrawTarget = backTarget;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x000A1DFC File Offset: 0x0009FFFC
		public void Draw(float dt, Rectangle dest, SpriteBatch sb, Action<SpriteBatch, Rectangle> render)
		{
			this.elapsedTime += dt;
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			sb.GraphicsDevice.SetRenderTarget(this.DrawTarget);
			sb.GraphicsDevice.Clear(Color.Transparent);
			Rectangle rectangle = new Rectangle(0, 0, this.DrawTarget.Width, this.DrawTarget.Height);
			Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, 2);
			this.rotation += dt * 3f;
			this.offset = new Vector2((float)(Math.Cos((double)(this.elapsedTime * 2f)) * 40.0), -1f * (float)(Math.Cos((double)(this.elapsedTime * 8f)) * 40.0));
			Vector2 origin = new Vector2((float)(this.DrawTarget.Width / 2), (float)(this.DrawTarget.Height / 2));
			this.innerBatch.Begin();
			destinationRectangle.X = this.DrawTarget.Width / 2;
			destinationRectangle.Y = this.DrawTarget.Height / 2;
			this.innerBatch.Draw(this.BackTarget, destinationRectangle, null, new Color(250, 250, 250), 0f, origin, SpriteEffects.None, 0.5f);
			if (render != null)
			{
				render(this.innerBatch, rectangle);
			}
			this.innerBatch.End();
			sb.GraphicsDevice.SetRenderTarget(currentRenderTarget);
			sb.Draw(this.DrawTarget, dest, Color.White);
			RenderTarget2D backTarget = this.BackTarget;
			this.BackTarget = this.DrawTarget;
			this.DrawTarget = backTarget;
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x000A1FC0 File Offset: 0x000A01C0
		public void Dispose()
		{
			if (FlyoutEffect.PooledTarget == null)
			{
				FlyoutEffect.PooledTarget = this.DrawTarget;
				FlyoutEffect.PooledBackTarget = this.BackTarget;
			}
			else
			{
				this.DrawTarget.Dispose();
				this.BackTarget.Dispose();
			}
			this.DrawTarget = null;
		}

		// Token: 0x04000B6D RID: 2925
		private static RenderTarget2D PooledTarget;

		// Token: 0x04000B6E RID: 2926
		private static RenderTarget2D PooledBackTarget;

		// Token: 0x04000B6F RID: 2927
		private RenderTarget2D DrawTarget;

		// Token: 0x04000B70 RID: 2928
		private RenderTarget2D BackTarget;

		// Token: 0x04000B71 RID: 2929
		private SpriteBatch innerBatch;

		// Token: 0x04000B72 RID: 2930
		private Vector2 offset = Vector2.Zero;

		// Token: 0x04000B73 RID: 2931
		private Texture2D UsedSprite;

		// Token: 0x04000B74 RID: 2932
		private Texture2D FlashSprite;

		// Token: 0x04000B75 RID: 2933
		private float timeSinceRender = 0f;

		// Token: 0x04000B76 RID: 2934
		private float rotation = 0f;

		// Token: 0x04000B77 RID: 2935
		private float elapsedTime = 0f;

		// Token: 0x04000B78 RID: 2936
		public float TimeBetweenRenderings = 0.1f;
	}
}
