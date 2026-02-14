using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpellNameSelector : MonoBehaviour
{
    public TMP_InputField spellNameInputField;

    [SerializeField] private Button submitButton;

    private Action<string> _onSubmit;

    private void Awake()
    {
        // If you assign submitButton in the prefab, this ensures Submit() actually runs.
        if (submitButton != null)
            submitButton.onClick.AddListener(Submit);
    }

    public void Initialize(Action<string> onSubmit)
    {
        _onSubmit = onSubmit;
    }

    public void Submit()
    {
        string spellName = spellNameInputField != null ? spellNameInputField.text : string.Empty;

        if (!string.IsNullOrWhiteSpace(spellName))
        {
            _onSubmit?.Invoke(spellName);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Spell name cannot be empty!");
        }
    }
}
