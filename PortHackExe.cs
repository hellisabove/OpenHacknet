using System;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000146 RID: 326
	internal class PortHackExe : ExeModule
	{
		// Token: 0x06000815 RID: 2069 RVA: 0x00087030 File Offset: 0x00085230
		public PortHackExe(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.IdentifierName = "PortHack";
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x000870A4 File Offset: 0x000852A4
		public override void LoadContent()
		{
			base.LoadContent();
			int num = PortExploits.passwords.Count / 3;
			this.textIndex = new int[3];
			this.textIndex[0] = 0;
			this.textIndex[1] = num;
			this.textIndex[2] = 2 * num;
			this.target = Programs.getComputer(this.os, this.targetIP);
			this.os.write("Porthack Initialized -- Running...");
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x00087118 File Offset: 0x00085318
		public override void Update(float t)
		{
			base.Update(t);
			if (this.IsTargetingPorthackHeart)
			{
				this.progress = Utils.rand(0.98f);
			}
			else if (!this.StopProgress)
			{
				this.progress += t / PortHackExe.CRACK_TIME;
			}
			if (this.progress >= 1f)
			{
				this.progress = 1f;
				if (!this.hasCompleted)
				{
					this.Completed();
					this.hasCompleted = true;
				}
				this.sucsessTimer -= t;
				if (this.sucsessTimer <= 0f)
				{
					this.isExiting = true;
				}
			}
			else
			{
				this.target.hostileActionTaken();
			}
			if (this.progress < 1f)
			{
				this.textSwitchTimer -= t;
				if (this.textSwitchTimer <= 0f)
				{
					this.textSwitchTimer = PortHackExe.TIME_BETWEEN_TEXT_SWITCH;
					this.textOffsetIndex++;
				}
			}
			if (!this.hasCheckedForheart && this.progress > 0.5f)
			{
				this.hasCheckedForheart = true;
				PorthackHeartDaemon porthackHeartDaemon = this.target.getDaemon(typeof(PorthackHeartDaemon)) as PorthackHeartDaemon;
				if (porthackHeartDaemon != null)
				{
					this.IsTargetingPorthackHeart = true;
					if (this.os.connectedComp != null && this.os.connectedComp.ip == this.target.ip)
					{
						porthackHeartDaemon.BreakHeart();
					}
					this.cubeSeq.ShouldCentralSpinInfinitley = true;
				}
			}
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			if (computer.ip != this.target.ip)
			{
				this.StopProgress = true;
				this.isExiting = true;
			}
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x00087328 File Offset: 0x00085528
		public override void Completed()
		{
			base.Completed();
			this.os.takeAdmin(this.targetIP);
			this.os.write("--Porthack Complete--");
			this.os.PorthackCompleteFlashTime = PortHackExe.COMPLETE_LIGHT_FLASH_TIME;
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x00087368 File Offset: 0x00085568
		public override void Draw(float t)
		{
			base.Draw(t);
			Rectangle destinationRectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + Module.PANEL_HEIGHT + 1, this.bounds.Width - 2, this.bounds.Height - (Module.PANEL_HEIGHT + 2));
			if (this.renderTarget == null)
			{
				this.renderTarget = new RenderTarget2D(this.spriteBatch.GraphicsDevice, destinationRectangle.Width, destinationRectangle.Height);
			}
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			this.spriteBatch.GraphicsDevice.SetRenderTarget(this.renderTarget);
			this.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
			try
			{
				Rectangle dest = new Rectangle(0, 0, destinationRectangle.Width, destinationRectangle.Height);
				this.cubeSeq.DrawSequence(dest, t, PortHackExe.CRACK_TIME);
			}
			catch (Exception ex)
			{
				Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex) + "\r\n\r\n");
			}
			this.spriteBatch.GraphicsDevice.SetRenderTarget(currentRenderTarget);
			Rectangle bounds = this.bounds;
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			this.drawOutline();
			this.spriteBatch.Draw(this.renderTarget, destinationRectangle, Utils.AddativeWhite * ((this.progress >= 1f) ? 0.2f : 0.6f));
			this.drawTarget("app:");
			if (this.progress < 1f)
			{
				Rectangle destinationRectangle2 = new Rectangle(this.bounds.X, this.bounds.Y + Module.PANEL_HEIGHT + 1, (int)((float)this.bounds.Width / 2f), this.bounds.Height - (Module.PANEL_HEIGHT + 2));
				this.spriteBatch.Draw(Utils.gradientLeftRight, destinationRectangle2, null, Color.Black * 0.9f, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.2f);
				destinationRectangle2.X += this.bounds.Width - destinationRectangle2.Width - 1;
				this.spriteBatch.Draw(Utils.gradientLeftRight, destinationRectangle2, null, Color.Black * 0.9f, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);
			}
			int num = this.bounds.Height - 16 - 16 - 4;
			num /= 12;
			Vector2 position = new Vector2((float)(this.bounds.X + 3), (float)(this.bounds.Y + 20));
			Color color = Color.White * this.fade;
			if (this.IsTargetingPorthackHeart)
			{
				color *= 0.5f;
			}
			if (this.progress >= 1f)
			{
				color = Color.Gray * this.fade;
			}
			for (int i = 0; i < num; i++)
			{
				int index = (this.textIndex[0] + this.textOffsetIndex + i) % (PortExploits.passwords.Count - 1);
				this.spriteBatch.DrawString(GuiData.UITinyfont, PortExploits.passwords[index], position, color);
				index = (this.textIndex[1] + this.textOffsetIndex + i) % (PortExploits.passwords.Count - 1);
				Vector2 vector = GuiData.UITinyfont.MeasureString(PortExploits.passwords[index]);
				position.X = (float)(this.bounds.X + this.bounds.Width) - vector.X - 3f;
				Rectangle destinationRectangle3 = new Rectangle((int)position.X - 1, (int)position.Y, (int)vector.X, 12);
				if (Settings.ActiveLocale != "en-us")
				{
					destinationRectangle3.Y += 6;
				}
				this.spriteBatch.Draw(Utils.white, destinationRectangle3, Color.Black * 0.8f);
				this.spriteBatch.DrawString(GuiData.UITinyfont, PortExploits.passwords[index], position, color);
				position.X = (float)(this.bounds.X + 3);
				position.Y += 12f;
			}
			if (this.progress >= 1f)
			{
				string text = "PASSWORD";
				Vector2 vector2 = GuiData.font.MeasureString(text);
				position.X = (float)(this.bounds.X + this.bounds.Width / 2) - vector2.X / 2f;
				position.Y = (float)(this.bounds.Y + this.bounds.Height / 2) - vector2.Y;
				float num2 = Utils.QuadraticOutCurve(Math.Min(2f, PortHackExe.TIME_ALIVE_AFTER_SUCSESS - this.sucsessTimer) / 2f);
				Rectangle destinationRectangle4 = new Rectangle(0, (int)position.Y - 3, (int)(num2 * (float)this.bounds.Width - 30f), 2);
				destinationRectangle4.X = this.bounds.X + (this.bounds.Width / 2 - destinationRectangle4.Width / 2);
				if (destinationRectangle4.Y > this.bounds.Y)
				{
					this.spriteBatch.Draw(Utils.white, destinationRectangle4, Utils.AddativeWhite * this.fade);
				}
				this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars(text, 0.012), position, Color.White * this.fade);
				this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars(text, 0.11), position, Utils.AddativeWhite * 0.2f * this.fade * this.fade);
				text = "FOUND";
				vector2 = GuiData.font.MeasureString(text);
				position.X = (float)(this.bounds.X + this.bounds.Width / 2) - vector2.X / 2f;
				position.Y += 35f;
				this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars(text, 0.012), position, Color.White * this.fade);
				this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars(text, 0.11), position, Utils.AddativeWhite * 0.2f * this.fade * this.fade);
				destinationRectangle4.Y = (int)(position.Y + 35f + 3f);
				if (destinationRectangle4.Y > this.bounds.Y)
				{
					this.spriteBatch.Draw(Utils.white, destinationRectangle4, Utils.AddativeWhite * this.fade);
				}
			}
			bounds.X += 2;
			bounds.Width -= 4;
			bounds.Y = this.bounds.Y + this.bounds.Height - 2 - 16;
			bounds.Height = 16;
			this.spriteBatch.Draw(Utils.white, bounds, this.os.outlineColor * this.fade);
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			if (this.IsTargetingPorthackHeart)
			{
				this.spriteBatch.Draw(Utils.white, bounds, Color.DarkRed * (Utils.rand(0.3f) + 0.7f) * this.fade);
				TextItem.doFontLabelToSize(bounds, LocaleTerms.Loc("UNKNOWN ERROR"), GuiData.font, Utils.AddativeWhite, false, false);
			}
			else
			{
				this.spriteBatch.Draw(Utils.white, bounds, this.os.darkBackgroundColor * this.fade);
				bounds.Width = (int)((float)bounds.Width * this.progress);
				this.spriteBatch.Draw(Utils.white, bounds, this.os.highlightColor * this.fade);
			}
		}

		// Token: 0x0400098E RID: 2446
		public static float CRACK_TIME = 6f;

		// Token: 0x0400098F RID: 2447
		public static float TIME_BETWEEN_TEXT_SWITCH = 0.06f;

		// Token: 0x04000990 RID: 2448
		public static float TIME_ALIVE_AFTER_SUCSESS = 5f;

		// Token: 0x04000991 RID: 2449
		public static float COMPLETE_LIGHT_FLASH_TIME = 2f;

		// Token: 0x04000992 RID: 2450
		private float progress = 0f;

		// Token: 0x04000993 RID: 2451
		private int[] textIndex;

		// Token: 0x04000994 RID: 2452
		private float textSwitchTimer = PortHackExe.TIME_BETWEEN_TEXT_SWITCH;

		// Token: 0x04000995 RID: 2453
		private int textOffsetIndex = 0;

		// Token: 0x04000996 RID: 2454
		private float sucsessTimer = PortHackExe.TIME_ALIVE_AFTER_SUCSESS;

		// Token: 0x04000997 RID: 2455
		private bool hasCompleted = false;

		// Token: 0x04000998 RID: 2456
		private Computer target;

		// Token: 0x04000999 RID: 2457
		private PortHackCubeSequence cubeSeq = new PortHackCubeSequence();

		// Token: 0x0400099A RID: 2458
		private RenderTarget2D renderTarget;

		// Token: 0x0400099B RID: 2459
		private bool IsTargetingPorthackHeart = false;

		// Token: 0x0400099C RID: 2460
		private bool hasCheckedForheart = false;

		// Token: 0x0400099D RID: 2461
		private bool StopProgress = false;
	}
}
