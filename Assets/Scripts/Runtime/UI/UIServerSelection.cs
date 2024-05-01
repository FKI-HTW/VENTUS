using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using VENTUS.Controlling;
using VENTUS.Networking;
using VENTUS.UI.Glossary;
using VENTUS.UI.Input;

namespace VENTUS.UI
{
    public class UIServerSelection : MonoBehaviour
    {
		#region attributes

		private const int MIN_NUMBER_CLIENTS = 1;
        private const int MAX_NUMBER_CLIENTS = 100;
        private const string SERVER_ID_PATTERN = @"^[6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw]{6,12}$";

        private NetworkManager _networkManager;
        private KeyboardManager _keyboardManager;

        [Header("Welcome Window")]
        [SerializeField] private GameObject _welcomeWindow;

		[Header("User Window")]
		[SerializeField] private GameObject _userWindow;
        [SerializeField] private TMP_InputField _userNameInputField;
        [SerializeField] private TextMeshProUGUI _userNameInfo;
		[SerializeField] private TextMeshProUGUI _hairStyleTitle;
		[SerializeField] private SkinnedMeshRenderer _avatarHair;
		[SerializeField] private SkinnedMeshRenderer _avatarBrow;
		[SerializeField] private SkinnedMeshRenderer _avatarShirt;
        [SerializeField] private TextMeshProUGUI _userWindowInfo;
        [SerializeField] private Button _selectCreateServerButton;
        [SerializeField] private Button _selectJoinSeverButton;
        private string _username;
		private Color _mainColor = DefaultValues.COLOR;
		private Color _hairColor = DefaultValues.HAIR_COLOR;
		private EHairCustomization[] _hairCustomizations;
		private int _hairIndex;

        [Header("Create Server Window")]
        [SerializeField] private GameObject _createServerWindow;
        [SerializeField] private TMP_InputField _maxClientsInputField;
        [SerializeField] private TMP_Dropdown _allocationRegionsDropdown;
        [SerializeField] private Button _createServerButton;
        [SerializeField] private TextMeshProUGUI _errorMessageCreateServer;
        private int _maxClients = 5;

        [Header("Join Server Window")]
        [SerializeField] private GameObject _joinServerWindow;
        [SerializeField] private TMP_InputField _joinServerIDInputField;
        [SerializeField] private Button _joinServerButton;
        [SerializeField] private TextMeshProUGUI _errorMessageJoinServer;
        private string _joinServerID;

		#endregion

		#region lifecycle

		private void Awake()
        {
            _networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
            _keyboardManager = GameObject.FindGameObjectWithTag("KeyboardManager").GetComponent<KeyboardManager>();

            ChangeMainColor(_mainColor);
            ChangeHairColor(_hairColor);
			_hairCustomizations = (EHairCustomization[])Enum.GetValues(typeof(EHairCustomization));
			_hairStyleTitle.SetText(_hairCustomizations[_hairIndex].ToString());
        }

		private void ExitApplication()
		{
			Application.Quit();
		}

		#endregion
		
		#region glossary

		public void OpenUserGlossary()
		{
			gameObject.SetActive(false);
			GlossaryManager.SetCurrentEntry(EntryType.AvatarCreation);
		}

		public void OpenCreateServerGlossary()
		{
			gameObject.SetActive(false);
			GlossaryManager.SetCurrentEntry(EntryType.CreateServer);
		}

		public void OpenJoinServerGlossary()
		{
			gameObject.SetActive(false);
			GlossaryManager.SetCurrentEntry(EntryType.JoinServer);
		}
		
		#endregion

		#region user window

		public void ShowWelcomeWindow()
		{
			_welcomeWindow.SetActive(true);
			_userWindow.SetActive(false);
			_joinServerWindow.SetActive(false);
			_createServerWindow.SetActive(false);
		}

