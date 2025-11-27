using Mushroom3d;

namespace Vanda.screen;

public class Screen : IRenderable, IDisposable
{
    public Screen? parent {get; set;}
    
    public Screen(Screen? parent)
    {
        this.parent = parent;
    }


    public virtual void Render(Graphics graphics)
    {
        
    }

    public virtual void Update(UnsafeWindow window, double deltaTime)
    {
        
    }

    public virtual void Initialize(UnsafeWindow window)
    {
        
    }

    public virtual void Dispose()
    {
        
    }
}