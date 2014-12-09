using UnityEngine;
using System.Collections;

public class RespawnPoint : MonoBehaviour {

	public GameObject[] DonutsEaten;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		other.SendMessage("SetRespawn", this);
	}
}
