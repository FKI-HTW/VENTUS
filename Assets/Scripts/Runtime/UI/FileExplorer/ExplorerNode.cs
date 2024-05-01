using CENTIS.UnityFileExplorer;
using UnityEngine;
using TMPro;

namespace VENTUS.UI.FileExplorer
{
	public class ExplorerNode : UINode
	{
		[SerializeField] private TMP_Text _name;
		[SerializeField] private TMP_Text _modifiedAt;

		public override void Initialize(NodeInformation info)
		{
			_name.text = gameObject.name = info.Name;
			_modifiedAt.text = info.Type == ENodeType.Drive
				? string.Empty
				: $"{info.UpdatedAt.ToLocalTime():d/M/yyyy HH:mm:ss}";
		}

		public override void OnFailedToLoad(ENodeFailedToLoad reason)
		{
			Debug.LogError($"Failed to load node because: {reason}");
		}
	}
}
