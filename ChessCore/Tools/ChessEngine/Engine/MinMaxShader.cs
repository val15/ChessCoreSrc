using ComputeSharp;

namespace ChessCore.Tools.ChessEngine.Engine
{


  [ThreadGroupSize(1, 1, 1)]
  public readonly struct MinMaxShader : IComputeShader
  {
    private readonly ReadWriteBuffer<int> buffer;
    private readonly int alpha;
    private readonly int beta;
    private readonly int depth;
    private readonly bool maximizingPlayer;

    public MinMaxShader(
        ReadWriteBuffer<int> buffer,
        int alpha, int beta, int depth,
        bool maximizingPlayer)
    {
      this.buffer = buffer;
      this.alpha = alpha;
      this.beta = beta;
      this.depth = depth;
      this.maximizingPlayer = maximizingPlayer;
    }

    public void Execute()
    {
      int index = ThreadIds.X;
      buffer[index] = alpha + beta + depth + 0; // Exemple de calcul
    }
  }

}
