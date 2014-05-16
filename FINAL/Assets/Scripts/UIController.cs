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

	private bool moveTowerInitial, repositionTower;
	private bool playMode, behindTurret, rangeMode, lostGame;
	private float rangeModeStartDist, rangeModeStartRadius, rangeModeLastDistance;
	public int waveNumber, lives, cash;
	private Vector3 repositionOriginalPos;

	public const int PRICE_CANNON = 10, PRICE_ROCKET = 20, PRICE_RADIATOR = 35;

	// Use this for initialization
	void Start () {
		moveTowerInitial = false;
		spawner = GameObject.Find("SpawnLocation").GetComponent<SpawnController>();
		playMode = false;
		rangeMode = false;
		lostGame = false;
	}

	// Update is called once per frame
	void Update () {

		// if we're currently in the process of moving a tower
		if (moveTowerInitial || repositionTower) {
			vCollider.transform.parent = selectedObject.transform;
			vIndicator.transform.parent = selectedObject.transform;
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
			if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && GUIUtility.hotControl == 0) {
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
		selectedTowerInfo = o.GetComponent<TowerController>();
		selectedTowerInfo.range = selectedObject.GetComponent<SphereCollider>().radius * selectedObject.transform.localScale.y;
		selectedTowerInfo.startRange = selectedTowerInfo.range;
		// sight radius
		vCollider = (GameObject) Instantiate(selectionCollider, o.transform.position, Quaternion.identity);
		float diameter = selectedTowerInfo.range * 2.0f;
		vCollider.transform.localScale = new Vector3(diameter, diameter, diameter);
		// small white sphere around tower
		vIndicator = (GameObject) Instantiate(selectionIndicator, o.transform.position, Quaternion.identity);
		diameter = o.transform.localScale.y * 3;
		vIndicator.transform.localScale = new Vector3(diameter, diameter, diameter);
		// get info to display
	}

	// deselects the selected object by destroying the indicators and nullifying the selection
	void DeselectAll() {
		vCollider.transform.parent = null;
		vIndicator.transform.parent = null;
		Destroy(vCollider);
		Destroy(vIndicator);
		selectedObject = null;
		rangeMode = false;
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
				moveTowerInitial = true;
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
		textBox.fontSize = 26;
		textBox.fontStyle = FontStyle.Bold;
		textBox.alignment = TextAnchor.MiddleLeft;
		GUI.Box (new Rect (20, 20, 300, 130), "Wave Number: " + spawner.waveNumber + "\nEnemy Health: "
		         + spawner.enemyHealthAtSpawn + "\nLives Left: " + lives + "\nCash: $" + cash, textBox);

		// display buttons
		GUIStyle customButton = new GUIStyle(GUI.skin.button);
		customButton.fontSize = 24;
		customButton.fontStyle = FontStyle.Bold;

		if (lostGame) {
			textBox = new GUIStyle(GUI.skin.box);
			textBox.fontSize = 36;
			textBox.alignment = TextAnchor.MiddleCenter;
			GUI.Box (new Rect (Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 100), "GAME OVER!", textBox);

			if (GUI.Button (new Rect (Screen.width / 2 - 150, Screen.height / 2 + 20, 300, 100), "TRY AGAIN", customButton)) {
				Application.LoadLevel("MainScene");
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 150, Screen.height / 2 + 120, 300, 100), "QUIT GAME", customButton)) {
				Application.Quit();
			}
		}
		else {
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
				playMode = true;
				spawner.waveNumber++;
				spawner.isActive = true;
				spawner.enemyHealthAtSpawn = Mathf.RoundToInt((float) spawner.waveNumber * 1.5f);
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
				info += "RANGE: " + Mathf.RoundToInt(selectedTowerInfo.range);
				GUI.Box (new Rect (20, 170, 250, 190), info, textBox);
				
				// if we are moving a tower, show button for placing tower
				if (moveTowerInitial || repositionTower) {
					if (GUI.Button (new Rect (Screen.width - 400, Screen.height - 150, 180, 130), "PLACE\nTOWER", customButton)) {
						moveTowerInitial = false;
						repositionTower = false;
						// PAY MONEY$$$
						cash -= selectedTowerInfo.towerPrice;
						DeselectAll();
					}
					if (GUI.Button (new Rect (Screen.width - 200, Screen.height - 150, 180, 130), "CANCEL", customButton)) {
						if (moveTowerInitial) {
							moveTowerInitial = false;
							Destroy(selectedObject);
							DeselectAll();
						}
						else if (repositionTower) {
							repositionTower = false;
							selectedObject.transform.position = repositionOriginalPos;
							DeselectAll();
						}
					}
				}
				// if we're in range mode, show buttons for confirming upgrade or cancelling
				else if (rangeMode) {
					float rangeModeCurrentDist = (Camera.main.transform.position - selectedObject.transform.position).magnitude;
					float distRatio = rangeModeCurrentDist / rangeModeLastDistance * 1;
					selectedTowerInfo.range = selectedTowerInfo.range * distRatio;
					vCollider.transform.localScale = new Vector3(selectedTowerInfo.range*2, selectedTowerInfo.range*2, selectedTowerInfo.range*2);
					rangeModeLastDistance = rangeModeCurrentDist;
					int cost = Mathf.RoundToInt(rangeModeCurrentDist / rangeModeStartDist * 125);
					GUI.enabled = cost <= cash && selectedTowerInfo.range >= rangeModeStartRadius;
					if (GUI.Button (new Rect (Screen.width - 400, Screen.height - 150, 180, 130), "CONFIRM\nUPGRADE\n-- $" + cost + " --", customButton)) {
						cash -= cost;
						selectedTowerInfo.range = Mathf.RoundToInt(selectedTowerInfo.range);
						selectedObject.GetComponent<SphereCollider>().radius = selectedTowerInfo.range / selectedObject.transform.localScale.y;
						rangeMode = false;
					}
					GUI.enabled = true;
					if (GUI.Button (new Rect (Screen.width - 200, Screen.height - 150, 180, 130), "CANCEL", customButton)) {
						vCollider.transform.localScale = new Vector3(rangeModeStartRadius*2, rangeModeStartRadius*2, rangeModeStartRadius*2);
						rangeMode = false;
					}
				}
				// else, show buttons for moving or selling selected tower
				else {
					if (!behindTurret) {
						int upgradePrice = selectedTowerInfo.damage * selectedTowerInfo.damageStage * 20;
						GUI.enabled = cash >= upgradePrice;
						if (GUI.Button (new Rect (Screen.width - 800, Screen.height - 150, 180, 130), "DAMAGE\n+60%\n-- $" + upgradePrice + " --", customButton)) {
							cash -= upgradePrice;
							selectedTowerInfo.damageStage++;
							selectedTowerInfo.damage = Mathf.CeilToInt((float) selectedTowerInfo.damage * 1.6f);
						}
						GUI.enabled = true;
						if (GUI.Button (new Rect (Screen.width - 600, Screen.height - 150, 180, 130), "UPGRADE\nTOWER\nRANGE", customButton)) {;
							rangeMode = true;
							rangeModeStartDist = (Camera.main.transform.position - selectedObject.transform.position).magnitude;
							rangeModeLastDistance = rangeModeStartDist;
							rangeModeStartRadius = selectedTowerInfo.range;
						}
						GUI.enabled = cash >= selectedTowerInfo.towerPrice;
						if (GUI.Button (new Rect (Screen.width - 400, Screen.height - 150, 180, 130), "MOVE\nTOWER\n-- $" + selectedTowerInfo.towerPrice + " --", customButton)) {
							repositionTower = true;
							repositionOriginalPos = selectedObject.transform.position;
						}
						GUI.enabled = true;
						int sellPrice = (selectedTowerInfo.towerPrice / 2 + 1) + ((selectedTowerInfo.damage - 1) * 8 + 1)
							+ ((int) (selectedTowerInfo.range - selectedTowerInfo.startRange) / 2 + 1);
						if (GUI.Button (new Rect (Screen.width - 200, Screen.height - 150, 180, 130), "SELL\nTOWER\n-- +$" + sellPrice + " --", customButton)) {
							cash += sellPrice;
							Destroy(selectedObject);
							DeselectAll();
						}
					}
				}
			}
		}



	}
}
