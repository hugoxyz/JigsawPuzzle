using UnityEngine;
using System.Collections;

public class Gesture : MonoBehaviour {

	private bool handle;
	public float threshold;
	private int mouseState; // 0:none, 1:down, 2:drag, 3:up
	private Vector3 downPos;
	public enum Direction {
		Left,
		Right,
		Up,
		Down
	}

	public delegate void onMoveDelegate(Direction dir, Vector3 pos);

	public onMoveDelegate onMove;

	// Use this for initialization
	void Start () {
		threshold = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			if (0 == mouseState) {
				mouseState = 1;
			} else if (1 == mouseState) {
				mouseState = 2;
			}
		} else {
			if (1 == mouseState || 2 == mouseState) {
				mouseState = 3;
			} else if (3 == mouseState) {
				mouseState = 0;
			}
		}

		if (1 == mouseState) {
			OnMouseDown ();
		} else if (2 == mouseState) {
			OnMouseDrag ();
		} else if (3 == mouseState) {
			OnMouseUp ();
		}
	}

	private void OnMouseDown() {
		handle = false;
		downPos = Input.mousePosition;
	}

	private void OnMouseDrag() {
		if (handle) {
			return;
		}

		Vector3 director = Input.mousePosition - downPos;
		if (director.magnitude < threshold) {
			return;
		}

		if (null != onMove) {
			onMove (detectDirection (director), downPos);
			handle = true;
		}
	}

	private void OnMouseUp() {		
	}

	private Direction detectDirection(Vector3 vec) {
		Direction dir;
		if (Mathf.Abs (vec.x) > Mathf.Abs (vec.y)) {
			if (vec.x > 0) {
				dir = Direction.Right;
			} else {
				dir = Direction.Left;
			}
		} else {
			if (vec.y > 0) {
				dir = Direction.Up;
			} else {
				dir = Direction.Down;
			}
		}

		return dir;
	}
}
