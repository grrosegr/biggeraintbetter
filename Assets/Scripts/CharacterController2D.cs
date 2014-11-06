using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {

	public AudioClip SmallJump;
	public AudioClip LargeJump;
	public AudioClip Powerup;
	public AudioClip DieSound;

	public float JumpSpeed = 3.5f;
	public float HorizontalSpeed = 2.0f;
	private Animator anim;
	private float lastJump;
	
	public bool randomize = false;
	public bool scaling = false;
	private float nextChange;
	private int randMult = 1;
	
	private void UpdateLocalScale() {
		Vector3 newLocalScale = Vector3.one;
		if (!FacingRight)
			newLocalScale.x = -1;
		newLocalScale *= Scale;
		transform.localScale = newLocalScale;
		rigidbody2D.gravityScale = Scale;
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
	
	private int _scale = 1;
	public int Scale {
		set {
			if (value == _scale) return;
			transform.localScale = Vector3.one * value;
			_scale = value;
			UpdateLocalScale();
		}
		
		get {
			return _scale;
		}
	}
	
	private static CharacterController2D instance;
	
	public static int StaticScale {
		get {
			return instance.Scale;
		}
	}

	void Start () {
		instance = this;
		anim = GetComponent<Animator>();
		Alive = true;
	}
	
	void FaceLeft() {
		FacingRight = false;
	}
	
	void FaceRight() {
		FacingRight = true;
	}
	
	void FixedUpdate () {
		if (!Alive)
			return;
			
		Vector2 velocity = rigidbody2D.velocity;

		Bounds bounds = collider2D.bounds;
		Vector2 topLeft = new Vector2(bounds.min.x, bounds.min.y);
		Vector2 bottomRight = new Vector2(bounds.max.x, bounds.max.y);
		// TODO: don't use area
		bool grounded = (bool)Physics2D.OverlapArea(topLeft, bottomRight, LayerMask.GetMask("Terrain"));
		if (grounded && Input.GetAxisRaw("Vertical") > 0 && (Time.time - lastJump > 0.2)) {
			lastJump = Time.time;
			if (Scale == 1)
				audio.PlayOneShot(SmallJump);
			else
				audio.PlayOneShot(LargeJump);
				
			velocity.y = JumpSpeed * Scale; // * Mathf.Max (1.0f, Mathf.Log(transform.localScale.y, 2.0f));
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
		
		velocity.x = horizantal * HorizontalSpeed * Scale;
		anim.SetFloat("Speed", Mathf.Abs(velocity.x));
		anim.SetBool("Grounded", grounded);
		rigidbody2D.velocity = velocity;
	}
	
	public bool Alive {
		get; private set;
	}
	
	IEnumerator ResetLevelAfter(float waitTime) 
	{
		yield return new WaitForSeconds(waitTime);
		Application.LoadLevel(Application.loadedLevel);
	}
	
	private void Die() {
		if (!Alive)
			return;
		Alive = false;
		rigidbody2D.velocity = new Vector2(0, JumpSpeed * Scale);
		collider2D.enabled = false;
		audio.PlayOneShot(DieSound);
		anim.SetBool("Alive", false);
		StartCoroutine(ResetLevelAfter(DieSound.length + 0.0f));
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "RedMushroom") {
			audio.PlayOneShot(Powerup);
			Destroy(coll.gameObject);
			if (scaling)
				Scale *= 2;
		} else if (coll.gameObject.name == "KillBox") {
			Die ();
		} else if (coll.gameObject.tag == "Enemy") {
			if (coll.transform.position.y > transform.position.y)
				Die();
		}
	}
}
