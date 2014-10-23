using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {

	public float JumpSpeed = 3.5f;
	public float HorizontalSpeed = 2.0f;
	private Animator anim;
	private float lastJump;
	
	public bool randomize = false;
	private float nextChange;
	private int randMult = 1;

	void Start () {
		anim = GetComponent<Animator>();
	}
	
	void FaceLeft() {
		Vector3 scale = transform.localScale;
		scale.x = -Mathf.Abs(scale.x);
		transform.localScale = scale;
	}
	
	void FaceRight() {
		Vector3 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x);
		transform.localScale = scale;
	}
	
	void FixedUpdate () {
		Vector2 velocity = rigidbody2D.velocity;

		Bounds bounds = collider2D.bounds;
		Vector2 topLeft = new Vector2(bounds.min.x, bounds.min.y);
		Vector2 bottomRight = new Vector2(bounds.max.x, bounds.max.y);
		bool grounded = (bool)Physics2D.OverlapArea(topLeft, bottomRight, LayerMask.GetMask("Terrain"));
		if (grounded && Input.GetAxisRaw("Vertical") > 0 && (Time.time - lastJump > 0.2)) {
			lastJump = Time.time;
			audio.Play();
			velocity.y = JumpSpeed;
		}

		float horizantal = Input.GetAxis("Horizontal");
		
		if (randomize && Time.time > nextChange) {
			nextChange = Time.time + Random.value * 0.5f;
			randMult *= -1;
		}
		
		horizantal *= randMult;
		
			
		// Don't change direction on 0 or avatar will awkwardly face
		// one way if no key is pressed
		if (horizantal > 0)
			FaceRight();
		else if (horizantal < 0)
			FaceLeft();
			
		velocity.x = horizantal * HorizontalSpeed;
		anim.SetFloat("Speed", Mathf.Abs(velocity.x));
		anim.SetBool("Grounded", grounded);
		rigidbody2D.velocity = velocity;
	}
}
