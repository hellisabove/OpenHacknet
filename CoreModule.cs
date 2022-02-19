using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000D6 RID: 214
	internal class CoreModule : Module
	{
		// Token: 0x0600044C RID: 1100 RVA: 0x00045479 File Offset: 0x00043679
		public CoreModule(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00045494 File Offset: 0x00043694
		public override void LoadContent()
		{
			base.LoadContent();
			if (CoreModule.LockSprite == null)
			{
				CoreModule.LockSprite = this.os.content.Load<Texture2D>("Lock");
			}
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x000454D4 File Offset: 0x000436D4
		public override void PreDrawStep()
		{
			base.PreDrawStep();
			if (this.inputLocked)
			{
				this.guiInputLockStatus = GuiData.blockingInput;
				GuiData.blockingInput = true;
			}
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0004550C File Offset: 0x0004370C
		public override void PostDrawStep()
		{
			base.PostDrawStep();
			if (this.inputLocked)
			{
				GuiData.blockingInput = false;
				GuiData.blockingInput = this.guiInputLockStatus;
				Rectangle bounds = this.bounds;
				if (bounds.Contains(GuiData.getMousePoint()))
				{
					GuiData.spriteBatch.Draw(Utils.white, bounds, Color.Gray * 0.5f);
					Vector2 position = new Vector2((float)(bounds.X + bounds.Width / 2 - CoreModule.LockSprite.Width / 2), (float)(bounds.Y + bounds.Height / 2 - CoreModule.LockSprite.Height / 2));
					GuiData.spriteBatch.Draw(CoreModule.LockSprite, position, Color.White);
				}
			}
		}

		// Token: 0x0400052B RID: 1323
		private static Texture2D LockSprite;

		// Token: 0x0400052C RID: 1324
		public bool inputLocked = false;

		// Token: 0x0400052D RID: 1325
		private bool guiInputLockStatus = false;
	}
}
