using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowComponentButton : MonoBehaviour {

	public Button btn = null;
	public Text title = null;
	public GameObject[] components;
	public string strShow = "Show Buttons";
	public string strHide = "Hide Buttons";
	public string saveKey = "ShowComponent"; 
	
	void Reset () {
		btn = this.GetComponent<Button> ();
		title = this.GetComponentInChildren<Text> ();
		
		if (btn == null || title == null) {
			Debug.LogError("Please Set ShowComponentButton Component.");
		}
	}

	// Use this for initialization
	void Start () {
		btn.onClick.AddListener (ShowComponentClicked);
		int show = PlayerPrefs.GetInt (saveKey, 0);
		if (show == 0) {
			foreach (GameObject o in components) {
				o.SetActive (false);
			}
			title.text = strShow;
		}
		else {
			foreach (GameObject o in components) {
				o.SetActive (true);
			}
			title.text = strHide;
		}
	}

	void ShowComponentClicked () {

		GameObject go = null;
		if (components.Length > 0) go = components [0]; 

		if (go.activeSelf) {
			foreach (GameObject o in components) {
				o.SetActive (false);
			}
			title.text = strShow;
			PlayerPrefs.SetInt(saveKey, 0);
		}
		else {
			foreach (GameObject o in components) {
				o.SetActive (true);
			}
			title.text = strHide;
			PlayerPrefs.SetInt(saveKey, 1);
		}

		PlayerPrefs.Save ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
