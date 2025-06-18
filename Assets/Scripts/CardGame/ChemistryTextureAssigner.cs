using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ChemistryTextureAssigner : MonoBehaviour
{
    [Header("Chemistry Database")]
    public ChemistryDatabase chemistryDatabase;
    
    [ContextMenu("Assign Element Textures")]
    public void AssignElementTextures()
    {
        if (chemistryDatabase == null)
        {
            Debug.LogError("Chemistry Database not assigned!");
            return;
        }
        
        
        Dictionary<string, string> textureMap = new Dictionary<string, string>()
        {
            {"H", "Hydrogen"},
            {"Na", "Sodium"},
            {"K", "Potassium"},
            {"Rb", "Rubidium"},
            {"Mg", "Magnesium"},
            {"Ca", "Calcium"},
            {"Sr", "Strontium"},
            {"C", "Carbon"},
            {"O", "Oxygen"},
            {"S", "Sulfur"}, 
            {"F", "Fluorine"},
            {"Cl", "Chlorine"},
            {"Br", "Bromine"},
            {"Cu", "Coppa"}, 
            {"Zn", "Zinc"}
        };
        
        int assignedCount = 0;
        
        foreach (var element in chemistryDatabase.allElements)
        {
            if (textureMap.ContainsKey(element.symbol))
            {
                string textureName = textureMap[element.symbol];
                string texturePath = $"Assets/Cards/{textureName}.jpg";
                
                
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                if (texture != null)
                {
                    
                    string spritePath = AssetDatabase.GetAssetPath(texture);
                    TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
                    if (importer != null)
                    {
                        
                        if (importer.textureType != TextureImporterType.Sprite)
                        {
                            importer.textureType = TextureImporterType.Sprite;
                            importer.spriteImportMode = SpriteImportMode.Single;
                            importer.spritePivot = Vector2.one * 0.5f;
                            importer.spritePixelsPerUnit = 100;
                            AssetDatabase.ImportAsset(spritePath);
                        }
                        
                        
                        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                        if (sprite != null)
                        {
                            element.cardSprite = sprite;
                            EditorUtility.SetDirty(element);
                            assignedCount++;
                            Debug.Log($"✓ Assigned {textureName} texture to {element.elementName} ({element.symbol})");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"Texture not found: {texturePath}");
                }
            }
            else
            {
                Debug.LogWarning($"No texture mapping for element: {element.symbol}");
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"✓ Assigned {assignedCount} element textures successfully!");
    }
    
    [ContextMenu("Create Missing Element Data")]
    public void CreateMissingElementData()
    {
        if (chemistryDatabase == null)
        {
            Debug.LogError("Chemistry Database not assigned!");
            return;
        }
        
        
        string[] expectedElements = {"H", "Na", "K", "Rb", "Mg", "Ca", "Sr", "C", "O", "S", "F", "Cl", "Br", "Cu", "Zn"};
        string[] elementNames = {"Hydrogen", "Sodium", "Potassium", "Rubidium", "Magnesium", "Calcium", "Strontium", "Carbon", "Oxygen", "Sulfur", "Fluorine", "Chlorine", "Bromine", "Copper", "Zinc"};
        int[] oxidationNumbers = {1, 1, 1, 1, 2, 2, 2, 4, -2, -2, -1, -1, -1, 2, 2};
        
        for (int i = 0; i < expectedElements.Length; i++)
        {
            string symbol = expectedElements[i];
            bool exists = false;
            
            foreach (var element in chemistryDatabase.allElements)
            {
                if (element.symbol == symbol)
                {
                    exists = true;
                    break;
                }
            }
            
            if (!exists)
            {
                
                ElementData newElement = ScriptableObject.CreateInstance<ElementData>();
                newElement.elementName = elementNames[i];
                newElement.symbol = symbol;
                newElement.oxidationNumber = oxidationNumbers[i];
                
                
                if (symbol == "H" || symbol == "Na" || symbol == "K" || symbol == "Rb")
                    newElement.group = ElementGroup.AlkaliMetal;
                else if (symbol == "Mg" || symbol == "Ca" || symbol == "Sr")
                    newElement.group = ElementGroup.AlkaliEarth;
                else if (symbol == "C")
                    newElement.group = ElementGroup.Carbon;
                else if (symbol == "O" || symbol == "S")
                    newElement.group = ElementGroup.Oxygen;
                else if (symbol == "F" || symbol == "Cl" || symbol == "Br")
                    newElement.group = ElementGroup.Halogen;
                else if (symbol == "Cu" || symbol == "Zn")
                    newElement.group = ElementGroup.TransitionMetal;
                
                
                string path = $"Assets/ScriptableObjects/Elements/{elementNames[i]}.asset";
                AssetDatabase.CreateAsset(newElement, path);
                
                
                chemistryDatabase.allElements.Add(newElement);
                
                Debug.Log($"✓ Created missing element: {elementNames[i]} ({symbol})");
            }
        }
        
        EditorUtility.SetDirty(chemistryDatabase);
        AssetDatabase.SaveAssets();
        Debug.Log("✓ Element data check complete!");
    }
}
#endif