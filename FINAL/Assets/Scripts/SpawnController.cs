using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {

	public bool isActive;
	public float spawnRate;
	public int enemiesPerRound;
	public GameObject enemyObject;
	public GameObject spawner;
	public int enemyHealthAtSpawn;

	private float spawnTimer;
	private int enemiesSpawned;

	public GameObject path;

	// Use this for initialization
	void Start () {
		spawnTimer = 0.0f;
		enemiesSpawned = 0;
		enemyHealthAtSpawn = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive) {
			spawnTimer += Time.deltaTime;
			if (spawnTimer >= spawnRate) {
				GameObject en = (GameObject) Instantiate(enemyObject, spawner.transform.position, spawner.transform.rotation);
				en.GetComponent<EnemyConroller>().maxHealth = enemyHealthAtSpawn;
				en.GetComponent<EnemyConroller>().cashOnKill = enemyHealthAtSpawn;
				spawnTimer = 0.0f;
				enemiesSpawned++;
			}
			if (enemiesSpawned >= enemiesPerRound) {
				isActive = false;
				enemiesSpawned = 0;
			}
		}

		if (!isActive && Input.GetKeyDown (KeyCode.Space)) {
			
			Debug.Log ("next wave");
			isActive = true;
		}
	}
}
