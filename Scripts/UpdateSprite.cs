using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    private SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Blackjack blackjack;


    // Start is called before the first frame update
    void Start()
    {
        Deck tempDeck = new Deck();
        tempDeck._deck = Blackjack.InitDeck();
        blackjack = FindObjectOfType<Blackjack>();
        int i = 0;
        foreach (Card c in tempDeck._deck)
        {
            if (this.name == Blackjack.ValueNames[(int)c.Value] + ((int)c.Suit).ToString())
            {

                //print(i);
                cardFace = blackjack.CardFaces[i];
                
                break;
            }
            i++;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        //   spriteRenderer.flipX = true;

        selectable = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        if(selectable.faceUp == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }
}
