using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float marginPercentage;
	public float xSpeed;
	public float ySpeed;

	public Transform cameraLimit1;
	public Transform cameraLimit2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		int cameraHeight = Camera.main.pixelHeight;
		int cameraWidth = Camera.main.pixelWidth;

		float xPercentage = (Input.mousePosition.x / cameraWidth) * 100;
		float yPercentage = (Input.mousePosition.y / cameraHeight) * 100;


		Vector3 cameraVelocity = new Vector3 (0, 0, 0);
		if (xPercentage < marginPercentage) {
			float speedFactor = 1 - Mathf.Max(0, xPercentage / marginPercentage);
			cameraVelocity -= new Vector3 (xSpeed * speedFactor * Time.deltaTime, 0, 0);
		} else if (xPercentage > 95) {
			float speedFactor = Mathf.Min(1, (xPercentage - 100 + marginPercentage) / marginPercentage);
			cameraVelocity += new Vector3 (xSpeed * speedFactor * Time.deltaTime, 0, 0);
		}

		if (yPercentage < 5) {
			float speedFactor = 1 - Mathf.Max(0, yPercentage / marginPercentage);
			cameraVelocity -= new Vector3 (0, ySpeed * speedFactor * Time.deltaTime, 0);
		} else if (yPercentage > 95) {
			float speedFactor = Mathf.Min(1, (yPercentage - 100 + marginPercentage) / marginPercentage);
			cameraVelocity += new Vector3 (0, ySpeed * speedFactor * Time.deltaTime, 0);
		}

		float minX = cameraLimit1.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect;
		float minY = cameraLimit1.transform.position.y + Camera.main.orthographicSize;

		float maxX = cameraLimit2.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect;
		float maxY = cameraLimit2.transform.position.y - Camera.main.orthographicSize * Camera.main.aspect;

		Vector3 newPosition = gameObject.transform.position + cameraVelocity;
		newPosition.x = Mathf.Clamp (newPosition.x, minX, maxX);
		newPosition.y = Mathf.Clamp (newPosition.y, minY, maxY);

		gameObject.transform.position = newPosition;
	}
}
