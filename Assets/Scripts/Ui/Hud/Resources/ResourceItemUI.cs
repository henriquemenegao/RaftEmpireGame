using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hud.Resources
{
    public class ResourceItemUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image resourceIcon;
        [SerializeField] private TMP_Text resourceAmount;

        public void SetResource(Sprite icon, int amount)
        {
            SetIcon(icon);
            SetAmount(amount);
        }

        public void SetIcon(Sprite icon)
        {
            if (resourceIcon == null)
                return;

            resourceIcon.sprite = icon;
            resourceIcon.enabled = icon != null;
        }

        public void SetAmount(int amount)
        {
            if (resourceAmount == null)
                return;

            resourceAmount.text = FormatAmount(amount);
        }

        private static string FormatAmount(int amount)
        {
            if (amount < 1000)
                return amount.ToString();

            if (amount < 1_000_000)
                return FormatCompact(amount / 1000f, "K");

            if (amount < 1_000_000_000)
                return FormatCompact(amount / 1_000_000f, "M");

            return FormatCompact(amount / 1_000_000_000f, "B");
        }

        private static string FormatCompact(float value, string suffix)
        {
            return value >= 100
                ? $"{Mathf.FloorToInt(value)}{suffix}"
                : $"{Mathf.Floor(value * 10f) / 10f:0.#}{suffix}";
        }
    }
}