
using System;
using System.Collections.Generic;
using Equipment.Progression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Construction
{
    public class ConstructionEquipmentCardUI : MonoBehaviour
    {
        [Header("Equipment")]
        [SerializeField] private Image equipmentIcon;
        [SerializeField] private TMP_Text equipmentName;
        [SerializeField] private TMP_Text equipmentDescription;

        [Header("Costs")]
        [SerializeField] private Transform costsContainer;
        [SerializeField] private ResourceCostItemUI costItemPrefab;

        [Header("Ownership")]
        [SerializeField] private TMP_Text ownershipInfo;

        [Header("Actions")]
        [SerializeField] private Button actionButton;
        [SerializeField] private TMP_Text statusMessage;

        private readonly List<ResourceCostItemUI> spawnedCosts = new();

        public void Configure(
            EquipmentDefinition definition,
            int ownedCount,
            int maxCount,
            EquipmentBuildStatus buildStatus,
            bool canAfford,
            IReadOnlyDictionary<Resources.ResourceType, Sprite> resourceIcons,
            Action onBuild)
        {
            if (definition == null)
                return;

            equipmentName.text = definition.DisplayName;
            equipmentDescription.text = definition.Description;

            equipmentIcon.sprite = definition.Icon;
            equipmentIcon.enabled = definition.Icon != null;

            ownershipInfo.text = $"Possui: {ownedCount} de {maxCount}";

            bool available =
                buildStatus == EquipmentBuildStatus.Available;

            actionButton.interactable = available && canAfford;

            actionButton.onClick.RemoveAllListeners();

            if (onBuild != null)
                actionButton.onClick.AddListener(() => onBuild());

            string message = "";

            if (buildStatus == EquipmentBuildStatus.LimitReached)
                message = "Continue evoluindo seu barco para construir mais";
            else if (buildStatus == EquipmentBuildStatus.Locked)
                message = "Equipamento ainda não desbloqueado";
            else if (buildStatus == EquipmentBuildStatus.InvalidConfiguration)
                message = "Equipamento indisponível";
            else if (!canAfford)
                message = "Recursos insuficientes";

            statusMessage.text = message;
            statusMessage.gameObject.SetActive(!string.IsNullOrEmpty(message));

            foreach (var item in spawnedCosts)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }

            spawnedCosts.Clear();

            var levelOne = definition.GetLevel(1);

            if (levelOne == null || levelOne.Costs == null)
                return;

            foreach (var cost in levelOne.Costs)
            {
                if (cost == null)
                    continue;

                Sprite icon = null;

                resourceIcons?.TryGetValue(cost.Resource, out icon);

                var item = Instantiate(costItemPrefab, costsContainer);
                item.Configure(icon, cost.Amount);

                spawnedCosts.Add(item);
            }
        }
    }
}
