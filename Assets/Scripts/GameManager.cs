using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
	public static GameManager Instance;
	public GameObject playerPrefab;

	private void Start()
	{
		Instance = this;
		if (playerPrefab != null)
		{
			if (PlayerManager.localPlayerInstance == null)
				PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
		}
	}

	public override void OnLeftRoom() => SceneManager.LoadScene(0);

	public void LeaveRoom() => PhotonNetwork.LeaveRoom();

	private void LoadArena()
	{
		if (!PhotonNetwork.isMasterClient)
			Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
		PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer other)
	{
		if (PhotonNetwork.isMasterClient)
			LoadArena();
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
	{
		if (PhotonNetwork.isMasterClient)
			LoadArena();
	}
}
