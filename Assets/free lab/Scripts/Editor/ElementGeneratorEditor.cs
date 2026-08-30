using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using PeriodicTableSystem.Data;

// این enum فقط برای منطق داخلی ادیتور استفاده می‌شه
public enum ElementCategory
{
    Nonmetal, NobleGas, AlkaliMetal, AlkalineEarth, 
    Metalloid, Halogen, TransitionMetal, PostTransition, 
    Lanthanide, Actinide
}

public class ElementGeneratorEditor : EditorWindow
{
    private string savePath = "Assets/Elements";
    
    [MenuItem("Tools/Chemistry/Generate All Elements")]
    static void Init()
    {
        var window = GetWindow<ElementGeneratorEditor>("Element Generator");
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Label("Generate 92 Chemical Elements", EditorStyles.boldLabel);
        savePath = EditorGUILayout.TextField("Save Path:", savePath);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🚀 Generate All 92 Elements", GUILayout.Height(40)))
        {
            GenerateElements();
        }
    }
    
    void GenerateElements()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        
        int count = 0;

        // 1-2
        count += Create(1, "H", "Hydrogen", 1.008f, 2.20f, 1, ElementCategory.Nonmetal, Color.white, 1, 0.53f);
        count += Create(2, "He", "Helium", 4.003f, 0f, 0, ElementCategory.NobleGas, new Color(1f, 0.75f, 0.8f), 2, 0.31f);
        
        // 3-4
        count += Create(3, "Li", "Lithium", 6.941f, 0.98f, 1, ElementCategory.AlkaliMetal, new Color(0.8f, 0.5f, 1f), 1, 1.67f);
        count += Create(4, "Be", "Beryllium", 9.012f, 1.57f, 2, ElementCategory.AlkalineEarth, new Color(0.76f, 1f, 0f), 2, 1.12f);
        
        // 5-10
        count += Create(5, "B", "Boron", 10.81f, 2.04f, 3, ElementCategory.Metalloid, new Color(1f, 0.71f, 0.71f), 3, 0.87f);
        count += Create(6, "C", "Carbon", 12.01f, 2.55f, 4, ElementCategory.Nonmetal, new Color(0.56f, 0.56f, 0.56f), 4, 0.67f);
        count += Create(7, "N", "Nitrogen", 14.01f, 3.04f, 3, ElementCategory.Nonmetal, new Color(0.19f, 0.31f, 0.97f), 5, 0.56f);
        count += Create(8, "O", "Oxygen", 16.00f, 3.44f, 2, ElementCategory.Nonmetal, new Color(1f, 0.05f, 0.05f), 6, 0.48f);
        count += Create(9, "F", "Fluorine", 19.00f, 3.98f, 1, ElementCategory.Halogen, new Color(0.56f, 0.88f, 0.31f), 7, 0.42f);
        count += Create(10, "Ne", "Neon", 20.18f, 0f, 0, ElementCategory.NobleGas, new Color(0.7f, 0.89f, 0.96f), 8, 0.38f);
        
        // 11-12
        count += Create(11, "Na", "Sodium", 22.99f, 0.93f, 1, ElementCategory.AlkaliMetal, new Color(0.67f, 0.36f, 0.95f), 1, 1.90f);
        count += Create(12, "Mg", "Magnesium", 24.31f, 1.31f, 2, ElementCategory.AlkalineEarth, new Color(0.54f, 1f, 0f), 2, 1.45f);
        
        // 13-18
        count += Create(13, "Al", "Aluminum", 26.98f, 1.61f, 3, ElementCategory.PostTransition, new Color(0.75f, 0.65f, 0.65f), 3, 1.18f);
        count += Create(14, "Si", "Silicon", 28.09f, 1.90f, 4, ElementCategory.Metalloid, new Color(0.94f, 0.78f, 0.63f), 4, 1.11f);
        count += Create(15, "P", "Phosphorus", 30.97f, 2.19f, 5, ElementCategory.Nonmetal, new Color(1f, 0.5f, 0f), 5, 0.98f);
        count += Create(16, "S", "Sulfur", 32.07f, 2.58f, 6, ElementCategory.Nonmetal, new Color(1f, 1f, 0.19f), 6, 0.88f);
        count += Create(17, "Cl", "Chlorine", 35.45f, 3.16f, 1, ElementCategory.Halogen, new Color(0.12f, 0.94f, 0.12f), 7, 0.79f);
        count += Create(18, "Ar", "Argon", 39.95f, 0f, 0, ElementCategory.NobleGas, new Color(0.5f, 0.82f, 0.89f), 8, 0.71f);
        
