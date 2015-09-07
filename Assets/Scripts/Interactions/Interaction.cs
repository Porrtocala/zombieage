using UnityEngine;
using System.Collections;

public abstract class Interaction {

	public abstract bool CheckInteraction (ISelectable[] selectedObjects, IInteractable obj);
	public abstract void ExecuteInteraction (ISelectable[] selectedObjects, IInteractable obj);
}
