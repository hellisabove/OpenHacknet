using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x020000AB RID: 171
	internal class Cube3D
	{
		// Token: 0x0600037E RID: 894 RVA: 0x00035434 File Offset: 0x00033634
		public static void Initilize(GraphicsDevice gd)
		{
			Cube3D.ConstructCube();
			Cube3D.vBuffer = new VertexBuffer(gd, VertexPositionNormalTexture.VertexDeclaration, 36, BufferUsage.WriteOnly);
			Cube3D.vBuffer.SetData<VertexPositionNormalTexture>(Cube3D.verts);
			gd.SetVertexBuffer(Cube3D.vBuffer);
			Cube3D.ib = new IndexBuffer(gd, IndexElementSize.SixteenBits, 14, BufferUsage.WriteOnly);
			Cube3D.ib.SetData<short>(new short[]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13
			});
			gd.Indices = Cube3D.ib;
			Cube3D.wireframeRaster = new RasterizerState();
			Cube3D.wireframeRaster.FillMode = FillMode.WireFrame;
			Cube3D.wireframeRaster.CullMode = CullMode.None;
			Cube3D.wireframeEfect = new BasicEffect(gd);
			Cube3D.wireframeEfect.Projection = Matrix.CreatePerspectiveFieldOfView(0.7853982f, Cube3D.wireframeEfect.GraphicsDevice.Viewport.AspectRatio, 0.01f, 3000f);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00035514 File Offset: 0x00033714
		private static void ResetBuffers()
		{
			GraphicsDevice graphicsDevice = Cube3D.wireframeEfect.GraphicsDevice;
			graphicsDevice.SetVertexBuffer(Cube3D.vBuffer);
			graphicsDevice.Indices = Cube3D.ib;
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00035545 File Offset: 0x00033745
		public static void RenderWireframe(Vector3 position, float scale, Vector3 rotation, Color color)
		{
			Cube3D.RenderWireframe(position, scale, rotation, color, new Vector3(0f, 0f, 20f));
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00035568 File Offset: 0x00033768
		public static void RenderWireframe(Vector3 position, float scale, Vector3 rotation, Color color, Vector3 cameraOffset)
		{
			scale = Math.Max(0.001f, scale);
			Cube3D.wireframeEfect.DiffuseColor = Utils.ColorToVec3(color);
			Cube3D.wireframeEfect.GraphicsDevice.BlendState = BlendState.Opaque;
			Cube3D.wireframeEfect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			Cube3D.wireframeEfect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			Cube3D.ResetBuffers();
			RasterizerState rasterizerState = Cube3D.wireframeEfect.GraphicsDevice.RasterizerState;
			Cube3D.wireframeEfect.GraphicsDevice.RasterizerState = Cube3D.wireframeRaster;
			Vector3 value = new Vector3(0f, 0f, 0f);
			Matrix world = Matrix.CreateTranslation(-value) * Matrix.CreateScale(scale) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateTranslation(position);
			Cube3D.wireframeEfect.World = world;
			Cube3D.wireframeEfect.View = Matrix.CreateLookAt(cameraOffset, position, Vector3.Up);
			try
			{
				foreach (EffectPass effectPass in Cube3D.wireframeEfect.CurrentTechnique.Passes)
				{
					effectPass.Apply();
					Cube3D.wireframeEfect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 36);
				}
			}
			catch (NotSupportedException)
			{
				Console.WriteLine("Not supported happened");
			}
			Cube3D.wireframeEfect.GraphicsDevice.RasterizerState = rasterizerState;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00035734 File Offset: 0x00033934
		private static void ConstructCube()
		{
			Cube3D.verts = new VertexPositionNormalTexture[36];
			Vector3 position = new Vector3(-1f, 1f, -1f);
			Vector3 position2 = new Vector3(-1f, 1f, 1f);
			Vector3 position3 = new Vector3(1f, 1f, -1f);
			Vector3 position4 = new Vector3(1f, 1f, 1f);
			Vector3 position5 = new Vector3(-1f, -1f, -1f);
			Vector3 position6 = new Vector3(-1f, -1f, 1f);
			Vector3 position7 = new Vector3(1f, -1f, -1f);
			Vector3 position8 = new Vector3(1f, -1f, 1f);
			Vector3 normal = new Vector3(0f, 0f, 1f);
			Vector3 normal2 = new Vector3(0f, 0f, -1f);
			Vector3 normal3 = new Vector3(0f, 1f, 0f);
			Vector3 normal4 = new Vector3(0f, -1f, 0f);
			Vector3 normal5 = new Vector3(-1f, 0f, 0f);
			Vector3 normal6 = new Vector3(1f, 0f, 0f);
			Vector2 textureCoordinate = new Vector2(1f, 0f);
			Vector2 textureCoordinate2 = new Vector2(0f, 0f);
			Vector2 textureCoordinate3 = new Vector2(1f, 1f);
			Vector2 textureCoordinate4 = new Vector2(0f, 1f);
			Cube3D.verts[0] = new VertexPositionNormalTexture(position, normal, textureCoordinate);
			Cube3D.verts[1] = new VertexPositionNormalTexture(position5, normal, textureCoordinate3);
			Cube3D.verts[2] = new VertexPositionNormalTexture(position3, normal, textureCoordinate2);
			Cube3D.verts[3] = new VertexPositionNormalTexture(position5, normal, textureCoordinate3);
			Cube3D.verts[4] = new VertexPositionNormalTexture(position7, normal, textureCoordinate4);
			Cube3D.verts[5] = new VertexPositionNormalTexture(position3, normal, textureCoordinate2);
			Cube3D.verts[6] = new VertexPositionNormalTexture(position2, normal2, textureCoordinate2);
			Cube3D.verts[7] = new VertexPositionNormalTexture(position4, normal2, textureCoordinate);
			Cube3D.verts[8] = new VertexPositionNormalTexture(position6, normal2, textureCoordinate4);
			Cube3D.verts[9] = new VertexPositionNormalTexture(position6, normal2, textureCoordinate4);
			Cube3D.verts[10] = new VertexPositionNormalTexture(position4, normal2, textureCoordinate);
			Cube3D.verts[11] = new VertexPositionNormalTexture(position8, normal2, textureCoordinate3);
			Cube3D.verts[12] = new VertexPositionNormalTexture(position, normal3, textureCoordinate3);
			Cube3D.verts[13] = new VertexPositionNormalTexture(position4, normal3, textureCoordinate2);
			Cube3D.verts[14] = new VertexPositionNormalTexture(position2, normal3, textureCoordinate);
			Cube3D.verts[15] = new VertexPositionNormalTexture(position, normal3, textureCoordinate3);
			Cube3D.verts[16] = new VertexPositionNormalTexture(position3, normal3, textureCoordinate4);
			Cube3D.verts[17] = new VertexPositionNormalTexture(position4, normal3, textureCoordinate2);
			Cube3D.verts[18] = new VertexPositionNormalTexture(position5, normal4, textureCoordinate);
			Cube3D.verts[19] = new VertexPositionNormalTexture(position6, normal4, textureCoordinate3);
			Cube3D.verts[20] = new VertexPositionNormalTexture(position8, normal4, textureCoordinate4);
			Cube3D.verts[21] = new VertexPositionNormalTexture(position5, normal4, textureCoordinate);
			Cube3D.verts[22] = new VertexPositionNormalTexture(position8, normal4, textureCoordinate4);
			Cube3D.verts[23] = new VertexPositionNormalTexture(position7, normal4, textureCoordinate2);
			Cube3D.verts[24] = new VertexPositionNormalTexture(position, normal5, textureCoordinate2);
			Cube3D.verts[25] = new VertexPositionNormalTexture(position6, normal5, textureCoordinate3);
			Cube3D.verts[26] = new VertexPositionNormalTexture(position5, normal5, textureCoordinate4);
			Cube3D.verts[27] = new VertexPositionNormalTexture(position2, normal5, textureCoordinate);
			Cube3D.verts[28] = new VertexPositionNormalTexture(position6, normal5, textureCoordinate3);
			Cube3D.verts[29] = new VertexPositionNormalTexture(position, normal5, textureCoordinate2);
			Cube3D.verts[30] = new VertexPositionNormalTexture(position3, normal6, textureCoordinate);
			Cube3D.verts[31] = new VertexPositionNormalTexture(position7, normal6, textureCoordinate3);
			Cube3D.verts[32] = new VertexPositionNormalTexture(position8, normal6, textureCoordinate4);
			Cube3D.verts[33] = new VertexPositionNormalTexture(position4, normal6, textureCoordinate2);
			Cube3D.verts[34] = new VertexPositionNormalTexture(position3, normal6, textureCoordinate);
			Cube3D.verts[35] = new VertexPositionNormalTexture(position8, normal6, textureCoordinate4);
		}

		// Token: 0x040003FB RID: 1019
		private const int NUM_VERTICES = 36;

		// Token: 0x040003FC RID: 1020
		private const int NUM_INDICIES = 14;

		// Token: 0x040003FD RID: 1021
		private static VertexPositionNormalTexture[] verts;

		// Token: 0x040003FE RID: 1022
		private static VertexBuffer vBuffer;

		// Token: 0x040003FF RID: 1023
		private static IndexBuffer ib;

		// Token: 0x04000400 RID: 1024
		private static BasicEffect wireframeEfect;

		// Token: 0x04000401 RID: 1025
		private static RasterizerState wireframeRaster;
	}
}
