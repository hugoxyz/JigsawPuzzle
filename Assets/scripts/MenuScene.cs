using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void ButtonStart() {
		Debug.Log ("start click");
		SceneManager.LoadScene ("Game");
	}

	public void ButtonSetting() {
		Debug.Log ("setting click");
	}

}
