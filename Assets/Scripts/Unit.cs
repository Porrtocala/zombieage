using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour, IMultipleSelectable {

	Vector3 targetPosition;
	public float speed;
	public float sizeRadius;

	#region IMultipleSelectable
	public void Register() {
		SelectionManager.Instance.RegisterMultipleSelectable(gameObject);
	}

	public void Select() {
		transform.FindChild ("Selected").gameObject.SetActive(true);
	}

	public void Deselect() {
		transform.FindChild ("Selected").gameObject.SetActive(false);
	}
	#endregion

	public void Interact(GameObject obj) {
		if (obj.name == "Map") {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 2000)) {
				Vector3 position = hit.point;
				MoveTo(position);
			}
		}
	}

	public void MoveTo(Vector3 position) {
		targetPosition = position;
		targetPosition.z = transform.position.z;
	}

	void Awake() {
		targetPosition = transform.position;
	}

	void Start() {
		this.Register();
	}

	void Update() {
		if ((transform.position - targetPosition).magnitude > 0.1) {
			Vector3 targetVector = targetPosition - transform.position;
			Vector3 velocity = targetVector.normalized * speed * Time.deltaTime;
			if (velocity.magnitude > targetVector.magnitude) {
				velocity = targetVector;
			}

			transform.position = transform.position + velocity;
			transform.rotation = Quaternion.Euler(new Vector3 (0, 0, Mathf.Rad2Deg * Mathf.Atan2(velocity.y, velocity.x)));

			Debug.DrawLine(transform.position, targetPosition, Color.green);
		}
	}
}
