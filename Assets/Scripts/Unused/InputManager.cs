/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class InputManager : MonoBehaviour {

	public static Vector3 getClickDown(){
		if(SystemInfo.deviceType.Equals(DeviceType.Desktop)){
			if(Input.GetMouseButtonDown (0)){
				return Camera.main.ScreenToWorldPoint (Input.mousePosition);
			}
		}

		if(SystemInfo.deviceType.Equals(DeviceType.Handheld)){
			Debug.Log ("SONO SU TOUCH");
		}
		return Vector3.zero;
	}

	public static Vector3 getContinuedClick(){
		if(SystemInfo.deviceType.Equals(DeviceType.Desktop)){
			if(Input.GetMouseButton (0)){
				return Camera.main.ScreenToWorldPoint (Input.mousePosition);
			}
		}

		if(SystemInfo.deviceType.Equals(DeviceType.Handheld)){
			Debug.Log ("SONO SU TOUCH");
		}
		return Vector3.zero;
	}

	public static Vector3 getClickUp(){
		if(SystemInfo.deviceType.Equals(DeviceType.Desktop)){
			if(Input.GetMouseButtonUp(0)){
				return Camera.main.ScreenToWorldPoint (Input.mousePosition);
			}
		}

		if(SystemInfo.deviceType.Equals(DeviceType.Handheld)){
			Debug.Log ("SONO SU TOUCH");
		}
		return Vector3.zero;
	}
}
*/