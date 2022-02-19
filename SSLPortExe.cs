using System;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000069 RID: 105
	internal class SSLPortExe : ExeModule
	{
		// Token: 0x06000212 RID: 530 RVA: 0x0001CF08 File Offset: 0x0001B108
		private SSLPortExe(Rectangle location, OS operatingSystem, SSLPortExe.SSLMode mode) : base(location, operatingSystem)
		{
			this.name = "SSLTrojan";
			this.ramCost = 220;
			this.IdentifierName = "SSLTrojan";
			this.Mode = mode;
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			computer.hostileActionTaken();
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0001CF9C File Offset: 0x0001B19C
		public static SSLPortExe GenerateInstanceOrNullFromArguments(string[] args, Rectangle location, object osObj, Computer target)
		{
			OS os = (OS)osObj;
			SSLPortExe result;
			if (args.Length < 4)
			{
				os.write("--------------------------------------");
				os.write("SSLTrojan " + LocaleTerms.Loc("ERROR: Not enough arguments!"));
				os.write(string.Concat(new string[]
				{
					LocaleTerms.Loc("Usage:"),
					" SSLTrojan [",
					LocaleTerms.Loc("PortNum"),
					"] [-",
					LocaleTerms.Loc("option"),
					"] [",
					LocaleTerms.Loc("option port num"),
					"]"
				}));
				os.write(LocaleTerms.Loc("Valid Options:") + " [-s (SSH)] [-f (FTP)] [-w (HTTP)] [-r (RTSP)]");
				os.write("--------------------------------------");
				result = null;
			}
			else
			{
				try
				{
					int num = Convert.ToInt32(args[1]);
					int num2 = Convert.ToInt32(args[3]);
					string text = args[2].ToLower();
					SSLPortExe.SSLMode mode = SSLPortExe.SSLMode.SSH;
					string text2 = text;
					if (text2 != null)
					{
						if (text2 == "-s")
						{
							mode = SSLPortExe.SSLMode.SSH;
							goto IL_15B;
						}
						if (text2 == "-f")
						{
							mode = SSLPortExe.SSLMode.FTP;
							goto IL_15B;
						}
						if (text2 == "-w")
						{
							mode = SSLPortExe.SSLMode.Web;
							goto IL_15B;
						}
						if (text2 == "-r")
						{
							mode = SSLPortExe.SSLMode.RTSP;
							goto IL_15B;
						}
					}
					text = null;
					IL_15B:
					if (text == null)
					{
						os.write("--------------------------------------");
						os.write("SSLTrojan " + string.Format(LocaleTerms.Loc("Error: Mode {0} is invalid."), args[2]));
						os.write(LocaleTerms.Loc("Valid Options:") + " [-s (SSH)] [-f (FTP)] [-w (HTTP)] [-r (RTSP)]");
						os.write("--------------------------------------");
						result = null;
					}
					else
					{
						int num3 = -1;
						bool flag = false;
						switch (mode)
						{
						case SSLPortExe.SSLMode.SSH:
							flag = target.isPortOpen(22);
							num3 = target.GetDisplayPortNumberFromCodePort(22);
							break;
						case SSLPortExe.SSLMode.FTP:
							flag = target.isPortOpen(21);
							num3 = target.GetDisplayPortNumberFromCodePort(21);
							break;
						case SSLPortExe.SSLMode.Web:
							flag = target.isPortOpen(80);
							num3 = target.GetDisplayPortNumberFromCodePort(80);
							break;
						case SSLPortExe.SSLMode.RTSP:
							flag = target.isPortOpen(554);
							num3 = target.GetDisplayPortNumberFromCodePort(554);
							break;
						}
						if (!flag)
						{
							os.write("--------------------------------------");
							os.write("SSLTrojan " + LocaleTerms.Loc("Error: Target bypass port is closed!"));
							result = null;
						}
						else
						{
							int num4 = -1;
							try
							{
								num4 = Convert.ToInt32(num2);
							}
							catch (FormatException)
							{
								os.write("--------------------------------------");
								os.write("SSLTrojan " + string.Format(LocaleTerms.Loc("Error: Invalid tunnel port number : \"{0}\""), num2));
								return null;
							}
							if (num4 != num3)
							{
								os.write("--------------------------------------");
								os.write("SSLTrojan " + string.Format(LocaleTerms.Loc("Error: Tunnel port number {0} does not match expected service \"{1}"), num4, text));
								result = null;
							}
							else
							{
								result = new SSLPortExe(location, os, mode);
							}
						}
					}
				}
				catch (Exception ex)
				{
					os.write("SSLTrojan " + LocaleTerms.Loc("Error:"));
					os.write(ex.Message);
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0001D330 File Offset: 0x0001B530
		public override void Update(float t)
		{
			base.Update(t);
			if (this.elapsedTime < 12f && this.elapsedTime + t >= 12f)
			{
				this.Completed();
			}
			else if (this.elapsedTime < 15f && this.elapsedTime + t >= 15f)
			{
				this.isExiting = true;
			}
			this.elapsedTime += t;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0001D3B0 File Offset: 0x0001B5B0
		public override void Completed()
		{
			base.Completed();
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(443, this.os.thisComputer.ip);
				this.os.write(":: " + LocaleTerms.Loc("HTTPS SSL Reverse Tunnel Opened"));
				this.IsComplete = true;
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0001D424 File Offset: 0x0001B624
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Rectangle rectangle = Utils.InsetRectangle(new Rectangle(this.bounds.X, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width, this.bounds.Height - Module.PANEL_HEIGHT), 2);
			if (this.bounds.Height >= Module.PANEL_HEIGHT + 4)
			{
				float num = 1f;
				if (this.elapsedTime < 12f)
				{
					num = this.elapsedTime / 12f;
				}
				float num2 = 0.6f;
				float num3 = Utils.CubicInCurve(Utils.CubicInCurve(Math.Min(1f, num * (1f / num2))));
				float num4 = Math.Max(0f, Math.Min(1f, (num - num2) * (1f / (1f - num2))));
				int num5 = 7;
				bool flag = true;
				Vector2 vector = new Vector2((float)(this.bounds.X + this.bounds.Width / 2), (float)(this.bounds.Y + Module.PANEL_HEIGHT + this.bounds.Height / 2 - 12));
				float num6 = num2 * 12f;
				float num7 = (this.elapsedTime - num6) / (15f - num6);
				if (num3 < 0.99f)
				{
					num7 = 0f;
				}
				Vector2 left = vector;
				float num8 = (float)this.bounds.Width - (100f - num7 * 1500f);
				float num9 = num4 * 1f;
				Vector2 vector2 = new Vector2((float)Math.Sin((double)(this.os.timer * 2f)) * (num8 / 4f), (float)Math.Cos((double)(this.os.timer * 2f)) * (num8 / 4f));
				vector -= this.LastCentralRenderOffset * num4 * 1f;
				this.LastCentralRenderOffset = Vector2.Zero;
				for (int i = 0; i < num5; i++)
				{
					int num10 = 13;
					int subdivisions = num10;
					if (i == 0)
					{
						subdivisions = (int)((float)(num10 - 1) * Math.Min(1f, num3 + 0.0831f)) + 1;
					}
					float num11 = Math.Max(1f, (float)num5 - (float)i * 2f + 1f);
					num11 += num9;
					float num12 = this.os.timer * 2f;
					if (flag)
					{
						num12 *= ((i % 2 == 0) ? 1f : -1f);
					}
					bool flag2 = (float)i < 1f + (float)((int)(6f * num4));
					if (flag2)
					{
						TunnelingCircleEffect.Draw(this.spriteBatch, vector, num8, num11, subdivisions, num12, Color.Lerp(Color.White, Color.Transparent, Utils.QuadraticOutCurve((float)i / (float)num5)), Color.Gray * (0.12f * (1f - num) + 0.1f), this.os.highlightColor, rectangle);
						Vector2 vector3;
						Vector2 vector4;
						Utils.ClipLineSegmentsForRect(rectangle, left, vector, out vector3, out vector4);
						Utils.drawLineAlt(this.spriteBatch, vector3, vector4, Vector2.Zero, this.os.highlightColor * 0.3f, 0.6f, 1f, Utils.gradientLeftRight);
					}
					float num13 = num8 / 4f;
					left = vector;
					Vector2 value = new Vector2((float)Math.Sin((double)num12) * num13, (float)Math.Cos((double)num12) * num13);
					this.LastCentralRenderOffset += value;
					vector += value;
					num8 = num8 / 2f - num11;
					if (i == num5 - 1 && num3 > 0.99f)
					{
						if (vector.X > (float)rectangle.X && vector.X < (float)(rectangle.X + rectangle.Width) && vector.Y > (float)rectangle.Y && vector.Y < (float)(rectangle.Y + rectangle.Height))
						{
							this.spriteBatch.Draw(Utils.white, vector, null, (num4 > 0.99f) ? this.os.unlockedColor : this.os.highlightColor, 0f, Vector2.Zero, Vector2.One * 2f, SpriteEffects.None, 0.7f);
						}
						if (this.IsComplete)
						{
							int num14 = 2;
							Rectangle destinationRectangle = new Rectangle((int)vector.X - num14 / 2, rectangle.Y, num14, rectangle.Height);
							this.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.makeColorAddative(this.os.highlightColor));
							Rectangle rectangle2 = new Rectangle(rectangle.X, (int)vector.Y - num14 / 2, rectangle.Width, num14);
							if (rectangle2.Y > rectangle.Y + rectangle.Height - 30 || rectangle2.Y < rectangle.Y)
							{
								rectangle2.Y = rectangle.Y + rectangle.Height / 2 - num14;
							}
							this.spriteBatch.Draw(Utils.white, rectangle2, Utils.makeColorAddative(this.os.highlightColor));
							rectangle2.Height = Math.Min(24, rectangle.Height / 2);
							rectangle2.Y = rectangle.Y + rectangle.Height / 2 - rectangle2.Height / 2;
							if (Utils.randm(0.25f) + 0.75f < num7)
							{
								this.spriteBatch.Draw(Utils.white, rectangle2, Color.Black * 0.5f);
								TextItem.doFontLabelToSize(rectangle2, "   -  " + LocaleTerms.Loc("SSL TUNNEL COMPLETE") + "  -", GuiData.smallfont, Color.White, true, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x04000242 RID: 578
		private const float RUN_TIME = 12f;

		// Token: 0x04000243 RID: 579
		private const float IDLE_TIME = 15f;

		// Token: 0x04000244 RID: 580
		private float elapsedTime = 0f;

		// Token: 0x04000245 RID: 581
		private SSLPortExe.SSLMode Mode = SSLPortExe.SSLMode.SSH;

		// Token: 0x04000246 RID: 582
		private Vector2 LastCentralRenderOffset = Vector2.Zero;

		// Token: 0x04000247 RID: 583
		private bool IsComplete = false;

		// Token: 0x0200006A RID: 106
		private enum SSLMode
		{
			// Token: 0x04000249 RID: 585
			SSH,
			// Token: 0x0400024A RID: 586
			FTP,
			// Token: 0x0400024B RID: 587
			Web,
			// Token: 0x0400024C RID: 588
			RTSP
		}
	}
}
