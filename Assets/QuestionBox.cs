using UnityEngine;
using System.Collections;

public class QuestionBox : MonoBehaviour {

	private Animator anim;
	public GameObject mushroom;
	private bool active = true;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
//		
	}
	
	void OnTriggerEnter2D(Collider2D coll) {
		if (!active)
			return;
		audio.Play();
		active = false;
		anim.SetBool("Active", active);
		Vector3 pos = transform.position;
		pos.y += 0.5f;
		Instantiate(mushroom, pos, Quaternion.identity);
	}
	
}
