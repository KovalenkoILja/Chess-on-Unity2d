using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Extensions
{
    // ReSharper disable StringLiteralTypo
    public static string ToString(this WindowCodeId idEnum)
    {
        switch (idEnum)
        {
            case WindowCodeId.TimeWindow:
                return "Таймер";
            case WindowCodeId.PauseWindow:
                return "ПАУЗА";
            case WindowCodeId.ChessMoveWindow:
                return "Текущий Ход";
            case WindowCodeId.LoadWindow:
                return "ЗАГРУЗИТЬ";
            case WindowCodeId.SaveWindow:
                return "СОХРАНИТЬ";
            case WindowCodeId.OptionsWindow:
                return "ОПЦИИ";
            case WindowCodeId.ConfirmWindow:
                return "Подтвердить";
            case WindowCodeId.LogWindow:
                return "Log";
            case WindowCodeId.Console:
                return "Console";
            case WindowCodeId.ScoreWindow:
                return "Оценка текущего положения на шахматной доске";
            default:
                return "ERROR!!!";
        }
    }

    // ReSharper disable once InconsistentNaming
    public static Vector2Int FromPosition(byte pos)
    {
        if (pos <= 7) return new Vector2Int(pos % 8, 7);

        if (pos > 7 && pos <= 15) return new Vector2Int(pos % 8, 6);

        if (pos > 15 && pos <= 23) return new Vector2Int(pos % 8, 5);

        if (pos > 23 && pos <= 31) return new Vector2Int(pos % 8, 4);

        if (pos > 31 && pos <= 39) return new Vector2Int(pos % 8, 3);

        if (pos > 39 && pos <= 47) return new Vector2Int(pos % 8, 2);

        if (pos > 47 && pos <= 55) return new Vector2Int(pos % 8, 1);

        if (pos > 55 && pos <= 63) return new Vector2Int(pos % 8, 0);
        return new Vector2Int(-1, -1);
    }

    public static string SetupToString(GameSetup setup)
    {
        var setupStr = "Режим игры: ";
        switch (setup)
        {
            case GameSetup.PlayerWhiteVsPlayerBlack:
                setupStr += "Игрок против Игрока";
                break;
            case GameSetup.PlayerWhiteVsAiBlack:
                setupStr += "Игрок за Белых против Компьютера";
                break;
            case GameSetup.PlayerBlackVsAiWhite:
                setupStr += "Игрок за Черных против Компьютера";
                break;
            case GameSetup.AiWhiteVsAiBlack:
                setupStr += "Компьютер против Компьютера";
                break;
            case GameSetup.None:
                setupStr += "Никакой";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return setupStr;
    }

    public static void CopyToClipboard(this string s)
    {
        var te = new TextEditor();
        te.text = s;
        te.SelectAll();
        te.Copy();
    }


    public static string PieceToStr(BasePiece currentPiece)
    {
        var message = "";

        switch (currentPiece.GetType().ToString())
        {
            case "Pawn":
                if (currentPiece.mainColor == Color.black)
                    message += "♟ (Черные " + currentPiece.GetType() + ")";
                else if (currentPiece.mainColor == Color.white)
                    message += "♙ (Белые " + currentPiece.GetType() + ")";
                break;
            case "Rook":
                if (currentPiece.mainColor == Color.black)
                    message += "♖ (Черные " + currentPiece.GetType() + ")";
                else if (currentPiece.mainColor == Color.white)
                    message += "♜ (Белые " + currentPiece.GetType() + ")";
                break;
            case "Knight":
                if (currentPiece.mainColor == Color.black)
                    message += "♞ (Черные " + currentPiece.GetType() + ")";
                else if (currentPiece.mainColor == Color.white)
                    message += "♘ (Белые " + currentPiece.GetType() + ")";
                break;
            case "Bishop":
                if (currentPiece.mainColor == Color.black)
                    message += "♝ (Черные " + currentPiece.GetType() + ")";
                else if (currentPiece.mainColor == Color.white)
                    message += "♗ (Белые " + currentPiece.GetType() + ")";
                break;
            case "Queen":
                if (currentPiece.mainColor == Color.black)
                    message += "♛ (Черные " + currentPiece.GetType() + ")";
                else if (currentPiece.mainColor == Color.white)
                    message += "♕ (Белые " + currentPiece.GetType() + ")";
                break;
            case "King":
                if (currentPiece.mainColor == Color.black)
                    message += "♚ (Черные " + currentPiece.GetType() + ")";
                else if (currentPiece.mainColor == Color.white)
                    message += "♔ (Белые " + currentPiece.GetType() + ")";
                break;
            default:
                Debug.Log("Неизвестная фигура!!!");
                break;
        }

        return message;
    }

    public static IEnumerator LoadingAsync(string nameOfScene)
    {
        var operation = SceneManager.LoadSceneAsync(nameOfScene);

        while (!operation.isDone) yield return new WaitForSeconds(1.5f);

        yield return null;
    }
}