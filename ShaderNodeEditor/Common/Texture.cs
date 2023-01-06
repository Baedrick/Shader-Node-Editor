using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace ShaderNodeEditor.Common
{
    public class Texture
    {
        private readonly int handle;

        private Texture(int handle)
        {
            this.handle = handle;
        }

        public static Texture LoadFromFile(string filePath)
        {
            var handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);
            StbImage.stbi_set_flip_vertically_on_load(1);

            var stream = File.OpenRead(filePath);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                PixelInternalFormat.Rgba,
                image.Width,
                image.Height, 
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                image.Data
            );
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            return new Texture(handle);
        }

        private void Use()
        {
            return;
        }
    }
}