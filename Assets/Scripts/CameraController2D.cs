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
	}
	
	// Update is called once per frame
	void Update () {
		if (!targetController || !targetController.Alive)
			return;
			
		if (targetController.AlternatingScale)
			return;
		
		GameObject targetObject = targetController.gameObject;
		
		Transform target = targetObject.transform;
		float scale = Mathf.Lerp(oldScale, targetController.Scale, 0.2f);
		
		oldScale = scale;
		camera.orthographicSize = scale; //Mathf.Pow(1.5f, Mathf.Log (targetController.Scale, 2.0f));
		transform.localScale = Vector3.one * scale;
		Vector3 t_pos = target.position;
		t_pos.y += 0.5f * scale;
		Vector3 point = camera.WorldToViewportPoint(t_pos);
		Vector3 delta = t_pos - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
		Vector3 destination = transform.position + delta;
		transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
	}
}
