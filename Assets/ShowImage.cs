using UnityEngine;
using System.Collections;

public class ShowImage : MonoBehaviour {

	public Texture2D img;
	
	void OnGUI() {
		var rect = new Rect(0, 0, Screen.width, Screen.height);
		GUI.DrawTexture(rect, img, ScaleMode.StretchToFill);
	}
	
	void Update() {
		if (Input.anyKeyDown)
			Application.LoadLevel((Application.loadedLevel + 1) % Application.levelCount);
	}
}