		public void ShowUserWindow()
		{
			_welcomeWindow.SetActive(false);
			_userWindow.SetActive(true);
			_joinServerWindow.SetActive(false);
			_createServerWindow.SetActive(false);
			_userNameInfo.text = string.Empty;
			_userWindowInfo.text = string.Empty;
		}

		public void ChangeHairstyle(bool isRight)
		{
			_hairIndex += isRight ? 1 : -1;
			if (_hairIndex < 0)
				_hairIndex = _hairCustomizations.Length - 1;
			if (_hairIndex > _hairCustomizations.Length - 1)
				_hairIndex = 0;
			_hairStyleTitle.SetText(_hairCustomizations[_hairIndex].ToString());
			_avatarHair.sharedMesh = _networkManager.GetHairMesh(_hairCustomizations[_hairIndex]);
		}

		public void ChangeHairColor(Color color)
		{
			_avatarHair.material.color = _avatarBrow.material.color = _hairColor = color;
		}

		public void ChangeMainColor(Color color)
		{
			_avatarShirt.material.color = _mainColor = color;
		}

		public void StartUsernameInput()
        {
	        if (SceneManager.IsInXRMode)
	        {
				_keyboardManager.GetKeyboardInput(
					KeyboardManager.KeyboardTypes.SmallWithNumbersAndArrows,
					connectedInputField: _userNameInputField);
	        }
		}

		public void CheckUsernameValidity(string input)
		{
            if (input.Length < 1)
            {
				_userNameInfo.text = "Please choose a username to continue!";
                _selectJoinSeverButton.interactable = false;
                _selectCreateServerButton.interactable = false;
				return;
            }

			_username = input;
			_userNameInfo.text = "";
			_selectJoinSeverButton.interactable = true;
			_selectCreateServerButton.interactable = true;
		}

		#endregion
		
		#region create server window
		
		public async void ShowCreateServerWindow()
		{
			try
			{
				if (!await _networkManager.InitializeNetwork() || !await _networkManager.GetRegions())
				{
					_userWindowInfo.text = "An error occurred. Try again later!";
					return;
				}
			}
			catch (Exception e)
			{
				_userWindowInfo.text = e switch
				{
					ServicesInitializationException =>
						"An error occurred while initializing the service. Try again later!",
					AuthenticationException => 
						"An error occurred while authenticating. Try again later!",
					RequestFailedException =>
						"An error occurred while authenticating. Contact the developers or try again later!",
					_ => "An error occurred. Try again later!"
				};
				
				Debug.LogError(e.Message);
				return;
			}
			
			var dropdownValue = 0;
			List<string> optionListToDisplay = new();
			for (var i = 0; i < _networkManager.AllocationRegions.Count; i++)
			{
				var region = _networkManager.AllocationRegions[i];
				optionListToDisplay.Add(region.Description);
				if (region.Id.Equals(_networkManager.CurrentAllocationRegion))
					dropdownValue = i;
			}
			_allocationRegionsDropdown.ClearOptions();
			_allocationRegionsDropdown.AddOptions(optionListToDisplay);
			_allocationRegionsDropdown.value = dropdownValue;

			_userWindowInfo.text = string.Empty;
			_errorMessageCreateServer.text = string.Empty;
			
			_welcomeWindow.SetActive(false);
			_userWindow.SetActive(false);
			_createServerWindow.SetActive(true);
			_joinServerWindow.SetActive(false);
		}

		public void StartMaxClientsInput()
		{
			if (SceneManager.IsInXRMode)
			{
				_keyboardManager.GetKeyboardInput(
					KeyboardManager.KeyboardTypes.Numbers,
					connectedInputField: _maxClientsInputField);
			}
		}

		public void CheckMaxClientsValidity(string input)
		{
			var isValid = false;
			try
			{
				_maxClients = int.Parse(input);
				isValid = _maxClients is >= MIN_NUMBER_CLIENTS and <= MAX_NUMBER_CLIENTS;
			}
			catch (Exception) { }

			if (!isValid)
			{
				_errorMessageCreateServer.text = $"Please choose a valid number between {MIN_NUMBER_CLIENTS} and {MAX_NUMBER_CLIENTS}!";
				return;
			}

			_errorMessageCreateServer.text = string.Empty;
			_createServerButton.interactable = true;
		}

