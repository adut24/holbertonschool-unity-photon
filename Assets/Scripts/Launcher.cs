using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
	#region Private Serializable Fields

	[Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
	[SerializeField]
	private byte maxPlayersPerRoom = 4;

	[Tooltip("The Ui Panel to let the user enter name, connect and play")]
	[SerializeField]
	private GameObject controlPanel;

	[Tooltip("The UI Label to inform the user that the connection is in progress")]
	[SerializeField]
	private GameObject progressLabel;

	#endregion

	#region Private Fields

	string gameVersion = "1";
	bool isConnecting;

	#endregion

	#region MonoBehaviour CallBacks

	void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	void Start()
	{
		progressLabel.SetActive(false);
		controlPanel.SetActive(true);
	}

	public override void OnConnectedToMaster()
	{
		if (isConnecting)
		{
			PhotonNetwork.JoinRandomRoom();
			isConnecting = false;
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		progressLabel.SetActive(false);
		controlPanel.SetActive(true);
		isConnecting = false;
		Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
	}

	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
		{
			PhotonNetwork.LoadLevel("Room for 1");
		}
	}

	#endregion

	#region Public Methods

	public void Connect()
	{
		progressLabel.SetActive(true);
		controlPanel.SetActive(false);
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			isConnecting = PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = gameVersion;
		}
	}

	#endregion
}
