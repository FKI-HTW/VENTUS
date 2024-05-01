using CENTIS.UnityFileExplorer;
using Unity.Mathematics;
using UnityEngine;

namespace VENTUS.UI.FileExplorer
{
    public class ShowPathFolderNodes : MonoBehaviour
    {
        [SerializeField] private ExplorerManager _manager;
        private float _pathFolderWidth;
        private RectTransform _transform;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            _manager.FolderPathUpdated += AdjustPathContainerOffset;
            Invoke(nameof(AdjustPathContainerOffset), 0.1f);
        }

        private void AdjustPathContainerOffset()
        {
            float childSize = 0;
            for (var i = 0; i < transform.childCount; i++)
                childSize += transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;

            var containerSize = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
            var newSize = math.min(containerSize - childSize, 0);
            _transform.offsetMin = new(newSize, _transform.offsetMin.y);
        }
    }
}