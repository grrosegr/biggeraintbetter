using UnityEngine;
using System.Collections;

public class Killable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private bool Alive = true;
	private void Die() {
		if (!Alive)
			return;
		Alive = false;
		rigidbody2D.velocity = new Vector2(0, -1);
		rigidbody2D.isKinematic = false;
		collider2D.enabled = false;
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {
			// TODO: fix this
			if (coll.transform.position.y > transform.position.y)
				Die();
		}
	}
}
