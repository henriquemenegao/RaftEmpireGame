using UI;
using UnityEngine;

namespace Equipment.Core
{
    [RequireComponent(typeof(Collider))]
    public class WorkstationInteraction : MonoBehaviour
    {
        private Workstation workstation;

        private void Awake()
        {
            workstation = GetComponent<Workstation>();

            if (workstation == null)
            {
                Debug.LogError(
                    $"Workstation não encontrada em {name}."
                );
            }
        }

        private void OnMouseDown()
        {
            if (workstation == null || UIManager.Instance == null)
                return;

            UIManager.Instance.OpenEquipmentPanel(workstation);
        }
    }
}