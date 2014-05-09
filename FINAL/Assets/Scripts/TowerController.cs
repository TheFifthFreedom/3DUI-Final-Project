﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerController : MonoBehaviour {

	public enum TowerType {Cannon, Launcher, Radiator};

	public GameObject projectileObject;
	public GameObject muzzleEffect;
	public float reloadTime;
	public float radius;
	public float damage;
	public Transform targetObject;
	public Transform[] muzzlePositions;
	public Transform turretHead;
	public TowerType towerType;
	
	private float nextFireTime;

	private LinkedList<GameObject> firingQueue;
	
	// Use this for initialization
	void Start () {
		firingQueue = new LinkedList<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (firingQueue.Count > 0) {
			while (firingQueue.First.Value == null) {
				firingQueue.RemoveFirst();
				if (firingQueue.Count <= 0) {
					return;
				}
			}
			turretHead.transform.LookAt(firingQueue.First.Value.transform);
			if (towerType != TowerType.Launcher) {
				turretHead.transform.eulerAngles = new Vector3(0, turretHead.transform.eulerAngles.y, 0);
			}
			if (Time.time >= nextFireTime) {
				FireProjectile();
				nextFireTime = Time.time + reloadTime;
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Enemy") {
			firingQueue.AddLast(other.gameObject);
		}
	}
	
	void OnTriggerStay(Collider other) {
		
	}
	
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Enemy") {
			firingQueue.Remove(other.gameObject);
		}
	}
	
	void FireProjectile() {
		audio.Play();
		nextFireTime = Time.time + reloadTime;

		if (towerType == TowerType.Launcher) {
			int m = Random.Range(0, muzzlePositions.Length);
			GameObject projectile = (GameObject) Instantiate(projectileObject, muzzlePositions[m].position, muzzlePositions[m].rotation);
			projectile.GetComponent<ProjectileController>().target = firingQueue.First.Value.transform;
			projectile.transform.parent = this.transform;
		}
		if (towerType == TowerType.Radiator) {
			foreach (Transform mPos in muzzlePositions) {
				GameObject projectile = (GameObject) Instantiate(projectileObject, mPos.position, mPos.rotation);
				projectile.transform.parent = this.transform;
				projectile.GetComponent<ProjectileController>().target = firingQueue.First.Value.transform;
			}
		}
		if (towerType == TowerType.Cannon) {
			foreach (Transform mPos in muzzlePositions) {
				GameObject projectile = (GameObject) Instantiate(projectileObject, mPos.position, mPos.rotation);
				projectile.transform.parent = this.transform;
				projectile.GetComponent<ProjectileController>().target = firingQueue.First.Value.transform;
				Instantiate(muzzleEffect, mPos.position, mPos.rotation);
			}
		}
	}
	
}
