﻿using System;
using System.Collections.Generic;

namespace RetSim
{
    public class Modifiers
    {
        public SchoolModifiers Schools { get; init; } = new SchoolModifiers();

        public float AttackSpeed { get; set; } = 1f;
        public float CastSpeed { get; set; } = 1f;
        public float WeaponDamage { get; set; } = 1f;
    }

    public abstract class FailsafeDictionary<Key, Value> : Dictionary<Key, Value>
    {
        protected Value Default { get; init; }

        public Value GetValue(Key key)
        {
            if (ContainsKey(key))
                return this[key];

            else
                return Default;
        }
    }

    public class SchoolModifiers : FailsafeDictionary<School, float>
    {
        public SchoolModifiers()
        {
            Default = 1f;

            foreach (School school in Enum.GetValues(typeof(School)))
            {
                Add(school, Default);
            }
        }
    }
}
