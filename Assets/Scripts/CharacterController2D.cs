using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {

	public AudioClip SmallJump;
	public AudioClip LargeJump;
	public AudioClip Powerup;
	public AudioClip DieSound;

	public float JumpSpeed = 3.5f;
	public float HorizontalSpeed = 1.0f;
	private Animator anim;
	private float lastJump;
	public float CameraZoomMultiplier = 1.0f;
	
	public bool randomize = false;
	private float nextChange;
	private int randMult = 1;
	private Vector3 respawn;
	
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
	
	private float _scale = 1;
	public float Scale {
		set {
			if (value == _scale) return;
			_scale = value;
			UpdateLocalScale();
		}
		
		get {
			return _scale;
		}
	}
	
	private static CharacterController2D instance;
	
	public static float StaticScale {
		get {
			return instance.Scale;
		}
	}
	
	private float initialScale;

	void Start () {
		respawn = transform.position;
		initialScale = transform.localScale.x;
		Scale = initialScale;
		instance = this;
		anim = GetComponent<Animator>();
		Alive = true;
		FacingRight = true;
		lastJump = -1;
		
		// TODO: reenable
		if (Cheater.Instance.ScaleIn)
			StartCoroutine(ScaleUp());
	}
	
	void FaceLeft() {
		FacingRight = false;
	}
	
	void FaceRight() {
		FacingRight = true;
	}
	
	private bool IsGrounded() {
		Bounds bounds = collider2D.bounds;
		float epsilon = 0.1f;
//		float epsilon_x = bounds.size.x * epsilon;
//		float epsilon_y = bounds.size.y * 0.3f + 0.1f;
//		Vector2 bottomLeft = new Vector2(bounds.min.x + epsilon_x, bounds.min.y);
//		Vector2 bottomRightPadded = new Vector2(bounds.max.x - epsilon_x, bounds.min.y - epsilon_y);
		
		Vector2 start = new Vector2(bounds.min.x, bounds.min.y - epsilon);
		Vector2 end = new Vector2(bounds.max.x, bounds.min.y - epsilon);
		
		bool grounded = (bool)Physics2D.Linecast(
			start,
			end, 
			LayerMask.GetMask("Terrain", "Background")
		);
		Debug.DrawRay(start, end - start, Color.blue);
		
		return grounded;
//		(bool)Physics2D.OverlapArea(bottomLeft, bottomRightPadded, LayerMask.GetMask("Terrain", "Background"));
	}
	
	private bool jumpTriggered;
	void Update() {
		// workaround because GetKeyDown doesn't work well in FixedUpdate
		// see http://answers.unity3d.com/questions/20717/inputgetbuttondown-inconsistent.html
		bool newJumpTriggered = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space);
		if (!jumpTriggered) // don't set false if it's been set, FixedUpdate still needs to see it!
			jumpTriggered = newJumpTriggered;
	}
	
	void FixedUpdate () {
		if (!Alive)
			return;
		if (AlternatingScale || ScalingToNextLevel)
			return;
			
		Vector2 velocity = rigidbody2D.velocity;
	
		bool grounded = IsGrounded();
		if (jumpTriggered) {
			jumpTriggered = false;
				
			if (grounded && (Time.time - lastJump > 0.05)) {
				lastJump = Time.time;
				if (Scale == 1)
					audio.PlayOneShot(SmallJump, 1.0f);
				else
					audio.PlayOneShot(LargeJump, 1.0f);
				
				velocity.y = JumpSpeed * Scale; // * Mathf.Max (1.0f, Mathf.Log(transform.localScale.y, 2.0f));
			}
				
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
		
		Vector2 position = transform.position;
		transform.position = position;
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
	
		Alive = true;
		rigidbody2D.velocity = Vector2.zero;
		collider2D.enabled = true;
		anim.SetBool("Alive", true);
		transform.position = respawn;
		
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
	
	public bool AlternatingScale {get; private set;}
	
	IEnumerator AlternateScale(float oldScale, float newScale) {
		float floorY = collider2D.bounds.min.y;
		float extentY = transform.position.y - floorY;
		
		AlternatingScale = true;
		collider2D.enabled = false;
		rigidbody2D.isKinematic = true;
		
		for (int i = 0; i <= 2; i += 1) {
			Vector2 correctedPos = transform.position;
		
			if (i % 2 == 0) {
				Scale = newScale;
				correctedPos.y = floorY + extentY * (float)newScale / (float)oldScale;
			} else {
				Scale = oldScale;
				correctedPos.y = floorY + extentY;
			}
			transform.position  = correctedPos;
			
			yield return new WaitForSeconds(0.075f);
		}
		
		collider2D.enabled = true;
		rigidbody2D.isKinematic = false;
		AlternatingScale = false;
	}
	
	// TODO: convert to states
	public bool ScalingToNextLevel {get; private set;}
	
	private IEnumerator ScaleUp() {
		// Important: need to collect these bounds before turning off collider2d
		// otherwise bounds becomes 0 x 0 px
		float floorY = collider2D.bounds.min.y;
		float extentY = (transform.position.y - floorY) / Scale;
		
		ScalingToNextLevel = true;
		collider2D.enabled = false;
		rigidbody2D.isKinematic = true;
		
		float endScale = Scale;
		for (int i = 1; i <= 50; i += 1) {
			Scale = endScale * i / 50.0f;
			
			Vector2 correctedPos = transform.position;
			
			correctedPos.y = floorY + extentY * Scale;
			transform.position  = correctedPos;
			
			yield return new WaitForSeconds(0.001f);
		}
		
		ScalingToNextLevel = false;
		collider2D.enabled = true;
		rigidbody2D.isKinematic = false;
	}
	
	IEnumerator ScaleToNextLevel() {
		ScalingToNextLevel = true;
		float floorY = collider2D.bounds.min.y;
		float extentY = transform.position.y - floorY;
		
		collider2D.enabled = false;
		rigidbody2D.isKinematic = true;
		
		float startScale = Scale;
		for (int i = 1; i <= 100; i += 1) {
			Scale *= 1.1f;
			
			Vector2 correctedPos = transform.position;
			
			correctedPos.y = floorY + extentY * (float)Scale / (float)startScale;
			transform.position  = correctedPos;
			
			yield return new WaitForSeconds(0.001f);
		}
		
		Application.LoadLevel((Application.loadedLevel + 1) % Application.levelCount);
	}
	
	public void SetRespawn(Vector3 pos) {
		respawn = pos;
	}
	
	public void TriggerNextLevel() {
		audio.loop = true;
		audio.clip = Powerup;
		audio.Play();
		
		StartCoroutine(ScaleToNextLevel());
	}
	
	private bool ExpScale = false;
	public void GetMushroom() {
		float newScale = Scale;
		if (!ExpScale)
			newScale += initialScale;
		else
			newScale *= 2;
	
		StartCoroutine(AlternateScale(Scale, newScale));
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (AlternatingScale)
			return;
		
		if (coll.gameObject.name == "NextLevel") {
			Destroy(coll.gameObject);
			TriggerNextLevel();
		} else if (coll.gameObject.tag == "RedMushroom") {
			audio.PlayOneShot(Powerup);
			Destroy(coll.gameObject);

			GetMushroom();
		} else if (coll.gameObject.name == "KillBox") {
			Die ();
		} else if (coll.gameObject.tag == "Enemy") {
			if (coll.transform.position.y > transform.position.y)
				Die();
		}
	}
}
