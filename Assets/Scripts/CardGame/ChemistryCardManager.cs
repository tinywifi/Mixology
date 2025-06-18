using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChemistryCardManager : MonoBehaviour
{
    [Header("Original Game References")]
    [SerializeField] CardStack mainStack;
    [SerializeField] CardStack playerStack;
    [SerializeField] CardStack opponentStack;
    [SerializeField] PlayerLocal player1;
    [SerializeField] PlayerLocal player2;
    [SerializeField] CardStack leftStack;
    [SerializeField] CardStack rightStack;
    [SerializeField] RectTransform playerBoard;
    [SerializeField] RectTransform opponentBoard;
    [SerializeField] Text infos;
    [SerializeField] Button leaveBtn;
    
    [Header("Chemistry Game Setup")]
    [SerializeField] private ChemistryDatabase chemistryDatabase;
    [SerializeField] private GameObject elementCardPrefab;
    [SerializeField] private GameObject compoundCardPrefab;
    [SerializeField] private CompoundCreationHelper compoundCreationHelper;
    
    [Header("Chemistry UI")]
    [SerializeField] private Transform playerHandParent;
    [SerializeField] private Transform opponentHandParent;
    [SerializeField] private Transform playerCompoundsParent;
    [SerializeField] private Transform opponentCompoundsParent;
    [SerializeField] private Button createCompoundButton;
    [SerializeField] private Button performReactionButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Text playerElementCountText;
    [SerializeField] private Text opponentElementCountText;
    [SerializeField] private Text playerCompoundCountText;
    [SerializeField] private Text opponentCompoundCountText;
    [SerializeField] private Text turnIndicatorText;
    
    [Header("Game Settings")]
    [SerializeField] private int handLimit = 10;
    [SerializeField] private int winConditionCompounds = 8;
    
    
    private List<ElementData> playerHand = new List<ElementData>();
    private List<ElementData> opponentHand = new List<ElementData>();
    private List<CompoundData> playerCompounds = new List<CompoundData>();
    private List<CompoundData> opponentCompounds = new List<CompoundData>();
    private List<ElementCard> selectedElementCards = new List<ElementCard>();
    private List<CompoundCard> selectedCompoundCards = new List<CompoundCard>();
    
    private bool isPlayerTurn = true;
    private bool gameEnded = false;
    private int reactionsPerformedThisTurn = 0;
    private int reactionLimit = int.MaxValue;
    
    
    private NetGameManager network;
    private bool pauseGame;
    
    void Awake()
    {
        network = GetComponent<NetGameManager>();
    }
    
    IEnumerator Start()
    {
        
        if (mainStack) mainStack.gameObject.SetActive(false);
        if (playerStack) playerStack.gameObject.SetActive(false);
        if (opponentStack) opponentStack.gameObject.SetActive(false);
        if (leftStack) leftStack.gameObject.SetActive(false);
        if (rightStack) rightStack.gameObject.SetActive(false);
        if (playerBoard) playerBoard.gameObject.SetActive(false);
        if (opponentBoard) opponentBoard.gameObject.SetActive(false);
        
        
        SetupChemistryUI();
        
        pauseGame = true;
        if (infos) infos.text = "Initializing Chemistry Game...";
        if (leaveBtn) leaveBtn.gameObject.SetActive(false);
        
        
        if (chemistryDatabase == null)
        {
            if (infos) infos.text = "Error: Chemistry Database not assigned!";
            Debug.LogError("ChemistryCardManager: Chemistry Database not assigned in inspector!");
            yield break;
        }
        
        
        if (network != null)
        {
            yield return new WaitUntil(() => network.Timer > 0);
            yield return new WaitForSeconds(network.Timer);
        }
        else
        {
            
            yield return new WaitForSeconds(1f);
        }
        
        
        yield return InitializeChemistryGame();
    }
    
    private void SetupChemistryUI()
    {
        
        if (createCompoundButton)
        {
            createCompoundButton.onClick.AddListener(TryCreateCompound);
            createCompoundButton.interactable = false;
        }
        
        if (performReactionButton)
        {
            performReactionButton.onClick.AddListener(TryPerformReaction);
            performReactionButton.interactable = false;
        }
        
        if (endTurnButton)
        {
            endTurnButton.onClick.AddListener(EndTurn);
        }
        
        
        if (compoundCreationHelper)
        {
            compoundCreationHelper.OnCompoundConfirmed += OnCompoundCreated;
        }
    }
    
    private IEnumerator InitializeChemistryGame()
    {
        infos.text = "Dealing starting elements...";
        
        
        for (int i = 0; i < 10; i++)
        {
            DealElementToPlayer(true);
            DealElementToPlayer(false);
            yield return new WaitForSeconds(0.1f);
        }
        
        UpdateUI();
        
        infos.text = "Chemistry Game Started! Create compounds to win!";
        pauseGame = false;
        
        yield return new WaitForSeconds(2f);
        
        StartPlayerTurn();
    }
    
    private void DealElementToPlayer(bool isPlayer)
    {
        var targetHand = isPlayer ? playerHand : opponentHand;
        
        if (targetHand.Count >= handLimit || chemistryDatabase == null || chemistryDatabase.allElements.Count == 0) 
            return;
        
        
        ElementData randomElement = chemistryDatabase.allElements[UnityEngine.Random.Range(0, chemistryDatabase.allElements.Count)];
        targetHand.Add(randomElement);
    }
    
    private void UpdateUI()
    {
        UpdateHandDisplay();
        UpdateCompoundDisplay();
        UpdateCounterTexts();
        UpdateActionButtons();
        UpdateTurnIndicator();
    }
    
    private void UpdateHandDisplay()
    {
        
        if (playerHandParent)
        {
            foreach (Transform child in playerHandParent)
                Destroy(child.gameObject);
        }
        
        if (opponentHandParent)
        {
            foreach (Transform child in opponentHandParent)
                Destroy(child.gameObject);
        }
        
        
        if (elementCardPrefab && playerHandParent)
        {
            foreach (var element in playerHand)
            {
                GameObject cardObj = Instantiate(elementCardPrefab, playerHandParent);
                ElementCard card = cardObj.GetComponent<ElementCard>();
                if (card != null)
                {
                    card.Initialize(element);
                    card.OnCardClicked += OnElementCardClicked;
                }
            }
        }
        
        
        if (elementCardPrefab && opponentHandParent)
        {
            for (int i = 0; i < opponentHand.Count; i++)
            {
                GameObject cardObj = Instantiate(elementCardPrefab, opponentHandParent);
                
            }
        }
    }
    
    private void UpdateCompoundDisplay()
    {
        
        if (playerCompoundsParent)
        {
            foreach (Transform child in playerCompoundsParent)
                Destroy(child.gameObject);
        }
        
        if (opponentCompoundsParent)
        {
            foreach (Transform child in opponentCompoundsParent)
                Destroy(child.gameObject);
        }
        
        
        if (compoundCardPrefab && playerCompoundsParent)
        {
            foreach (var compound in playerCompounds)
            {
                GameObject cardObj = Instantiate(compoundCardPrefab, playerCompoundsParent);
                CompoundCard card = cardObj.GetComponent<CompoundCard>();
                if (card != null)
                {
                    card.Initialize(compound);
                    card.OnCardClicked += OnCompoundCardClicked;
                }
            }
        }
        
        
        if (compoundCardPrefab && opponentCompoundsParent)
        {
            foreach (var compound in opponentCompounds)
            {
                GameObject cardObj = Instantiate(compoundCardPrefab, opponentCompoundsParent);
                CompoundCard card = cardObj.GetComponent<CompoundCard>();
                if (card != null)
                {
                    card.Initialize(compound);
                    card.IsInteractable = false; 
                }
            }
        }
    }
    
    private void UpdateCounterTexts()
    {
        if (playerElementCountText)
            playerElementCountText.text = $"Elements: {playerHand.Count}/{handLimit}";
        if (opponentElementCountText)
            opponentElementCountText.text = $"Elements: {opponentHand.Count}/{handLimit}";
        if (playerCompoundCountText)
            playerCompoundCountText.text = $"Compounds: {playerCompounds.Count}/{winConditionCompounds}";
        if (opponentCompoundCountText)
            opponentCompoundCountText.text = $"Compounds: {opponentCompounds.Count}/{winConditionCompounds}";
    }
    
    private void UpdateActionButtons()
    {
        if (!isPlayerTurn || pauseGame)
        {
            if (createCompoundButton) createCompoundButton.interactable = false;
            if (performReactionButton) performReactionButton.interactable = false;
            return;
        }
        
        
        if (createCompoundButton)
            createCompoundButton.interactable = selectedElementCards.Count > 0 && CanCreateAnyCompound();
        
        
        if (performReactionButton)
            performReactionButton.interactable = selectedCompoundCards.Count > 0 && CanPerformAnyReaction() && 
                                               reactionsPerformedThisTurn < reactionLimit;
    }
    
    private void UpdateTurnIndicator()
    {
        if (turnIndicatorText)
        {
            turnIndicatorText.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
        }
    }
    
    private bool CanCreateAnyCompound()
    {
        if (chemistryDatabase == null || selectedElementCards.Count == 0) return false;
        
        List<ElementData> selectedElements = selectedElementCards.Select(c => c.Data).ToList();
        
        foreach (var compound in chemistryDatabase.allCompounds)
        {
            if (IsExactMatch(compound, selectedElements))
                return true;
        }
        return false;
    }
    
    private bool IsExactMatch(CompoundData compound, List<ElementData> elements)
    {
        
        Dictionary<ElementData, int> selectedCounts = new Dictionary<ElementData, int>();
        foreach (var element in elements)
        {
            if (selectedCounts.ContainsKey(element))
                selectedCounts[element]++;
            else
                selectedCounts[element] = 1;
        }
        
        
        Dictionary<ElementData, int> requiredCounts = new Dictionary<ElementData, int>();
        foreach (var requirement in compound.requiredElements)
        {
            requiredCounts[requirement.element] = requirement.quantity;
        }
        
        
        if (selectedCounts.Count != requiredCounts.Count)
            return false;
            
        foreach (var kvp in requiredCounts)
        {
            if (!selectedCounts.ContainsKey(kvp.Key) || selectedCounts[kvp.Key] != kvp.Value)
                return false;
        }
        
        return true;
    }
    
    private bool CanPerformAnyReaction()
    {
        if (chemistryDatabase == null || selectedCompoundCards.Count == 0) return false;
        
        List<CompoundData> selectedCompounds = selectedCompoundCards.Select(c => c.Data).ToList();
        
        foreach (var reaction in chemistryDatabase.allReactions)
        {
            if (reaction.CanPerformReaction(selectedCompounds))
                return true;
        }
        return false;
    }
    
    private void OnElementCardClicked(ElementCard card)
    {
        if (!isPlayerTurn || pauseGame) return;
        
        if (card.IsSelected)
        {
            if (!selectedElementCards.Contains(card))
                selectedElementCards.Add(card);
        }
        else
        {
            selectedElementCards.Remove(card);
        }
        
        UpdateActionButtons();
    }
    
    private void OnCompoundCardClicked(CompoundCard card)
    {
        if (!isPlayerTurn || pauseGame) return;
        
        if (card.IsSelected)
        {
            if (!selectedCompoundCards.Contains(card))
                selectedCompoundCards.Add(card);
        }
        else
        {
            selectedCompoundCards.Remove(card);
        }
        
        UpdateActionButtons();
    }
    
    private void TryCreateCompound()
    {
        if (!isPlayerTurn || pauseGame || selectedElementCards.Count == 0) return;
        
        List<ElementData> selectedElements = selectedElementCards.Select(c => c.Data).ToList();
        
        if (compoundCreationHelper)
        {
            compoundCreationHelper.ShowCompoundCreation(selectedElements);
        }
    }
    
    private void OnCompoundCreated(CompoundData compound, List<ElementData> usedElements)
    {
        
        foreach (var element in usedElements)
        {
            playerHand.Remove(element);
        }
        
        
        playerCompounds.Add(compound);
        
        
        ClearSelections();
        
        
        UpdateUI();
        
        
        CheckWinCondition();
        
        infos.text = $"Created {compound.formula}!";
    }
    
    private void TryPerformReaction()
    {
        if (!isPlayerTurn || pauseGame || reactionsPerformedThisTurn >= reactionLimit) return;
        
        List<CompoundData> selectedCompounds = selectedCompoundCards.Select(c => c.Data).ToList();
        
        foreach (var reaction in chemistryDatabase.allReactions)
        {
            if (reaction.CanPerformReaction(selectedCompounds))
            {
                PerformReaction(reaction, selectedCompounds);
                break;
            }
        }
    }
    
    private void PerformReaction(ReactionData reaction, List<CompoundData> usedCompounds)
    {
        
        if (reaction.banishReactants)
        {
            foreach (var compound in usedCompounds)
            {
                playerCompounds.Remove(compound);
            }
        }
        
        
        foreach (var product in reaction.producedCompounds)
        {
            playerCompounds.Add(product);
        }
        
        
        foreach (var element in reaction.producedElements)
        {
            if (playerHand.Count < handLimit)
                playerHand.Add(element);
        }
        
        
        ApplyReactionEffects(reaction);
        
        
        reactionsPerformedThisTurn++;
        
        
        ClearSelections();
        
        
        UpdateUI();
        
        
        CheckWinCondition();
        
        infos.text = $"Performed {reaction.reactionName}!";
        
        
        if (reaction.forciblyEndTurn)
        {
            EndTurn();
        }
    }
    
    private void ApplyReactionEffects(ReactionData reaction)
    {
        switch (reaction.reactionType)
        {
            case ReactionType.Corrosion:
                reactionLimit = 5;
                break;
            case ReactionType.WaterGasShift:
                while (playerHand.Count < 10 && playerHand.Count < handLimit)
                {
                    DealElementToPlayer(true);
                }
                break;
        }
    }
    
    private void ClearSelections()
    {
        foreach (var card in selectedElementCards)
        {
            card.SetSelected(false);
        }
        foreach (var card in selectedCompoundCards)
        {
            card.SetSelected(false);
        }
        
        selectedElementCards.Clear();
        selectedCompoundCards.Clear();
    }
    
    private void EndTurn()
    {
        if (!isPlayerTurn || pauseGame) return;
        
        
        reactionsPerformedThisTurn = 0;
        reactionLimit = int.MaxValue;
        
        
        foreach (Transform child in playerCompoundsParent)
        {
            var compound = child.GetComponent<CompoundCard>();
            if (compound != null)
                compound.ResetTurnUsage();
        }
        
        
        DealElementsAtTurnStart(true);
        
        
        isPlayerTurn = false;
        
        
        ClearSelections();
        
        
        UpdateUI();
        
        infos.text = "Opponent's turn";
        
        
        StartCoroutine(OpponentTurn());
    }
    
    private void DealElementsAtTurnStart(bool isPlayer)
    {
        var hand = isPlayer ? playerHand : opponentHand;
        int elementsToAdd = 0;
        
        if (hand.Count >= 5)
            elementsToAdd = 2;
        else if (hand.Count > 0)
            elementsToAdd = 4;
        else
            elementsToAdd = 5;
        
        for (int i = 0; i < elementsToAdd && hand.Count < handLimit; i++)
        {
            DealElementToPlayer(isPlayer);
        }
    }
    
    private IEnumerator OpponentTurn()
    {
        yield return new WaitForSeconds(2f);
        
        
        for (int attempts = 0; attempts < 3; attempts++)
        {
            if (chemistryDatabase != null)
            {
                foreach (var compound in chemistryDatabase.allCompounds)
                {
                    if (compound.CanCreateFrom(opponentHand))
                    {
                        
                        foreach (var requirement in compound.requiredElements)
                        {
                            for (int i = 0; i < requirement.quantity; i++)
                            {
                                opponentHand.Remove(requirement.element);
                            }
                        }
                        
                        
                        opponentCompounds.Add(compound);
                        
                        infos.text = $"Opponent created {compound.formula}!";
                        UpdateUI();
                        
                        yield return new WaitForSeconds(1f);
                        break;
                    }
                }
            }
        }
        
        
        DealElementsAtTurnStart(false);
        
        
        CheckWinCondition();
        
        
        StartPlayerTurn();
    }
    
    private void StartPlayerTurn()
    {
        isPlayerTurn = true;
        UpdateUI();
        infos.text = "Your turn";
    }
    
    private void CheckWinCondition()
    {
        if (gameEnded) return;
        
        if (playerCompounds.Count >= winConditionCompounds)
        {
            gameEnded = true;
            pauseGame = true;
            infos.text = "You Win! You collected 8 unique compounds!";
            leaveBtn.gameObject.SetActive(true);
        }
        else if (opponentCompounds.Count >= winConditionCompounds)
        {
            gameEnded = true;
            pauseGame = true;
            infos.text = "Opponent Wins! They collected 8 unique compounds!";
            leaveBtn.gameObject.SetActive(true);
        }
    }
    
    void Update()
    {
        if (pauseGame || gameEnded)
            return;
        
        
        if (!isPlayerTurn)
            return;
    }
}