using System;
using System.Globalization;
using System.IO;
using ChessEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveGameHelper;

// ReSharper disable StringLiteralTypo
public class PauseMenu : MonoBehaviour
{
    #region PauseDialogWindow

    private void PauseDialogWindow(int windowId)
    {
        if (GUI.Button(
            new Rect(5, 20, _pauseDialogWindowRect.width - 10, 35),
            "ГЛАВНОЕ МЕНЮ"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("MainMenu LoadScene Button pressed");
            SceneManager.LoadScene("MainMenuScene");
        }

        if (GUI.Button(
            new Rect(5, 60, _pauseDialogWindowRect.width - 10, 35),
            "ЗАНОВО"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Restart Game Button pressed");

            menuEnabled = !menuEnabled;

            MenuEvents.RestartGame();
        }

        if (GUI.Button(
            new Rect(5, 100, _pauseDialogWindowRect.width - 10, 35),
            "СОХРАНИТЬ ИГРУ"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Save Game Button pressed");
            showPauseDialog = false;
            showSaveGameDialog = true;
        }

        if (GUI.Button(
            new Rect(5, 140, _pauseDialogWindowRect.width - 10, 35),
            "ЗАГРУЗИТЬ ИГРУ"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Load Game Button pressed");
            showPauseDialog = false;
            showLoadGameDialog = true;
        }

        if (GUI.Button(
            new Rect(5, 180, _pauseDialogWindowRect.width - 10, 35),
            "ОПЦИИ"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Option Button pressed");
            showOptionDialog = true;
            showPauseDialog = false;
        }
        else if (GUI.Button(
            new Rect(5, 220, _pauseDialogWindowRect.width - 10, 35),
            "ВЫЙТИ ИЗ ИГРЫ"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Quit Game Button pressed");
            Application.Quit();
        }
    }

    #endregion

    #region SaveGameDialogWindow

    private void SaveGameDialogWindow(int windowId)
    {
        stringToEdit = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd h-mm");

        var guiStyle = GUI.skin.box;
        guiStyle.alignment = TextAnchor.MiddleCenter;

        GUI.Label(
            new Rect(10, 20, _pauseDialogWindowRect.width - 10, 30),
            "Название файла:",
            new GUIStyle
            {
                //richText = enabled,
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = new Color32(235, 235, 235, 255)}
            });


        stringToEdit = GUI.TextField(
            new Rect(5, 60, _pauseDialogWindowRect.width - 10, 75),
            stringToEdit, 25, guiStyle);

        if (GUI.Button(
            new Rect(5, 180, _pauseDialogWindowRect.width - 10, 35),
            "СОХРАНИТЬ"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Savegame Button pressed");

            MenuEvents.SaveGame(stringToEdit);

            showSaveGameDialog = false;
            showPauseDialog = true;
        }
        else if (GUI.Button(new Rect(5, 220, _pauseDialogWindowRect.width - 10, 35), "НАЗАД"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Back to PauseMenu Button pressed");
            showSaveGameDialog = false;
            showPauseDialog = true;
        }
    }

    #endregion

    #region LoadGameDialogWindow

    private void LoadGameDialogWindow(int windowId)
    {
        var scrollPosition = Vector2.zero;

        if (!IsSaveGameDirExist())
            return;

        var saves = GetAllSaveFiles();
        if (saves.Length == 0)
            return;

        GUILayout.BeginArea(new Rect(10, 25, 175, 150));
        GUILayout.BeginScrollView(scrollPosition);

        foreach (var save in saves)
        {
            GUILayout.BeginHorizontal("box", GUILayout.Height(25));
            if (GUILayout.Button(Path.GetFileNameWithoutExtension(save)))
            {
                ClickSource.PlayOneShot(clickSound);
                Debug.Log("Load Game Button pressed");
                Debug.Log("Save File : " + Path.GetFileName(save));

                var saveData = ReadFile(Path.GetFileNameWithoutExtension(save));

                if (string.IsNullOrEmpty(saveData))
                    return;

                SaveGameData.Data = GameData.Deserialize(saveData);

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (GUI.Button(new Rect(5, 220,
                _pauseDialogWindowRect.width - 10, 35),
            "НАЗАД"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Back to PauseMenu Button pressed");
            showLoadGameDialog = false;
            showPauseDialog = true;
        }
    }

    #endregion

    #region OptionsDialogWindow

    private void OptionsDialogWindow(int windowId)
    {
        GUI.Label(
            new Rect(10, 20, _pauseDialogWindowRect.width - 10, 30),
            "Громкость:",
            new GUIStyle
            {
                //richText = enabled,
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = new Color32(235, 235, 235, 255)}
            });
        GUI.Label(
            new Rect(10, 35, _pauseDialogWindowRect.width - 10, 30),
            AudioSliderValue.ToString(CultureInfo.CurrentCulture),
            new GUIStyle
            {
                //richText = enabled,
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = new Color32(235, 235, 235, 255)}
            });
        AudioSliderValue = GUI.HorizontalSlider(new Rect(15, 60, 170, 40),
            AudioSliderValue, 0.0F, 1.0F);
        GUI.Label(
            new Rect(10, 85, _pauseDialogWindowRect.width - 10, 30),
            "Сложность ИИ:",
            new GUIStyle
            {
                //richText = enabled,
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = new Color32(235, 235, 235, 255)}
            });
        if (GUI.Button(new Rect(15, 115, 40, 40), "<"))
            if (Difficulty != Engine.Difficulty.Easy)
                Difficulty--;
        GUI.Label(
            new Rect(10, 120, _pauseDialogWindowRect.width - 10, 30),
            Difficulty.ToString(),
            new GUIStyle
            {
                //richText = enabled,
                alignment = TextAnchor.MiddleCenter,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = new Color32(235, 235, 235, 255)}
            });
        if (GUI.Button(new Rect(145, 115, 40, 40), ">"))
            if (Difficulty != Engine.Difficulty.VeryHard)
                Difficulty++;

        if (GUI.Button(new Rect(5, 180, _pauseDialogWindowRect.width - 10, 35),
            "ПРИМЕНИТЬ"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Apply Button pressed");
            AudioListener.volume = AudioSliderValue;
            if (SettingsData.Settings != null)
                SettingsData.Settings.Difficulty = Difficulty;
            StaticEvents.SettingsChangeEvent(Difficulty);
        }
        else if (GUI.Button(new Rect(5, 220, _pauseDialogWindowRect.width - 10, 35),
            "НАЗАД"))
        {
            ClickSource.PlayOneShot(clickSound);
            Debug.Log("Back to PauseMenu Button pressed");
            showOptionDialog = false;
            showPauseDialog = true;
        }
    }

    #endregion

    #region Variables

    public bool menuEnabled;
    public AudioClip clickSound;
    private AudioSource ClickSource;

    private bool showPauseDialog;
    private bool showSaveGameDialog;
    private bool showLoadGameDialog;
    private bool showOptionDialog;

    private Engine.Difficulty Difficulty;

    private static Rect _pauseDialogWindowRect =
        new Rect(
            (Screen.width - 200) / 2.0f,
            (Screen.height - 300) / 2.0f,
            200,
            260);

    private Action<EventParam> PauseMenuAction;
    private float AudioSliderValue;
    private string stringToEdit = " ";

    #endregion

    #region UnityEvents

    private void Start()
    {
        ClickSource = GetComponent<AudioSource>();
        ClickSource.clip = clickSound;
        ClickSource.playOnAwake = false;
    }

    private void Awake()
    {
        PauseMenuAction = PauseMenuActionHandler;
    }

    private void PauseMenuActionHandler(EventParam eventParam)
    {
        if (eventParam.TypeOfEvent != EventResponseType.GameManagerEvent)
            return;

        if (!(eventParam.Response is MenuResponse result))
            return;

        menuEnabled = result.IsShowed;
        if (menuEnabled)
        {
            showPauseDialog = true;
            showSaveGameDialog = false;
            showLoadGameDialog = false;
            showOptionDialog = false;
        }
        else
        {
            showPauseDialog = false;
            showSaveGameDialog = false;
            showLoadGameDialog = false;
            showOptionDialog = false;
        }
    }

    private void OnEnable()
    {
        AudioSliderValue = AudioListener.volume;
        if (SettingsData.Settings != null)
            Difficulty = SettingsData.Settings != null ? Engine.Difficulty.Medium : SettingsData.Settings.Difficulty;
        EventManager.StartListening(EventsNames.PauseMenuEvent, PauseMenuAction);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventsNames.PauseMenuEvent, PauseMenuAction);
    }

    private void OnGUI()
    {
        if (!menuEnabled)
            return;

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        if (!menuEnabled)
            return;

        if (showPauseDialog)
            _pauseDialogWindowRect = GUI.Window(
                (int) WindowCodeId.PauseWindow,
                _pauseDialogWindowRect,
                PauseDialogWindow,
                Extensions.ToString(WindowCodeId.PauseWindow));
        else if (showLoadGameDialog)
            _pauseDialogWindowRect = GUI.Window(
                (int) WindowCodeId.LoadWindow,
                _pauseDialogWindowRect,
                LoadGameDialogWindow,
                Extensions.ToString(WindowCodeId.LoadWindow));
        else if (showSaveGameDialog)
            _pauseDialogWindowRect = GUI.Window(
                (int) WindowCodeId.SaveWindow,
                _pauseDialogWindowRect,
                SaveGameDialogWindow,
                Extensions.ToString(WindowCodeId.SaveWindow));
        else if (showOptionDialog)
            _pauseDialogWindowRect = GUI.Window(
                (int) WindowCodeId.OptionsWindow,
                _pauseDialogWindowRect,
                OptionsDialogWindow,
                Extensions.ToString(WindowCodeId.OptionsWindow));
    }

    #endregion
}