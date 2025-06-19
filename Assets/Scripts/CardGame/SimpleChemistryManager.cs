using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SimpleChemistryManager : MonoBehaviourPun, IOnEventCallback
{
    [Header("Chemistry Database")]
    [SerializeField] private ChemistryDatabase chemistryDatabase;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject elementCardPrefab;
    [SerializeField] private GameObject compoundCardPrefab;
    
    [Header("UI References")]
    [SerializeField] private Transform playerHandParent;
    [SerializeField] private Transform playerCompoundsParent;
    [SerializeField] private Transform opponentCompoundsParent;
    [SerializeField] private Button createCompoundButton;
    [SerializeField] private Button discardButton;
    [SerializeField] private Button activateEffectButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button cheatModeButton;
    [SerializeField] private GameObject cheatModeDisplay;
    [SerializeField] private Text statusText;
    [SerializeField] private Text elementCountText;
    [SerializeField] private Text compoundCountText;
    [SerializeField] private Text opponentInfoText;
    
    
    private List<ElementData> playerHand = new List<ElementData>();
    private List<CompoundData> playerCompounds = new List<CompoundData>();
    private List<ElementData> opponentHand = new List<ElementData>();
    private List<CompoundData> opponentCompounds = new List<CompoundData>();
    private List<ElementCard> selectedElementCards = new List<ElementCard>();
    private List<CompoundCard> selectedCompoundCards = new List<CompoundCard>();
    private int handLimit = 10;
    private int winCondition = 8;
    private bool isPlayerTurn = true;
    private bool cheatModeEnabled = false;
    private bool hasDiscardedThisTurn = false;
    
    
    private bool waitingForTurnAck = false;
    private float turnSwitchTimeout = 10f;
    private Coroutine turnTimeoutCoroutine;
    
    void Start()
    {
        
        Invoke("StartChemistryGame", 0.5f);
    }
    
    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    
    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    
    public void OnEvent(EventData photonEvent)
    {
        
    }
    
    public void StartChemistryGame()
    {
        
        HideOldGameUI();
        
        if (chemistryDatabase == null)
        {
            Debug.LogError("Chemistry Database not assigned!");
            if (statusText) statusText.text = "Error: No Chemistry Database!";
            return;
        }
        
        if (chemistryDatabase.allElements.Count == 0)
        {
            Debug.LogError("Chemistry Database is empty! Run the setup first.");
            if (statusText) statusText.text = "Error: Database is empty!";
            return;
        }
        
        
        SetupUI();
        
        StartCoroutine(DelayedButtonSetup());
        
        
        DealStartingHand();
        
        
        InitializeTurnSystem();
        
        
        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(DelayedNetworkSync());
        }
        
        
        UpdateUI();
    }
    
    private void InitializeTurnSystem()
    {
        if (PhotonNetwork.IsConnected)
        {
            
            if (PhotonNetwork.IsMasterClient)
            {
                
                bool masterStarts = Random.Range(0, 2) == 0;
                isPlayerTurn = masterStarts;
                
                
                if (photonView != null)
                {
                    try
                    {
                        photonView.RPC("SetInitialTurn", RpcTarget.Others, !masterStarts);
                        Debug.Log("Initial turn RPC sent successfully");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to send initial turn RPC: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError("PhotonView is null - cannot send initial turn RPC");
                }
                
                Debug.Log($"Master client decided: {(masterStarts ? "Master" : "Client")} starts first");
            }
            
        }
        else
        {
            
            isPlayerTurn = true;
        }
    }
    
    private IEnumerator DelayedNetworkSync()
    {
        
        yield return new WaitForSeconds(1f);
        
        
        if (PhotonNetwork.IsConnected && photonView != null && photonView.IsMine)
        {
            try
            {
                string[] compoundFormulas = playerCompounds.Select(c => c.formula).ToArray();
                photonView.RPC("SyncPlayerData", RpcTarget.Others, playerHand.Count, compoundFormulas);
                Debug.Log("Initial network sync completed");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to sync initial network state: {e.Message}");
            }
        }
    }
    
    private void SetupButtons()
    {
        if (createCompoundButton)
        {
            createCompoundButton.onClick.RemoveAllListeners();
            createCompoundButton.onClick.AddListener(TryCreateCompound);
        }
        
        if (discardButton)
        {
            discardButton.onClick.RemoveAllListeners();
            discardButton.onClick.AddListener(TryDiscardElements);
            Debug.Log("✓ Discard button found and connected to TryDiscardElements");
        }
        
        if (activateEffectButton)
        {
            activateEffectButton.onClick.RemoveAllListeners();
            activateEffectButton.onClick.AddListener(TryActivateEffect);
            Debug.Log("✓ Activate Effect button found and connected to TryActivateEffect");
        }
        
        if (endTurnButton)
        {
            endTurnButton.onClick.RemoveAllListeners();
            endTurnButton.onClick.AddListener(EndTurn);
        }
        
        if (cheatModeButton)
        {
            cheatModeButton.onClick.RemoveAllListeners();
            cheatModeButton.onClick.AddListener(ToggleCheatMode);
        }
    }
    
    private void DealStartingHand()
    {
        
        for (int i = 0; i < 8; i++)
        {
            AddRandomElementToHand(true);  
            AddRandomElementToHand(false); 
        }
    }
    
    private void UpdateUI()
    {
        UpdateHandDisplay();
        UpdateCompoundDisplay();
        UpdateCounters();
        UpdateButtonStates();
        UpdateTurnIndicator();
    }
    
    private void SetupUI()
    {
        Debug.Log("Setting up UI...");
        
        QuickChemistryUISetup uiSetup = FindObjectOfType<QuickChemistryUISetup>();
        if (uiSetup == null)
        {
            Debug.Log("Creating QuickChemistryUISetup...");
            GameObject setupObj = new GameObject("ChemistryUISetup");
            uiSetup = setupObj.AddComponent<QuickChemistryUISetup>();
        }
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            var field = typeof(QuickChemistryUISetup).GetField("targetCanvas", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(uiSetup, canvas);
                Debug.Log("Canvas assigned to UI setup");
            }
        }
        
        Debug.Log("UI setup completed");
        
        UpdateCompoundConfigurations();
    }
    
    private void UpdateCompoundConfigurations()
    {
        Debug.Log("Updating compound configurations...");
        
        if (chemistryDatabase != null)
        {
            foreach (var compound in chemistryDatabase.allCompounds)
            {
                if (compound.formula == "Metallic")
                {
                    compound.isMetallicCompound = true;
                    compound.requiredElements.Clear();
                    Debug.Log("✓ Updated Metallic compound configuration");
                }
                else if (compound.formula.Contains("Metal Hydride") || compound.compoundName.Contains("Metal Hydride"))
                {
                    compound.isMetalHydrideCompound = true;
                    Debug.Log("✓ Updated Metal Hydride compound configuration");
                }
                else if (compound.formula.Contains("CH4") || compound.compoundName.Contains("Hydrocarbon"))
                {
                    compound.isHydrocarbonCompound = true;
                    Debug.Log("✓ Updated Hydrocarbon compound configuration");
                }
                else if (compound.formula.Contains("Network") || compound.compoundName.Contains("Network"))
                {
                    compound.isNetworkSolidCompound = true;
                    Debug.Log("✓ Updated Network Solid compound configuration");
                }
                else if (compound.formula.Contains("Metal Oxide") || compound.compoundName.Contains("Metal Oxide"))
                {
                    compound.isMetalOxideCompound = true;
                    Debug.Log("✓ Updated Metal Oxide compound configuration");
                }
            }
        }
    }
    
    private IEnumerator DelayedButtonSetup()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Running delayed button setup...");
        SetupButtons();
        yield return new WaitForSeconds(0.1f);
        AssignDynamicButtons();
    }
    
    private void AssignDynamicButtons()
    {
        Debug.Log("Assigning dynamic buttons...");
        AutoAssignMissingReferences();
    }
    
    private void UpdateHandDisplay()
    {
        if (playerHandParent == null) 
        {
            Debug.LogError("Player hand parent not assigned!");
            return;
        }
        
        if (elementCardPrefab == null) 
        {
            Debug.LogError("Element card prefab not assigned!");
            return;
        }
        
        Debug.Log($"Updating hand display with {playerHand.Count} elements");
        
        
        foreach (Transform child in playerHandParent)
        {
            Destroy(child.gameObject);
        }
        
        
        foreach (var element in playerHand)
        {
            GameObject cardObj = Instantiate(elementCardPrefab, playerHandParent);
            ElementCard card = cardObj.GetComponent<ElementCard>();
            if (card != null)
            {
                card.Initialize(element);
                card.OnCardClicked += OnElementCardClicked;
                Debug.Log($"Created card for {element.elementName}");
            }
            else
            {
                Debug.LogError($"ElementCard component not found on prefab for {element.elementName}");
            }
        }
    }
    
    private void UpdateCompoundDisplay()
    {
        
        if (playerCompoundsParent != null && compoundCardPrefab != null)
        {
            
            foreach (Transform child in playerCompoundsParent)
            {
                Destroy(child.gameObject);
            }
            
            
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
        
        
        if (opponentCompoundsParent != null && compoundCardPrefab != null)
        {
            
            foreach (Transform child in opponentCompoundsParent)
            {
                Destroy(child.gameObject);
            }
            
            
            foreach (var compound in opponentCompounds)
            {
                GameObject cardObj = Instantiate(compoundCardPrefab, opponentCompoundsParent);
                CompoundCard card = cardObj.GetComponent<CompoundCard>();
                if (card != null)
                {
                    card.Initialize(compound);
                }
            }
        }
    }
    
    private void UpdateCounters()
    {
        if (elementCountText)
            elementCountText.text = $"Elements: {playerHand.Count}/{handLimit}";
        if (compoundCountText)
            compoundCountText.text = $"Compounds: {playerCompounds.Count}/{winCondition}";
        if (opponentInfoText)
            opponentInfoText.text = $"Opponent: {opponentHand.Count} Elements | {opponentCompounds.Count}/{winCondition} Compounds";
    }
    
    private void UpdateButtonStates()
    {
        
        bool canInteract = isPlayerTurn;
        
        if (createCompoundButton)
            createCompoundButton.interactable = canInteract && selectedElementCards.Count > 0;
            
        if (discardButton)
        {
            bool discardEnabled = canInteract && !hasDiscardedThisTurn && selectedElementCards.Count > 0;
            discardButton.interactable = discardEnabled;
            Debug.Log($"Discard button state: canInteract={canInteract}, hasDiscardedThisTurn={hasDiscardedThisTurn}, selectedCount={selectedElementCards.Count}, enabled={discardEnabled}");
        }
        else
        {
            Debug.LogWarning("Discard button is null! Not assigned in inspector.");
        }
        
        Debug.Log("UpdateButtonStates: checking activate effect button...");
        if (activateEffectButton)
        {
            bool activateEnabled = canInteract && selectedCompoundCards.Count > 0 && HasActivatableCompounds();
            activateEffectButton.interactable = activateEnabled;
            Debug.Log($"Activate Effect button state: canInteract={canInteract}, selectedCompounds={selectedCompoundCards.Count}, hasActivatable={HasActivatableCompounds()}, enabled={activateEnabled}");
        }
        else
        {
            Debug.LogWarning("Activate Effect button is null! Not assigned in inspector.");
        }
            
        if (endTurnButton)
            endTurnButton.interactable = canInteract;
            
        if (cheatModeButton)
            cheatModeButton.interactable = canInteract;
            
        
        if (playerHandParent != null)
        {
            ElementCard[] cards = playerHandParent.GetComponentsInChildren<ElementCard>();
            foreach (var card in cards)
            {
                card.IsInteractable = canInteract;
            }
        }
    }
    
    private void UpdateTurnIndicator()
    {
        if (statusText)
        {
            if (isPlayerTurn)
            {
                statusText.text = "YOUR TURN - Select elements to create compounds";
                statusText.color = Color.green;
            }
            else
            {
                statusText.text = "OPPONENT'S TURN - Please wait...";
                statusText.color = Color.red;
            }
        }
    }
    
    private void OnElementCardClicked(ElementCard card)
    {
        if (card.IsSelected)
        {
            if (!selectedElementCards.Contains(card))
                selectedElementCards.Add(card);
        }
        else
        {
            selectedElementCards.Remove(card);
        }
        
        UpdateButtonStates();
    }
    
    private void OnCompoundCardClicked(CompoundCard card)
    {
        if (card.IsSelected)
        {
            if (!selectedCompoundCards.Contains(card))
                selectedCompoundCards.Add(card);
        }
        else
        {
            selectedCompoundCards.Remove(card);
        }
        
        UpdateButtonStates();
    }
    
    public void TryCreateCompound()
    {
        if (selectedElementCards.Count == 0) return;
        
        List<ElementData> selectedElements = selectedElementCards.Select(c => c.Data).ToList();
        
        
        CompoundData validCompound = FindValidCompound(selectedElements);
        
        if (validCompound != null)
        {
            CreateCompound(validCompound, selectedElements);
        }
        else
        {
            if (statusText) statusText.text = "Cannot create compound from selected elements. Check oxidation numbers!";
        }
    }
    
    private CompoundData FindValidCompound(List<ElementData> elements)
    {
        Debug.Log($"Searching for compound from {elements.Count} selected elements:");
        foreach (var element in elements)
        {
            Debug.Log($"- {element.symbol} (oxidation: {element.oxidationNumber})");
        }
        
        foreach (var compound in chemistryDatabase.allCompounds)
        {
            Debug.Log($"Checking compound {compound.formula}...");
            
            if (compound.isMetallicCompound)
            {
                Debug.Log($"Testing metallic compound: {compound.formula}");
                if (compound.CanCreateMetallicFrom(elements))
                {
                    Debug.Log($"✓ Found matching metallic compound: {compound.formula}");
                    return compound;
                }
            }
            else if (compound.isMetalHydrideCompound)
            {
                Debug.Log($"Testing metal hydride compound: {compound.formula}");
                if (compound.CanCreateMetalHydrideFrom(elements))
                {
                    Debug.Log($"✓ Found matching metal hydride compound: {compound.formula}");
                    return compound;
                }
            }
            else if (compound.isHydrocarbonCompound)
            {
                Debug.Log($"Testing hydrocarbon compound: {compound.formula}");
                if (compound.CanCreateHydrocarbonFrom(elements))
                {
                    Debug.Log($"✓ Found matching hydrocarbon compound: {compound.formula}");
                    return compound;
                }
            }
            else if (compound.isNetworkSolidCompound)
            {
                Debug.Log($"Testing network solid compound: {compound.formula}");
                if (compound.CanCreateNetworkSolidFrom(elements))
                {
                    Debug.Log($"✓ Found matching network solid compound: {compound.formula}");
                    return compound;
                }
            }
            else if (compound.isMetalOxideCompound)
            {
                Debug.Log($"Testing metal oxide compound: {compound.formula}");
                if (compound.CanCreateMetalOxideFrom(elements))
                {
                    Debug.Log($"✓ Found matching metal oxide compound: {compound.formula}");
                    return compound;
                }
            }
            else if (IsExactMatch(compound, elements))
            {
                Debug.Log($"✓ Found matching compound: {compound.formula}");
                return compound;
            }
        }
        
        Debug.Log("No matching compound found");
        return null;
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
    
    private void CreateCompound(CompoundData compound, List<ElementData> usedElements)
    {
        
        if (compound.isMetallicCompound)
        {
            List<ElementData> elementsToRemove = GetMetallicElementsToRemove(usedElements);
            foreach (var element in elementsToRemove)
            {
                playerHand.Remove(element);
            }
        }
        else if (compound.isMetalHydrideCompound || compound.isHydrocarbonCompound || 
                 compound.isNetworkSolidCompound || compound.isMetalOxideCompound)
        {
            List<ElementData> elementsToRemove = GetSpecialCompoundElementsToRemove(compound, usedElements);
            foreach (var element in elementsToRemove)
            {
                playerHand.Remove(element);
            }
        }
        else
        {
            
            foreach (var element in usedElements)
            {
                playerHand.Remove(element);
            }
        }
        
        
        playerCompounds.Add(compound);
        
        
        if (PhotonNetwork.IsConnected && photonView != null && photonView.IsMine)
        {
            
            string[] compoundFormulas = playerCompounds.Select(c => c.formula).ToArray();
            photonView.RPC("SyncPlayerData", RpcTarget.Others, playerHand.Count, compoundFormulas);
        }
        
        
        ClearSelection();
        
        
        UpdateUI();
        
        if (cheatModeEnabled)
        {
            AnalyzeHandCombinations();
        }
        
        
        if (playerCompounds.Count >= winCondition)
        {
            if (statusText) statusText.text = $"YOU WIN! You created {winCondition} compounds!";
        }
        else
        {
            if (statusText) statusText.text = $"Created {compound.formula}! ({playerCompounds.Count}/{winCondition} compounds)";
        }
    }
    
    private List<ElementData> GetMetallicElementsToRemove(List<ElementData> selectedElements)
    {
        
        Dictionary<int, List<ElementData>> metalElementsByCharge = new Dictionary<int, List<ElementData>>();
        
        foreach (var element in selectedElements)
        {
            
            if (element.oxidationNumber > 0 && element.IsMetal())
            {
                if (!metalElementsByCharge.ContainsKey(element.oxidationNumber))
                    metalElementsByCharge[element.oxidationNumber] = new List<ElementData>();
                metalElementsByCharge[element.oxidationNumber].Add(element);
            }
        }
        
        
        foreach (var chargeGroup in metalElementsByCharge)
        {
            if (chargeGroup.Value.Count >= 3)
            {
                
                return chargeGroup.Value.Take(3).ToList();
            }
        }
        
        
        return selectedElements.Take(3).ToList();
    }
    
    private List<ElementData> GetSpecialCompoundElementsToRemove(CompoundData compound, List<ElementData> selectedElements)
    {
        List<ElementData> elementsToRemove = new List<ElementData>();
        
        if (compound.isMetalHydrideCompound)
        {
            // Metal + Hydrogen
            ElementData metal = selectedElements.FirstOrDefault(e => e.IsMetal() && e.oxidationNumber > 0);
            ElementData hydrogen = selectedElements.FirstOrDefault(e => e.symbol == "H");
            if (metal != null) elementsToRemove.Add(metal);
            if (hydrogen != null) elementsToRemove.Add(hydrogen);
        }
        else if (compound.isHydrocarbonCompound)
        {
            // Carbon + 4 Hydrogens
            ElementData carbon = selectedElements.FirstOrDefault(e => e.symbol == "C");
            var hydrogens = selectedElements.Where(e => e.symbol == "H").Take(4);
            if (carbon != null) elementsToRemove.Add(carbon);
            elementsToRemove.AddRange(hydrogens);
        }
        else if (compound.isNetworkSolidCompound)
        {
            // 4 Carbons
            var carbons = selectedElements.Where(e => e.symbol == "C").Take(4);
            elementsToRemove.AddRange(carbons);
        }
        else if (compound.isMetalOxideCompound)
        {
            // Metal + Oxygen
            ElementData metal = selectedElements.FirstOrDefault(e => e.IsMetal() && e.oxidationNumber > 0);
            ElementData oxygen = selectedElements.FirstOrDefault(e => e.symbol == "O");
            if (metal != null) elementsToRemove.Add(metal);
            if (oxygen != null) elementsToRemove.Add(oxygen);
        }
        
        return elementsToRemove;
    }
    
    private void ClearSelection()
    {
        foreach (var card in selectedElementCards)
        {
            card.SetSelected(false);
        }
        selectedElementCards.Clear();
        
        if (createCompoundButton)
            createCompoundButton.interactable = false;
    }
    
    public void TryDiscardElements()
    {
        if (!isPlayerTurn || hasDiscardedThisTurn || selectedElementCards.Count == 0) return;
        
        Debug.Log($"Attempting to discard {selectedElementCards.Count} elements");
        
        int discardCount = selectedElementCards.Count;
        List<ElementData> selectedElements = selectedElementCards.Select(c => c.Data).ToList();
        
        
        foreach (var element in selectedElements)
        {
            playerHand.Remove(element);
        }
        
        
        for (int i = 0; i < discardCount; i++)
        {
            if (playerHand.Count < handLimit)
            {
                AddRandomElementToHand(true);
            }
        }
        
        
        hasDiscardedThisTurn = true;
        
        
        ClearSelection();
        
        
        UpdateUI();
        
        if (cheatModeEnabled)
        {
            AnalyzeHandCombinations();
        }
        
        if (statusText) statusText.text = $"Discarded {discardCount} elements and drew {discardCount} new ones!";
        
        Debug.Log($"Successfully discarded {discardCount} elements. Hand now has {playerHand.Count} elements.");
        
        
        EndTurn();
    }
    
    private bool HasActivatableCompounds()
    {
        Debug.Log($"HasActivatableCompounds: checking {selectedCompoundCards.Count} selected compounds");
        foreach (var compoundCard in selectedCompoundCards)
        {
            Debug.Log($"Compound {compoundCard.Data.formula} has effect: {compoundCard.Data.effect}");
            if (compoundCard.Data.effect != CompoundEffect.None)
            {
                Debug.Log("Found activatable compound!");
                return true;
            }
        }
        Debug.Log("No activatable compounds found");
        return false;
    }
    
    public void TryActivateEffect()
    {
        Debug.Log($"TryActivateEffect called! isPlayerTurn={isPlayerTurn}, selectedCompounds={selectedCompoundCards.Count}");
        
        if (!isPlayerTurn || selectedCompoundCards.Count == 0) 
        {
            Debug.Log("Early return: either not player turn or no compounds selected");
            return;
        }
        
        Debug.Log($"Attempting to activate effects on {selectedCompoundCards.Count} compounds");
        
        foreach (var compoundCard in selectedCompoundCards)
        {
            ActivateCompoundEffect(compoundCard.Data);
        }
        
        ClearCompoundSelection();
        UpdateUI();
    }
    
    private void ActivateCompoundEffect(CompoundData compound)
    {
        Debug.Log($"Activating effect for compound: {compound.formula} - Effect: {compound.effect}");
        
        switch (compound.effect)
        {
            case CompoundEffect.DrawElements:
                ActivateDrawElementsEffect(compound.effectValue);
                break;
            case CompoundEffect.SkipPlayerTurn:
                ActivateSkipPlayerTurnEffect();
                break;
            case CompoundEffect.DiscardElements:
                ActivateDiscardElementsEffect(compound.effectValue);
                break;
            case CompoundEffect.ReceiveElements:
                ActivateReceiveElementsEffect(compound.effectValue);
                break;
            case CompoundEffect.NegateDissolusion:
                ActivateNegateDissolusionEffect();
                break;
            case CompoundEffect.H2O_ChangeElements:
                ActivateH2OEffect();
                break;
            case CompoundEffect.CO2_DestroyCompound:
                ActivateCO2Effect();
                break;
            case CompoundEffect.Acid_SkipTurn:
                ActivateAcidEffect();
                break;
            case CompoundEffect.Base_DiscardThree:
                ActivateBaseEffect();
                break;
            case CompoundEffect.Salt_TakeElements:
                ActivateSaltEffect();
                break;
            case CompoundEffect.Metallic_NegateDisso:
                ActivateMetallicEffect();
                break;
            case CompoundEffect.MetalOxide_Protect:
                ActivateMetalOxideEffect();
                break;
            case CompoundEffect.MetalHydride_Draw4:
                ActivateMetalHydrideEffect();
                break;
            case CompoundEffect.Hydrocarbon_DiscardDraw:
                ActivateHydrocarbonEffect();
                break;
            case CompoundEffect.NetworkSolid_Protection:
                ActivateNetworkSolidEffect();
                break;
            default:
                Debug.Log($"No specific effect implementation for {compound.effect}");
                break;
        }
        
        if (compound.isOncePerTurn)
        {
            playerCompounds.Remove(compound);
            Debug.Log($"Consumed {compound.formula} after use");
        }
    }
    
    private void ActivateDrawElementsEffect(int count)
    {
        Debug.Log($"DrawElements Effect: Drawing {count} elements");
        for (int i = 0; i < count; i++)
        {
            if (playerHand.Count < handLimit)
                AddRandomElementToHand(true);
        }
        if (statusText) statusText.text = $"Drew {count} elements!";
    }
    
    private void ActivateSkipPlayerTurnEffect()
    {
        Debug.Log("SkipPlayerTurn Effect: Opponent's turn will be skipped");
        if (statusText) statusText.text = "Opponent's turn will be skipped!";
    }
    
    private void ActivateDiscardElementsEffect(int count)
    {
        Debug.Log($"DiscardElements Effect: Opponent must discard {count} elements");
        if (statusText) statusText.text = $"Opponent must discard {count} elements (not implemented in basic version)";
    }
    
    private void ActivateReceiveElementsEffect(int count)
    {
        Debug.Log($"ReceiveElements Effect: Drawing {count} elements");
        for (int i = 0; i < count; i++)
        {
            if (playerHand.Count < handLimit)
                AddRandomElementToHand(true);
        }
        if (statusText) statusText.text = $"Drew {count} elements!";
    }
    
    private void ActivateNegateDissolusionEffect()
    {
        Debug.Log("NegateDissolusion Effect: Protects from dissolution attacks");
        if (statusText) statusText.text = "Protected from dissolution attacks!";
    }
    
    private void ActivateH2OEffect()
    {
        Debug.Log("H2O Effect: Pick up to 2 elements and change them to any desired elements");
        if (statusText) statusText.text = "H2O: Select up to 2 elements to transform (not implemented in basic version)";
    }
    
    private void ActivateCO2Effect()
    {
        Debug.Log("CO2 Effect: Destroy a compound on any player's field");
        if (statusText) statusText.text = "CO2: Would destroy opponent compound (not implemented in basic version)";
    }
    
    private void ActivateAcidEffect()
    {
        Debug.Log("Acid Effect: Skip opponent's turn");
        if (statusText) statusText.text = "Acid: Opponent's turn will be skipped!";
    }
    
    private void ActivateBaseEffect()
    {
        Debug.Log("Base Effect: Target player discards 3 elements");
        if (statusText) statusText.text = "Base: Opponent must discard 3 elements (not implemented in basic version)";
    }
    
    private void ActivateSaltEffect()
    {
        Debug.Log("Salt Effect: Ask a player for 2 element cards");
        for (int i = 0; i < 2; i++)
        {
            if (playerHand.Count < handLimit)
                AddRandomElementToHand(true);
        }
        if (statusText) statusText.text = "Salt: Received 2 elements from opponent!";
    }
    
    private void ActivateMetallicEffect()
    {
        Debug.Log("Metallic Effect: Negate dissociation and receive 2 elements");
        AddRandomElementToHand(true);
        AddRandomElementToHand(true);
        if (statusText) statusText.text = "Metallic: Negated attack and drew 2 elements!";
    }
    
    private void ActivateMetalOxideEffect()
    {
        Debug.Log("Metal Oxide Effect: Nullify H2O reactions until start of turn");
        if (statusText) statusText.text = "Metal Oxide: Protected from water reactions until your next turn!";
    }
    
    private void ActivateMetalHydrideEffect()
    {
        Debug.Log("Metal Hydride Effect: Receive 4 elements");
        for (int i = 0; i < 4; i++)
        {
            if (playerHand.Count < handLimit)
                AddRandomElementToHand(true);
        }
        if (statusText) statusText.text = "Metal Hydride: Drew 4 elements!";
    }
    
    private void ActivateHydrocarbonEffect()
    {
        Debug.Log("Hydrocarbon Effect: Discard any elements and draw same amount");
        if (statusText) statusText.text = "Hydrocarbon: Select elements to discard and redraw (basic: draw 2)";
        AddRandomElementToHand(true);
        AddRandomElementToHand(true);
    }
    
    private void ActivateNetworkSolidEffect()
    {
        Debug.Log("Network Solid Effect: Cannot be targeted by other players");
        if (statusText) statusText.text = "Network Solid: You are protected from targeting until your next turn!";
    }
    
    private void ClearCompoundSelection()
    {
        foreach (var compoundCard in selectedCompoundCards)
        {
            if (compoundCard != null)
                compoundCard.SetSelected(false);
        }
        selectedCompoundCards.Clear();
    }
    
    public void EndTurn()
    {
        if (!isPlayerTurn) 
        {
            Debug.LogWarning("EndTurn called but it's not this player's turn!");
            return;
        }
        
        
        
        Debug.Log($"EndTurn called by {(PhotonNetwork.IsMasterClient ? "Master" : "Client")} player");
        
        
        hasDiscardedThisTurn = false;
        isPlayerTurn = false;
        
        
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Sending turn change via Custom Properties");
            
            
            var props = new ExitGames.Client.Photon.Hashtable();
            props["TurnPlayer"] = PhotonNetwork.LocalPlayer.ActorNumber; 
            props["TurnTimestamp"] = PhotonNetwork.Time; 
            props["NextPlayer"] = GetOtherPlayerActorNumber(); 
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            
            Debug.Log($"Set turn properties: TurnPlayer={PhotonNetwork.LocalPlayer.ActorNumber}, NextPlayer={GetOtherPlayerActorNumber()}");
        }
        else if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Single player mode - using AI simulation");
            
            StartCoroutine(SimulateOpponentTurn());
        }
        else
        {
            Debug.LogError("Failed to send turn change - photonView or network issue");
        }
        
        
        ClearSelection();
        
        
        UpdateUI();
    }
    
    private IEnumerator SimulateOpponentTurn()
    {
        yield return new WaitForSeconds(2f);
        
        
        
        
        yield return new WaitForSeconds(1f);
        
        
        StartPlayerTurn();
    }
    
    private void StartPlayerTurn()
    {
        
        isPlayerTurn = true;
        hasDiscardedThisTurn = false;
        
        
        DealElementsToPlayerNewRules(true);
        
        
        UpdateUI();
    }
    
    private void DealElementsToPlayer(bool isPlayer)
    {
        List<ElementData> targetHand = isPlayer ? playerHand : opponentHand;
        int elementsToAdd = 0;
        
        
        if (targetHand.Count >= 5)
            elementsToAdd = 2;
        else if (targetHand.Count > 0)
            elementsToAdd = 4;
        else
            elementsToAdd = 5;
        
        for (int i = 0; i < elementsToAdd && targetHand.Count < handLimit; i++)
        {
            AddRandomElementToHand(isPlayer);
        }
        
        Debug.Log($"{(isPlayer ? "Player" : "Opponent")} received {elementsToAdd} elements. Total: {targetHand.Count}");
    }
    
    private void DealElementsToPlayerNewRules(bool isPlayer)
    {
        List<ElementData> targetHand = isPlayer ? playerHand : opponentHand;
        int initialCount = targetHand.Count;
        int elementsToAdd = 0;
        
        
        if (targetHand.Count == 0)
        {
            elementsToAdd = 5; 
        }
        else if (targetHand.Count <= 5)
        {
            elementsToAdd = 4; 
        }
        else 
        {
            elementsToAdd = 3; 
        }
        
        
        int spaceAvailable = handLimit - targetHand.Count;
        elementsToAdd = Mathf.Min(elementsToAdd, spaceAvailable);
        
        int actualAdded = 0;
        for (int i = 0; i < elementsToAdd; i++)
        {
            if (targetHand.Count < handLimit) 
            {
                AddRandomElementToHand(isPlayer);
                actualAdded++;
            }
            else
            {
                Debug.Log($"Hand limit reached for {(isPlayer ? "Player" : "Opponent")}");
                break;
            }
        }
        
        Debug.Log($"{(isPlayer ? "Player" : "Opponent")} turn start: Had {initialCount} elements, received {actualAdded} elements. Total: {targetHand.Count}/{handLimit}");
    }
    
    [PunRPC]
    void SyncPlayerData(int elementCount, string[] compoundFormulas)
    {
        
        opponentHand.Clear();
        opponentCompounds.Clear();
        
        
        for (int i = 0; i < elementCount; i++)
        {
            if (chemistryDatabase.allElements.Count > 0)
                opponentHand.Add(chemistryDatabase.allElements[0]);
        }
        
        
        foreach (string formula in compoundFormulas)
        {
            CompoundData compound = chemistryDatabase.GetCompoundByFormula(formula);
            if (compound != null)
            {
                opponentCompounds.Add(compound);
            }
            else
            {
                Debug.LogWarning($"Could not find compound with formula: {formula}");
            }
        }
        
        Debug.Log($"Synced opponent data: {elementCount} elements, {compoundFormulas.Length} compounds");
        
        
        UpdateUI();
    }
    
    [PunRPC]
    void SetInitialTurn(bool isMyTurn)
    {
        
        isPlayerTurn = isMyTurn;
        Debug.Log($"Received initial turn assignment: {(isMyTurn ? "My turn" : "Opponent's turn")}");
        UpdateUI();
    }
    
    [PunRPC]
    void SyncTurnChange(bool isMyTurn)
    {
        string senderName = "Unknown";
        try
        {
            if (photonView != null && photonView.Owner != null)
                senderName = photonView.Owner.NickName;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not get sender name: {e.Message}");
        }
        
        Debug.Log($"SyncTurnChange RPC received: isMyTurn={isMyTurn}, sender={senderName}");
        
        
        isPlayerTurn = isMyTurn;
        
        if (isMyTurn)
        {
            Debug.Log("It's now my turn - dealing new elements");
            
            DealElementsToPlayerNewRules(true);
            
            
            if (PhotonNetwork.IsConnected && photonView != null && photonView.IsMine)
            {
                string[] compoundFormulas = playerCompounds.Select(c => c.formula).ToArray();
                photonView.RPC("SyncPlayerData", RpcTarget.Others, playerHand.Count, compoundFormulas);
            }
        }
        else
        {
            Debug.Log("It's now opponent's turn - waiting");
        }
        
        UpdateUI();
    }
    
    
    
    private void AddRandomElementToHand(bool isPlayer = true)
    {
        List<ElementData> targetHand = isPlayer ? playerHand : opponentHand;
        
        if (targetHand.Count >= handLimit || chemistryDatabase.allElements.Count == 0) return;
        
        ElementData randomElement = chemistryDatabase.allElements[Random.Range(0, chemistryDatabase.allElements.Count)];
        targetHand.Add(randomElement);
        
        if (isPlayer && cheatModeEnabled)
        {
            AnalyzeHandCombinations();
        }
    }
    
    
    [ContextMenu("Debug: Show Available Compounds")]
    public void DebugShowCompounds()
    {
        if (chemistryDatabase == null) return;
        
        Debug.Log("=== AVAILABLE COMPOUNDS ===");
        foreach (var compound in chemistryDatabase.allCompounds)
        {
            string requirements = "";
            foreach (var req in compound.requiredElements)
            {
                requirements += $"{req.quantity}x{req.element.symbol} ";
            }
            Debug.Log($"{compound.formula}: {requirements}");
        }
    }
    
    [ContextMenu("Debug: Add Test Elements")]
    public void DebugAddTestElements()
    {
        
        ElementData hydrogen = chemistryDatabase.allElements.Find(e => e.symbol == "H");
        ElementData oxygen = chemistryDatabase.allElements.Find(e => e.symbol == "O");
        
        if (hydrogen != null && oxygen != null)
        {
            playerHand.Add(hydrogen);
            playerHand.Add(hydrogen);
            playerHand.Add(oxygen);
            UpdateUI();
            Debug.Log("Added 2 Hydrogen + 1 Oxygen for water creation");
        }
    }
    
    [ContextMenu("Debug: Test Cheat Mode")]
    public void DebugTestCheatMode()
    {
        Debug.Log("=== TESTING CHEAT MODE ===");
        Debug.Log($"Cheat mode button: {(cheatModeButton != null ? "Found" : "NULL")}");
        Debug.Log($"Cheat mode display: {(cheatModeDisplay != null ? "Found" : "NULL")}");
        Debug.Log($"Chemistry database: {(chemistryDatabase != null ? "Found" : "NULL")}");
        Debug.Log($"Player hand count: {playerHand.Count}");
        
        if (cheatModeButton != null)
        {
            ToggleCheatMode();
        }
        else
        {
            Debug.LogError("Cannot test cheat mode - button not assigned!");
        }
    }

    [ContextMenu("Auto-Assign Missing UI References")]
    public void AutoAssignMissingReferences()
    {
        Debug.Log("=== AUTO-ASSIGNING MISSING UI REFERENCES ===");
        
        
        if (cheatModeButton == null)
        {
            GameObject cheatButtonObj = GameObject.Find("CheatModeButton");
            if (cheatButtonObj != null)
            {
                cheatModeButton = cheatButtonObj.GetComponent<Button>();
                if (cheatModeButton != null)
                {
                    Debug.Log("✓ Found and assigned CheatModeButton");
                    cheatModeButton.onClick.RemoveAllListeners();
                    cheatModeButton.onClick.AddListener(ToggleCheatMode);
                }
            }
        }
        
        
        if (cheatModeDisplay == null)
        {
            GameObject cheatDisplayObj = GameObject.Find("CheatModeDisplay");
            if (cheatDisplayObj != null)
            {
                cheatModeDisplay = cheatDisplayObj;
                Debug.Log("✓ Found and assigned CheatModeDisplay");
            }
        }
        
        
        if (playerHandParent == null)
        {
            GameObject handContent = GameObject.Find("PlayerHandScrollArea");
            if (handContent != null)
            {
                Transform content = handContent.transform.Find("Content");
                if (content != null)
                {
                    playerHandParent = content;
                    Debug.Log("✓ Found and assigned PlayerHandParent");
                }
            }
        }
        
        if (createCompoundButton == null)
        {
            GameObject createButtonObj = GameObject.Find("CreateCompoundButton");
            if (createButtonObj != null)
            {
                createCompoundButton = createButtonObj.GetComponent<Button>();
                if (createCompoundButton != null)
                {
                    Debug.Log("✓ Found and assigned CreateCompoundButton");
                    createCompoundButton.onClick.RemoveAllListeners();
                    createCompoundButton.onClick.AddListener(TryCreateCompound);
                }
            }
        }
        
        if (discardButton == null)
        {
            GameObject discardButtonObj = GameObject.Find("DiscardButton");
            if (discardButtonObj != null)
            {
                discardButton = discardButtonObj.GetComponent<Button>();
                if (discardButton != null)
                {
                    Debug.Log("✓ Found and assigned DiscardButton");
                    discardButton.onClick.RemoveAllListeners();
                    discardButton.onClick.AddListener(TryDiscardElements);
                }
            }
        }
        
        if (activateEffectButton == null)
        {
            GameObject activateButtonObj = GameObject.Find("ActivateEffectButton");
            if (activateButtonObj != null)
            {
                activateEffectButton = activateButtonObj.GetComponent<Button>();
                if (activateEffectButton != null)
                {
                    Debug.Log("✓ Found and assigned ActivateEffectButton");
                    activateEffectButton.onClick.RemoveAllListeners();
                    activateEffectButton.onClick.AddListener(TryActivateEffect);
                }
            }
        }
        
        if (endTurnButton == null)
        {
            GameObject endTurnButtonObj = GameObject.Find("EndTurnButton");
            if (endTurnButtonObj != null)
            {
                endTurnButton = endTurnButtonObj.GetComponent<Button>();
                if (endTurnButton != null)
                {
                    Debug.Log("✓ Found and assigned EndTurnButton");
                    endTurnButton.onClick.RemoveAllListeners();
                    endTurnButton.onClick.AddListener(EndTurn);
                }
            }
        }
        
        if (statusText == null)
        {
            GameObject statusTextObj = GameObject.Find("StatusText");
            if (statusTextObj != null)
            {
                statusText = statusTextObj.GetComponent<Text>();
                if (statusText != null) Debug.Log("✓ Found and assigned StatusText");
            }
        }
        
        if (elementCountText == null)
        {
            GameObject elementCountObj = GameObject.Find("ElementCountText");
            if (elementCountObj != null)
            {
                elementCountText = elementCountObj.GetComponent<Text>();
                if (elementCountText != null) Debug.Log("✓ Found and assigned ElementCountText");
            }
        }
        
        if (compoundCountText == null)
        {
            GameObject compoundCountObj = GameObject.Find("CompoundCountText");
            if (compoundCountObj != null)
            {
                compoundCountText = compoundCountObj.GetComponent<Text>();
                if (compoundCountText != null) Debug.Log("✓ Found and assigned CompoundCountText");
            }
        }
        
        if (opponentCompoundsParent == null)
        {
            GameObject opponentCompoundsArea = GameObject.Find("OpponentCompoundsArea");
            if (opponentCompoundsArea != null)
            {
                Transform content = opponentCompoundsArea.transform.Find("Content");
                if (content != null)
                {
                    opponentCompoundsParent = content;
                    Debug.Log("✓ Found and assigned OpponentCompoundsParent");
                }
            }
        }
        
        
        if (cheatModeDisplay == null)
        {
            CreateCheatModeDisplay();
        }
        
        Debug.Log("=== AUTO-ASSIGNMENT COMPLETE ===");
        Debug.Log($"Cheat mode button: {(cheatModeButton != null ? "ASSIGNED" : "STILL NULL")}");
        Debug.Log($"Cheat mode display: {(cheatModeDisplay != null ? "ASSIGNED" : "STILL NULL")}");
        
        
        DebugPhotonViewSetup();
    }
    
    [ContextMenu("Debug PhotonView Setup")]
    public void DebugPhotonViewSetup()
    {
        Debug.Log("=== PHOTON VIEW DEBUG ===");
        Debug.Log($"PhotonNetwork.IsConnected: {PhotonNetwork.IsConnected}");
        Debug.Log($"PhotonView exists: {(photonView != null)}");
        
        if (photonView != null)
        {
            try
            {
                Debug.Log($"PhotonView.IsMine: {photonView.IsMine}");
                Debug.Log($"PhotonView.ViewID: {photonView.ViewID}");
                
                string ownerName = "NULL";
                if (photonView.Owner != null)
                    ownerName = photonView.Owner.NickName;
                Debug.Log($"PhotonView.Owner: {ownerName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error accessing PhotonView properties: {e.Message}");
            }
        }
        
        try
        {
            string localPlayerName = "NULL";
            if (PhotonNetwork.LocalPlayer != null)
                localPlayerName = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log($"Local Player: {localPlayerName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error accessing LocalPlayer: {e.Message}");
        }
        
        Debug.Log($"Current turn state: {(isPlayerTurn ? "MY TURN" : "OPPONENT'S TURN")}");
        Debug.Log($"Waiting for turn ack: {waitingForTurnAck}");
    }
    
    [ContextMenu("Force Turn Recovery")]
    public void ForceTurnRecovery()
    {
        Debug.Log("=== FORCING TURN RECOVERY ===");
        waitingForTurnAck = false;
        
        if (turnTimeoutCoroutine != null)
        {
            StopCoroutine(turnTimeoutCoroutine);
            turnTimeoutCoroutine = null;
        }
        
        
        if (PhotonNetwork.IsConnected && photonView != null && photonView.IsMine)
        {
            Debug.Log("Attempting to resync turn state...");
            
            
            bool shouldBeMyTurn = !isPlayerTurn; 
            isPlayerTurn = shouldBeMyTurn;
            
            if (shouldBeMyTurn)
            {
                DealElementsToPlayerNewRules(true);
                string[] compoundFormulas = playerCompounds.Select(c => c.formula).ToArray();
                photonView.RPC("SyncPlayerData", RpcTarget.Others, playerHand.Count, compoundFormulas);
            }
            
            
            photonView.RPC("SyncTurnChange", RpcTarget.Others, !shouldBeMyTurn);
        }
        
        UpdateUI();
        Debug.Log("Turn recovery completed");
    }
    
    private int GetOtherPlayerActorNumber()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return player.ActorNumber;
            }
        }
        return -1; 
    }
    
    void Update()
    {
        
        if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
        {
            var props = PhotonNetwork.CurrentRoom.CustomProperties;
            
            if (props.ContainsKey("NextPlayer"))
            {
                int nextPlayer = (int)props["NextPlayer"];
                bool shouldBeMyTurn = (nextPlayer == PhotonNetwork.LocalPlayer.ActorNumber);
                
                
                if (shouldBeMyTurn && !isPlayerTurn)
                {
                    Debug.Log("Detected turn change via properties - it's now my turn!");
                    isPlayerTurn = true;
                    DealElementsToPlayerNewRules(true);
                    UpdateUI();
                    
                    
                    var clearProps = new ExitGames.Client.Photon.Hashtable();
                    clearProps["NextPlayer"] = null;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(clearProps);
                }
                else if (!shouldBeMyTurn && isPlayerTurn)
                {
                    Debug.Log("Detected turn change via properties - it's now opponent's turn!");
                    isPlayerTurn = false;
                    UpdateUI();
                }
            }
        }
    }
    
    private void CreateCheatModeDisplay()
    {
        Debug.Log("Creating CheatModeDisplay...");
        
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            return;
        }
        
        
        GameObject cheatDisplay = new GameObject("CheatModeDisplay");
        cheatDisplay.transform.SetParent(canvas.transform);
        cheatDisplay.layer = LayerMask.NameToLayer("UI");
        
        
        RectTransform cheatRect = cheatDisplay.AddComponent<RectTransform>();
        cheatRect.localScale = Vector3.one;
        cheatRect.anchorMin = new Vector2(0.05f, 0.15f);
        cheatRect.anchorMax = new Vector2(0.45f, 0.55f);
        cheatRect.sizeDelta = Vector2.zero;
        cheatRect.anchoredPosition = Vector2.zero;
        
        
        Image bg = cheatDisplay.AddComponent<Image>();
        bg.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        
        
        GameObject content = new GameObject("Content");
        content.transform.SetParent(cheatDisplay.transform);
        content.layer = LayerMask.NameToLayer("UI");
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.localScale = Vector3.one;
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;
        contentRect.anchoredPosition = Vector2.zero;
        
        
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 5;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        
        ScrollRect scrollRect = cheatDisplay.AddComponent<ScrollRect>();
        scrollRect.content = contentRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        
        
        cheatDisplay.SetActive(false);
        
        
        cheatModeDisplay = cheatDisplay;
        
        Debug.Log("✓ CheatModeDisplay created and assigned!");
    }
    
    private void HideOldGameUI()
    {
        
        ChemistryCardManager oldManager = FindObjectOfType<ChemistryCardManager>();
        if (oldManager != null)
        {
            
            oldManager.gameObject.SetActive(false);
            Debug.Log("Hidden old ChemistryCardManager");
        }
        
        
        CardStack[] cardStacks = FindObjectsOfType<CardStack>();
        foreach (var stack in cardStacks)
        {
            stack.gameObject.SetActive(false);
            Debug.Log($"Hidden CardStack: {stack.name}");
        }
        
        
        PlayerLocal[] players = FindObjectsOfType<PlayerLocal>();
        foreach (var player in players)
        {
            
            Transform playerBoard = player.transform.Find("PlayerBoard");
            if (playerBoard != null)
            {
                playerBoard.gameObject.SetActive(false);
                Debug.Log($"Hidden PlayerBoard for {player.name}");
            }
        }
    }
    
    public void ToggleCheatMode()
    {
        Debug.Log("ToggleCheatMode called!");
        cheatModeEnabled = !cheatModeEnabled;
        Debug.Log($"Cheat mode enabled: {cheatModeEnabled}");
        
        if (cheatModeDisplay)
        {
            cheatModeDisplay.SetActive(cheatModeEnabled);
            Debug.Log($"Cheat display set to: {cheatModeEnabled}");
        }
        else
        {
            Debug.LogError("Cheat mode display is null!");
        }
        
        if (cheatModeEnabled)
        {
            AnalyzeHandCombinations();
            if (statusText) statusText.text = "Cheat Mode ON - Possible combinations shown";
        }
        else
        {
            if (statusText) statusText.text = "Cheat Mode OFF";
        }
        
        
        if (cheatModeButton)
        {
            Text buttonText = cheatModeButton.GetComponentInChildren<Text>();
            if (buttonText) buttonText.text = cheatModeEnabled ? "Hide Cheat" : "Cheat Mode";
        }
        else
        {
            Debug.LogError("Cheat mode button is null!");
        }
    }
    
    private void AnalyzeHandCombinations()
    {
        Debug.Log("AnalyzeHandCombinations called!");
        if (cheatModeDisplay == null)
        {
            Debug.LogError("cheatModeDisplay is null!");
            return;
        }
        if (chemistryDatabase == null)
        {
            Debug.LogError("chemistryDatabase is null!");
            return;
        }
        
        Debug.Log($"Player hand has {playerHand.Count} elements");
        
        
        Transform contentParent = cheatModeDisplay.transform.Find("Content");
        if (contentParent == null) 
        {
            Debug.LogError("Content parent not found in cheat display!");
            return;
        }
        
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        
        List<string> possibleCombinations = new List<string>();
        
        
        foreach (var compound in chemistryDatabase.allCompounds)
        {
            bool canCreate = false;
            string requirements = "";
            
            if (compound.isMetallicCompound)
            {
                canCreate = compound.CanCreateMetallicFrom(playerHand);
                if (canCreate)
                {
                    requirements = "Any 3 METAL elements with same positive charge";
                }
            }
            else
            {
                canCreate = compound.CanCreateFrom(playerHand);
                if (canCreate)
                {
                    foreach (var req in compound.requiredElements)
                    {
                        requirements += $"{req.quantity}x{req.element.symbol} ";
                    }
                }
            }
            
            if (canCreate)
            {
                string effectDescription = GetEffectDescription(compound.effect);
                possibleCombinations.Add($"{compound.formula}: {requirements}→ {effectDescription}");
            }
        }
        
        
        if (possibleCombinations.Count == 0)
        {
            CreateCheatText("No combinations possible with current hand", contentParent);
        }
        else
        {
            CreateCheatText($"Possible combinations ({possibleCombinations.Count}):", contentParent);
            foreach (string combination in possibleCombinations)
            {
                CreateCheatText(combination, contentParent);
            }
        }
        
        Debug.Log($"Cheat Mode: Found {possibleCombinations.Count} possible combinations");
    }
    
    private void CreateCheatText(string text, Transform parent)
    {
        GameObject textObj = new GameObject("CheatText");
        textObj.transform.SetParent(parent);
        textObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(400, 25);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 18;
        textComponent.alignment = TextAnchor.MiddleLeft;
        textComponent.color = Color.green;
        textComponent.fontStyle = FontStyle.Bold;
    }
    
    private string GetEffectDescription(CompoundEffect effect)
    {
        switch (effect)
        {
            case CompoundEffect.None: return "No effect";
            case CompoundEffect.DrawElements: return "Draw 4 elements";
            case CompoundEffect.SkipPlayerTurn: return "Skip opponent turn";
            case CompoundEffect.DiscardElements: return "Opponent discards 3";
            case CompoundEffect.ReceiveElements: return "Receive 4 elements";
            case CompoundEffect.NegateDissolusion: return "Negate dissociation";
            case CompoundEffect.SwapHands: return "Swap all hands";
            case CompoundEffect.DiscardDraw: return "Discard/draw elements";
            case CompoundEffect.ExchangeHands: return "Exchange with right player";
            default: return "Unknown effect";
        }
    }
}