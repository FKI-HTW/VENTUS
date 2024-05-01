using UnityEngine;

namespace VENTUS.DataStructures.CommandLogic.Snapshot
{
    public interface ICommandSnapshot
    {
        GameObject RelatedObject { get; }
        
        void ApplySnapshot();
    }
}
