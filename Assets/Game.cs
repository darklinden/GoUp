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
	public float floorOffsetMin = 0.125f;
	public float floorOffsetMax = 0.375f;
	public float floorWidthScale = 0.6f;
	public int floorRangeLimitMin = 10;
	public int floorRangeLimitMax = 60;

	public float ballPower = 100f;

	public bool gameOver = false;

	public Text statusFloorCount;
	public Text statusJumpCount;
	public Text statusMaxHeight;
	public Text scoreMaxHeight;
	public Text scoreRecordHeight;
	public Text statusRecordHeight;
	public Image gameoverPanel;

	public Button btnLeft;
	public Button btnRight;
	public Button btnJump;
	public Text btnShowHide;
	public GameObject fireworks;

	public int floorCount = 0;
	public int jumpCount = 10;
	public float maxHeight = 0;

	public float jumpEndHeight = 0;

	//private
	private GameObject borderLeft, borderRight;
	public List<GameObject> floorsPool; // reuse pool
	private GameObject ball;

	Vector3 leftEuler () {

		return new Vector3 (0, 0, -Random.Range(floorRangeLimitMin, floorRangeLimitMax));
	}

	Vector3 rightEuler () {
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
		
		borderLeft.transform.position = new Vector3 (Camera.main.transform.position.x - (cameraSize.x * 0.4f), 
		                                             Camera.main.transform.position.y,
		                                             0f);
		borderRight.transform.position = new Vector3 (Camera.main.transform.position.x + (cameraSize.x * 0.4f), 
		                                              Camera.main.transform.position.y,
		                                              0f);
		
		Vector3 floorScale = new Vector3(cameraSize.y / sr.sprite.bounds.size.y * floorWidthScale, 1f / sr.sprite.bounds.size.y, 1);
		
		//floors
		floorsPool = new List<GameObject> (floorsCount);
		
		for (int i = 0; i < floorsCount; i++) {
			
			GameObject t = Instantiate (square) as GameObject;
			t.transform.localScale = floorScale;

			t.name = "" + i;

//			Debug.Log(i + "-" + (Camera.main.transform.position.y + ((i - Mathf.FloorToInt(floorsCount / 2)) * floorDistance)));

			if (i % 2 == 0) {
				t.transform.position = new Vector3 (Camera.main.transform.position.x - (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
				                                    Camera.main.transform.position.y + ((i - Mathf.FloorToInt(floorsCount / 2)) * floorDistance),
				                                    0);
				t.transform.rotation = Quaternion.Euler (leftEuler());
			}
			else {
				t.transform.position = new Vector3 (Camera.main.transform.position.x + (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
				                                    Camera.main.transform.position.y + ((i - Mathf.FloorToInt(floorsCount / 2)) * floorDistance),
				                                    0);
				t.transform.rotation = Quaternion.Euler (rightEuler());
			}
			
			floorsPool.Add(t);
		}
		
		//ball
		ball = Instantiate (circle) as GameObject;
		SpriteRenderer bsr = ball.GetComponent<SpriteRenderer>();
		ball.transform.localScale = new Vector3 (ballSize / bsr.sprite.bounds.size.x, ballSize / bsr.sprite.bounds.size.y, 1);
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
		PlayerPrefs.DeleteAll ();

		setupObjects ();
		fillLabel ();
		ball.rigidbody2D.AddForce (new Vector2(0, ballPower));
		statusRecordHeight.text = "" + getScore ();
	}

	public void restart() {
		if (gameOver) {
			ball.transform.position = new Vector3(0, 0, 0);
			ball.rigidbody2D.velocity = new Vector2(0, 0);
			gameOver = false;
			gameoverPanel.gameObject.SetActive (false);
			ball.transform.position = new Vector3(0, 0, 0);
			jumpCount = 10;
			floorCount = 0;
			maxHeight = 0;
			jumpEndHeight = 0;

			for (int i = 0; i < floorsCount; i++) {
				
				GameObject t = floorsPool[i];

				t.name = "" + i;
				
				if (i % 2 == 0) {
					t.transform.position = new Vector3 (0 - (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
					                                    0 + ((i - Mathf.FloorToInt(floorsCount / 2)) * floorDistance),
					                                    0);
					t.transform.rotation = Quaternion.Euler (leftEuler());
				}
				else {
					t.transform.position = new Vector3 (0 + (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
					                                    0 + ((i - Mathf.FloorToInt(floorsCount / 2)) * floorDistance),
					                                    0);
					t.transform.rotation = Quaternion.Euler (rightEuler());
				}
			}

			ball.rigidbody2D.AddForce (new Vector2(0, ballPower));
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
			if (ball.transform.position.y > jumpEndHeight) {
				jumpEndHeight = ball.transform.position.y;
			}
		}

		if (jumpEndHeight - ball.transform.position.y > gameoverDistance
		    || ball.transform.position.y < -gameoverDistance) {
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
					ball.rigidbody2D.AddForce (new Vector2(-ballPower, 0));
					jumpCount--;
				}
				else if (Input.GetButtonDown ("Right")) {
					ball.rigidbody2D.AddForce (new Vector2(ballPower, 0));
					jumpCount--;
				}
				else if (Input.GetButtonDown ("Jump")) {
					ball.rigidbody2D.AddForce (new Vector2(0, ballPower));
					jumpCount--;
				}
			}

			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, ball.transform.position.y, Camera.main.transform.position.z);
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
				
				bottomOne.transform.position = new Vector3(bottomOne.transform.position.x, topOne.transform.position.y + floorDistance, bottomOne.transform.position.z);
				
				int index = int.Parse(bottomOne.name);
				if (index % 2 == 0) {
					bottomOne.transform.rotation = Quaternion.Euler (leftEuler());
				}
				else {
					bottomOne.transform.rotation = Quaternion.Euler (rightEuler());
				}
				
				floorsPool.Insert(0, bottomOne);
			}
			else if (floorsPool[floorCheckUpStep].transform.position.y > Camera.main.transform.position.y) {
				GameObject topOne = floorsPool[0];
				GameObject bottomOne = floorsPool[floorsCount - 1];
				floorsPool.Remove(topOne);
				
//				Debug.Log("top:" + topOne.name + " bottom:" + bottomOne.name + " toppos:" + topOne.transform.position.y + " newpos:" + (bottomOne.transform.position.y - floorDistance));
				
				int index = int.Parse(topOne.name);
				if (index % 2 == 0) {
					topOne.transform.rotation = Quaternion.Euler (leftEuler());
					topOne.transform.position = new Vector3(0 - (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
					                                        bottomOne.transform.position.y - floorDistance,
					                                        topOne.transform.position.z);
				}
				else {
					topOne.transform.rotation = Quaternion.Euler (rightEuler());
					topOne.transform.position = new Vector3(0 + (Random.Range(floorOffsetMin, floorOffsetMax) * cameraSize.x),
					                                        bottomOne.transform.position.y - floorDistance,
					                                        topOne.transform.position.z);
				}
				
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
			ball.rigidbody2D.AddForce (new Vector2(-ballPower, 0));
			jumpCount--;
		}
	}

	public void PushRight () {
		if (jumpCount > 0) {
			ball.rigidbody2D.AddForce (new Vector2(ballPower, 0));
			jumpCount--;
		}
	}

	public void Jump () {
		if (jumpCount > 0) {
			ball.rigidbody2D.AddForce (new Vector2(0, ballPower));
			jumpCount--;
		}
	}

	public void ShowHideButton () {
		if (btnLeft.gameObject.activeSelf) {
			btnLeft.gameObject.SetActive(false);
			btnRight.gameObject.SetActive(false);
			btnJump.gameObject.SetActive(false);
			btnShowHide.text = "Show Buttons";
		}
		else {
			btnLeft.gameObject.SetActive(true);
			btnRight.gameObject.SetActive(true);
			btnJump.gameObject.SetActive(true);
			btnShowHide.text = "Hide Buttons";
		}
	}
	
	IEnumerator stopFireworks () {
		yield return new WaitForSeconds (2);

		fireworks.SetActive (false);
		yield break;				
	}
}