        // 19-20
        count += Create(19, "K", "Potassium", 39.10f, 0.82f, 1, ElementCategory.AlkaliMetal, new Color(0.56f, 0.25f, 0.83f), 1, 2.43f);
        count += Create(20, "Ca", "Calcium", 40.08f, 1.00f, 2, ElementCategory.AlkalineEarth, new Color(0.24f, 1f, 0f), 2, 1.94f);
        
        // 21-30 (Transition)
        count += Create(21, "Sc", "Scandium", 44.96f, 1.36f, 2, ElementCategory.TransitionMetal, Color.gray, 2, 1.84f);
        count += Create(22, "Ti", "Titanium", 47.87f, 1.54f, 2, ElementCategory.TransitionMetal, new Color(0.75f, 0.76f, 0.78f), 2, 1.76f);
        count += Create(23, "V", "Vanadium", 50.94f, 1.63f, 2, ElementCategory.TransitionMetal, new Color(0.65f, 0.65f, 0.67f), 2, 1.71f);
        count += Create(24, "Cr", "Chromium", 52.00f, 1.66f, 2, ElementCategory.TransitionMetal, new Color(0.54f, 0.6f, 0.78f), 1, 1.66f);
        count += Create(25, "Mn", "Manganese", 54.94f, 1.55f, 2, ElementCategory.TransitionMetal, new Color(0.61f, 0.48f, 0.78f), 2, 1.61f);
        count += Create(26, "Fe", "Iron", 55.85f, 1.83f, 2, ElementCategory.TransitionMetal, new Color(0.88f, 0.4f, 0.2f), 2, 1.56f);
        count += Create(27, "Co", "Cobalt", 58.93f, 1.88f, 2, ElementCategory.TransitionMetal, new Color(0.94f, 0.56f, 0.63f), 2, 1.52f);
        count += Create(28, "Ni", "Nickel", 58.69f, 1.91f, 2, ElementCategory.TransitionMetal, new Color(0.31f, 0.82f, 0.31f), 2, 1.49f);
        count += Create(29, "Cu", "Copper", 63.55f, 1.90f, 2, ElementCategory.TransitionMetal, new Color(0.78f, 0.5f, 0.2f), 1, 1.45f);
        count += Create(30, "Zn", "Zinc", 65.38f, 1.65f, 2, ElementCategory.TransitionMetal, new Color(0.49f, 0.5f, 0.69f), 2, 1.42f);
        
        // 31-36
        count += Create(31, "Ga", "Gallium", 69.72f, 1.81f, 3, ElementCategory.PostTransition, new Color(0.76f, 0.56f, 0.56f), 3, 1.36f);
        count += Create(32, "Ge", "Germanium", 72.63f, 2.01f, 4, ElementCategory.Metalloid, new Color(0.4f, 0.56f, 0.56f), 4, 1.25f);
        count += Create(33, "As", "Arsenic", 74.92f, 2.18f, 3, ElementCategory.Metalloid, new Color(0.74f, 0.5f, 0.89f), 5, 1.14f);
        count += Create(34, "Se", "Selenium", 78.97f, 2.55f, 2, ElementCategory.Nonmetal, new Color(1f, 0.63f, 0f), 6, 1.03f);
        count += Create(35, "Br", "Bromine", 79.90f, 2.96f, 1, ElementCategory.Halogen, new Color(0.65f, 0.16f, 0.16f), 7, 0.94f);
        count += Create(36, "Kr", "Krypton", 83.80f, 0f, 0, ElementCategory.NobleGas, new Color(0.36f, 0.72f, 0.82f), 8, 0.88f);
        