		public async void OnCreateServerPressed()
		{
			_networkManager.Username = _username;
			_networkManager.MaxPlayers = _maxClients;
			_networkManager.Color = _mainColor;
			_networkManager.HairColor = _hairColor;
			_networkManager.HairCustomization = _hairCustomizations[_hairIndex];

			try
			{
				var selectedOption = _allocationRegionsDropdown.value;
				_networkManager.CurrentAllocationRegion = _networkManager.AllocationRegions[selectedOption].Id;

				if (!await _networkManager.StartHost())
				{
					_errorMessageCreateServer.text = "Could not create session. Please check your internet connection and try again!";
					return;
				}
			}
			catch (Exception e)
			{
				_errorMessageCreateServer.text = "Could not create session. Please check your internet connection and try again!";
				Debug.LogError(e.Message);
				return;
			}
			
			_errorMessageCreateServer.text = string.Empty;
		}

		#endregion
		
		#region join server window

		public async void ShowJoinServerWindow()
		{
			try
			{
				if (!await _networkManager.InitializeNetwork())
				{
					_userWindowInfo.text = "An error occurred. Try again later!";
					return;
				}
			}
			catch (Exception e)
			{
				_userWindowInfo.text = e switch
				{
					ServicesInitializationException =>
						"An error occurred while initializing the service. Try again later!",
					AuthenticationException => 
						"An error occurred while authenticating. Try again later!",
					RequestFailedException =>
						"An error occurred while authenticating. Contact the developers or try again later!",
					_ => "An error occurred. Try again later!"
				};
				
				Debug.LogError(e.Message);
				return;
			}
			
			_userWindowInfo.text = string.Empty;
			_errorMessageJoinServer.text = string.Empty;
			
			_welcomeWindow.SetActive(false);
			_userWindow.SetActive(false);
			_createServerWindow.SetActive(false);
			_joinServerWindow.SetActive(true);
		}

		public void StartServerIDInput()
		{
			if (SceneManager.IsInXRMode)
			{
				_keyboardManager.GetKeyboardInput(
					KeyboardManager.KeyboardTypes.SmallWithNumbers,
					connectedInputField: _joinServerIDInputField);
			}
		}

		public void PasteServerId()
		{
			_joinServerIDInputField.text = GUIUtility.systemCopyBuffer;
			CheckServerIDValidity(_joinServerIDInputField.text);
		}

		public void CheckServerIDValidity(string input)
		{
			if (string.IsNullOrEmpty(input) || !Regex.IsMatch(input, SERVER_ID_PATTERN))
			{
				_joinServerButton.interactable = false;
				_errorMessageJoinServer.text = "Your input is no valid join code.\nPlease verify that you used the pattern above!";
				return;
			}

			_joinServerID = _joinServerIDInputField.text;
			_joinServerButton.interactable = true;
			_errorMessageJoinServer.text = string.Empty;
		}

		public async void OnJoinServerPressed()
		{
			_networkManager.Username = _username;
			_networkManager.MaxPlayers = _maxClients;
			_networkManager.Color = _mainColor;
			_networkManager.HairColor = _hairColor;
			_networkManager.HairCustomization = _hairCustomizations[_hairIndex];

			try
			{
				if (!await _networkManager.JoinAsClient(_joinServerID))
				{
					_errorMessageJoinServer.text = "Could not join session. Please check your internet connection and try again!";
					return;
				}
			}
			catch (Exception e)
			{
				_errorMessageJoinServer.text = "Could not join session. Please check your internet connection and try again!";
				Debug.LogError(e.Message);
				return;
			}
			
			_errorMessageJoinServer.text = string.Empty;
		}

		#endregion
	}
}
