using TMPro;
using UnityEngine.EventSystems;

namespace VENTUS.UI
{
    public class ExtendedDropdown : TMP_Dropdown
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsExpanded)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}
