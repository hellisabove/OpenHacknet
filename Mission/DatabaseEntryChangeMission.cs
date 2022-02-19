using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000087 RID: 135
	internal class DatabaseEntryChangeMission : MisisonGoal
	{
		// Token: 0x060002B0 RID: 688 RVA: 0x00027807 File Offset: 0x00025A07
		public DatabaseEntryChangeMission(string computerIP, OS os, string operation, string FieldName, string targetValue, string recordName)
		{
			this.c = Programs.getComputer(os, computerIP);
			this.Operation = operation;
			this.FieldName = FieldName;
			this.TargetValue = targetValue;
			this.RecordName = recordName;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00027840 File Offset: 0x00025A40
		public override bool isComplete(List<string> additionalDetails = null)
		{
			bool result;
			try
			{
				DatabaseDaemon databaseDaemon = (DatabaseDaemon)this.c.getDaemon(typeof(DatabaseDaemon));
				object objectForRecordName = databaseDaemon.GetObjectForRecordName(this.RecordName);
				if (objectForRecordName != null)
				{
					object valueFromObject = ObjectSerializer.GetValueFromObject(objectForRecordName, this.FieldName);
					try
					{
						double num = Convert.ToDouble(valueFromObject);
						double num2 = Convert.ToDouble(this.TargetValue);
						string operation = this.Operation;
						if (operation != null)
						{
							if (operation == ">" || operation == "greater")
							{
								return num > num2;
							}
							if (operation == "<" || operation == "less")
							{
								return num < num2;
							}
							if (operation == "=" || operation == "equals")
							{
								return Math.Abs(num - num2) < 0.0001;
							}
						}
					}
					catch (FormatException)
					{
						if (valueFromObject == null)
						{
							return false;
						}
						return valueFromObject.ToString() == this.TargetValue;
					}
				}
				result = false;
			}
			catch (Exception ex)
			{
				Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
				result = true;
			}
			return result;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x000279B8 File Offset: 0x00025BB8
		public override string TestCompletable()
		{
			return "";
		}

		// Token: 0x040002D9 RID: 729
		private Computer c;

		// Token: 0x040002DA RID: 730
		private string Operation;

		// Token: 0x040002DB RID: 731
		private string RecordName;

		// Token: 0x040002DC RID: 732
		private string FieldName;

		// Token: 0x040002DD RID: 733
		private string TargetValue;
	}
}
