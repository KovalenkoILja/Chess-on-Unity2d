using UnityEngine;
using UnityEngine.UI;

public class King : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        GetComponent<Image>()
            .sprite = Resources.Load<Sprite>(newTeamColor == Color.black
            ? "Sprites/blackKing"
            : "Sprites/whiteKing");
    }
}