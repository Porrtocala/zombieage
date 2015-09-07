using UnityEngine;
using System.Collections;

public class MovementInteraction : Interaction {

	public override bool CheckInteraction (ISelectable[] selectedObjects, IInteractable obj)
	{
		if (selectedObjects[0].GetType() == typeof(Unit) && obj.GetType() == typeof(Map)) {
			return true;
		}

		return false;
	}

	public override void ExecuteInteraction (ISelectable[] selectedObjects, IInteractable obj)
	{
		int cntPoints = selectedObjects.Length;
		Vector3[] points = new Vector3[cntPoints];

		int ring = 0;
		int ringCnt = 0;
		float ringAngle = 0;

		Vector3 targetPos;
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 2000)) {
			targetPos = hit.point;
		} else {
			return;
		}

		float unitSize = 0;
		for (int i = 0; i < selectedObjects.Length; i++) {
			unitSize = Mathf.Max(unitSize, (selectedObjects[i] as Unit).sizeRadius);
		}

		// TODO: We should use map tiles for this
		for (int i = 0; i < cntPoints; i++) {
			if (ringCnt == ring * 2 + 1) {
				ring++;
				ringCnt = 0;
				ringAngle = 0;
			}

			points[i] = targetPos + new Vector3(unitSize * ring * Mathf.Cos(ringAngle),
			                        			unitSize * ring * Mathf.Sin(ringAngle),
			                        			0);

			ringCnt = ringCnt + 1;
			ringAngle = ringAngle + 2 * Mathf.PI / (ring * 2 + 1);
		}

		for (int i = 0; i < cntPoints; i++) {
			(selectedObjects[i] as Unit).MoveTo(points[i]);
		}
	}
}
