using CENTIS.UnityFileExplorer;
using UnityEngine;
using TMPro;

namespace VENTUS.UI.FileExplorer
{
	public class PathFolder : UIPathFolder
	{
		[SerializeField] private TMP_Text _pathText;

		public override void Initialize(string folderName)
		{
			_pathText.text = folderName;
			gameObject.name = folderName;
		}
	}
}
