using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                        var dionStateUpdates = new Dictionary<CreatureState, object> 
                        {
                            { CreatureState.IsDioned, true },
                            { CreatureState.LastDioned, DateTime.UtcNow },
                            { CreatureState.DionName, "Perfect Defense" },
                            { CreatureState.DionDuration, Spell.GetSpellDuration("Perfect Defense") }
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, dionStateUpdates);

                    }
                    break;

                case (ushort)SpellAnimation.Dion:
                    if (_targetID == _sourceID && _targetID != _client.PlayerID && !_server._clientList.Any(c => c.PlayerID == _targetID))
                    {
                        var dionStateUpdates = new Dictionary<CreatureState, object>
                        {
                            { CreatureState.IsDioned, true },
                            { CreatureState.LastDioned, DateTime.UtcNow },
                            { CreatureState.DionName, "Dion" },
                            { CreatureState.DionDuration, Spell.GetSpellDuration("Dion") }
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, dionStateUpdates);
                    }
                    break;

                case (ushort)SpellAnimation.WingsOfProtection:
                    if (_targetID == _sourceID)
                    {
                        var dionStateUpdates = new Dictionary<CreatureState, object>
                        {
                            { CreatureState.IsDioned, true },
                            { CreatureState.LastDioned, DateTime.UtcNow },
                            { CreatureState.DionName, "Wings of Protection" },
                            { CreatureState.DionDuration, Spell.GetSpellDuration("Wings of Protection") }
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, dionStateUpdates);
                    }
                    break;

                case (ushort)SpellAnimation.IronSkin:
                    if (_targetID == _sourceID)
                    {
                        var dionStateUpdates = new Dictionary<CreatureState, object>
                        {
                            { CreatureState.IsDioned, true },
                            { CreatureState.LastDioned, DateTime.UtcNow },
                            { CreatureState.DionName, "Iron Skin" },
                            { CreatureState.DionDuration, Spell.GetSpellDuration("Iron Skin") }
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, dionStateUpdates);

                    }
                    break;

                case (ushort)SpellAnimation.MorDionComhla:
                    if (_targetID != _sourceID)
                    {
                        var dionStateUpdates = new Dictionary<CreatureState, object>
                        {
                            { CreatureState.IsDioned, true },
                            { CreatureState.LastDioned, DateTime.UtcNow },
                            { CreatureState.DionName, "Mor Dion Comhla" },
                            { CreatureState.DionDuration, Spell.GetSpellDuration("Mor Dion Comhla") }
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, dionStateUpdates);
                    }
                    break;

                case (ushort)SpellAnimation.Aite:
                    if ((_sourceID != _client.Player.ID) || _client._spellHistory.Count <= 0) 
                    {
                        var aiteStateUpdates = new Dictionary<CreatureState, object> 
                        {
                            { CreatureState.IsAited, true },
                            { CreatureState.LastAited, DateTime.UtcNow },
                            { CreatureState.AiteName, "ard naomh aite" },
                            { CreatureState.AiteDuration, Spell.GetSpellDuration("ard naomh aite") } //we didn't cast it or our creature to spell list is empty
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, aiteStateUpdates);
                    }
                    else
                    {
                        var aiteStateUpdates = new Dictionary<CreatureState, object>
                        {
                            { CreatureState.IsAited, true },
                            { CreatureState.LastAited, DateTime.UtcNow },
                            { CreatureState.AiteName, _client._spellHistory[0].Spell.Name },
                            { CreatureState.AiteDuration, Spell.GetSpellDuration(_client._spellHistory[0].Spell.Name) }
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, aiteStateUpdates);
                    }
                    break;

                case (ushort)SpellAnimation.DeireasFaileas:
                    break;

                case (ushort)SpellAnimation.Fas:
                    if ((_sourceID != _client.Player.ID) || (_client._spellHistory.Count <= 0)) //we didn't cast it or our creature to spell list is empty
                    {

                        double fasDuration = Spell.GetSpellDuration("ard fas nadur");

                        var fasStateUpdate = new Dictionary<CreatureState, object> //Adam new
                        {
                            { CreatureState.IsFassed, true },
                            { CreatureState.LastFassed, DateTime.UtcNow },
                            { CreatureState.FasName, "ard fas nadur" },//we didnt cast it so we assume it's max?
                            { CreatureState.FasDuration, fasDuration } // Duration in seconds
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, fasStateUpdate);


                        //_targetCreature.FasDuration = Spell.GetSpellDuration("ard fas nadur");
                        //_targetCreature.LastFassed = DateTime.UtcNow;
                    }
                    else
                    {

                        string spellName = _client._spellHistory[0].Spell.Name;
                        double fasDuration = Spell.GetSpellDuration(spellName);

                        var fasStateUpdate = new Dictionary<CreatureState, object> //Adam new
                        {
                            { CreatureState.IsFassed, true },
                            { CreatureState.LastFassed, DateTime.UtcNow },
                            { CreatureState.FasName, spellName },
                            { CreatureState.FasDuration, fasDuration } // Duration in seconds
                        };

                        CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, fasStateUpdate);

                        //_targetCreature.FasDuration = Spell.GetSpellDuration(_client._spellHistory[0].Spell.Name);
                        //_targetCreature.LastFassed = DateTime.UtcNow;
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

                    double bcDuration = Spell.GetSpellDuration("beag cradh");

                    var bcStateUpdates = new Dictionary<CreatureState, object> //Adam new
                    {
                        { CreatureState.IsCursed, true },
                        { CreatureState.LastCursed, DateTime.UtcNow },
                        { CreatureState.CurseName, "beag cradh" },
                        { CreatureState.CurseDuration, bcDuration } // Duration in seconds
                    };

                    CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, bcStateUpdates);
                    
                    break;

                case (ushort)SpellAnimation.Cradh:
                case (ushort)SpellAnimation.OrangeCloud:

                    double cDuration = Spell.GetSpellDuration("cradh");

                    var cStateUpdates = new Dictionary<CreatureState, object> //Adam new
                    {
                        { CreatureState.IsCursed, true },
                        { CreatureState.LastCursed, DateTime.UtcNow },
                        { CreatureState.CurseName, "cradh" },
                        { CreatureState.CurseDuration, cDuration } // Duration in seconds
                    };

                    CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, cStateUpdates);

                    break;

                case (ushort)SpellAnimation.MorCradh:
                case (ushort)SpellAnimation.BlackCloud:

                    double mcDuration = Spell.GetSpellDuration("mor cradh");

                    var mcStateUpdates = new Dictionary<CreatureState, object> //Adam new
                    {
                        { CreatureState.IsCursed, true },
                        { CreatureState.LastCursed, DateTime.UtcNow },
                        { CreatureState.CurseName, "mor cradh" },
                        { CreatureState.CurseDuration, mcDuration } // Duration in seconds
                    };

                    CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, mcStateUpdates);

                    break;

                case (ushort)SpellAnimation.ArdCradh:
                case (ushort)SpellAnimation.RedCloud:

                    double acDuration = Spell.GetSpellDuration("ard cradh");

                    var acStateUpdates = new Dictionary<CreatureState, object> //Adam new
                    {
                        { CreatureState.IsCursed, true },
                        { CreatureState.LastCursed, DateTime.UtcNow },
                        { CreatureState.CurseName, "ard cradh" },
                        { CreatureState.CurseDuration, acDuration } // Duration in seconds
                    };

                    CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, acStateUpdates);

                    //_targetCreature.Curse = "ard cradh";
                    //_targetCreature.CurseDuration = Spell.GetSpellDuration(_targetCreature.Curse);
                    //_targetCreature.LastCursed = DateTime.UtcNow;
                    
                    Console.WriteLine($"[AnimationHandler] curse duration set on Animation. Duration: {_targetCreature.GetState<double>(CreatureState.CurseDuration)}, LastCursed: {_targetCreature.GetState<DateTime>(CreatureState.LastCursed)}");
                    
                    break;

                case (ushort)SpellAnimation.Demise:

                    double demiseDuration = Spell.GetSpellDuration("Demise");

                    var demiseStateUpdates = new Dictionary<CreatureState, object> //Adam new
                    {
                        { CreatureState.IsCursed, true },
                        { CreatureState.LastCursed, DateTime.UtcNow },
                        { CreatureState.CurseName, "Demise" },
                        { CreatureState.CurseDuration, demiseDuration } // Duration in seconds
                    };

                    CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, demiseStateUpdates);

                    break;

                case (ushort)SpellAnimation.DarkerSeal: //Adam how do we handle demon seal

                    double darkerSealDuration = Spell.GetSpellDuration("Darker Seal");

                    var darkerSealStateUpdates = new Dictionary<CreatureState, object> //Adam new
                    {
                        { CreatureState.IsCursed, true },
                        { CreatureState.LastCursed, DateTime.UtcNow },
                        { CreatureState.CurseName, "Darker Seal" },
                        { CreatureState.CurseDuration, darkerSealDuration } // Duration in seconds
                    };

                    CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, darkerSealStateUpdates);

                    break;

                case (ushort)SpellAnimation.DarkSeal:

                    double darkSealDuration = Spell.GetSpellDuration("Dark Seal");

                    var darkSealStateUpdates = new Dictionary<CreatureState, object> //Adam new
                    {
                        { CreatureState.IsCursed, true },
                        { CreatureState.LastCursed, DateTime.UtcNow },
                        { CreatureState.CurseName, "Dark Seal" },
                        { CreatureState.CurseDuration, darkSealDuration } // Duration in seconds
                    };

                    CreatureStateHelper.UpdateCreatureStates(_client, _targetCreature.ID, darkSealStateUpdates);

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
