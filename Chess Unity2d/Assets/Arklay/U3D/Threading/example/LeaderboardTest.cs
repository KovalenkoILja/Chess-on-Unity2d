using System;
using System.Collections;
using U3D.Threading;
using U3D.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using AggregateException = U3D.AggregateException;
using Random = UnityEngine.Random;

public class LeaderboardTest : MonoBehaviour
{
    private const int queryLimit = 10;

    private bool m_executing;

    private bool m_stopProcessing;
    public Text text;

    public void Start()
    {
        Dispatcher.Initialize();
    }

    private void CleanLog()
    {
        lock (text)
        {
            text.text = "";
        }
    }

    private void WriteLog(string fmt, params object[] pars)
    {
        lock (text)
        {
            text.text += string.Format("{0}\t\t{1}\n", DateTime.Now, string.Format(fmt, pars));
        }
    }

    public void Execute(Button sender)
    {
        var senderImage = sender.GetComponent<Image>();
        var senderText = sender.GetComponentInChildren<Text>();
        if (m_executing)
        {
            m_stopProcessing = true;
        }
        else
        {
            if (m_stopProcessing) return;
            m_executing = true;

            m_stopProcessing = false;
            CleanLog();
            senderImage.color = Color.red;
            senderText.text = "Stop";

            GetCompleteLeaderboard(
                (pos, name) => { WriteLog("{0} - {1}", pos, name); }
            ).ContinueInMainThreadWith(t =>
            {
                senderImage.color = Color.white;
                senderText.text = "Run leaderboard tests";
                m_executing = false;

                if (t.IsFaulted)
                {
                    var e = t.Exception.InnerExceptions[0];
                    while (e is AggregateException) e = ((AggregateException) e).InnerExceptions[0];
                    WriteLog("--- Error getting leaderboard {0}", e.Message);
                }
                else
                {
                    WriteLog("--- Finished getting leaderboard");
                }
            });
        }
    }

    private Task GetCompleteLeaderboard(Action<int, string> intermediateResult)
    {
        var tcs = new TaskCompletionSource<bool>();
        GetCompleteLeaderboardAux(0, tcs, intermediateResult);
        return tcs.Task;
    }

    private void GetCompleteLeaderboardAux(int offset, TaskCompletionSource<bool> tcs,
        Action<int, string> intermediateResult)
    {
        if (m_stopProcessing)
        {
            m_stopProcessing = false;
            tcs.SetError(new Exception("User cancelled"));
        }
        else
        {
            StartCoroutine(ConnectToAPI(offset, tcs, intermediateResult));
        }
    }

    private IEnumerator ConnectToAPI(int offset, TaskCompletionSource<bool> tcs, Action<int, string> intermediateResult)
    {
        yield return StartCoroutine(YOUR_FAVORITE_API_GetLeaderBoardUsingWWW(offset, queryLimit));

        var result = YOUR_FAVORITE_API_GetResultsFromCall();

        if (result == null) tcs.SetError(new Exception("API returns NULL"));

        foreach (int k in result.Keys) intermediateResult(k, (string) result[k]);
        if (result.Count == queryLimit)
            GetCompleteLeaderboardAux(offset + queryLimit, tcs, intermediateResult);
        else
            tcs.SetResult(true);
    }

    #region API mock up (WWW based)

    private const int secondsDelay = 1;
    private int m_currentOffset;
    private int m_currentLimit;

    private IEnumerator YOUR_FAVORITE_API_GetLeaderBoardUsingWWW(int offset, int limit)
    {
        m_currentOffset = -1;
        yield return new WaitForSeconds(secondsDelay);
        m_currentOffset = offset;
        m_currentLimit = limit;
    }

    private const int nbOfRegistersTotal = 303;

    private SortedList YOUR_FAVORITE_API_GetResultsFromCall()
    {
        var ret = new SortedList();
        for (var i = 0; i < m_currentLimit; i++)
        {
            var currentRegister = m_currentOffset + i;
            if (currentRegister > nbOfRegistersTotal)
                break;
            ret.Add(currentRegister, "Player " + Random.Range(0, nbOfRegistersTotal));
        }

        return ret;
    }

    #endregion
}