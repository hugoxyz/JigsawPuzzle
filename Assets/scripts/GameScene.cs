using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameScene : MonoBehaviour {

	public List<GameObject> gameObjs;

	// Use this for initialization
	void Start () {
		Texture2D tex = LoadImage (Application.dataPath + "/images/test.jpg");
		gameObjs = createGameObjectList (tex, 3, 3, 8);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Texture2D LoadImage(string filePath) {
		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists(filePath)) {
			fileData = File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		}
		return tex;
	}

	List<GameObject> createGameObjectList(Texture2D tex, int width, int height, int space) {
		List<GameObject> spriteList = new List<GameObject> ();
		int spriteW = tex.width / width;
		int spriteH = tex.height / height;
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				int idx = j * width + i;
				if (space != idx) {
					Rect r = new Rect (i * spriteW, j * spriteH, spriteW, spriteH);
					Vector2 v = new Vector2 (0.5f, 0.5f);
					Sprite sp = Sprite.Create (tex, r, v);

					GameObject obj = new GameObject ("sprite" + idx);
					obj.AddComponent<SpriteRenderer> ();
					obj.GetComponent<SpriteRenderer> ().sprite = sp;
					obj.GetComponent<Transform> ().position = new Vector3 (i, j, 0);
					spriteList.Add (obj);

					//return spriteList;
				}
			}
		}

		return spriteList;
	}
}
