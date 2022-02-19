using System;
using System.IO;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000192 RID: 402
	public static class MemoryDumpInjector
	{
		// Token: 0x06000A26 RID: 2598 RVA: 0x000A2018 File Offset: 0x000A0218
		public static void InjectMemory(string memoryFilepath, object computer)
		{
			using (FileStream fileStream = File.OpenRead(memoryFilepath))
			{
				XmlReader rdr = XmlReader.Create(fileStream);
				MemoryContents memory = MemoryContents.Deserialize(rdr);
				Computer computer2 = (Computer)computer;
				computer2.Memory = memory;
			}
		}
	}
}
