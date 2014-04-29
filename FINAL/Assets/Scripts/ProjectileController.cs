using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public float speed;
	public float range;

	private float dist;

	// Use this for initialization
	void Start () {
		dist = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;
		transform.Translate(Vector3.forward * dt * speed);
		dist += dt * speed;
		if (dist >= range) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Enemy") {
			Destroy (gameObject);
		}
	}

	public void SetRange(float r) {
		range = r;
	}
}
