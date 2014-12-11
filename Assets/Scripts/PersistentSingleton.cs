using UnityEngine;
using System.Collections;

public class PersistentSingleton : MonoBehaviour {
	
	private static PersistentSingleton instance = null;
	public static PersistentSingleton Instance {
		get { return instance; }
	}
	
	void Awake() {
		// Only called on the first level this object is loaded into
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}
	
	void OnLevelWasLoaded() {
		// Not called on the first level this object is loaded into, but will be
		// called every time the level changes afterwards.
		audio.enabled = Application.loadedLevelName.Contains("Level");
		if (Application.loadedLevelName == "Level1") {
			audio.Stop();
			audio.Play();
		}			
	}
}
