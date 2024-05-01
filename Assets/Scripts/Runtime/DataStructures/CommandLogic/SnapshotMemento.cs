using System;
using FishNet.Connection;
using UnityEngine;
using VENTUS.DataStructures.CommandLogic.Disposable;
using VENTUS.DataStructures.CommandLogic.Snapshot;

namespace VENTUS.DataStructures.CommandLogic
{
    public readonly struct SnapshotMemento : IMemento
    {
        private readonly ICommandSnapshot[] _previousCommandSnapshotArray;
        private readonly ICommandSnapshot[] _currentCommandSnapshotArray;
        private readonly ICommandDisposable[] _commandDisposable;

        private SnapshotMemento(ICommandSnapshot[] previousCommandSnapshotArray, ICommandSnapshot[] currentCommandSnapshotArray, 
            ICommandDisposable[] commandDisposable = null)
        {
            _previousCommandSnapshotArray = previousCommandSnapshotArray;
            _currentCommandSnapshotArray = currentCommandSnapshotArray;
            _commandDisposable = commandDisposable ?? Array.Empty<ICommandDisposable>();
        }

        private SnapshotMemento(ICommandSnapshot previousCommandSnapshot, ICommandSnapshot currentCommandSnapshot, 
            ICommandDisposable commandDisposable = null)
        {
            _previousCommandSnapshotArray = new []{previousCommandSnapshot};
            _currentCommandSnapshotArray = new []{currentCommandSnapshot};
            _commandDisposable = commandDisposable != null ? new []{commandDisposable} : Array.Empty<ICommandDisposable>();
        }

        public static void PushToCommandHandler(NetworkConnection networkConnection, GameObject relatedObject, 
            ICommandSnapshot previousCommandSnapshot, ICommandSnapshot currentCommandSnapshot, ICommandDisposable commandDisposable = null)
        {
            if (!previousCommandSnapshot.Equals(currentCommandSnapshot))
            {
                CommandHandler.Push(networkConnection, new []{relatedObject}, new SnapshotMemento(previousCommandSnapshot, currentCommandSnapshot, commandDisposable));
            }
        }
        
        public static void PushToCommandHandler(NetworkConnection networkConnection, GameObject[] relatedObjects, 
            ICommandSnapshot[] previousCommandSnapshot, ICommandSnapshot[] currentCommandSnapshot, ICommandDisposable[] commandDisposable = null)
        {
            if (!previousCommandSnapshot.Equals(currentCommandSnapshot))
            {
                CommandHandler.Push(networkConnection, relatedObjects, new SnapshotMemento(previousCommandSnapshot, currentCommandSnapshot, commandDisposable));
            }
        }

        public void Execute()
        {
            foreach (var currentCommandSnapshot in _currentCommandSnapshotArray)
            {
                currentCommandSnapshot.ApplySnapshot();
            }
        }

        public void Undo()
        {
            foreach (var previousCommandSnapshot in _previousCommandSnapshotArray)
            {
                previousCommandSnapshot.ApplySnapshot();
            }
        }
        
        public void OnDispose()
        {
            if (_commandDisposable.Length == 0)
                return;
            
            foreach (var currentCommandSnapshot in _commandDisposable)
            {
                if (currentCommandSnapshot == null)
                {
                    Debug.LogWarning($"Snapshot Memento has an null element in it's Disposables! Please provide a valid Disposable!");
                    continue;
                }
                
                currentCommandSnapshot.OnDispose();
            }
        }
    }
}
