using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200006C RID: 108
	internal class TuneswapExe : ExeModule
	{
		// Token: 0x0600021B RID: 539 RVA: 0x0001DED8 File Offset: 0x0001C0D8
		public TuneswapExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.needsProxyAccess = false;
			this.name = "Tuneswap";
			this.ramCost = 300;
			this.IdentifierName = "Tuneswap";
			this.backdrop = new RaindropsEffect();
			this.backdrop.Init(this.os.content);
			this.backdrop.MaxVerticalLandingVariane = 0.06f;
			this.backdrop.FallRate = 0.2f;
			this.themeColor = new Color(38, 171, 146, 0) * 0.4f;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0001E08E File Offset: 0x0001C28E
		public override void Update(float t)
		{
			base.Update(t);
			this.backdrop.Update(t, 50f);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0001E0AB File Offset: 0x0001C2AB
		public override void Completed()
		{
			base.Completed();
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0001E0B8 File Offset: 0x0001C2B8
		private bool CanActivateSong(int i)
		{
			return MusicManager.currentSongName != this.SongOptions[i];
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0001E0E0 File Offset: 0x0001C2E0
		private void ActivateSong(string song)
		{
			if (!this.SongOptions.Contains(MusicManager.currentSongName))
			{
				this.oldPlayingSong = MusicManager.currentSongName;
			}
			try
			{
				MusicManager.playSongImmediatley(song);
			}
			catch (Exception)
			{
				this.os.write("Tuneswap.exe :: ERROR PLAYING SONG " + song + "\n -- EXITING\n");
				this.isExiting = true;
			}
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0001E154 File Offset: 0x0001C354
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Rectangle contentAreaDest = base.GetContentAreaDest();
			Rectangle dest = contentAreaDest;
			dest.Height = (int)((float)dest.Height + 20f);
			this.backdrop.Render(dest, this.spriteBatch, this.themeColor, 5f, 30f);
			int num = contentAreaDest.X + 10;
			int width = contentAreaDest.Width - 20;
			int num2 = contentAreaDest.Y + 10;
			if (!this.isExiting)
			{
				this.mousedOverArtistName = null;
				for (int i = 0; i < this.SongOptions.Count; i++)
				{
					Rectangle destinationRectangle = new Rectangle(num, num2, width, 20);
					this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black * 0.85f);
					int num3 = 10777001 + this.PID * 1000 + i;
					if (Button.doButton(num3, num, num2, width, 20, this.SongNames[i], new Color?(this.CanActivateSong(i) ? this.themeColor : Color.Gray)))
					{
						this.ActivateSong(this.SongOptions[i]);
					}
					if (GuiData.hot == num3)
					{
						this.mousedOverArtistName = this.ArtistNames[i];
					}
					num2 += 23;
				}
				if (this.oldPlayingSong != null)
				{
					if (Button.doButton(10797009 + this.PID * 1000, num, num2, width, 20, LocaleTerms.Loc("Default") + " Track", new Color?(this.themeColor)))
					{
						this.ActivateSong(this.oldPlayingSong);
					}
					num2 += 23;
				}
				if (this.mousedOverArtistName != null)
				{
					this.spriteBatch.DrawString(GuiData.smallfont, this.mousedOverArtistName, new Vector2((float)num, (float)num2), Color.White);
				}
				Rectangle destinationRectangle2 = new Rectangle(num, contentAreaDest.Y + contentAreaDest.Height - 24, width, 20);
				this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black * 0.6f);
				if (Button.doButton(109271000 + this.PID, num, contentAreaDest.Y + contentAreaDest.Height - 24, width, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.isExiting = true;
				}
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0001E420 File Offset: 0x0001C620
		public void RenderMainDisplay(Rectangle dest, SpriteBatch sb)
		{
		}

		// Token: 0x04000255 RID: 597
		private RaindropsEffect backdrop;

		// Token: 0x04000256 RID: 598
		private Color themeColor = Color.Pink;

		// Token: 0x04000257 RID: 599
		private string oldPlayingSong = null;

		// Token: 0x04000258 RID: 600
		private List<string> SongOptions = new List<string>(new string[]
		{
			"DLC\\Music\\snidelyWhiplash",
			"DLC\\Music\\Userspacelike",
			"DLC\\Music\\Slow_Motion",
			"DLC\\Music\\World_Chase",
			"DLC\\Music\\HOME_Resonance",
			"DLC\\Music\\Remi2",
			"DLC\\Music\\Remi_Finale",
			"DLC\\Music\\DreamHead"
		});

		// Token: 0x04000259 RID: 601
		private List<string> SongNames = new List<string>(new string[]
		{
			"Snidely Whiplash",
			"Payload (AKA Userspacelike)",
			"Slow Motion",
			"World Chase",
			"Resonance",
			"ClearText",
			"Sabotage (AKA Altitude_Loss)",
			"Dream Head"
		});

		// Token: 0x0400025A RID: 602
		private List<string> ArtistNames = new List<string>(new string[]
		{
			"OGRE",
			"The Algorithm",
			"Tonspender",
			"Cinematrik",
			"HOME",
			"The Algorithm",
			"The Algorithm",
			"HOME"
		});

		// Token: 0x0400025B RID: 603
		private string mousedOverArtistName = null;
	}
}
