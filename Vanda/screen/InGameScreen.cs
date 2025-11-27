using Mushroom3d;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Vanda.screen;


public class InGameScreen : Screen
{
    public Texture texture = new Texture("./madaotheory-Anime-Artist-artist-8588752.png");
    public Shader shader;
    public VertexBuffer buffer = new VertexBuffer();
    public Camera camera = new Camera();
    
    private float _pitch = 0;
    private float _yaw = 0;
    private bool _firstMouse = true;
    private float _speed = 12.5f;
    private float _sensitivity = 0.1f;
    private Vector2 _lastMousePos;

    public InGameScreen(Screen? parent) : base(parent)
    {

    }
    

    public override void Update(UnsafeWindow window, double deltaTime)
    {
        if (window.KeyboardState.IsKeyPressed(Keys.Escape))
        {
            window.CursorState = CursorState.Normal;
            _firstMouse = true;
        }
        if (window.MouseState.IsButtonPressed(MouseButton.Left))
        {
            window.CursorState = CursorState.Grabbed;
            _firstMouse = true;
        }

        if (window.CursorState != CursorState.Grabbed)
            return;
        
        var mouse = window.MousePosition;
        if (_firstMouse)
        {
            _lastMousePos = mouse;
            _firstMouse = false;
        }
        
        var xOffset = mouse.X - _lastMousePos.X;
        var yOffset = _lastMousePos.Y - mouse.Y;
        _lastMousePos = mouse;

        xOffset *= _sensitivity;
        yOffset *= _sensitivity;

        _yaw += xOffset;
        _pitch += yOffset;
        _pitch = Math.Clamp(_pitch, -89f, 89f);

        Vector3 front;
        front.X = MathF.Cos(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(_yaw)) * MathF.Cos(MathHelper.DegreesToRadians(_pitch));
        camera.CameraFront = Vector3.Normalize(front);

        var velocity = _speed * (float)deltaTime;
        if (window.KeyboardState.IsKeyDown(Keys.W)) camera.Position += camera.CameraFront * velocity;
        if (window.KeyboardState.IsKeyDown(Keys.S)) camera.Position -= camera.CameraFront * velocity;
        if (window.KeyboardState.IsKeyDown(Keys.A)) camera.Position -= Vector3.Normalize(Vector3.Cross(camera.CameraFront, camera.CameraUp)) * velocity;
        if (window.KeyboardState.IsKeyDown(Keys.D)) camera.Position += Vector3.Normalize(Vector3.Cross(camera.CameraFront, camera.CameraUp)) * velocity;
        
        if (window.KeyboardState.IsKeyDown(Keys.Space)) camera.Position += new Vector3(0, 1, 0) * velocity / 2;
        if (window.KeyboardState.IsKeyDown(Keys.LeftShift)) camera.Position += new Vector3(0, -1, 0) * velocity / 2;
        
        
        camera.UpdateCameraVectors(_yaw, _pitch);
    }



    public override void Initialize(UnsafeWindow window)
    {
        shader = new Shader(Resources.VertexShader, Resources.FragmentShader);
        base.Initialize(window);
    }
    
    

    public override void Render(Graphics graphics)
    {
        GL.Enable(EnableCap.DepthTest);
        shader.Use();
        
        var view = Matrix4.LookAt(camera.Position, camera.Position + camera.CameraFront, camera.CameraUp);
        var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), graphics.unsafeWindow.Size.X / (float)graphics.unsafeWindow.Size.Y , 0.1f, 100f);
        
        shader.SetMatrix4("view", view);
        shader.SetMatrix4("projection", proj);
        shader.SetMatrix4("model", Matrix4.CreateTranslation(0,0,0));
        shader.SetVector3("world_position", new Vector3());

        buffer.VertexUv(new VertexUv { X = 0, Y = 0, Z = 0, U = 0, V = 0 });
        buffer.VertexUv(new VertexUv { X = 1, Y = 0, Z = 0, U = 1, V = 0 });
        buffer.VertexUv(new VertexUv { X = 1, Y = 1, Z = 0, U = 1, V = 1 });
        buffer.VertexUv(new VertexUv { X = 0, Y = 1, Z = 0, U = 0, V = 1 });

        
        buffer.Index([0, 1, 2, 2, 3, 0]);
        
        var (v,i) = buffer.Flush();
        
        
        GL.BindVertexArray(buffer._vao);
        GL.BindTexture(TextureTarget.Texture2D, texture.Get());
        GL.DrawElements(PrimitiveType.Triangles, i, DrawElementsType.UnsignedInt, 0);
        
        buffer.VertexUv(new VertexUv { X = 0, Y = 0, Z = 0, U = 0, V = 0 });
        buffer.VertexUv(new VertexUv { X = -1, Y = 0, Z = 0, U = 1, V = 0 });
        buffer.VertexUv(new VertexUv { X = -1, Y = -1, Z = 0, U = 1, V = 1 });
        buffer.VertexUv(new VertexUv { X = 0, Y = -1, Z = 0, U = 0, V = 1 });

        
        buffer.Index([0, 1, 2, 2, 3, 0]);
        
        (v,i) = buffer.Flush();
        
        
        GL.BindVertexArray(buffer._vao);
        GL.BindTexture(TextureTarget.Texture2D, texture.Get());
        GL.DrawElements(PrimitiveType.Triangles, i, DrawElementsType.UnsignedInt, 0);
        
        base.Render(graphics);
    }
}