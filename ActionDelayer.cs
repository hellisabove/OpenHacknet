using System;
using System.Collections.Generic;

namespace Hacknet
{
	// Token: 0x02000093 RID: 147
	internal class ActionDelayer
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x0002913C File Offset: 0x0002733C
		// (set) Token: 0x060002D8 RID: 728 RVA: 0x00029153 File Offset: 0x00027353
		public DateTime Time { get; private set; }

		// Token: 0x060002D9 RID: 729 RVA: 0x0002915C File Offset: 0x0002735C
		public void Pump()
		{
			this.Time = DateTime.Now;
			this.pairs.AddRange(this.nextPairs);
			this.nextPairs.Clear();
			for (int i = 0; i < this.pairs.Count; i++)
			{
				ActionDelayer.Pair pair = this.pairs[i];
				if (pair.Condition(this))
				{
					pair.Action();
					this.pairs.RemoveAt(i--);
				}
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x000291F4 File Offset: 0x000273F4
		public void RunAllDelayedActions()
		{
			this.pairs.AddRange(this.nextPairs);
			this.nextPairs.Clear();
			this.Time = DateTime.MaxValue;
			for (int i = 0; i < this.pairs.Count; i++)
			{
				ActionDelayer.Pair pair = this.pairs[i];
				if (pair.Condition(this))
				{
					pair.Action();
					this.pairs.RemoveAt(i--);
				}
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0002928C File Offset: 0x0002748C
		public void Post(ActionDelayer.Condition condition, Action action)
		{
			this.nextPairs.Add(new ActionDelayer.Pair
			{
				Condition = condition,
				Action = action
			});
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00029308 File Offset: 0x00027508
		public void PostAnimation(IEnumerator<ActionDelayer.Condition> animation)
		{
			Action tick = null;
			tick = delegate()
			{
				if (animation.MoveNext())
				{
					this.Post(animation.Current, tick);
				}
			};
			tick();
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0002937C File Offset: 0x0002757C
		public static ActionDelayer.Condition WaitUntil(DateTime time)
		{
			return (ActionDelayer x) => x.Time >= time;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x000293A8 File Offset: 0x000275A8
		public static ActionDelayer.Condition Wait(double time)
		{
			return ActionDelayer.WaitUntil(DateTime.Now + TimeSpan.FromSeconds(time));
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000293E4 File Offset: 0x000275E4
		public static ActionDelayer.Condition NextTick()
		{
			return (ActionDelayer x) => true;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00029448 File Offset: 0x00027648
		public static ActionDelayer.Condition FileDeleted(Folder f, string filename)
		{
			return (ActionDelayer x) => !f.containsFile(filename);
		}

		// Token: 0x0400031A RID: 794
		private List<ActionDelayer.Pair> pairs = new List<ActionDelayer.Pair>();

		// Token: 0x0400031B RID: 795
		private List<ActionDelayer.Pair> nextPairs = new List<ActionDelayer.Pair>();

		// Token: 0x02000094 RID: 148
		// (Invoke) Token: 0x060002E4 RID: 740
		public delegate bool Condition(ActionDelayer messagePump);

		// Token: 0x02000095 RID: 149
		private struct Pair
		{
			// Token: 0x0400031E RID: 798
			public ActionDelayer.Condition Condition;

			// Token: 0x0400031F RID: 799
			public Action Action;
		}
	}
}
