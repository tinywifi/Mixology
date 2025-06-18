using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocal : MonoBehaviour
{
    public bool IsLocal { get; set; }
    public bool IsOpponent { get; set; }
    public bool IsReady { get; set; }
    public bool IsLocked { get; set; }
    public bool CanPlay { get; private set; }
    public int Count => CountStacks();
    
    [SerializeField] List<CardStack> stacks;
    [SerializeField] CardStack deck;
    [SerializeField] List<CardStack> slots; 
    [SerializeField] PlayerLocal opponent;
    [SerializeField] bool keyboardControls = false;

    CardStack selection;
    int selectionId;
    NetPlayer network;

    private void Awake()
    {
        IsLocal = false;
        IsOpponent = false;
        IsLocked = false;
    }

    
    void Start()
    {
        network = GetComponent<NetPlayer>();

        keyboardControls = keyboardControls && !network.IsConnected;
    }

    
    void Update()
    {
        if (!keyboardControls)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ClickSlot(0);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ClickSlot(1);

        if (Input.GetKeyDown(KeyCode.A))
            OnClick(4);
        else if (Input.GetKeyDown(KeyCode.Z))
            OnClick(3);
        else if(Input.GetKeyDown(KeyCode.E))
            OnClick(2);
        else if (Input.GetKeyDown(KeyCode.R))
            OnClick(1);
        else if(Input.GetKeyDown(KeyCode.T))
            OnClick(0);

        if (Input.GetKeyDown(KeyCode.Space))
            ClickDeck();
    }

    public void OnClick(int stack)
    {
        if (IsLocked)
            return;

        if (selection == null)
        {
            selection = stacks[stack];
            selectionId = stack;

            if (IsLocal)
                selection.Selection();
        }
        else
        {
            CardStack goal = stacks[stack];

            if (goal.Count == 0 && goal != selection)
            {
                selection.ServeTo(goal);

                if (IsLocal && network.IsConnected)
                {
                    network.RegisterStack(selectionId, stack);
                }

                PlayerCanPlay();
            }

            selection.Deselect();
            selection = null;
        }
    }

    public void ClickDeck()
    {
        if (deck.Count == 0 || !IsLocked)
            return;

        
        IsReady = true;
        network.SendPlayerReady(true);
        deck.Deselect();

        if (selection != null)
        {
            selection.Deselect();
            selection = null;
        }
    }

    public void SetReady()
    {
        IsReady = true;
        deck.Deselect();
        Debug.Log("Deck " + deck + " ready");
    }

    public void ClickSlot(int slot)
    {
        if (selection == null || IsLocked)
            return;

        CardStack goal = slots[slot];

        if (goal.TopCard - selection.TopCard == 1)
        {
            selection.ServeTo(goal, false, PlayerCanPlay);

            if (IsLocal && network.IsConnected)
            {
                Debug.Log("Click slot");
                network.RegisterSlot(selectionId, slot);
            }

            PlayerCanPlay();
            opponent.PlayerCanPlay();
        }

        selection.Deselect();
        selection = null;

    }
    
    public void PlayerCanPlay()
    {
        CanPlay = false;
        int stacked = 0;
        foreach (CardStack stack in stacks)
        {
            if (stack.Count > 1)
                stacked++;
        }

        foreach (CardStack stack in stacks)
        {
            if (stack.Count == 0 && stacked > 0)
            {
                CanPlay = true;
                break;
            }
            else if (stack.Count > 0)
            {
                foreach (CardStack slot in slots)
                {
                    if (slot.TopCard - stack.TopCard == 1)
                    {
                        Debug.Log(name+" " + stack.name + " => " + slot.name+" / "+stack.TopCard.sprite.name+" => "+slot.TopCard.sprite.name);
                        CanPlay = true;
                        break;
                    }
                }

                if (CanPlay)
                    break;
            }
        }
    }

    private int CountStacks()
    {
        int count = 0;
        foreach (CardStack stack in stacks)
        {
            count += stack.Count;
        }
        return count;
    }
}
