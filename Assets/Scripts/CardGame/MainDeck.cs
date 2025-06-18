using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainDeck : MonoBehaviour
{
    public static MainDeck instance = null;
    public Sprite CardBack => defaultBack;
    public Card DefaultCard => defaultCard;

    [SerializeField] Sprite defaultBack;
    [SerializeField] Card defaultCard;
    [SerializeField] CardStack mainStack;
    [SerializeField] List<Sprite> clubs;
    [SerializeField] List<Sprite> diams;
    [SerializeField] List<Sprite> spades;
    [SerializeField] List<Sprite> hearts;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {

    }

    public void MixCards(int alea)
    {
        Random.InitState(alea);

        List<Card.CardData> cards = new List<Card.CardData>(52);
        AddPack(clubs, ref cards);
        AddPack(hearts, ref cards);
        AddPack(diams, ref cards);
        AddPack(spades, ref cards);

        while (cards.Count > 0)
        {
            int rng = Random.Range(0, cards.Count);
            mainStack.Add(cards[rng]);
            cards.RemoveAt(rng);
        }

        mainStack.Evaluate();
    }

    private void AddPack(in List<Sprite> pack, ref List<Card.CardData> deck)
    {
        for (int i = 0; i < pack.Count; i++)
        {
            Card.CardData infos = new Card.CardData() { sprite = pack[i], value = i + 1 };
            deck.Add(infos);
        }
    }
}
