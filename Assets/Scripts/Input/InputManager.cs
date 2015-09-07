using UnityEngine;
using System.Collections;

public enum ObjectState {
	Hovering = 0, Pressing = 1
}

public class InputManager : MonoBehaviour {

	public Camera[] cameraOrder;

	public EventTrigger currentObject;
	public ObjectState currentObjectState;
	

	public void Start() {
		currentObject = null;
		currentObjectState = ObjectState.Hovering;
	}

	public void Update() {
		bool hit = false;
		foreach (Camera raycastCamera in cameraOrder) {
			EventTrigger objectHit = Util.ScanFor<EventTrigger>(Input.mousePosition, raycastCamera);
			if (objectHit == null) {
				continue;
			}

			UpdateObjectHit(objectHit);
			hit = true;
			break;
		}

		if (!hit) {
			UpdateObjectHit(null);
		}
	}

	public void UpdateObjectHit(EventTrigger objectHit) {
		// current object is changed only if the state is hovering
		// Previous object is no longer hit
		if (currentObject != null &&
			objectHit != currentObject &&
			currentObjectState == ObjectState.Hovering) {
			currentObject.OnHoverOut();
		}

		if (objectHit != null &&
			objectHit != currentObject &&
			currentObjectState == ObjectState.Hovering) {
			currentObjectState = ObjectState.Hovering;
			objectHit.OnHoverIn();
		}

		if (currentObjectState == ObjectState.Hovering) {
			currentObject = objectHit;
		}

		if (currentObject == null) {
			return;
		}

		// We are calling the press only if the current object is being hovered
		if (Input.GetMouseButtonDown(0) &&
			currentObjectState == ObjectState.Hovering) {
			currentObjectState = ObjectState.Pressing;
			currentObject.OnPress();
		}

		// Only if we are currently pressing
		if (Input.GetMouseButtonUp(0) &&
			currentObjectState == ObjectState.Pressing) {
			currentObject.OnRelease();
			currentObjectState = ObjectState.Hovering;

			// Only if we are currently pressing and the object hit is the same we call the on click
			if (objectHit == currentObject) {
				currentObject.OnClick();
			}
		}		
	}

    void OnGUI()
    {

    }
}
