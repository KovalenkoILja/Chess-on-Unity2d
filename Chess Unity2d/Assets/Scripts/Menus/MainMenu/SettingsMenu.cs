using System;
using System.Collections;
using ChessEngine;
using MaterialUI;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    #region Variables

    private bool isMenuSet;

    public Settings settings;

    public Toggle fullscreenToggle;
    public MaterialDropdown resolutionDropdown;
    public MaterialDropdown qualityDropdown;
    public MaterialDropdown antiAliasDropdown;
    public MaterialDropdown vSyncDropdown;
    public Slider volumeSlider;
    public Button applyBtn;
    public Toggle fpsToggle;
    public MaterialDropdown aiDropdown;
    public AudioClip clickSound;

    private AudioSource ClickSource => GetComponent<AudioSource>();

    private Resolution[] Resolutions { get; set; }

    #endregion

    #region UnityEvents

    private void Start()
    {
        // ReSharper disable StringLiteralTypo
        var dialog = DialogManager.ShowProgressLinear(
            "Выполняеться построение меню настроек... Подождите!",
            "Построение",
            MaterialIconHelper.GetIcon(MaterialIconEnum.SETTINGS_OVERSCAN));

        StartCoroutine(CheckSettingsOnEnable(dialog, 1.0f));
    }

    private void Awake()
    {
        gameObject.AddComponent<AudioSource>();

        ClickSource.clip = clickSound;
        ClickSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        settings = SettingsData.Settings ?? SettingsHelper.LoadSettings();
    }

    private void Update()
    {
        if (isMenuSet)
            return;

        volumeSlider.value = AudioListener.volume;
        fullscreenToggle.isOn = Screen.fullScreen;
        fpsToggle.isOn = FPSDisplay.IsShowed;
        AudioListener.volume = settings.Volume = volumeSlider.value;

        var difficulty = settings.Difficulty;
        switch (difficulty)
        {
            case Engine.Difficulty.Easy:
                aiDropdown.currentlySelected = 0;
                break;
            case Engine.Difficulty.Medium:
                aiDropdown.currentlySelected = 1;
                break;
            case Engine.Difficulty.Hard:
                aiDropdown.currentlySelected = 2;
                break;
            case Engine.Difficulty.VeryHard:
                aiDropdown.currentlySelected = 3;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var i = 0;
        foreach (var res in resolutionDropdown.optionDataList.options)
        {
            if (res.text == Screen.currentResolution.ToString())
                resolutionDropdown.currentlySelected = i;

            i++;
        }

        isMenuSet = true;
    }

    private IEnumerator CheckSettingsOnEnable(MaterialDialog dialog, float duration)
    {
        try
        {
            Resolutions = Screen.resolutions;

            resolutionDropdown.ClearData();
            qualityDropdown.ClearData();
            antiAliasDropdown.ClearData();
            vSyncDropdown.ClearData();
            aiDropdown.ClearData();

            settings = SettingsData.Settings ?? SettingsHelper.LoadSettings();
            resolutionDropdown.ClearData();
            qualityDropdown.ClearData();
            antiAliasDropdown.ClearData();
            vSyncDropdown.ClearData();
            aiDropdown.ClearData();
            vSyncDropdown.AddData(new OptionData
            {
                text = "No sync"
            });
            vSyncDropdown.AddData(new OptionData
            {
                text = "Every V Blank (60 fps)"
            });
            vSyncDropdown.AddData(new OptionData
            {
                text = "Every Second V Blank (30 fps)"
            });
            vSyncDropdown.AddData(new OptionData
            {
                text = "Every Third V Blank (15 fps)"
            });

            foreach (var res in Resolutions)
                resolutionDropdown.AddData(new OptionData
                {
                    //text = res.width + "x" + res.height + " : " + res.refreshRate
                    text = res.ToString()
                });

            antiAliasDropdown.AddData(new OptionData
            {
                text = "None"
            });
            antiAliasDropdown.AddData(new OptionData
            {
                text = "Low"
            });
            antiAliasDropdown.AddData(new OptionData
            {
                text = "Normal"
            });
            antiAliasDropdown.AddData(new OptionData
            {
                text = "High"
            });

            qualityDropdown.AddData(new OptionData
            {
                text = "Very Low"
            });
            qualityDropdown.AddData(new OptionData
            {
                text = "Low"
            });
            qualityDropdown.AddData(new OptionData
            {
                text = "Medium"
            });
            qualityDropdown.AddData(new OptionData
            {
                text = "High"
            });
            qualityDropdown.AddData(new OptionData
            {
                text = "Very High"
            });
            qualityDropdown.AddData(new OptionData
            {
                text = "Ultra"
            });

            aiDropdown.AddData(new OptionData
            {
                text = "Easy"
            });
            aiDropdown.AddData(new OptionData
            {
                text = "Medium"
            });
            aiDropdown.AddData(new OptionData
            {
                text = "Hard"
            });
            aiDropdown.AddData(new OptionData
            {
                text = "Very Hard"
            });

            fullscreenToggle.onValueChanged.AddListener(delegate { FullscreenChange(); });
            resolutionDropdown.onItemSelected.AddListener(delegate { ResolutionChange(); });
            vSyncDropdown.onItemSelected.AddListener(delegate { VSyncChange(); });
            qualityDropdown.onItemSelected.AddListener(delegate { QualityChange(); });
            antiAliasDropdown.onItemSelected.AddListener(delegate { AntiAliasChange(); });
            volumeSlider.onValueChanged.AddListener(delegate { VolumeChange(); });
            applyBtn.onClick.AddListener(SaveSettings);
            fpsToggle.onValueChanged.AddListener(SwitchFpsChange);
            aiDropdown.onItemSelected.AddListener(delegate { AiChange(); });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }

        yield return new WaitForSeconds(duration);
        dialog.Hide();
    }

    #endregion

    #region Methods

    private void AiChange()
    {
        var difficulty = aiDropdown.currentlySelected;
        switch (difficulty)
        {
            case 0:
                settings.Difficulty = Engine.Difficulty.Easy;
                break;
            case 1:
                settings.Difficulty = Engine.Difficulty.Medium;
                break;
            case 2:
                settings.Difficulty = Engine.Difficulty.Hard;
                break;
            case 3:
                settings.Difficulty = Engine.Difficulty.VeryHard;
                break;
            default:
                settings.Difficulty = Engine.Difficulty.Medium;
                break;
        }
    }

    private void SwitchFpsChange(bool arg)
    {
        settings.IsFpsIsShowed = FPSDisplay.IsShowed = arg;
    }

    private void VolumeChange()
    {
        AudioListener.volume = settings.Volume = volumeSlider.value;
    }

    private void VSyncChange()
    {
        QualitySettings.vSyncCount = settings.VSync = vSyncDropdown.currentlySelected;
    }

    private void AntiAliasChange()
    {
        switch (antiAliasDropdown.currentlySelected)
        {
            case 0:
                QualitySettings.antiAliasing = settings.AntiAliasing = 0;
                break;
            case 1:
                QualitySettings.antiAliasing = settings.AntiAliasing = 2;
                break;
            case 2:
                QualitySettings.antiAliasing = settings.AntiAliasing = 4;
                break;
            case 3:
                QualitySettings.antiAliasing = settings.AntiAliasing = 8;
                break;
            default:
                QualitySettings.antiAliasing = 0;
                break;
        }
    }

    private void QualityChange()
    {
        var qualityDropdownValue = qualityDropdown.currentlySelected;
        settings.TextureQuality = qualityDropdownValue;
        QualitySettings.SetQualityLevel(qualityDropdownValue, true);
    }

    private void ResolutionChange()
    {
        try
        {
            settings.Resolution = Resolutions[resolutionDropdown.currentlySelected];
            Screen.SetResolution(
                settings.Resolution.width,
                settings.Resolution.height,
                settings.IsFullscreen);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void FullscreenChange()
    {
        Screen.fullScreen = settings.IsFullscreen = fullscreenToggle.isOn;
    }

    public void SaveSettings()
    {
        ClickSource.PlayOneShot(clickSound);

        if (SettingsHelper.CheckSettings(settings))
            SettingsHelper.WriteSettingsToFile(settings);

        SettingsData.Settings = settings;

        // ReSharper disable StringLiteralTypo
        ToastManager.Show("Настройки сохранены", 1f);
    }

    #endregion
}