using System;
using System.Collections.Generic;
using Equipment.Progression;
using UnityEngine;

namespace Equipment.Slots
{
    [Serializable]
    public class EquipmentSlotOption
    {
        [Header("Equipment")]
        public EquipmentDefinition Equipment;

        [Header("Placement")]
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
    }

    public class EquipmentSlot : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private string slotId;

        [Header("Allowed Equipment")]
        [SerializeField]
        private List<EquipmentSlotOption> allowedEquipments = new();

        [Header("Visual")]
        [SerializeField] private GameObject emptySlotVisual;

        public string SlotId => slotId;

        public IReadOnlyList<EquipmentSlotOption> AllowedEquipments =>
            allowedEquipments;

        public bool Allows(EquipmentDefinition equipment)
        {
            if (equipment == null)
                return false;

            return allowedEquipments.Exists(
                option => option.Equipment == equipment);
        }

        public bool TryGetPlacement(
            EquipmentDefinition equipment,
            out Vector3 localPosition,
            out Quaternion localRotation)
        {
            foreach (var option in allowedEquipments)
            {
                if (option.Equipment != equipment)
                    continue;

                localPosition = option.LocalPosition;
                localRotation = Quaternion.Euler(option.LocalRotation);
                return true;
            }

            localPosition = Vector3.zero;
            localRotation = Quaternion.identity;
            return false;
        }

        public void SetEmptyVisual(bool visible)
        {
            if (emptySlotVisual != null)
                emptySlotVisual.SetActive(visible);
        }
    }
}