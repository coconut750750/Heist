﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public abstract class MovingObject : MonoBehaviour {

	public GameObject doorTilemapGameObj;

	public SortingLayer ground;
	public SortingLayer floor2;

	private Rigidbody2D rb2D;
	private bool paused = false;

	private const string DOOR_TAG = "Door";
	private const float DOOR_DELAY_SECONDS = 0.02f;
	private Sprite currentDoorSprite;

	private const string STAIRS_TAG = "Stairs";
	private int onStairs = 0;

	private int floor = 0;

	IEnumerator doorDelay(Collider2D door) {
		paused = true;
		yield return new WaitForSeconds(DOOR_DELAY_SECONDS);
		paused = false;
		door.gameObject.GetComponent<SpriteRenderer> ().sprite = null;
	}

	void Awake() {
		rb2D = GetComponent<Rigidbody2D>();
		rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb2D.interpolation = RigidbodyInterpolation2D.Extrapolate;
		rb2D.freezeRotation = true;

	}

	// Use this for initialization
	protected virtual void Start () {
	}

	protected abstract void FixedUpdate ();

	protected void Move(Vector3 movement, float moveSpeed) {
		if (!paused) {
			rb2D.velocity = movement * moveSpeed;
		} else {
			rb2D.velocity = new Vector3(0, 0, 0);
		}
	}

	protected void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag (DOOR_TAG)) {
			StartCoroutine (doorDelay (other));
			currentDoorSprite = other.gameObject.GetComponent<SpriteRenderer> ().sprite; 
		} else if (other.gameObject.CompareTag (STAIRS_TAG)) {
			if (onStairs == 0) {
				floor = 1 - floor;
				rb2D.transform.position = new Vector3 (rb2D.transform.position.x, rb2D.transform.position.y, 
					0 - floor);
				gameObject.layer = 17 - gameObject.layer;
				gameObject.GetComponent<SpriteRenderer> ().sortingLayerName = floor2;
				onStairs += 2;
				Debug.Log (onStairs);
			}
		}
	}

	protected void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag (DOOR_TAG)) {
			other.gameObject.GetComponent<SpriteRenderer> ().sprite = currentDoorSprite;
		} else if (other.gameObject.CompareTag (STAIRS_TAG) && onStairs > 0) {
			onStairs -= 1;
			Debug.Log (onStairs);
		}
	}
}
