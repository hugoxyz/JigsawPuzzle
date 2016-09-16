using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	public Sprite sp;
	public Vector3 sizeInUnit;
	public int idx;
	public int actualIdx;
	public bool blank;

	private bool touchDown;
	private Vector3 downPos;
	private Rect touchRect;
	private Vector3 destPos;
	private bool moving;

	public delegate void onMoveFinishDelegate();
	public onMoveFinishDelegate onMoveFinish;

	// Use this for initialization
	void Start () {
		if (null == gameObject) {
			return;
		}

		gameObject.AddComponent<SpriteRenderer> ();
		gameObject.GetComponent<SpriteRenderer> ().sprite = sp;
		gameObject.transform.localScale =
			new Vector3(sizeInUnit.x / sp.bounds.size.x, sizeInUnit.y / sp.bounds.size.y, 1);

		touchRect = getRect();
	}
	
	// Update is called once per frame
	void Update () {
		if (moving) {
			Vector3 direction = destPos - transform.position;
			if (direction.magnitude > 0.5f) {
				direction.Normalize();
				transform.position = transform.position + direction * 10f * Time.deltaTime;
			} else {
				// Without this game object jumps around target and never settles
				transform.position = destPos;
				moving = false;
				touchRect = getRect();
				if (null != onMoveFinish) {
					onMoveFinish ();
				}
			}
		}
	}

	public bool hit(Vector3 pos) {
		Vector3 wPos = Camera.main.ScreenToWorldPoint (pos);
		if (touchRect.Contains (wPos)) {
			return true;
		} else {
			return false;
		}
	}

	Rect getRect() {
		Rect r = new Rect ();
		r.size = new Vector2(sp.bounds.size.x * gameObject.transform.localScale.x,
			sp.bounds.size.y * gameObject.transform.localScale.y);
		r.center = gameObject.transform.position;

		return r;
	}

	public void Move(Gesture.Direction dir, bool animation = true, bool reverse = false) {
		setDestPosition (dir, reverse);
		if (animation) {
			moving = true;
		} else {
			gameObject.transform.position = destPos;
			if (null != onMoveFinish) {
				onMoveFinish ();
			}
		}
	}

	private void setDestPosition(Gesture.Direction dir, bool reverse) {
		switch (dir) {
		case Gesture.Direction.Left:
			{
				destPos = new Vector3 (-touchRect.size.x, 0, 0);
				break;
			}
		case Gesture.Direction.Right:
			{
				destPos = new Vector3 (touchRect.size.x, 0, 0);
				break;
			}
		case Gesture.Direction.Up:
			{
				destPos = new Vector3 (0, touchRect.size.y, 0);
				break;
			}
		case Gesture.Direction.Down:
			{
				destPos = new Vector3 (0, -touchRect.size.y, 0);
				break;
			}
		}
		if (reverse) {
			destPos = -destPos;
		}
		destPos += gameObject.transform.position;
	}

}
