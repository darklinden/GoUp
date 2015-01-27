using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class PauseButton : MonoBehaviour {

	public static PauseButton sharedInstance;
	public Button btn = null;
	public Text title = null;
	public string strPause = "Pause Game";
	public string strResume = "Resume Game";

	public delegate void DelegatePauseChanged(bool paused);
	DelegatePauseChanged pauseChangeNoti;

	void Awake () {
		if (sharedInstance == null) {
			sharedInstance = this;
		}
	}

	void Reset () {
		btn = this.GetComponent<Button> ();
		title = this.GetComponentInChildren<Text> ();
		
		if (btn == null || title == null) {
			Debug.LogError("Please Set PauseButton Component.");
		}
	}

	// Use this for initialization
	void Start () {
		btn.onClick.AddListener (PauseClicked);
	}

	public void addDelegate (DelegatePauseChanged d) {
		pauseChangeNoti += d;
	}

	public void removeDelegate (DelegatePauseChanged d) {
		pauseChangeNoti -= d;
	}

	public void clearDelegate () {
		pauseChangeNoti = null;
	}

	void sendNoti (bool paused) {
		if (pauseChangeNoti != null) {
			pauseChangeNoti(paused);
		}
	}

	void PauseClicked () {
		
		if (Time.timeScale == 1) {
			Time.timeScale = 0;
			title.text = strResume;
			sendNoti(true);
		}
		else {
			Time.timeScale = 1;
			title.text = strPause;
			sendNoti(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
