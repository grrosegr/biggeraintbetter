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
		//t_pos.y += 0.5f * scale;
		Vector3 point = camera.WorldToViewportPoint(t_pos);
		
//		float max = 0.75f;
//		float min = 0.25f;
//		if (point.x < min || point.x > max || point.y < min || point.y > max) {
//			instantSnap = true;
//			Vector3 fixedPoint = new Vector2(Mathf.Clamp(point.x, min, max), Mathf.Clamp(point.y, min, max));
//			
//			t_pos = camera.ViewportToWorldPoint((point - fixedPoint) + new Vector3(0.5f, 0.5f, 0));
//		} else {
//			instantSnap = false;
//		}

//		if (point.x < min || point.x > max || point.y < min || point.y > max) {
//			dampTime *= .96f;
//			Debug.Log ("shrink");
//		} else {
//			dampTime = Mathf.Lerp(dampTime, .15f, 2f);
//			Debug.Log ("Grow");
//		}
		
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
			
		Vector3 dest = GetDestination();
		
		GameObject targetObject = targetController.gameObject;
		float speed = targetObject.rigidbody2D.velocity.magnitude;
		float newDamp;
		if (speed > 10)
			newDamp = Mathf.Min(.15f, 1/(speed - 10));
		else
			newDamp = .15f;
		dampTime = newDamp;
//		Debug.Log (dampTime);
//		if (speed > 10) {
//			dampTime = Mathf.Lerp(dampTime, .02f, 0.1f);
//		} else
//			dampTime = Mathf.Lerp(dampTime, .15f, 0.1f);
		
		if (instantSnap) {
			transform.position = dest;
			velocity = Vector3.zero;
		}
		else
			transform.position = Vector3.SmoothDamp(transform.position, dest, ref velocity, dampTime);
	}
}
