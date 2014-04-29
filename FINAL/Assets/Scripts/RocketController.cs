using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour {

	public GameObject explosion;
	public Transform target;
	public float speed;
	public float range;
	
	private float dist;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;
		transform.Translate(Vector3.forward * dt * speed);
		dist += dt * speed;
		if (dist >= range) {
			Destroy(gameObject);
		}
		if (target) {
			transform.LookAt(target);
		}
		else {
			Explode();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Enemy") {
			Explode();
		}
	}

	void Explode() {
		Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
