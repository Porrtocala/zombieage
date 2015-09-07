using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {

	// Singleton
	static SelectionManager _instance;
	public static SelectionManager Instance {
		get {
			if (_instance == null) {
				_instance = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
			}
			return _instance;
		}
	}

	Vector3 startDragPosition;
	bool dragging;
	bool multipleSelection;
	List<GameObject> _multipleSelectableObjects;
	List<ISelectable> selectedObjs;

	public void RegisterMultipleSelectable(GameObject obj) {
		_multipleSelectableObjects.Add(obj);
	}

	void Awake() {
		_multipleSelectableObjects = new List<GameObject> ();
		selectedObjs = new List<ISelectable> ();
		dragging = false;
		multipleSelection = false;
	}

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			startDragPosition = Input.mousePosition;
			dragging = true;
		}

		if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && !multipleSelection) {
			dragging = false;
	
			// Selecting object
			GameObject obj;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 2000)) {
				obj = hit.collider.gameObject;
				Debug.Log("Hit " + obj.name);

				if (Input.GetMouseButtonUp(0)) {
					SelectObject(obj);
				} else {
					InteractObject(obj);
				}
			} else {
				Debug.Log("Hit Nothing");
			}
		}

		if (Input.GetMouseButtonUp (0) && multipleSelection) {
			dragging = false;
			multipleSelection = false;
			SelectRectangle();
		}

		if (Input.GetKeyUp (KeyCode.Escape)) {
			ClearSelection();
		}
	}
	
	void OnGUI() {
		if (dragging && (Input.mousePosition - startDragPosition).magnitude > 2) {
			multipleSelection = true;
		}

		if (multipleSelection) {
			Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			texture.SetPixel(0, 0, new Color(0.1f, 0.8f, 0.1f, 0.6f));
			texture.Apply();

			Vector3 p1 = startDragPosition;
			Vector3 p2 = Input.mousePosition;
			p1.y = Camera.main.pixelHeight - p1.y;
			p2.y = Camera.main.pixelHeight - p2.y;

			Vector3 leftCorner = new Vector3(Mathf.Min(p1.x, p2.x),
			                                 Mathf.Min(p1.y, p2.y),
			                                 0);
			Vector3 size = new Vector3(Mathf.Abs(p1.x - p2.x),
			                           Mathf.Abs(p1.y - p2.y),
			                           0);

			// Debug.Log("Drawing Rect: " + leftCorner.ToString() + " " + size.ToString());

			GUI.DrawTexture(new Rect(leftCorner.x, leftCorner.y, size.x, size.y), texture, ScaleMode.StretchToFill, true);
		}
	}

	public void SelectObject(GameObject obj) {
		if (obj.GetComponent<ISelectable>() == null) {
			return;
		}

		ClearSelection ();
		selectedObjs.Add(obj.GetComponent<ISelectable>());
		obj.GetComponent<ISelectable>().Select();
	}

	public void SelectRectangle() {
		bool selectionCleared = false;
		Vector3 p1 = new Vector3 (Mathf.Min(startDragPosition.x, Input.mousePosition.x),
		                          Mathf.Min(startDragPosition.y, Input.mousePosition.y));
		Vector3 p2 = new Vector3 (Mathf.Max(startDragPosition.x, Input.mousePosition.x),
		                          Mathf.Max(startDragPosition.y, Input.mousePosition.y));

		foreach (GameObject obj in _multipleSelectableObjects) {
			Vector3 screenPoint = Camera.main.WorldToScreenPoint(obj.transform.position);
			if (p1.x < screenPoint.x && screenPoint.x < p2.x &&
			    p1.y < screenPoint.y && screenPoint.y < p2.y) {
				if (!selectionCleared) {
					ClearSelection();
					selectionCleared = true;
				}

				selectedObjs.Add(obj.GetComponent<IMultipleSelectable>());
				obj.GetComponent<IMultipleSelectable>().Select();
			}
		}
	}

	public void ClearSelection() {
		foreach (ISelectable selectedObj in selectedObjs) {
			selectedObj.Deselect();
		}
		selectedObjs.Clear();
	}

	public void InteractObject(GameObject obj) {
		if (obj.GetComponent<IInteractable> () == null) {
			ClearSelection();
			return;
		}

		IInteractable interactObj = obj.GetComponent<IInteractable>();

		foreach (Interaction interaction in InteractionManager.GetInteractions()) {
			if (interaction.CheckInteraction(selectedObjs.ToArray(), interactObj)) {
				interaction.ExecuteInteraction(selectedObjs.ToArray(), interactObj);
				break;
			}
		}
	}
}
