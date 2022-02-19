using System;
using System.Globalization;
using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000004 RID: 4
	internal class AircraftDaemon : Daemon
	{
		// Token: 0x06000010 RID: 16 RVA: 0x0000227C File Offset: 0x0000047C
		public AircraftDaemon(Computer c, OS os, string name, Vector2 mapOrigin, Vector2 mapDest, float progress) : base(c, name, os)
		{
			this.WorldMap = os.content.Load<Texture2D>("DLC/Sprites/SmallWorldMap");
			this.Circle = os.content.Load<Texture2D>("Circle");
			this.StatusOKIcon = os.content.Load<Texture2D>("CircleOutlineLarge");
			this.CircleOutline = os.content.Load<Texture2D>("CircleOutlineLarge");
			this.CautionIcon = os.content.Load<Texture2D>("Sprites/Icons/CautionIcon");
			this.Plane = os.content.Load<Texture2D>("DLC/Sprites/Airplane");
			this.mapOrigin = mapOrigin;
			this.mapDest = mapDest;
			this.FlightProgress = progress;
			this.ThemeColor = os.highlightColor;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000023B4 File Offset: 0x000005B4
		public override void initFiles()
		{
			base.initFiles();
			this.MainFolder = this.comp.files.root.searchForFolder("FlightSystems");
			if (this.MainFolder == null)
			{
				this.MainFolder = new Folder("FlightSystems");
				this.comp.files.root.folders.Add(this.MainFolder);
			}
			FileEntry item = new FileEntry(PortExploits.ValidAircraftOperatingDLL, "747FlightOps.dll");
			this.MainFolder.files.Add(item);
			this.MainFolder.files.Add(new FileEntry(Computer.generateBinaryString(200), "InFlightWifiRouter.dll"));
			this.MainFolder.files.Add(new FileEntry(Computer.generateBinaryString(200), "Scheduler.dll"));
			this.MainFolder.files.Add(new FileEntry(Computer.generateBinaryString(200), "EntertainmentServices.dll"));
			this.MainFolder.files.Add(new FileEntry(Computer.generateBinaryString(200), "AnnouncementsSys.dll"));
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000024E2 File Offset: 0x000006E2
		public override void loadInit()
		{
			base.loadInit();
			this.MainFolder = this.comp.files.root.searchForFolder("FlightSystems");
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000250C File Offset: 0x0000070C
		public override void navigatedTo()
		{
			base.navigatedTo();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002518 File Offset: 0x00000718
		public void StartUpdating()
		{
			if (!this.IsSubscribedForUpdates)
			{
				OS os = this.os;
				os.UpdateSubscriptions = (Action<float>)Delegate.Combine(os.UpdateSubscriptions, new Action<float>(this.Update));
			}
			this.IsSubscribedForUpdates = true;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002560 File Offset: 0x00000760
		public void UnsubscribeFromUpdates()
		{
			if (this.IsSubscribedForUpdates)
			{
				OS os = this.os;
				os.UpdateSubscriptions = (Action<float>)Delegate.Remove(os.UpdateSubscriptions, new Action<float>(this.Update));
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000025A4 File Offset: 0x000007A4
		private void Update(float t)
		{
			if (this.IsReloadingFirmware)
			{
				this.firmwareReloadProgress += t;
				if (this.firmwareReloadProgress > 6f)
				{
					this.FinishReloadingFirmware();
				}
			}
			this.timeSinceLastDataUpdate += t;
			if (this.timeSinceLastDataUpdate > 0.1f)
			{
				this.timeSinceLastDataUpdate -= 0.1f;
				t = 0.1f;
				if (this.IsInCriticalFirmwareFailure)
				{
					this.timeFallingFor += t;
					float num = -876.9231f;
					if (this.AircraftFallStartsImmediatley)
					{
						this.rateOfClimb = num;
						float num2 = this.timeFallingFor / 135f;
						this.CurrentAltitude = (double)(38000f * (1f - num2));
					}
					else
					{
						float num3 = 15f;
						this.rateOfClimb = Math.Max(num, 0f - Utils.QuadraticOutCurve(Math.Min(num3, this.timeFallingFor) / num3) * (-1f * num));
						this.CurrentAltitude += (double)(this.rateOfClimb * t);
					}
					if (Utils.FloatEquals(this.rateOfClimb, num))
					{
						this.rateOfClimb += (1f - Utils.rand(2f)) * t;
					}
					float num4 = -1600f;
					if (this.currentAirspeed < num4)
					{
						this.currentAirspeed -= (1f - Utils.rand(2f)) * this.rateOfClimb * t;
					}
					else
					{
						this.currentAirspeed -= this.rateOfClimb * t;
					}
				}
				else
				{
					double num5 = 38000.0;
					double num6 = 500.0;
					if ((double)this.currentAirspeed > num6 * 1.25)
					{
						this.currentAirspeed -= t * this.rateOfClimb * 3f;
					}
					else
					{
						this.currentAirspeed += (float)((double)(5 - Utils.random.Next(11)) * (double)t * (((double)this.currentAirspeed > num6) ? -2.0 : 1.0));
					}
					if (this.CurrentAltitude > num5 + 500.0)
					{
						this.CurrentAltitude -= (double)(this.rateOfClimb * t + (float)((double)(5 - Utils.random.Next(11)) * (double)t * 2.0) * ((this.CurrentAltitude > num5) ? -1f : 1f));
					}
					else
					{
						this.CurrentAltitude += (double)(this.rateOfClimb * t + (float)((double)(5 - Utils.random.Next(11)) * (double)t * 2.0) * ((this.CurrentAltitude > num5) ? -1f : 1f));
					}
					if (this.rateOfClimb < -0.1f || this.CurrentAltitude < 37500.0)
					{
						if (this.rateOfClimb < -1f)
						{
							this.rateOfClimb += -1f * this.rateOfClimb / 2f * t;
						}
						else
						{
							this.rateOfClimb += 5f * t;
						}
					}
					else if (this.rateOfClimb > 0.1f)
					{
						this.rateOfClimb -= 1.6666666f * t;
					}
					else
					{
						this.rateOfClimb += (Utils.rand(0.1f) - Utils.rand(0.06f)) * t;
					}
				}
				if (this.CurrentAltitude <= 0.0)
				{
					this.CrashAircraft();
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000029B8 File Offset: 0x00000BB8
		private void CrashAircraft()
		{
			if (this.os.connectedComp == this.comp)
			{
				this.os.execute("disconnect");
				this.os.display.command = "dc";
			}
			if (this.CrashAction != null)
			{
				this.CrashAction();
			}
			this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.comp));
			this.comp.ip = "DCLOC:" + this.comp.ip;
			this.UnsubscribeFromUpdates();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002A79 File Offset: 0x00000C79
		public void StartReloadFirmware()
		{
			this.StartUpdating();
			this.IsReloadingFirmware = true;
			this.firmwareReloadProgress = 0f;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002A98 File Offset: 0x00000C98
		private void FinishReloadingFirmware()
		{
			this.IsReloadingFirmware = false;
			FileEntry fileEntry = this.MainFolder.searchForFile("747FlightOps.dll");
			if (fileEntry == null || fileEntry.data != PortExploits.ValidAircraftOperatingDLL)
			{
				this.IsInCriticalFirmwareFailure = true;
			}
			else
			{
				this.IsInCriticalFirmwareFailure = false;
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002AF0 File Offset: 0x00000CF0
		public bool IsInCriticalDescent()
		{
			return this.rateOfClimb < -0.8f || this.CurrentAltitude < 800.0;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002B24 File Offset: 0x00000D24
		public override string getSaveString()
		{
			return string.Concat(new string[]
			{
				"<AircraftDaemon ",
				this.VSString("Name", this.name),
				this.VSString("OriginX", this.mapOrigin.X),
				this.VSString("OriginY", this.mapOrigin.Y),
				this.VSString("DestX", this.mapDest.X),
				this.VSString("DestY", this.mapDest.Y),
				this.VSString("Progress", this.FlightProgress),
				"/>"
			});
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002BE0 File Offset: 0x00000DE0
		private string VSString(string name, string result)
		{
			return name + "=\"" + result + "\" ";
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002C04 File Offset: 0x00000E04
		private string VSString(string name, float result)
		{
			return name + "=\"" + result.ToString("0.0000", CultureInfo.InvariantCulture) + "\" ";
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002C38 File Offset: 0x00000E38
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			if (!this.IsSubscribedForUpdates)
			{
				this.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			}
			Rectangle dest = Utils.InsetRectangle(bounds, 1);
			this.DrawMap(dest, sb);
			Rectangle bounds2 = new Rectangle(bounds.X, bounds.Y, (int)((double)bounds.Width * 0.666), (int)((double)bounds.Height * 0.666));
			this.DrawHeadings(bounds2, sb);
			AircraftAltitudeIndicator.RenderAltitudeIndicator(dest, sb, (int)(this.CurrentAltitude + 0.5), this.IsInCriticalDescent(), AircraftAltitudeIndicator.GetFlashRateFromTimer(OS.currentInstance.timer), 50000, 40000, 30000, 14000, 3000);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002D18 File Offset: 0x00000F18
		private void DrawHeadings(Rectangle bounds, SpriteBatch sb)
		{
			Rectangle rectangle = new Rectangle(bounds.X, bounds.Y + 4, bounds.Width, 40);
			Rectangle dest = rectangle;
			dest.X += 8;
			dest.Width -= 8;
			TextItem.doFontLabelToSize(dest, this.name, GuiData.font, Color.White, true, true);
			rectangle.Y += rectangle.Height - 1;
			rectangle.Height = 1;
			sb.Draw(Utils.white, rectangle, Color.White);
			Color themeColor = this.ThemeColor;
			rectangle.Y += 2;
			rectangle.Height = 20;
			Color patternColor = this.IsInCriticalFirmwareFailure ? Color.DarkRed : (themeColor * 0.28f);
			if (!this.IsInCriticalFirmwareFailure && this.PilotAlerted)
			{
				patternColor = this.os.warningColor * 0.5f;
			}
			PatternDrawer.draw(rectangle, 1f, themeColor * 0.1f, patternColor, sb, PatternDrawer.warningStripe);
			if (this.IsReloadingFirmware)
			{
				Rectangle destinationRectangle = rectangle;
				destinationRectangle.Width = (int)((float)destinationRectangle.Width * Utils.QuadraticOutCurve(this.firmwareReloadProgress / 6f));
				sb.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.4f);
			}
			Rectangle dest2 = Utils.InsetRectangle(rectangle, 1);
			string text = this.IsReloadingFirmware ? LocaleTerms.Loc("RELOADING FIRMWARE") : (this.IsInCriticalFirmwareFailure ? LocaleTerms.Loc("CRITICAL FIRMWARE FAILURE") : (this.PilotAlerted ? LocaleTerms.Loc("PILOT ALERTED") : LocaleTerms.Loc("FLIGHT IN PROGRESS")));
			TextItem.doCenteredFontLabel(dest2, text, GuiData.font, Color.White, false);
			Rectangle rectangle2 = new Rectangle(dest2.X, dest2.Y + dest2.Height + 8, dest2.Width, 24);
			int num = 4;
			int num2 = (rectangle2.Width - num * 3) / 3;
			if (Button.doButton(632877701, rectangle2.X, rectangle2.Y, num2 - 20, rectangle2.Height, LocaleTerms.Loc("Disconnect"), new Color?(this.os.lockedColor)))
			{
				this.os.runCommand("disconnect");
			}
			if (Button.doButton(632877703, rectangle2.X + num + num2 - 20, rectangle2.Y, num2 + 10 + num, rectangle2.Height, LocaleTerms.Loc("Pilot Alert"), new Color?(this.ThemeColor)))
			{
				this.PilotAlerted = true;
			}
			if (Button.doButton(632877706, rectangle2.X + num * 3 + num2 * 2 - 10, rectangle2.Y, num2 + 10 + num, rectangle2.Height, LocaleTerms.Loc("Reload Firmware"), new Color?(this.os.lockedColor)))
			{
				this.StartReloadFirmware();
			}
			Rectangle dest3 = new Rectangle(rectangle2.X + 6, rectangle2.Y + rectangle2.Height + 20, rectangle2.Width - 75, 70);
			byte status = (this.currentAirspeed <= 500f) ? 0 : ((this.currentAirspeed < 600f) ? 1 : 2);
			this.DrawFieldDisplay(dest3, sb, LocaleTerms.Loc("Air Speed (kn)"), this.currentAirspeed.ToString("0.0"), status);
			dest3.Y += dest3.Height + 6;
			byte status2 = (this.rateOfClimb > -0.2f) ? 0 : ((this.rateOfClimb > -1f) ? 1 : 2);
			this.DrawFieldDisplay(dest3, sb, LocaleTerms.Loc("Rate of Climb (f/s)"), this.rateOfClimb.ToString("0.000"), status2);
			dest3.Y += dest3.Height + 6;
			this.DrawFieldDisplay(dest3, sb, LocaleTerms.Loc("Heading (deg)"), string.Concat(67.228f), 0);
			dest3.Y += dest3.Height + 6;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003174 File Offset: 0x00001374
		private void DrawFieldDisplay(Rectangle dest, SpriteBatch sb, string title, string value, byte status)
		{
			Rectangle rectangle = new Rectangle(dest.X, dest.Y, dest.Height, dest.Height);
			Texture2D texture = (status == 0) ? this.StatusOKIcon : this.CautionIcon;
			Color color = (status == 0) ? this.ThemeColor : ((status == 1) ? Color.Orange : Color.Red);
			sb.Draw(this.CircleOutline, rectangle, color);
			if (status < 2 || this.os.timer % 0.4f >= 0.2f)
			{
				bool flag = status != 0;
				Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, rectangle.Width / 5);
				if (flag)
				{
					destinationRectangle.Y -= 2;
				}
				sb.Draw(texture, destinationRectangle, color);
			}
			Rectangle dest2 = new Rectangle(rectangle.X + rectangle.Width + 6, dest.Y, dest.Width - rectangle.Width, dest.Height / 3 - 1);
			TextItem.doFontLabelToSize(dest2, title, GuiData.font, color, true, true);
			Rectangle destinationRectangle2 = new Rectangle(dest2.X - 8, dest2.Y + dest2.Height, dest2.Width + 8, 1);
			dest2.Y += dest2.Height + 1;
			sb.Draw(Utils.white, destinationRectangle2, color);
			dest2.Height = (int)((float)dest.Height / 3f * 2f) - 1;
			TextItem.doFontLabelToSize(dest2, value, GuiData.font, (status == 0) ? (Color.White * 0.9f) : color, true, true);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003334 File Offset: 0x00001534
		private void DrawMap(Rectangle dest, SpriteBatch sb)
		{
			Rectangle rectangle = Utils.DrawSpriteAspectCorrect(dest, sb, this.WorldMap, Color.Gray, true);
			float num = 10f;
			Vector2 vector = new Vector2((float)rectangle.X + (float)rectangle.Width * this.mapOrigin.X, (float)rectangle.Y + (float)rectangle.Height * this.mapOrigin.Y);
			Vector2 vector2 = new Vector2((float)rectangle.X + (float)rectangle.Width * this.mapDest.X, (float)rectangle.Y + (float)rectangle.Height * this.mapDest.Y);
			sb.Draw(this.Circle, vector, null, Color.Black, 0f, this.Circle.GetCentreOrigin(), new Vector2((num + 3f) / (float)this.Circle.Width), SpriteEffects.None, 0.4f);
			sb.Draw(this.Circle, vector, null, Utils.AddativeWhite, 0f, this.Circle.GetCentreOrigin(), new Vector2(num / (float)this.Circle.Width), SpriteEffects.None, 0.4f);
			sb.Draw(this.Circle, vector2, null, Color.Black, 0f, this.Circle.GetCentreOrigin(), new Vector2((num + 3f) / (float)this.Circle.Width), SpriteEffects.None, 0.4f);
			sb.Draw(this.Circle, vector2, null, this.ThemeColor, 0f, this.Circle.GetCentreOrigin(), new Vector2(num / (float)this.Circle.Width), SpriteEffects.None, 0.4f);
			Utils.drawLine(sb, vector, vector2, Vector2.Zero, this.ThemeColor * 0.5f, 0.3f);
			Vector2 vector3 = Vector2.Lerp(vector, vector2, this.FlightProgress);
			float num2 = 55f;
			Vector2 scale = new Vector2(num2 / (float)this.Plane.Width);
			Vector2 vector4 = vector2 - vector3;
			float rotation = (float)(Math.Atan2((double)vector4.Y, (double)vector4.X) + 1.5707963267948966);
			sb.Draw(this.Plane, vector3, null, Color.Black, rotation, this.Plane.GetCentreOrigin(), scale, SpriteEffects.None, 0.4f);
			num2 = 53f;
			scale = new Vector2(num2 / (float)this.Plane.Width);
			sb.Draw(this.Plane, vector3, null, this.IsInCriticalFirmwareFailure ? Color.Red : this.ThemeColor, rotation, this.Plane.GetCentreOrigin(), scale, SpriteEffects.None, 0.4f);
		}

		// Token: 0x04000007 RID: 7
		private const float FlightHoursPerLengthUnit = 0.06855416f;

		// Token: 0x04000008 RID: 8
		private const float ReloadFirmwareTime = 6f;

		// Token: 0x04000009 RID: 9
		internal const string CriticalFilename = "747FlightOps.dll";

		// Token: 0x0400000A RID: 10
		private const float RoughTotalFallTimeSeconds = 135f;

		// Token: 0x0400000B RID: 11
		private const float StartingAltitude = 38000f;

		// Token: 0x0400000C RID: 12
		private Texture2D WorldMap;

		// Token: 0x0400000D RID: 13
		private Texture2D Circle;

		// Token: 0x0400000E RID: 14
		private Texture2D Plane;

		// Token: 0x0400000F RID: 15
		private Texture2D CautionIcon;

		// Token: 0x04000010 RID: 16
		private Texture2D StatusOKIcon;

		// Token: 0x04000011 RID: 17
		private Texture2D CircleOutline;

		// Token: 0x04000012 RID: 18
		private Vector2 mapOrigin;

		// Token: 0x04000013 RID: 19
		private Vector2 mapDest;

		// Token: 0x04000014 RID: 20
		private float FlightProgress;

		// Token: 0x04000015 RID: 21
		public double CurrentAltitude = 37900.0;

		// Token: 0x04000016 RID: 22
		private float currentAirspeed = 460f;

		// Token: 0x04000017 RID: 23
		private float rateOfClimb = 0.073f;

		// Token: 0x04000018 RID: 24
		private Color ThemeColor = Color.CornflowerBlue;

		// Token: 0x04000019 RID: 25
		private Folder MainFolder;

		// Token: 0x0400001A RID: 26
		private bool PilotAlerted = false;

		// Token: 0x0400001B RID: 27
		private bool IsReloadingFirmware = false;

		// Token: 0x0400001C RID: 28
		private float firmwareReloadProgress = 0f;

		// Token: 0x0400001D RID: 29
		private float timeFallingFor = 0f;

		// Token: 0x0400001E RID: 30
		private float timeSinceLastDataUpdate = 0f;

		// Token: 0x0400001F RID: 31
		private bool IsSubscribedForUpdates = false;

		// Token: 0x04000020 RID: 32
		public bool IsInCriticalFirmwareFailure = false;

		// Token: 0x04000021 RID: 33
		public bool AircraftFallStartsImmediatley = true;

		// Token: 0x04000022 RID: 34
		public Action CrashAction;
	}
}
