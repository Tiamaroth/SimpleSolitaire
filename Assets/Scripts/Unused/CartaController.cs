/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CartaController : MonoBehaviour {

	[HideInInspector]
	public Card[] mazzo;
	public static List<Card> listaCarte = new List<Card>(52);
	public Card carta;
	private readonly int CARTE_IN_SEME = 13;
	private readonly int CARTE_IN_MAZZO = 52;


	void Awake () {
		mazzo = new Card[CARTE_IN_MAZZO];

		System.Array enumVal = System.Enum.GetValues (typeof(SuitEnum));
		int j = 0;
		//Creo il mazzo
		foreach (SuitEnum seme in enumVal) {
			for (int i = 1; i <= CARTE_IN_SEME; i++) {
				Card newCarta = Instantiate (carta ,new Vector3(carta.transform.position.x, carta.transform.position.y, 0), Quaternion.identity) as Card;
				newCarta.sortingGrp.sortingOrder = j;
				newCarta.InitCard (seme, i);
				newCarta.Cover ();


				mazzo[j++] = newCarta;
			}
		}


	}
		
	void Start(){
		
	}
	

	void Update () {
		
	}




}
*/

