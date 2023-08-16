using Cloo;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("GPU TEST");

        ComputeContextPropertyList properties = new ComputeContextPropertyList(ComputePlatform.Platforms[0]);
        ComputeContext context = new ComputeContext(ComputeDeviceTypes.Gpu, properties, null, IntPtr.Zero);

        string kernelSource = @"
    __kernel void MultiplyByTwo(__global float* input, __global float* output)
    {
        int gid = get_global_id(0);
        output[gid] = input[gid] * 2;
    }
";

        ComputeProgram program = new ComputeProgram(context, kernelSource);
        program.Build(null, null, null, IntPtr.Zero);

        //TEST
        List<float> inputData = new List<float> { 1.0f, 2.0f, 3.0f, 4.0f };
        List<float> outputData = new List<float>(inputData.Count);

        Parallel.ForEach(inputData, (inputItem, state) =>
        {
            using (var commandQueue = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None))
            {
                ComputeBuffer<float> inputBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, new float[] { inputItem });
                ComputeBuffer<float> outputBuffer = new ComputeBuffer<float>(context, ComputeMemoryFlags.WriteOnly, 1);

                ComputeKernel kernel = program.CreateKernel("MultiplyByTwo");
                kernel.SetMemoryArgument(0, inputBuffer);
                kernel.SetMemoryArgument(1, outputBuffer);

                commandQueue.Execute(kernel, null, new long[] { 1 }, null, null);
                commandQueue.Finish();

                float[] result = new float[1];
                commandQueue.ReadFromBuffer(outputBuffer, ref result, true, null);

                outputData.Add(result[0]);
            }
        });


    }
}