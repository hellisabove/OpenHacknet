using System;
using Microsoft.Xna.Framework.Input;
using SDL2;

namespace Hacknet.Input
{
	// Token: 0x02000002 RID: 2
	public class TextInputHook : IDisposable
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public string Buffer
		{
			get
			{
				return this.buffer;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		public bool BackSpace
		{
			get
			{
				bool result = this.backSpace;
				this.backSpace = false;
				return result;
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002089 File Offset: 0x00000289
		public void clearBuffer()
		{
			this.buffer = "";
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002097 File Offset: 0x00000297
		public TextInputHook(IntPtr whnd)
		{
			TextInputEXT.TextInput += this.OnTextInput;
			SDL.SDL_StartTextInput();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020CC File Offset: 0x000002CC
		public void Dispose()
		{
			SDL.SDL_StopTextInput();
			TextInputEXT.TextInput -= this.OnTextInput;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020E8 File Offset: 0x000002E8
		private void OnTextInput(char c)
		{
			if (c == '\b')
			{
				this.backSpace = true;
			}
			else if (c == '\u0016')
			{
				this.buffer += SDL.SDL_GetClipboardText();
			}
			else
			{
				this.buffer += c;
			}
		}

		// Token: 0x04000001 RID: 1
		private string buffer = "";

		// Token: 0x04000002 RID: 2
		private bool backSpace = false;
	}
}
