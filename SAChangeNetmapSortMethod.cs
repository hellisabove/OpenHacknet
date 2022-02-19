using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000045 RID: 69
	public class SAChangeNetmapSortMethod : SerializableAction
	{
		// Token: 0x0600016C RID: 364 RVA: 0x00014674 File Offset: 0x00012874
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				NetmapSortingAlgorithm sortingAlgorithm = NetmapSortingAlgorithm.Grid;
				string text = this.Method.ToLower();
				switch (text)
				{
				case "scatter":
					sortingAlgorithm = NetmapSortingAlgorithm.Scatter;
					break;
				case "grid":
					sortingAlgorithm = NetmapSortingAlgorithm.Grid;
					break;
				case "chaos":
					sortingAlgorithm = NetmapSortingAlgorithm.Chaos;
					break;
				case "scangrid":
				case "seqgrid":
				case "sequencegrid":
				case "sequence grid":
					sortingAlgorithm = NetmapSortingAlgorithm.LockGrid;
					break;
				}
				os.netMap.SortingAlgorithm = sortingAlgorithm;
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

		// Token: 0x0600016D RID: 365 RVA: 0x000147C8 File Offset: 0x000129C8
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAChangeNetmapSortMethod sachangeNetmapSortMethod = new SAChangeNetmapSortMethod();
			if (rdr.MoveToAttribute("Delay"))
			{
				sachangeNetmapSortMethod.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sachangeNetmapSortMethod.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("Method"))
			{
				sachangeNetmapSortMethod.Method = rdr.ReadContentAsString();
			}
			return sachangeNetmapSortMethod;
		}

		// Token: 0x0400016F RID: 367
		public string Method;

		// Token: 0x04000170 RID: 368
		public string DelayHost;

		// Token: 0x04000171 RID: 369
		public float Delay;
	}
}
