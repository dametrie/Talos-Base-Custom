using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Base;
using Talos.Objects;
using Talos.Structs;
#if IGNORE_THIS_CODE
namespace Talos.Bashing
{
    internal abstract class BashingBase
    {
        protected static readonly List<int> SHINEWOOD_USE_HOLY;
        protected static readonly List<int> SHINEWOOD_USE_DARK;
        protected DateTime LastNeckSwap = DateTime.UtcNow.Subtract(TimeSpan.FromHours(5.0));
        protected DateTime LastPointInSync = DateTime.UtcNow;
        protected DateTime LastDirectionInSync = DateTime.UtcNow;
        protected DateTime LastAssailed = DateTime.UtcNow;
        protected DateTime LastSendTarget = DateTime.UtcNow;
        protected DateTime LastUsedSkill { get; set; } = DateTime.Now;
        protected List<Player> NearbyPlayers { get; set; }
        protected List<Creature> NearbyMonsters { get; set; }
        protected List<Creature> KillableTargets { get; set; }
        protected Bot Bot { get; }
        protected Client Client => Bot.Client;

        protected int MonsterWalkIntervalMs
        {
            get => Convert.ToInt32(Client.ClientTab.monsterWalkIntervalNum1.Value);
        }

        protected int PingCompensation => Convert.ToInt32(Client.ClientTab.pingCompensationNum1.Value);

        protected bool BashAsgall => Client.ClientTab.chkBashAsgall.Checked;

        protected int SkillIntervalMs => Convert.ToInt32(Client.ClientTab.numSkillInt.Value);

        protected int SendTargetIntervalMs { get; set; } = 250;

        protected bool RequireDionForRiskySkills => Client.ClientTab.riskySkillsDionCbox.Checked;

        protected bool UseRiskySkills => Client.ClientTab.riskySkillsCbox.Checked;

        protected bool UseCrasher => Client.ClientTab.crasherCbox.Checked;

        protected HashSet<Location> Warps { get; set; }

        public Creature Target { get; set; }

        protected List<ushort> PrioritySprites { get; set; }

        protected bool PriorityOnly => Client.ClientTab.priorityOnlyCbox.Checked;

        internal BashingBase(Bot bot) => Bot = bot;
        internal void Update()
        {
            NearbyPlayers = Client.GetNearbyPlayers();
            NearbyMonsters = Client.GetNearbyValidCreatures(10);
            Warps = Client.GetAllWarpPoints();

            // Step 2: Set priority sprites
            if (!Client.ClientTab.priorityCbox.Checked)
            {
                PrioritySprites = new List<ushort>();
            }
            else
            {
                PrioritySprites = Client.ClientTab.priorityLbox.Items
                                    .OfType<string>()
                                    .Select(ushort.Parse)
                                    .ToList();
            }

        }

        protected IEnumerable<Player> GetStrangers()
        {
            // Create a HashSet with OrdinalIgnoreCase for fast lookups
            var friendSet = new HashSet<string>(Client._friendBindingList, StringComparer.OrdinalIgnoreCase);

            return NearbyPlayers.Where(user => !friendSet.Contains(user.Name));
        }

        protected virtual IEnumerable<Creature> FilterMonstersByCursedFased(
             IEnumerable<Creature> monsters = null)
        {
            // Use the provided list or default to NearbyMonsters
            var creatureList = monsters ?? NearbyMonsters;

            return creatureList.Where(monster =>
                (BashAsgall || !monster.IsAsgalled) && // Filter based on IsAsgalled
                (!Client.ClientTab.chkWaitForCradhNew.Checked || monster.IsCursed) && // Filter if 'WaitForCradh' is enabled
                (!Client.ClientTab.chkWaitForFasNew.Checked || monster.IsFassed)); // Filter if 'WaitForFas' is enabled
        }


    }
}
#endif
