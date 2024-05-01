using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace VENTUS.UI.Glossary
{
    public class GlossaryUI : MonoBehaviour
    {
        [SerializeField] private GameObject _glossaryUI;
        [SerializeField] private Transform _entriesContainer;
        [SerializeField] private TMP_Text _entryTitle;
        [SerializeField] private TMP_Text _entryText;
        [SerializeField] private VideoPlayer _videoPlayer;

        [SerializeField] private GlossaryEntryUI _entryPrefab;

        private void Awake()
		{
            PopulateLinks();

            var entry = GlossaryManager.CurrentEntry;
			_entryTitle.text = entry.Title;
			_entryText.text = entry.Text;

			if (string.IsNullOrEmpty(entry.VideoUrl))
			{
				_videoPlayer.gameObject.SetActive(false);
			}
			else
			{
				_videoPlayer.gameObject.SetActive(true);
				_videoPlayer.url = $"{Application.streamingAssetsPath}/GlossaryVideos/{entry.VideoUrl}";
			}
        }

        private void OnEnable()
        {
            GlossaryManager.OnCurrentEntryUpdated += SetCurrentEntry;
        }

        private void OnDisable()
        {
            GlossaryManager.OnCurrentEntryUpdated -= SetCurrentEntry;
        }

        private void SetCurrentEntry(GlossaryEntry entry)
        {
            _glossaryUI.SetActive(true);
            _entryTitle.text = entry.Title;
            _entryText.text = entry.Text;
            
            if (string.IsNullOrEmpty(entry.VideoUrl)) {
                _videoPlayer.gameObject.SetActive(false);
            }
            else
            {
                _videoPlayer.gameObject.SetActive(true);
                _videoPlayer.url = $"{Application.streamingAssetsPath}/GlossaryVideos/{entry.VideoUrl}";
            }
        }

        private void PopulateLinks()
        {
            foreach (var (key, val) in GlossaryManager.Entries)
            {
                var entryUI = Instantiate(_entryPrefab, _entriesContainer);
                entryUI.SetUIEntry(key, val);
            }
        }
    }
}
