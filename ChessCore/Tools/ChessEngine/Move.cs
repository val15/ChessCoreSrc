using ChessCore.Tools.ChessEngine;

public class Move
{
    public Move() { }

    public Move(int fromIndex, int toIndex, BoardCE board)
    {
        FromIndex = fromIndex;
        ToIndex = toIndex;

        // Valeur de la pièce qui se déplace
        PieceValue = board.GetPieceValue(board._cases[fromIndex]);

        // Valeur de la pièce capturée
        CapturedPieceValue = board.GetPieceValue(board._cases[toIndex]);

        // Détermine si c'est une capture
        IsCapture = CapturedPieceValue > 0 && board._cases[toIndex] != "__";
    }

    public int FromIndex { get; set; }
    public int ToIndex { get; set; }

    // Valeur de la pièce qui effectue le mouvement
    public int PieceValue { get; set; }

    // True si le mouvement capture une pièce adverse
    public bool IsCapture { get; set; }

    // Valeur de la pièce capturée (0 si pas de capture)
    public int CapturedPieceValue { get; set; }

    // Pour MVV-LVA (Most Valuable Victim - Least Valuable Aggressor)
    public int MvvLvaScore => CapturedPieceValue * 100 - PieceValue;

    public override bool Equals(object obj) =>
        obj is Move m && m.FromIndex == FromIndex && m.ToIndex == ToIndex;

    public override int GetHashCode() => HashCode.Combine(FromIndex, ToIndex);
}