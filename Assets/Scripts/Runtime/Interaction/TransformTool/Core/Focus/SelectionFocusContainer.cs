using System;
using System.Collections.Generic;

namespace VENTUS.Interaction.TransformTool.Core.Focus
{
    public class SelectionFocusContainer
    {
        #region Fields
        public event Action<ISelectionFocusElement> OnInitialize;
        public event Action<ISelectionFocusElement> OnFinalize;
    
        public ISelectionFocusElement CurrentSelectionFocusElement { get; private set; }
        #endregion

        #region Public Methods
        
        public void ChangeSelection(ISelectionFocusElement selectionFocusElement)
        {
            if (CurrentSelectionFocusElement != selectionFocusElement)
            {
                Apply(selectionFocusElement);
                return;
            }

            Restore();
        }

        private void Apply(ISelectionFocusElement newSelectionFocusElement)
        {
            if (CurrentSelectionFocusElement != null)
            {
                CurrentSelectionFocusElement.OnRestoring();
            }
            else
            {
                OnInitialize?.Invoke(newSelectionFocusElement);
            }

            ISelectionFocusElement previousSelectionFocusElement = CurrentSelectionFocusElement;
            CurrentSelectionFocusElement = newSelectionFocusElement;
            newSelectionFocusElement.OnApplying(previousSelectionFocusElement);
        }

        public void Restore()
        {
            if (CurrentSelectionFocusElement == null) return;
            
            CurrentSelectionFocusElement.OnRestoring();
            OnFinalize?.Invoke(CurrentSelectionFocusElement);
            CurrentSelectionFocusElement = null;
        }
        #endregion
    }
}
