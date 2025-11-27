using NLog;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Mushroom3d;


public class TextureAtlas : Texture
{
    private readonly List<ImageResult> _images = [];
    public readonly List<AtlasTexture> Textures = [];

    public int Width;
    public int Height;

    private const int Padding = 2;

    public uint AddTexture(ImageResult img)
    {
        _images.Add(img);
        return (uint)_images.Count - 1;
    }

    public override int CreateTexture()
    {
        var x = 0;
        var y = 0;
        var rowH = 0;

        foreach (var img in _images)
        {
            if (x + img.Width > 2048)
            {
                x = 0;
                y += rowH + Padding;
                rowH = 0;
            }

            Textures.Add(new AtlasTexture((uint)x, (uint)y,
                (uint)img.Width, (uint)img.Height,
                (uint)Textures.Count));

            x += img.Width + Padding;
            rowH = Math.Max(rowH, img.Height);

            Width = Math.Max(Width, x);
            Height = Math.Max(Height, y + rowH);
        }

        var tex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, tex);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
            Width, Height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

        foreach (var t in Textures)
        {
            var img = _images[(int)t.Id];
            unsafe
            {
                fixed (byte* ptr = img.Data)
                {
                    GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                        (int)t.OffsetX, (int)t.OffsetY,
                        (int)t.Width, (int)t.Height,
                        PixelFormat.Rgba, PixelType.UnsignedByte,
                        (IntPtr)ptr);
                }
            }
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        return tex;
    }
}