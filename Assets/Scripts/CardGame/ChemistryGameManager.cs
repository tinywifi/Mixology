using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChemistryGameManager : MonoBehaviour
{
    [Header("Game Database")]
    [SerializeField] private List<ElementData> allElements = new List<ElementData>();
    [SerializeField] private List<CompoundData> allCompounds = new List<CompoundData>();
    [SerializeField] private List<ReactionData> allReactions = new List<ReactionData>();
    
    [Header("UI References")]
    [SerializeField] private Transform playerHandParent;
    [SerializeField] private Transform opponentHandParent;
    [SerializeField] private Transform playerCompoundsParent;
    [SerializeField] private Transform opponentCompoundsParent;
    [SerializeField] private GameObject elementCardPrefab;
    [SerializeField] private GameObject compoundCardPrefab;
    [SerializeField] private Button createCompoundButton;
    [SerializeField] private Button performReactionButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Text gameStatusText;
    [SerializeField] private Text playerElementCountText;
    [SerializeField] private Text opponentElementCountText;
    [SerializeField] private Text playerCompoundCountText;
    [SerializeField] private Text opponentCompoundCountText;
    
    [Header("Game State")]
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
    
    
    private NetGameManager networkManager;
    
    void Awake()
    {
        networkManager = GetComponent<NetGameManager>();
    }
    
    void Start()
    {
        InitializeGame();
        SetupUI();
    }
    
    private void InitializeGame()
    {
        
        for (int i = 0; i < 10; i++)
        {
            DealElementToPlayer(true);
            DealElementToPlayer(false);
        }
        
        UpdateUI();
        UpdateGameStatus("Game Started! Create compounds and reactions to win!");
    }
    
    private void SetupUI()
    {
        createCompoundButton.onClick.AddListener(TryCreateCompound);
        performReactionButton.onClick.AddListener(TryPerformReaction);
        endTurnButton.onClick.AddListener(EndTurn);
        
        
        createCompoundButton.interactable = false;
        performReactionButton.interactable = false;
    }
    
    private void DealElementToPlayer(bool isPlayer)
    {
        var targetHand = isPlayer ? playerHand : opponentHand;
        
        if (targetHand.Count >= handLimit) return;
        
        
        ElementData randomElement = allElements[Random.Range(0, allElements.Count)];
        targetHand.Add(randomElement);
    }
    
    private void UpdateUI()
    {
        UpdateHandDisplay();
        UpdateCompoundDisplay();
        UpdateCounterTexts();
        UpdateActionButtons();
    }
    
    private void UpdateHandDisplay()
    {
        
        foreach (Transform child in playerHandParent)
            Destroy(child.gameObject);
        foreach (Transform child in opponentHandParent)
            Destroy(child.gameObject);
        
        
        foreach (var element in playerHand)
        {
            GameObject cardObj = Instantiate(elementCardPrefab, playerHandParent);
            ElementCard card = cardObj.GetComponent<ElementCard>();
            card.Initialize(element);
            card.OnCardClicked += OnElementCardClicked;
        }
        
        
        for (int i = 0; i < opponentHand.Count; i++)
        {
            GameObject cardObj = Instantiate(elementCardPrefab, opponentHandParent);
            
        }
    }
    
    private void UpdateCompoundDisplay()
    {
        
        foreach (Transform child in playerCompoundsParent)
            Destroy(child.gameObject);
        foreach (Transform child in opponentCompoundsParent)
            Destroy(child.gameObject);
        
        
        foreach (var compound in playerCompounds)
        {
            GameObject cardObj = Instantiate(compoundCardPrefab, playerCompoundsParent);
            CompoundCard card = cardObj.GetComponent<CompoundCard>();
            card.Initialize(compound);
            card.OnCardClicked += OnCompoundCardClicked;
        }
        
        
        foreach (var compound in opponentCompounds)
        {
            GameObject cardObj = Instantiate(compoundCardPrefab, opponentCompoundsParent);
            CompoundCard card = cardObj.GetComponent<CompoundCard>();
            card.Initialize(compound);
            card.IsInteractable = false; 
        }
    }
    
    private void UpdateCounterTexts()
    {
        playerElementCountText.text = $"Elements: {playerHand.Count}/{handLimit}";
        opponentElementCountText.text = $"Elements: {opponentHand.Count}/{handLimit}";
        playerCompoundCountText.text = $"Compounds: {playerCompounds.Count}/{winConditionCompounds}";
        opponentCompoundCountText.text = $"Compounds: {opponentCompounds.Count}/{winConditionCompounds}";
    }
    
    private void UpdateActionButtons()
    {
        if (!isPlayerTurn)
        {
            createCompoundButton.interactable = false;
            performReactionButton.interactable = false;
            return;
        }
        
        
        createCompoundButton.interactable = CanCreateAnyCompound();
        
        
        performReactionButton.interactable = CanPerformAnyReaction() && 
                                           reactionsPerformedThisTurn < reactionLimit;
    }
    
    private bool CanCreateAnyCompound()
    {
        foreach (var compound in allCompounds)
        {
            if (compound.CanCreateFrom(playerHand))
                return true;
        }
        return false;
    }
    
    private bool CanPerformAnyReaction()
    {
        foreach (var reaction in allReactions)
        {
            if (reaction.CanPerformReaction(playerCompounds))
                return true;
        }
        return false;
    }
    
    private void OnElementCardClicked(ElementCard card)
    {
        if (!isPlayerTurn) return;
        
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
        if (!isPlayerTurn) return;
        
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
        if (!isPlayerTurn) return;
        
        List<ElementData> selectedElements = selectedElementCards.Select(c => c.Data).ToList();
        
        foreach (var compound in allCompounds)
        {
            if (compound.CanCreateFrom(selectedElements))
            {
                CreateCompound(compound, selectedElements);
                break;
            }
        }
    }
    
    private void CreateCompound(CompoundData compound, List<ElementData> usedElements)
    {
        
        foreach (var element in usedElements)
        {
            playerHand.Remove(element);
        }
        
        
        playerCompounds.Add(compound);
        
        
        ClearSelections();
        
        
        UpdateUI();
        
        
        CheckWinCondition();
        
        UpdateGameStatus($"Created {compound.formula}!");
    }
    
    private void TryPerformReaction()
    {
        if (!isPlayerTurn || reactionsPerformedThisTurn >= reactionLimit) return;
        
        List<CompoundData> selectedCompounds = selectedCompoundCards.Select(c => c.Data).ToList();
        
        foreach (var reaction in allReactions)
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
        
        UpdateGameStatus($"Performed {reaction.reactionName}!");
        
        
        if (reaction.forciblyEndTurn)
        {
            EndTurn();
        }
    }
    
    private void ApplyReactionEffects(ReactionData reaction)
    {
        switch (reaction.reactionType)
        {
            case ReactionType.AcidForming:
                
                break;
            case ReactionType.Blast:
                
                break;
            case ReactionType.Neutralisation:
                
                break;
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
        if (!isPlayerTurn) return;
        
        
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
        
        UpdateGameStatus("Opponent's turn");
        
        
        StartCoroutine(OpponentTurn());
    }
    
    private void DealElementsAtTurnStart(bool isPlayer)
    {
        var hand = isPlayer ? playerHand : opponentHand;
        int elementsToAdd = 0;
        
        if (hand.Count >= 5)
            elementsToAdd = 2;
        else if (hand.Count <= 5 && hand.Count > 0)
            elementsToAdd = 4;
        else if (hand.Count == 0)
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
            foreach (var compound in allCompounds)
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
                    
                    UpdateGameStatus($"Opponent created {compound.formula}!");
                    UpdateUI();
                    
                    yield return new WaitForSeconds(1f);
                    break;
                }
            }
        }
        
        
        DealElementsAtTurnStart(false);
        
        
        CheckWinCondition();
        
        
        isPlayerTurn = true;
        UpdateUI();
        UpdateGameStatus("Your turn");
    }
    
    private void CheckWinCondition()
    {
        if (gameEnded) return;
        
        if (playerCompounds.Count >= winConditionCompounds)
        {
            gameEnded = true;
            UpdateGameStatus("You Win! You collected 8 unique compounds!");
        }
        else if (opponentCompounds.Count >= winConditionCompounds)
        {
            gameEnded = true;
            UpdateGameStatus("Opponent Wins! They collected 8 unique compounds!");
        }
    }
    
    private void UpdateGameStatus(string message)
    {
        gameStatusText.text = message;
    }
}