        // 37-38
        count += Create(37, "Rb", "Rubidium", 85.47f, 0.82f, 1, ElementCategory.AlkaliMetal, new Color(0.44f, 0.18f, 0.69f), 1, 2.65f);
        count += Create(38, "Sr", "Strontium", 87.62f, 0.95f, 2, ElementCategory.AlkalineEarth, Color.green, 2, 2.19f);
        
        // 39-48
        count += Create(39, "Y", "Yttrium", 88.91f, 1.22f, 2, ElementCategory.TransitionMetal, new Color(0.58f, 1f, 1f), 2, 2.12f);
        count += Create(40, "Zr", "Zirconium", 91.22f, 1.33f, 2, ElementCategory.TransitionMetal, new Color(0.58f, 0.88f, 0.88f), 2, 2.06f);
        count += Create(41, "Nb", "Niobium", 92.91f, 1.6f, 2, ElementCategory.TransitionMetal, new Color(0.45f, 0.76f, 0.79f), 2, 1.98f);
        count += Create(42, "Mo", "Molybdenum", 95.96f, 2.16f, 2, ElementCategory.TransitionMetal, new Color(0.33f, 0.71f, 0.71f), 1, 1.90f);
        count += Create(43, "Tc", "Technetium", 98f, 1.9f, 2, ElementCategory.TransitionMetal, new Color(0.23f, 0.62f, 0.62f), 2, 1.83f);
        count += Create(44, "Ru", "Ruthenium", 101.1f, 2.2f, 2, ElementCategory.TransitionMetal, new Color(0.14f, 0.56f, 0.56f), 1, 1.78f);
        count += Create(45, "Rh", "Rhodium", 102.9f, 2.28f, 2, ElementCategory.TransitionMetal, new Color(0.04f, 0.49f, 0.55f), 1, 1.73f);
        count += Create(46, "Pd", "Palladium", 106.4f, 2.20f, 2, ElementCategory.TransitionMetal, new Color(0f, 0.41f, 0.52f), 2, 1.69f);
        count += Create(47, "Ag", "Silver", 107.9f, 1.93f, 2, ElementCategory.TransitionMetal, new Color(0.75f, 0.75f, 0.75f), 1, 1.65f);
        count += Create(48, "Cd", "Cadmium", 112.4f, 1.69f, 2, ElementCategory.TransitionMetal, new Color(1f, 0.85f, 0.56f), 2, 1.61f);
        
        // 49-54
        count += Create(49, "In", "Indium", 114.8f, 1.78f, 3, ElementCategory.PostTransition, new Color(0.65f, 0.46f, 0.45f), 3, 1.56f);
        count += Create(50, "Sn", "Tin", 118.7f, 1.96f, 4, ElementCategory.PostTransition, new Color(0.4f, 0.5f, 0.5f), 4, 1.45f);
        count += Create(51, "Sb", "Antimony", 121.8f, 2.05f, 3, ElementCategory.Metalloid, new Color(0.62f, 0.39f, 0.71f), 5, 1.33f);
        count += Create(52, "Te", "Tellurium", 127.6f, 2.1f, 2, ElementCategory.Metalloid, new Color(0.83f, 0.48f, 0f), 6, 1.23f);
        count += Create(53, "I", "Iodine", 126.9f, 2.66f, 1, ElementCategory.Halogen, new Color(0.58f, 0f, 0.58f), 7, 1.15f);
        count += Create(54, "Xe", "Xenon", 131.3f, 2.6f, 0, ElementCategory.NobleGas, new Color(0.26f, 0.62f, 0.69f), 8, 1.08f);
        
        // 55-56
        count += Create(55, "Cs", "Cesium", 132.9f, 0.79f, 1, ElementCategory.AlkaliMetal, new Color(0.34f, 0.09f, 0.56f), 1, 2.98f);
        count += Create(56, "Ba", "Barium", 137.3f, 0.89f, 2, ElementCategory.AlkalineEarth, new Color(0f, 0.79f, 0f), 2, 2.53f);
        
