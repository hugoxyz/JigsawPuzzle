using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameScene : MonoBehaviour {

	private List<GameObject> gameObjs;
	private GameObject controlObj;
	private UnityEngine.UI.Text stepsComp;
	private UnityEngine.UI.Text infoComp;

	private Vector2 cameraSizeUnit;
	private Vector2 pieceSizeUnit;
	private Piece target;

	private int column;
	private int row;

	// Use this for initialization
	void Start () {
		Gesture g = GetComponent<Gesture> ();
		g.onMove = this.OnMoveDirection;
		column = 3;
		row = 3;

		resetCameraSizeUnit ();
		pieceSizeUnit = new Vector2(cameraSizeUnit.x / (column + 1), cameraSizeUnit.y / row);
		createControlPanel ();

		Texture2D tex = LoadImage (Application.dataPath + "/images/test.jpg");
		gameObjs = createGameObjectList (tex, column, row, 8);
		disorder (100);
		resetGameObjectPosition ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	private void resetCameraSizeUnit() {
		float h = Camera.main.orthographicSize * 2;
		float w = h * Screen.width / Screen.height;
		cameraSizeUnit = new Vector2 (w, h);
		//Debug.Log (string.Format("Camera Size Unit: {0}", cameraSizeUnit));
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

	List<GameObject> createGameObjectList(Texture2D tex, int colomn, int row, int space) {
		List<GameObject> spriteList = new List<GameObject> ();
		GameObject CameraObj = GameObject.Find ("Main Camera");
		if (null == CameraObj) {
			print ("camera obj is null");
			return spriteList;
		}
		Camera camera = CameraObj.GetComponent<Camera>();
		if (null == camera) {
			print ("can not find camera component");
			return spriteList;
		}

		Debug.Log (string.Format("Screen W: {0}, H: {1}", Screen.width, Screen.height));

		float spriteW = tex.width / colomn;
		float spriteH = tex.height / row;
		Vector3 pieceSize = new Vector3 (cameraSizeUnit.x/ (colomn + 1), cameraSizeUnit.y/ row, 1);
		for (int j = 0; j < row; j++) {
			for (int i = 0; i < colomn; i++) {
				int idx = j * colomn + i;
				spriteList.Add (createPiece(idx, i, j, pieceSize, tex, spriteW, spriteH));
			}
		}

		spriteList.Add(createBlank (colomn * row, colomn, row - 1, pieceSize));

		return spriteList;
	}

	GameObject createObj(int idx, int i, int j, Vector3 pieceUnitSize) {
		GameObject obj = new GameObject ("sprite" + idx);

		Piece piece = obj.AddComponent<Piece> ();
		piece.sizeInUnit = pieceUnitSize;
		piece.idx = idx;
		piece.actualIdx = piece.idx;

		return obj;
	}

	GameObject createPiece(int idx, int i, int j, Vector3 pieceUnitSize, Texture2D tex, float spriteW, float spriteH) {
		GameObject obj = createObj(idx, i, j, pieceUnitSize);

		Piece p = obj.GetComponent<Piece> ();
		p.blank = false;
		p.sp = createSprite(tex,
			new Rect (i * spriteW, j * spriteH, spriteW, spriteH),
			new Vector2(0.5f, 0.5f));

		return obj;
	}

	GameObject createBlank(int idx, int i, int j, Vector3 pieceUnitSize) {
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, new Color(0, 0, 0, 0));
		texture.Apply();

		GameObject obj = createObj(idx, i, j, pieceUnitSize);

		Piece piece = obj.GetComponent<Piece> ();
		piece.blank = true;
		piece.sp = Sprite.Create (texture, new Rect (0, 0, 1, 1), new Vector2 (0.5f, 0.5f));
		piece.onMoveFinish = this.MoveFinish;

		return obj;
	}

	void resetGameObjectPosition() {
		Vector3 pieceUnitSize = gameObjs [0].GetComponent<Piece> ().sizeInUnit;
		GameObject obj;
		Vector2 pos;
		for (int i = 0; i < column; i++) {
			for (int j = 0; j < row; j++) {
				obj = gameObjs [j * column + i];
				pos = new Vector2 ((i + 0.5f) * pieceUnitSize.x, (j + 0.5f) * pieceUnitSize.y);
				obj.transform.position = pos - new Vector2 (cameraSizeUnit.x/2, cameraSizeUnit.y/2);
			}
		}

		pos = new Vector2 ((column + 0.5f) * pieceUnitSize.x, (row - 0.5f) * pieceUnitSize.y);
		gameObjs[gameObjs.Count - 1].transform.position = pos - new Vector2 (cameraSizeUnit.x/2, cameraSizeUnit.y/2);
	}

	private Sprite createSprite(Texture2D tex, Rect r, Vector2 pivotal) {
		return Sprite.Create (tex, r, pivotal);
	}

	void OnMoveDirection(Gesture.Direction dir, Vector3 vec) {
		Piece p = null;
		foreach (GameObject obj in gameObjs) {
			p = obj.GetComponent<Piece> ();
			if (null != p && p.hit (vec)) {
				break;
			}
			p = null;
		}

		if (null == p) {
			Debug.Log ("Can't find touch piece");
		} else if (p.blank) {
			Debug.Log ("This is blank, ignore");
		} else {
			GameObject blank = getBlankPiece ();
			Piece blankPiece = blank.GetComponent<Piece> ();
			if (canMove (p, blankPiece, dir)) {
				Debug.Log ("Can Move");
				p.Move (dir);
				blankPiece.Move (dir, reverse: true);
				p.idx = p.idx ^ blankPiece.idx;
				blankPiece.idx = p.idx ^ blankPiece.idx;
				p.idx = p.idx ^ blankPiece.idx;
			} else {
				Debug.Log ("Can't Move");
				//Debug.Log (string.Format ("Direction: {0}, Vec: {1}", dir, p.gameObject.name));
			}
		}
	}

	GameObject getBlankPiece() {
		Piece p = null;
		foreach (GameObject obj in gameObjs) {
			Piece t = obj.GetComponent<Piece> ();
			if (null == p) {
				p = t;
			} else if (p.actualIdx < t.actualIdx) {
				p = t;
			}
		}

		return p.gameObject;
	}

	bool canMove(Piece src, Piece dst, Gesture.Direction dir) {
		int dis = dst.idx - src.idx;
		if (1 == dis && Gesture.Direction.Right == dir) {
			return true;
		} else if (-1 == dis && Gesture.Direction.Left == dir) {
			return true;
		} else if (3 == dis && Gesture.Direction.Up == dir) {
			return true;
		} else if (-3 == dis && Gesture.Direction.Down == dir) {
			return true;
		} else {
			return false;
		}
	}

	void MoveFinish() {
		bool bOver = true;
		foreach (GameObject obj in gameObjs) {
			Piece p = obj.GetComponent<Piece> ();
			if (p.idx != p.actualIdx) {
				bOver = false;
				break;				
			}
		}

		if (bOver) {
			Debug.Log ("game success");
		} else {
			Debug.Log ("need more move");
		}
	}

	void disorder(int step) {
		GameObject blankObj = getBlankPiece ();
		int blankIdx = blankObj.GetComponent<Piece> ().idx;

		for (int i = 0; i < step; i++) {
			blankIdx = randomMoveBlank (blankIdx, gameObjs);
		}
	}

	int randomMoveBlank(int blankIdx, List<GameObject> objList) {
		List<int> nearby = new List<int> ();
		if (blankIdx == objList.Count - 1) {
			nearby.Add (blankIdx - 1);
		} else {
			if (blankIdx == objList.Count - 2) {
				nearby.Add (blankIdx + 1);
			}

			int c = blankIdx % column;
			if (0 != c) {
				nearby.Add (blankIdx - 1);
			}
			if (column - 1 != c) {
				nearby.Add (blankIdx + 1);
			}
			if (blankIdx >= column) {
				nearby.Add (blankIdx - column);
			}
			if (blankIdx + column < objList.Count - 1) {
				nearby.Add (blankIdx + column);
			}
		}

		if (0 == nearby.Count) {
			Debug.Log ("impossable");
		}
		int target = nearby[Random.Range (0, nearby.Count)];
		objList [blankIdx].GetComponent<Piece> ().idx = target;
		objList [target].GetComponent<Piece> ().idx = blankIdx;
		GameObject blankObj = objList [blankIdx];
		objList [blankIdx] = objList [target];
		objList[target] = blankObj;

		return target;
	}

	private void createControlPanel() {
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, new Color(64/255f, 75/255f, 94/255f));
		texture.Apply();

		controlObj = new GameObject ("control panel");
		controlObj.transform.position = new Vector3 (pieceSizeUnit.x * (column + 0.5f), pieceSizeUnit.y * (row - 1) / 2, 0f);
		controlObj.transform.position -= new Vector3 (cameraSizeUnit.x / 2, cameraSizeUnit.y / 2, 0);
		SpriteRenderer renderer = controlObj.AddComponent<SpriteRenderer> ();
		renderer.sprite = Sprite.Create (texture, new Rect (0, 0, 1, 1), new Vector2 (0.5f, 0.5f));
		controlObj.transform.localScale = new Vector3 (pieceSizeUnit.x / renderer.sprite.bounds.size.x,
			pieceSizeUnit.y * (row - 1) / renderer.sprite.bounds.size.y,
			1);
	}

	private void showInfo(string info) {
	}

}
