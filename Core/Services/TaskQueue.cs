﻿using System.Collections.Concurrent;

namespace Core.Services
{
    public class TaskQueue
    {
        private ConcurrentQueue<Task> _startQueue;
        private ConcurrentQueue<Task> _waitQueue;
        private readonly SemaphoreSlim _semaphore;
        private readonly CancellationToken _token;
        private readonly Task _startNext;
        private readonly Task _waitNext;

        public TaskQueue(ushort maxThreadCount, CancellationTokenSource tokenSource)
        {
            _startQueue = new ConcurrentQueue<Task>();
            _waitQueue = new ConcurrentQueue<Task>();
            _semaphore = new SemaphoreSlim(maxThreadCount);
            _token = tokenSource.Token;
            _startNext = new Task(() => StartNext(), _token);
            _waitNext = new Task(() => WaitNext(), _token);
        }

        public void Enqueue(Task task)
        {
            _startQueue.Enqueue(task);
            _waitQueue.Enqueue(task);
        }

        public void StartAndWaitAll()
        {
            _startNext.Start();
            _waitNext.Start();
            try
            {
                _startNext.Wait(_token);
                _waitNext.Wait(_token);
            }
            catch(OperationCanceledException)
            {
                return;
            }
        }

        private void StartNext()
        {
            Task? task;
            while(!_waitNext.IsCompleted && !_token.IsCancellationRequested)
            {
                bool result = _startQueue.TryDequeue(out task);
                if (result && task != null)
                {
                    try
                    {
                        _semaphore.Wait(_token);
                        task.Start();
                    }
                    catch(OperationCanceledException)
                    {
                        return;
                    }
                }           
            }
        }

        private void WaitNext()
        {
            Task? task;
            while (!_waitQueue.IsEmpty && !_token.IsCancellationRequested)
            {
                bool result = _waitQueue.TryDequeue(out task);
                if (result && task != null)
                {
                    try
                    {
                        task.Wait(_token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
            }
        }
    }
}
