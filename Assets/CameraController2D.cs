using UnityEngine;
using System.Collections;

public class CameraController2D : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	private CharacterController2D targetController;

	// Use this for initialization
	void Start () {
		targetController = FindObjectOfType<CharacterController2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!targetController || !targetController.Alive)
			return;
			
		GameObject targetObject = targetController.gameObject;
		
		Transform target = targetObject.transform;
		camera.orthographicSize = targetController.Scale; //Mathf.Pow(1.5f, Mathf.Log (targetController.Scale, 2.0f));
		Vector3 t_pos = target.position;
		t_pos.y += 0.5f;
		Vector3 point = camera.WorldToViewportPoint(t_pos);
		Vector3 delta = t_pos - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
		Vector3 destination = transform.position + delta;
		transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
	}
}
