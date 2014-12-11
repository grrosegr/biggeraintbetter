using UnityEngine;
using System.Collections;

public class Cheater : MonoBehaviour {
	
	public bool ScaleIn = true;
	
	private static Cheater _instance;
	public static Cheater Instance {
		get {
			return _instance;
		}
	}
	
	void Awake() {
		_instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.N))
			Application.LoadLevel((Application.loadedLevel + 1) % Application.levelCount);
		if (Input.GetKeyDown(KeyCode.P)) {
			if (Application.loadedLevel == 0)
				Application.LoadLevel(Application.levelCount - 1);
			else
				Application.LoadLevel(Application.loadedLevel - 1);
		}
			
			
//		if (Input.GetKeyDown(KeyCode.M))
//			controller.GetMushroom();
	}
}
