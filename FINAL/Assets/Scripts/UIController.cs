using UnityEngine;
using System.Collections;


// TODO: FIRING during play mode
public class UIController : MonoBehaviour {

	// selection indicator prefabs
	public GameObject selectionCollider, selectionIndicator;

	// tower prefabs
	public GameObject cannonPrefab, rocketLauncherPrefab;

	// minimap camera
	public Camera minimapCamera;

	private GameObject newTower; // temporary for creating new towers
	private SpawnController spawner; // spawn controler
	private bool selection; // true if there is a current selection
	private GameObject selectedObject; // the currently selected object
	private string colliderTag; // tag of collider for raycasting
	private GameObject vCollider, vIndicator;
	private bool repositionTower;
	private bool playMode;

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
				if (Application.platform == RuntimePlatform.Android) {
					castRay = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
				}
				else {
					castRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
				}
				RaycastHit hit;
				// cast a ray through the touch point and see if it hits a tower
				if (Physics.Raycast(castRay, out hit)) {
					selection = true;
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
		float diameter = ((SphereCollider) o.collider).radius * 6;
		vCollider.transform.localScale = new Vector3(diameter, diameter, diameter);
		vCollider.transform.parent = o.transform;
		// small white sphere around tower
		vIndicator = (GameObject) Instantiate(selectionIndicator, o.transform.position, Quaternion.identity);
		diameter = o.transform.localScale.y * 4;
		vIndicator.transform.localScale = new Vector3(diameter, diameter, diameter);
		vIndicator.transform.parent = o.transform;
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
			}
		}
	}

	void OnGUI() {

		// display aiming reticle
		GUIStyle textBox = new GUIStyle(GUI.skin.box);
		textBox.fontSize = 30;
		textBox.fontStyle = FontStyle.Bold;
		GUI.Box (new Rect (Screen.width / 2 - 20, Screen.height / 2 - 20, 40, 40), "+", textBox);

		// display lives and cash
		GUI.Box (new Rect (20, 20, 250, 80), "LIVES: 150\nCASH: $800", textBox);

		// display buttons
		GUIStyle customButton = new GUIStyle(GUI.skin.button);
		customButton.fontSize = 24;
		customButton.fontStyle = FontStyle.Bold;
		// if we are in play phase...
		if (playMode) {
			// firing during play mode
			if (GUI.Button(new Rect (Screen.width - 220, Screen.height / 2 - 100, 200, 200), "FIRE!!", customButton)) {
				// firing goes here!!!!!
			}
			// if all enemies are destroyed and the spawner has deactivated, play phase is over
			if (spawner.active == false && GameObject.FindGameObjectWithTag("Enemy") == null) {
				playMode = false;
			}
		}
		// if we are in build phase
		else {
			// if nothing is selected, show buttons for building towers
			if (selectedObject == null) {
				if (GUI.Button (new Rect (Screen.width - 440, Screen.height - 150, 200, 130), "BUILD\nCANNON", customButton)) {
					CreateTowerAtCamera(cannonPrefab);
					repositionTower = true;
				}
				if (GUI.Button (new Rect (Screen.width - 220, Screen.height - 150, 200, 130), "BUILD\nROCKET\nLAUNCHER", customButton)) {
					CreateTowerAtCamera(rocketLauncherPrefab);
					repositionTower = true;
				}
			}
			// otherwise...
			else {
				// if we are moving a tower, show button for placing tower
				if (repositionTower) {
					if (GUI.Button (new Rect (Screen.width - 220, Screen.height - 150, 200, 130), "PLACE\nTOWER", customButton)) {;
						repositionTower = false;
						DeselectAll();
					}
				}
				// else, show buttons for moving or selling selected tower
				else {
					if (GUI.Button (new Rect (Screen.width - 440, Screen.height - 150, 200, 130), "MOVE\nSELECTED\nTOWER", customButton)) {;
						repositionTower = true;
					}
					if (GUI.Button (new Rect (Screen.width - 220, Screen.height - 150, 200, 130), "SELL\nSELECTED\nTOWER", customButton)) {;
						Destroy(selectedObject);
						// add $$$$$$$$
					}
				}
			}
			// if start next wave is tapped, activate spawner and play mode
			if (GUI.Button (new Rect (Screen.width - 220, 20, 200, 130), "START\nNEXT WAVE", customButton)) {
				spawner.active = true;
				playMode = true;
			}
		}

	}
}
