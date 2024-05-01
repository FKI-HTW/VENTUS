using UnityEngine;

namespace VENTUS.UI.Glossary
{
    public class GlossaryEvent : MonoBehaviour
    {
        [SerializeField] private EntryType _glossaryType;
        public void SetGlossaryEntry()
        {
            GlossaryManager.SetCurrentEntry(_glossaryType);
        }
    }
}
