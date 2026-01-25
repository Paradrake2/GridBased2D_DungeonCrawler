using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillNodeButton : MonoBehaviour
{
    [SerializeField] private SkillNode skillNode;
    [SerializeField] private Image overlay;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;
    public void OnButtonClick()
    {
        skillNode.AcquireSkill();
        RefreshButton();
    }
    void Start()
    {
        skillNode = Instantiate(skillNode);
        RefreshButton();
        iconImage.sprite = skillNode.Icon;
        SetupCostText();
    }
    private void SetupCostText()
    {
        costText.text = skillNode.SkillPointCost.ToString();
    }
    public void RefreshButton()
    {
        if (skillNode.Acquired == true)
        {
            overlay.enabled = true;
        }
        else
        {
            overlay.enabled = false;
        }
    }
}
