
using System.Collections.Generic;
using Equipment.Progression;
using Equipment.Slots;
using TMPro;
using UnityEngine;

namespace UI.Construction
{
    public class ConstructionPanelUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EquipmentInventory equipmentInventory;
        [SerializeField] private RaftLevelDefinition raftLevel;
        [SerializeField] private ConstructionEquipmentCardUI cardPrefab;
        [SerializeField] private Transform buildContent;

        [Header("Resource Icons")]
        [SerializeField] private List<ResourceIconEntry> resourceIcons = new();

        private EquipmentSlot selectedSlot;
        private readonly List<ConstructionEquipmentCardUI> spawnedCards = new();
        private Dictionary<Resources.ResourceType, Sprite> iconLookup;

        private void Awake()
        {
            iconLookup = new Dictionary<Resources.ResourceType, Sprite>();
            foreach (var entry in resourceIcons)
            {
                if (entry == null)
                    continue;

                iconLookup[entry.Resource] = entry.Icon;
            }
        }

        public void Open(EquipmentSlot slot)
        {
            if (slot == null)
                return;

            selectedSlot = slot;
            gameObject.SetActive(true);

            RefreshBuildList();
        }

        public void RefreshBuildList()
        {
            foreach (var card in spawnedCards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }

            spawnedCards.Clear();

            if (selectedSlot == null ||
                equipmentInventory == null ||
                raftLevel == null ||
                cardPrefab == null ||
                buildContent == null)
            {
                Debug.LogWarning(
                    "ConstructionPanelUI: referências incompletas.");
                return;
            }

            foreach (var option in selectedSlot.AllowedEquipments)
            {
                var definition = option.Equipment;

                if (definition == null)
                    continue;

                int owned = equipmentInventory.Count(definition.EquipmentId);
                int max = raftLevel.GetMaxEquipmentInstances(definition);

                var status = EquipmentBuildValidator.Validate(
                    definition,
                    raftLevel,
                    equipmentInventory);

                var card = Instantiate(cardPrefab, buildContent);

                card.Configure(
                    definition,
                    owned,
                    max,
                    status,
                    iconLookup);

                spawnedCards.Add(card);
            }
        }
    }
}
