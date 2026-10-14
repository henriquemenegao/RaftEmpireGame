using Equipment.Core;
using TMPro;
using UnityEngine;

namespace UI.Equipment
{
    public class EquipmentPanelUI : MonoBehaviour
    {
        [Header("Cabeçalho")]
        [SerializeField] private TMP_Text equipmentTitle;
        [SerializeField] private TMP_Text equipmentSubtitle;
        
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
            RefreshOperator();
            crewList.RefreshCards();
        }


        private void RefreshHeader()
        {
            if (SelectedWorkstation == null)
                return;

            equipmentTitle.text = SelectedWorkstation.DisplayName;

            equipmentSubtitle.text =
                $"Nível {SelectedWorkstation.Level} · " +
                SelectedWorkstation.ProductionDescription;
        }
        
        private void RefreshOperator()
        {
            if (SelectedWorkstation == null)
                return;

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