using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class Card:MonoBehaviour{
	#region VARS
	public SuitEnum e_suit {get; private set;} 

	public int i_rank {get; private set;} 



	public bool b_isBackFacing {get; private set;} //Vero se coperta 



	public AssetManager assetManager; 

	private Transform tr_lastParent;

	[HideInInspector]
	public Row currentRow = null;


	public SpriteRenderer spr_back = null; //La sprite di sfondo


	public SpriteRenderer spr_suit  = null; //La sprite del seme


	public SpriteRenderer spr_rank  = null; //La sprite del numero

	public SortingGroup sortingGrp = null; 

	public BoxCollider2D collider = null;

	#endregion

	#region METHODS

	//Sets the suit , the rank , the name in editor and sprites of a single card
	public void InitCard(SuitEnum suit, int rank){
		this.e_suit = suit;
		this.i_rank = rank;
		this.gameObject.name = this.i_rank + " di " + this.e_suit; 

		spr_suit.sprite = assetManager.GetSuit (suit);
		spr_rank.sprite = assetManager.GetRank (rank);

		if(suit == SuitEnum.Spades || suit == SuitEnum.Clubs){
			spr_rank.color = Color.black;
		}else{
			spr_rank.color = Color.red;
		}


	}


	//Flip the card revealing it
	public void Uncover(){
		b_isBackFacing = false;
		spr_rank.enabled = true;
		spr_suit.enabled = true;
		spr_back.sprite = assetManager.uncovered;

	}
	//Flip the card covering it
	public void Cover(){
		b_isBackFacing = true;
		spr_rank.enabled = false;
		spr_suit.enabled = false;
		spr_back.sprite = assetManager.covered;

	}



	#endregion

}
