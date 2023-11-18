using UnityEngine;

public class Launcher : Photon.PunBehaviour
{
	private string _gameVersion = "1";
	private bool isConnecting;
	public PhotonLogLevel loglevel = PhotonLogLevel.Informational;
	public byte maxPlayersPerRoom = 4;
	public GameObject controlPanel;
	public GameObject progressLabel;

	private void Awake()
	{
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.logLevel = loglevel;
	}

	private void Start()
	{
		progressLabel.SetActive(false);
		controlPanel.SetActive(true);
	}

	public void Connect()
	{
		progressLabel.SetActive(true);
		controlPanel.SetActive(false);
		if (PhotonNetwork.connected)
			PhotonNetwork.JoinRandomRoom();
		else
			isConnecting = PhotonNetwork.ConnectUsingSettings(_gameVersion);
	}

	public override void OnConnectedToMaster()
	{
		if (isConnecting)
		{
			PhotonNetwork.JoinRandomRoom();
			isConnecting = false;
		}
	}

	public override void OnDisconnectedFromPhoton()
	{
		progressLabel.SetActive(false);
		controlPanel.SetActive(true);
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) => PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayersPerRoom }, null);

	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.room.PlayerCount == 1)
			PhotonNetwork.LoadLevel("Room for 1");
	}
}
