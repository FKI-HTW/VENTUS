using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using UnityEngine;

namespace VENTUS.DataStructures.CommandLogic
{
    public static class CommandHandler
    {
        private const uint CommandSize = 100;
        
        private static readonly Dictionary<NetworkConnection, CommandInstance> ConnectionCommandInstanceLookup = new();

        private static CommandInstance GetConnectionCommandHandlerInstance(NetworkConnection requestingConnection)
        {
            if (!ConnectionCommandInstanceLookup.TryGetValue(requestingConnection, out CommandInstance commandHandler))
            {
                commandHandler = new CommandInstance(requestingConnection, CommandSize);
                ConnectionCommandInstanceLookup.Add(requestingConnection, commandHandler);
            }

            return commandHandler;
        }

        public static List<CommandInstance> CommandInstances => ConnectionCommandInstanceLookup.Values.ToList();
        
        public static void Push(NetworkConnection requestingConnection, GameObject[] identifier, IMemento memento)
        {
            GetConnectionCommandHandlerInstance(requestingConnection).Push(identifier, memento);
        }

        public static bool Undo(NetworkConnection requestingConnection, out string errorMessage)
        {
            return GetConnectionCommandHandlerInstance(requestingConnection).UndoCommand(out errorMessage);
        }

        public static bool Redo(NetworkConnection requestingConnection, out string errorMessage)
        {
            return GetConnectionCommandHandlerInstance(requestingConnection).RedoCommand(out errorMessage);
        }
    }
    
    public class CommandInstance
    {
        public NetworkConnection Owner { get; }
        
        private readonly uint _commandSize;
        private readonly List<(GameObject[], IMemento)> _undoableStack = new();
        private readonly List<(GameObject[], IMemento)> _redoableStack = new();

        public CommandInstance(NetworkConnection networkConnection, uint commandSize)
        {
            Owner = networkConnection;
            _commandSize = commandSize;
        }

        public void Push(GameObject[] relatedIdentifiers, IMemento memento)
        {
            if (_undoableStack.Count >= _commandSize)
            {
                _undoableStack[0].Item2.OnDispose();
                _undoableStack.RemoveAt(0);
            }
            
            _undoableStack.Add((relatedIdentifiers, memento));
            
            foreach (var redoableMemento in _redoableStack)
            {
                redoableMemento.Item2.OnDispose();
            }
            
            _redoableStack.Clear();
        }

        public void RemoveCommandsByIdentifier(GameObject identifier)
        {
            RemoveCommands(identifier, _undoableStack);
            RemoveCommands(identifier, _redoableStack);
        }

        private void RemoveCommands(GameObject relatedIdentifiers, List<(GameObject[], IMemento)> listToClear)
        {
            for (var index = listToClear.Count - 1; index >= 0; index--)
            {
                var (elementIdentifier, _) = listToClear[index];
                foreach (var identifier in elementIdentifier)
                {
                    if (identifier == relatedIdentifiers)
                    {
                        listToClear.RemoveAt(index);
                    }
                }
            }
        }

        public bool UndoCommand(out string errorMessage)
        {
            errorMessage = String.Empty;

            if (_undoableStack.Count == 0)
            {
                errorMessage = "There is nothing to Undo!";
                return false;
            }

            (GameObject[], IMemento) peek = _undoableStack[^1];
            _undoableStack.RemoveAt(_undoableStack.Count - 1);
            _redoableStack.Add(peek);
            peek.Item2.Undo();

            return true;
        }

        public bool RedoCommand(out string errorMessage)
        {
            errorMessage = String.Empty;

            if (_redoableStack.Count == 0)
            {
                errorMessage = "There is nothing to Redo!";
                return false;
            }
            
            (GameObject[], IMemento) peek = _redoableStack[^1];
            _redoableStack.RemoveAt(_redoableStack.Count - 1);
            _undoableStack.Add(peek);
            peek.Item2.Execute();

            return true;
        }
    }
}