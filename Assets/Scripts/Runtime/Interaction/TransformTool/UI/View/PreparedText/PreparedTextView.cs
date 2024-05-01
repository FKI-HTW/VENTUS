using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VENTUS.Interaction.TransformTool.Core.Focus;
using VENTUS.Interaction.TransformTool.UI.View.Annotation;

namespace VENTUS.Interaction.TransformTool.UI.View.PreparedText
{
    public class PreparedTextView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private List<PreparedTextGroup> _exampleTextViewGroups;
        [SerializeField] private PreparedTextViewElement _preparedTextViewElementPrefab;
        [SerializeField] private Transform _instantiationParent;

        public event Action<string> OnSelectTextElement;

        private readonly Dictionary<PreparedTextViewElement, List<PreparedTextViewElement>> _elements = new();
        private PreparedTextViewElement _openedGroup;
        private string _baseTitle;

        private void Awake()
        {
            _baseTitle = _title.text;
        }

        private void Start()
        {
            foreach (var exampleTextViewGroup in _exampleTextViewGroups)
            {
                List<PreparedTextViewElement> elements = new List<PreparedTextViewElement>();
                foreach (var exampleTextElement in exampleTextViewGroup.GetItems())
                {
                    PreparedTextViewElement element = Instantiate(_preparedTextViewElementPrefab, _instantiationParent);
                    element.gameObject.SetActive(false);
                    element.SetText(exampleTextElement.Get());
                    element.RegisterOnClickAction(() => ElementOnClick(exampleTextElement.Get()));
                    elements.Add(element);
                }

                PreparedTextViewElement group = Instantiate(_preparedTextViewElementPrefab, _instantiationParent);
                group.SetText(exampleTextViewGroup.GroupName);
                group.RegisterOnClickAction(() => GroupOnClick(group));
                _elements.Add(group, elements);
            }
        }

        private void ElementOnClick(string element)
        {
            OnSelectTextElement?.Invoke(element);
        }

        private void GroupOnClick(PreparedTextViewElement selectedGroup)
        {
            foreach (var (group, elements) in _elements)
            {
                group.gameObject.SetActive(false);
                
                if (group == selectedGroup)
                {
                    _title.text = group.Text;
                    _openedGroup = group;
                    
                    foreach (var element in elements)
                    {
                        element.gameObject.SetActive(true);
                    }
                }
            }
        }
    
        public virtual void NavigateBack()
        {
            if (_openedGroup)
            {
                if (_elements.TryGetValue(_openedGroup, out List<PreparedTextViewElement> elements))
                {
                    foreach (var element in elements)
                    {
                        element.gameObject.SetActive(false);
                    }
                }

                _title.text = _baseTitle;

                foreach (var element in _elements)
                {
                    element.Key.gameObject.SetActive(true);
                }

                _openedGroup = null;
            }
            else
            {
                SelectionFocusSystem.ExitSelection(typeof(BasePreparedTextSpawner));
            }
        }

        public void Close()
        {
            SelectionFocusSystem.ExitSelection(typeof(BasePreparedTextSpawner));
        }
    }
}
