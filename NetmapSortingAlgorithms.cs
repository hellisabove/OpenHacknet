using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000029 RID: 41
	public static class NetmapSortingAlgorithms
	{
		// Token: 0x0600010B RID: 267 RVA: 0x000103E8 File Offset: 0x0000E5E8
		internal static Vector2 GetNodePosition(NetmapSortingAlgorithm algorithm, float mapWidth, float mapHeight, Computer node, int nodeIndex, int totalNodes, int totalRevealedNodes, OS os)
		{
			Vector2 result;
			switch (algorithm)
			{
			default:
			{
				Vector2 vector = Utils.Clamp(node.location, 0f, 1f);
				result = new Vector2(vector.X * mapWidth, vector.Y * mapHeight);
				break;
			}
			case NetmapSortingAlgorithm.Grid:
			{
				int num = 5;
				int num2 = (int)((float)totalNodes / (float)num) + 1;
				if (totalRevealedNodes > 60)
				{
					num = 7;
					num2 = (int)((float)totalNodes / (float)num) + 1;
				}
				if (totalRevealedNodes > 150)
				{
					num = 9;
					num2 = (int)((float)totalNodes / (float)num) + 1;
				}
				int num3 = (int)((float)nodeIndex / (float)num + 0f);
				int num4 = num * num2;
				if (totalNodes > num4 - 10)
				{
					num3 = (int)((float)nodeIndex / (float)num + 0.5f);
					num2++;
				}
				int num5 = nodeIndex % num;
				float num6 = (float)num3 / ((float)num2 - 2f);
				float num7 = (float)num5 / ((float)num - 1f);
				NetmapSortingAlgorithms.ClipToMargins(num6, num7, totalRevealedNodes, out num6, out num7);
				num6 *= mapWidth;
				num7 *= mapHeight;
				Vector2 vector2 = new Vector2(num6, num7);
				result = vector2;
				break;
			}
			case NetmapSortingAlgorithm.Chaos:
				result = new Vector2(Utils.randm(1f) * mapWidth, Utils.randm(1f) * mapHeight);
				break;
			case NetmapSortingAlgorithm.LockGrid:
			{
				int num8 = 5;
				int count = os.netMap.visibleNodes.Count;
				int num9 = count + (int)((float)(totalNodes - count) / 8f);
				int num10 = (int)((float)num9 / (float)num8) + 1;
				if (count > 50)
				{
					num8 = 6;
					num10 = (int)((float)count / (float)num8) + 1;
				}
				if (count > 80)
				{
					num8 = 7;
					num10 = (int)((float)count / (float)num8) + 1;
				}
				if (count > 110)
				{
					num8 = 8;
					num10 = (int)((float)count / (float)num8) + 1;
				}
				if (count > 150)
				{
					num8 = 9;
					num10 = (int)((float)count / (float)num8) + 1;
				}
				num10++;
				num10 = Math.Max(num10, 3);
				int num11 = os.netMap.visibleNodes.IndexOf(os.netMap.nodes.IndexOf(node));
				if (num11 == -1)
				{
					num11 = 1;
				}
				int num12 = num11 / num8;
				if (count > 150)
				{
					num12 = (int)((float)num11 / (float)num8 + 0.5f);
					num10++;
				}
				int num13 = num11 % num8;
				float num14 = (float)num12 / ((float)num10 - 2f);
				float num15 = (float)num13 / ((float)num8 - 1f);
				NetmapSortingAlgorithms.ClipToMargins(num14, num15, totalRevealedNodes, out num14, out num15);
				num14 *= mapWidth;
				num15 *= mapHeight;
				Vector2 vector3 = new Vector2(num14, num15);
				result = vector3;
				break;
			}
			}
			return result;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000106C4 File Offset: 0x0000E8C4
		private static void ClipToMargins(float x, float y, int revealed, out float xout, out float yout)
		{
			bool flag = revealed > 60;
			xout = Math.Min(flag ? 0.999f : 0.98f, Math.Max(flag ? 0.001f : 0.02f, x));
			yout = Math.Min(flag ? 0.98f : 0.93f, Math.Max(flag ? 0.002f : 0.02f, y));
		}
	}
}
