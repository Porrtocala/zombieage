using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class EventTrigger : MonoBehaviour {
	[SerializeField]
	public EventDelegateList onClick;
	[SerializeField]
	public EventDelegateList onHoverIn;
	[SerializeField]
	public EventDelegateList onHoverOut;
	[SerializeField]
	public EventDelegateList onPress;
	[SerializeField]
	public EventDelegateList onRelease;	

	public EventDelegateList OnClickEvents {
		get { 
			if (onClick == null) {
				onClick = ScriptableObject.CreateInstance<EventDelegateList>();
			}

			return onClick;
		}
	}

	public EventDelegateList OnHoverInEvents {
		get { 
			if (onHoverIn == null) {
				onHoverIn = ScriptableObject.CreateInstance<EventDelegateList>();
			}
			
			return onHoverIn;
		}
	}

	public EventDelegateList OnHoverOutEvents {
		get { 
			if (onHoverOut == null) {
				onHoverOut = ScriptableObject.CreateInstance<EventDelegateList>();
			}
			
			return onHoverOut;
		}
	}

	public EventDelegateList OnPressEvents {
		get { 
			if (onPress == null) {
				onPress = ScriptableObject.CreateInstance<EventDelegateList>();
			}
			
			return onPress;
		}
	}
	
	public EventDelegateList OnReleaseEvents {
		get { 
			if (onRelease == null) {
				onRelease = ScriptableObject.CreateInstance<EventDelegateList>();
			}
			
			return onRelease;
		}
	}
	

	public static MethodInfo[] GetObjectMethods(System.Type objType) {
		System.Type t = objType;
		List<System.Type> validTypes = new List<System.Type>();
		while (t != null) {
			if (t.Namespace == "UnityEngine" || t.Namespace == "System") {
				break;
			}
			validTypes.Add(t);
			t = t.BaseType;
		}

		BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
		List<MethodInfo> methods = new List<MethodInfo>();
		foreach (System.Type currentType in validTypes) {
			methods.AddRange(currentType.GetMethods(flags));
		}
		
		return methods.ToArray();
	}

	void TriggerEventList(EventDelegateList eventList) {
		if (eventList == null) {
			return;
		}

		for (int i = 0; i < eventList.eventTarget.Length; i++) {
			GameObject target = eventList.eventTarget[i];
			string componentName = eventList.eventMethod[i].Split('.')[0];
			string methodName = eventList.eventMethod[i].Split('.')[1];

			UnityEngine.Object component = target.GetComponent(componentName) as UnityEngine.Object;
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			MethodInfo[] methods = component.GetType().GetMethods(flags);
			MethodInfo method = null;
			foreach (MethodInfo m in methods) {
				if (m.Name == methodName) {
					method = m;
				}
			}
			
			if (method == null) {
				continue;
			}
			
			System.Object[] paramValues = eventList.eventParams[i].GetParams(method.GetParameters());
			method.Invoke(component, paramValues);
		}
	}

	public void OnClick() {
		TriggerEventList(onClick);
	}

	public void OnHoverIn() {
		TriggerEventList(onHoverIn);
	}

	public void OnHoverOut() {
		TriggerEventList(onHoverOut);
	}

	public void OnPress() {
		TriggerEventList(onPress);
	}

	public void OnRelease() {
		TriggerEventList(onRelease);
	}
}
