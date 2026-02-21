using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DFNumberSetter : MonoBehaviour
{
    public DF_ConstantNumberComponent numberComponent;
    public TMP_InputField inputField;
    public Button submitButton;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnInputValueChanged);
        submitButton.onClick.AddListener(OnSubmitButtonClicked);
    }
    void Awake()
    {
        if (inputField == null)
            inputField = GetComponentInChildren<TMP_InputField>();

        if (submitButton == null)
            submitButton = GetComponentInChildren<Button>();

        Hide();
    }
    private void OnSubmitButtonClicked()
    {
        if (float.TryParse(inputField.text, out float result))
        {
            numberComponent.SetValue(result);
            Hide();
        }
        else
        {
            Debug.LogWarning($"Invalid input for number: {inputField.text}");
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show(DF_ConstantNumberComponent numberComponent)    {
        this.numberComponent = numberComponent;
        gameObject.SetActive(true);
        inputField.text = numberComponent._value.ToString();
    }

    private void OnInputValueChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            numberComponent.SetValue(result);
        }
        else
        {
            Debug.LogWarning($"Invalid input for number: {value}");
        }
    }
}
