
using UI.Construction;
using UnityEngine;

namespace Equipment.Slots
{
    public class EmptySlotInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EquipmentSlot equipmentSlot;

        private ConstructionPanelUI constructionPanel;

        private void Awake()
        {
            constructionPanel = FindAnyObjectByType<ConstructionPanelUI>(
                FindObjectsInactive.Include);
        }

        private void OnMouseDown()
        {
            if (equipmentSlot == null)
            {
                Debug.LogWarning(
                    "EmptySlotInteraction: EquipmentSlot não configurado.");
                return;
            }

            if (constructionPanel == null)
            {
                Debug.LogWarning(
                    "EmptySlotInteraction: ConstructionPanelUI não encontrado.");
                return;
            }

            Debug.Log($"Abrindo construção para o slot: {equipmentSlot.SlotId}");

            constructionPanel.Open(equipmentSlot);
        }
    }
}
