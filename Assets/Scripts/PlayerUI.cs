using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	#region Private Fields

	[Tooltip("UI Text to display the Player's Name")]
	[SerializeField]
	private Text playerNameText;

	[Tooltip("UI Slider to display the Player's Health")]
	[SerializeField]
	private Slider playerHealthSlider;

	private PlayerManager target;
	float characterControllerHeight = 0f;
	Transform targetTransform;
	Renderer targetRenderer;
	CanvasGroup _canvasGroup;
	Vector3 targetPosition;

	#endregion

	#region Public Fields

	[Tooltip("Pixel offset from the player target")]
	[SerializeField]
	private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

	#endregion

	#region MonoBehaviour Callbacks

	void Awake()
	{
		transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
		_canvasGroup = this.GetComponent<CanvasGroup>();
	}

	void Update()
	{
		if (playerHealthSlider != null)
		{
			playerHealthSlider.value = target.Health;
		}
		if (target == null)
		{
			Destroy(gameObject);
			return;
		}
	}

	void LateUpdate()
	{
		if (targetRenderer != null)
		{
			_canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
		}


		if (targetTransform != null)
		{
			targetPosition = targetTransform.position;
			targetPosition.y += characterControllerHeight;
			transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
		}
	}

	#endregion

	#region Public Methods

	public void SetTarget(PlayerManager _target)
	{
		if (_target == null)
		{
			return;
		}
		target = _target;
		targetTransform = target.GetComponent<Transform>();
		targetRenderer = target.GetComponent<Renderer>();
		CharacterController characterController = _target.GetComponent<CharacterController>();
		if (characterController != null)
		{
			characterControllerHeight = characterController.height;
		}
		if (playerNameText != null)
		{
			playerNameText.text = target.photonView.Owner.NickName;
		}
	}

	#endregion
}