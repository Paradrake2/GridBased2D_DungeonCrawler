using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttributePickerUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Dropdown dropdown;

    private readonly List<StatType> options = new();
    private bool isInitializing;
    private Action<StatType> onSelected;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (dropdown == null)
            dropdown = GetComponentInChildren<TMP_Dropdown>(true);

        Hide();

        if (dropdown != null)
            dropdown.onValueChanged.AddListener(HandleDropdownValueChanged);
    }

    private void OnDestroy()
    {
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(HandleDropdownValueChanged);
    }

    public void Show(StatType currentValue, Action<StatType> onSelectedCallback)
    {
        onSelected = onSelectedCallback;
        RebuildOptions();

        if (dropdown == null)
        {
            Debug.LogError("AttributePickerUI missing TMP_Dropdown reference.");
            return;
        }

        isInitializing = true;
        try
        {
            int index = 0;
            if (currentValue != null)
            {
                int found = options.IndexOf(currentValue);
                if (found >= 0) index = found;
            }

            dropdown.SetValueWithoutNotify(index);
        }
        finally
        {
            isInitializing = false;
        }

        if (root != null)
            root.SetActive(true);
    }

    public void Hide()
    {
        onSelected = null;
        if (root != null)
            root.SetActive(false);
    }

    private void RebuildOptions()
    {
        options.Clear();

        StatDatabase db = StatDatabase.Instance;
        if (db != null)
            options.AddRange(db.GetStatsByCategory(StatCategory.Attribute));

        if (dropdown == null) return;

        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> optionData = new();
        for (int i = 0; i < options.Count; i++)
        {
            StatType stat = options[i];
            if (stat == null) continue;

            string label = string.IsNullOrEmpty(stat.displayName) ? stat.name : stat.displayName;
            optionData.Add(new TMP_Dropdown.OptionData(label, stat.icon, Color.white));
        }
        dropdown.AddOptions(optionData);

        if (optionData.Count == 0)
            dropdown.AddOptions(new List<string> { "No attributes" });
    }

    private void HandleDropdownValueChanged(int index)
    {
        if (isInitializing) return;
        if (index < 0 || index >= options.Count) return;

        StatType selected = options[index];
        if (selected == null) return;

        onSelected?.Invoke(selected);
    }
}
