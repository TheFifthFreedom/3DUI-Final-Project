using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour {

	public GameObject head;
	public Transform muzzlePosition;
	public GameObject muzzleEffect;
	public GameObject projectileObject;
	public float fireRate;

	private bool fireEnabled;
	private float nextFireTime;
	private AudioSource enterSound, fireSound;
	private UIController ui;

	// Use this for initialization
	void Start () {
		nextFireTime = Time.time;
		enterSound = gameObject.GetComponents<AudioSource>()[0];
		fireSound = gameObject.GetComponents<AudioSource>()[1];
		ui = Camera.main.GetComponent<UIController>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "MainCamera") {
			fireEnabled = true;
			ui.SetBehindTurret(true);
			enterSound.Play();
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "MainCamera") {
			Ray castRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			RaycastHit hit;
			if (Physics.Raycast(castRay, out hit)) {
				head.transform.LookAt(hit.point);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "MainCamera") {
			fireEnabled = false;
			ui.SetBehindTurret(false);
		}
	}

	void OnGUI() {
		if (fireEnabled) {

			// display text box under button
			GUIStyle textBox = new GUIStyle(GUI.skin.box);
			textBox.fontSize = 16;
			textBox.fontStyle = FontStyle.Bold;
			textBox.alignment = TextAnchor.MiddleLeft;
			GUI.Box (new Rect (Screen.width - 200, Screen.height / 2 + 120, 180, 180), "MOUNTED TURRET\nACTIVATED!\n\nPress \"FIRE!\" to " +
				"fire.\n\nYou cannot build or\nmodify towers while\nbehind a turret.", textBox);

			GUIStyle customButton = new GUIStyle(GUI.skin.button);
			customButton.fontSize = 24;
			customButton.fontStyle = FontStyle.Bold;
			// firing during play mode
			if (GUI.RepeatButton(new Rect (Screen.width - 200, Screen.height / 2 - 100, 180, 200), "-- FIRE! --", customButton)) {
				// firing goes here!!!!!
				if (Time.time >= nextFireTime)  {
					fireSound.Play();
					Instantiate(muzzleEffect, muzzlePosition.position, muzzlePosition.rotation);

					// "fire" the shot
					/*
					Ray castRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
					RaycastHit hit;
					if (Physics.Raycast(castRay, out hit)) {
						//Debug.Log("HIT!");
						if (hit.collider.tag == "Enemy") {
							//hit.collider.gameObject.GetComponent<EnemyConroller>().DealDamage(1);
						}
					}
					*/

					GameObject projectile = (GameObject) Instantiate(projectileObject, muzzlePosition.position, muzzlePosition.rotation);
					projectile.transform.parent = this.transform;

					nextFireTime = Time.time + fireRate;
				}
			}
		}
	}
}
