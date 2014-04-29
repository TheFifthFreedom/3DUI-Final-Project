using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationX = 0F;
	float rotationY = 0F;
	
	Quaternion originalRotation;

	public float movementSpeed;
	
	void Update ()
	{
		// WASD to move
		float dt = Time.deltaTime;
		float dx = 0.0f;
		float dy = 0.0f;
		float dz = 0.0f;
		if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) dz = movementSpeed * dt;
		if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) dz = -movementSpeed * dt;
		if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) dx = -movementSpeed * dt;
		if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) dx = movementSpeed * dt;
		transform.Translate(dx, dy, dz, Space.Self);

		//Debug.Log (Input.GetKey (KeyCode.Space));

		if (Input.GetMouseButton(1)) {
			if (axes == RotationAxes.MouseXAndY)
			{
				// Read the mouse input axis
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				
				rotationX = ClampAngle (rotationX, minimumX, maximumX);
				rotationY = ClampAngle (rotationY, minimumY, maximumY);
				
				Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
				
				transform.localRotation = originalRotation * xQuaternion * yQuaternion;
			}
			else if (axes == RotationAxes.MouseX)
			{
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
				rotationX = ClampAngle (rotationX, minimumX, maximumX);
				
				Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				transform.localRotation = originalRotation * xQuaternion;
			}
			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = ClampAngle (rotationY, minimumY, maximumY);
				
				Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
				transform.localRotation = originalRotation * yQuaternion;
			}
		}

	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
		originalRotation = transform.localRotation;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}

}
