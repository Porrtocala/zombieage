using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MethodParamList : ScriptableObject {
	[System.Serializable]
	public class IntDict : SerializableDictionary<string, int> {};
	[System.Serializable]
	public class ObjDict : SerializableDictionary<string, UnityEngine.Object> {};
	[System.Serializable]
	public class StrDict : SerializableDictionary<string, string> {};
	[System.Serializable]
	public class BoolDict : SerializableDictionary<string, bool> {};
	[System.Serializable]
	public class FloatDict : SerializableDictionary<string, float> {};

	[SerializeField]
	ObjDict objParamList;
	[SerializeField]
	StrDict strParamList;
	[SerializeField]
	IntDict intParamList;
	[SerializeField]
	BoolDict boolParamList;
	[SerializeField]
	FloatDict floatParamList;
		
	public void Init(ParameterInfo[] methodInfo) {
		this.boolParamList = new BoolDict();		
		this.strParamList = new StrDict();
		this.intParamList = new IntDict();
		this.objParamList = new ObjDict();
		this.floatParamList = new FloatDict();		
	}
	
	public System.Object GetParam(int index, ParameterInfo[] methodInfo) {
		System.Type paramType = methodInfo[index].ParameterType;
		string paramName = methodInfo[index].Name;
		if (paramType == typeof(int)) {
			return (intParamList.ContainsKey(paramName) ? intParamList[paramName] : 0) as System.Object;
		} else if (paramType == typeof(string)) {
			return (strParamList.ContainsKey(paramName) ? strParamList[paramName] : null) as System.Object;
		} else if (paramType == typeof(bool)) {
			return (boolParamList.ContainsKey(paramName) ? boolParamList[paramName] : false) as System.Object;
		} else if (paramType == typeof(float)) {
			return (floatParamList.ContainsKey(paramName) ? floatParamList[paramName] : 0f) as System.Object;			
		} else if (paramType.IsSubclassOf(typeof(Object))) {
			return (objParamList.ContainsKey(paramName) ? objParamList[paramName] : null) as System.Object;
		} else {
			return null;
		}
	}
	
	public void SetParam(int index, System.Object value, ParameterInfo[] methodInfo) {
		System.Type paramType = methodInfo[index].ParameterType;
		string paramName = methodInfo[index].Name;
		if (paramType == typeof(int)) {
			intParamList[paramName] = (int) value;
		} else if (paramType == typeof(string)) {
			strParamList[paramName] = (string) value;
		} else if (paramType == typeof(bool)) {
			boolParamList[paramName] = (bool) value;
		} else if (paramType == typeof(float)) {
			floatParamList[paramName] = (float) value;
		} else if (paramType.IsSubclassOf(typeof(Object))) {
			objParamList[paramName] = value as UnityEngine.Object;
		}
	}
	
	public System.Object[] GetParams(ParameterInfo[] methodInfo) {
		System.Object[] values = new System.Object[methodInfo.Length];
		for (int i = 0; i < methodInfo.Length; i++) {
			values[i] = GetParam(i, methodInfo);
		}
		
		return values;
	}
}

/*
[System.Serializable]
public class MethodParamList : ScriptableObject {
	[SerializeField]
	Object[] objParamList;
	[SerializeField]
	string[] strParamList;
	[SerializeField]
	int[] intParamList;
	[SerializeField]
	bool[] boolParamList;
	
	int CountParamInfoType(System.TYPE type, ParameterInfo[] methodInfo, int index = -1) {
		if (index == -1) {
			index = methodInfo.Length + 1;
		}
		
		int cnt = 0;
		for (int i = 0; i < index - 1; i++) {
			ParameterInfo paramInfo = methodInfo[i];
			if (type.IsSubclassOf(paramInfo.ParameterType) || type == paramInfo.ParameterType) {
				cnt++;
			}
		}
		
		return cnt;
	}
	
	public void Init(ParameterInfo[] methodInfo) {		
		this.objParamList = new Object[CountParamInfoType(typeof(Object), methodInfo)];
		this.strParamList = new string[CountParamInfoType(typeof(string), methodInfo)];
		this.intParamList = new int[CountParamInfoType(typeof(int), methodInfo)];
		this.boolParamList = new bool[CountParamInfoType(typeof(bool), methodInfo)];
	}
	
	public System.Object GetParam(int index, ParameterInfo[] methodInfo) {
		System.TYPE paramType = methodInfo[index].ParameterType;
		if (paramType == typeof(int)) {
			return intParamList[CountParamInfoType(typeof(int), methodInfo, index)] as System.Object;
		} else if (paramType == typeof(string)) {
			return strParamList[CountParamInfoType(typeof(string), methodInfo, index)] as System.Object;
		} else if (paramType == typeof(bool)) {
			return boolParamList[CountParamInfoType(typeof(bool), methodInfo, index)] as System.Object;
		} else if (paramType == typeof(Object)) {
			return objParamList[CountParamInfoType(typeof(int), methodInfo, index)] as System.Object;
		} else {
			return null;
		}
	}
	
	public void SetParam(int index, System.Object value, ParameterInfo[] methodInfo) {
		System.TYPE paramType = methodInfo[index].ParameterType;
		if (paramType == typeof(int)) {
			intParamList[CountParamInfoType(typeof(int), methodInfo, index)] = (int) value;
		} else if (paramType == typeof(string)) {
			strParamList[CountParamInfoType(typeof(string), methodInfo, index)] = (string) value;
		} else if (paramType == typeof(bool)) {
			boolParamList[CountParamInfoType(typeof(bool), methodInfo, index)] = (bool) value;
		} else if (paramType == typeof(Object)) {
			objParamList[CountParamInfoType(typeof(int), methodInfo, index)] = value as Object;
		}
	}

	public System.Object[] GetParams(ParameterInfo[] methodInfo) {
		System.Object[] values = new System.Object[methodInfo.Length];
		for (int i = 0; i < methodInfo.Length; i++) {
			values[i] = GetParam(i, methodInfo);
		}

		return values;
	}
}*/