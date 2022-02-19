using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.UIUtils
{
	// Token: 0x02000177 RID: 375
	public static class AnimatedSpriteExporter
	{
		// Token: 0x06000965 RID: 2405 RVA: 0x0009BE6C File Offset: 0x0009A06C
		public static void ExportAnimation(string folderPath, string nameStarter, int width, int height, float framesPerSecond, float totalTime, GraphicsDevice gd, Action<float> update, Action<SpriteBatch, Rectangle> draw, int antialiasingMultiplier = 1)
		{
			int width2 = width * antialiasingMultiplier;
			int height2 = height * antialiasingMultiplier;
			RenderTarget2D renderTarget2D = new RenderTarget2D(gd, width2, height2, false, SurfaceFormat.Rgba64, DepthFormat.Depth16, 8, RenderTargetUsage.PlatformContents);
			SpriteBatch spriteBatch = new SpriteBatch(gd);
			gd.PresentationParameters.MultiSampleCount = 8;
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			gd.SetRenderTarget(renderTarget2D);
			float num = 1f / framesPerSecond;
			float num2 = 0f;
			int num3 = 0;
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			SpriteBatch spriteBatch2 = GuiData.spriteBatch;
			GuiData.spriteBatch = spriteBatch;
			Rectangle arg = new Rectangle(0, 0, width2, height2);
			while (num2 < totalTime)
			{
				gd.Clear(Color.Transparent);
				spriteBatch.Begin();
				draw(spriteBatch, arg);
				spriteBatch.End();
				update(num);
				gd.SetRenderTarget(null);
				string str = string.Concat(new object[]
				{
					nameStarter,
					"_",
					num3,
					".png"
				});
				using (FileStream fileStream = File.Create(folderPath + "/" + str))
				{
					renderTarget2D.SaveAsPng(fileStream, width, height);
				}
				gd.SetRenderTarget(renderTarget2D);
				num3++;
				num2 += num;
			}
			GuiData.spriteBatch = spriteBatch2;
			gd.SetRenderTarget(currentRenderTarget);
		}
	}
}
