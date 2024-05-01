using UnityEngine;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.Interaction.Core.Interaction
{
    public class CommandView : MonoBehaviour
    {
        public void UndoAction()
        {
            UndoRedoBroadcastInstaller.BroadcastUndoRedoToServer(UndoRedoBroadcastInstaller.CommandType.Undo);
        }

        public void RedoAction()
        {
            UndoRedoBroadcastInstaller.BroadcastUndoRedoToServer(UndoRedoBroadcastInstaller.CommandType.Redo);
        }
    }
}
