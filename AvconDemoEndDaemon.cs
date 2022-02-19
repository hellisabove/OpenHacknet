using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000FB RID: 251
	internal class AvconDemoEndDaemon : Daemon
	{
		// Token: 0x06000564 RID: 1380 RVA: 0x00054E21 File Offset: 0x00053021
		public AvconDemoEndDaemon(Computer c, string name, OS os) : base(c, name, os)
		{
			this.name = "Complete Demo";
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x00054E41 File Offset: 0x00053041
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.confirmed = false;
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00054E54 File Offset: 0x00053054
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			if (Button.doButton(255, bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 4, bounds.Width / 2, 35, LocaleTerms.Loc("Back"), null))
			{
				this.os.display.command = "connect";
			}
			if (Button.doButton(258, bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 4 + 40, bounds.Width / 2, 35, (!this.confirmed) ? "End Session" : "Confirm End", new Color?(this.confirmed ? Color.Red : Color.DarkRed)))
			{
				if (this.confirmed)
				{
					this.endDemo();
				}
				else
				{
					this.confirmed = true;
				}
			}
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x00054F68 File Offset: 0x00053168
		public override string getSaveString()
		{
			return "<addAvconDemoEndDaemon name=\"" + this.name + "\"/>";
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x00054F8F File Offset: 0x0005318F
		private void endDemo()
		{
			this.os.ScreenManager.AddScreen(new MainMenu());
			this.os.ExitScreen();
		}

		// Token: 0x04000624 RID: 1572
		private bool confirmed = false;
	}
}
