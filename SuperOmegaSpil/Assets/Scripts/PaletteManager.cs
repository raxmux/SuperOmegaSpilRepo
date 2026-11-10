using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Palette
{
    public string name;
    [Header("Color 1 & 2")]
    public Color mainColor12 = Color.white;
    public Color secoColor12 = Color.white;
    [Header("Color 3")]
    public Color mainColor3 = Color.white;
    public Color secoColor3 = Color.white;
}
public class PaletteManager : MonoBehaviour
{
    public static PaletteManager Instance { get; private set; }
    public static event Action PaletteChanged;

    [SerializeField] Material color1;
    [SerializeField] Material color2;
    [SerializeField] Material color3;
    [SerializeField] List<Palette> palettes = new List<Palette>();

    public Palette CurrentPalette { get; private set; }

    static readonly int MainColorId = Shader.PropertyToID("_mainColor");
    static readonly int SecoColorId = Shader.PropertyToID("_secoColor");

    int lastIndex = -1;
    readonly Dictionary<Material, (Color main, Color seco)> originalColors = new Dictionary<Material, (Color, Color)>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (Material m in new[] { color1, color2, color3 })
            if (m != null && !originalColors.ContainsKey(m))
                originalColors[m] = (m.GetColor(MainColorId), m.GetColor(SecoColorId));
    }

    void OnDestroy()
    {
        if (Instance != this) return;

        foreach (var pair in originalColors)
        {
            pair.Key.SetColor(MainColorId, pair.Value.main);
            pair.Key.SetColor(SecoColorId, pair.Value.seco);
        }
    }

    public void RandomizePalette()
    {
        if (palettes.Count == 0) return;

        int index = UnityEngine.Random.Range(0, palettes.Count);
        if (palettes.Count > 1 && index == lastIndex)
            index = (index + UnityEngine.Random.Range(1, palettes.Count)) % palettes.Count;

        lastIndex = index;
        ApplyPalette(palettes[index]);
    }

    public void ApplyPalette(Palette palette)
    {
        CurrentPalette = palette;

        SetColors(color1, palette.mainColor12, palette.secoColor12);
        SetColors(color2, palette.mainColor12, palette.secoColor12);
        SetColors(color3, palette.mainColor3, palette.secoColor3);

        PaletteChanged?.Invoke();
    }

    static void SetColors(Material material, Color main, Color seco)
    {
        if (material == null) return;
        material.SetColor(MainColorId, main);
        material.SetColor(SecoColorId, seco);
    }
}