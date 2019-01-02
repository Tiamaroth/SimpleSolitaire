using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This class is used to preLoad all the assets of the game
[CreateAssetMenu (fileName="AssetManager" , menuName="Carte")]
public class AssetManager : ScriptableObject {

	public Sprite[] ranks;

	[Space (10)]

	public Sprite[] suits;

	[Space (10)]
	public Sprite covered;

	[Space (10)]

	public Sprite uncovered;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Sprite GetRank(int rank){
		//TODO controlli lunghezza numero
		return ranks [rank-1];
	}

	public Sprite GetSuit(SuitEnum suit){
		//TODO controlli lunghezza seme
		return suits [(int) suit - 1];
	}
}
