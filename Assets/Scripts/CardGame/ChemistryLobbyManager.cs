using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChemistryLobbyManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject rulesPanel;
    [SerializeField] private Text rulesText;
    [SerializeField] private Button closeRulesButton;
    
    [Header("Chemistry Info Display")]
    [SerializeField] private Transform elementsPreviewParent;
    [SerializeField] private Transform compoundsPreviewParent;
    [SerializeField] private GameObject elementPreviewPrefab;
    [SerializeField] private GameObject compoundPreviewPrefab;
    [SerializeField] private ChemistryDatabase chemistryDatabase;
    
    void Start()
    {
        SetupUI();
        SetupGameInfo();
        
        if (chemistryDatabase != null)
        {
            DisplayChemistryPreview();
        }
    }
    
    private void SetupUI()
    {
        if (titleText)
            titleText.text = "Chemistry Card Game";
            
        if (descriptionText)
        {
            descriptionText.text = "Combine elements to create compounds and perform chemical reactions!\n" +
                                 "First player to collect 8 unique compounds wins!";
        }
        
        if (startGameButton)
            startGameButton.onClick.AddListener(StartGame);
            
        if (rulesButton)
            rulesButton.onClick.AddListener(ShowRules);
            
        if (backButton)
            backButton.onClick.AddListener(BackToMenu);
            
        if (closeRulesButton)
            closeRulesButton.onClick.AddListener(HideRules);
            
        if (rulesPanel)
            rulesPanel.SetActive(false);
    }
    
    private void SetupGameInfo()
    {
        if (rulesText)
        {
            rulesText.text = @"CHEMISTRY CARD GAME RULES

OBJECTIVE:
Be the first player to collect 8 unique compounds!

SETUP:
• Each player starts with 10 element cards
• Elements have oxidation numbers that must balance to 0 when forming compounds
• Hand limit is 10 cards

TURN STRUCTURE:
1. Draw elements at turn start:
   - If you have ≥5 elements: receive 2 elements
   - If you have ≤5 elements: receive 4 elements  
   - If you have no elements: receive 5 elements

2. During your turn, you can:
   - Create as many compounds as possible
   - Perform as many reactions as possible
   - Use compound effects (once per turn each)

3. End your turn when done

COMPOUND CREATION:
• Select elements that follow chemistry rules
• Oxidation numbers must sum to 0
• Example: H₂O requires 2[H] + 1[O] = 2(+1) + 1(-2) = 0

MAIN COMPOUNDS:
• H₂O (Water) - No effect
• CO₂ (Carbon Dioxide) - Draw 4 elements
• Acid - Skip next player's turn
• Base - Target player discards 3 elements
• Salt - Receive 4 elements
• Metallic - Negates dissolution + receive 2 elements
• Metal Hydride - Swap all hand elements
• Hydrocarbon - Discard any elements, draw same amount
• Network Solid - Exchange hands with player to right

REACTIONS:
Combine compounds to create powerful effects:
• Dissociation: [H₂O] + [Acid/Base/Salt] → Banish all reactants
• Neutralisation: [Acid] + [Base] → [Salt] + target 2 compounds for dissolution
• Combustion: [Hydrocarbon] + O → [CO₂] + [H₂O] + force end turn
• And more complex reactions...

WINNING:
First player to collect 8 different compounds wins!

STRATEGY TIPS:
• Balance element collection with compound creation
• Time your reactions for maximum impact
• Use compound effects strategically
• Pay attention to oxidation number balancing";
        }
    }
    
    private void DisplayChemistryPreview()
    {
        if (elementsPreviewParent && chemistryDatabase.allElements.Count > 0)
        {
            
            for (int i = 0; i < Mathf.Min(6, chemistryDatabase.allElements.Count); i++)
            {
                var element = chemistryDatabase.allElements[i];
                GameObject preview = CreateElementPreview(element);
                if (preview != null)
                    preview.transform.SetParent(elementsPreviewParent);
            }
        }
        
        if (compoundsPreviewParent && chemistryDatabase.allCompounds.Count > 0)
        {
            
            for (int i = 0; i < Mathf.Min(4, chemistryDatabase.allCompounds.Count); i++)
            {
                var compound = chemistryDatabase.allCompounds[i];
                GameObject preview = CreateCompoundPreview(compound);
                if (preview != null)
                    preview.transform.SetParent(compoundsPreviewParent);
            }
        }
    }
    
    private GameObject CreateElementPreview(ElementData element)
    {
        if (elementPreviewPrefab != null)
        {
            GameObject preview = Instantiate(elementPreviewPrefab);
            ElementCard card = preview.GetComponent<ElementCard>();
            if (card != null)
            {
                card.Initialize(element);
                card.IsInteractable = false; 
            }
            return preview;
        }
        
        
        GameObject obj = new GameObject($"Element_{element.symbol}");
        obj.AddComponent<RectTransform>();
        
        Text text = obj.AddComponent<Text>();
        text.text = $"{element.symbol}\n{element.elementName}\n{element.oxidationNumber:+0;-0;0}";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 10;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        
        Image bg = obj.AddComponent<Image>();
        bg.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
        
        return obj;
    }
    
    private GameObject CreateCompoundPreview(CompoundData compound)
    {
        if (compoundPreviewPrefab != null)
        {
            GameObject preview = Instantiate(compoundPreviewPrefab);
            CompoundCard card = preview.GetComponent<CompoundCard>();
            if (card != null)
            {
                card.Initialize(compound);
                card.IsInteractable = false; 
            }
            return preview;
        }
        
        
        GameObject obj = new GameObject($"Compound_{compound.formula}");
        obj.AddComponent<RectTransform>();
        
        Text text = obj.AddComponent<Text>();
        text.text = $"{compound.formula}\n{compound.compoundName}";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 10;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        
        Image bg = obj.AddComponent<Image>();
        bg.color = new Color(0.8f, 0.8f, 1f, 0.8f);
        
        return obj;
    }
    
    private void StartGame()
    {
        
        if (chemistryDatabase == null || chemistryDatabase.allElements.Count == 0)
        {
            Debug.LogWarning("Chemistry database not set up properly!");
            return;
        }
        
        
        SceneManager.LoadScene("CardGame-Board");
    }
    
    private void ShowRules()
    {
        if (rulesPanel)
            rulesPanel.SetActive(true);
    }
    
    private void HideRules()
    {
        if (rulesPanel)
            rulesPanel.SetActive(false);
    }
    
    private void BackToMenu()
    {
        
        SceneManager.LoadScene(0); 
    }
}