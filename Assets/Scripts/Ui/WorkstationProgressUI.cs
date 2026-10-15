using Equipment.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class WorkstationProgressUI : MonoBehaviour
    {
        [SerializeField] private Workstation workstation;
        [SerializeField] private Image progressFill;
        [SerializeField] private GameObject progressVisual;

        private void Awake()
        {
            if (workstation == null)
                workstation = GetComponentInParent<Workstation>();
        }

        private void Start()
        {
            UpdateProgress();
        }

        private void Update()
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (workstation == null ||
                progressFill == null ||
                progressVisual == null)
                return;

            bool isProducing = workstation.IsProducing;

            if (progressVisual.activeSelf != isProducing)
                progressVisual.SetActive(isProducing);

            if (isProducing)
                progressFill.fillAmount = workstation.Progress;
        }
    }
}