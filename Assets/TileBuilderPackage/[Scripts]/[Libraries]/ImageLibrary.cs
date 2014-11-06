using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImageLibrary : MonoBehaviour {
	//Reference.
	private static ImageLibrary _instance;
	public static List<Sprite> spriteImages = new List<Sprite>();
	public static List<Texture> textureImages = new List<Texture>();
    
	//SpriteList
	public static Dictionary<int, Sprite> spriteDictionary = new Dictionary<int, Sprite>();
    
	private ImageLibrary() {
	}
    
	public static ImageLibrary Instance {
		get {
			if (_instance == null) {
				_instance = new ImageLibrary();    
			}
            
			return _instance;
		}
	}

	public static void GetSpriteAndTextureData() {
		Object[] spriteImageFromDataPath = Resources.LoadAll(@"[Graphics]/SourceFiles/Tilesheets", typeof(Sprite));
        
		if (spriteImageFromDataPath == null) {
			return;
		}
        
		foreach (Object sprite in spriteImageFromDataPath) {
			Sprite s = (Sprite)sprite;
			spriteImages.Add(s);
		}
				
		for (int i = 0; i < spriteImages.Count; i++) {
			spriteDictionary.Add(i, spriteImages [i]);
		}
				
		foreach (Sprite sprite in spriteImageFromDataPath) {
			Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
			var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
			                                      (int)sprite.textureRect.y,
			                                      (int)sprite.textureRect.width,
			                                      (int)sprite.textureRect.height);
			
			croppedTexture.SetPixels(pixels);
			croppedTexture.name = sprite.name;
			croppedTexture.Apply();
			textureImages.Add(croppedTexture);
		}
	}
    
	public static void DestroySpriteAndTextureData() {
		spriteImages.Clear();
		textureImages.Clear();
		spriteDictionary.Clear();
	}
}
