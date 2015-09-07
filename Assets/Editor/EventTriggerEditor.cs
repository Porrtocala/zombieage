using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

[CustomEditor(typeof(EventTrigger))]
public class EventTriggerEditor : Editor {
	
	EventTrigger _target;
	Dictionary<string, bool> folded;

	public void OnEnable() {
		folded = new Dictionary<string, bool>();
		_target = (EventTrigger) target;
	}

	System.Object GenericField(System.Object value, System.Type type) {
		if (type == typeof(int)) {
			if (value == null) {
				value = 0 as System.Object;
			}
			value = EditorGUILayout.IntField((int)value) as System.Object;
		} else if (type == typeof(string)) {
			if (value == null) {
				value = "" as System.Object;
			}
			value = EditorGUILayout.TextField((string)value) as System.Object;
		} else if (type == typeof(bool)) {
			if (value == null) {
				value = false as System.Object;
			}

			int x = (bool) value == true ? 1 : 0;
			x = EditorGUILayout.Popup(x, new string[] {"False", "True"});
			value = (x == 1) as System.Object;
		} else if (type == typeof(float)) {
			if (value == null) {
				value = 0f as System.Object;
			}

			value = EditorGUILayout.FloatField((float) value) as System.Object;
		} else {
			value = EditorGUILayout.ObjectField(value as UnityEngine.Object, type, true) as System.Object;
		}

		return value;
	}

	public void RemoveEvent(EventDelegateList eventList, int index) {
		GameObject[] oldTargets = eventList.eventTarget;
		string[] oldMethods = eventList.eventMethod;
		MethodParamList[] oldParams = eventList.eventParams;
		
		// Copy old list
		int newLength = eventList.eventTarget.Length - 1;
		eventList.eventTarget = new GameObject[newLength];
		eventList.eventMethod = new string[newLength];
		eventList.eventParams = new MethodParamList[newLength];

		int cnt = 0;
		for (int i = 0; i < oldTargets.Length; i++) {
			if (i == index) {
				continue;
			}

			eventList.eventTarget[cnt] = oldTargets[i];
			eventList.eventMethod[cnt] = oldMethods[i];
			eventList.eventParams[cnt] = oldParams[i];
			cnt++;
		}
	}

	public void AddEvent(EventDelegateList eventList) {
		GameObject[] oldTargets = eventList.eventTarget;
		string[] oldMethods = eventList.eventMethod;
		MethodParamList[] oldParams = eventList.eventParams;
		
		// Copy old list
		int cnt = eventList.eventTarget.Length + 1;
		eventList.eventTarget = new GameObject[cnt];
		eventList.eventMethod = new string[cnt];
		eventList.eventParams = new MethodParamList[cnt];
		for (int i = 0; i < cnt - 1; i++) {
			eventList.eventTarget[i] = oldTargets[i];
			eventList.eventMethod[i] = oldMethods[i];
			eventList.eventParams[i] = oldParams[i];
		}
	}

	public void ClearEvents(EventDelegateList eventList) {
		eventList.eventTarget = new GameObject[0];
		eventList.eventMethod = new string[0];
		eventList.eventParams = new MethodParamList[0];
	}

	public EventDelegateList EventDelegateListField(string label, EventDelegateList eventList) {
		if (eventList == null) {
			eventList = ScriptableObject.CreateInstance<EventDelegateList>();
		}

		if (eventList.eventTarget == null) {
			ClearEvents(eventList);
		}

		EditorGUILayout.BeginVertical("box");
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		bool foldValue = folded.ContainsKey(label) ? folded[label] : true;		
		foldValue = EditorGUILayout.Foldout(foldValue, label);
		EditorGUILayout.EndHorizontal();

		folded[label] = foldValue;
		if (!foldValue) {
						
			EditorGUILayout.EndVertical();
			return eventList;
		}

		int eventCount = eventList.eventTarget.Length;
		for (int i = 0; i < eventList.eventTarget.Length; i++) {
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.BeginHorizontal();			
			EditorGUILayout.LabelField("#" + i.ToString());
			if (GUILayout.Button("-", GUILayout.Width(20))) {
				RemoveEvent(eventList, i);
				EditorGUILayout.EndVertical();				
				break;
			}
			EditorGUILayout.EndHorizontal();
			

			GameObject gameobject = eventList.eventTarget[i];
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("GameObject");
			gameobject = EditorGUILayout.ObjectField((Object) gameobject, typeof(GameObject), true) as GameObject;
			EditorGUILayout.EndHorizontal();
			eventList.eventTarget[i] = gameobject;

			if (gameobject == null) {
				EditorGUILayout.EndVertical();				
				continue;
			}

			Object[] components = gameobject.GetComponents<MonoBehaviour>();
			List<string> methodNames = new List<string>();
			List<MethodInfo> allMethods = new List<MethodInfo>();
			methodNames.Add("None");
			foreach (Object component in components) {
				if ((component as EventTrigger) == _target) {
					continue;
				}
				
				//BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
				//MethodInfo[] methods = component.GetType().GetMethods(flags);
				MethodInfo[] methods = EventTrigger.GetObjectMethods(component.GetType());
				foreach (MethodInfo method in methods) {
					methodNames.Add(component.GetType().ToString() + "." + method.Name);
					allMethods.Add(method);					
				}
			}

			string currentMethod = eventList.eventMethod[i] == null ? eventList.eventMethod[i] : "None";
			int selectedIndex = methodNames.FindIndex(item => item == eventList.eventMethod[i]);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Method: ");
			selectedIndex = EditorGUILayout.Popup(selectedIndex, methodNames.ToArray());
			EditorGUILayout.EndHorizontal();
			
			if (selectedIndex > -1) {
				eventList.eventMethod[i] = methodNames[selectedIndex];

				if (selectedIndex == 0) {
					EditorGUILayout.EndVertical();
					continue;
				}

				ParameterInfo[] methodParams = allMethods[selectedIndex - 1].GetParameters();
				if (eventList.eventParams[i] == null) {				
					eventList.eventParams[i] = ScriptableObject.CreateInstance<MethodParamList>();
					eventList.eventParams[i].Init(methodParams);
				}
				
				for (int j = 0; j < methodParams.Length; j++) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(methodParams[j].Name);
					System.Object value = eventList.eventParams[i].GetParam(j, methodParams);
					value = GenericField(value, methodParams[j].ParameterType);
					eventList.eventParams[i].SetParam(j, value, methodParams);
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();			
		}

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add")) {
			AddEvent(eventList);
		}

		if (GUILayout.Button("Clear")) {
			ClearEvents(eventList);
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		return eventList;
	}

	public override void OnInspectorGUI() {
		_target.onClick = EventDelegateListField("On Click", _target.onClick);
		_target.onHoverIn = EventDelegateListField("On Hover In", _target.onHoverIn);
		_target.onHoverOut = EventDelegateListField("On Hover Out", _target.onHoverOut);
		_target.onPress = EventDelegateListField("On Press", _target.onPress);
		_target.onRelease = EventDelegateListField("On Release", _target.onRelease);

		EditorUtility.SetDirty(_target);
	}
}
