using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	// selection indicator prefabs
	public GameObject selectionCollider, selectionIndicator;

	// tower prefabs
	public GameObject basicCannon, basicRocketLauncher, basicRadiator;

	// minimap camera
	public Camera minimapCamera;

	private GameObject newTower; // temporary for creating new towers
	private SpawnController spawner; // spawn controller 
	private GameObject selectedObject; // the currently selected object
	private TowerController selectedTowerInfo; // to get info on selected tower
	private string colliderTag; // tag of collider for raycasting
	private GameObject vCollider, vIndicator;

	private bool repositionTower;
	private bool playMode, behindTurret;
	public int waveNumber, lives, cash;

	private const int PRICE_CANNON = 10, PRICE_ROCKET = 20, PRICE_RADIATOR = 35;

	// Use this for initialization
	void Start () {
		repositionTower = false;
		spawner = GameObject.Find("SpawnLocation").GetComponent<SpawnController>();
		playMode = false;
	}

	// Update is called once per frame
	void Update () {

		// if we're currently in the process of moving a tower
		if (repositionTower) {
			Ray castRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			RaycastHit hit;
			// point a ray through camera center, place tower where ray hits the ground
			if (Physics.Raycast(castRay, out hit)) {
				if (hit.collider.tag == "Ground") {
					selectedObject.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
				}
			}
		}
		// if we're not moving a tower
		else {
			// if the screen is tapped
			if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
				Ray castRay;
				if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
					castRay = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
				}
				else {
					castRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
				}
				RaycastHit hit;
				// cast a ray through the touch point and see if it hits a tower
				if (Physics.Raycast(castRay, out hit)) {
					colliderTag = hit.collider.tag;
					if (colliderTag == "Tower") {
						// if a tower is tapped, deselect everything and select the tapped tower
						if (selectedObject != null) {
							DeselectAll();
						}
						Select(hit.collider.transform.parent.gameObject);
					}
					// if a tower is not hit, deselect everything
					else {
						if (selectedObject != null && vCollider != null && vIndicator != null) {
							DeselectAll();
						}
					}
				}
				// if nothing is hit, deselect everything
				else {
					if (selectedObject != null && vCollider != null && vIndicator != null) {
						DeselectAll();
					}
				}
			}
		}

	}

	// selects an object by instantiating the selection indicator around it
	void Select(GameObject o) {
		selectedObject = o;
		// sight radius
		vCollider = (GameObject) Instantiate(selectionCollider, o.transform.position, Quaternion.identity);
		float diameter = o.transform.localScale.y * ((SphereCollider) o.collider).radius * 2;
		vCollider.transform.localScale = new Vector3(diameter, diameter, diameter);
		vCollider.transform.parent = o.transform;
		// small white sphere around tower
		vIndicator = (GameObject) Instantiate(selectionIndicator, o.transform.position, Quaternion.identity);
		diameter = o.transform.localScale.y * 3;
		vIndicator.transform.localScale = new Vector3(diameter, diameter, diameter);
		vIndicator.transform.parent = o.transform;
		// get info to display
		selectedTowerInfo = o.GetComponent<TowerController>();
	}

	// deselects the selected object by destroying the indicators and nullifying the selection
	void DeselectAll() {
		Destroy(vCollider);
		Destroy(vIndicator);
		selectedObject = null;
	}

	// spawns a tower where the camera is pointing
	void CreateTowerAtCamera(GameObject tower) {
		Ray castRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		RaycastHit hit;
		if (Physics.Raycast(castRay, out hit)) {
			colliderTag = hit.collider.tag;
			if (colliderTag == "Ground") {
				newTower = (GameObject) Instantiate(tower, hit.point, Quaternion.identity);
				Select(newTower);
				repositionTower = true;
			}
		}
	}

	public void SetBehindTurret(bool b) {
		behindTurret = b;
	}

	void OnGUI() {

		// display aiming reticle
		GUIStyle textBox = new GUIStyle(GUI.skin.box);
		textBox.fontSize = 30;
		GUI.Box (new Rect (Screen.width / 2 - 20, Screen.height / 2 - 20, 40, 40), "+", textBox);

		// display lives and cash
		textBox.fontStyle = FontStyle.Bold;
		textBox.alignment = TextAnchor.MiddleLeft;
		GUI.Box (new Rect (20, 20, 250, 110), "WAVE: " + waveNumber + "\nLIVES: " + lives + "\nCASH: $" + cash, textBox);

		// display buttons
		GUIStyle customButton = new GUIStyle(GUI.skin.button);
		customButton.fontSize = 24;
		customButton.fontStyle = FontStyle.Bold;
		// if we are in play phase...
		if (playMode) {
			// if all enemies are destroyed and the spawner has deactivated, play phase is over
			if (spawner.isActive == false && GameObject.FindGameObjectWithTag("Enemy") == null) {
				playMode = false;
			}
		}
		GUI.enabled = !playMode;
		// if start next wave is tapped, activate spawner and play mode
		if (GUI.Button (new Rect (Screen.width - 200, 20, 180, 130), "START\nNEXT WAVE", customButton)) {
			spawner.isActive = true;
			spawner.enemyHealthAtSpawn++;
			playMode = true;
			waveNumber++;
		}
		GUI.enabled = true;
		// build controls
		// if nothing is selected, show buttons for building towers
		if (selectedObject == null) {
			if (!behindTurret) {
				GUI.enabled = cash >= PRICE_CANNON;
				if (GUI.Button (new Rect (Screen.width - 600, Screen.height - 150, 180, 130), "BUILD\nCANNON\n-- $" + PRICE_CANNON + " --", customButton)) {
					CreateTowerAtCamera(basicCannon);
				}
				GUI.enabled = cash >= PRICE_ROCKET;
				if (GUI.Button (new Rect (Screen.width - 400, Screen.height - 150, 180, 130), "BUILD\nLAUNCHER\n-- $" + PRICE_ROCKET + " --", customButton)) {
					CreateTowerAtCamera(basicRocketLauncher);
				}
				GUI.enabled = cash >= PRICE_RADIATOR;
				if (GUI.Button (new Rect (Screen.width - 200, Screen.height - 150, 180, 130), "BUILD\nRADIATOR\n-- $" + PRICE_RADIATOR + " --", customButton)) {
					CreateTowerAtCamera(basicRadiator);
				}
				GUI.enabled = true;
			}
		}
		// otherwise...
		else {
			// show object properties
			textBox.fontSize = 22;
			string info = "SELECTED TOWER\n\n";
			info += "TYPE: " + selectedTowerInfo.towerType + "\n\n";
			info += "DAMAGE: " + selectedTowerInfo.damage + "\n";
			info += "RATE: " + selectedTowerInfo.reloadTime + " s\n";
			info += "RANGE: " + ((SphereCollider) selectedTowerInfo.gameObject.collider).radius * 3;
			GUI.Box (new Rect (20, 150, 250, 190), info, textBox);

			// if we are moving a tower, show button for placing tower
			if (repositionTower) {
				if (GUI.Button (new Rect (Screen.width - 400, Screen.height - 150, 180, 130), "PLACE\nTOWER", customButton)) {;
					repositionTower = false;
					// PAY MONEY$$$
					if (selectedTowerInfo.towerType == TowerController.TowerType.Cannon) cash -= PRICE_CANNON;
					else if (selectedTowerInfo.towerType == TowerController.TowerType.Launcher) cash -= PRICE_ROCKET;
					else if (selectedTowerInfo.towerType == TowerController.TowerType.Radiator) cash -= PRICE_RADIATOR;
					DeselectAll();
				}
				if (GUI.Button (new Rect (Screen.width - 200, Screen.height - 150, 180, 130), "CANCEL", customButton)) {;
					repositionTower = false;
					Destroy(selectedObject);
					DeselectAll();
				}
			}
			// else, show buttons for moving or selling selected tower
			else {
				if (!behindTurret) {
					if (GUI.Button (new Rect (Screen.width - 400, Screen.height - 150, 180, 130), "MOVE\nSELECTED\nTOWER", customButton)) {;
						repositionTower = true;
					}
					if (GUI.Button (new Rect (Screen.width - 200, Screen.height - 150, 180, 130), "SELL\nSELECTED\nTOWER", customButton)) {;
						Destroy(selectedObject);
						// add $$$$$$$$
					}
				}
			}
		}

	}
}
