using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathScript : MonoBehaviour {

	public GameObject[] paths;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public LinkedList<Vector3> GetRandomPath() {
		GameObject choosePath = paths[Random.Range(0, paths.Length)];
		LinkedList<Vector3> pathPointsList = new LinkedList<Vector3>();
		for (int i = 0; i < choosePath.transform.childCount; i++) {
			pathPointsList.AddLast(choosePath.transform.GetChild(i).transform.position);
		}
		return new LinkedList<Vector3>(pathPointsList);
	}
}
