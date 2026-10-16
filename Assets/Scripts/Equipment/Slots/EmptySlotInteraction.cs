using UI.Construction;
using UnityEngine;

namespace Equipment.Slots
{
    public class EmptySlotInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EquipmentSlot equipmentSlot;
        [SerializeField] private ConstructionPanelUI constructionPanel;

        private void OnMouseDown()
        {
            if (equipmentSlot == null || constructionPanel == null)
            {
                Debug.LogWarning(
                    "EmptySlotInteraction: referências não configuradas.");
                return;
            }

            constructionPanel.Open(equipmentSlot);
        }
    }
}