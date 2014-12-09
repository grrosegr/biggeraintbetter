using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
public class GridScriptRemovalWizard : ScriptableWizard {
	
	[MenuItem("Wizards/Remove Scripts")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard<GridScriptRemovalWizard>("Remove scripts", "Remove scripts");
	}
	
	void OnWizardCreate()
	{
		foreach (GameObject g in UnityEditor.Selection.gameObjects) {
			foreach(var c in g.GetComponentsInChildren<MonoBehaviour>()) {
				var scriptName = c.GetType().Name;
				if (scriptName.Contains("Tile")) {
					DestroyImmediate(c);
				}
			}
		}
		Debug.Log ("It is done.");
	}
	
	void OnWizardUpdate()
	{
		helpString = "Remove all scripts in selection";
	}
	
}