using System;
using System.Collections;
using System.Collections.Generic;
using U3D.Threading.Tasks;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace U3D.Threading
{
    /// <summary>
    ///     Dispatches actions into the main thread.
    /// </summary>
    public class Dispatcher : MonoBehaviour
    {
        private static Dispatcher _instance;

        private static bool _initialized;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Queue<Action> QueueActions = new Queue<Action>();

        /// <summary>
        ///     Gets the singleton instance.
        ///     Script gets instantiated into a gameobject in the scene if needed
        /// </summary>
        /// <value>The instance.</value>
        public static Dispatcher Instance
        {
            get
            {
                if (_initialized)
                    return _instance;

                Debug.LogError(
                    "You need to call Initialize on the Main thread before using the Dispatcher");
                throw new InvalidOperationException(
                    "You need to call Initialize on the Main thread before using the Dispatcher");
            }
        }

        public static void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;

            var go = GameObject.Find(typeof(Dispatcher).Name);
            if (go == null)
                go = new GameObject(typeof(Dispatcher).Name);
            _instance = go.GetComponent<Dispatcher>();
            if (_instance == null)
                _instance = go.AddComponent<Dispatcher>();
            DontDestroyOnLoad(_instance.gameObject);
            _instance.gameObject.SendMessage("InitializeInstance",
                SendMessageOptions.DontRequireReceiver);
        }

        public void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (QueueActions.Count > 0)
            {
                var a = QueueActions.Dequeue();
                a?.Invoke();
            }
        }

        /// <summary>
        ///     Execute the Action in the main thread as soon as posible.
        /// </summary>
        /// <param name="a">Action to be executed.</param>
        public void ToMainThread(Action a)
        {
            QueueActions.Enqueue(a);
        }

        /// <summary>
        ///     Executes the Action in the main thread as soon as posible
        ///     and returns a Task which monitors its execution.
        /// </summary>
        /// <returns>Task monintoring the execution of the action.</returns>
        /// <param name="a">Action to be executed.</param>
        public Task TaskToMainThread(Action a)
        {
            var tcs = new TaskCompletionSource<bool>();
            QueueActions.Enqueue(() =>
            {
                a();
                tcs.SetResult(true);
            });
            return tcs.Task;
        }

        public Task<TResult> TaskToMainThread<TResult>(Func<TResult> f)
        {
            var tcs = new TaskCompletionSource<TResult>();
            QueueActions.Enqueue(() => { tcs.SetResult(f()); });
            return tcs.Task;
        }

        /// <summary>
        ///     Execute the Action in the main thread after a delay.
        /// </summary>
        /// <param name="seconds">Execution delay, in seconds.</param>
        /// <param name="a">Action to be executed.</param>
        public static void ToMainThreadAfterDelay(double seconds, Action a)
        {
            Instance.ToMainThread(() => { LaunchCoroutine(Instance.ExecuteDelayed(seconds, a)); });
        }

        private IEnumerator ExecuteDelayed(double seconds, Action a)
        {
            yield return new WaitForSeconds((float) seconds);
            QueueActions.Enqueue(a);
        }

        /// <summary>
        ///     Executes the coroutine passed as parameter in the main thread.
        /// </summary>
        /// <param name="firstIterationResult">Coroutine to be executed.</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void LaunchCoroutine(IEnumerator firstIterationResult)
        {
            Instance.StartCoroutine(firstIterationResult);
        }
    }
}