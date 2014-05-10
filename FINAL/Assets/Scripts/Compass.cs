using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour {

	Camera cam; // Camera to use
	GameObject target; // Target to point at
	Vector3 targetPosition; // Target position on screen
	Vector3 screenMiddle; //Middle of the screen

	// Use this for initialization
	void Start () {
		cam = GameObject.Find("MiniMapCamera").camera;
		target = GameObject.Find("ARCamera");
		//Get the targets position on screen into a Vector3
		targetPosition = cam.WorldToScreenPoint(target.transform.position);
		//Get the middle of the screen into a Vector3
		screenMiddle = new Vector3 (Screen.width/2, Screen.height/2, 0);
	}
	
	// Update is called once per frame
	void Update () {

		//Get the targets position on screen into a Vector3
		targetPosition = cam.WorldToScreenPoint(target.transform.position);
		//Compute the angle from screenMiddle to targetPos
		float tarAngle = (Mathf.Atan2(targetPosition.x - screenMiddle.x, Screen.height - targetPosition.y - screenMiddle.y) * Mathf.Rad2Deg) + 90;
		if (tarAngle < 0) {
			tarAngle += 360;	
		}

		//Calculate the angle from the camera to the target  
		Vector3 targetDirection = target.transform.position - cam.transform.position;
		Vector3 forward = cam.transform.forward;
		float angle = Vector3.Angle (targetDirection, forward);

		//If the angle exceeds 90deg inverse the rotation to point correctly
		if (angle < 90) {
			transform.localRotation = Quaternion.Euler(-tarAngle, 90, 270);
		} 
		else {
			transform.localRotation = Quaternion.Euler(tarAngle, 270, 90);	
		}
	}
}