        // 57-71 (Lanthanides)
        count += Create(57, "La", "Lanthanum", 138.9f, 1.1f, 2, ElementCategory.Lanthanide, new Color(0.44f, 0.83f, 1f), 2, 2.69f);
        count += Create(58, "Ce", "Cerium", 140.1f, 1.12f, 2, ElementCategory.Lanthanide, new Color(1f, 1f, 0.78f), 2, 2.70f);
        count += Create(59, "Pr", "Praseodymium", 140.9f, 1.13f, 2, ElementCategory.Lanthanide, new Color(0.85f, 1f, 0.78f), 2, 2.67f);
        count += Create(60, "Nd", "Neodymium", 144.2f, 1.14f, 2, ElementCategory.Lanthanide, new Color(0.78f, 1f, 0.78f), 2, 2.64f);
        count += Create(61, "Pm", "Promethium", 145f, 1.13f, 2, ElementCategory.Lanthanide, new Color(0.64f, 1f, 0.78f), 2, 2.62f);
        count += Create(62, "Sm", "Samarium", 150.4f, 1.17f, 2, ElementCategory.Lanthanide, new Color(0.56f, 1f, 0.78f), 2, 2.59f);
        count += Create(63, "Eu", "Europium", 152.0f, 1.2f, 2, ElementCategory.Lanthanide, new Color(0.38f, 1f, 0.78f), 2, 2.56f);
        count += Create(64, "Gd", "Gadolinium", 157.3f, 1.2f, 2, ElementCategory.Lanthanide, new Color(0.27f, 1f, 0.78f), 2, 2.54f);
        count += Create(65, "Tb", "Terbium", 158.9f, 1.2f, 2, ElementCategory.Lanthanide, new Color(0.19f, 1f, 0.78f), 2, 2.51f);
        count += Create(66, "Dy", "Dysprosium", 162.5f, 1.22f, 2, ElementCategory.Lanthanide, new Color(0.12f, 1f, 0.78f), 2, 2.49f);
        count += Create(67, "Ho", "Holmium", 164.9f, 1.23f, 2, ElementCategory.Lanthanide, new Color(0f, 1f, 0.61f), 2, 2.47f);
        count += Create(68, "Er", "Erbium", 167.3f, 1.24f, 2, ElementCategory.Lanthanide, new Color(0f, 0.9f, 0.46f), 2, 2.45f);
        count += Create(69, "Tm", "Thulium", 168.9f, 1.25f, 2, ElementCategory.Lanthanide, new Color(0f, 0.83f, 0.32f), 2, 2.42f);
        count += Create(70, "Yb", "Ytterbium", 173.0f, 1.1f, 2, ElementCategory.Lanthanide, new Color(0f, 0.75f, 0.22f), 2, 2.40f);
        count += Create(71, "Lu", "Lutetium", 175.0f, 1.27f, 2, ElementCategory.Lanthanide, new Color(0f, 0.67f, 0.14f), 2, 2.25f);
        
        // 72-80
        count += Create(72, "Hf", "Hafnium", 178.5f, 1.3f, 2, ElementCategory.TransitionMetal, new Color(0.3f, 0.76f, 1f), 2, 2.08f);
        count += Create(73, "Ta", "Tantalum", 180.9f, 1.5f, 2, ElementCategory.TransitionMetal, new Color(0.3f, 0.65f, 1f), 2, 2.00f);
        count += Create(74, "W", "Tungsten", 183.8f, 2.36f, 2, ElementCategory.TransitionMetal, new Color(0.13f, 0.58f, 0.84f), 2, 1.93f);
        count += Create(75, "Re", "Rhenium", 186.2f, 1.9f, 2, ElementCategory.TransitionMetal, new Color(0.15f, 0.49f, 0.67f), 2, 1.88f);
        count += Create(76, "Os", "Osmium", 190.2f, 2.2f, 2, ElementCategory.TransitionMetal, new Color(0.15f, 0.4f, 0.59f), 2, 1.85f);
        count += Create(77, "Ir", "Iridium", 192.2f, 2.20f, 2, ElementCategory.TransitionMetal, new Color(0.09f, 0.33f, 0.53f), 2, 1.80f);
        count += Create(78, "Pt", "Platinum", 195.1f, 2.28f, 2, ElementCategory.TransitionMetal, new Color(0.82f, 0.82f, 0.88f), 1, 1.77f);
        count += Create(79, "Au", "Gold", 197.0f, 2.54f, 2, ElementCategory.TransitionMetal, new Color(1f, 0.82f, 0.14f), 1, 1.74f);
        count += Create(80, "Hg", "Mercury", 200.6f, 2.00f, 2, ElementCategory.TransitionMetal, new Color(0.72f, 0.72f, 0.82f), 2, 1.71f);
        
