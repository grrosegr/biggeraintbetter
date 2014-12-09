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
	private float respawnScale;
	
	private GameObject[] respawnPoints;
	
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
	
	private Collider2D[] collider2Ds;
	private bool ColliderEnabled {
		set {
			foreach (Collider2D c in collider2Ds) {
				c.enabled = value;
			}
		}
	}
	
	private Collider2D mainCollider2D;

	void Start () {
		collider2Ds = GetComponents<Collider2D>();
		mainCollider2D = GetComponent<BoxCollider2D>();
		initialScale = transform.localScale.x;
		Scale = initialScale;
		SetRespawn(transform.position);
		instance = this;
		anim = GetComponent<Animator>();
		Alive = true;
		FacingRight = true;
		lastJump = -1;
		respawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
		
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
		Bounds bounds = mainCollider2D.bounds;
		float epsilon = bounds.size.y * 0.1f;
		
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
	private float jumpTriggeredTime;
	void Update() {
		if (restartingLevel)
			return;
			
		// workaround because GetKeyDown doesn't work well in FixedUpdate
		// see http://answers.unity3d.com/questions/20717/inputgetbuttondown-inconsistent.html
		bool newJumpTriggered = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
		if (!jumpTriggered) // don't set false if it's been set, FixedUpdate still needs to see it!
			jumpTriggered = newJumpTriggered;
		if (newJumpTriggered)
			jumpTriggeredTime = Time.time;
			
		if (Input.GetKeyDown(KeyCode.R))
			StartCoroutine(ResetLevelAfter(0));
		else {
			for (int i = 1; i <= respawnPoints.Length; i++) {
				if (Input.GetKeyDown(i.ToString())) {
					SetRespawn(respawnPoints[i - 1].GetComponent<RespawnPoint>());
					StartCoroutine(ResetLevelAfter(0));
					break;
				}
			}
		}
	}
	
	void FixedUpdate () {
		if (!Alive)
			return;
		if (AlternatingScale || ScalingToNextLevel)
			return;
			
		Vector2 velocity = rigidbody2D.velocity;
	
		bool grounded = IsGrounded();
		if (jumpTriggered) {
		
			if (grounded && (Time.time - lastJump > 0.05)) {
				jumpTriggered = false;
				lastJump = Time.time;
				if (Scale == 1)
					audio.PlayOneShot(SmallJump, 1.0f);
				else
					audio.PlayOneShot(LargeJump, 1.0f);
				
				velocity.y = JumpSpeed * Scale; // * Mathf.Max (1.0f, Mathf.Log(transform.localScale.y, 2.0f));
			} else if (Time.time - jumpTriggeredTime > 0.1) {
				jumpTriggered = false;
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
		CheckFit();
	}		
	
	public void CheckFit() {
		// Check if we're too big to fit in the area
		Bounds bounds = mainCollider2D.bounds;
		bool left, right, up, down;
		
		int layerMask = LayerMask.GetMask("Terrain", "Background");
		
		down = (bool)Physics2D.OverlapPoint(
			new Vector2(bounds.center.x, bounds.min.y),
			layerMask
			);
		up = (bool)Physics2D.OverlapPoint(
			new Vector2(bounds.center.x, bounds.max.y),
			layerMask
			);
		left = (bool)Physics2D.OverlapPoint(
			new Vector2(bounds.min.x, bounds.center.y),
			layerMask
			);
		right = (bool)Physics2D.OverlapPoint(
			new Vector2(bounds.max.x, bounds.center.y),
			layerMask
			);
		
		if ((left && right) || (up && down)) {
			Die();
		}
	}
	
	public bool Alive {
		get; private set;
	}
	
	bool restartingLevel;
	IEnumerator ResetLevelAfter(float waitTime) 
	{
		restartingLevel = true;
		yield return new WaitForSeconds(waitTime);
	
		Alive = true;
		rigidbody2D.velocity = Vector2.zero;
		ColliderEnabled = true;
		anim.SetBool("Alive", true);
		transform.position = respawn;
		Scale = respawnScale;
		
		var doughnuts = GameObject.FindGameObjectsWithTag("RedMushroom");
		foreach (var doughnut in doughnuts) {
			doughnut.SendMessage("Show");
		}
		restartingLevel = false;
	}
	
	private void Die() {
		if (!Alive)
			return;
		Alive = false;
		rigidbody2D.velocity = new Vector2(0, JumpSpeed * Scale);
		ColliderEnabled = false;
		audio.PlayOneShot(DieSound);
		anim.SetBool("Alive", false);
		StartCoroutine(ResetLevelAfter(DieSound.length));
	}
	
	public bool AlternatingScale {get; private set;}
	
	IEnumerator AlternateScale(float oldScale, float newScale) {
		float floorY = mainCollider2D.bounds.min.y;
		float extentY = transform.position.y - floorY;
		
		AlternatingScale = true;
		ColliderEnabled = false;
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
		
		ColliderEnabled = true;
		rigidbody2D.isKinematic = false;
		AlternatingScale = false;
	}
	
	// TODO: convert to states
	public bool ScalingToNextLevel {get; private set;}
	
	private IEnumerator ScaleUp() {
		// Important: need to collect these bounds before turning off collider2d
		// otherwise bounds becomes 0 x 0 px
		float floorY = mainCollider2D.bounds.min.y;
		float extentY = (transform.position.y - floorY) / Scale;
		
		ScalingToNextLevel = true;
		ColliderEnabled = false;
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
		ColliderEnabled = true;
		rigidbody2D.isKinematic = false;
	}
	
	IEnumerator ScaleToNextLevel() {
		ScalingToNextLevel = true;
		float floorY = mainCollider2D.bounds.min.y;
		float extentY = transform.position.y - floorY;
		
		ColliderEnabled = false;
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
	
	public void SetRespawn(Vector3 v) {
		respawn = v;
		respawnScale = Scale;
	}
	
	public void SetRespawn(RespawnPoint p) {
		respawn = p.transform.position;
		respawnScale = p.RespawnSize;
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
	
	void Kill() {
		Die ();
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (AlternatingScale)
			return;
		
		if (coll.gameObject.name == "NextLevel") {
			Destroy(coll.gameObject);
			TriggerNextLevel();
		} else if (coll.gameObject.tag == "RedMushroom") {
			audio.PlayOneShot(Powerup);
//			Destroy(coll.gameObject);
			coll.gameObject.SendMessage("Hide");

			GetMushroom();
		} else if (coll.gameObject.tag == "Enemy") {
			if (coll.transform.position.y > transform.position.y)
				Die();
		}
	}
}
