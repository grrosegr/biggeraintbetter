using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player")
			other.SendMessage("Kill");
	}
}
