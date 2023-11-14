using System.Runtime.InteropServices;

namespace LcsServer.Models.LCProjectModels.GlobalBase;

public class IntPointer : IDisposable
{
    GCHandle pinnedArray;
    internal IntPointer(object obj)
    {
        pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned);
    }

    public static implicit operator IntPtr(IntPointer ipc)
    {
        return ipc.pinnedArray.AddrOfPinnedObject();
    }

    public void Dispose()
    {
        pinnedArray.Free();
    }
}