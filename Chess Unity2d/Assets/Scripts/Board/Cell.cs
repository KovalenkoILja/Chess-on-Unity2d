using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [HideInInspector] public Board board;

    [HideInInspector] public Vector2Int boardPosition = Vector2Int.zero;

    [HideInInspector] public BasePiece currentPiece;

    public Image outlineImage;

    [HideInInspector] public RectTransform rectTransform;

    public void Setup(Vector2Int newBoardPosition, Board newBoard)
    {
        boardPosition = newBoardPosition;
        board = newBoard;

        rectTransform = GetComponent<RectTransform>();
    }
}