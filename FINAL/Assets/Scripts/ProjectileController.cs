using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public enum ProjectileType {CannonBall, HomingRocket, Radiation, TurretBullet};
	public float speed;
	public float range;
	public ProjectileType projectileType;
	public GameObject explosionPrefab;
	public Transform target;

	private float dist;

	// Use this for initialization
	void Start () {
		dist = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;
		dist += dt * speed;
		if (dist >= range) {
			Explode();
		}

		if (projectileType == ProjectileType.Radiation) {
			transform.localScale = transform.localScale + new Vector3(dt*speed, dt*speed, dt*speed);
		}
		else if (projectileType == ProjectileType.TurretBullet) {
			transform.Translate(Vector3.forward * dt * speed);
		}
		else {
			transform.Translate(Vector3.forward * dt * speed);
			if (target) {
				transform.LookAt(target);
				if (projectileType == ProjectileType.CannonBall) {
					transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
				}
			}
			else {
				if (projectileType == ProjectileType.HomingRocket) {
					GameObject newObj = GameObject.FindGameObjectWithTag("Enemy");
					if (newObj == null) Explode();
					else target = newObj.transform;
				}
				else {
					Explode();
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Enemy") {
			if (projectileType != ProjectileType.Radiation) Explode();
		}
	}

	public void SetRange(float r) {
		range = r;
	}

	void Explode() {
		if (projectileType == ProjectileType.HomingRocket) {
			Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}
}
