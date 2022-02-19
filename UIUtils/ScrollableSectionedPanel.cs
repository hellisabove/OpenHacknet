using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.UIUtils
{
	// Token: 0x0200017D RID: 381
	public class ScrollableSectionedPanel
	{
		// Token: 0x0600097E RID: 2430 RVA: 0x0009CADC File Offset: 0x0009ACDC
		public ScrollableSectionedPanel(int panelHeight, GraphicsDevice graphics)
		{
			this.PanelHeight = panelHeight;
			this.fragmentBatch = new SpriteBatch(graphics);
			this.fragmentBatch.Name = "AltRTBatch";
			this.graphics = graphics;
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x0009CB45 File Offset: 0x0009AD45
		private void UpdateInput(Rectangle dest)
		{
			this.ScrollDown += GuiData.getMouseWheelScroll() * 20f;
			this.ScrollDown = Math.Max(Math.Min(this.ScrollDown, this.GetMaxScroll(dest)), 0f);
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x0009CB84 File Offset: 0x0009AD84
		private float GetMaxScroll(Rectangle dest)
		{
			float num = (float)(this.NumberOfPanels * this.PanelHeight);
			return num - (float)dest.Height;
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0009CBB0 File Offset: 0x0009ADB0
		public void Draw(Action<int, Rectangle, SpriteBatch> DrawSection, SpriteBatch sb, Rectangle destination)
		{
			this.UpdateInput(destination);
			this.SetRenderTargetsToFrame(destination);
			bool flag = destination.Contains(GuiData.getMousePoint());
			Vector2 scrollOffset = GuiData.scrollOffset;
			Vector2 scrollOffset2 = new Vector2((float)destination.X, (float)destination.Y - this.ScrollDown % (float)this.PanelHeight);
			if (flag)
			{
				GuiData.scrollOffset = scrollOffset2;
			}
			SpriteBatch spriteBatch = GuiData.spriteBatch;
			int num = 0;
			float num2 = this.ScrollDown;
			while (num2 >= (float)this.PanelHeight)
			{
				num2 -= (float)this.PanelHeight;
				num++;
			}
			int num3 = 0;
			if (this.ScrollDown % (float)this.PanelHeight != 0f)
			{
				GuiData.spriteBatch = this.fragmentBatch;
				this.RenderToTarget(DrawSection, this.AboveFragment, num, new Rectangle(0, 0, destination.Width, this.PanelHeight));
				int num4 = this.PanelHeight - (int)(this.ScrollDown % (float)this.PanelHeight);
				Rectangle destinationRectangle = new Rectangle(destination.X, destination.Y, destination.Width, num4);
				Rectangle value = new Rectangle(0, this.PanelHeight - destinationRectangle.Height, destination.Width, num4);
				sb.Draw(this.AboveFragment, destinationRectangle, new Rectangle?(value), Color.White);
				num3 += num4;
				num++;
			}
			GuiData.spriteBatch = spriteBatch;
			if (flag)
			{
				GuiData.scrollOffset = scrollOffset;
			}
			int num5 = num;
			Rectangle arg = new Rectangle(destination.X, destination.Y + num3, destination.Width, this.PanelHeight);
			while (num3 + this.PanelHeight < destination.Height && num5 < this.NumberOfPanels)
			{
				DrawSection(num5, arg, sb);
				num5++;
				num3 += this.PanelHeight;
				arg.Y = destination.Y + num3;
			}
			scrollOffset2 = new Vector2((float)destination.X, (float)arg.Y);
			if (flag)
			{
				GuiData.scrollOffset = scrollOffset2;
			}
			if (num5 < this.NumberOfPanels && destination.Height - num3 > 0)
			{
				GuiData.spriteBatch = this.fragmentBatch;
				this.RenderToTarget(DrawSection, this.BelowFragment, num5, new Rectangle(0, 0, destination.Width, this.PanelHeight));
				int num6 = destination.Height - num3;
				Rectangle destinationRectangle2 = new Rectangle(destination.X, arg.Y, destination.Width, num6);
				Rectangle value2 = new Rectangle(0, 0, destination.Width, num6);
				sb.Draw(this.BelowFragment, destinationRectangle2, new Rectangle?(value2), Color.White);
				num3 += num6;
				num++;
			}
			GuiData.spriteBatch = spriteBatch;
			if (flag)
			{
				GuiData.scrollOffset = scrollOffset;
			}
			if (this.HasScrollBar)
			{
				int num7 = 7;
				this.DrawScrollBar(new Rectangle(destination.X + destination.Width - num7 - 2, destination.Y, num7, destination.Height - 1), num7);
			}
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x0009CF00 File Offset: 0x0009B100
		private void DrawScrollBar(Rectangle dest, int width)
		{
			this.ScrollDown = ScrollBar.doVerticalScrollBar(184602004 + this.ScrollbarUIIndexOffset, dest.X, dest.Y, dest.Width, dest.Height, this.NumberOfPanels * this.PanelHeight, this.ScrollDown);
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0009CF54 File Offset: 0x0009B154
		private void RenderToTarget(Action<int, Rectangle, SpriteBatch> DrawSection, RenderTarget2D target, int index, Rectangle destination)
		{
			RenderTarget2D renderTarget = (RenderTarget2D)this.graphics.GetRenderTargets()[0].RenderTarget;
			this.graphics.SetRenderTarget(target);
			this.graphics.Clear(Color.Transparent);
			this.fragmentBatch.Begin();
			DrawSection(index, destination, this.fragmentBatch);
			this.fragmentBatch.End();
			this.graphics.SetRenderTarget(renderTarget);
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0009CFD4 File Offset: 0x0009B1D4
		private void SetRenderTargetsToFrame(Rectangle frame)
		{
			if (this.AboveFragment == null || this.AboveFragment.Width != frame.Width)
			{
				this.AboveFragment = new RenderTarget2D(this.graphics, frame.Width, this.PanelHeight);
				this.BelowFragment = new RenderTarget2D(this.graphics, frame.Width, this.PanelHeight);
			}
		}

		// Token: 0x04000B10 RID: 2832
		public int PanelHeight = 100;

		// Token: 0x04000B11 RID: 2833
		public float ScrollDown = 0f;

		// Token: 0x04000B12 RID: 2834
		public int NumberOfPanels = 0;

		// Token: 0x04000B13 RID: 2835
		private RenderTarget2D AboveFragment;

		// Token: 0x04000B14 RID: 2836
		private RenderTarget2D BelowFragment;

		// Token: 0x04000B15 RID: 2837
		private SpriteBatch fragmentBatch;

		// Token: 0x04000B16 RID: 2838
		private GraphicsDevice graphics;

		// Token: 0x04000B17 RID: 2839
		public int ScrollbarUIIndexOffset = 0;

		// Token: 0x04000B18 RID: 2840
		public bool HasScrollBar = true;
	}
}
