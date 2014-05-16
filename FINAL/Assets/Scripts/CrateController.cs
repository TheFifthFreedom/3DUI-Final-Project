using UnityEngine;
using System.Collections;

public class CrateController : MonoBehaviour {

	public enum CrateFunction {Money, Damage};

	public CrateFunction crateFunction;
	public float lifeSpan;

	//private UIController ui;
	private float currentTime;

	public int cashAmount, damageAmount;

	// Use this for initialization
	void Start () {
		//ui = Camera.main.GetComponent<UIController>();
		currentTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		if (currentTime >= lifeSpan) {
			Destroy(gameObject);
		}

		gameObject.transform.Rotate (new Vector3 (0, 50 * Time.deltaTime, 0));
	}

}
