using UnityEngine;
using System.Collections;

public class Cheater : MonoBehaviour {

	private GameObject[] respawnPoints;
	
	public bool ScaleIn = true;
	
	private GameObject player;
	private CharacterController2D controller;
	
	private static Cheater _instance;
	public static Cheater Instance {
		get {
			return _instance;
		}
	}
	
	void Awake() {
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		respawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
		player = GameObject.FindGameObjectWithTag("Player");
		
		controller = GetComponent<CharacterController2D>();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i <= 9; i++) {
		
		}
		
		if (Input.GetKeyDown(KeyCode.N))
			Application.LoadLevel((Application.loadedLevel + 1) % Application.levelCount);
			
		if (Input.GetKeyDown(KeyCode.M))
			controller.GetMushroom();
			
	}
}
