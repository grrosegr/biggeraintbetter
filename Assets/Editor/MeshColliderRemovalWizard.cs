using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
public class RemoveColliders : ScriptableWizard {
	
	[MenuItem("Wizards/Remove Mesh Colliders")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard<RemoveColliders>("Remove mesh colliders", "Remove Colliders");
	}
	
	void OnWizardCreate()
	{
		foreach (GameObject g in UnityEditor.Selection.gameObjects) {
			foreach(MeshCollider c in g.GetComponentsInChildren<MeshCollider>()) {
				DestroyImmediate(c);
			}
		}
	}
	void OnWizardUpdate()
	{
		helpString = "Remove mesh colliders to all items that have a mesh renderer";
	}
	
}