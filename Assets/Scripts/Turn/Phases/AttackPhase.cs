using map;
using player;

namespace Turn.Phases
{
    public class AttackPhase : IPhase
    {
        private readonly GameManager _gm;
        private readonly ContinentRepository _cr;
        private readonly TerritoryRepository _tr;

        public AttackPhase(GameManager gameManager, ContinentRepository continentRepository, TerritoryRepository territoryRepository)
        {
            _gm = gameManager;
            _cr = continentRepository;
            _tr = territoryRepository;
        }


        public void Start(Player player)
        {
            _gm.NextTurnPhase(); // TODO: remove this line
            throw new System.NotImplementedException();
        }

        public void OnAction(Player player, PlayerAction action)
        {
            throw new System.NotImplementedException();
        }

        public void End(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}