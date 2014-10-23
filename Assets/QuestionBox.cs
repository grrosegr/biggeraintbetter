using UnityEngine;
using System.Collections;

public class QuestionBox : MonoBehaviour {

	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
//		
	}
	
	void OnTriggerEnter2D(Collider2D coll) {
		Debug.Log ("lulz");
		anim.SetBool("Active", false);
	}
	
}
