using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Gui
{
	// Token: 0x0200011D RID: 285
	public static class ScrollablePanel
	{
		// Token: 0x060006B6 RID: 1718 RVA: 0x0006DA7C File Offset: 0x0006BC7C
		public static void beginPanel(int id, Rectangle drawbounds, Vector2 scroll)
		{
			if (ScrollablePanel.targets == null)
			{
				ScrollablePanel.targets = new Stack<RenderTarget2D>();
				ScrollablePanel.batches = new Stack<SpriteBatch>();
				ScrollablePanel.targets.Push((RenderTarget2D)GuiData.spriteBatch.GraphicsDevice.GetRenderTargets()[0].RenderTarget);
				ScrollablePanel.targetPool = new List<RenderTarget2D>();
				ScrollablePanel.targetPool.Add(new RenderTarget2D(GuiData.spriteBatch.GraphicsDevice, drawbounds.Width, drawbounds.Height));
				ScrollablePanel.batchPool = new List<SpriteBatch>();
				ScrollablePanel.batchPool.Add(new SpriteBatch(GuiData.spriteBatch.GraphicsDevice));
				ScrollablePanel.batches.Push(GuiData.spriteBatch);
				ScrollablePanel.offsetStack = new Stack<Vector2>();
			}
			if (ScrollablePanel.batchPool.Count <= 0)
			{
				ScrollablePanel.batchPool.Add(new SpriteBatch(GuiData.spriteBatch.GraphicsDevice));
			}
			SpriteBatch item = ScrollablePanel.batchPool[ScrollablePanel.batchPool.Count - 1];
			ScrollablePanel.batchPool.RemoveAt(ScrollablePanel.batchPool.Count - 1);
			ScrollablePanel.batches.Push(item);
			bool flag = false;
			for (int i = ScrollablePanel.targetPool.Count - 1; i >= 0; i--)
			{
				RenderTarget2D renderTarget2D = ScrollablePanel.targetPool[i];
				if (renderTarget2D.Width == drawbounds.Width && renderTarget2D.Height == drawbounds.Height)
				{
					ScrollablePanel.targets.Push(renderTarget2D);
					ScrollablePanel.targetPool.RemoveAt(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				RenderTarget2D renderTarget2D = new RenderTarget2D(GuiData.spriteBatch.GraphicsDevice, drawbounds.Width, drawbounds.Height);
				ScrollablePanel.targets.Push(renderTarget2D);
				Console.WriteLine("Creating RenderTarget");
			}
			ScrollablePanel.offsetStack.Push(GuiData.scrollOffset);
			GuiData.scrollOffset = new Vector2((float)drawbounds.X - scroll.X, (float)drawbounds.Y - scroll.Y);
			GuiData.spriteBatch.GraphicsDevice.SetRenderTarget(ScrollablePanel.targets.Peek());
			GuiData.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
			ScrollablePanel.batches.Peek().Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			GuiData.spriteBatch = ScrollablePanel.batches.Peek();
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0006DCFC File Offset: 0x0006BEFC
		public static Vector2 endPanel(int id, Vector2 scroll, Rectangle bounds, float maxScroll, bool onlyScrollWithMouseOver = false)
		{
			ScrollablePanel.batches.Peek().End();
			RenderTarget2D renderTarget2D = ScrollablePanel.targets.Pop();
			GuiData.spriteBatch.GraphicsDevice.SetRenderTarget(ScrollablePanel.targets.Peek());
			ScrollablePanel.targetPool.Add(renderTarget2D);
			ScrollablePanel.batchPool.Add(ScrollablePanel.batches.Pop());
			GuiData.spriteBatch = ScrollablePanel.batches.Peek();
			GuiData.scrollOffset = ScrollablePanel.offsetStack.Pop();
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = (int)scroll.X;
			tmpRect.Y = (int)scroll.Y;
			tmpRect.Width = bounds.Width;
			tmpRect.Height = bounds.Height;
			try
			{
				GuiData.spriteBatch.Draw(renderTarget2D, bounds, new Rectangle?(tmpRect), Color.White);
			}
			catch (InvalidOperationException)
			{
				return scroll;
			}
			if (!onlyScrollWithMouseOver || bounds.Contains(GuiData.getMousePoint()))
			{
				scroll.Y += GuiData.getMouseWheelScroll() * 20f;
			}
			scroll.Y = Math.Max(Math.Min(scroll.Y, maxScroll), 0f);
			Rectangle tmpRect2 = GuiData.tmpRect;
			float num = 5f;
			float num2 = (float)bounds.Height / maxScroll;
			num2 *= (float)bounds.Height;
			float num3 = (float)bounds.Height;
			num3 -= 4f;
			float num4 = scroll.Y / maxScroll;
			num4 *= (float)bounds.Height - num2;
			tmpRect2.Y = (int)(num4 - num2 / 2f + num2 / 2f + (float)bounds.Y);
			tmpRect2.X = (int)((double)(bounds.X + bounds.Width) - 1.5 * (double)num - 2.0);
			tmpRect2.Height = (int)num2;
			tmpRect2.Width = (int)num;
			scroll.Y = ScrollBar.doVerticalScrollBar(id, tmpRect2.X, bounds.Y, tmpRect2.Width, bounds.Height, renderTarget2D.Height, scroll.Y);
			scroll.Y = Math.Max(Math.Min(scroll.Y, maxScroll), 0f);
			return scroll;
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x0006DF64 File Offset: 0x0006C164
		public static void ClearCache()
		{
			ScrollablePanel.targets = null;
		}

		// Token: 0x0400078D RID: 1933
		private static Stack<RenderTarget2D> targets;

		// Token: 0x0400078E RID: 1934
		private static Stack<SpriteBatch> batches;

		// Token: 0x0400078F RID: 1935
		private static List<RenderTarget2D> targetPool;

		// Token: 0x04000790 RID: 1936
		private static List<SpriteBatch> batchPool;

		// Token: 0x04000791 RID: 1937
		private static Color scrollBarColor = new Color(120, 120, 120, 80);

		// Token: 0x04000792 RID: 1938
		private static Stack<Vector2> offsetStack;
	}
}
