using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x0200005E RID: 94
	internal class ExeModule : Module
	{
		// Token: 0x060001C1 RID: 449 RVA: 0x00018094 File Offset: 0x00016294
		public ExeModule(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.targetIP = ((operatingSystem.connectedComp == null) ? operatingSystem.thisComputer.ip : operatingSystem.connectedComp.ip);
			bool flag;
			byte randomByte;
			do
			{
				flag = false;
				randomByte = Utils.getRandomByte();
				for (int i = 0; i < this.os.exes.Count; i++)
				{
					if (this.os.exes[i].PID == (int)randomByte)
					{
						flag = true;
						break;
					}
				}
			}
			while (flag);
			this.os.currentPID = (int)randomByte;
			this.PID = (int)randomByte;
			this.bounds = location;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x000181A0 File Offset: 0x000163A0
		public override void LoadContent()
		{
			this.bounds.Height = this.ramCost;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x000181B4 File Offset: 0x000163B4
		public override void Update(float t)
		{
			this.bounds.Height = this.ramCost;
			if (this.isExiting)
			{
				if (this.fade >= 1f)
				{
					this.baseRamCost = this.ramCost;
				}
				this.ramCost = (int)((float)this.baseRamCost * this.fade);
				this.bounds.Height = this.ramCost;
				this.fade -= t * ExeModule.FADEOUT_RATE;
				if (this.fade <= 0f)
				{
					this.needsRemoval = true;
				}
			}
			if (this.moveUpBy >= 0f)
			{
				int num = (int)(ExeModule.MOVE_UP_RATE * t);
				this.bounds.Y = this.bounds.Y - num;
				this.moveUpBy -= (float)num;
				this.bounds.Y = Math.Max(this.bounds.Y, this.os.ram.bounds.Y + RamModule.contentStartOffset);
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x000182C7 File Offset: 0x000164C7
		public override void Draw(float t)
		{
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x000182CA File Offset: 0x000164CA
		public virtual void Completed()
		{
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x000182CD File Offset: 0x000164CD
		public virtual void Killed()
		{
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x000182D0 File Offset: 0x000164D0
		public virtual void drawOutline()
		{
			Rectangle bounds = this.bounds;
			RenderedRectangle.doRectangleOutline(bounds.X, bounds.Y, bounds.Width, bounds.Height, 1, new Color?(this.os.moduleColorSolid));
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			this.spriteBatch.Draw(Utils.white, bounds, this.os.moduleColorBacking * this.fade);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0001837C File Offset: 0x0001657C
		public virtual void drawTarget(string typeName = "app:")
		{
			if (this.bounds.Height > 14)
			{
				string text = "IP: " + this.targetIP;
				Rectangle bounds = this.bounds;
				bounds.Height = Math.Min(this.bounds.Height, 14);
				bounds.X++;
				bounds.Width -= 2;
				this.spriteBatch.Draw(Utils.white, bounds, this.os.exeModuleTopBar);
				RenderedRectangle.doRectangleOutline(bounds.X, bounds.Y, bounds.Width, bounds.Height, 1, new Color?(this.os.topBarColor));
				if (bounds.Height >= 14)
				{
					Vector2 vector = GuiData.detailfont.MeasureString(text);
					this.spriteBatch.DrawString(GuiData.detailfont, text, new Vector2((float)(this.bounds.X + this.bounds.Width) - vector.X, (float)this.bounds.Y), this.os.exeModuleTitleText);
					this.spriteBatch.DrawString(GuiData.detailfont, typeName + this.IdentifierName, new Vector2((float)(this.bounds.X + 2), (float)this.bounds.Y), this.os.exeModuleTitleText);
				}
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x000184FC File Offset: 0x000166FC
		public Rectangle GetContentAreaDest()
		{
			return new Rectangle(this.bounds.X + 1, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width - 2, this.bounds.Height - Module.PANEL_HEIGHT - 1);
		}

		// Token: 0x040001D3 RID: 467
		public static float FADEOUT_RATE = 0.5f;

		// Token: 0x040001D4 RID: 468
		public static float MOVE_UP_RATE = 350f;

		// Token: 0x040001D5 RID: 469
		public static int DEFAULT_RAM_COST = 246;

		// Token: 0x040001D6 RID: 470
		public int PID = 0;

		// Token: 0x040001D7 RID: 471
		public float fade = 1f;

		// Token: 0x040001D8 RID: 472
		public bool isExiting = false;

		// Token: 0x040001D9 RID: 473
		public bool needsRemoval = false;

		// Token: 0x040001DA RID: 474
		public float moveUpBy = 0f;

		// Token: 0x040001DB RID: 475
		public int ramCost = ExeModule.DEFAULT_RAM_COST;

		// Token: 0x040001DC RID: 476
		private int baseRamCost = 0;

		// Token: 0x040001DD RID: 477
		public string targetIP = "";

		// Token: 0x040001DE RID: 478
		public bool needsProxyAccess = false;

		// Token: 0x040001DF RID: 479
		public string IdentifierName = "UNKNOWN";
	}
}
