using ExitGames.Demos.DemoAnimator;

using UnityEngine;

public class PlayerManager : Photon.PunBehaviour, IPunObservable
{
	public static GameObject localPlayerInstance;
	public GameObject beams;
	public float health = 1f;
	public GameObject playerUiPrefab;
	private bool _isFiring;

	private void Awake()
	{
		if (beams != null)
			beams.SetActive(false);
		if (photonView.isMine)
			localPlayerInstance = gameObject;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

		if (_cameraWork != null)
		{
			if (photonView.isMine)
				_cameraWork.OnStartFollowing();
		}
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
		if (playerUiPrefab != null)
		{
			GameObject _uiGo = Instantiate(playerUiPrefab) as GameObject;
			_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		}
	}

	void Update()
	{
		if (photonView.isMine)
		{
			ProcessInputs();
			if (beams != null && _isFiring != beams.activeInHierarchy)
				beams.SetActive(_isFiring);
			if (health <= 0f)
				GameManager.Instance.LeaveRoom();
		}
	}

	void ProcessInputs()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			if (!_isFiring)
				_isFiring = true;
		}

		if (Input.GetButtonUp("Fire1"))
		{
			if (_isFiring)
				_isFiring = false;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!photonView.isMine)
			return;
		if (!other.name.Contains("Beam"))
			return;
		health -= 0.1f;
	}

	void OnTriggerStay(Collider other)
	{
		if (!photonView.isMine)
			return;
		if (!other.name.Contains("Beam"))
			return;
		health -= 0.1f * Time.deltaTime;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(_isFiring);
			stream.SendNext(health);
		}
		else
		{
			_isFiring = (bool)stream.ReceiveNext();
			health = (float)stream.ReceiveNext();
		}
	}

	void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode) => CalledOnLevelWasLoaded(scene.buildIndex);

	void CalledOnLevelWasLoaded(int level)
	{
		if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
			transform.position = new Vector3(0f, 5f, 0f);
		GameObject _uiGo = Instantiate(playerUiPrefab);
		_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
	}

	private void OnDisable() => UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
}
