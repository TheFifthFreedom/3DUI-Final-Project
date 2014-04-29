using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyConroller : MonoBehaviour {

	public float speed;
	public float range;
	public float maxHealth;

	private float health;

	private Vector3[] path;
	private LinkedList<Vector3> pathPoints;
	private Vector3 currentPoint;

	private const float ERROR = 0.5f;

	public static int id = 0;

	//private Transform[] path;

	private float dist;
	
	// Use this for initialization
	void Start () {
		id++;
		dist = 0.0f;
		health = maxHealth;
		pathPoints = GameObject.Find("Path").GetComponent<PathScript>().GetPathPoints();
	}
	
	// Update is called once per frame
	void Update () {
		if (pathPoints.Count > 0) {
			transform.LookAt(pathPoints.First.Value);
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
			float dist = Vector3.Distance(transform.position, pathPoints.First.Value);
			if (dist <= ERROR) {
				pathPoints.RemoveFirst();
			}
		}
		else {
			Destroy(gameObject);
		}

		if (health <= 0) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Projectile") {
			health -= 1; // replace with projectile's damage value
		}
	}

	private Vector3 CopyVector(Vector3 v) {
		return new Vector3(v.x, v.y, v.z);
	}

}
