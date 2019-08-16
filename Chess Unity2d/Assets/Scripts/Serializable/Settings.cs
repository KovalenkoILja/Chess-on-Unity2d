using System;
using ChessEngine;
using UnityEngine;

[Serializable]
public class Settings
{
    [SerializeField] protected int antiAliasing;

    [SerializeField] protected Engine.Difficulty difficulty;

    [SerializeField] protected int height;

    [SerializeField] protected bool isFpsIsShowed;

    [SerializeField] protected bool isFullscreen;

    [SerializeField] protected int refreshRate;

    [SerializeField] protected int textureQuality;

    [SerializeField] protected float volume;

    [SerializeField] protected int vSync;

    [SerializeField] protected int width;

    public Settings()
    {
    }

    public Settings(float volume,
        int height,
        int width,
        int refreshRate,
        int vSync,
        int antiAliasing,
        int textureQuality,
        bool isFullscreen,
        bool isFpsIsShowed,
        Engine.Difficulty difficulty)
    {
        this.volume = volume;
        this.height = height;
        this.width = width;
        this.refreshRate = refreshRate;
        this.vSync = vSync;
        this.antiAliasing = antiAliasing;
        this.textureQuality = textureQuality;
        this.isFullscreen = isFullscreen;
        this.isFpsIsShowed = isFpsIsShowed;
        this.difficulty = difficulty;
    }

    public float Volume
    {
        get => volume;
        set => volume = value;
    }

    public Resolution Resolution
    {
        get => new Resolution {height = height, width = width, refreshRate = refreshRate};
        set
        {
            height = value.height;
            width = value.width;
            refreshRate = value.refreshRate;
        }
    }

    public int VSync
    {
        get => vSync;
        set => vSync = value;
    }

    public int AntiAliasing
    {
        get => antiAliasing;
        set => antiAliasing = value;
    }

    public int TextureQuality
    {
        get => textureQuality;
        set => textureQuality = value;
    }

    public bool IsFullscreen
    {
        get => isFullscreen;
        set => isFullscreen = value;
    }

    public bool IsFpsIsShowed
    {
        get => isFpsIsShowed;
        set => isFpsIsShowed = value;
    }

    public Engine.Difficulty Difficulty
    {
        get => difficulty;
        set => difficulty = value;
    }

    public static Settings Deserialize(string obj)
    {
        return JsonUtility.FromJson<Settings>(obj);
    }
}