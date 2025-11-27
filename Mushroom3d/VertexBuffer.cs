using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Mushroom3d;

[StructLayout(LayoutKind.Sequential)]
public struct VertexUv(float x, float y, float z, float u, float v)
{
    public float X = x;
    public float Y = y;
    public float Z = z;
    public float U = u;
    public float V = v;
}

public class VertexBuffer : IDisposable
{
    public int _vao {get; set;}
    public int _vbo {get; set;}
    public int _ebo {get; set;}
    
    public List<VertexUv> vertices {get; private set;}
    public List<uint> indices {get; private set;}
    
    public VertexBuffer()
    {
        vertices = [];
        indices = [];
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();
        
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        var stride = 5 * sizeof(float);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
    }

    public void VertexUv(VertexUv uv)
    {
        vertices.Add(uv);
    }

    public void Index(uint[] indexes)
    {
        indices.AddRange(indexes);
    }

    public (int, int) Flush()
    {
        GL.BindVertexArray(_vao);
        
        var verticesSpan = MemoryMarshal.Cast<VertexUv, float>(CollectionsMarshal.AsSpan(vertices));
        var indicesSpan = CollectionsMarshal.AsSpan(indices);
        
        
        unsafe
        {
            fixed (float* vPtr = verticesSpan)
            fixed (uint* iPtr = indicesSpan)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, 
                    verticesSpan.Length * sizeof(float), 
                    (IntPtr)vPtr, 
                    BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, 
                    indicesSpan.Length * sizeof(uint), 
                    (IntPtr)iPtr, 
                    BufferUsageHint.StaticDraw);
            }
        }
        Clear();
        return (verticesSpan.Length, indicesSpan.Length);
    }

    public void Clear()
    {
        vertices.Clear();
        indices.Clear();
    }

    public void Dispose()
    {
       Clear();
       
    }
}