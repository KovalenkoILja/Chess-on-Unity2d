using System;
using U3D.Threading;
using U3D.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using AggregateException = U3D.AggregateException;
using Thread = System.Threading.Thread;

public class BasicTests : MonoBehaviour
{
    public Text text;

    public void Start()
    {
        Dispatcher.Initialize();
    }

    public void Execute()
    {
        CleanLog();
#if UNITY_WSA && !UNITY_EDITOR
        System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(ThExecute);
#else
        var t = new Thread(ThExecute);
#endif
        t.Start();
    }

    private void ThExecute()
    {
        Dispatcher.Instance.ToMainThread(() => { WriteLog("[0s]\tT1 executing on main thread"); });

        Dispatcher.ToMainThreadAfterDelay(5,
            () => { WriteLog("[5s]\tT2 executing on main thread after 5 seconds delay"); });
        var t3 = Dispatcher.Instance.TaskToMainThread(() =>
        {
            WriteLog("[0s]\tT3 executing on main thread controlled by task");
        });
        t3.ContinueInMainThreadWith(t3c => { WriteLog("*[0s]\tT3 continued"); });

        var t4 = Task.RunInMainThread(() =>
        {
            WriteLog("[0s]\tT4 starts, 6 seconds delay");
            Task.Delay(6000);
            WriteLog("[6s]\tT4 finished 6 seconds delay");
        });
        t4.ContinueInMainThreadWith(t4c => { WriteLog("*[6s]\tT4 continued"); });

        Task.Run(() =>
        {
            DateTime? t = null;
            if (t.Value.Second == 0)
            {
            }

            ;
        }).ContinueInMainThreadWith(t5c =>
        {
            if (t5c.IsFaulted)
                WriteLog("[0s]\tT5 is faulted as expected: {0}", t5c.Exception.InnerExceptions[0].Message);
            else
                WriteLog("ERROR!!!: T5 is not faulted!");
        });

        var tcs = new TaskCompletionSource<bool>();
        tcs.Task.ContinueInMainThreadWith(t =>
        {
            WriteLog("*[8s]\tT6 is continued after completion source result is set");
        });
        Dispatcher.ToMainThreadAfterDelay(8, () =>
        {
            WriteLog("[8s]\tT6 executing on main thread after 8 seconds delay, setting result");
            tcs.SetResult(true);
        });
        Task.WhenAll(
            Task.RunInMainThread(() => { WriteLog("[0s]\tT7.1 is finished"); }),
            Task.RunInMainThread(() =>
            {
                Task.Delay(5000);
                WriteLog("[5s]\tT7.2 is finished");
            }),
            Task.RunInMainThread(() => { WriteLog("[0s]\tT7.3 is finished"); }),
            Task.RunInMainThread(() =>
            {
                Task.Delay(2000);
                WriteLog("[2s]\tT7.4 is finished");
            }),
            Task.RunInMainThread(() => { WriteLog("[0s]\tT7.5 is finished"); })
        ).ContinueInMainThreadWith(t7c => { WriteLog("*[5s]\tT7 is continued after all 7.* are finished"); });

        Task.WhenAll(
                Task.Run(() => { }),
                Task.Run(() =>
                {
                    DateTime? t = null;
                    if (t.Value.Second == 0)
                    {
                    }

                    ;
                }),
                Task.Run(() => { }),
                Task.Run(() =>
                {
                    DateTime? t = null;
                    if (t.Value.Second == 0)
                    {
                    }

                    ;
                }),
                Task.Run(() => { }))
            .ContinueInMainThreadWith(t8c =>
            {
                if (t8c.IsFaulted)
                {
                    var e0 = t8c.Exception.InnerExceptions[0];
                    while (e0 is AggregateException) e0 = ((AggregateException) e0).InnerExceptions[0];
                    var e1 = t8c.Exception.InnerExceptions[1];
                    while (e1 is AggregateException) e1 = ((AggregateException) e1).InnerExceptions[0];

                    WriteLog("[0s]\tT8 is faulted as expected: {0}, {1}", e0.Message, e1.Message);
                }
                else
                {
                    WriteLog("ERROR!!!: T8 is not faulted!");
                }
            });
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
}