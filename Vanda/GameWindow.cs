using Mushroom3d;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Vanda.screen;

namespace Vanda;

public class GameWindow : UnsafeWindow, IRenderable
{
    private static readonly GameWindow _instance = new();
    public static GameWindow Instance => _instance;
    
    public static readonly double render_MS = 1000 / 60.0;
    public static readonly double update_MS = 1000 / 30.0;
    
    private Graphics _graphics;
    
    private Screen? screen;
    
    public int Width { get; private set; }
    public int Height { get; private set; }
    private GameWindow() : base(new NativeWindowSettings()
        {
            Title = "Game",
            Flags = ContextFlags.Default, 
            DepthBits = 24
        }, 
        new UnsafeWindowsSettings()
        {
            renderMS = render_MS,
            updateMS = update_MS,
        })
    {
        Resize += (e) =>
        {
            Width = e.Width;
            Height = e.Height;
            GL.Viewport(0, 0, e.Width, e.Height);
        };
        _graphics = new Graphics(this);
    }

    public override void OnLoad()
    {
        GL.Enable(EnableCap.DepthTest);
        
        OpenScreen(new InGameScreen(null));
        
        base.OnLoad();
    }

    public override void OnRenderFrame()
    {
        GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

        Render(_graphics);
        
        SwapBuffers();
        base.OnRenderFrame();
    }

    public override void OnUpdate(float deltaTime)
    {
        screen?.Update(this, deltaTime);
        base.OnUpdate(deltaTime);
    }

    public void OpenScreen(Screen screen)
    {
        this.screen?.Dispose();
        this.screen = screen;
        this.screen.Initialize(this);
    }

    public void Render(Graphics graphics)
    {
        screen?.Render(graphics);
    }
}