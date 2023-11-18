using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
	private static string playerNamePrefKey = "PlayerName";

	void Start()
	{
		string defaultName = "";
		InputField _inputField = GetComponent<InputField>();
		if (_inputField != null)
		{
			if (PlayerPrefs.HasKey(playerNamePrefKey))
			{
				defaultName = PlayerPrefs.GetString(playerNamePrefKey);
				_inputField.text = defaultName;
			}
		}
		PhotonNetwork.playerName = defaultName;
	}

	public void SetPlayerName(string value)
	{
		PhotonNetwork.playerName = value + " ";
		PlayerPrefs.SetString(playerNamePrefKey, value);
	}
}
