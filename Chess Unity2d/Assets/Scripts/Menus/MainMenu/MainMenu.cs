using System;
using System.Collections;
using System.Linq;
using MaterialUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Variables

    private bool isSettingsChecked;

    public AudioClip clickSound;

    //public GameObject loadGameMenu;
    public GameObject loadingMenu;
    public Slider loadingSlider;

    private Action<EventParam> LoadSaveGame;

    public GameObject menu;

    //public Text progressText;
    //public GameObject recordsMenu;
    public GameObject settingsMenu;

    private AudioSource ClickSource => GetComponent<AudioSource>();

    #endregion

    #region UnityEvents

    private void Awake()
    {
        LoadSaveGame = LoadSaveGameFunction;
    }

    private void Start()
    {
        gameObject.AddComponent<AudioSource>();

        ClickSource.clip = clickSound;
        ClickSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventsNames.LoadSaveGameEvent, LoadSaveGame);

        /*
        var setups = SQLiteHelper.GetGameSetups();
        
        foreach (var setup in setups)
            Debug.Log(setup.Setup);
*/
    }

    private void Update()
    {
        if (SettingsData.Settings != null)
            return;
        if (isSettingsChecked)
            return;
        // ReSharper disable StringLiteralTypo
        var dialog = DialogManager.ShowProgressLinear(
            "Выполняеться проверка файла настроек... Подождите пару секунд!",
            "Проверка",
            MaterialIconHelper.GetIcon(MaterialIconEnum.SETTINGS_OVERSCAN));

        StartCoroutine(CheckSettingsOnEnable(dialog, 2.0f));
        isSettingsChecked = true;
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventsNames.LoadSaveGameEvent, LoadSaveGame);
    }

    #endregion

    #region Methods

    private void LoadSaveGameFunction(EventParam obj)
    {
        StartGame();
    }

    private void StartGame()
    {
        settingsMenu.SetActive(false);
        //loadGameMenu.SetActive(false);
        //recordsMenu.SetActive(false);
        menu.SetActive(false);
        //loadingMenu.SetActive(true);

        StartCoroutine(LoadingAsync("MainScene"));
    }

    #endregion

    #region ButtonClicks

    public void NewGameButtonClick()
    {
        ClickSource.PlayOneShot(clickSound);
        Debug.Log("New Game Button pressed");
        StartGame();
    }

    public void ReturnToMainMenuClick()
    {
        ClickSource.PlayOneShot(clickSound);
        Debug.Log("Return To Main Menu Button pressed");
        settingsMenu.SetActive(false);
        //loadGameMenu.SetActive(false);
        //recordsMenu.SetActive(false);
        menu.SetActive(true);
    }

    public void LoadGameButtonClick()
    {
        ClickSource.PlayOneShot(clickSound);
        Debug.Log("Load Game Button pressed");

        if (!SaveGameHelper.IsSaveGameDirExist())
        {
            ToastManager.Show("Сохранений не найдено!");
            return;
        }

        var saveFiles = SaveGameHelper.GetAllSaveFileNames();
        if (saveFiles.Length == 0)
        {
            ToastManager.Show("Файлов сохранений не найдено!");
            return;
        }

        DialogManager.ShowRadioList(saveFiles, selectedIndex =>
            {
                ClickSource.PlayOneShot(clickSound);
                var saveData = SaveGameHelper.ReadFile(saveFiles[selectedIndex]);

                if (string.IsNullOrEmpty(saveData))
                {
                    ToastManager.Show(
                        "Файл " + saveFiles[selectedIndex] + " поврежден!");
                    return;
                }

                SaveGameData.Data = GameData.Deserialize(saveData);
                SaveGameData.LogList = SaveGameData.Data.logs;
                StartGame();
            },
            "ЗАГРУЗИТЬ",
            "Список сохранений",
            MaterialIconHelper.GetIcon(MaterialIconEnum.FILE_DOWNLOAD),
            () =>
            {
                ClickSource.PlayOneShot(clickSound);
                Debug.Log("Clicked the Cancel button");
            }, "НАЗАД");

        //menu.SetActive(false);
        //loadGameMenu.SetActive(true);
    }

    public void SettingsButtonClick()
    {
        ClickSource.PlayOneShot(clickSound);
        Debug.Log("Settings Button pressed");
        menu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void RecordsButtonClick()
    {
        ClickSource.PlayOneShot(clickSound);
        Debug.Log("Records Table Button pressed");

        var strings = SQLiteHelper.GetRecords();
        var rec = SQLiteHelper.GetGameResults().ToArray();
        if (strings == null)
        {
            ToastManager.Show("Таблица рекордов пуста!");
            return;
        }

        DialogManager.ShowSimpleList(strings,
            selectedIndex =>
            {
                ClickSource.PlayOneShot(clickSound);
                DialogManager.ShowAlert(rec[selectedIndex].PGN,
                    () => ClickSource.PlayOneShot(clickSound),
                    "OK",
                    "Portable Game Notation (PGN)",
                    MaterialIconHelper.GetIcon(MaterialIconEnum.BOOK),
                    () =>
                    {
                        rec[selectedIndex].PGN.CopyToClipboard();
                        ToastManager.Show("Скопировано в буфер!");
                    },
                    "СКОПИРОВАТЬ В БУФЕР");
            },
            "Список рекордов ",
            MaterialIconHelper.GetIcon(MaterialIconEnum.LIST));

        //menu.SetActive(false);
        //recordsMenu.SetActive(true);
    }

    public void QuitButtonClick()
    {
        ClickSource.PlayOneShot(clickSound);
        Debug.Log("Quit Button pressed");

        DialogManager.ShowAlert(
            "Выйти из игры?",
            () =>
            {
                ClickSource.PlayOneShot(clickSound);
                Debug.Log("Quit affirmative pressed");
                Application.Quit();
            },
            "ДА",
            "Выйти?",
            MaterialIconHelper.GetIcon(MaterialIconEnum.QUESTION_ANSWER),
            () =>
            {
                ClickSource.PlayOneShot(clickSound);
                Debug.Log("Quit Dismiss pressed");
            }, "НЕТ");
    }

    #endregion

    #region IEnumerators

    private static IEnumerator CheckSettingsOnEnable(MaterialDialog dialog, float duration)
    {
        var result = "";
        var settings = new Settings();
        try
        {
            if (SettingsHelper.SettingsFileExist())
            {
                Debug.Log("settings file found");
                settings = SettingsHelper.LoadSettings();
                if (settings == null) yield break;

                if (SettingsHelper.CheckSettings(settings))
                {
                    Debug.Log("settings applied");
                    SettingsHelper.ApplySettings(settings);

                    result = "Файл найден\nНастройки применены";
                    /*ToastManager.Show("Файл найден\nНастройки применены"
                        , 2.0f);*/
                }
                else
                {
                    Debug.Log("settings file invalid");
                    SettingsHelper.DeleteSettingsFile();
                    settings = SettingsHelper.GetCurrentSettings();
                    SettingsHelper.WriteSettingsToFile(settings);
                    Debug.Log("created new settings file");
                    result = "Файл не найден или поврежден\nНастройки сброшены";
                    /*ToastManager.Show("Файл не найден или поврежден\nНастройки сброшены"
                        , 2.0f);*/
                }
            }
            else
            {
                Debug.Log("settings file not found");
                settings = SettingsHelper.GetCurrentSettings();
                SettingsHelper.WriteSettingsToFile(settings);
                Debug.Log("created new settings file");
                Debug.Log(settings);
                result = "Файл не найден или поврежден\nНастройки сброшены";
                /*ToastManager.Show("Файл не найден или поврежден\nНастройки сброшены"
                    , 2.0f);*/
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }

        yield return new WaitForSeconds(duration);

        SettingsData.Settings = settings;
        dialog.Hide();
        ToastManager.Show(result, 2.0f);
    }

    private IEnumerator LoadingAsync(string nameOfScene)
    {
        var operation = SceneManager.LoadSceneAsync(nameOfScene);

        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = progress;
            //progressText.text = progress * 100f + "%";

            yield return new WaitForSeconds(1.5f);
        }

        yield return null;
    }

    #endregion
}