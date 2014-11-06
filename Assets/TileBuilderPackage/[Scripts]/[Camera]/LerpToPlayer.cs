using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LerpToPlayer : MonoBehaviour {

	private Player[] player;
    
	private void Awake() {
		player = FindObjectsOfType(typeof(Player)) as Player[];
	}
    
	private void FixedUpdate() {
		float xPositionOfAllPlayers = 0;
		float yPositionOfAllPlayers = 0;
    
		for (int i = 0; i < player.Length; i++) {
			xPositionOfAllPlayers += player [i].transform.position.x;
			yPositionOfAllPlayers += player [i].transform.position.y;
		}
        
		//Lerp the camera to the center point of all players on the screen.
		this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(xPositionOfAllPlayers / player.Length, yPositionOfAllPlayers / player.Length, this.transform.position.z), 0.1f);
	}
}
