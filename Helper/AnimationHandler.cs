using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talos.Base;
using Talos.Definitions;
using Talos.Enumerations;
using Talos.Objects;

namespace Talos.Helper
{
    internal class AnimationHandler
    {
        private Client _client;
        private Server _server;
        private Creature _targetCreature;
        private Creature _sourceCreature;
        private ushort _targetAnimation;
        private int _targetID;
        private int _sourceID;

        public AnimationHandler(Client client,Server server, Creature targetCreature, Creature sourceCreature, ushort targetAnimation, int targetID, int sourceID)
        {
            _client = client;
            _server = server;
            _targetCreature = targetCreature;
            _sourceCreature = sourceCreature;
            _targetAnimation = targetAnimation;
            _targetID = targetID;
            _sourceID = sourceID;
        }

        public void HandleAnimation()
        {
            switch (_targetAnimation)
            {
                case (ushort)SpellAnimation.PND:
                case (ushort)SpellAnimation.MPND:
                    _targetCreature._hitCounter++;
                    _targetCreature.HealthPercent = 0;
                    break;

                case (ushort)SpellAnimation.PerfectDefense:
                    if (_targetCreature is Player)
                    {
                        _targetCreature.LastDioned = DateTime.UtcNow;
                        _targetCreature.Dion = "Perfect Defense";
                        _targetCreature.DionDuration = Spell.GetSpellDuration(_targetCreature.Dion);
                    }
                    break;
                case (ushort)SpellAnimation.Dion:
                    if (_targetID == _sourceID && _targetID != _client.PlayerID && !_server._clientList.Any(c => c.PlayerID == _targetID))
                    {
                        _targetCreature.LastDioned = DateTime.UtcNow;
                        _targetCreature.Dion = "mor dion";
                        _targetCreature.DionDuration = Spell.GetSpellDuration(_targetCreature.Dion);
                    }
                    break;
                case (ushort)SpellAnimation.WingsOfProtection:
                    if (_targetID == _sourceID)
                    {
                        _targetCreature.LastDioned = DateTime.UtcNow;
                        _targetCreature.Dion = "Wings of Protection";
                        _targetCreature.DionDuration = Spell.GetSpellDuration(_targetCreature.Dion);
                    }
                    break;
                case (ushort)SpellAnimation.IronSkin:
                    if (_targetID == _sourceID)
                    {
                        _targetCreature.LastDioned = DateTime.UtcNow;
                        _targetCreature.Dion = "Iron Skin";
                        _targetCreature.DionDuration = Spell.GetSpellDuration(_targetCreature.Dion);
                    }
                    break;
                case (ushort)SpellAnimation.MorDionComhla:
                    if (_targetID != _sourceID)
                    {
                        _targetCreature.LastDioned = DateTime.UtcNow;
                        _targetCreature.Dion = "mor dion comlha";
                        _targetCreature.DionDuration = Spell.GetSpellDuration(_targetCreature.Dion);
                    }
                    break;

                case (ushort)SpellAnimation.Aite:
                    if ((_sourceID != _client.Player.ID) || _client._creatureToSpellList.Count <= 0) //we didn't cast it or our creature to spell list is empty
                    {
                        _targetCreature.AiteDuration = Spell.GetSpellDuration("ard naomh aite");
                        _targetCreature.LastAited = DateTime.UtcNow;
                    }
                    else
                    {
                        _targetCreature.AiteDuration = Spell.GetSpellDuration(_client._creatureToSpellList[0].Spell.Name);
                        _targetCreature.LastAited = DateTime.UtcNow;
                    }
                    break;

                case (ushort)SpellAnimation.DeireasFaileas:
                    break;

                case (ushort)SpellAnimation.Fas:
                    if ((_sourceID != _client.Player.ID) || (_client._creatureToSpellList.Count <= 0)) //we didn't cast it or our creature to spell list is empty
                    {
                        _targetCreature.FasDuration = Spell.GetSpellDuration("ard fas nadur");
                        _targetCreature.LastFassed = DateTime.UtcNow;
                    }
                    else
                    {
                        _targetCreature.FasDuration = Spell.GetSpellDuration(_client._creatureToSpellList[0].Spell.Name);
                        _targetCreature.LastFassed = DateTime.UtcNow;
                    }
                    break;

                case (ushort)SpellAnimation.Rescue:
                    if (ReferenceEquals(_targetCreature, _client.Player))
                    {
                        _client.Bot._hasRescue = true;
                    }
                    break;
                case (ushort)SpellAnimation.AoSith:
                    if ((_sourceCreature != null) && !(_sourceCreature is Player))
                    {
                        if (_client.Bot.IsStrangerNearby())
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(_ => _server.ResetBuffsOnAoSith(_client, _targetID, true)));
                        }
                        else
                        {
                            _server.ResetBuffsOnAoSith(_client, _targetID, false);
                        }
                    }
                    break;

