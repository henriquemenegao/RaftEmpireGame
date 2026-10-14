using Equipment.Fishing;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class FishingProgressUI : MonoBehaviour
    {
        [SerializeField] private FishingStation fishingStation;
        [SerializeField] private Image progressFill;
        [SerializeField] private GameObject progressVisual;

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
            if (fishingStation == null ||
                progressFill == null ||
                progressVisual == null)
                return;

            bool isProducing = fishingStation.IsProducing;

            if (progressVisual.activeSelf != isProducing)
                progressVisual.SetActive(isProducing);

            if (isProducing)
                progressFill.fillAmount = fishingStation.Progress;
        }
    }
}