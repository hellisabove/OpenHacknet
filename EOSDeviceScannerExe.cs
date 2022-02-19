using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000C8 RID: 200
	internal class EOSDeviceScannerExe : ExeModule
	{
		// Token: 0x0600040B RID: 1035 RVA: 0x0003FCF0 File Offset: 0x0003DEF0
		public EOSDeviceScannerExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "eOS_DeviceScanner";
			this.ramCost = 300;
			this.IdentifierName = "eOS Device Scanner";
			this.targetComp = this.os.connectedComp;
			if (this.targetComp == null)
			{
				this.targetComp = this.os.thisComputer;
			}
			this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
			this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
			if (!this.os.hasConnectionPermission(true))
			{
				this.isError = true;
				this.errorMessage = LocaleTerms.Loc("ADMIN ACCESS\nREQUIRED FOR SCAN");
				this.IsComplete = true;
				for (int i = 0; i < 30; i++)
				{
					this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
				}
			}
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0003FE40 File Offset: 0x0003E040
		public override void Update(float t)
		{
			base.Update(t);
			this.timer += t;
			bool flag = this.targetComp.attatchedDeviceIDs == null;
			if ((this.timer > 8f || (flag && this.timer > 3.5f)) && !this.IsComplete)
			{
				this.Completed();
			}
			if (this.timer < 8f && this.errorMessage == null)
			{
				this.timeToNextBounce -= t;
				if (this.timeToNextBounce <= 0f)
				{
					this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
					this.timeToNextBounce = 0.07f;
				}
			}
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00040160 File Offset: 0x0003E360
		public override void Completed()
		{
			base.Completed();
			this.IsComplete = true;
			if (this.targetComp.attatchedDeviceIDs != null)
			{
				string[] array = this.targetComp.attatchedDeviceIDs.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
				float num = 0f;
				for (int i = 0; i < array.Length; i++)
				{
					Computer device = Programs.getComputer(this.os, array[i]);
					if (device != null)
					{
						Action action = delegate()
						{
							this.os.netMap.discoverNode(device);
							Vector2 loc = this.os.netMap.GetNodeDrawPos(device) + new Vector2((float)this.os.netMap.bounds.X, (float)this.os.netMap.bounds.Y) + new Vector2((float)(NetworkMap.NODE_SIZE / 2));
							SFX.addCircle(loc, this.os.highlightColor, 120f);
							this.os.delayer.Post(ActionDelayer.Wait(0.2), delegate
							{
								SFX.addCircle(loc, this.os.highlightColor, 80f);
							});
							this.os.delayer.Post(ActionDelayer.Wait(0.4), delegate
							{
								SFX.addCircle(loc, this.os.highlightColor, 65f);
							});
							string text = string.Format(LocaleTerms.Loc("eOS Device \"{0}\" opened for connection at {1}"), device.name, device.ip);
							this.os.write(text);
							this.ResultTitles.Add(device.name);
							this.ResultBodies.Add(string.Concat(new string[]
							{
								device.ip,
								" ",
								device.location.ToString(),
								"\n",
								Guid.NewGuid().ToString()
							}));
						};
						this.os.delayer.Post(ActionDelayer.Wait((double)num), action);
						num += 1f;
						this.devicesFound++;
					}
				}
			}
			if (this.devicesFound == 0)
			{
				this.isError = true;
			}
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00040268 File Offset: 0x0003E468
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			bool drawShadow = TextItem.DrawShadow;
			TextItem.DrawShadow = false;
			Vector2 value = this.locations[0];
			Vector2 vector = new Vector2((float)this.bounds.X + 2f, (float)this.bounds.Y + 26f);
			Vector2 value2 = new Vector2((float)this.bounds.Width - 4f, (float)this.bounds.Height - 30f);
			if (value2.X > 0f && value2.Y > 0f)
			{
				for (int i = 1; i < this.locations.Count; i++)
				{
					Vector2 vector2 = this.locations[i];
					if (i == this.locations.Count - 1)
					{
						vector2 = Vector2.Lerp(value, vector2, 1f - this.timeToNextBounce / 0.07f);
					}
					Utils.drawLine(this.spriteBatch, vector + value * value2, vector + vector2 * value2, Vector2.Zero, (this.isError ? Utils.AddativeRed : Utils.AddativeWhite) * 0.5f * ((float)i / (float)this.locations.Count), 0.4f);
					value = this.locations[i];
				}
				for (int i = 1; i < this.locations.Count; i++)
				{
					this.spriteBatch.Draw(Utils.white, this.locations[i] * value2 + vector, Utils.AddativeWhite);
				}
			}
			SpriteFont font = (Settings.ActiveLocale == "en-us") ? GuiData.titlefont : GuiData.font;
			if (this.IsComplete)
			{
				bool flag = this.errorMessage != null;
				Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 26, this.bounds.Width - 2, this.bounds.Height - 28);
				this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.7f);
				Rectangle rectangle2 = rectangle;
				rectangle2.Height = Math.Min(35, this.bounds.Height / 5);
				string text = flag ? "ERROR" : "SCAN COMPLETE";
				text = LocaleTerms.Loc(text);
				TextItem.doFontLabel(new Vector2((float)rectangle2.X, (float)rectangle2.Y), text, font, new Color?(this.os.highlightColor), (float)rectangle2.Width, (float)rectangle2.Height, false);
				TextItem.doFontLabel(new Vector2((float)rectangle2.X, (float)rectangle2.Y), Utils.FlipRandomChars(text, 0.1), font, new Color?(Utils.AddativeWhite * (0.1f * Utils.rand())), (float)rectangle2.Width, (float)rectangle2.Height, false);
				Rectangle destinationRectangle = new Rectangle(rectangle2.X, rectangle2.Y + rectangle2.Height, rectangle2.Width, 1);
				this.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.5f);
				if (!this.isExiting)
				{
					string text2 = string.Format(LocaleTerms.Loc("DEVICES FOUND : {0}"), this.devicesFound);
					if (flag)
					{
						text2 = this.errorMessage;
					}
					Vector2 pos = new Vector2((float)(rectangle2.X + 2), (float)(rectangle2.Y + rectangle2.Height + 2));
					TextItem.doFontLabel(pos, text2, font, new Color?(((this.devicesFound > 0) ? Utils.AddativeWhite : Color.Red) * 0.8f), (float)(this.bounds.Width - 10), flag ? ((float)this.bounds.Height * 0.8f) : 23f, false);
					pos.Y += 25f;
					for (int i = 0; i < this.ResultTitles.Count; i++)
					{
						if (pos.Y - (float)this.bounds.Y + 60f > (float)this.bounds.Height)
						{
							break;
						}
						TextItem.doFontLabel(pos, Utils.FlipRandomChars(this.ResultTitles[i], 0.01), GuiData.font, new Color?(Color.Lerp(this.os.highlightColor, Utils.AddativeWhite, 0.2f + 0.1f * Utils.rand())), (float)(this.bounds.Width - 10), 24f, false);
						pos.Y += 22f;
						TextItem.doFontLabel(pos, this.ResultBodies[i], GuiData.detailfont, new Color?(Utils.AddativeWhite * 0.85f), (float)(this.bounds.Width - 10), 30f, false);
						pos.Y += 30f;
						pos.Y += 4f;
					}
				}
				if (!this.isExiting && Button.doButton(646464029 + this.PID, this.bounds.X + 2, this.bounds.Y + this.bounds.Height - 2 - 20, this.bounds.Width - 50, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.isExiting = true;
				}
			}
			else
			{
				int num = Math.Min(38, this.bounds.Height / 3);
				Rectangle rectangle3 = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height / 2 - num / 2, this.bounds.Width - 2, num);
				this.spriteBatch.Draw(Utils.white, rectangle3, Color.Black * 0.7f);
				TextItem.doFontLabelToSize(rectangle3, Utils.FlipRandomChars(LocaleTerms.Loc("SCANNING"), 0.009), font, this.IsComplete ? this.os.highlightColor : (Utils.AddativeWhite * 0.8f), false, false);
				TextItem.doFontLabelToSize(rectangle3, Utils.FlipRandomChars(LocaleTerms.Loc("SCANNING"), 0.15), font, this.IsComplete ? this.os.highlightColor : (Utils.AddativeWhite * (0.18f * Utils.rand())), false, false);
			}
			TextItem.DrawShadow = drawShadow;
		}

		// Token: 0x040004D8 RID: 1240
		private const float TOTAL_TIME = 8f;

		// Token: 0x040004D9 RID: 1241
		private const float SHORTCUT_TIME = 3.5f;

		// Token: 0x040004DA RID: 1242
		private const float timeBetweenBounces = 0.07f;

		// Token: 0x040004DB RID: 1243
		private List<Vector2> locations = new List<Vector2>();

		// Token: 0x040004DC RID: 1244
		private float timeToNextBounce = 0f;

		// Token: 0x040004DD RID: 1245
		private List<string> ResultTitles = new List<string>();

		// Token: 0x040004DE RID: 1246
		private List<string> ResultBodies = new List<string>();

		// Token: 0x040004DF RID: 1247
		private float timer = 0f;

		// Token: 0x040004E0 RID: 1248
		private Computer targetComp;

		// Token: 0x040004E1 RID: 1249
		private bool IsComplete = false;

		// Token: 0x040004E2 RID: 1250
		private int devicesFound = 0;

		// Token: 0x040004E3 RID: 1251
		private bool isError = false;

		// Token: 0x040004E4 RID: 1252
		private string errorMessage = null;
	}
}
