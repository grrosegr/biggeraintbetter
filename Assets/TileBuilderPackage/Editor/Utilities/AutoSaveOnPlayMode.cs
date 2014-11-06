using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoSaveOnPlayMode
{
    
		static AutoSaveOnPlayMode ()
		{
    
				EditorApplication.playmodeStateChanged = () =>
				{
						if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) {   
								Debug.Log ("Auto-Saving scene before entering Play mode: " + EditorApplication.currentScene);
                
								EditorApplication.SaveScene ();
								EditorApplication.SaveAssets ();
						}  
				};   
		}
}