using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//This class acts as a way of holding the players name and money
//In the future players will be able to save and load their profile
public class Player
{
    public double money = 100000;
    public string name;
    public float hitcount = 0f;

    public double getMoney()
    {
        return money;
    }

    public void setMoney(double amount)
    {
        money = amount;
    }

    public void addMoney(double amount)
    {
        money += amount;
    }

    public void setName(string name9)
    {
        name = name9;
    }

    public string getName()
    {
        return name;
    }
    public void bet(double amount)
    {
        money = money - amount;
    }
}
//The suit and values are given to each card.
public enum Suits
{
    Hearts = 3,
    Diamonds,
    Clubs,
    Spades
};
public enum Values
{
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
};


public struct Card
{

    public Suits Suit;
    public Values Value;

    public bool played;


};

public class Deck
{
    public static string[] ValueNames = new string[]
{
    "Two",
    "Three",
    "Four",
    "Five",
    "Six",
    "Seven",
    "Eight",
    "Nine",
    "Ten",
    "Jack",
    "Queen",
    "King",
    "Ace"
};
    public int[] blackValue = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11 };
    public List<Card> _deck; //This acts as a deck and a players's hand

    public Deck()
    {
        _deck = new List<Card>();
    }
 
    //Shuffle
    public void Shuffle(int decknum)
    {
        System.Random rnd2 = new System.Random();

        int i;
        int j;
        Card Temp;

        for (i = 0; i < _deck.Count * decknum; i++)
        {
            j = rnd2.Next(52) * decknum;
            Temp = _deck[i];
            _deck[i] = _deck[j];
            _deck[j] = Temp;
        }
    }
    //Deal Card
    public Card DealOneCard()
    {
        Card card = _deck[0];
        _deck.RemoveAt(0);
        return (card);
    }
    //Gets the value of a player's hand
    public int getCount()
    {

        int aceCount = 0;
        int handcount = 0;
        foreach (Card c in _deck)
        {
            handcount += blackValue[(int)c.Value];
            if (c.Value == Values.Ace)
                aceCount++;
        }
        while (handcount > 21 && aceCount > 0)
        {
            handcount = handcount - 10;
            aceCount--;
        }
        return handcount;
    }
};
/*
 TO DO:
 - Display current bet
 - Remove bet
 - Double bet
 - double
 - split
 - newBackground
 - new UI
 - Can still hit after stay
     
     
     */
