using System;
using System.Collections.Generic;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Daemons.Helpers
{
	// Token: 0x0200009C RID: 156
	public class BasicMedicalMonitor : IMedicalMonitor
	{
		// Token: 0x06000329 RID: 809 RVA: 0x0002E4F7 File Offset: 0x0002C6F7
		public BasicMedicalMonitor(Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>> updateAction, Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>> heartbeatAction)
		{
			this.UpdateAction = updateAction;
			this.HeartBeatAction = heartbeatAction;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0002E520 File Offset: 0x0002C720
		public float GetCurrentValue(float timeRollback)
		{
			int num = 0;
			Vector2 vector = new Vector2(0f, 0f);
			BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint;
			for (float num2 = 0f; num2 < timeRollback; num2 += monitorRecordKeypoint.timeOffset)
			{
				monitorRecordKeypoint = this.data.Get(num);
				if (monitorRecordKeypoint.timeOffset == 0f)
				{
					break;
				}
				if (num2 + monitorRecordKeypoint.timeOffset >= timeRollback)
				{
					BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint2 = this.data.Get(num - 1);
					float num3 = (timeRollback - num2) / monitorRecordKeypoint.timeOffset;
					Vector2 value = new Vector2(0f, monitorRecordKeypoint2.value * 1f);
					Vector2 value2 = new Vector2(0f, monitorRecordKeypoint.value * 1f);
					vector = Vector2.Lerp(value, value2, 1f - num3);
					num--;
					break;
				}
				num--;
			}
			return vector.Y;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0002E620 File Offset: 0x0002C820
		public void Update(float dt)
		{
			List<BasicMedicalMonitor.MonitorRecordKeypoint> list = this.UpdateAction(this.data.Get(0).value, dt);
			for (int i = 0; i < list.Count; i++)
			{
				this.data.Add(list[i]);
			}
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0002E678 File Offset: 0x0002C878
		public void HeartBeat(float beatTime)
		{
			List<BasicMedicalMonitor.MonitorRecordKeypoint> list = this.HeartBeatAction(this.data.Get(0).value, beatTime);
			for (int i = 0; i < list.Count; i++)
			{
				this.data.Add(list[i]);
			}
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0002E6D0 File Offset: 0x0002C8D0
		public void Draw(Rectangle bounds, SpriteBatch sb, Color c, float timeRollback)
		{
			int num = 0;
			float num2 = 4f;
			float num3 = 100f;
			float num4 = (float)bounds.Height / 3f;
			float num5 = (float)bounds.Width - num2;
			Vector2 value = new Vector2(num5, 0f);
			bool flag = false;
			Vector2 value2 = new Vector2((float)bounds.X, (float)bounds.Y + (float)bounds.Height / 2f);
			if (timeRollback > 0f)
			{
				BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint;
				for (float num6 = 0f; num6 < timeRollback; num6 += monitorRecordKeypoint.timeOffset)
				{
					monitorRecordKeypoint = this.data.Get(num);
					if (monitorRecordKeypoint.timeOffset == 0f)
					{
						break;
					}
					if (num6 + monitorRecordKeypoint.timeOffset >= timeRollback)
					{
						BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint2 = this.data.Get(num - 1);
						float num7 = (timeRollback - num6) / monitorRecordKeypoint.timeOffset;
						Vector2 value3 = new Vector2(num5, monitorRecordKeypoint2.value * num4);
						Vector2 vector = new Vector2(num5, monitorRecordKeypoint.value * num4);
						value = Vector2.Lerp(value3, vector, 1f - num7);
						flag = true;
						num5 -= monitorRecordKeypoint.timeOffset * num3 * (1f - num7);
						num--;
						break;
					}
					num--;
				}
			}
			while (num5 >= num2)
			{
				BasicMedicalMonitor.MonitorRecordKeypoint monitorRecordKeypoint = this.data.Get(num);
				if (monitorRecordKeypoint.timeOffset == 0f)
				{
					break;
				}
				Vector2 vector = new Vector2(num5, monitorRecordKeypoint.value * num4);
				if (flag)
				{
					Utils.drawLine(sb, value2 + value, value2 + vector, Vector2.Zero, c, 0.56f);
				}
				num5 -= monitorRecordKeypoint.timeOffset * num3;
				value = vector;
				num--;
				flag = true;
			}
		}

		// Token: 0x04000384 RID: 900
		private CLinkBuffer<BasicMedicalMonitor.MonitorRecordKeypoint> data = new CLinkBuffer<BasicMedicalMonitor.MonitorRecordKeypoint>(1024);

		// Token: 0x04000385 RID: 901
		private Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>> UpdateAction;

		// Token: 0x04000386 RID: 902
		private Func<float, float, List<BasicMedicalMonitor.MonitorRecordKeypoint>> HeartBeatAction;

		// Token: 0x0200009D RID: 157
		public struct MonitorRecordKeypoint
		{
			// Token: 0x04000387 RID: 903
			public float timeOffset;

			// Token: 0x04000388 RID: 904
			public float value;
		}
	}
}
