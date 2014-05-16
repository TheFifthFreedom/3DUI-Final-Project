using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {

	public bool isActive;
	public float spawnRate, crateRate;
	public int enemiesPerRound;
	public GameObject enemyObject, crateObject;
	public GameObject spawner;
	public int enemyHealthAtSpawn;
	public Transform[] crateSpawnLocations;
	public int waveNumber;

	private float spawnTimer, crateTimer;
	private int enemiesSpawned;

	public GameObject path;

	// Use this for initialization
	void Start () {
		spawnTimer = crateTimer = 0.0f;
		enemiesSpawned = 0;
		enemyHealthAtSpawn = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive) {
			spawnTimer += Time.deltaTime;
			crateTimer += Time.deltaTime;
			if (spawnTimer >= spawnRate) {
				GameObject en = (GameObject) Instantiate(enemyObject, spawner.transform.position, spawner.transform.rotation);
				en.GetComponent<EnemyConroller>().maxHealth = enemyHealthAtSpawn;
				en.GetComponent<EnemyConroller>().cashOnKill = waveNumber;//enemyHealthAtSpawn; // make this waveNumber?
				spawnTimer = 0.0f;
				enemiesSpawned++;
			}

			if (enemiesSpawned >= enemiesPerRound) {
				isActive = false;
				enemiesSpawned = 0;
			}


		}

		// ITEM CRATES!!!! runs while spawner active or enemies still alive
		if (isActive || GameObject.FindGameObjectWithTag ("Enemy") != null) {
			if (crateTimer >= crateRate) {
				// 5% chance each second of a crate spawning
				if (Random.Range(0.0f, 1.0f) < .05){
					//Debug.Log("spawning crate!");
					Vector3 spawnLocation = crateSpawnLocations[Random.Range(0, crateSpawnLocations.Length)].position;
					GameObject newCrate = (GameObject) Instantiate(crateObject, spawnLocation, Quaternion.identity);
					// 80% chance money, 20% chance damage
					newCrate.GetComponent<CrateController>().crateFunction =
						Random.Range(0.0f, 1.0f) < .8 ? CrateController.CrateFunction.Money : CrateController.CrateFunction.Damage;
				}
				crateTimer = 0.0f;
			}
		}

		if (!isActive && Input.GetKeyDown (KeyCode.Space)) {
			
			//Debug.Log ("next wave");
			//isActive = true;
		}
	}
}
