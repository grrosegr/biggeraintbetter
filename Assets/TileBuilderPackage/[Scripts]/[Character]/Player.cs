using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private enum CharacterStates {
		Idle,
		Walk,
		Jump
	}
    
	public Transform groundCheck;
	private CharacterStates currentCharacterState = CharacterStates.Idle;
	private string axisName = "Horizontal";
	private float movementSpeed = 5;
	private float AnimationMovementSpeed;
	private float jumpPower = 300.0f;
	private bool isGrounded = false;
	private Vector2 newScale;
	private Animator playerAnimator;
	private Rigidbody2D playerBody2D;
    
	private void Awake() {
		playerAnimator = this.GetComponent<Animator>();
		playerBody2D = this.GetComponent<Rigidbody2D>();
	}
    
	private void Start() {
		newScale = transform.localScale;
	}
    
	private void FixedUpdate() {
		UpdateEnumStatesHandler();
		UpdateAnimationHandler();
		UpdateMovementHandler();
		UpdateDirectionHandler();
		UpdateJumpHandler();
		UpdateLandHandler();
	}
    
	private void UpdateEnumStatesHandler() {
		AnimationMovementSpeed = playerAnimator.GetFloat("MovementSpeed");
        
		if (AnimationMovementSpeed <= 0.1 && isGrounded == true) {
			currentCharacterState = CharacterStates.Idle;
		} else if (AnimationMovementSpeed >= 0.1f && isGrounded == true) {
			currentCharacterState = CharacterStates.Walk;
		} else if (isGrounded == false) {
			currentCharacterState = CharacterStates.Jump;
		}
	}
    
	private void UpdateAnimationHandler() {
		if (currentCharacterState == CharacterStates.Idle) {
			playerAnimator.SetBool("Idle", true);
			playerAnimator.SetBool("Walk", false);
			playerAnimator.SetBool("Jump", false);
		} else if (currentCharacterState == CharacterStates.Walk) {
			playerAnimator.SetBool("Idle", false);
			playerAnimator.SetBool("Walk", true);
			playerAnimator.SetBool("Jump", false);
		} else if (currentCharacterState == CharacterStates.Jump) {
			playerAnimator.SetBool("Idle", false);
			playerAnimator.SetBool("Walk", false);
			playerAnimator.SetBool("Jump", true);
		}
	}

	private void UpdateMovementHandler() {
		playerAnimator.SetFloat("MovementSpeed", Mathf.Abs(Input.GetAxis(axisName)));        
		transform.position += transform.right * Input.GetAxis(axisName) * movementSpeed * Time.deltaTime;
	}
    
	private void UpdateDirectionHandler() {
		if (Input.GetAxis(axisName) < 0) {
			newScale.x = -1.0f;
			transform.localScale = newScale;
		} else if (Input.GetAxis(axisName) > 0) {
			newScale.x = 1.0f;
			transform.localScale = newScale;
		}
	}
    
	private void UpdateLandHandler() {
		isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
	}
    
	private void UpdateJumpHandler() {
		if (currentCharacterState == CharacterStates.Idle || currentCharacterState == CharacterStates.Walk) {
			if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Space)) {
				playerBody2D.AddForce(transform.up * jumpPower);
			}
		}
	}
}
