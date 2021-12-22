using System.Collections;

using UnityEngine;

public class TransformLerpFollow : MonoBehaviour
{

	[SerializeField] public Transform targetTransform;


	[Header("Configuration")]




	[SerializeField] public bool useLocalPos = true;

	[SerializeField] public bool followPosX = true;
	[SerializeField] public bool followPosY = true;
	[SerializeField] public bool followPosZ = true;

	[SerializeField] public float followPosStrength = 10f;

	[Space]


	[SerializeField] public bool useLocalRot = true;


	[SerializeField] public bool followRotX = true;
	[SerializeField] public bool followRotY = true;
	[SerializeField] public bool followRotZ = true;

	[SerializeField] public float followRotStrength = 10f;

	/// <summary>
	///		On PS4, we cannot have the player rotate because it
	///     breaks tracking - so we need to stop following the Y
	///     rotation on the platform (may have to do this differently).
	/// </summary>
	public void DisableFollowRotY()
	{
		followRotY = false;
	}


	private bool catchingUpSnapRot = false;
	//private Vector3 snapRotTargetRot = Vector3.zero;

	private bool targetWasNull = true;

	private bool overrideSpeedCatchUp = false;

	private void Update()
	{

		if (targetTransform == null)
		{
			return;
		}

		Vector3 pos = Vector3.zero;

		if (useLocalPos)
		{
			if (followPosX) pos.x = targetTransform.localPosition.x;
			if (followPosY) pos.y = targetTransform.localPosition.y;
			if (followPosZ) pos.z = targetTransform.localPosition.z;
		}
		else
		{
			if (followPosX) pos.x = targetTransform.position.x;
			if (followPosY) pos.y = targetTransform.position.y;
			if (followPosZ) pos.z = targetTransform.position.z;
		}

		Vector3 rot = Vector3.zero;
		Quaternion quat;

		if (useLocalRot)
		{
			if (followRotX) rot.x = targetTransform.localEulerAngles.x;
			if (followRotY) rot.y = targetTransform.localEulerAngles.y;
			if (followRotZ) rot.z = targetTransform.localEulerAngles.z;
		}
		else
		{
			if (followRotX) rot.x = targetTransform.eulerAngles.x;
			if (followRotY) rot.y = targetTransform.eulerAngles.y;
			if (followRotZ) rot.z = targetTransform.eulerAngles.z;
		}

		quat = Quaternion.Euler(rot);



		// ------- ANIMATE POSITION --------
		float posMulti = Time.deltaTime * followPosStrength;
		if (overrideSpeedCatchUp) posMulti = 1f;

		if (useLocalPos) transform.localPosition = Vector3.Lerp(transform.localPosition, pos, posMulti);
		else transform.position = Vector3.Lerp(transform.position, pos, posMulti);


		
		float rotMulti = Time.deltaTime * followRotStrength;
		if (overrideSpeedCatchUp) rotMulti = 1f;

		if (useLocalRot) transform.localRotation = Quaternion.Lerp(transform.localRotation, quat, rotMulti);
		else transform.rotation = Quaternion.Lerp(transform.rotation, quat, followRotStrength);
	



	}







}
