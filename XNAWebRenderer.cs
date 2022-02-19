using System;
using System.Runtime.InteropServices;

// Token: 0x0200018E RID: 398
public static class XNAWebRenderer
{
	// Token: 0x06000A17 RID: 2583
	[DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void XNAWR_Initialize([MarshalAs(UnmanagedType.LPStr)] string initialURL, XNAWebRenderer.TextureUpdatedDelegate callback, int width, int height);

	// Token: 0x06000A18 RID: 2584
	[DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void XNAWR_Shutdown();

	// Token: 0x06000A19 RID: 2585
	[DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void XNAWR_Update();

	// Token: 0x06000A1A RID: 2586
	[DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void XNAWR_LoadURL([MarshalAs(UnmanagedType.LPStr)] string URL);

	// Token: 0x06000A1B RID: 2587
	[DllImport("XNAWebRenderer.dll", CallingConvention = CallingConvention.Cdecl)]
	public static extern void XNAWR_SetViewport(int width, int height);

	// Token: 0x04000B6B RID: 2923
	private const string nativeLibName = "XNAWebRenderer.dll";

	// Token: 0x0200018F RID: 399
	// (Invoke) Token: 0x06000A1D RID: 2589
	public delegate void TextureUpdatedDelegate(IntPtr buffer);
}
