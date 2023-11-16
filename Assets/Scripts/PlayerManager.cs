using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
	#region Private Fields

	[Tooltip("The Beams GameObject to control")]
	[SerializeField]
	private GameObject beams;
	//True, when the user is firing
	bool IsFiring;
	#endregion

	#region Public Fields

	[Tooltip("The current Health of our player")]
	public float Health = 1f;

	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject LocalPlayerInstance;

	[Tooltip("The Player's UI GameObject Prefab")]
	[SerializeField]
	public GameObject PlayerUiPrefab;

	#endregion

	#region MonoBehaviour CallBacks

	void Awake()
	{
		if (photonView.IsMine)
		{
			PlayerManager.LocalPlayerInstance = this.gameObject;
		}
		DontDestroyOnLoad(gameObject);
		if (beams == null)
		{
			Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
		}
		else
		{
			beams.SetActive(false);
		}
	}

	void Start()
	{
		CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

		if (_cameraWork != null)
		{
			if (photonView.IsMine)
			{
				_cameraWork.OnStartFollowing();
			}
		}
		else
		{
			Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
		}
		#if UNITY_5_4_OR_NEWER
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
		#endif
		if (PlayerUiPrefab != null)
		{
			GameObject _uiGo = Instantiate(PlayerUiPrefab);
			_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		}
		else
		{
			Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
		}
	}

	void Update()
	{
		if (photonView.IsMine)
		{
			ProcessInputs();
			if (Health <= 0f)
			{
				GameManager.Instance.LeaveRoom();
			}
			if (beams != null && IsFiring != beams.activeInHierarchy)
			{
				beams.SetActive(IsFiring);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!photonView.IsMine)
		{
			return;
		}
		if (!other.name.Contains("Beam"))
		{
			return;
		}
		Health -= 0.1f;
	}

	void OnTriggerStay(Collider other)
	{
		if (!photonView.IsMine)
		{
			return;
		}
		if (!other.name.Contains("Beam"))
		{
			return;
		}
		Health -= 0.1f * Time.deltaTime;
	}

	#if !UNITY_5_4_OR_NEWER
	void OnLevelWasLoaded(int level)
	{
		this.CalledOnLevelWasLoaded(level);
	}
	#endif

	void CalledOnLevelWasLoaded(int level)
	{
		if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
		{
			transform.position = new Vector3(0f, 5f, 0f);
		}
		GameObject _uiGo = Instantiate(PlayerUiPrefab);
		_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
	}

	#if UNITY_5_4_OR_NEWER
		public override void OnDisable()
		{
			base.OnDisable();
			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
		}
	#endif

	#endregion

	#region Private Methods

	#if UNITY_5_4_OR_NEWER
	void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
	{
		this.CalledOnLevelWasLoaded(scene.buildIndex);
	}
	#endif

	#endregion

	#region Custom

	void ProcessInputs()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			if (!IsFiring)
			{
				IsFiring = true;
			}
		}
		if (Input.GetButtonUp("Fire1"))
		{
			if (IsFiring)
			{
				IsFiring = false;
			}
		}
	}
	#endregion

	#region IPunObservable implementation

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// Send the others our data
			stream.SendNext(IsFiring);
			stream.SendNext(Health);
		}
		else
		{
			// Network player, receive data
			this.IsFiring = (bool)stream.ReceiveNext();
			this.Health = (float)stream.ReceiveNext();
		}
	}

	#endregion
}