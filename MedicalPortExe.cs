using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x020000C9 RID: 201
	internal class MedicalPortExe : ExeModule
	{
		// Token: 0x0600040F RID: 1039 RVA: 0x000409D8 File Offset: 0x0003EBD8
		public MedicalPortExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.needsProxyAccess = true;
			this.name = "KBT_PortTest";
			this.ramCost = 400;
			this.IdentifierName = "KBT Port Tester";
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00040A78 File Offset: 0x0003EC78
		public override void Update(float t)
		{
			base.Update(t);
			if (this.sucsessTimer <= 0f)
			{
				this.elapsedTime += t;
				if (this.elapsedTime >= 22f)
				{
					this.Complete();
				}
			}
			else
			{
				this.sucsessTimer -= t;
				if (this.sucsessTimer <= 0f)
				{
					this.isExiting = true;
				}
			}
			if (this.displayData == null)
			{
				this.InitializeDisplay();
			}
			this.UpdateDisplay();
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00040B18 File Offset: 0x0003ED18
		private void InitializeDisplay()
		{
			this.displayData = new Color[this.bounds.Height - 4 - Module.PANEL_HEIGHT];
			this.CompletedIndexes = new bool[this.displayData.Length];
			for (int i = 0; i < this.displayData.Length; i++)
			{
				this.CompletedIndexes[i] = false;
			}
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00040B7C File Offset: 0x0003ED7C
		private void UpdateDisplay()
		{
			float num = 22f / (float)this.displayData.Length;
			if (this.elapsedTime % num < 0.02f)
			{
				int num2 = 0;
				int num3;
				do
				{
					num3 = Utils.random.Next(this.displayData.Length);
					num2++;
				}
				while (this.CompletedIndexes[num3] && num2 < this.bounds.Height * 2);
				this.CompletedIndexes[num3] = true;
			}
			int num4 = Utils.random.Next(this.displayData.Length);
			if (this.CompletedIndexes[num4])
			{
				this.displayData[num4] = Color.Lerp(this.displayData[num4], this.LightBaseColor, Utils.rand(0.5f));
			}
			for (int i = 0; i < this.displayData.Length; i++)
			{
				if (this.CompletedIndexes[i])
				{
					this.displayData[i] = Color.Lerp(this.displayData[i], Color.Lerp(this.DarkFinColor, this.LightFinColor, Utils.rand(1f)), 0.05f);
				}
				else
				{
					this.displayData[i] = Color.Lerp(this.displayData[i], Color.Lerp(this.DarkBaseColor, this.LightBaseColor, Utils.rand(1f)), 0.05f);
				}
			}
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00040D28 File Offset: 0x0003EF28
		private void Complete()
		{
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(104, this.os.thisComputer.ip);
			}
			this.sucsessTimer = 2f;
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00040D78 File Offset: 0x0003EF78
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			if (this.displayData != null)
			{
				Rectangle bounds = this.bounds;
				bounds.X++;
				bounds.Width -= 2;
				bounds.Y += 2 + Module.PANEL_HEIGHT;
				bounds.Height = 1;
				int width = bounds.Width;
				for (int i = 0; i < this.displayData.Length; i++)
				{
					float num = 0.85f * (1f - this.elapsedTime / 22f);
					float num2 = 0.95f - num;
					if (this.CompletedIndexes[i])
					{
						bounds.Width = (int)((float)width * (Utils.rand(num) + num2));
					}
					else
					{
						bounds.Width = width;
					}
					this.spriteBatch.Draw(Utils.white, bounds, this.displayData[i]);
					bounds.Y++;
					if (bounds.Y > this.bounds.Y + this.bounds.Height - 2)
					{
						break;
					}
				}
			}
		}

		// Token: 0x040004E5 RID: 1253
		private const float RUNTIME = 22f;

		// Token: 0x040004E6 RID: 1254
		private const float COMPLETE_TIME = 2f;

		// Token: 0x040004E7 RID: 1255
		private float elapsedTime = 0f;

		// Token: 0x040004E8 RID: 1256
		private float sucsessTimer = 0f;

		// Token: 0x040004E9 RID: 1257
		private Color[] displayData;

		// Token: 0x040004EA RID: 1258
		private bool[] CompletedIndexes;

		// Token: 0x040004EB RID: 1259
		private Color DarkBaseColor = new Color(5, 0, 36);

		// Token: 0x040004EC RID: 1260
		private Color LightBaseColor = new Color(39, 32, 83);

		// Token: 0x040004ED RID: 1261
		private Color DarkFinColor = new Color(179, 25, 94);

		// Token: 0x040004EE RID: 1262
		private Color LightFinColor = new Color(225, 14, 79);
	}
}
