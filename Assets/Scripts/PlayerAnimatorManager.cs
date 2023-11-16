using Photon.Pun;

using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviourPun
{
	#region Private Fields

	private Animator animator;

	[SerializeField]
	private float directionDampTime = 0.25f;

	#endregion

	#region MonoBehaviour Callbacks

	void Start()
	{
		animator = GetComponent<Animator>();
		if (!animator)
		{
			Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
		}
	}

	void Update()
	{
		if (!photonView.IsMine && PhotonNetwork.IsConnected)
		{
			return;
		}
		if (!animator)
		{
			return;
		}
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.IsName("Base Layer.Run"))
		{
			if (Input.GetButtonDown("Fire2"))
			{
				animator.SetTrigger("Jump");
			}
		}
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		if (v < 0)
		{
			v = 0;
		}
		animator.SetFloat("Speed", h * h + v * v);
		animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
	}

	#endregion
}