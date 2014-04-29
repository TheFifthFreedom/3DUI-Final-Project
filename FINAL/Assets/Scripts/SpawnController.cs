using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {

	public bool active;
	public float spawnRate;
	public int enemiesPerRound;
	public GameObject enemyObject;
	public GameObject spawner;

	private float spawnTimer;
	private int enemiesSpawned;

	public GameObject path;

	// Use this for initialization
	void Start () {
		spawnTimer = 0.0f;
		enemiesSpawned = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
			spawnTimer += Time.deltaTime;
			if (spawnTimer >= spawnRate) {
				Instantiate(enemyObject, spawner.transform.position, spawner.transform.rotation);
				spawnTimer = 0.0f;
				enemiesSpawned++;
			}
			if (enemiesSpawned >= enemiesPerRound) {
				active = false;
				enemiesSpawned = 0;
			}
		}

		if (!active && Input.GetKeyDown (KeyCode.Space)) {
			
			Debug.Log ("next wave");
			active = true;
		}
	}
}