public class Blackjack : MonoBehaviour
{
    public int[] blackValue = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11 }; //This gives the value of each card
    public Scene scene;
    public Sprite[] CardFaces;
    public GameObject CardPreFab;

    public Text pMoney;
    public Text Count;
    public Text dCount;
    public Text showBet1;
    public Text winText;
    public Deck bjDeck = new Deck();
    public Deck player = new Deck();
    public Deck dealer = new Deck();

    public static Player bjPlay;
    public static string[] ValueNames = new string[]
{
    "Two",
    "Three",
    "Four",
    "Five",
    "Six",
    "Seven",
    "Eight",
    "Nine",
    "Ten",
    "Jack",
    "Queen",
    "King",
    "Ace"
};
    public double bet;
    public bool gameOn;
    public bool playing;

    public bool busted;
    public bool flipped;
    
    public bool doneBet;
    public bool showBet;
    public UnityEngine.UI.Button dealButton; //Dealer Button
    public UnityEngine.UI.Button hitButton; //Hit Button
    public UnityEngine.UI.Button stayButton; //Stay Button
    //Mainloop
    IEnumerator PlayBJ()

    {



        while (gameOn)
        {

            flipped = false;
            showBet = false;
            doneBet = false;
            busted = false;
            deleteText(winText);
            bjPlay.hitcount = 0f;
            playing = true;
            dealButton.gameObject.SetActive(true);
            hitButton.gameObject.SetActive(false);
            stayButton.gameObject.SetActive(false);


            if (bjDeck._deck.Count < 13)
            {
                print("shuffling");
                printText("Shuffling...");
                bjDeck._deck = InitDeck();
                bjDeck.Shuffle(1);
                yield return new WaitForSeconds(2.0f);
                deleteText(winText);
            }
            bjPlay.setMoney(bjPlay.getMoney() - bet);
            while (doneBet != true)
            {
                print("waiting...");
                yield return new WaitForSeconds(2.0f);
            }
           // 
            dealButton.gameObject.SetActive(false);
            hitButton.gameObject.SetActive(true);
            stayButton.gameObject.SetActive(true);

            player = new Deck();
            dealer = new Deck();
            //   yield return new WaitForSeconds(1.0f);
            StartCoroutine(dealInitialCards());

            if (EarlyWinner())
            {
                print("Won early");
                playing = false;
                yield return new WaitForSeconds(3.0f);
                flipDealer();
            }
            else
            {

                while (playing == true)
                {

                    if (player.getCount() > 21)
                    {
                        playing = false;
                    }
                    if(bet>bjPlay.getMoney())
                    {
                        bet = 0;
                    }
                    yield return new WaitForSeconds(2.0f);
                }
                hitButton.gameObject.SetActive(false);
                stayButton.gameObject.SetActive(false);
                flipDealer();
                yield return new WaitForSeconds(.5f);
                if (busted == false)
                {
                    StartCoroutine(addToDealer());
                }
                yield return new WaitForSeconds(1.0f);
             
                outcome();
                yield return new WaitForSeconds(2.0f);
                StartCoroutine(DeleteDeck(player, dealer));
            }
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(DeleteDeck(player, dealer));

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //winText.transform.position = new Vector3(-12.0f, -365.0f, -5.0f);
        scene = SceneManager.GetActiveScene();
        bjPlay = new Player();
        bjPlay.hitcount = 0f;
        gameOn = true;
        bjDeck._deck = InitDeck();
        bjDeck.Shuffle(1);
        StartCoroutine(PlayBJ());


    }
    IEnumerator addToDealer()
    {
        float xOFFsetdd = 4.5f - 1.5f;
        float yOFFsetdd = -3.39f;
        float zOFFsetdd = .5f;
        while (dealer.getCount() < 17)
        {

            yield return new WaitForSeconds(.3f);
            dealer._deck.Add(bjDeck.DealOneCard());
            GameObject newCard = Instantiate(CardPreFab, new Vector3(transform.position.x - xOFFsetdd + 2.5f, transform.position.y - yOFFsetdd, transform.position.z - zOFFsetdd), Quaternion.identity);
            newCard.name = ValueNames[(int)dealer._deck[dealer._deck.Count - 1].Value] + ((int)dealer._deck[dealer._deck.Count - 1].Suit).ToString();
            newCard.GetComponent<Selectable>().faceUp = true;
            yOFFsetdd += 0.0f;
            zOFFsetdd += 0.1f;
            xOFFsetdd -= 1.0f;
        }
    }
    bool outcome()
    {
        //Determine win/lose

        if (player.getCount() > 21)
        {
            printText("You lose $" + bet.ToString());
            playing = false;
            return false;
        }
        else if (dealer.getCount() > 21)
        {
            printText("You won $" + (bet * 2).ToString());
            playing = false;
            bjPlay.addMoney(bet * 2);
            return true;
        }
        else
        {
            if (player.getCount() > dealer.getCount())
            {

                printText("You win $" + (bet * 2).ToString());
                bjPlay.addMoney(bet * 2);
                playing = false;
                return true;
            }
            if (player.getCount() < dealer.getCount())
            {
                printText("You lost $" + bet.ToString());
                playing = false;
                return false;
            }
            if (dealer.getCount() == player.getCount())
            {
                printText("Push +" + bet.ToString());
                playing = false;
                bjPlay.addMoney(bet);
            }
        }
        return false;
    }
    void flipDealer()
    {
        flipped = true;
        GameObject.Find(ValueNames[(int)dealer._deck[1].Value] + ((int)dealer._deck[1].Suit).ToString()).GetComponent<Selectable>().faceUp = true;

    }
    bool EarlyWinner()
    {
        if (player.getCount() == 21)
        {

            printText("You win $" + (bet * 2.5).ToString());
            bjPlay.addMoney(bet * 2.5);

            playing = false;
            return true;
        }
        else if (player.getCount() == 21 && dealer.getCount() == 21)
        {

            bjPlay.addMoney(bet);
            printText("You lost $" + bet.ToString());
            playing = false;
            return true;
        }
        else if (dealer.getCount() == 21 && player.getCount() != 21)
        {
            printText("You lost $" + bet.ToString());
            playing = false;
            return true;
        }
        return false;
    }
    void printText(string s)
    {
        winText.text = s;
    }
    void deleteText(Text t)
    {
        t.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        showBet1.text = "$"+bet.ToString();
        pMoney.text = "$" + bjPlay.getMoney().ToString();
        if (showBet)
        {
            Count.text = player.getCount().ToString();
            if (!flipped)
                dCount.text = (blackValue[(int)dealer._deck[0].Value]).ToString();
            else
                dCount.text = dealer.getCount().ToString();
        }
        else
        {
            Count.text = "";
            dCount.text = "";
        }
    }
    //Deletes the player and dealer deck
    public IEnumerator DeleteDeck(Deck d, Deck d1)
    {
        //  yield return new WaitForSeconds(3f);
        foreach (Card c in d._deck)
        {
            yield return new WaitForSeconds(.2f);
            string temp;
            temp = ValueNames[(int)c.Value] + ((int)c.Suit).ToString();
            Destroy(GameObject.Find(temp));
        }
        foreach (Card c in d1._deck)
        {
            yield return new WaitForSeconds(.2f);
            string temp;
            temp = ValueNames[(int)c.Value] + ((int)c.Suit).ToString();
            Destroy(GameObject.Find(temp));
        }
    }
    //Initialize 52 card deck
    public static List<Card> InitDeck()
    {
        List<Card> tempDeck = new List<Card>();
        int i = 0;
        Suits S;
        Values V;
        while (i < 1)
        {
            for (S = Suits.Hearts; S <= Suits.Spades; S = (Suits)(S + 1))
            {
                for (V = Values.Two; V <= Values.Ace; V = (Values)(V + 1))
                {

                    Card temp = new Card();
                    temp.played = false;
                    temp.Suit = S;
                    temp.Value = V;
                    tempDeck.Add(temp);
                    i++;
                }
            }
        }
        return tempDeck;
    }
    void deal()
    {
        foreach (Card c in bjDeck._deck)
        {
            GameObject newCard = Instantiate(CardPreFab, transform.position, Quaternion.identity);
            newCard.name = ValueNames[(int)c.Value] + ((int)c.Suit).ToString();
        }
    }

    //Deal out player and dealer cards
    IEnumerator dealInitialCards()
    {
        for (int i = 0; i < 2; i++)
        {
            player._deck.Add(bjDeck.DealOneCard());
            dealer._deck.Add(bjDeck.DealOneCard());
        }

        float xOFFset = 1.0f;
        float yOFFset = 3.0f;
        float zOFFset = .30f;
        float xOFFsetd = 5.0f;
        float yOFFsetd = -1.1f;
        float zOFFsetd = .30f;
        yield return new WaitForSeconds(.3f);
        GameObject newCard = Instantiate(CardPreFab, new Vector3(transform.position.x - xOFFset, transform.position.y - yOFFset, transform.position.z - zOFFset), Quaternion.identity);
        newCard.name = ValueNames[(int)player._deck[0].Value] + ((int)player._deck[0].Suit).ToString();
        newCard.GetComponent<Selectable>().faceUp = true;
        yOFFset += 0.0f;
        zOFFset += 0.1f;
        xOFFset -= 1.0f;
        yield return new WaitForSeconds(.3f);
        GameObject newCard1 = Instantiate(CardPreFab, new Vector3(transform.position.x - xOFFsetd + 2.5f, transform.position.y - yOFFsetd + 2.3f, transform.position.z - zOFFsetd), Quaternion.identity);
        newCard1.name = ValueNames[(int)dealer._deck[0].Value] + ((int)dealer._deck[0].Suit).ToString();
        newCard1.GetComponent<Selectable>().faceUp = true;
        yOFFsetd += 0.0f;
        zOFFsetd += 0.1f;
        xOFFsetd -= 1.0f;
        yield return new WaitForSeconds(.3f);
        GameObject newCard2 = Instantiate(CardPreFab, new Vector3(transform.position.x - xOFFset, transform.position.y - yOFFset, transform.position.z - zOFFset), Quaternion.identity);
        newCard2.name = ValueNames[(int)player._deck[1].Value] + ((int)player._deck[1].Suit).ToString();
        newCard2.GetComponent<Selectable>().faceUp = true;
        yield return new WaitForSeconds(.3f);
        GameObject newCard3 = Instantiate(CardPreFab, new Vector3(transform.position.x - xOFFsetd + 2.5f, transform.position.y - yOFFsetd + 2.3f, transform.position.z - zOFFsetd), Quaternion.identity);
        newCard3.name = ValueNames[(int)dealer._deck[1].Value] + ((int)dealer._deck[1].Suit).ToString();
        newCard3.GetComponent<Selectable>().faceUp = false;
        showBet = true;
    }

    public void hitCard(string s)
    {
        //   while (playing) {
        if (player._deck.Count > 1)
        {
            if (player.getCount() < 21 && doneBet)
            {

                bjPlay.hitcount++;
                player._deck.Add(bjDeck.DealOneCard());

                float xOFFset = -.5f - bjPlay.hitcount++ / 2f;

                float yOFFset = 3.0f;
                float zOFFset = .10f + bjPlay.hitcount / 5;

                GameObject newCard = Instantiate(CardPreFab, new Vector3(transform.position.x - xOFFset, transform.position.y - yOFFset, transform.position.z - zOFFset), Quaternion.identity);
                newCard.name = ValueNames[(int)player._deck[player._deck.Count - 1].Value] + ((int)player._deck[player._deck.Count - 1].Suit).ToString();
                newCard.GetComponent<Selectable>().faceUp = true;
                if (player.getCount() > 21)
                {
                    playing = false;
                    printText("Busted!");
                    busted = true;
                }
                if (player.getCount() == 21)
                {
                    playing = false;
                }
            }
            else
            {
                playing = false;
                print("Can't hit");
            }
        }
    }
    public void stay(string s)
    {
        if (player._deck.Count > 1)
        {
            playing = false;
        }

    }
    public void setbet(string s)
    {
        if (bjPlay.getMoney() < 50)
        {
            print("No more money");
        }
        else if (!doneBet && bjPlay.getMoney()>=50)
        {
            bet += 50;
            print("Added $5 to bet");
            print("Bet = $" + bet);
            bjPlay.setMoney(bjPlay.getMoney() - 50);
        }

    }
    public void cancelbet(string s)
    {

        if (!doneBet)
        {
            bjPlay.setMoney(bjPlay.getMoney() + bet);
            bet = 0;

        }
   
    }
    public void doublebet(string s)
    {
        if (!doneBet && bjPlay.getMoney() >= bet *2)
        {
            bjPlay.setMoney(bjPlay.getMoney() - bet);
            bet *= 2;
      
            print("Bet = $" + bet);
        }
    }
    public void donebetting(string s)
    {
        print("donebetting");
        if (bet != 0)
        doneBet = true;
    }
};
