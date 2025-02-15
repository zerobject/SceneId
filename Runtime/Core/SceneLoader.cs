using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zerobject.SceneManagement.Runtime
{
    public static class SceneLoader
    {
        public static Action<SceneId> OnLoadingStart;
        public static Action<SceneId> OnLoadingEnd;
        public static Action<SceneId> OnUnloadingStart;
        public static Action<SceneId> OnUnloadingEnd;
        public static AsyncOperation HandleOperation { get; private set; }
        public static SceneId HandlingSceneId { get; private set; } = SceneId.Unknown;

        private static TaskQueueHandler _taskHandler;

        public static void Load(SceneId id, LoadSceneMode mode = LoadSceneMode.Single, bool async = false)
        {
            if (id == SceneId.Unknown)
            {
                Debug.LogError($"Невозможно загрузить сцену с идентификатором {id}");
                return;
            }

            Debug.Log($"Загружается сцена {id}...");

            if (_taskHandler == null)
            {
                _taskHandler = new();
                SceneManager.sceneLoaded += LoadComplete;
            }

            _taskHandler.EnqueueAndRun(() => PerformLoading(id, mode, async));
        }
        public static void Unload(SceneId id)
        {
            if (id == SceneId.Unknown)
            {
                Debug.LogError($"Невозможно выгрузить сцену с идентификатором {id}.");
                return;
            }

            Debug.Log($"Выгружается сцена {id}...");

            if (_taskHandler == null)
            {
                _taskHandler = new();
                SceneManager.sceneUnloaded += UnloadComplete;
            }

            _taskHandler.EnqueueAndRun(() => PerformUnloading(id));
        }

        private static void PerformLoading(SceneId id, LoadSceneMode mode, bool async)
        {
            if (id == SceneId.Unknown)
            {
                Debug.LogError("Произошла попытка загрузить сцену неизвестного типа.");
                return;
            }
            if (string.IsNullOrEmpty(id.Name()))
            {
                Debug.LogError($"Сцена с идентификатором <b>{id}</b> не найдена.");
                return;
            }

            HandlingSceneId = id;
            OnLoadingStart?.Invoke(id);

            if (async) HandleOperation = SceneManager.LoadSceneAsync(id.Name(), mode);
            else SceneManager.LoadScene(id.Name(), mode);
        }
        private static void PerformUnloading(SceneId id)
        {
            if (id == SceneId.Unknown)
            {
                Debug.LogError("Произошла попытка выгрузить сцену неизвестного типа.");
                return;
            }
            if (string.IsNullOrEmpty(id.Name()))
            {
                Debug.LogError($"Сцена с идентификатором {id} не найдена.");
                return;
            }

            HandlingSceneId = id;
            OnUnloadingStart?.Invoke(id);
            HandleOperation = SceneManager.UnloadSceneAsync(id.Name());
        }

        private static void LoadComplete(Scene scene, LoadSceneMode _)
        {
            SceneId id = scene.name.ToId();
            OnLoadingEnd?.Invoke(id);

            if (HandlingSceneId != SceneId.Unknown && id == HandlingSceneId)
            {
                HandleOperation = null;
                HandlingSceneId = SceneId.Unknown;
                _taskHandler.CompleteCurrentTask();
            }
        }
        private static void UnloadComplete(Scene scene)
        {
            SceneId id = scene.name.ToId();
            OnUnloadingEnd?.Invoke(id);

            if (HandlingSceneId != SceneId.Unknown && id == HandlingSceneId)
            {
                HandleOperation = null;
                HandlingSceneId = SceneId.Unknown;
                _taskHandler.CompleteCurrentTask();
            }
        }
    }
}
