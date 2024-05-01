using VENTUS.DataStructures.CommandLogic.Disposable;

namespace VENTUS.DataStructures.CommandLogic
{
    public interface IMemento : ICommandDisposable
    {
        void Execute();
        void Undo();
    }
}
