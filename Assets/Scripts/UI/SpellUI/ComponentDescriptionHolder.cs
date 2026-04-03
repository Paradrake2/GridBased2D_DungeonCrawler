using UnityEngine;
using TMPro;
public class ComponentDescriptionHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    public void SetDescription(SpellComponent sc)
    {
        descriptionText.text = sc.Description.GetDescription(sc);
    }
    void Start()
    {
        descriptionText.text = "";
    }
}
