using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Objects
{
    internal sealed class Skillbook : IEnumerable
    {

        private Dictionary<string, Skill> SkillbookDictionary { get; } = new Dictionary<string, Skill>();
        private Skill?[] SkillArray { get; } = new Skill?[90];

        internal int MaxSkills => 90;


        internal Skill? this[string skillName] => SkillbookDictionary.TryGetValue(skillName, out var skill) ? skill : null;
        internal Skill? this[byte slot] => slot >= 0 && slot < MaxSkills ? SkillArray[slot] : null;


        internal Skillbook()
        {
            SkillbookDictionary = new Dictionary<string, Skill>();
            SkillArray = new Skill[90];
        }

        internal void AddOrUpdateSkill(Skill skill)
        {
            if (skill == null)
                return;

            if (SkillbookDictionary.TryGetValue(skill.Name, out var existingSkill))
            {
                existingSkill = SkillbookDictionary[skill.Name];
                existingSkill.Slot = skill.Slot;
                existingSkill.CurrentLevel = skill.CurrentLevel;
                existingSkill.MaxLevel = skill.MaxLevel;
                if (skill.Ticks > existingSkill.Ticks)
                {
                    existingSkill.Ticks = skill.Ticks;
                }
                skill = existingSkill;
            }
            else
            {
                SkillbookDictionary.Add(skill.Name, skill);
            }

            SkillArray[skill.Slot] = skill;
        }

        internal void RemoveSkill(byte slot)
        {
            if (slot >= 0 && slot < MaxSkills && SkillArray[slot] != null)
            {
                SkillbookDictionary.Remove(SkillArray[slot]!.Name);
                SkillArray[slot] = null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)SkillbookDictionary).GetEnumerator();
        }
    }
}
