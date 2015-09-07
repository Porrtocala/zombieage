using UnityEngine;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class EventDelegateList : ScriptableObject {
	[SerializeField]
	public GameObject[] eventTarget;
	[SerializeField]
	public string[] eventMethod;
	[SerializeField]
	public MethodParamList[] eventParams;

	public EventDelegateList() {
		eventTarget = new GameObject[0];
		eventMethod = new string[0];
		eventParams = new MethodParamList[0];
	}

	MethodInfo GetMethodInfo(GameObject target, string method) {
		string componentName = method.Split('.')[0];
		string methodName = method.Split('.')[1];

		UnityEngine.Object component = target.GetComponent(componentName) as UnityEngine.Object;
		MethodInfo[] methods = EventTrigger.GetObjectMethods(component.GetType());
		MethodInfo methodInfo = null;
		foreach (MethodInfo m in methods) {
			if (m.Name == methodName) {
				methodInfo = m;
			}
		}

		return methodInfo;
	}

	void AddEmptyEvent() {
		GameObject[] oldTargets = this.eventTarget;
		string[] oldMethods = this.eventMethod;
		MethodParamList[] oldParams = this.eventParams;
		
		// Copy old list
		int cnt = this.eventTarget.Length + 1;
		this.eventTarget = new GameObject[cnt];
		this.eventMethod = new string[cnt];
		this.eventParams = new MethodParamList[cnt];
		for (int i = 0; i < cnt - 1; i++) {
			this.eventTarget[i] = oldTargets[i];
			this.eventMethod[i] = oldMethods[i];
			this.eventParams[i] = oldParams[i];
		}
	}

	public void AddEvent(GameObject target, string method, System.Object[] paramList) {
		AddEmptyEvent();
		int index = eventTarget.Length - 1;
		eventTarget[index] = target;
		eventMethod[index] = method;
		eventParams[index] = ScriptableObject.CreateInstance<MethodParamList>();
		
		MethodInfo methodInfo = this.GetMethodInfo(target, method);
		ParameterInfo[] paramInfo = methodInfo.GetParameters();
		eventParams[index].Init(paramInfo);
		int cnt = 0;
		foreach (System.Object obj in paramList) {			
			eventParams[index].SetParam(cnt, obj, paramInfo);
			cnt++;
		}
	}

	public void Clear() {
		eventTarget = new GameObject[0];
		eventMethod = new string[0];
		eventParams = new MethodParamList[0];
	}
}
