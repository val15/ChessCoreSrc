using OpenCL.Net;

namespace ChessCore.Tools.ChessEngine.Engine
{



  public class OpenCLHelper
  {
    public static Device GetFirstGPUDevice()
    {
      Platform[] platforms = Cl.GetPlatformIDs(out ErrorCode error);
      if (error != ErrorCode.Success)
        throw new Exception("No OpenCL platform found.");

      foreach (var platform in platforms)
      {
        Device[] devices = Cl.GetDeviceIDs(platform, DeviceType.Gpu, out error);
        if (devices.Length > 0) return devices[0];
      }
      throw new Exception("No OpenCL GPU found.");
    }

    public static Context CreateContext(Device device)
    {
      return Cl.CreateContext(null, 1, new[] { device }, null, IntPtr.Zero, out ErrorCode error);
    }

    public static CommandQueue CreateQueue(Context context, Device device)
    {
      return Cl.CreateCommandQueue(context, device, CommandQueueProperties.None, out ErrorCode error);
    }
  }


}
