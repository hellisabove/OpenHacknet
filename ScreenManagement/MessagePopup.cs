using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hacknet.ScreenManagement
{
	// Token: 0x0200012E RID: 302
	internal class MessagePopup : GameScreen
	{
		// Token: 0x06000738 RID: 1848 RVA: 0x00076288 File Offset: 0x00074488
		public MessagePopup(string message)
		{
			base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
			base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
			this.messageContent = message;
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x000762E1 File Offset: 0x000744E1
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x000762E4 File Offset: 0x000744E4
		public override void HandleInput(InputState input)
		{
			KeyboardState state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.Z) || state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.X))
			{
				base.ExitScreen();
			}
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x0007634C File Offset: 0x0007454C
		public override void Draw(GameTime gameTime)
		{
			SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
			Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
			Rectangle rectangle = new Rectangle(0, 0, viewport.Width, viewport.Height);
			Rectangle destinationRectangle = new Rectangle(viewport.Width / 2 - MessagePopup.BOX_WIDTH / 2, viewport.Height / 2 - MessagePopup.BOX_HEIGHT / 2, MessagePopup.BOX_WIDTH, MessagePopup.BOX_HEIGHT);
			byte transitionAlpha = base.TransitionAlpha;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			spriteBatch.Draw(Utils.white, destinationRectangle, new Color((int)transitionAlpha, (int)transitionAlpha, (int)transitionAlpha));
			SpriteFont font = base.ScreenManager.Font;
			Vector2 origin = new Vector2(0f, (float)(font.LineSpacing / 2));
			Vector2 position = new Vector2((float)destinationRectangle.X, (float)destinationRectangle.Y);
			spriteBatch.DrawString(font, this.messageContent, position, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
			spriteBatch.End();
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x0007645B File Offset: 0x0007465B
		public override void UnloadContent()
		{
		}

		// Token: 0x04000815 RID: 2069
		private static int BOX_WIDTH = 500;

		// Token: 0x04000816 RID: 2070
		private static int BOX_HEIGHT = 350;

		// Token: 0x04000817 RID: 2071
		private Texture2D blankTexture = null;

		// Token: 0x04000818 RID: 2072
		private string messageContent = "";
	}
}
