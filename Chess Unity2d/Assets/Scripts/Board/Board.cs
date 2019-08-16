using UnityEngine;
using UnityEngine.UI;

// New

public class Board : MonoBehaviour
{
    // ReSharper disable once InconsistentNaming
    [HideInInspector] public Cell[,] AllCells = new Cell[8, 8];

    // ReSharper disable once InconsistentNaming
    public GameObject CellPrefab;

    public void Create()
    {
        for (var y = 0; y < 8; y++)
        for (var x = 0; x < 8; x++)
        {
            var newCell = Instantiate(CellPrefab, transform);

            var rectTransform = newCell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(x * 100 + 50, y * 100 + 50);

            AllCells[x, y] = newCell.GetComponent<Cell>();
            AllCells[x, y].Setup(new Vector2Int(x, y), this);
        }

        for (var x = 0; x < 8; x += 2)
        for (var y = 0; y < 8; y++)
        {
            var offset = y % 2 != 0 ? 0 : 1;
            var finalX = x + offset;

            //AllCells[finalX , y ].GetComponent<Image>().color = new Color32(230, 220, 187, 255);
            AllCells[finalX, y].GetComponent<Image>().color = Colors.BoardDarkSquaresColor;
        }
    }

    public CellState ValidateCell(int targetX, int targetY, BasePiece checkingPiece)
    {
        if (targetX < 0 || targetX > 7)
            return CellState.OutOfBounds;
        if (targetY < 0 || targetY > 7)
            return CellState.OutOfBounds;

        var targetCell = AllCells[targetX, targetY];

        if (targetCell.currentPiece == null)
            return CellState.Free;

        if (checkingPiece.mainColor == targetCell.currentPiece.mainColor)
            return CellState.Friendly;

        return checkingPiece.mainColor != targetCell.currentPiece.mainColor ? CellState.Enemy : CellState.Free;
    }
}