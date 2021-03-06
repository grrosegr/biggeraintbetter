﻿using UnityEngine;
using System.Collections;

public class QuestionBox : MonoBehaviour {

	private Animator anim;
	public GameObject mushroom;
	private bool hasMushroom = true;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
//		
	}
	
	void OnTriggerEnter2D(Collider2D coll) {
		if (!hasMushroom)
			return;
		if (!(coll.bounds.max.y < collider2D.bounds.min.y + 0.2))
			return;
			
		audio.Play();
		hasMushroom = false;
		anim.SetBool("Active", hasMushroom);
		Vector3 pos = transform.position;
		pos.y += 0.5f * transform.parent.localScale.y;
		GameObject mushroom_object = (GameObject)Instantiate(mushroom, pos, Quaternion.identity);
		mushroom_object.transform.localScale = transform.parent.localScale;
	}
	
}
