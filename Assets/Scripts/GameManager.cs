using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {
	#region  VARS

	// const vars for deck and cards
	private const int CARDS_IN_SUIT = 13;
	private const int CARDS_IN_DECK = 52;
	private const int STARTING_CARDS_IN_DECK = 24;

	// const vars for Score
	private const int MASTER_SUIT_SCORE = 10;
	private const int UNCOVER_CARD_SCORE = 5;
	private const int RESET_DECK_SCORE = -100;

	//Initial card's array for shuffling
	[HideInInspector]
	public Card[] deck = new Card[CARDS_IN_DECK];

	//Il prefab di carta per instanziare
	public Card prefabCard;


	private UndoEnum undoEnum = UndoEnum.None;

	public Row movableRow = null; //This row is used to move cards or listofcards around the gametable 

	public Row uncoveredDeckCardsRow = null; 

	public Row coveredDeckCardsRow = null;

	public Camera camera;

	Row lastUsedRow = null;
	Card lastUsedCard = null;
	private int lastPointsAdded = 0;
	public Text movesUI;
	public Text scoreUI;

	public GameObject victoryPanel;

	public Text victoryMoves;
	public Text victoryScore;

	private int i_moves = 0;
	private int i_score = 0;


	[SerializeField]
	Row[] rowsOnTableArray = null; //Array Containing the rows on the game table

	[SerializeField]
	Row[] masterSuitsRowsArray = null; //Array containing the rows of master suits

	#endregion

	#region UNITY METHODS
	//Init the deck
	void Awake () {

	
		//Number of cards in deck, incremented every loops til max
		int cardsInDeck = 0;

		foreach (SuitEnum suit in System.Enum.GetValues (typeof(SuitEnum))){
			if (suit != SuitEnum.None) {
				for (int index = 1; index <= CARDS_IN_SUIT; index++) {
					//Instantiate new card and init it before adding to the card array for shuffling
					Card newCard = Instantiate (prefabCard, new Vector3 (prefabCard.transform.position.x, prefabCard.transform.position.y, 0), Quaternion.identity) as Card;
					newCard.sortingGrp.sortingOrder = cardsInDeck;
					newCard.InitCard (suit, index);
					newCard.Cover ();


					deck [cardsInDeck++] = newCard;
				}
			}
		}

		shuffleDeck ();

	}
	//Init the game table
	void Start () {
		
		for(int rowOfTableArray = 0; rowOfTableArray < rowsOnTableArray.Length; rowOfTableArray++){
			Transform rowTransform = rowsOnTableArray [rowOfTableArray].transform;
			for(int cardsInRow = 0; cardsInRow <= rowOfTableArray; cardsInRow++){
				Card carta = coveredDeckCardsRow.coveredCardsInRow.DropLastMono ();
				if(cardsInRow == rowOfTableArray){
					carta.Uncover ();
				}



				rowsOnTableArray [rowOfTableArray].AddCardToRow (carta);
			}
		}



	}
	
	// Game Logic
	void Update () {
		//If there are no cards in the movable row
		#if UNITY_ANDROID
		if(Input.touchCount > 0){
			Touch touch = Input.GetTouch (0); 
			Vector3 touchPos = new Vector3(touch.position.x,touch.position.y, 10);
			if(movableRow.uncoveredCardsInRow.Count == 0){
				//If mouse or tap happen
				if(Input.GetTouch (0).phase == TouchPhase.Began){
					//We try to move cards, if selected, to the movable row

					TransferCardToMovableRow (touchPos);
				}
			}else{
				//if mouse or tap is released
				if(Input.GetTouch (0).phase == TouchPhase.Ended){
					//We try to transfer cards to the overlapped column
					TransferCardsToHitColumn (touchPos);
					//Then we check if we have win condition
					if(CheckIfWinConditions ()){
						DisplayVictoryMessage ();
					}
				}else if(Input.GetTouch (0).phase == TouchPhase.Stationary || Input.GetTouch (0).phase == TouchPhase.Moved){ //Else If cards are dragged we drag them
					Vector3 pointToBeDrag = camera.ScreenToWorldPoint (touchPos);
					pointToBeDrag.z = 0; //Fix for position.Z that goes to -10
					movableRow.transform.position = pointToBeDrag;
				}

			}
		}
		#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
			if(movableRow.uncoveredCardsInRow.Count == 0){
				//If mouse or tap happen
				if(Input.GetMouseButtonDown (0)){
					//We try to move cards, if selected, to the movable row
					TransferCardToMovableRow (Input.mousePosition);
				}
			}else{
				//if mouse or tap is released
				if(Input.GetMouseButtonUp (0)){
					//We try to transfer cards to the overlapped column
				TransferCardsToHitColumn (Input.mousePosition);
					//Then we check if we have win condition
					if(CheckIfWinConditions ()){
					DisplayVictoryMessage ();
				}
				}else if(Input.GetMouseButton (0)){ //Else If cards are dragged we drag them
					Vector3 pointToBeDrag = camera.ScreenToWorldPoint (Input.mousePosition);
					pointToBeDrag.z = 0; //Fix for position.Z that goes to -10
					movableRow.transform.position = pointToBeDrag;
				}

			}
		#endif



	}

	#endregion

	#region ROW METHODS

	//We try to transfer the cards of the movable Row into the hit row if they meet transfer conditions
	public void TransferCardsToHitColumn(Vector3 touchPosition){
		Ray ray = camera.ScreenPointToRay (touchPosition);
		RaycastHit2D hit =  Physics2D.Raycast (ray.origin, ray.direction);
		Card firstCardOfMovableRow = movableRow.uncoveredCardsInRow[0];
		if (hit && hit.collider) {

			Card hitCard = hit.collider.GetComponent<Card> ();
			//Check if we hit a card
			if(hitCard){
				if(hitCard.currentRow.CheckIfCanBeAddedToThisRow (firstCardOfMovableRow)){ //Can it be added?
					ExecuteCardTransfer (hitCard.currentRow,firstCardOfMovableRow);
					lastUsedCard = firstCardOfMovableRow;
				}else{ //No it can't so we put cards back in the last used row 
					
					MoveToLastUsedRow (lastUsedRow,firstCardOfMovableRow);
				}
			}else{ //Or if we hit a Row
				Row hitRow = hit.collider.GetComponent<Row> ();
				if(hitRow && hitRow.CheckIfCanBeAddedToEmptyRow (firstCardOfMovableRow)){ //Can it be added?
					ExecuteCardTransfer (hitRow,firstCardOfMovableRow);
					lastUsedCard = firstCardOfMovableRow;
				}else{//No it can't so we put cards back in the last used row 
					
					MoveToLastUsedRow (lastUsedRow,firstCardOfMovableRow);
				}
			}

		}else{//We didn't hit anything so we put cards back in the last used row 
			
			MoveToLastUsedRow (lastUsedRow,firstCardOfMovableRow);
		}

	}

	//We try to transfer cards if hit to the movableRow in order to be movedAround
	public void TransferCardToMovableRow(Vector3 touchPosition){
		Ray ray = camera.ScreenPointToRay (touchPosition);
		RaycastHit2D hit =  Physics2D.Raycast (ray.origin, ray.direction);
		//Check if something is hit
		if(hit && hit.collider){
			Debug.Log (hit.collider.name);
			Card hitCard = hit.collider.GetComponent<Card> ();
			//Check if card is hit and its backfacing and is not in a masterSuitRow
			if(hitCard && !hitCard.b_isBackFacing && !hitCard.currentRow.b_isMasterSuitRow){
				Transform colonnaMobileTrans = movableRow.transform;
				colonnaMobileTrans.position = hitCard.transform.position;
				lastUsedRow = hitCard.currentRow; 			//I save the lastUsed Row
				MoveToLastUsedRow (movableRow, hitCard);
			}

		}
	}


	//Move cards back to the last used row
	public void MoveToLastUsedRow( Row destinationRow, Card card){
		List<Card> tempCardsInRow = card.currentRow.uncoveredCardsInRow;
		int indexHit = tempCardsInRow.IndexOf (card);

		//Transfert the hit card and children in movableRow
		for(int i = indexHit; i < tempCardsInRow.Count; i++){
			destinationRow.AddCardToRow (tempCardsInRow [i]);
		}
		while(tempCardsInRow.Count > indexHit){
			tempCardsInRow.RemoveAt (tempCardsInRow.Count-1);
		}
	}

	//Move cards to the destination row and add move and points 
	public void ExecuteCardTransfer(Row destinationRow,Card firstCardOfMovableRow){
		MoveToLastUsedRow (destinationRow,firstCardOfMovableRow);
		if(destinationRow != lastUsedRow){
			addMove ();
			lastPointsAdded = 0;
			if(destinationRow.b_isMasterSuitRow){
				addPoints (MASTER_SUIT_SCORE);
			}
			undoEnum = UndoEnum.RowMove;
			if(lastUsedRow.UncoverLastCoveredCard ()){
				undoEnum = UndoEnum.RowMoveCover;
				addPoints (UNCOVER_CARD_SCORE);
			}

			//lastUsedRow = null;
		}
	}

	#endregion

	#region DECK METHODS

	//Called when the button on top of the coverDeck is clicked, loops the cards in it and reset if the finish.
	public void OnCoverDeckClick(){

		//If there are still cards in the cover deck
		if(coveredDeckCardsRow.coveredCardsInRow.Count > 0){
			addMove ();
			Card card = coveredDeckCardsRow.coveredCardsInRow.DropLastMono ();
			card.Uncover ();
			uncoveredDeckCardsRow.AddCardToRow (card);
			undoEnum = UndoEnum.DeckClick;
		}else{ //If cover deck is empty reset it and detract 100 points
			if(uncoveredDeckCardsRow.uncoveredCardsInRow.Count > 0){
				addMove ();
				coveredDeckCardsRow.addCardsToRow (uncoveredDeckCardsRow.uncoveredCardsInRow, true);

				uncoveredDeckCardsRow.uncoveredCardsInRow.Clear ();
				coveredDeckCardsRow.coveredCardsInRow.Reverse ();
				addPoints (RESET_DECK_SCORE);
				undoEnum = UndoEnum.DeckReset;
			}
		}

	}

	private void shuffleDeck (){
		coveredDeckCardsRow.coveredCardsInRow.Clear (); //For match reset
		int max = deck.Length;
		while (max > 0){
			int randIndex = Random.Range (0, max - 1);
			coveredDeckCardsRow.AddCardToRow (deck[randIndex]);
			deck [randIndex] = deck [max - 1];
			max--;
		}
	}

	//Check if all the master suit array are full, returns true if they are
	public bool CheckIfWinConditions(){
		
		for(int i = 0; i < masterSuitsRowsArray.Length; i++){
			if(masterSuitsRowsArray[i].uncoveredCardsInRow.Count < CARDS_IN_SUIT){
				return false;
			}
		}

		return true;
	}

	#endregion

	#region UI AND RESET METHODS

	//add a move to the moveUI
	private void addMove(){
		i_moves++;
		movesUI.text = i_moves.ToString ();
	}

	//add a move to the moveUI
	private void removeMove(){
		i_moves--;
		movesUI.text = i_moves.ToString ();
	}

	//add points to the scoreUI
	private void addPoints(int punti){
		
		i_score = Mathf.Max (i_score + punti, 0);
		lastPointsAdded = -(i_score);
		scoreUI.text = i_score.ToString ();
	}

	//Enable the victory panel
	private void DisplayVictoryMessage(){
		victoryPanel.SetActive (true);
		victoryScore.text += scoreUI.text;
		victoryMoves.text += movesUI.text;
	}

	public void NewGame(){
		
		for(int index = 0; index < rowsOnTableArray.Length; index++){
			ClearRow (rowsOnTableArray [index]);
		}

		for (int index = 0; index < masterSuitsRowsArray.Length; index++) {
			ClearRow (masterSuitsRowsArray [index]);
		}
		ClearRow (uncoveredDeckCardsRow);
		ClearRow (coveredDeckCardsRow);
		ResetUI ();
		SceneManager.LoadScene ("Main");
	}

	private void ClearRow(Row rowToBeCleared){
		rowToBeCleared.coveredCardsInRow.Clear ();
		rowToBeCleared.uncoveredCardsInRow.Clear ();
	}

	private void ResetUI(){
		movesUI.text = "0";
		scoreUI.text = "0";
	}

	#endregion


	//Check the last action and revert it
	public void UndoMove(){
		switch(undoEnum)
		{
		case UndoEnum.RowMove: //row move
			{
				removeMove ();
				addPoints (lastPointsAdded);
				MoveToLastUsedRow (lastUsedRow, lastUsedCard);
				break;
			}
		case UndoEnum.DeckClick: //Deck click
			{
				removeMove ();
				Card card = uncoveredDeckCardsRow.uncoveredCardsInRow.DropLastMono ();
				card.Cover ();
				coveredDeckCardsRow.AddCardToRow (card);
				undoEnum = UndoEnum.None;
				break;
			}
		case UndoEnum.DeckReset: //Deck reset
			{
				if(coveredDeckCardsRow.coveredCardsInRow.Count > 0){
					removeMove ();
					coveredDeckCardsRow.coveredCardsInRow.Reverse ();
					uncoveredDeckCardsRow.addCardsToRow (coveredDeckCardsRow.coveredCardsInRow, false);
					coveredDeckCardsRow.coveredCardsInRow.Clear ();

					addPoints (lastPointsAdded);
					undoEnum = UndoEnum.None;

				}
				break;
			}
		case UndoEnum.RowMoveCover:
			{
				removeMove ();
				addPoints (lastPointsAdded);
				lastUsedRow.uncoveredCardsInRow.ReadLastMono ().Cover ();
				lastUsedRow.coveredCardsInRow.Add (lastUsedRow.uncoveredCardsInRow.DropLastMono ());
				MoveToLastUsedRow (lastUsedRow, lastUsedCard);
				break;
			}
		}
	}
}
