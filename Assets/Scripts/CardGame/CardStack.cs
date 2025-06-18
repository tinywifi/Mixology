using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class CardStack : MonoBehaviour
{
    [SerializeField] int maxCards;
    [SerializeField] int stepCards;
    [SerializeField] bool revealTop;

    public bool IsFull { get => maxCards == cards.Count; }
    public int Count => cards.Count;
    public Card.CardData TopCard => cards.Peek();
    public Vector2 TopPos => CalculateDelta(cards.Count);
    public Vector3 TopPos3D => CalculateDelta3D(cards.Count);


    Stack<Card.CardData> cards;

    
    void Awake()
    {
        

        stepCards = (stepCards <= 0) ? 1 : stepCards; 

        Clear();
    }

    
    void Update()
    {
        
    }

    public void Evaluate(bool forceBack = false)
    {
        int actualStack = Mathf.Max(transform.childCount, 0);
        int futureStack = Mathf.CeilToInt(cards.Count / (float)stepCards);
        bool isNew = false;

        if (transform.childCount > 0 && cards.Count == 0)
        {
            foreach(Transform tsf in transform)
            {
                Destroy(tsf.gameObject);
            }
        }
        else if (actualStack < futureStack)
        {
            for (int i = 0; i < (futureStack - actualStack); i++)
            {
                Card card = Instantiate<Card>(MainDeck.instance.DefaultCard, transform, false);
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (actualStack + i) * 5f);
                card.Setup(default, true);
            }

            isNew = true;
        }
        else if (actualStack > futureStack)
        {
            for (int i = 0; i < (actualStack-futureStack); i++)
            {
                if (transform.childCount - 2 >= 0)
                    Destroy(transform.GetChild(transform.childCount-2).gameObject);
            }
        }

        if (cards.Count > 0)
        {
            if (isNew)
                transform.GetChild(transform.childCount - 1).GetComponent<Card>().Setup(cards.Peek(), forceBack || !revealTop);
            else
            {
                Card card = transform.GetChild(transform.childCount - 1).GetComponent<Card>();
                card.SetCard(TopCard);
                if (forceBack || !revealTop)
                    card.ShowBack();
                else
                    card.ShowFront();
            }
        }
        

    }

    public Vector2 CalculateDelta(int number)
    {
        int futureStack = Mathf.CeilToInt(number / (float)stepCards);
        return new Vector2(0, futureStack * 5f);
    }

    public Vector3 CalculateDelta3D(int number)
    {        
        return transform.position;
    }

    public void Add(Card.CardData card)
    {
        cards.Push(card);
    }

    public void ServeTo(CardStack newStack, bool forceBack = false, Action callback = null)
    {
        Deselect();
        Card.CardData card = cards.Pop();
        newStack.Add(card);

        RectTransform cardGO = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
        cardGO.anchoredPosition = CalculateDelta(cards.Count + 1);
        cardGO.eulerAngles = Vector3.zero;
        cardGO.DOMove(newStack.TopPos3D, 0.24f).OnComplete(() => {
            
            ResetCard(cardGO);
            newStack.Evaluate(forceBack);
            Evaluate(forceBack);
            callback?.Invoke();
        });
        cardGO.DORotate(Vector3.forward * (Random.value > 0.57f ? 180 : -180), 0.2f);
    }

    public void ResetCard(RectTransform tsf)
    {
        int actualStack = Mathf.Max(transform.childCount - 1, 0);
        tsf.anchoredPosition = new Vector2(0, actualStack * 5f);
    }

    public void Selection()
    {
        if (transform.childCount == 0)
            return;

        Image cardGO = transform.GetChild(transform.childCount - 1).GetComponent<Image>();
        cardGO.color = Color.green;
    }

    public void Deselect()
    {
        if (transform.childCount == 0)
            return;

        Image cardGO = transform.GetChild(transform.childCount - 1).GetComponent<Image>();
        cardGO.color = Color.white;
    }

    public void Clear()
    {
        if (cards == null)
            cards = new Stack<Card.CardData>();
        else
            cards.Clear();

        foreach (Transform tsf in transform)
        {
            Destroy(tsf.gameObject);
        }
    }
}
