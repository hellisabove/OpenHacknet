using System;
using System.Runtime.InteropServices;
using Hacknet.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200018D RID: 397
	internal class WebRenderer
	{
		// Token: 0x06000A0F RID: 2575 RVA: 0x000A13DC File Offset: 0x0009F5DC
		public static void setSize(int frameWidth, int frameHeight)
		{
			WebRenderer.width = frameWidth;
			WebRenderer.height = frameHeight;
			if (WebRenderer.Enabled)
			{
				if (WebRenderer.graphics != null)
				{
					if (WebRenderer.texture != null)
					{
						WebRenderer.texture.Dispose();
					}
					WebRenderer.texture = new Texture2D(WebRenderer.graphics, WebRenderer.width, WebRenderer.height, false, SurfaceFormat.Color);
					WebRenderer.texBuffer = new byte[WebRenderer.width * WebRenderer.height * 4];
					XNAWebRenderer.XNAWR_SetViewport(WebRenderer.width, WebRenderer.height);
				}
				WebRenderer.loadingPage = true;
			}
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x000A1470 File Offset: 0x0009F670
		public static void init(GraphicsDevice gd)
		{
			if (!WebRenderer.Enabled)
			{
				WebRenderer.loadingPage = true;
			}
			else
			{
				WebRenderer.instance = new WebRenderer();
				WebRenderer.graphics = gd;
				WebRenderer.loadingPage = true;
			}
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x000A14A8 File Offset: 0x0009F6A8
		public static void navigateTo(string urlTo)
		{
			WebRenderer.loadingPage = true;
			Console.WriteLine("Launching Web Thread");
			WebRenderer.url = urlTo;
			if (WebRenderer.Enabled)
			{
				XNAWebRenderer.XNAWR_LoadURL(new Uri(WebRenderer.url).ToString());
			}
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x000A14F0 File Offset: 0x0009F6F0
		private static void TextureUpdated(IntPtr buffer)
		{
			try
			{
				Marshal.Copy(buffer, WebRenderer.texBuffer, 0, WebRenderer.texBuffer.Length);
				WebRenderer.texture.SetData<byte>(WebRenderer.texBuffer);
				WebRenderer.loadingPage = false;
			}
			catch (AccessViolationException value)
			{
				Console.WriteLine(value);
				WebRenderer.navigateTo(WebRenderer.url);
			}
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x000A1554 File Offset: 0x0009F754
		public static void drawTo(Rectangle bounds, SpriteBatch sb)
		{
			if (!WebRenderer.loadingPage)
			{
				sb.Draw(WebRenderer.texture, bounds, Color.White);
			}
			else
			{
				WebpageLoadingEffect.DrawLoadingEffect(bounds, sb, OS.currentInstance, true);
			}
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x000A1594 File Offset: 0x0009F794
		public static string getURL()
		{
			return WebRenderer.url;
		}

		// Token: 0x04000B61 RID: 2913
		private static WebRenderer instance;

		// Token: 0x04000B62 RID: 2914
		public static bool Enabled = true;

		// Token: 0x04000B63 RID: 2915
		public static Texture2D texture;

		// Token: 0x04000B64 RID: 2916
		private static byte[] texBuffer;

		// Token: 0x04000B65 RID: 2917
		public static XNAWebRenderer.TextureUpdatedDelegate textureUpdated = new XNAWebRenderer.TextureUpdatedDelegate(WebRenderer.TextureUpdated);

		// Token: 0x04000B66 RID: 2918
		private static GraphicsDevice graphics;

		// Token: 0x04000B67 RID: 2919
		private static int width = 500;

		// Token: 0x04000B68 RID: 2920
		private static int height = 500;

		// Token: 0x04000B69 RID: 2921
		private static string url = "http://www.google.com";

		// Token: 0x04000B6A RID: 2922
		private static bool loadingPage;
	}
}
