using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VENTUS.UI.Glossary
{
    public class GlossaryEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _button;

        public void SetUIEntry(EntryType key, GlossaryEntry entry)
        {
            _title.text = entry.Title;
            _button.onClick.AddListener(() => GlossaryManager.SetCurrentEntry(key));
        }
    }
}
