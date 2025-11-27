using NLog;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Mushroom3d;

public class Texture
{
    private readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    private int? Id;
    private readonly string path;

    public Texture(string path)
    {
        this.path = path;
    }

    public int Get()
    {
        if (!Id.HasValue)
        {
            Id = CreateTexture();
        }
        return Id.Value;
    }

    public int CreateTexture()
    {
        int tex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, tex);

        using (var stream = File.OpenRead(path))
        {
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        LOGGER.Info($"Texture created: {path}");
        return tex;
    }
}