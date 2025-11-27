namespace Mushroom3d;

public struct AtlasTexture
{
    public uint Id;
    public uint OffsetX;
    public uint OffsetY;
    public uint Width;
    public uint Height;

    public AtlasTexture(uint offsetX, uint offsetY, uint width, uint height, uint id)
    {
        OffsetX = offsetX;
        OffsetY = offsetY;
        Width = width;
        Height = height;
        Id = id;
    }
}