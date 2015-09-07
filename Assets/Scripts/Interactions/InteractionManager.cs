using UnityEngine;
using System.Collections;

public class InteractionManager {

	public static Interaction[] GetInteractions() {
		return new Interaction[] {new MovementInteraction ()};
	}
}