        // 81-86
        count += Create(81, "Tl", "Thallium", 204.4f, 2.33f, 3, ElementCategory.PostTransition, new Color(0.65f, 0.33f, 0.3f), 3, 1.56f);
        count += Create(82, "Pb", "Lead", 207.2f, 2.02f, 4, ElementCategory.PostTransition, new Color(0.34f, 0.35f, 0.38f), 4, 1.54f);
        count += Create(83, "Bi", "Bismuth", 209.0f, 2.02f, 5, ElementCategory.PostTransition, new Color(0.62f, 0.31f, 0.71f), 5, 1.43f);
        count += Create(84, "Po", "Polonium", 209f, 2.0f, 2, ElementCategory.Metalloid, new Color(0.67f, 0.36f, 0f), 6, 1.35f);
        count += Create(85, "At", "Astatine", 210f, 2.2f, 1, ElementCategory.Halogen, new Color(0.46f, 0.31f, 0.27f), 7, 1.27f);
        count += Create(86, "Rn", "Radon", 222f, 0f, 0, ElementCategory.NobleGas, new Color(0.26f, 0.51f, 0.59f), 8, 1.20f);
        
        // 87-88
        count += Create(87, "Fr", "Francium", 223f, 0.7f, 1, ElementCategory.AlkaliMetal, new Color(0.26f, 0f, 0.4f), 1, 3.24f);
        count += Create(88, "Ra", "Radium", 226f, 0.9f, 2, ElementCategory.AlkalineEarth, new Color(0f, 0.49f, 0f), 2, 2.83f);
        
        // 89-92 (Actinides)
        count += Create(89, "Ac", "Actinium", 227f, 1.1f, 2, ElementCategory.Actinide, new Color(0.44f, 0.67f, 0.98f), 2, 2.60f);
        count += Create(90, "Th", "Thorium", 232.0f, 1.3f, 2, ElementCategory.Actinide, new Color(0f, 0.73f, 1f), 2, 2.37f);
        count += Create(91, "Pa", "Protactinium", 231.0f, 1.5f, 2, ElementCategory.Actinide, new Color(0f, 0.63f, 1f), 2, 2.00f);
        count += Create(92, "U", "Uranium", 238.0f, 1.38f, 2, ElementCategory.Actinide, new Color(0f, 0.56f, 1f), 2, 1.96f);
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done!", $"Created {count} elements in {savePath}", "OK");
    }
    
    int Create(int number, string symbol, string name, float mass, float eneg, 
        int maxBonds, ElementCategory cat, Color color, int valence, float radius)
    {
        var asset = ScriptableObject.CreateInstance<PeriodicElementData>();
        asset.atomicNumber = number;
        asset.symbol = symbol;
        asset.elementName = name;
        asset.electronegativity = eneg;
        asset.maxBonds = maxBonds;
        asset.elementColor = color;
        asset.valenceElectrons = valence;
        asset.atomicRadius = radius;
        
        // تبدیل category به isMetal
        asset.isMetal = (cat == ElementCategory.AlkaliMetal || 
                         cat == ElementCategory.AlkalineEarth || 
                         cat == ElementCategory.TransitionMetal ||
                         cat == ElementCategory.Lanthanide ||
                         cat == ElementCategory.Actinide ||
                         cat == ElementCategory.PostTransition);
        
        string path = $"{savePath}/Element_{number:D3}_{symbol}.asset";
        AssetDatabase.CreateAsset(asset, path);
        
        return 1;
    }
}