                case (ushort)SpellAnimation.BeagCradh:
                case (ushort)SpellAnimation.BlueCloud:
                    _targetCreature.Curse = "beag cradh";
                    _targetCreature.CurseDuration = Spell.GetSpellDuration(_targetCreature.Curse);
                    _targetCreature.LastCursed = DateTime.UtcNow;
                    break;

                case (ushort)SpellAnimation.Cradh:
                case (ushort)SpellAnimation.OrangeCloud:
                    _targetCreature.Curse = "cradh";
                    _targetCreature.CurseDuration = Spell.GetSpellDuration(_targetCreature.Curse);
                    _targetCreature.LastCursed = DateTime.UtcNow;
                    break;

                case (ushort)SpellAnimation.MorCradh:
                case (ushort)SpellAnimation.BlackCloud:
                    _targetCreature.Curse = "mor cradh";
                    _targetCreature.CurseDuration = Spell.GetSpellDuration(_targetCreature.Curse);
                    _targetCreature.LastCursed = DateTime.UtcNow;
                    break;

                case (ushort)SpellAnimation.ArdCradh:
                case (ushort)SpellAnimation.RedCloud:
                    _targetCreature.Curse = "ard cradh";
                    _targetCreature.CurseDuration = Spell.GetSpellDuration(_targetCreature.Curse);
                    _targetCreature.LastCursed = DateTime.UtcNow;
                    break;

                case (ushort)SpellAnimation.Demise:
                    _targetCreature.Curse = "Demise";
                    _targetCreature.CurseDuration = Spell.GetSpellDuration(_targetCreature.Curse);
                    _targetCreature.LastCursed = DateTime.UtcNow;
                    break;

                case (ushort)SpellAnimation.DarkerSeal:
                    _targetCreature.Curse = "Darker Seal";
                    _targetCreature.CurseDuration = Spell.GetSpellDuration(_targetCreature.Curse);
                    _targetCreature.LastCursed = DateTime.UtcNow;
                    break;

                case (ushort)SpellAnimation.DarkSeal:
                    _targetCreature.Curse = "Dark Seal";
                    _targetCreature.CurseDuration = Spell.GetSpellDuration(_targetCreature.Curse);
                    _targetCreature.LastCursed = DateTime.UtcNow;
                    break;

                case (ushort)SpellAnimation.AssassinStrike:
                case (ushort)SpellAnimation.Cupping:
                    if (_targetCreature is Player && !(_sourceCreature is Player))
                    {
                        Player targetPlayer = _targetCreature as Player;
                        targetPlayer.NeedsHeal = true;
                    }
                    break;

                case (ushort)SpellAnimation.Kelb:
                    if (_sourceCreature is Player)
                    {
                        Player targetPlayer = _sourceCreature as Player;
                        targetPlayer.NeedsHeal = true;
                    }
                    break;

                case (ushort)SpellAnimation.Crasher:
                    bool isSourceCreatureNull = _sourceCreature == null;
                    bool isTargetPlayer = _targetCreature is Player;
                    bool isSourceNotPlayer = !(_sourceCreature is Player);

                    if (isTargetPlayer)
                    {
                        Player targetPlayer = _targetCreature as Player;
                        targetPlayer.NeedsHeal = true;

                        if (isSourceCreatureNull || isSourceNotPlayer)
                        {
                            _client._recentlyCrashered = isSourceCreatureNull;
                        }
                        else if (_client._recentlyCrashered)
                        {
                            _client._recentlyCrashered = false;
                        }
                    }
                    break;

                case (ushort)SpellAnimation.MadSoul:
                    if (_sourceCreature is Player)
                    {
                        Player targetPlayer = _sourceCreature as Player;
                        targetPlayer.NeedsHeal = true;
                    }
                    else if ((_sourceCreature != null) && (_targetCreature is Player))
                    {
                        Player targetPlayer = _targetCreature as Player;
                        targetPlayer.NeedsHeal = true;
                    }
                    break;

                case (ushort)SpellAnimation.FasSpiorad:
                    if (_targetCreature is Player && !CONSTANTS.KNOWN_RANGERS.Contains(_targetCreature.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        Player targetPlayer = _targetCreature as Player;
                        targetPlayer.NeedsHeal = true;
                    }
                    break;

                case (ushort)SpellAnimation.RedPotion:
                    if (_targetCreature is Player && _sourceCreature != null && _sourceCreature is Player || CONSTANTS.GREEN_BOROS.Contains(_sourceCreature.SpriteID) || CONSTANTS.RED_BOROS.Contains(_sourceCreature.SpriteID))
                    {
                        Player targetPlayer = _targetCreature as Player;
                        if (targetPlayer.IsSkulled)
                        {
                            targetPlayer.SpellAnimationHistory[(ushort)SpellAnimation.Skull] = DateTime.MinValue;
                        }
                        if (_targetID == _client.Player.ID)
                        {
                            _client.Bot._skullTime = DateTime.MinValue;
                        }
                        targetPlayer.NeedsHeal = true;
                    }
                    break;

            }
        }
    }
}
