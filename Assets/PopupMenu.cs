using UnityEngine;
using System.Collections;

public class PopupMenu : MonoBehaviour {

	private bool displaying;
	public Texture2D PopupMenuTexture;
	public Texture2D FloatTexture;
	private int offset;
	
	// Update is called once per frame
	void Update () {
		if (displaying) {
			if (Input.anyKeyDown)
				displaying = false;
		} else {
			if (Input.GetKeyDown(KeyCode.H))
				displaying = true;
		}	
	}
	
	void OnGUI() {
		if (displaying) {
			var rect = new Rect(0, 0, Screen.width, Screen.height);
			GUI.DrawTexture(rect, PopupMenuTexture, ScaleMode.ScaleToFit);
		} 
		
		double scale = 1;
		int width = (int)(FloatTexture.width*scale);
		int height = (int)(FloatTexture.height*scale);
	
		var otherRect = new Rect(Screen.width - width, Screen.height - height, width, height);
		
		GUI.DrawTexture(otherRect, FloatTexture, ScaleMode.StretchToFill);
	}
}
