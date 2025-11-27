using NLog;
using OpenTK.Graphics.OpenGL4;
namespace Mushroom3d;

public abstract class Texture
{
    private readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    private int? Id;

    public Texture()
    {
    }

    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, Get());
    }

    public int Get()
    {
        if (!Id.HasValue)
        {
            Id = CreateTexture();
        }
        return Id.Value;
    }

    public abstract int CreateTexture();
}