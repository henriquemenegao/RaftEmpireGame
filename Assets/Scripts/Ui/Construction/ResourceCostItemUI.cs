using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Construction
{
    public class ResourceCostItemUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image resourceIcon;
        [SerializeField] private TMP_Text amountText;

        public void Configure(Sprite icon, int amount)
        {
            if (resourceIcon != null)
            {
                resourceIcon.sprite = icon;
                resourceIcon.enabled = icon != null;
            }

            if (amountText != null)
                amountText.text = amount.ToString();
        }
    }
}