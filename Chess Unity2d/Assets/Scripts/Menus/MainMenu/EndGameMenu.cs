using System;
using MaterialUI;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    #region Variables

    public GameObject menu;
    public AudioClip clickSound;

    public Text resultText;
    public Text gameSetupText;
    public Text playtime;
    public Text whiteTime;
    public Text blackTime;
    public Text score;

    private bool IsResultSaved;

    private AudioSource ClickSource => GetComponent<AudioSource>();

    #endregion

    #region UnityEvents

    private void Start()
    {
        gameObject.AddComponent<AudioSource>();

        ClickSource.clip = clickSound;
        ClickSource.playOnAwake = false;
    }

    // ReSharper disable StringLiteralTypo
    private void OnEnable()
    {
        gameSetupText.text = Extensions.SetupToString(GameStatData.GameSetup);
        resultText.text = GameStatData.Result;
        playtime.text = "Общее время игры:                                                  " +
                        TimeSpan.FromSeconds(GameStatData.Playtime).ToString(@"hh\:mm\:ss\:fff");

        whiteTime.text = "Время ходов белых:                                                 " +
                         TimeSpan.FromSeconds(GameStatData.WhiteTime).ToString(@"hh\:mm\:ss\:fff");

        blackTime.text = "Время ходов черных:                                               " +
                         TimeSpan.FromSeconds(GameStatData.BlackTime).ToString(@"hh\:mm\:ss\:fff");

        score.text = "Финальные очки:                                                       " + GameStatData.Score;
    }

    #endregion

    #region ButtonsClicks

    public void RestartButtonClick()
    {
        /*ToastManager.Show("RestartButtonClick");*/
        ClickSource.PlayOneShot(clickSound);
        menu.SetActive(false);

        StartCoroutine(Extensions.LoadingAsync("MainScene"));
    }

    public void MainMenuButtonClick()
    {
        /*ToastManager.Show("MainMenuButtonClick");*/
        ClickSource.PlayOneShot(clickSound);
        menu.SetActive(false);

        StartCoroutine(Extensions.LoadingAsync("MainMenuScene"));
    }

    public void SaveButtonClick()
    {
        ClickSource.PlayOneShot(clickSound);

        if (!IsResultSaved)
            DialogManager.ShowAlert(
                "Сохранить результат в таблицу результатов?",
                () =>
                {
                    ClickSource.PlayOneShot(clickSound);

                    if (SQLiteHelper.InsertResult())
                    {
                        IsResultSaved = true;
                        ToastManager.Show("Результат сохранен в таблицу результатов!");
                    }
                    else
                    {
                        ToastManager.Show("Результат не удалось сохранить в таблицу!");
                    }
                },
                "ДА", "Сохранить?",
                MaterialIconHelper.GetIcon(MaterialIconEnum.QUESTION_ANSWER),
                () => { ClickSource.PlayOneShot(clickSound); }, "НЕТ");
        else
            ToastManager.Show("Результат уже сохранен!");
    }

    // ReSharper disable once InconsistentNaming
    public void PGNButtonClick()
    {
        ClickSource.PlayOneShot(clickSound);
        DialogManager.ShowAlert(GameStatData.PGN,
            () => ClickSource.PlayOneShot(clickSound),
            "OK",
            "Portable Game Notation (PGN)",
            MaterialIconHelper.GetIcon(MaterialIconEnum.BOOK),
            () =>
            {
                GameStatData.PGN.CopyToClipboard();
                ToastManager.Show("Скопировано в буфер!");
            },
            "СКОПИРОВАТЬ В БУФЕР");
    }

    #endregion
}