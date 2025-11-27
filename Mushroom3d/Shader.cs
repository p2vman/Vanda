using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using NLog;

namespace Mushroom3d;

public class Shader : IDisposable
{
    private readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
    public int Handle;

    public Shader(string vertex, string fragment)
    {
        int v = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(v, vertex);
        GL.CompileShader(v);

        int success;
        GL.GetShader(v, ShaderParameter.CompileStatus, out success);
        if (success == 0)
        {
            string info = GL.GetShaderInfoLog(v);
            LOGGER.Warn($"ERROR: VERTEX SHADER COMPILATION FAILED\n{info}");
        }

        int f = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(f, fragment);
        GL.CompileShader(f);
        
        GL.GetShader(f, ShaderParameter.CompileStatus, out success);
        if (success == 0)
        {
            string info = GL.GetShaderInfoLog(f);
            LOGGER.Warn($"ERROR: FRAGMENT SHADER COMPILATION FAILED\n{info}");
        }

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, v);
        GL.AttachShader(Handle, f);
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
        if (success == 0)
        {
            string info = GL.GetProgramInfoLog(Handle);
            LOGGER.Warn($"ERROR: SHADER PROGRAM LINKING FAILED\n{info}");
        }

        GL.DetachShader(Handle, v);
        GL.DetachShader(Handle, f);
        GL.DeleteShader(v);
        GL.DeleteShader(f);
    }

    public void Use() => GL.UseProgram(Handle);

    public void SetMatrix4(string name, Matrix4 mat)
    {
        GL.UniformMatrix4(GL.GetUniformLocation(Handle, name), false, ref mat);
    }

    public void SetVector3(string name, Vector3 vec)
    {
        GL.Uniform3(GL.GetUniformLocation(Handle, name), ref vec);
    }

    public int GetUniformLocation(string name)
    {
        return GL.GetUniformLocation(Handle, name);
    }

    public void SetQuaternion(string name, Quaternion quaternion)
    {
        GL.Uniform4(GL.GetUniformLocation(Handle, name), quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
        Handle = 0;
    }
}
