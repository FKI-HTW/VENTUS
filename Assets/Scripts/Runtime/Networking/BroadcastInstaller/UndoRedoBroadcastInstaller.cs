using System;
using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using Unity.VisualScripting;
using UnityEngine;
using VENTUS.DataStructures.CommandLogic;
using VENTUS.UI;

namespace VENTUS.Networking.BroadcastInstaller
{
    public class UndoRedoBroadcastInstaller : MonoBehaviour
    {
        [SerializeField] private ErrorPopupView _errorPopupViewPrefab;

        private ErrorPopupView _instantiatedPopupView;

        #region Broadcast Call

        public static void BroadcastUndoRedoToServer(CommandType commandType)
        {
            UndoRedoBroadcast undoRedoBroadcast = new UndoRedoBroadcast()
            {
                commandType = commandType,
                origin = InstanceFinder.ClientManager.Connection
            };
            InstanceFinder.ClientManager.Broadcast(undoRedoBroadcast);
        }
        
        private static void BroadcastErrorMessageToClient(NetworkConnection origin, string errorMessage)
        {
            FailedCommandBroadcast failedCommandBroadcast = new FailedCommandBroadcast()
            {
                target = origin,
                message = errorMessage
            };
            InstanceFinder.ServerManager.Broadcast(failedCommandBroadcast);
        }

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            InstanceFinder.ServerManager.RegisterBroadcast<UndoRedoBroadcast>(OnUndoRedoBroadcast_ServerListenClient);
            InstanceFinder.ClientManager.RegisterBroadcast<FailedCommandBroadcast>(OnUndoRedoBroadcast_ClientListenServer);
        }

        private void OnDisable()
        {
            InstanceFinder.ServerManager.UnregisterBroadcast<UndoRedoBroadcast>(OnUndoRedoBroadcast_ServerListenClient);
            InstanceFinder.ClientManager.RegisterBroadcast<FailedCommandBroadcast>(OnUndoRedoBroadcast_ClientListenServer);
        }

        #endregion

        #region Private Methods

        private void OnUndoRedoBroadcast_ServerListenClient(NetworkConnection networkConnection, UndoRedoBroadcast undoRedoBroadcast)
        {
            string errorMessage = "";
            switch (undoRedoBroadcast.commandType)
            {
                case CommandType.Undo:
                    if (!CommandHandler.Undo(networkConnection, out errorMessage))
                    {
                        BroadcastErrorMessageToClient(undoRedoBroadcast.origin, errorMessage);
                    }
                    break;
                case CommandType.Redo:
                    if (!CommandHandler.Redo(networkConnection, out errorMessage))
                    {
                        BroadcastErrorMessageToClient(undoRedoBroadcast.origin, errorMessage);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnUndoRedoBroadcast_ClientListenServer(FailedCommandBroadcast undoRedoBroadcast)
        {
            if (InstanceFinder.ClientManager.Connection == undoRedoBroadcast.target)
            {
                if (_instantiatedPopupView == null || _instantiatedPopupView.IsDestroyed())
                {
                    _instantiatedPopupView = Instantiate(_errorPopupViewPrefab);
                }

                _instantiatedPopupView.OpenErrorPopupView(undoRedoBroadcast.message);
            }
        }

        #endregion
        
        #region Data Types
        
        public enum CommandType { Undo, Redo }

        private struct UndoRedoBroadcast : IBroadcast
        {
            public NetworkConnection origin;
            public CommandType commandType;
        }

        private struct FailedCommandBroadcast : IBroadcast
        {
            public NetworkConnection target;
            public string message;
        }
        
        #endregion
    }
}
