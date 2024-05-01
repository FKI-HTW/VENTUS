using FishNet;
using UnityEngine;
using VENTUS.DataStructures.CommandLogic;
using VENTUS.Networking.BroadcastInstaller;

namespace VENTUS.Interaction.TransformTool.UI.Clickable
{
    public class ModelDestroyClickable : InformationClickable
    {
        public GameObject ObjectToDestroy { get; set; }
        
        protected override void OnAcceptButton()
        {
            SelectionViewStackMachine.ClearStack(() =>
            {
                CommandNetworkObject[] commandObjects = ObjectToDestroy.GetComponentsInChildren<CommandNetworkObject>();
                foreach (CommandNetworkObject commandNetworkObject in commandObjects)
                {
                    commandNetworkObject.SetCommandOwnership(InstanceFinder.ClientManager.Connection);
                }
                
                GameObject[] objectsToDestroy = { ObjectToDestroy != null ? ObjectToDestroy : SelectableGo };
                ActiveStateBroadcastInstaller.BroadcastActiveStateToServer(objectsToDestroy, false, true);
            });
        }

        protected override void OnDeclineButton()
        {
            SelectionViewStackMachine.PopStackElement();
        }
    }
}
