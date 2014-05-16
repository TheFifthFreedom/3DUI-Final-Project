using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour {

	public GameObject head;
	public Transform muzzlePosition;
	public GameObject muzzleEffect;
	public GameObject projectileObject;
	public GameObject entryCollider;
	public float fireRate;

	private bool fireEnabled;
	private float nextFireTime;
	private AudioSource enterSound, fireSound, cashSound, damageSound;
	private UIController ui;
	private GameObject entryZone;

	// Use this for initialization
	void Start () {
		nextFireTime = Time.time;
		enterSound = gameObject.GetComponents<AudioSource>()[0];
		fireSound = gameObject.GetComponents<AudioSource>()[1];
		cashSound = gameObject.GetComponents<AudioSource>()[2];
		damageSound = gameObject.GetComponents<AudioSource>()[3];
		ui = Camera.main.GetComponent<UIController>();
		//Transform cTransform = ((SphereCollider) gameObject.collider).transform;
		SphereCollider sCollider = (SphereCollider) gameObject.collider;
		float sRadius = sCollider.radius;// * gameObject.transform.localScale.y;
		//Debug.Log (sRadius);
		entryZone = (GameObject) Instantiate (entryCollider, Vector3.zero, Quaternion.identity);
		entryZone.transform.parent = gameObject.transform;
		entryZone.transform.localPosition = new Vector3(sCollider.center.x, sCollider.center.y, sCollider.center.z);
		entryZone.transform.localScale = new Vector3 (sRadius*2, sRadius*2, sRadius*2);
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
			if (GUI.RepeatButton(new Rect (Screen.width - 200, Screen.height / 2 - 100, 180, 200), "-- FIRE! --", customButton)
			    || Application.platform == RuntimePlatform.WindowsEditor && Input.GetKey(KeyCode.Space)) {
				// firing goes here!!!!!
				if (Time.time >= nextFireTime)  {
					fireSound.Play();
					Instantiate(muzzleEffect, muzzlePosition.position, muzzlePosition.rotation);

					// "fire" the shot

					Ray castRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
					RaycastHit hit;
					if (Physics.Raycast(castRay, out hit)) {
						//Debug.Log("HIT!");
						if (hit.collider.tag == "Enemy") {
							hit.collider.gameObject.GetComponent<EnemyConroller>().DealDamage(1);
						}
						else if (hit.collider.tag == "ItemCrate") {
							CrateController c = hit.collider.gameObject.GetComponent<CrateController>();
							if (c.crateFunction == CrateController.CrateFunction.Money) {
								ui.cash += c.cashAmount;
								cashSound.Play();
							}
							else {
								ui.lives -= c.damageAmount;
								damageSound.Play();
							}
							Destroy(hit.collider.gameObject);
						}
					}


					GameObject projectile = (GameObject) Instantiate(projectileObject, muzzlePosition.position, muzzlePosition.rotation);
					projectile.transform.parent = this.transform;

					nextFireTime = Time.time + fireRate;
				}
			}
		}
	}
}
