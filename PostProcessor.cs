using System;
using Hacknet.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000150 RID: 336
	public static class PostProcessor
	{
		// Token: 0x06000845 RID: 2117 RVA: 0x0008B6F8 File Offset: 0x000898F8
		public static void init(GraphicsDevice gDevice, SpriteBatch spriteBatch, ContentManager content)
		{
			PostProcessor.device = gDevice;
			PostProcessor.GenerateMainTarget(gDevice);
			PostProcessor.backTarget = new RenderTarget2D(gDevice, gDevice.Viewport.Width, gDevice.Viewport.Height);
			PostProcessor.dangerBufferTarget = new RenderTarget2D(gDevice, gDevice.Viewport.Width, gDevice.Viewport.Height);
			PostProcessor.sb = spriteBatch;
			PostProcessor.bloom = content.Load<Effect>("Shaders/Bloom");
			PostProcessor.blur = content.Load<Effect>("Shaders/DOFBlur");
			PostProcessor.danger = content.Load<Effect>("Shaders/DangerEffect");
			PostProcessor.blur.CurrentTechnique = PostProcessor.blur.Techniques["SmoothGaussBlur"];
			PostProcessor.danger.CurrentTechnique = PostProcessor.danger.Techniques["PostProcess"];
			PostProcessor.bloomColor = new Color(90, 90, 90, 0);
			PostProcessor.bloomAbsenceHighlighterColor = new Color(70, 70, 70, 0);
			PostProcessor.dangerLineColor = new Color(255, 255, 255, 0);
			PostProcessor.dangerLineColorAlt = new Color(240, 0, 0, 0);
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x0008B824 File Offset: 0x00089A24
		public static void GenerateMainTarget(GraphicsDevice gDevice)
		{
			PostProcessor.target = new RenderTarget2D(gDevice, gDevice.Viewport.Width, gDevice.Viewport.Height, false, SurfaceFormat.Rgba64, DepthFormat.None, SettingsLoader.ShouldMultisample ? 4 : 0, RenderTargetUsage.PlatformContents);
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0008B86A File Offset: 0x00089A6A
		public static void begin()
		{
			PostProcessor.device.SetRenderTarget(PostProcessor.target);
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0008B880 File Offset: 0x00089A80
		public static void end()
		{
			PostProcessor.device.SetRenderTarget(PostProcessor.backTarget);
			PostProcessor.sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, PostProcessor.blur);
			PostProcessor.sb.Draw(PostProcessor.target, Vector2.Zero, Color.White);
			PostProcessor.sb.End();
			RenderTarget2D renderTarget = PostProcessor.dangerModeEnabled ? PostProcessor.dangerBufferTarget : null;
			PostProcessor.device.SetRenderTarget(renderTarget);
			if (PostProcessor.EndingSequenceFlashOutActive)
			{
				PostProcessor.device.Clear(Color.Black);
			}
			PostProcessor.sb.Begin();
			Rectangle fullscreenRect = PostProcessor.GetFullscreenRect();
			if (PostProcessor.EndingSequenceFlashOutActive)
			{
				FlickeringTextEffect.DrawFlickeringSprite(PostProcessor.sb, fullscreenRect, PostProcessor.target, 12f, 0f, null, Color.White);
			}
			else
			{
				PostProcessor.sb.Draw(PostProcessor.target, fullscreenRect, Color.White);
			}
			if (PostProcessor.bloomEnabled)
			{
				PostProcessor.sb.Draw(PostProcessor.backTarget, fullscreenRect, PostProcessor.bloomColor);
			}
			else
			{
				PostProcessor.sb.Draw(PostProcessor.target, fullscreenRect, PostProcessor.bloomAbsenceHighlighterColor);
			}
			PostProcessor.sb.End();
			if (PostProcessor.dangerModeEnabled)
			{
				PostProcessor.DrawDangerModeFliters();
			}
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x0008B9D8 File Offset: 0x00089BD8
		internal static Texture2D GetLastRenderedCompleteFrame()
		{
			return PostProcessor.backTarget;
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x0008B9F0 File Offset: 0x00089BF0
		public static string GetStatusReportString()
		{
			string arg = "Post Processor";
			arg = arg + "\r\n Target : " + PostProcessor.target;
			arg = arg + "\r\n TargetDisposed : " + PostProcessor.target.IsDisposed;
			arg = arg + "\r\n BTarget : " + PostProcessor.backTarget;
			arg = arg + "\r\n BTargetDisposed : " + PostProcessor.backTarget.IsDisposed;
			arg = arg + "\r\n DBTarget : " + PostProcessor.dangerBufferTarget;
			return arg + "\r\n DBTargetDisposed : " + PostProcessor.dangerBufferTarget.IsDisposed;
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x0008BA90 File Offset: 0x00089C90
		private static Rectangle GetFullscreenRect()
		{
			bool flag = 1 == 0;
			return new Rectangle(0, 0, PostProcessor.target.Width, PostProcessor.target.Height);
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x0008BAC8 File Offset: 0x00089CC8
		private static void DrawDangerModeFliters()
		{
			PostProcessor.danger.Parameters["FlickerMultiplier"].SetValue(Utils.randm(0.4f));
			int y = (int)(PostProcessor.dangerModePercentComplete * (float)PostProcessor.dangerBufferTarget.Height);
			PostProcessor.danger.Parameters["PercentDown"].SetValue(PostProcessor.dangerModePercentComplete);
			PostProcessor.device.SetRenderTarget(null);
			PostProcessor.sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, PostProcessor.danger);
			PostProcessor.sb.Draw(PostProcessor.dangerBufferTarget, Vector2.Zero, Color.White);
			PostProcessor.sb.End();
			PostProcessor.sb.Begin();
			Rectangle destinationRectangle = new Rectangle(0, y, PostProcessor.dangerBufferTarget.Width, 1);
			PostProcessor.sb.Draw(Utils.white, destinationRectangle, PostProcessor.dangerLineColor * (Utils.randm(0.7f) + 0.3f));
			destinationRectangle.Y -= 1 + (Utils.flipCoin() ? 1 : 0);
			PostProcessor.sb.Draw(Utils.white, destinationRectangle, Color.Lerp(PostProcessor.dangerLineColor * 0.4f, PostProcessor.dangerLineColorAlt, Utils.randm(0.5f)));
			destinationRectangle.Y += 1 + (Utils.flipCoin() ? -2 : 0);
			PostProcessor.sb.Draw(Utils.white, destinationRectangle, Color.Lerp(PostProcessor.dangerLineColor * 0.4f, PostProcessor.dangerLineColorAlt, Utils.randm(0.5f)));
			PostProcessor.sb.End();
		}

		// Token: 0x040009D7 RID: 2519
		private static RenderTarget2D target;

		// Token: 0x040009D8 RID: 2520
		private static RenderTarget2D backTarget;

		// Token: 0x040009D9 RID: 2521
		private static RenderTarget2D dangerBufferTarget;

		// Token: 0x040009DA RID: 2522
		private static GraphicsDevice device;

		// Token: 0x040009DB RID: 2523
		private static SpriteBatch sb;

		// Token: 0x040009DC RID: 2524
		private static Effect bloom;

		// Token: 0x040009DD RID: 2525
		private static Effect blur;

		// Token: 0x040009DE RID: 2526
		private static Effect danger;

		// Token: 0x040009DF RID: 2527
		private static Color bloomColor;

		// Token: 0x040009E0 RID: 2528
		private static Color dangerLineColor;

		// Token: 0x040009E1 RID: 2529
		private static Color dangerLineColorAlt;

		// Token: 0x040009E2 RID: 2530
		private static Color bloomAbsenceHighlighterColor;

		// Token: 0x040009E3 RID: 2531
		public static bool bloomEnabled = true;

		// Token: 0x040009E4 RID: 2532
		public static bool scanlinesEnabled = true;

		// Token: 0x040009E5 RID: 2533
		public static bool dangerModeEnabled = false;

		// Token: 0x040009E6 RID: 2534
		public static float dangerModePercentComplete = 0f;

		// Token: 0x040009E7 RID: 2535
		public static bool EndingSequenceFlashOutActive = false;

		// Token: 0x040009E8 RID: 2536
		public static float EndingSequenceFlashOutPercentageComplete = 0f;
	}
}
