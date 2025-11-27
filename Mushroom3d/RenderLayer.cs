namespace Mushroom3d;

public class RenderLayer
{
    public VertexBuffer buffer {get; private set;}
    public RenderLayer()
    {
        buffer = new VertexBuffer();
    }
}