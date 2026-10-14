using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

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