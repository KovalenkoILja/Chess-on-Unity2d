using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameMenu : MonoBehaviour
{
    #region Varibles

    public GameObject scrollViewContent;
    public GameObject item;
    public AudioClip clickSound;

    private readonly List<GameObject> Items = new List<GameObject>();

    private AudioSource ClickSource => GetComponent<AudioSource>();

    #endregion

    #region UnityEvents

    private void Start()
    {
        gameObject.AddComponent<AudioSource>();

        ClickSource.clip = clickSound;
        ClickSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (!SaveGameHelper.IsSaveGameDirExist()) return;

        var saveFiles = SaveGameHelper.GetAllSaveFiles();

        if (saveFiles.Length == 0) return;

        scrollViewContent.transform.DetachChildren();

        foreach (var file in saveFiles)
        {
            Debug.Log(Path.GetFileName(file));
            var newItem = Instantiate(item, scrollViewContent.transform, false);
            newItem.transform.localPosition = Vector3.zero;
            Items.Add(newItem);

            newItem.GetComponentInChildren<Text>().text = Path.GetFileNameWithoutExtension(file);

            newItem.GetComponent<Button>().onClick.AddListener(
                () => OnItemClick(newItem));
        }
    }

    #endregion

    #region Clicks

    private void OnItemClick(GameObject newItem)
    {
        ClickSource.PlayOneShot(clickSound);
        foreach (var i in Items)
            if (i != newItem)
                i.GetComponentInChildren<Toggle>().isOn = false;

        newItem.GetComponentInChildren<Toggle>().isOn = !newItem.GetComponentInChildren<Toggle>().isOn;
        Debug.Log("Clicked on " + newItem.GetComponentInChildren<Text>().text);
    }

    public void OnApplyBtnClick()
    {
        Debug.Log("Clicked on OnApplyBtnClick");
        ClickSource.PlayOneShot(clickSound);

        foreach (var i in Items)
        {
            if (!i.GetComponentInChildren<Toggle>().isOn)
                continue;

            var saveData = SaveGameHelper.ReadFile(i.GetComponentInChildren<Text>().text);
            if (string.IsNullOrEmpty(saveData))
                return;

            SaveGameData.Data = GameData.Deserialize(saveData);

            EventManager.TriggerEvent(EventsNames.LoadSaveGameEvent, new EventParam());
        }
    }

    public void OnDeleteBtnClick()
    {
        ClickSource.PlayOneShot(clickSound);
        foreach (var i in Items)
        {
            if (!i.GetComponentInChildren<Toggle>().isOn) continue;
            if (SaveGameHelper.DeleteSaveGame(i.GetComponentInChildren<Text>().text))
                i.transform.parent = null;
        }

        Debug.Log("Clicked on OnDeleteBtnClick");
    }

    #endregion
}