using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000170 RID: 368
	public static class TextureBank
	{
		// Token: 0x06000937 RID: 2359 RVA: 0x00098190 File Offset: 0x00096390
		public static Texture2D load(string filename, ContentManager content)
		{
			Texture2D result;
			for (int i = 0; i < TextureBank.textures.Count; i++)
			{
				if (TextureBank.textures[i].path.Equals(filename))
				{
					if (!TextureBank.textures[i].tex.IsDisposed)
					{
						LoadedTexture value = TextureBank.textures[i];
						value.retainCount++;
						TextureBank.textures[i] = value;
						result = value.tex;
						return result;
					}
					TextureBank.textures.Remove(TextureBank.textures[i]);
				}
			}
			try
			{
				Texture2D texture2D = content.Load<Texture2D>(filename);
				LoadedTexture item = default(LoadedTexture);
				item.tex = texture2D;
				item.path = filename;
				item.retainCount = 1;
				TextureBank.textures.Add(item);
				result = texture2D;
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Concat(new object[]
				{
					"File \"",
					filename,
					"\" Experienced an Error in Loading\n",
					ex
				}));
				result = null;
			}
			return result;
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x000982E4 File Offset: 0x000964E4
		public static Texture2D getIfLoaded(string filename)
		{
			foreach (LoadedTexture item in TextureBank.textures)
			{
				if (item.path.Equals(filename))
				{
					if (!item.tex.IsDisposed)
					{
						return item.tex;
					}
					TextureBank.textures.Remove(item);
				}
			}
			return null;
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x00098380 File Offset: 0x00096580
		public static void unload(Texture2D tex)
		{
			TextureBank.unloadWithoutRemoval(tex);
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x00098398 File Offset: 0x00096598
		public static void unloadWithoutRemoval(Texture2D tex)
		{
			for (int i = 0; i < TextureBank.textures.Count; i++)
			{
				if (TextureBank.textures[i].tex.Equals(tex))
				{
					if (!TextureBank.textures[i].tex.IsDisposed)
					{
						LoadedTexture value = TextureBank.textures[i];
						value.retainCount--;
						TextureBank.textures[i] = value;
						break;
					}
					TextureBank.textures.Remove(TextureBank.textures[i]);
				}
			}
		}

		// Token: 0x04000AC2 RID: 2754
		public static List<LoadedTexture> textures = new List<LoadedTexture>();
	}
}
