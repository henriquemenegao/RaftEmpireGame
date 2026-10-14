using Crew;
using Equipment.Core;
using TMPro;
using UI.Equipment;
using UnityEngine;

namespace UI
{
    public class CrewMemberCardUI : MonoBehaviour
    {
        [Header("Informações do tripulante")]
        [SerializeField] private TMP_Text crewName;
        [SerializeField] private TMP_Text crewEfficiency;
        [SerializeField] private TMP_Text crewStatus;
        
        [Header("Ação")]
        [SerializeField] private UnityEngine.UI.Button crewActionButton;
        
        [SerializeField] private TMP_Text crewActionText;
        [SerializeField] private UnityEngine.UI.Image crewActionBackground;

        private CrewManager crewManager;
        private CrewMember crewMember;
        private Workstation selectedWorkstation;
        private EquipmentPanelUI equipmentPanel;

        public void Initialize(CrewMember member, Workstation workstation)
        {
            if (member == null || workstation == null)
            {
                Debug.LogWarning(
                    "CrewMemberCardUI: tripulante ou equipamento inválido."
                );
                return;
            }

            crewMember = member;
            selectedWorkstation = workstation;
            equipmentPanel = GetComponentInParent<EquipmentPanelUI>(true);

            Debug.Log(
                $"[CrewMemberCardUI] Painel encontrado: {equipmentPanel != null}"
            );
            
            crewManager = FindAnyObjectByType<CrewManager>();

            crewActionButton.onClick.RemoveAllListeners();
            crewActionButton.onClick.AddListener(OnActionClicked);

            crewName.text = member.CrewName;
            crewEfficiency.text = $"Eficiência: {member.Efficiency}x";

            RefreshStatus();
            RefreshAction();
        }
        
        
        private void OnActionClicked()
        {
            if (crewManager == null ||
                crewMember == null ||
                selectedWorkstation == null)
                return;

            if (crewMember.AssignedEquipment == selectedWorkstation)
            {
                crewManager.Unassign(crewMember);
            }
            else
            {
                bool success = crewManager.Assign(
                    crewMember,
                    selectedWorkstation
                );

                if (!success)
                {
                    Debug.LogWarning(
                        $"Não foi possível alocar {crewMember.CrewName}."
                    );
                    return;
                }
            }

            RefreshStatus();
            RefreshAction();
            if (equipmentPanel != null)
                equipmentPanel.Refresh();
        }

        public void Refresh()
        {
            RefreshStatus();
            RefreshAction();
        }

        public void RefreshStatus()
        {
            if (crewMember == null || selectedWorkstation == null)
                return;

            if (crewMember.AssignedEquipment == null)
            {
                crewStatus.text = "Disponível";
            }
            else if (crewMember.AssignedEquipment == selectedWorkstation)
            {
                crewStatus.text = "Operando aqui";
            }
            else
            {
                crewStatus.text = "Ocupado em outra estação";
            }
        }
        
        
        private void RefreshAction()
        {
            if (crewMember == null || selectedWorkstation == null)
                return;
            
            if (selectedWorkstation.HasOperator &&
                selectedWorkstation.Operator != crewMember &&
                crewMember.AssignedEquipment == null)
            {
                crewActionText.text = "Sem vaga";
                crewActionBackground.color = new Color32(140, 130, 110, 255);
                crewActionButton.interactable = false;
                return;
            }

            if (crewMember.AssignedEquipment == null)
            {
                crewActionText.text = "Designar";
                crewActionBackground.color = new Color32(39, 134, 74, 255);
                crewActionButton.interactable = true;
            }
            else if (crewMember.AssignedEquipment == selectedWorkstation)
            {
                crewActionText.text = "Remover";
                crewActionBackground.color = new Color32(201, 48, 36, 255);
                crewActionButton.interactable = true;
            }
            else
            {
                crewActionText.text = "Ocupado";
                crewActionBackground.color = new Color32(140, 130, 110, 255);
                crewActionButton.interactable = false;
            }
        }

    }
}