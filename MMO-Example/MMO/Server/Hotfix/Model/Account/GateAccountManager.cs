using Fantasy;
using Fantasy.Entitas;

namespace BestGame
{
    public sealed class GateAccountManager : Entity
    {
        public readonly Dictionary<long, GateAccount> GateAccounts = new Dictionary<long, GateAccount>();
    }
}