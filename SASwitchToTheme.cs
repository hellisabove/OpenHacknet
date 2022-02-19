using System;
using System.Xml;
using Hacknet.Extensions;

namespace Hacknet
{
	// Token: 0x0200003C RID: 60
	public class SASwitchToTheme : SerializableAction
	{
		// Token: 0x06000150 RID: 336 RVA: 0x00013518 File Offset: 0x00011718
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				OSTheme ostheme = OSTheme.Custom;
				if (!Enum.TryParse<OSTheme>(this.ThemePathOrName, out ostheme))
				{
					ostheme = OSTheme.Custom;
				}
				try
				{
					os.EffectsUpdater.StartThemeSwitch(this.FlickerInDuration, ostheme, os, (ostheme == OSTheme.Custom) ? this.ThemePathOrName : null);
				}
				catch (Exception ex)
				{
					os.write(" ");
					os.write("---------------------- ");
					os.write("ERROR LOADING X-SERVER");
					os.write(ex.ToString());
					os.write(ex.Message);
					string text = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/xserver_error.txt";
					Utils.writeToFile("x-server load error for theme : " + this.ThemePathOrName + "\r\n." + Utils.GenerateReportFromException(ex), text);
					os.write("---------------------- ");
					os.write("Full report saved to " + text);
					os.write("---------------------- ");
					os.write(" ");
				}
			}
			else
			{
				Computer computer = Programs.getComputer(os, this.DelayHost);
				if (computer == null)
				{
					throw new NullReferenceException("Computer " + computer + " could not be found as DelayHost for Function");
				}
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction(this, delay);
			}
		}

		// Token: 0x06000151 RID: 337 RVA: 0x000136AC File Offset: 0x000118AC
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SASwitchToTheme saswitchToTheme = new SASwitchToTheme();
			if (rdr.MoveToAttribute("Delay"))
			{
				saswitchToTheme.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("ThemePathOrName"))
			{
				saswitchToTheme.ThemePathOrName = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("FlickerInDuration"))
			{
				saswitchToTheme.FlickerInDuration = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				saswitchToTheme.DelayHost = rdr.ReadContentAsString();
			}
			return saswitchToTheme;
		}

		// Token: 0x04000149 RID: 329
		public string ThemePathOrName;

		// Token: 0x0400014A RID: 330
		public float FlickerInDuration = 2f;

		// Token: 0x0400014B RID: 331
		public string DelayHost;

		// Token: 0x0400014C RID: 332
		public float Delay;
	}
}
