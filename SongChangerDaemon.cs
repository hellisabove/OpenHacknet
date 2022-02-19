using System;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000017 RID: 23
	internal class SongChangerDaemon : Daemon
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x0000CA5C File Offset: 0x0000AC5C
		public SongChangerDaemon(Computer c, OS os) : base(c, LocaleTerms.Loc("Music Changer"), os)
		{
			this.topEffect = new MovingBarsEffect();
			this.botEffect = new MovingBarsEffect
			{
				IsInverted = true
			};
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000CAA0 File Offset: 0x0000ACA0
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			this.topEffect.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			this.botEffect.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			int num = 30;
			Rectangle rectangle = new Rectangle(bounds.X + 30, bounds.Y + bounds.Height / 2 - num / 2, bounds.Width - 60, num);
			Rectangle bounds2 = rectangle;
			bounds2.Height = 60;
			bounds2.Y -= bounds2.Height;
			this.topEffect.Draw(sb, bounds2, 1f, 3f, 1f, this.os.highlightColor);
			if (Button.doButton(73518921, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Shuffle Music"), new Color?(this.os.highlightColor)))
			{
				int maxValue = 12;
				int value = Utils.random.Next(maxValue) + 1;
				MissionFunctions.runCommand(value, "changeSong");
			}
			bounds2.Y += bounds2.Height + num;
			this.botEffect.Draw(sb, bounds2, 1f, 3f, 1f, this.os.highlightColor);
			Rectangle rectangle2 = new Rectangle(bounds.X + 4, bounds.Y + bounds.Height - 4 - 20, (int)((float)bounds.Width * 0.5f), 20);
			if (Button.doButton(73518924, rectangle2.X, rectangle2.Y, rectangle2.Width, rectangle2.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
			{
				this.os.display.command = "connect";
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000CCC0 File Offset: 0x0000AEC0
		public override string getSaveString()
		{
			return "<SongChangerDaemon />";
		}

		// Token: 0x040000B8 RID: 184
		private MovingBarsEffect topEffect;

		// Token: 0x040000B9 RID: 185
		private MovingBarsEffect botEffect;
	}
}
