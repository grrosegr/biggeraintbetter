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
		//t_pos.y += 0.5f * scale;
		Vector3 point = camera.WorldToViewportPoint(t_pos);
		Vector3 delta = t_pos - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
		Vector3 destination = transform.position + delta;
		
		return destination;
	}
	
	// Update is called once per frame
	void Update () {
		if (!targetController || !targetController.Alive)
			return;
			
		if (targetController.AlternatingScale)
			return;
		
		
		transform.position = Vector3.SmoothDamp(transform.position, GetDestination(), ref velocity, dampTime);
	}
}
