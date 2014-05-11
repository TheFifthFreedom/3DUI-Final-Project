using UnityEngine;
using System.Collections;

public class Compass2 : MonoBehaviour {

	Camera cam;
	GameObject target;

	// Use this for initialization
	void Start () {
		cam = GameObject.Find("MiniMapCamera").camera;
		target = GameObject.Find ("Main Camera");
		transform.renderer.material.color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
	
		renderer.enabled = false;
		
		Vector3 v3Pos = cam.WorldToViewportPoint(target.transform.position);
		
		if (v3Pos.z < cam.nearClipPlane)
			return;  // Object is behind the camera
		
		if (v3Pos.x >= 0.0f && v3Pos.x <= 1.0f && v3Pos.y >= 0.0f && v3Pos.y <= 1.0f)
			return; // Object center is visible
		
		renderer.enabled = true;
		v3Pos.x -= 0.5f;  // Translate to use center of viewport
		v3Pos.y -= 0.5f; 
		v3Pos.z = 0;      // I think I can do this rather than do a 
		//   a full projection onto the plane
		
		float fAngle = Mathf.Atan2 (v3Pos.x, v3Pos.y);
		transform.localEulerAngles = new Vector3(0.0f, -fAngle * Mathf.Rad2Deg, 0.0f);
		
		v3Pos.x = 0.5f * Mathf.Sin (fAngle) + 0.5f;  // Place on ellipse touching 
		v3Pos.y = 0.5f * Mathf.Cos (fAngle) + 0.5f;  //   side of viewport
		v3Pos.z = cam.nearClipPlane + 100f;  // Looking from neg to pos Z;
		transform.position = cam.ViewportToWorldPoint(v3Pos);

	}
}
