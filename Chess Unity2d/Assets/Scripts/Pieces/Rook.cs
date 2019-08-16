using UnityEngine;
using UnityEngine.UI;

public class Rook : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        GetComponent<Image>()
            .sprite = Resources.Load<Sprite>(newTeamColor == Color.black
            ? "Sprites/blackRook"
            : "Sprites/whiteRook");
    }
}