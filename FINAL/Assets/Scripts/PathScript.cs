using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathScript : MonoBehaviour {

	public GameObject[] waypoints;
	private Vector3[] points;
	private LinkedList<Vector3> pathPointsList;

	// Use this for initialization
	void Start () {
		points = new Vector3[waypoints.Length];
		pathPointsList = new LinkedList<Vector3>();
		for (int i = 0; i < waypoints.Length; i++) {
			points[i] = waypoints[i].transform.position;
			pathPointsList.AddLast(points[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3[] GetPoints() {
		return points;
	}

	public LinkedList<Vector3> GetPathPoints() {
		return new LinkedList<Vector3>(pathPointsList);
	}
}
