using BlazorCheckers.GameEngine;

namespace BlazorCheckers
{
    public class Renderer
    {
        public string GetCellHtmlClass(Cell cell, Cell? selectedCell, 
            List<Cell> highlightedCells, List<Cell> movableCells)
        {
            string result = cell.Color == CellColor.Light ? "cell light-cell" : "cell dark-cell";
            if (cell == selectedCell)
            {
                result += " selected-cell";
            }
            else if (highlightedCells.Contains(cell))
            {
                result += " highlighted-cell";
            }
            else if (movableCells.Contains(cell))
            {
                result += " movable-cell";
            }
            return result;
        }

        public string GetPieceHtmlClass(Piece piece)
        {
            string result = piece.Side == Side.Light ? "piece light-piece" : "piece dark-piece";
            if (piece.Rank == PieceRank.King)
            {
                result += " king";
            }
            return result;
        }
    }
}
