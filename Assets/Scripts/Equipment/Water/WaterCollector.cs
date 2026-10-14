using Crew;
using Equipment.Core;

namespace Equipment.Water
{
    public class WaterCollector : Workstation
    {
        public override CrewActivity Activity => CrewActivity.CollectingWater;
    }
}