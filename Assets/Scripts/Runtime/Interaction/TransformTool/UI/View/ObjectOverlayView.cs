using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.View;

namespace VENTUS.Interaction.TransformTool.UI.View
{
    public abstract class ObjectOverlayView : BaseSelectionView
    {
        #region Fields
        
        [SerializeField] private BaseSelectionContextGroup _additionalSelectionPrefabs;

        protected abstract Transform InstantiationParent { get; }
    
        protected List<BaseSelectionElement> instantiatedSelectionOptions;
        private Action _onDestroyAction;
        
        #endregion

        protected override void InternalInitialize()
        {
            base.InternalInitialize();
            
            SelectionViewStackMachine.PushStackElement(this);
        }

        #region Unity Lifecycle
        
        private void LateUpdate()
        {
            SetPosition();
            SetRotation();
        }
        
        #endregion
        
        #region Inheritance
        
        public override IEnumerator OnEnter()
        {
            InstantiateSelectionOptions();
            yield return base.OnEnter();
        }

        public override IEnumerator OnExit()
        {
            yield return base.OnExit();
            DestroySelectionOptions();
        }

        public override void OnExitImmediate()
        {
            base.OnExitImmediate();
            DestroySelectionOptions();
        }

        protected virtual void InstantiateSelectionOptions()
        {
            instantiatedSelectionOptions = new List<BaseSelectionElement>();
            foreach (var selectableSelectionOptionPrefab in _additionalSelectionPrefabs.additionalSelectionPrefabs)
            {
                BaseSelectionElement baseSelectionElement = Instantiate(selectableSelectionOptionPrefab, InstantiationParent);
                baseSelectionElement.Initialize(InteractorGroup, SelectableGo, SelectionViewStackMachine);
                instantiatedSelectionOptions.Add(baseSelectionElement);
            }
        }
        
        #endregion
        
        #region Private Methods

        private void DestroySelectionOptions()
        {
            for (var index = instantiatedSelectionOptions.Count - 1; index >= 0; index--)
            {
                var instantiatedSelectionOption = instantiatedSelectionOptions[index];
                Destroy(instantiatedSelectionOption);
            }
        }
        
        #endregion
    }
}
