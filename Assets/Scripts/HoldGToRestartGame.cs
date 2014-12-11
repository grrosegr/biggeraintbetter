using UnityEngine;
using System.Collections;

public class HoldGToRestartGame : MonoBehaviour {

	public float SecondsToHold = 3.0f;
	private float keyDownStartTime = 0;
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.G))
			keyDownStartTime = Time.time;
		else if (Input.GetKey(KeyCode.G) && ((Time.time - keyDownStartTime) > SecondsToHold))
			Application.LoadLevel(0);
	}
}
