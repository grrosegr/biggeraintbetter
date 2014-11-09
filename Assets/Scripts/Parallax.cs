using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	GameObject Player;
	Vector3 PlayerStartPosition;
	Vector3 MyStartPosition;

	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectWithTag("Player");
		PlayerStartPosition = Player.transform.position;
		MyStartPosition = transform.position;
		if (!Player)
			Debug.LogError("Could not find player!");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 delta = Player.transform.position - PlayerStartPosition;
		delta.y = 0;
		transform.position = MyStartPosition + 0.2f * delta;
	}
}
