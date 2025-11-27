namespace Mushroom3d;

public class Graphics
{
    public UnsafeWindow unsafeWindow { get; private set; }

    public Graphics(UnsafeWindow unsafeWindow)
    {
        this.unsafeWindow = unsafeWindow;
    }
}