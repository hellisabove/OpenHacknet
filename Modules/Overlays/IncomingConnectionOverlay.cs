using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Modules.Overlays
{
	// Token: 0x020000E0 RID: 224
	public class IncomingConnectionOverlay
	{
		// Token: 0x06000486 RID: 1158 RVA: 0x00048CB4 File Offset: 0x00046EB4
		public IncomingConnectionOverlay(object OSobj)
		{
			this.CautionSign = ((OS)OSobj).content.Load<Texture2D>("Sprites/Icons/CautionIcon");
			this.CautionSignBG = ((OS)OSobj).content.Load<Texture2D>("Sprites/Icons/CautionIconBG");
			this.sound1 = ((OS)OSobj).content.Load<SoundEffect>("SFX/DoomShock");
			this.sound2 = ((OS)OSobj).content.Load<SoundEffect>("SFX/BrightFlash");
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x00048D48 File Offset: 0x00046F48
		public void Activate()
		{
			this.IsActive = true;
			this.timeElapsed = 0f;
			this.sound1.Play();
			this.sound2.Play();
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00048D78 File Offset: 0x00046F78
		public void Update(float dt)
		{
			this.timeElapsed += dt;
			if (this.timeElapsed > 6f)
			{
				this.IsActive = false;
			}
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00048DB0 File Offset: 0x00046FB0
		public void Draw(Rectangle dest, SpriteBatch sb)
		{
			if (this.IsActive)
			{
				float num = this.timeElapsed;
				if (this.timeElapsed > 5.5f)
				{
					num -= 5.5f;
				}
				if (num <= 0.5f)
				{
					if (num % 0.1f < 0.05f)
					{
						return;
					}
				}
				int num2 = 120;
				float num3 = this.timeElapsed / 6f;
				float num4 = 1f;
				float num5 = 0.2f;
				if (this.timeElapsed < num5)
				{
					num4 = this.timeElapsed / num5;
					num2 = (int)((double)num2 * (double)(this.timeElapsed / num5));
				}
				else if (this.timeElapsed > 6f - num5)
				{
					float num6 = 1f - (this.timeElapsed - (6f - num5));
					num4 = num6;
					num2 = (int)((double)num2 * (double)num6);
				}
				Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y + dest.Height / 2 - num2 / 2, dest.Width, num2);
				sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.9f * num4);
				string text = "INCOMING CONNECTION";
				string text2 = "External unsyndicated UDP traffic on port 22\nLogging all activity to ~/log";
				int num7 = dest.Width / 3;
				int num8 = (int)(24f * num4);
				Rectangle dest2 = new Rectangle(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, num8);
				PatternDrawer.draw(dest2, 1f, Color.Transparent, IncomingConnectionOverlay.DrawColor, sb, PatternDrawer.warningStripe);
				dest2.Y += num2 - num8;
				PatternDrawer.draw(dest2, 1f, Color.Transparent, IncomingConnectionOverlay.DrawColor, sb, PatternDrawer.warningStripe);
				int num9 = 700;
				Rectangle rectangle = new Rectangle(destinationRectangle.X + destinationRectangle.Width / 2 - num9 / 2, destinationRectangle.Y, num9, destinationRectangle.Height);
				int width = (int)((float)this.CautionSign.Width / (float)this.CautionSign.Height * (float)num2);
				Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y, width, num2);
				rectangle2 = Utils.InsetRectangle(rectangle2, -30);
				sb.Draw(this.CautionSignBG, rectangle2, Color.Black * num4);
				int num10 = 4;
				rectangle2 = new Rectangle(rectangle2.X + num10, rectangle2.Y + num10, rectangle2.Width - num10 * 2, rectangle2.Height - num10 * 2);
				sb.Draw(this.CautionSign, rectangle2, Color.Lerp(Color.Red, IncomingConnectionOverlay.DrawColor, 0.95f + 0.05f * Utils.rand()));
				Rectangle dest3 = new Rectangle(rectangle.X + rectangle2.Width + 2 * num10 - 18, rectangle.Y + 4, rectangle.Width - (rectangle2.Width + num10 * 2) + 20, (int)((double)rectangle.Height * 0.8));
				TextItem.doFontLabelToSize(dest3, text, GuiData.titlefont, IncomingConnectionOverlay.DrawColor, false, false);
				dest3.Y += dest3.Height - 27;
				dest3.Height = (int)((double)rectangle.Height * 0.2);
				TextItem.doFontLabelToSize(dest3, text2, GuiData.detailfont, IncomingConnectionOverlay.DrawColor * num4, false, false);
			}
		}

		// Token: 0x04000573 RID: 1395
		private const float DURATION = 6f;

		// Token: 0x04000574 RID: 1396
		public bool IsActive = false;

		// Token: 0x04000575 RID: 1397
		private float timeElapsed = 0f;

		// Token: 0x04000576 RID: 1398
		private Texture2D CautionSign;

		// Token: 0x04000577 RID: 1399
		private Texture2D CautionSignBG;

		// Token: 0x04000578 RID: 1400
		private static Color DrawColor = new Color(290, 0, 0, 0);

		// Token: 0x04000579 RID: 1401
		private SoundEffect sound1;

		// Token: 0x0400057A RID: 1402
		private SoundEffect sound2;
	}
}
