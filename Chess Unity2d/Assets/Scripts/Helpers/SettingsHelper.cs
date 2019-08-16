using System.IO;
using System.Linq;
using ChessEngine;
using UnityEngine;

public static class SettingsHelper
{
    #region StaticStrings

    private static readonly string SettingsFileName = Application.persistentDataPath + "\\settings.json";

    #endregion

    #region WorkWithSettings

    public static Settings LoadSettings()
    {
        return JsonUtility.FromJson<Settings>(
            File.ReadAllText(SettingsFileName));
    }

    public static bool SettingsFileExist()
    {
        return File.Exists(SettingsFileName);
    }

    public static bool DeleteSettingsFile()
    {
        if (!File.Exists(SettingsFileName))
            return false;

        File.Delete(SettingsFileName);
        return true;
    }

    public static void WriteSettingsToFile(Settings settings)
    {
        File.WriteAllText(SettingsFileName,
            JsonUtility.ToJson(settings, true));
    }

    public static bool CheckSettings(Settings settings)
    {
        var result = Screen.resolutions.Any(res =>
            settings.Resolution.width == res.width &&
            settings.Resolution.height == res.height &&
            settings.Resolution.refreshRate == res.refreshRate);

        if (settings.Volume < 0 || settings.Volume > 1)
            result = false;

        if (settings.VSync < 0 || settings.VSync > 4)
            result = false;

        if (settings.AntiAliasing == 0 ||
            settings.AntiAliasing == 2 ||
            settings.AntiAliasing == 4 ||
            settings.AntiAliasing == 8)
            result = true;

        if (settings.TextureQuality <= 0 || settings.TextureQuality >= 5)
            result = false;

        return result;
    }

    public static void ApplySettings(Settings settings)
    {
        Screen.SetResolution(settings.Resolution.width, settings.Resolution.height, settings.IsFullscreen);
        QualitySettings.vSyncCount = settings.VSync;
        QualitySettings.antiAliasing = settings.AntiAliasing;
        QualitySettings.SetQualityLevel(settings.TextureQuality, true);
        AudioListener.volume = settings.Volume;
        FPSDisplay.IsShowed = settings.IsFpsIsShowed;
    }

    public static Settings GetCurrentSettings()
    {
        return new Settings(
            AudioListener.volume,
            Screen.currentResolution.height,
            Screen.currentResolution.width,
            Screen.currentResolution.refreshRate,
            QualitySettings.vSyncCount,
            QualitySettings.antiAliasing,
            QualitySettings.GetQualityLevel(),
            Screen.fullScreen,
            FPSDisplay.IsShowed,
            Engine.Difficulty.Medium
        );
    }

    #endregion
}