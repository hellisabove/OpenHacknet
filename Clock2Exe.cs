using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000197 RID: 407
	internal class Clock2Exe : ExeModule
	{
		// Token: 0x06000A46 RID: 2630 RVA: 0x000A3AEC File Offset: 0x000A1CEC
		public Clock2Exe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "Clock v2";
			this.ramCost = 60;
			for (int i = 1; i < p.Length; i++)
			{
				if (p[i].ToLower() == "-c")
				{
					this.ramCost = 40;
				}
				else if (p[i].ToLower() == "-l")
				{
					this.ramCost = 160;
					this.isLargeMode = true;
				}
			}
			this.IdentifierName = "Clock v2";
			this.targetIP = this.os.thisComputer.ip;
			this.triangle = operatingSystem.content.Load<Texture2D>("DLC/Sprites/Triangle");
			this.arc = operatingSystem.content.Load<Texture2D>("DLC/Sprites/CircleOutlineThick");
			this.arcThin = operatingSystem.content.Load<Texture2D>("CircleOutlineLarge");
			this.arcClip = new Rectangle(this.arc.Width / 4, 0, this.arc.Width / 2, this.arc.Height / 2);
			this.arcClipSmaller = new Rectangle((int)((double)this.arc.Width * 0.45), 0, (int)((double)this.arc.Width * 0.1), this.arc.Height / 2);
			this.os.write("Executing ClockV2.exe");
			this.os.write("Additional Arguments: -c / -l");
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x000A3C84 File Offset: 0x000A1E84
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			DateTime now = DateTime.Now;
			Rectangle bounds = this.bounds;
			Rectangle rectangle = new Rectangle(bounds.X, bounds.Y, Math.Min(bounds.Width, bounds.Height), Math.Min(bounds.Width, bounds.Height));
			rectangle = Utils.InsetRectangle(rectangle, 1);
			Rectangle rectangle2 = new Rectangle(bounds.X + rectangle.Width, bounds.Y, bounds.Width - rectangle.Width, bounds.Height);
			this.DrawRadialPointer((float)now.Hour / 24f, rectangle, (float)rectangle.Width / 5f, Color.Gray, false);
			this.DrawRadialPointer((float)now.Minute / 60f, rectangle, (float)rectangle.Width / 3f, this.os.moduleColorStrong, false);
			this.DrawRadialPointer((float)now.Second / 60f, rectangle, (float)rectangle.Width / 2.25f, this.os.highlightColor * 0.5f, true);
			this.DrawRadialPointer((float)now.Millisecond / 1000f, rectangle, (float)rectangle.Width / 2f, this.os.topBarColor * 0.2f, true);
			Rectangle rectangle3 = new Rectangle(rectangle2.X, rectangle2.Y + 1, rectangle2.Width - 1, 9);
			this.spriteBatch.Draw(Utils.gradientLeftRight, rectangle3, this.os.exeModuleTopBar);
			rectangle3.Y += 5;
			TextItem.doRightAlignedBackingLabel(rectangle3, "ClockV2.exe", GuiData.detailfont, Color.Transparent, this.os.exeModuleTitleText);
			if (this.isLargeMode)
			{
				rectangle2 = new Rectangle(this.bounds.X + 80, this.bounds.Y + this.bounds.Height - 41, this.bounds.Width - 81, 40);
				rectangle3 = new Rectangle(rectangle2.X, rectangle2.Y + 1, rectangle2.Width - 1, 9);
			}
			string text = " " + now.ToShortTimeString();
			Rectangle rectangle4 = new Rectangle(rectangle3.X, rectangle3.Y + rectangle3.Height - 5, rectangle3.Width, rectangle2.Height - rectangle3.Height);
			Vector2 vector = GuiData.font.MeasureString(text);
			float num = vector.X / (float)rectangle4.Width;
			float num2 = (float)(rectangle4.Height / 2) - vector.Y * num / 2f;
			Vector2 position = new Vector2((float)rectangle4.X, (float)rectangle4.Y + num2);
			for (int i = 0; i < text.Length; i++)
			{
				this.spriteBatch.DrawString(GuiData.font, string.Concat(text[i]), position, Utils.AddativeWhite * 0.8f, 0f, Vector2.Zero, num, SpriteEffects.None, 0.5f);
				position.X += (float)rectangle4.Width / (float)text.Length;
			}
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x000A4000 File Offset: 0x000A2200
		private void DrawRadialPointer(float rotationPercent, Rectangle fullArea, float radius, Color c, bool small = false)
		{
			float rotation = (float)((double)rotationPercent * 6.283185307179586);
			Vector2 position = new Vector2((float)fullArea.X + (float)fullArea.Width / 2f, (float)fullArea.Y + (float)fullArea.Height / 2f);
			float value = radius / ((float)this.arc.Width / 2f);
			this.spriteBatch.Draw(small ? this.arcThin : this.arc, position, null, c * 0.2f, rotation, this.arc.GetCentreOrigin(), new Vector2(value), SpriteEffects.None, 0.4f);
			this.spriteBatch.Draw(small ? this.arcThin : this.arc, position, new Rectangle?(this.arcClip), c, rotation, new Vector2((float)(this.arcClip.Width / 2), (float)this.arcClip.Height), new Vector2(value), SpriteEffects.None, 0.4f);
			this.spriteBatch.Draw(this.arcThin, position, new Rectangle?(this.arcClipSmaller), Utils.AddativeWhite * 0.2f, rotation, new Vector2((float)(this.arcClipSmaller.Width / 2), (float)this.arcClipSmaller.Height), new Vector2(value), SpriteEffects.None, 0.4f);
		}

		// Token: 0x04000B94 RID: 2964
		private Texture2D triangle;

		// Token: 0x04000B95 RID: 2965
		private Texture2D arc;

		// Token: 0x04000B96 RID: 2966
		private Texture2D arcThin;

		// Token: 0x04000B97 RID: 2967
		private Rectangle arcClip;

		// Token: 0x04000B98 RID: 2968
		private Rectangle arcClipSmaller;

		// Token: 0x04000B99 RID: 2969
		private bool isLargeMode = false;
	}
}
