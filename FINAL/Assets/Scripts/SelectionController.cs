using UnityEngine;
using System.Collections;

public class SelectionController : MonoBehaviour {
	
	public Camera camera;
	private string colliderTag;
	private bool selection;
	private RaycastHit lastHit;
	private GameObject selectedObject;
	private GameObject selector;
	
	// Use this for initialization
	void Start () {
		colliderTag = "no collisions yet";
		selection = false;
		selectedObject = null;
		selector = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
		
		//if (Input.touchCount > 0) {
		RaycastHit hit;
		Ray castRay;
		if (Application.platform == RuntimePlatform.Android)
			castRay = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0));
		else
			castRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

		if (Physics.Raycast(castRay, out hit)) {
			selection = true;
			colliderTag = hit.collider.tag;
			if (selectedObject != null) {
				if (hit.collider.gameObject.Equals(selectedObject)) {
					selectedObject = null;
					selection = false;
					hit.collider.gameObject.renderer.material.SetColor("_Color", Color.white);
				}
				else {
					selectedObject.renderer.material.SetColor("_Color", Color.white);
				}
			}
			
			selectedObject = hit.collider.gameObject;
			selectedObject.renderer.material.SetColor("_Color", Color.green);
		}
		else {
			if (selectedObject != null) {
				selectedObject.renderer.material.SetColor("_Color", Color.white);
			}
			selectedObject = null;
			selection = false;
		}
		//}
		
	}
	
	public Vector3 GetSelectorPosition() {
		return selector.transform.position;
	}
	
	public GameObject GetSelectedObject() {
		return selectedObject;
	}
}
