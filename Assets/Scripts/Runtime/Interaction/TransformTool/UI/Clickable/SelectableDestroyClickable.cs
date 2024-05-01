using UnityEngine;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.Interaction.TransformTool.UI.Clickable
{
    public class SelectableDestroyClickable : InformationClickable
    {
        protected override void OnAcceptButton()
        {
            SelectionViewStackMachine.ClearStack(() =>
            {
                GameObject[] objectsToDestroy = { SelectableGo };
                ActiveStateBroadcastInstaller.BroadcastActiveStateToServer(objectsToDestroy, false, true);
            });
        }

        protected override void OnDeclineButton()
        {
            SelectionViewStackMachine.PopStackElement();
        }
    }
}
