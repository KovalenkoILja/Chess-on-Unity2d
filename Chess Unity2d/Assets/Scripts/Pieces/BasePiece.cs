using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePiece : EventTrigger
{
    #region VirtualMethods

    protected virtual void Move()
    {
        /*if (!PieceManager.IsValidMove(CurrentCell, TargetCell))
        {
            SoundEvents.PlayRejectSound();
            StaticEvents.LogMsgEvent("Недопустимый ход!", LogType.Warning);
            Place(CurrentCell);
            return;
        }*/
        //SoundEvents.PlayMoveSound();

        PieceManager.PieceMoveEvent(this, CurrentCell, TargetCell);
        //SendMoveMsg(this, CurrentCell, TargetCell);
        //SendMove(this, CurrentCell, TargetCell);

        /*TargetCell.RemovePiece();

        CurrentCell.mCurrentPiece = null;

        CurrentCell = TargetCell;
        CurrentCell.mCurrentPiece = this;

        transform.position = CurrentCell.transform.position;
        TargetCell = null;*/

        //PieceManager.SwitchSides(mainColor);
    }

    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        PieceManager = newPieceManager;
        mainColor = newTeamColor;
        GetComponent<Image>().color = newSpriteColor;
        RectTransform = GetComponent<RectTransform>();
    }

    public virtual void Place(Cell newCell)
    {
        CurrentCell = newCell;
        OriginalCell = newCell;
        CurrentCell.currentPiece = this;

        transform.position = newCell.transform.position;
        gameObject.SetActive(true);
    }

    public virtual void Kill()
    {
        CurrentCell.currentPiece = null;
        SoundEvents.PlayCaptureSound();
        gameObject.SetActive(false);
    }

    protected virtual void CheckPathing()
    {
        CreateCellPath(1, 0, Movement.x);
        CreateCellPath(-1, 0, Movement.x);

        CreateCellPath(0, 1, Movement.y);
        CreateCellPath(0, -1, Movement.y);

        CreateCellPath(1, 1, Movement.z);
        CreateCellPath(-1, 1, Movement.z);

        CreateCellPath(-1, -1, Movement.z);
        CreateCellPath(1, -1, Movement.z);
    }

    #endregion

    #region Methods

    public void Reset()
    {
        Kill();
        Place(OriginalCell);
    }

    private void CreateCellPath(int xDirection, int yDirection, int movement)
    {
        var currentX = CurrentCell.boardPosition.x;
        var currentY = CurrentCell.boardPosition.y;

        for (var i = 1; i <= movement; i++)
        {
            currentX += xDirection;
            currentY += yDirection;

            var cellState = CurrentCell.board.ValidateCell(currentX, currentY, this);

            if (cellState == CellState.Enemy)
            {
                HighlightedCells.Add(CurrentCell.board.AllCells[currentX, currentY]);
                break;
            }

            if (cellState != CellState.Free) break;

            HighlightedCells.Add(CurrentCell.board.AllCells[currentX, currentY]);
        }
    }

    private void ShowCells()
    {
        foreach (var cell in HighlightedCells)
            cell.outlineImage.enabled = true;
    }

    private void ClearCells()
    {
        foreach (var cell in HighlightedCells) cell.outlineImage.enabled = false;
        HighlightedCells.Clear();
    }

    public void MovePieceToCell(Cell to)
    {
        TargetCell = to;
        if (!TargetCell)
        {
            transform.position = CurrentCell.gameObject.transform.position;
            return;
        }

        SoundEvents.PlayMoveSound();

        //PieceManager.PieceMoveEvent(this, CurrentCell, TargetCell);
        //SendMoveMsg(this, CurrentCell, TargetCell);
        //SendMove(this, CurrentCell, TargetCell);

        //TargetCell.RemovePiece();

        CurrentCell.currentPiece = null;

        CurrentCell = TargetCell;
        CurrentCell.currentPiece = this;

        transform.position = CurrentCell.transform.position;
        TargetCell = null;

        PieceManager.SwitchSides();
    }

    #endregion

    #region Variables

    private readonly List<Cell> HighlightedCells = new List<Cell>();
    private Cell CurrentCell;

    [HideInInspector] public Color mainColor;

    protected Vector3Int Movement = Vector3Int.one;

    private Cell OriginalCell;
    private PieceManager PieceManager;

    private RectTransform RectTransform;

    private Cell TargetCell;

    #endregion

    #region OverrideMethods

    public override void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag called.");
        //Debug.Log(Movement);
        base.OnBeginDrag(eventData);

        var moves = PieceManager.GetValidMoves(CurrentCell);
        HighlightedCells.Clear();

        foreach (var move in moves)
            //Debug.Log(move.boardPosition.x);
            //Debug.Log(move.boardPosition.y);

            HighlightedCells.Add(move);

        ShowCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag called.");
        base.OnDrag(eventData);

        transform.position += (Vector3) eventData.delta;

        foreach (var cell in HighlightedCells)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition))
            {
                TargetCell = cell;
                break;
            }

            TargetCell = null;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag called.");
        base.OnEndDrag(eventData);

        ClearCells();

        if (!TargetCell)
        {
            transform.position = CurrentCell.gameObject.transform.position;
            return;
        }

        Move();

        PieceManager.SwitchSides();
    }

    #endregion
}