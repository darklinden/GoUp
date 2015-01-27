using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FullScreenButton : MonoBehaviour {

	public Button btn = null;
	public Text title = null;
	public string strFull = "Full Screen";
	public string strNomal = "Back To Nomal";

	void Reset () {
		btn = this.GetComponent<Button> ();
		title = this.GetComponentInChildren<Text> ();

		if (btn == null || title == null) {
			Debug.LogError("Please Set FullScreenButton Component.");
		}
	}

	// Use this for initialization
	void Start () {
		btn.onClick.AddListener (FullScreenClicked);
	}

	void FullScreenClicked () {

		Screen.SetResolution (Screen.currentResolution.width, Screen.currentResolution.height, true);
		Screen.fullScreen = !Screen.fullScreen;

		if (Screen.fullScreen) {
			title.text = strNomal;
		}
		else {
			title.text = strFull;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
