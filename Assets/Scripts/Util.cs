using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : ScriptableObject
{
   	public static T ScanFor<T>(Vector3 mousePosition, Camera raycastCamera = null) where T : MonoBehaviour
	{		
		if (raycastCamera == null) {
			raycastCamera = Camera.main;
		}
		
		RaycastHit hit;
		var ray = raycastCamera.ScreenPointToRay(mousePosition);
		if (!Physics.Raycast(ray, out hit, 2000)) {
			return null;
		}

		var clickedObject = hit.collider.gameObject;
		return clickedObject.GetComponent<T>();
	}
}