﻿using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour {

	// Use this for initialization
	void Start () {
		rigidbody2D.velocity = new Vector2(1,0) * CharacterController2D.StaticScale;
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody2D.velocity = new Vector2(1,rigidbody2D.velocity.y) * CharacterController2D.StaticScale;
	}
}