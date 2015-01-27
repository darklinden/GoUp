using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	//public
	public GameObject square;
	public GameObject circle;
	public float upSpeed = 0.001f;
	public Vector2 cameraSize;

	public int floorsCount = 10;
	public int floorCheckDownStep = 3;
	public int floorCheckUpStep = 6;
	public float squareHeight = 1f;
	public float ballSize = 1f;
	public float floorDistance = 4f;
	public float gameoverDistance = 30f;
	public float floorOffsetMin = -0.4f;
	public float floorOffsetMax = 0.4f;
	public float borderOffset = 0.4f;
	public float floorWidthScale = 0.6f;
	public int floorRangeLimitMin = 10;
	public int floorRangeLimitMax = 60;

	public bool gameOver = false;

	public Text statusFloorCount;
	public Text statusJumpCount;
	public Text statusMaxHeight;
	public Text scoreMaxHeight;
	public Text scoreRecordHeight;
	public Text statusRecordHeight;
	public Image gameoverPanel;
	
	public GameObject fireworks;

	public int floorCount = 0;
	public int jumpCount = 3;
	public float maxHeight = 0;

	public float jumpEndHeight = 0;

	//private
	private GameObject borderLeft, borderRight;
	public List<GameObject> floorsPool; // reuse pool

	Vector3 floorEuler () {
		return new Vector3 (0, 0, Random.Range(floorRangeLimitMin, floorRangeLimitMax));
	}

	float getScore () {
		float score = PlayerPrefs.GetFloat("score");
		return score;
	}

	void setScore (float s) {
		float score = PlayerPrefs.GetFloat("score");
		if (s > score) score = s;
		PlayerPrefs.SetFloat ("score", score);
		PlayerPrefs.Save ();
	}

	void setupObjects() {
		Camera.main.transform.position = new Vector3 (0, 0, -10);

		//init consts & objs
		float worldScreenHeight = Camera.main.orthographicSize * 2f;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
		cameraSize = new Vector2 (worldScreenWidth, worldScreenHeight);
		
		//border
		borderLeft = Instantiate (square) as GameObject;
		borderRight = Instantiate (square) as GameObject;
		
		SpriteRenderer sr = borderLeft.GetComponent<SpriteRenderer>();
		
		Vector3 borderScale = new Vector3(squareHeight / sr.sprite.bounds.size.x,
		                                  cameraSize.y / sr.sprite.bounds.size.y * 2f,
		                                  1);
		borderLeft.transform.localScale = borderScale;
		borderRight.transform.localScale = borderScale;
		
		borderLeft.transform.position = new Vector3 (Camera.main.transform.position.x - (cameraSize.x * borderOffset), 
		                                             Camera.main.transform.position.y,
		                                             0f);
		borderRight.transform.position = new Vector3 (Camera.main.transform.position.x + (cameraSize.x * borderOffset), 
		                                              Camera.main.transform.position.y,
		                                              0f);
		
		Vector3 floorScale = new Vector3(cameraSize.y / sr.sprite.bounds.size.y * floorWidthScale, 1f / sr.sprite.bounds.size.y, 1);
		
		//floors
		floorsPool = new List<GameObject> (floorsCount);
		
		for (int i = 0; i < floorsCount; i++) {
			
			GameObject t = Instantiate (square) as GameObject;
			t.transform.localScale = floorScale;

			t.name = "" + i;
				t.transform.position = new Vector3 (Camera.main.transform.position.x + (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
				                                    Camera.main.transform.position.y + ((i - Mathf.FloorToInt(floorsCount / 2)) * floorDistance),
				                                    0);
				t.transform.rotation = Quaternion.Euler (floorEuler());
			
			floorsPool.Add(t);
		}
	}

	void fillLabel () {
		if (Camera.main.transform.position.y > maxHeight) {
			maxHeight = Camera.main.transform.position.y;
		}

		int c = Mathf.FloorToInt (maxHeight / floorDistance);
		if (c > floorCount) {
			jumpCount += (c - floorCount) * 1;
			floorCount = c;
		}

		statusJumpCount.text = "" + jumpCount;
		statusFloorCount.text = "" + floorCount;
		statusMaxHeight.text = "" + maxHeight;
		scoreMaxHeight.text = "" + maxHeight;
	}
	
	// Use this for initialization
	void Start () {
		PauseButton.sharedInstance.addDelegate (didPauseChanged);

		setupObjects ();
		fillLabel ();

		Ball.sharedInstance.pushUp ();
		Ball.sharedInstance.setSize (new Vector3 (ballSize, ballSize, 1));
		statusRecordHeight.text = "" + getScore ();
	}

	void didPauseChanged (bool paused) {
		Debug.Log ("paused: " + paused);
	}

	public void restart() {
		if (gameOver) {
			gameOver = false;
			gameoverPanel.gameObject.SetActive (false);
			jumpCount = 10;
			floorCount = 0;
			maxHeight = 0;
			jumpEndHeight = 0;

			for (int i = 0; i < floorsCount; i++) {
				
				GameObject t = floorsPool[i];

				t.transform.position = new Vector3 (0 + (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
				                                    0 + ((i - Mathf.FloorToInt(floorsCount / 2)) * floorDistance),
				                                    0);
				t.transform.rotation = Quaternion.Euler (floorEuler());
			}

			Ball.sharedInstance.resetVelocity ();
			Ball.sharedInstance.transform.position = new Vector3(0, 0, 0);
			Ball.sharedInstance.pushUp ();
			statusRecordHeight.text = "" + getScore ();
		}
	}
	
	// Update is called once per frame
	void Update () {
//		foreach (GameObject floor in floorsPool) {
//			float y = floor.transform.position.y + upSpeed;
//			floor.transform.position = new Vector3(floor.transform.position.x, y, floor.transform.position.z);
//		}
//

		//check gameover
		if (jumpCount > 0) {
			jumpEndHeight = 0;
		}
		else {
			if (Ball.sharedInstance.transform.position.y > jumpEndHeight) {
				jumpEndHeight = Ball.sharedInstance.transform.position.y;
			}
		}

		if (jumpEndHeight - Ball.sharedInstance.transform.position.y > gameoverDistance
		    || Ball.sharedInstance.transform.position.y < -gameoverDistance) {
			if (!gameOver) {
				gameOver = true;
//				Debug.Log("game over");
				gameoverPanel.gameObject.SetActive (true);

				float oldScore = getScore();
				scoreRecordHeight.text = "" + oldScore;
				if (maxHeight > oldScore) {
					setScore(maxHeight);
					fireworks.SetActive(true);
					StartCoroutine(stopFireworks ());
				}
			}
		}

		if (!gameOver) {
			if (jumpCount > 0) {
				if (Input.GetButtonDown ("Left")) {
					Ball.sharedInstance.pushLeft ();
					jumpCount--;
				}
				else if (Input.GetButtonDown ("Right")) {
					Ball.sharedInstance.pushRight ();
					jumpCount--;
				}
				else if (Input.GetButtonDown ("Jump")) {
					Ball.sharedInstance.pushUp ();
					jumpCount--;
				}
			}

			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, 
			                                              Ball.sharedInstance.transform.position.y,
			                                              Camera.main.transform.position.z);
			borderLeft.transform.position = new Vector3 (borderLeft.transform.position.x, 
			                                             Camera.main.transform.position.y,
			                                             0f);
			borderRight.transform.position = new Vector3 (borderRight.transform.position.x, 
			                                              Camera.main.transform.position.y,
			                                              0f);
			
			if (floorsPool[floorCheckDownStep].transform.position.y < Camera.main.transform.position.y) {
				GameObject topOne = floorsPool[0];
				GameObject bottomOne = floorsPool[floorsCount - 1];
				floorsPool.Remove(bottomOne);
				
//				Debug.Log("top:" + topOne.name + " bottom:" + bottomOne.name + " bottompos:" + bottomOne.transform.position.y + " newpos:" + (topOne.transform.position.y + floorDistance));
				
				bottomOne.transform.position = new Vector3 (0 + (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
				                                            topOne.transform.position.y + floorDistance,
				                                            0);
				bottomOne.transform.rotation = Quaternion.Euler (floorEuler());
								
				floorsPool.Insert(0, bottomOne);
			}
			else if (floorsPool[floorCheckUpStep].transform.position.y > Camera.main.transform.position.y) {
				GameObject topOne = floorsPool[0];
				GameObject bottomOne = floorsPool[floorsCount - 1];
				floorsPool.Remove(topOne);

				topOne.transform.rotation = Quaternion.Euler (floorEuler());
				topOne.transform.position = new Vector3 (0 - (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
			                                         bottomOne.transform.position.y - floorDistance,
			                                         0);
				floorsPool.Add(topOne);
			}
			
//			if (Input.GetButtonUp ("Pause")) {
//				if (Time.timeScale == 0) {
//					Time.timeScale = 1;
//				}
//				else {
//					Time.timeScale = 0;
//				}
//			}

			fillLabel ();


		}
	}

	public void PushLeft() {
		if (jumpCount > 0) {
			Ball.sharedInstance.pushLeft ();
			jumpCount--;
		}
	}

	public void PushRight () {
		if (jumpCount > 0) {
			Ball.sharedInstance.pushRight ();
			jumpCount--;
		}
	}

	public void Jump () {
		if (jumpCount > 0) {
			Ball.sharedInstance.pushUp ();
			jumpCount--;
		}
	}

	IEnumerator stopFireworks () {

		yield return new WaitForSeconds (2);

		fireworks.SetActive (false);
		yield break;				
	}
}
