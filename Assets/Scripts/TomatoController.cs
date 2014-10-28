using UnityEngine;
using System.Collections;

public class TomatoController : MonoBehaviour {

	public float Speed = 1.0f;

	private void UpdateLocalScale() {
		Vector3 newLocalScale = Vector3.one;
		if (FacingRight)
			newLocalScale.x = -1;
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
		velocity.x = -transform.localScale.x * Speed;
		rigidbody2D.velocity = velocity;
	}
}
