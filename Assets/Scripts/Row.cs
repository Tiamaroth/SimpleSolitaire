using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Row : MonoBehaviour {

	#region VARS

	private const int K_CARD = 13;

	private const int ACE_CARD = 1;



	public bool b_hasCollider = true;

	public bool b_cardsCanBeDropped = true;

	public bool b_isMasterSuitRow = false;

	public float f_cardVerticalOffset = -0.5f;

	public SuitEnum rowSuit;

	[HideInInspector]
	public List<Card> uncoveredCardsInRow = new List<Card>(13);

	[HideInInspector]
	public List<Card> coveredCardsInRow = new List<Card>(6);

	#endregion


	#region METHODS
	//Add a single card to the correct list of this row (depend if is covered or not)
	public void AddCardToRow(Card card){
		int cardIndex = uncoveredCardsInRow.Count + coveredCardsInRow.Count;
		card.currentRow = this;
		card.transform.SetParent (this.transform);
		card.collider.enabled = b_hasCollider;
		card.transform.localPosition = new Vector3 (0, (cardIndex * f_cardVerticalOffset), -(cardIndex+1));
		card.sortingGrp.sortingOrder = cardIndex;
		if(card.b_isBackFacing){
			coveredCardsInRow.Add (card);
		}else{
			uncoveredCardsInRow.Add (card);
		}
	}

	//Add a list of cards to the end of the correct list of this row (depend if cards are covered or not)
	public void addCardsToRow(List<Card> cardList, bool cardsAreCovered){
		
		for(int i = 0; i < cardList.Count; i++){
			if (cardsAreCovered) {
				cardList [i].Cover ();
			} else {
				cardList [i].Uncover ();
			}
			AddCardToRow (cardList[i]);
		}
	}

	//Check if a card can be added to an empty row , returns true if it can.
	public bool CheckIfCanBeAddedToEmptyRow(Card card){
		
		if (b_cardsCanBeDropped) {
			if ((uncoveredCardsInRow.Count + coveredCardsInRow.Count) == 0) {
				if (b_isMasterSuitRow) {
					return card.i_rank == ACE_CARD && card.e_suit == rowSuit;
				}
				return  card.i_rank == K_CARD;
			} else if (uncoveredCardsInRow.Count == 0) {
				return  false;
			}

			return  CheckIfCanBeAddedToThisRow (card);
		}

		return false;
	}

	//Uncover the last covered card if there are no uncoveredCard in the row , returns true if uncover and its used to manage score
	public bool UncoverLastCoveredCard(){
		
		if(coveredCardsInRow.Count > 0 && uncoveredCardsInRow.Count == 0){
			coveredCardsInRow.ReadLastMono ().Uncover ();
			uncoveredCardsInRow.Add (coveredCardsInRow.DropLastMono ());
			return true;
		}
		return false;
	}

	//Check if a card can be added to the row from wich this method is called, returns true if it can
	public bool CheckIfCanBeAddedToThisRow(Card cardToAdd){
		
		if (b_cardsCanBeDropped) {
			Card card = uncoveredCardsInRow.ReadLastMono ();
			if (card) {
				if (b_isMasterSuitRow) {
					return cardToAdd.e_suit == card.e_suit && cardToAdd.i_rank == (card.i_rank + 1);
				} else {
					return cardToAdd.spr_rank.color != card.spr_rank.color && cardToAdd.i_rank == (card.i_rank - 1);
				}
			}

		}
		return false;
	}

	#endregion
}
