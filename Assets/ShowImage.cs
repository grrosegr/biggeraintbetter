using UnityEngine;
using System.Collections;

public class ShowImage : MonoBehaviour {

	public Texture2D img;
	public float MinimumWaitTime = 1;
	
	private float startTime;
	
	void Start() {
		startTime = Time.time;
	}
	
	void OnGUI() {
		var rect = new Rect(0, 0, Screen.width, Screen.height);
		GUI.DrawTexture(rect, img, ScaleMode.StretchToFill);
	}
	
	void Update() {
		/* Note: the time delay also prevents people holding down a button on one
		showImage scene, and having that skip all images after that as well. If a button
		was held down before the scene loaded, there'll be one anyKeyDown frame, and not
		necessarily the first one. But within the first second, easily. */
		
		if (Time.time - startTime < MinimumWaitTime)
			return;
			
		if (Input.anyKeyDown)
			Application.LoadLevel((Application.loadedLevel + 1) % Application.levelCount);
	}
}
