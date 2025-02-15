using System;
using System.Collections.Generic;

namespace Zerobject.SceneManagement.Runtime
{
    public class TaskQueueHandler
    {
        private readonly Queue<Action> _queue = new();
        private bool _inProgress = false;

        public void EnqueueAndRun(Action action)
        {
            _queue.Enqueue(action);
            if (!_inProgress) CompleteCurrentTask();
        }
        public void CompleteCurrentTask()
        {
            _inProgress = _queue.TryDequeue(out var next);
            if (_inProgress) next();
        }
    }
}
