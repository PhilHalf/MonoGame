using System;
using System.Runtime.InteropServices;

#if MONOMAC
using MonoMac.OpenGL;
#else
using OpenTK.Graphics.ES20;
#endif

namespace Microsoft.Xna.Framework.Graphics
{
	public class TextureCube : Texture
	{
		int textureId;
		protected int size;
		
		PixelInternalFormat glInternalFormat;
		PixelFormat glFormat;
		PixelType glType;
		
		public TextureCube (GraphicsDevice graphicsDevice, int size, bool mipMap, SurfaceFormat format)
		{
			
			this.size = size;
			GL.GenTextures(1, out _textureId);
			GL.BindTexture (TextureTarget.TextureCubeMap, _textureId);
			

			format.GetGLFormat (out glInternalFormat, out glFormat, out glType);
			
			for (int i=0; i<6; i++) {
				TextureTarget target = GetGLCubeFace((CubeMapFace)i);

				if (glFormat == (PixelFormat)All.CompressedTextureFormats) {
					throw new NotImplementedException();
				} else {
					GL.TexImage2D (target, 0, glInternalFormat, size, size, 0, glFormat, glType, IntPtr.Zero);
				}
				
				if (mipMap)
				{
					GL.TexParameter(target, TextureParameterName.GenerateMipmap, (int)All.True);
				}
			}
			
		}
		
		public unsafe void SetData<T>(CubeMapFace face, int level, Rectangle? rect,
		                       T[] data, int startIndex, int elementCount) where T : struct
		{
            if (data == null) throw new ArgumentNullException("data");
			
			var elementSizeInByte = sizeof(T);
			var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
			var dataPtr = (IntPtr)(dataHandle.AddrOfPinnedObject().ToInt64() + startIndex * elementSizeInByte);
			
			var xOffset = 0;
			var yOffset = 0;
			var width = this.size;
			var height = this.size;
			
			if (rect.HasValue)
			{
				xOffset = rect.Value.X;
				yOffset = rect.Value.Y;
				width = rect.Value.Width;
				height = rect.Value.Height;
			}
			
			TextureTarget target = GetGLCubeFace(face);
			
			GL.BindTexture (target, _textureId);
			
			if (glFormat == (PixelFormat)All.CompressedTextureFormats) {
				throw new NotImplementedException();
			} else {
				GL.TexSubImage2D(target, level, xOffset, yOffset, width, height, glFormat, glType, data);
			}
			
			dataHandle.Free ();
		}
		
		private TextureTarget GetGLCubeFace(CubeMapFace face) {
			switch (face) {
			case CubeMapFace.PositiveX: return TextureTarget.TextureCubeMapPositiveX;
			case CubeMapFace.NegativeX: return TextureTarget.TextureCubeMapNegativeX;
			case CubeMapFace.PositiveY: return TextureTarget.TextureCubeMapPositiveY;
			case CubeMapFace.NegativeY: return TextureTarget.TextureCubeMapNegativeY;
			case CubeMapFace.PositiveZ: return TextureTarget.TextureCubeMapPositiveZ;
			case CubeMapFace.NegativeZ: return TextureTarget.TextureCubeMapNegativeZ;
			}
			throw new ArgumentException();
		}
		
	}
}

