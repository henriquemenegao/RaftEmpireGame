using Equipment.Core;
using UI.Equipment;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Painéis")]
        [SerializeField] private EquipmentPanelUI equipmentPanel;

        private GameObject activeModal;

        public bool HasOpenModal =>
            activeModal != null && activeModal.activeInHierarchy;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void OpenEquipmentPanel(Workstation workstation)
        {
            if (workstation == null || HasOpenModal)
                return;

            if (equipmentPanel == null)
            {
                Debug.LogWarning(
                    "UIManager: EquipmentPanelUI não configurado."
                );
                return;
            }

            equipmentPanel.Open(workstation);
        }

        public void OpenModal(GameObject modal)
        {
            if (modal == null)
                return;

            if (activeModal != null && activeModal != modal)
                activeModal.SetActive(false);

            activeModal = modal;
            activeModal.SetActive(true);
        }

        public void CloseModal()
        {
            if (activeModal == null)
                return;

            activeModal.SetActive(false);
            activeModal = null;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}