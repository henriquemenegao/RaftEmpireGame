using UI;
using UI.Equipment;
using UnityEngine;

namespace Equipment.Core
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Workstation))]
    public class WorkstationInteraction : MonoBehaviour
    {
        [SerializeField] private EquipmentPanelUI equipmentPanel;

        private Workstation workstation;

        private void Awake()
        {
            workstation = GetComponent<Workstation>();
        }

        private void OnMouseDown()
        {
            if (UIManager.Instance == null)
                return;

            if (UIManager.Instance.HasOpenModal)
                return;

            if (equipmentPanel == null)
            {
                Debug.LogWarning(
                    $"EquipmentPanelUI não configurado em {name}."
                );
                return;
            }

            equipmentPanel.Open(workstation);
        }
    }
}