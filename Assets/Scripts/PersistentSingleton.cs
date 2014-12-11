using UnityEngine;
using System.Collections;

public class PersistentSingleton : MonoBehaviour {
	
	private static PersistentSingleton instance = null;
	public static PersistentSingleton Instance {
		get { return instance; }
	}
	
	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}
	
	// any other methods you need
}
