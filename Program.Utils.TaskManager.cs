using System;
using System.Collections;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        interface ITask
        {
            ITask Every(float seconds);
            ITask Pause(bool pause = true);
            bool Paused {
                get;
            }
            ITask Once();
            void Restart();
            T Result<T>();
            ITask Then<T>(Action<T> callback);
            ITask Then(Action callback);
        }
        class Task : ITask
        {
            IEnumerator Enumerator;
            IEnumerable Ref;
            TimeSpan Interval;
            TimeSpan TimeSinceLastRun;
            object TaskResult;
            bool IsPaused;
            bool IsOnce;
            Action onDone;

            bool ITask.Paused => IsPaused;

            ITask ITask.Every(float seconds) {
                Interval = TimeSpan.FromSeconds(seconds);
                return this;
            }
            ITask ITask.Pause(bool pause) {
                IsPaused = pause;
                return this;
            }

            ITask ITask.Once() {
                IsOnce = true;
                return this;
            }

            void ITask.Restart() {
                Enumerator = Ref.GetEnumerator();
                TimeSinceLastRun = TimeSpan.Zero;
                TaskResult = null;
            }
            T ITask.Result<T>() {
                if (TaskResult == null)
                    return default(T);
                return (T)TaskResult;
            }
            ITask ITask.Then(Action callback) {
                onDone += callback;
                return this;
            }
            ITask ITask.Then<T>(Action<T> callback) {
                onDone += () => callback(((ITask)this).Result<T>());
                return this;
            }
            static List<Task> tasks = new List<Task>();

            public static ITask RunTask(IEnumerable task) {
                var newTask = new Task {
                    Ref = task,
                    Enumerator = task.GetEnumerator(),
                    Interval = TimeSpan.FromSeconds(0),
                    TimeSinceLastRun = TimeSpan.Zero,
                    TaskResult = null,
                    IsPaused = false,
                    IsOnce = false
                };
                tasks.Add(newTask);
                return newTask;
            }

            static IEnumerable InternalTask(Action cb, bool timeout = false) {
                if (timeout) {
                    cb();
                    yield break;
                }
                while (true) {
                    cb();
                    yield return null;
                }
            }
            public static ITask SetInterval(Action cb, float intervalSeconds) =>
                RunTask(InternalTask(cb)).Every(intervalSeconds);

            public static ITask SetTimeout(Action cb, float delaySeconds) =>
                RunTask(InternalTask(cb, true)).Once().Every(delaySeconds);

            public static void StopTask(ITask task = null) {
                var t = task ?? CurrentTask;
                tasks.Remove((Task)t);
                ((Task)t).onDone?.Invoke();
            }

            public static bool IsRunning(ITask task) {
                return tasks.Contains((Task)task) && !task.Paused;
            }

            public static TimeSpan CurrentTaskLastRun;
            public static ITask CurrentTask;
            public static void Tick(TimeSpan TimeSinceLastRun) {
                for (int i = tasks.Count - 1; i >= 0; i--) {
                    var task = tasks[i];
                    CurrentTask = task;
                    if (task.IsPaused)
                        continue;

                    task.TaskResult = null;

                    task.TimeSinceLastRun += TimeSinceLastRun;
                    if (task.TimeSinceLastRun < task.Interval)
                        continue;

                    CurrentTaskLastRun = task.TimeSinceLastRun;
                    try {
                        if (!task.Enumerator.MoveNext()) {
                            if (task.IsOnce) {
                                tasks.RemoveAt(i);
                                task.onDone?.Invoke();
                                continue;
                            }
                            task.Enumerator = task.Ref.GetEnumerator();
                        }
                    }
                    catch (Exception e) {
                        Util.Echo(e.ToString());
                    }
                    task.TimeSinceLastRun = TimeSpan.Zero;
                    task.TaskResult = task.Enumerator.Current;
                }
            }
        }
    }
}
