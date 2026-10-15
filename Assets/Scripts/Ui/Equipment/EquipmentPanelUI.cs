
using Equipment.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Equipment
{
    public class EquipmentPanelUI : MonoBehaviour
    {
        [Header("Cabeçalho")]
        [SerializeField] private TMP_Text equipmentTitle;
        [SerializeField] private TMP_Text equipmentSubtitle;

        [Header("Produção")]
        [SerializeField] private Image productionIcon;
        [SerializeField] private TMP_Text productionAmount;

        [Header("Operador")]
        [SerializeField] private TMP_Text operatorName;
        [SerializeField] private TMP_Text operatorEfficiency;

        [Header("Tripulação")]
        [SerializeField] private CrewListUI crewList;

        public Workstation SelectedWorkstation { get; private set; }

        public void Open(Workstation workstation)
        {
            if (workstation == null)
                return;

            SelectedWorkstation = workstation;

            Refresh();
            crewList.Refresh(SelectedWorkstation);

            UIManager.Instance.OpenModal(gameObject);
        }

        public void Refresh()
        {
            if (SelectedWorkstation == null)
                return;

            RefreshHeader();
            RefreshProduction();
            RefreshOperator();
            crewList.RefreshCards();
        }

        private void RefreshHeader()
        {
            equipmentTitle.text = SelectedWorkstation.DisplayName;

            equipmentSubtitle.text =
                $"Nível {SelectedWorkstation.Level} · " +
                SelectedWorkstation.ProductionDescription;
        }

        private void RefreshProduction()
        {
            if (productionIcon != null)
            {
                productionIcon.sprite = SelectedWorkstation.ProductionIcon;
                productionIcon.enabled =
                    SelectedWorkstation.ProductionIcon != null;
            }

            if (productionAmount != null)
            {
                int amount = SelectedWorkstation.BaseProductionAmount;
                float duration = SelectedWorkstation.ProductionDuration;

                productionAmount.text =
                    $"{amount} / {duration:0.#}s";
            }
        }

        private void RefreshOperator()
        {
            var currentOperator = SelectedWorkstation.Operator;

            if (currentOperator == null)
            {
                operatorName.text = "Nenhum";
                operatorEfficiency.text = "";
                return;
            }

            operatorName.text = currentOperator.CrewName;
            operatorEfficiency.text =
                $"Eficiência: {currentOperator.Efficiency}x";
        }

        public void Close()
        {
            UIManager.Instance.CloseModal();
            SelectedWorkstation = null;
        }
    }
}
