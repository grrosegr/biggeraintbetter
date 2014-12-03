using UnityEngine;
using System.Collections;

public class DoughnutController : MonoBehaviour {

	private Vector2 startPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float offset = Mathf.Sin(Time.time*2) * collider2D.bounds.size.y * 0.1f;
		transform.position = startPos + new Vector2(0, offset);
	}
}
