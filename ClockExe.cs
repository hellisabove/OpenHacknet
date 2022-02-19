using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x020000BE RID: 190
	internal class ClockExe : ExeModule
	{
		// Token: 0x060003E1 RID: 993 RVA: 0x0003BB1C File Offset: 0x00039D1C
		public ClockExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "Clock";
			this.ramCost = 60;
			this.IdentifierName = "Clock";
			this.targetIP = this.os.thisComputer.ip;
			AchievementsManager.Unlock("clock_run", false);
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0003BB74 File Offset: 0x00039D74
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			DateTime now = DateTime.Now;
			string text = string.Concat(new string[]
			{
				(now.Hour % 12).ToString("00"),
				" : ",
				now.Minute.ToString("00"),
				" : ",
				now.Second.ToString("00")
			});
			TextItem.doFontLabel(new Vector2((float)(this.bounds.X + 2), (float)(this.bounds.Y + 12)), text, GuiData.titlefont, new Color?(RamModule.USED_RAM_COLOR), (float)(this.bounds.Width - 34), (float)(this.bounds.Height - 10), true);
			TextItem.doFontLabel(new Vector2((float)(this.bounds.X + this.bounds.Width - 28), (float)(this.bounds.Y + this.bounds.Height - 38)), (now.Hour > 12) ? "PM" : "AM", GuiData.titlefont, new Color?(RamModule.USED_RAM_COLOR), 30f, 26f, false);
			int num = this.bounds.Width - 2;
			Rectangle destinationRectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height - 1 - 6, num, 1);
			float num2 = (float)now.Millisecond / 1000f;
			float scale = 0f;
			if (num2 < 0.5f)
			{
				scale = 1f - num2 * 2f;
			}
			this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.moduleColorSolidDefault * 0.2f * scale);
			destinationRectangle.Width = (int)((float)num * num2);
			this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.moduleColorSolidDefault * 0.2f);
			float num3 = ((float)now.Second + num2) / 60f;
			destinationRectangle.Width = (int)((float)num * num3);
			destinationRectangle.Y += 2;
			this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.moduleColorStrong);
			float num4 = (float)now.Minute / 60f;
			float num5 = (float)now.Hour / 60f;
			destinationRectangle.Width = (int)((float)num * num4);
			destinationRectangle.Y += 2;
			this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.moduleColorSolid);
		}
	}
}
