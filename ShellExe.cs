using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x0200015E RID: 350
	internal class ShellExe : ExeModule
	{
		// Token: 0x060008CF RID: 2255 RVA: 0x000935FC File Offset: 0x000917FC
		public ShellExe(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.ramCost = ShellExe.BASE_RAM_COST;
			this.IdentifierName = "Shell@" + this.targetIP;
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00093668 File Offset: 0x00091868
		public override void LoadContent()
		{
			base.LoadContent();
			this.infoBar = new Rectangle(this.bounds.X, this.bounds.Y, this.bounds.Width, ShellExe.INFOBAR_HEIGHT);
			this.os.shells.Add(this);
			this.os.shellIPs.Add(this.targetIP);
			this.compThisShellIsRunningOn = Programs.getComputer(this.os, this.targetIP);
			this.compThisShellIsRunningOn.log(this.os.thisComputer.ip + "_Opened_#SHELL");
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x00093714 File Offset: 0x00091914
		public override void Update(float t)
		{
			base.Update(t);
			if (!Programs.getComputer(this.os, this.targetIP).adminIP.Equals(this.os.thisComputer.ip) || Programs.getComputer(this.os, this.targetIP).disabled)
			{
				if (this.state != -1)
				{
					this.os.write(">>");
					this.os.write(">> " + string.Format(LocaleTerms.Loc("SHELL ERROR: Administrator account lost on {0}"), this.compThisShellIsRunningOn.ip));
					this.os.write(">>");
				}
				this.Completed();
			}
			if (this.targetRamUse != this.ramCost)
			{
				if (this.targetRamUse < this.ramCost)
				{
					this.ramCost -= (int)(t * ShellExe.RAM_CHANGE_PS);
					if (this.ramCost < this.targetRamUse)
					{
						this.ramCost = this.targetRamUse;
					}
				}
				else
				{
					int num = (int)(t * ShellExe.RAM_CHANGE_PS);
					if (this.os.ramAvaliable >= num)
					{
						this.ramCost += num;
						if (this.ramCost > this.targetRamUse)
						{
							this.ramCost = this.targetRamUse;
						}
					}
				}
			}
			switch (this.state)
			{
			case 1:
				if (this.destComp.hasProxy)
				{
					this.destComp.proxyOverloadTicks -= t;
					if (this.destComp.proxyOverloadTicks <= 0f)
					{
						this.destComp.proxyOverloadTicks = 0f;
						this.destComp.proxyActive = false;
						this.completedAction(1);
					}
					else
					{
						this.destComp.hostileActionTaken();
					}
				}
				break;
			}
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x0009393C File Offset: 0x00091B3C
		public override void Draw(float t)
		{
			string identifierName = this.IdentifierName;
			this.IdentifierName = "@" + this.targetIP;
			base.Draw(t);
			base.drawOutline();
			base.drawTarget("S");
			this.IdentifierName = identifierName;
			this.doGui();
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x00093990 File Offset: 0x00091B90
		public void doGui()
		{
			int num = this.state;
			if (num == 2)
			{
				if (this.ramCost == this.targetRamUse)
				{
					Color value = this.os.highlightColor;
					if (this.os.opponentLocation.Equals(this.targetIP))
					{
						value = this.os.lockedColor;
					}
					if (Button.doButton(95000 + this.os.exes.IndexOf(this), this.bounds.X + 10, this.bounds.Y + 20, this.bounds.Width - 20, 50, LocaleTerms.Loc("Trigger"), new Color?(value)))
					{
						this.destComp.forkBombClients(this.targetIP);
						this.completedAction(2);
						this.compThisShellIsRunningOn.log("#SHELL_TrapActivate_:_ConnectionsFlooded");
					}
				}
			}
			this.doControlButtons();
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x00093A98 File Offset: 0x00091C98
		public void StartOverload()
		{
			this.state = 1;
			this.targetRamUse = ShellExe.BASE_RAM_COST;
			this.destinationIP = ((this.os.connectedComp == null) ? this.os.thisComputer.ip : this.os.connectedComp.ip);
			if (this.destComp == null || this.destComp.ip != this.destinationIP)
			{
				this.compThisShellIsRunningOn.log("#SHELL_Overload_@_" + this.destinationIP);
			}
			this.destComp = Programs.getComputer(this.os, this.destinationIP);
			this.destCompIndex = this.os.netMap.nodes.IndexOf(this.destComp);
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x00093B6C File Offset: 0x00091D6C
		public void doControlButtons()
		{
			int num = 2;
			int num2 = 76;
			int num3 = this.bounds.Width - (num + 3 * num + num2 - 10);
			int num4 = (int)((double)num3 * 0.33333);
			int num5 = this.bounds.X + num + 1;
			int num6 = this.bounds.Height - Module.PANEL_HEIGHT - 8;
			int num7 = this.bounds.Y + this.bounds.Height - num6 - 6;
			if (this.bounds.Height - Module.PANEL_HEIGHT > 5)
			{
				Button.ForceNoColorTag = true;
				if (this.state == 2)
				{
					int num8 = 50;
					int num9 = 17;
					num7 += num8 + 9;
					num6 = num9;
				}
				if (Button.doButton(89200 + this.os.exes.IndexOf(this), num5, num7, (int)((double)num3 * 0.5), num6, LocaleTerms.Loc("Overload"), new Color?(this.os.shellButtonColor * this.fade)))
				{
					this.StartOverload();
				}
				if (Button.doButton(89300 + this.os.exes.IndexOf(this), num5 + ((int)((double)num3 * 0.5) + num), num7, (int)((double)num3 * 0.36), num6, LocaleTerms.Loc("Trap"), new Color?(this.os.shellButtonColor * this.fade)))
				{
					this.state = 2;
					this.targetRamUse = ShellExe.TRAP_RAM_USE;
					this.destinationIP = ((this.os.connectedComp == null) ? this.os.thisComputer.ip : this.os.connectedComp.ip);
					if (this.destComp == null || this.destComp.ip != this.destinationIP)
					{
						this.compThisShellIsRunningOn.log("#SHELL_TrapAcive");
					}
					this.destComp = Programs.getComputer(this.os, this.destinationIP);
					this.destCompIndex = this.os.netMap.nodes.IndexOf(this.destComp);
				}
				if (Button.doButton(89101 + this.os.exes.IndexOf(this), this.bounds.X + this.bounds.Width - num2 - (num + 1), num7, num2, num6, LocaleTerms.Loc("Close"), new Color?(this.os.lockedColor * this.fade)))
				{
					this.Completed();
				}
				Button.ForceNoColorTag = false;
			}
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x00093E49 File Offset: 0x00092049
		public void completedAction(int action)
		{
			this.cancelTarget();
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x00093E53 File Offset: 0x00092053
		public void cancelTarget()
		{
			this.state = 0;
			this.destinationIP = "";
			this.destComp = null;
			this.destCompIndex = -1;
			this.targetRamUse = ShellExe.BASE_RAM_COST;
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x00093E84 File Offset: 0x00092084
		public override void Completed()
		{
			base.Completed();
			this.cancelTarget();
			this.state = -1;
			this.os.shells.Remove(this);
			this.os.shellIPs.Remove(this.targetIP);
			if (!this.isExiting)
			{
				this.compThisShellIsRunningOn.log("#SHELL_Closed");
			}
			this.isExiting = true;
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x00093EF2 File Offset: 0x000920F2
		public void reportedTo(string data)
		{
		}

		// Token: 0x04000A42 RID: 2626
		private const int IDLE_STATE = 0;

		// Token: 0x04000A43 RID: 2627
		private const int CLOSING_STATE = -1;

		// Token: 0x04000A44 RID: 2628
		private const int PROXY_OVERLOAD_STATE = 1;

		// Token: 0x04000A45 RID: 2629
		private const int FORKBOMB_TRAP_STATE = 2;

		// Token: 0x04000A46 RID: 2630
		public static int INFOBAR_HEIGHT = 16;

		// Token: 0x04000A47 RID: 2631
		public static int BASE_RAM_COST = 40;

		// Token: 0x04000A48 RID: 2632
		public static float RAM_CHANGE_PS = 200f;

		// Token: 0x04000A49 RID: 2633
		public static int TRAP_RAM_USE = 100;

		// Token: 0x04000A4A RID: 2634
		private Rectangle infoBar;

		// Token: 0x04000A4B RID: 2635
		public string destinationIP = "";

		// Token: 0x04000A4C RID: 2636
		private Computer destComp = null;

		// Token: 0x04000A4D RID: 2637
		private Computer compThisShellIsRunningOn = null;

		// Token: 0x04000A4E RID: 2638
		private int destCompIndex = -1;

		// Token: 0x04000A4F RID: 2639
		private int state = 0;

		// Token: 0x04000A50 RID: 2640
		private int targetRamUse = ShellExe.BASE_RAM_COST;
	}
}
