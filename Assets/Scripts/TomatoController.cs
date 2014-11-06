using UnityEngine;
using System.Collections;

public class TomatoController : MonoBehaviour {

	public float Speed = 1.0f;

	private void UpdateLocalScale() {
		Vector3 newLocalScale = transform.localScale;
		if (FacingRight)
			newLocalScale.x = Mathf.Abs (newLocalScale.x);
		else
			newLocalScale.x = -Mathf.Abs (newLocalScale.x);
		transform.localScale = newLocalScale;
	}
	
	private bool _facingRight;
	private bool FacingRight {
		set {
			if (value == _facingRight) return;
			_facingRight = value;
			UpdateLocalScale();
		}
		
		get {
			return _facingRight;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 velocity = rigidbody2D.velocity;
		
		// TODO: relative to velocity
		Vector3 next = transform.position + new Vector3(velocity.x, velocity.y).normalized * 0.1f;
		Vector3 nextGround = next - new Vector3(0, collider2D.bounds.extents.y + 0.1f, 0);
		if (!Physics2D.Linecast(next, nextGround, LayerMask.GetMask("Terrain")))
			FacingRight = !FacingRight;
		
		velocity.x = -transform.localScale.x * Speed;
		rigidbody2D.velocity = velocity;
	}
	
	private bool Alive = true;
	private void Die() {
		if (!Alive)
			return;
		Alive = false;
		rigidbody2D.velocity = new Vector2(0, -1);
		collider2D.enabled = false;
	}
	
	
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") {
			// TODO: fix this
			if (coll.transform.position.y > transform.position.y)
				Die();
		} else if (coll.gameObject.name == "KillBox") {
			Die ();
		}
	}
}
