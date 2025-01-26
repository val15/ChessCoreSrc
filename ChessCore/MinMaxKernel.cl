__kernel void MinMaxKernel(__global int* board, __global int* results, int depth, int alpha, int beta, int maximizing)
{
    int index = get_global_id(0);
    int score = board[index] + depth; // Exemple simplifié
    results[index] = score;
}
