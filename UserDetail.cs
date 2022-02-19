using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000100 RID: 256
	public struct UserDetail
	{
		// Token: 0x0600059A RID: 1434 RVA: 0x00057EBC File Offset: 0x000560BC
		public UserDetail(string user, string password, byte accountType)
		{
			this.name = user;
			this.pass = password;
			this.type = accountType;
			this.known = false;
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x00057EDB File Offset: 0x000560DB
		public UserDetail(string user)
		{
			this.name = user;
			this.pass = PortExploits.getRandomPassword();
			this.type = 1;
			this.known = false;
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x00057F00 File Offset: 0x00056100
		public string getSaveString()
		{
			return string.Concat(new object[]
			{
				"<user name=\"",
				this.name,
				"\" pass=\"",
				this.pass,
				"\" type=\"",
				this.type,
				"\" known=\"",
				this.known,
				"\" />"
			});
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x00057F78 File Offset: 0x00056178
		public static UserDetail loadUserDetail(XmlReader reader)
		{
			reader.MoveToAttribute("name");
			string user = reader.ReadContentAsString();
			reader.MoveToAttribute("pass");
			string password = reader.ReadContentAsString();
			reader.MoveToAttribute("type");
			byte accountType = (byte)reader.ReadContentAsInt();
			reader.MoveToAttribute("known");
			string text = reader.ReadContentAsString();
			bool flag = text.ToLower().Equals("true");
			return new UserDetail(user, password, accountType)
			{
				known = flag
			};
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x00058004 File Offset: 0x00056204
		public override bool Equals(object obj)
		{
			UserDetail? userDetail = obj as UserDetail?;
			return userDetail != null && (this.name == userDetail.Value.name && this.pass == userDetail.Value.pass && this.type == userDetail.Value.type) && this.known == userDetail.Value.known;
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x00058094 File Offset: 0x00056294
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x04000656 RID: 1622
		public string name;

		// Token: 0x04000657 RID: 1623
		public string pass;

		// Token: 0x04000658 RID: 1624
		public byte type;

		// Token: 0x04000659 RID: 1625
		public bool known;
	}
}
