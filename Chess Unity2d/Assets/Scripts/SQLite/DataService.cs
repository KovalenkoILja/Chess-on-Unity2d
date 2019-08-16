using System.Collections.Generic;
using SQLiteTypes;
using UnityEngine;

#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif

public class DataService
{
    private readonly SQLiteConnection Connection;

    // ReSharper disable once InconsistentNaming
    public DataService(string DatabaseName)
    {
#if UNITY_EDITOR
        var dbPath = $@"Assets/StreamingAssets/{DatabaseName}";
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb =
 new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb =
 Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb =
 Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb =
 Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb =
 Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb =
 Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif

        Connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final DB PATH: " + dbPath);
    }

    public void CreateDB()
    {
        Connection.DropTable<GameResult>();
        Connection.DropTable<GameSetups>();
        Connection.DropTable<SQLiteTypes.EndGamesStates>();
        Connection.CreateTable<GameResult>();
        Connection.CreateTable<GameSetups>();
        Connection.CreateTable<SQLiteTypes.EndGamesStates>();
    }

    public IEnumerable<GameResult> GetGameResults()
    {
        return Connection.Table<GameResult>();
    }

    public IEnumerable<GameSetups> GetGameSetups()
    {
        return Connection.Table<GameSetups>();
    }

    public IEnumerable<SQLiteTypes.EndGamesStates> GetEndGamesStates()
    {
        return Connection.Table<SQLiteTypes.EndGamesStates>();
    }

    public int InsertResult(GameResult result)
    {
        return Connection.Insert(result);
    }
}