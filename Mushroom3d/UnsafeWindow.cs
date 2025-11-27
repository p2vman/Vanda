using System.Diagnostics;
using OpenTK.Core;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Mushroom3d;

public class UnsafeWindow : NativeWindow
{
    public class UnsafeWindowsSettings
    {
        public double renderMS { get; set; }
        public double updateMS { get; set; }
    }
    
    public event Action Load;
    public event Action Unload;
    public event Action<float> Update;
    public event Action RenderFrame;
    
    private readonly UnsafeWindowsSettings _settings;
    
    
    public UnsafeWindow(NativeWindowSettings settings, UnsafeWindowsSettings unsafeWindowsSettings) : base(settings)
    {
        _settings = unsafeWindowsSettings;
    }
    
    private static readonly Stopwatch frameWatch = new Stopwatch();
    private static readonly Stopwatch watch60 = new Stopwatch();
    private static readonly Stopwatch watch30 = new Stopwatch();

    public virtual void OnLoad()
    {
        Load?.Invoke();
    }

    public virtual void OnUnload()
    {
        Unload?.Invoke();
    }

    public virtual void OnUpdate(float deltaTime)
    {
        Update?.Invoke(deltaTime);
    }

    public virtual void OnRenderFrame()
    {
        RenderFrame?.Invoke();
    }

    public virtual unsafe void Run()
    {
        Context?.MakeCurrent();
        OnLoad();
        OnResize(new ResizeEventArgs(ClientSize));

        Debug.Print("Entering main loop.");
        frameWatch.Start();
        watch60.Start();
        watch30.Start();
        

        while (!GLFW.WindowShouldClose(WindowPtr))
        {
            frameWatch.Restart();

            if (watch60.Elapsed.TotalMilliseconds >= _settings.renderMS)
            {
                OnRenderFrame();
                watch60.Restart();
            }

            if (watch30.Elapsed.TotalMilliseconds >= _settings.updateMS)
            {
                NewInputFrame();
                ProcessWindowEvents(IsEventDriven);
                OnUpdate((float)watch30.Elapsed.TotalMilliseconds);
                watch30.Restart();
            }

            var elapsed = frameWatch.Elapsed.TotalMilliseconds;
            var sleep = _settings.updateMS - elapsed;
            if (sleep > 0)
                Utils.AccurateSleep(sleep / 1000.0, 1);
        }

        OnUnload();
    }

    protected virtual void SwapBuffers()
    {
        if (Context == null)
        {
            throw new InvalidOperationException("Cannot use SwapBuffers when running with ContextAPI.NoAPI.");
        }

        Context.SwapBuffers();
    }
}