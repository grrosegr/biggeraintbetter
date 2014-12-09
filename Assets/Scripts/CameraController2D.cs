using UnityEngine;
using System.Collections;

public class CameraController2D : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	private CharacterController2D targetController;
	
	private float oldScale = 1;

	// Use this for initialization
	void Start () {
		targetController = FindObjectOfType<CharacterController2D>();
		transform.position = GetDestination();
	}

	float AlmostAsBig(float x) {
		if (x < 1)
			return x;
		return Mathf.Pow(1.85f, Mathf.Log (targetController.Scale, 2.0f));
	}
	
	bool instantSnap = false;
	Vector3 GetDestination() {
		GameObject targetObject = targetController.gameObject;
		
		Transform target = targetObject.transform;
		float newScale = targetController.Scale;
//		if (targetController.ScalingToNextLevel)
		newScale = AlmostAsBig(newScale);
		newScale *= targetController.CameraZoomMultiplier;
		float scale = Mathf.Lerp(oldScale, newScale, 0.2f);
		oldScale = scale;
		camera.orthographicSize = scale;
		transform.localScale = Vector3.one * scale;
		Vector3 t_pos = target.position;
		Vector3 point = camera.WorldToViewportPoint(t_pos);
		
		Vector3 delta = t_pos - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
		Vector3 destination = transform.position + delta;
		destination.z = -10;
		
		return destination;
	}
	
	// Update is called once per frame
	void Update () {
		if (!targetController || !targetController.Alive)
			return;
			
		if (targetController.AlternatingScale)
			return;
		
		GameObject targetObject = targetController.gameObject;
		float speed = targetObject.rigidbody2D.velocity.magnitude;
		float pivot = 10 * transform.localScale.x;
		float newDamp;
		if (speed > pivot)
			newDamp = Mathf.Min(.15f, 1/(speed - pivot));
		else
			newDamp = .15f;
		dampTime = newDamp;
		
		transform.position = Vector3.SmoothDamp(transform.position, GetDestination(), ref velocity, dampTime);
	}
}
