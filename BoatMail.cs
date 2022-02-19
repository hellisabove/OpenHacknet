using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000FE RID: 254
	internal class BoatMail : MailServer
	{
		// Token: 0x0600058D RID: 1421 RVA: 0x00057B58 File Offset: 0x00055D58
		public BoatMail(Computer c, string name, OS os) : base(c, name, os)
		{
			this.oddLine = Color.White;
			this.evenLine = Color.White;
			base.setThemeColor(new Color(155, 155, 230));
			this.seperatorLineColor = new Color(22, 22, 22, 150);
			this.panel = TextureBank.load("BoatmailHeader", os.content);
			this.logo = TextureBank.load("BoatmailLogo", os.content);
			this.textColor = Color.Black;
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00057BEF File Offset: 0x00055DEF
		public override void drawBackingGradient(Rectangle boundsTo, SpriteBatch sb)
		{
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x00057BF2 File Offset: 0x00055DF2
		public override void doInboxHeader(Rectangle bounds, SpriteBatch sb)
		{
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00057CE0 File Offset: 0x00055EE0
		public override void drawTopBar(Rectangle bounds, SpriteBatch sb)
		{
			OS os = this.os;
			os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
			{
				Rectangle destinationRectangle = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				sb.Draw(Utils.white, destinationRectangle, Color.White);
				destinationRectangle.Height = this.panel.Height;
				sb.Draw(this.panel, destinationRectangle, Color.White);
				destinationRectangle.Width = (destinationRectangle.Height = 36);
				destinationRectangle.X += 30;
				destinationRectangle.Y += 10;
				sb.Draw(this.logo, destinationRectangle, Color.White);
			}));
		}

		// Token: 0x04000652 RID: 1618
		public Texture2D logo;

		// Token: 0x04000653 RID: 1619
		public static string JunkEmail = "HOr$e Exp@nding R0Lexxxx corp\n\nHello Sir/Madam,\nI am but a humble nigerian prince who is crippled by the instability in my country, and require a transfer of 5000 united states dollar in order to rid my country of the scourge of the musclebeasts which roam our plains and ravage our villages\nPlease send these funds to real_nigerian_prince_the_third@boatmail.com\nYours in jegus,\nNigerian Prince";
	}
}
