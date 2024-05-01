using System;
using System.Collections.Generic;
using UnityEngine;

namespace VENTUS.Interaction.TransformTool.Core.Focus
{
    public enum SelectionEvent {OnInitialize, OnFinalize}
    
    public static class SelectionFocusSystem
    {
        #region Fields
        
        private static readonly Dictionary<Type, SelectionFocusContainer> Elements = new();
        
        #endregion
        
        #region Public Methods

        public static bool IsSelected(Type type, ISelectionFocusElement selectionFocusElement)
        {
            return Elements.TryGetValue(type, out SelectionFocusContainer selectionFocusContainer) && selectionFocusContainer.CurrentSelectionFocusElement == selectionFocusElement;
        }
        
        public static void ChangeSelection(Type type, ISelectionFocusElement selectionFocusElement)
        {
            if (!Elements.ContainsKey(type))
            {
                Elements.TryAdd(type, new SelectionFocusContainer());
            }

            Elements[type].ChangeSelection(selectionFocusElement);
        }
        
        public static void ExitSelection(Type type)
        {
            if (!Elements.ContainsKey(type))
            {
                return;
            }

            Elements[type].Restore();
            Elements.Remove(type);
        }

        public static void RegisterListener(Type type, SelectionEvent selectionEvent, Action<ISelectionFocusElement> action)
        {
            switch (selectionEvent)
            {
                case SelectionEvent.OnInitialize:
                    Elements[type].OnInitialize += action;
                    break;
                case SelectionEvent.OnFinalize:
                    Elements[type].OnFinalize += action;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(selectionEvent), selectionEvent, null);
            }
        }
        
        public static void UnregisterListener(Type type, SelectionEvent selectionEvent, Action<ISelectionFocusElement> action)
        {
            switch (selectionEvent)
            {
                case SelectionEvent.OnInitialize:
                    Elements[type].OnInitialize -= action;
                    break;
                case SelectionEvent.OnFinalize:
                    Elements[type].OnFinalize -= action;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(selectionEvent), selectionEvent, null);
            }
        }
        
        #endregion
    }
}
