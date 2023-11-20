using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	public Text playerNameText;
	public Slider playerHealthSlider;
	public Text winText;
	public Vector3 screenOffset = new Vector3(0f, 30f, 0f);
	private PlayerManager _target;
	private float _characterControllerHeight = 0f;
	private Transform _targetTransform;
	private Renderer _targetRenderer;
	private CanvasGroup _canvasGroup;
	private Vector3 _targetPosition;
	private bool _wasMoreThanOne = false;

	void Awake()
	{
		GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
		_canvasGroup = GetComponent<CanvasGroup>();
		if (PhotonNetwork.room != null && PhotonNetwork.room.PlayerCount == 1 && _wasMoreThanOne)
		{
			winText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			winText.gameObject.SetActive(true);
		}
		else if (PhotonNetwork.room != null && PhotonNetwork.room.PlayerCount > 1)
		{
			_wasMoreThanOne = true;
			winText.gameObject.SetActive(false);
		}
	}

	void Update()
	{
		if (playerHealthSlider != null)
			playerHealthSlider.value = _target.health;
		if (_target == null)
		{
			Destroy(gameObject);
			return;
		}
	}


	void LateUpdate()
	{
		if (_targetRenderer != null)
			_canvasGroup.alpha = _targetRenderer.isVisible ? 1f : 0f;

		if (_targetTransform != null)
		{
			_targetPosition = _targetTransform.position;
			_targetPosition.y += _characterControllerHeight;
			transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + screenOffset;
		}
	}

	public void SetTarget(PlayerManager target)
	{
		if (target == null)
			return;
		_target = target;
		if (playerNameText != null)
			playerNameText.text = _target.photonView.owner.NickName;
		_targetTransform = _target.GetComponent<Transform>();
		_targetRenderer = _target.GetComponent<Renderer>();
		CharacterController _characterController = _target.GetComponent<CharacterController>();
		if (_characterController != null)
			_characterControllerHeight = _characterController.height;
	}
}
