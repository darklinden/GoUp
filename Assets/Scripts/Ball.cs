using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D))]

public class Ball : MonoBehaviour {

	public static Ball sharedInstance;
	public float pushPower = 100f;
	public float jumpPower = 100f;

	void Awake () {
		if (sharedInstance == null) {
			sharedInstance = this;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void pushLeft () {
		rigidbody2D.AddForce (new Vector2(-pushPower, 0));
	}

	public void pushRight () {
		rigidbody2D.AddForce (new Vector2(pushPower, 0));
	}

	public void pushUp () {
		rigidbody2D.AddForce (new Vector2(0, jumpPower));
	}

	public void setSize (Vector3 size) {
		SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
		size = sr.sprite.bounds.size;
		
		transform.localScale = new Vector3 (size.x / sr.sprite.bounds.size.x,
		                                    size.y / sr.sprite.bounds.size.y, 
		                                    1);
	}

	public void resetVelocity () {
		rigidbody2D.velocity = new Vector2 (0, 0);
	}
}
