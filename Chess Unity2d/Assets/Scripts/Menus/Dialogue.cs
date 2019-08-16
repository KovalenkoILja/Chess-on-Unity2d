using System;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    #region Variables

    [Serializable]
    public class DialogueNode
    {
        public Answer[] answer;
        public string text;
    }

    [Serializable]
    public class Answer
    {
        public GameSetup setup;
        public bool speakEnd;
        public string text;
        public int toNode;
    }

    public DialogueNode[] node;
    public int currentNode;
    public bool showDialogue;
    public bool isEnd;

    public AudioClip clickSound;
    private AudioSource ClickSource;

    private StartDialogResults StartDialogResults;

    #endregion

    #region UnityEvents

    private void Start()
    {
        ClickSource = GetComponent<AudioSource>();
        ClickSource.clip = clickSound;
        ClickSource.playOnAwake = false;
    }

    private void OnGUI()
    {
        if (showDialogue != true)
            return;

        GUI.Box(
            new Rect(Screen.width / 2 - 300, Screen.height - 300, 600, 250),
            "");
        GUI.Label(
            new Rect(Screen.width / 2 - 250, Screen.height - 280, 500, 90),
            node[currentNode].text,
            new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = Screen.height * 2 / 100,
                normal = {textColor = Color.white}
            });

        for (var i = 0; i < node[currentNode].answer.Length; i++)
        {
            if (!GUI.Button(
                new Rect(Screen.width / 2 - 250, Screen.height - 225 + 55 * i, 500, 45),
                node[currentNode].answer[i].text))
                continue;
            if (node[currentNode].answer[i].speakEnd)
            {
                ClickSource.PlayOneShot(clickSound);
                StaticEvents.DialogEndEvent(node[currentNode].answer[i].setup);

                showDialogue = false;
                isEnd = true;
            }

            currentNode = node[currentNode].answer[i].toNode;
        }
    }

    #endregion
}