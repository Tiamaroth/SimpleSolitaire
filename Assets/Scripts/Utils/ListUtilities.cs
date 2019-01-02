using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This class inject methods in the list class .
public static class ListUtilities{
	

	//Read the last element of a monobehaviour list
	public static T ReadLastMono <T> (this List<T> list) where T : MonoBehaviour {
		
		return list.Count > 0 ? list [list.Count - 1] : null;
	}

	//Returns the last element of a monobehaviour list after removing it
	public static T DropLastMono <T> (this List<T> list) where T : MonoBehaviour {
		int listLenght = list.Count - 1;
		if (listLenght >= 0) {
			T lastMono = list [listLenght];
			list.RemoveAt (listLenght);
			return lastMono;
		}
		return null;
	}
	//Returns the i elements of the monobehaviour list after removing from it
	public static T DropMonoAtIndex <T> (this List<T> list, int index) where T : MonoBehaviour {
		
		T indexMono = list [index];
		list.RemoveAt (index);
		return indexMono;
	}



}
