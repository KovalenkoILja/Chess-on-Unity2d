using System;
using System.IO;
using UnityEngine;

public static class SaveGameHelper
{
    #region StaticStrings

    private const string SaveFileExtension = ".sav";
    private static readonly string SaveFolder = Application.dataPath + "/Saves";

    #endregion

    #region SaveGame

    public static bool IsSaveGameDirExist()
    {
        return Directory.Exists(SaveFolder);
    }

    public static void CreateSaveGameDir()
    {
        Directory.CreateDirectory(SaveFolder);
    }

    private static string GetSavePath(string name)
    {
        return Path.Combine(SaveFolder, name + SaveFileExtension);
    }

    public static bool IsSaveGameExist(string name)
    {
        return File.Exists(GetSavePath(name));
    }

    public static bool DeleteSaveGame(string name)
    {
        try
        {
            File.Delete(GetSavePath(name));
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            return false;
        }

        return true;
    }

    public static string[] GetAllSaveFiles()
    {
        return Directory.GetFiles(SaveFolder, "*" + SaveFileExtension,
            SearchOption.TopDirectoryOnly);
    }

    public static string[] GetAllSaveFileNames()
    {
        var saveList = GetAllSaveFiles();
        if (saveList.Length <= 0) return new string[0];
        var nameList = new string[saveList.Length];
        for (var i = 0; i < saveList.Length; i++)
            nameList[i] = Path.GetFileNameWithoutExtension(saveList[i]);
        return nameList;
    }

    public static string ReadFile(string fileName)
    {
        string jsonStr;

        using (var fs = new FileStream(GetSavePath(fileName), FileMode.Open))
        {
            using (var fileReader = new BinaryReader(fs))
            {
                jsonStr = fileReader.ReadString();
            }
        }

        return jsonStr;
    }

    public static bool SaveFile(string fileName, string jsonText)
    {
        try
        {
            using (var fs = new FileStream(GetSavePath(fileName), FileMode.Create))
            {
                using (var fileWriter = new BinaryWriter(fs))
                {
                    fileWriter.Write(jsonText);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
            return false;
        }

        return true;
    }

    #endregion
}