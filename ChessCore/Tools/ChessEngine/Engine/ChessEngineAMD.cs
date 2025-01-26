using OpenCL.Net;
using System;
using System.Linq;
namespace ChessCore.Tools.ChessEngine.Engine
{
  public class ChessEngineAMD
  {
    private Device _gpuDevice;
    private Context _context;
    private CommandQueue _queue;
    private Program _program;
    private Kernel _kernel;

    public ChessEngineAMD()
    {
      _gpuDevice = OpenCLHelper.GetFirstGPUDevice();
      _context = OpenCLHelper.CreateContext(_gpuDevice);
      _queue = OpenCLHelper.CreateQueue(_context, _gpuDevice);

      string kernelSource = System.IO.File.ReadAllText("MinMaxKernel.cl");
      OpenCL.Net.Program _program = Cl.CreateProgramWithSource(_context, 1, new[] { kernelSource }, null, out _);
      Cl.BuildProgram(_program, 1, new[] { _gpuDevice }, string.Empty, null, IntPtr.Zero);
      _kernel = Cl.CreateKernel(_program, "MinMaxKernel", out _);
    }

    public int[] RunMinMaxOnGPU(int[] board)
    {
      int boardSize = board.Length;
      int[] results = new int[boardSize];

      Mem boardBuffer = (Mem)Cl.CreateBuffer(_context, MemFlags.ReadOnly | MemFlags.CopyHostPtr,
          boardSize * sizeof(int), board, out _);
      Mem resultBuffer = (Mem)Cl.CreateBuffer(_context, MemFlags.WriteOnly,
          boardSize * sizeof(int), IntPtr.Zero, out _);

      Cl.SetKernelArg(_kernel, 0, boardBuffer);
      Cl.SetKernelArg(_kernel, 1, resultBuffer);
      Cl.SetKernelArg(_kernel, 2, 3);
      Cl.SetKernelArg(_kernel, 3, -1000);
      Cl.SetKernelArg(_kernel, 4, 1000);
      Cl.SetKernelArg(_kernel, 5, 1);

      Cl.EnqueueNDRangeKernel(_queue, _kernel, 1, null, new IntPtr[] { new IntPtr(boardSize) }, null, 0, null, out _);
      Cl.Finish(_queue);

      Cl.EnqueueReadBuffer(_queue, resultBuffer, Bool.True, IntPtr.Zero, new IntPtr(boardSize * sizeof(int)), results, 0, null, out _);

      return results;
    }
  }
}
