﻿using RetSim.Simulation.CombatLogEntries;

namespace RetSim.Simulation;

public class CombatLog
{
    public int Duration { get; private set; }
    public int Damage { get; private set; }
    public float DPS { get; private set; }

    public Dictionary<string, List<DamageEntry>> DamageBreakdown { get; init; }
    public Dictionary<string, List<BuffEntry>> AuraBreakdown { get; init; }

    public List<LogEntry> Log { get; init; }
    public List<DamageEntry> DamageLog { get; init; }

    public CombatLog()
    {
        Damage = 0;
        DPS = 0f;

        DamageBreakdown = new Dictionary<string, List<DamageEntry>>();
        AuraBreakdown = new Dictionary<string, List<BuffEntry>>();

        Log = new List<LogEntry>();
        DamageLog = new List<DamageEntry>();
    }

    public void Add(LogEntry entry)
    {
        Log.Add(entry);
    }

    public void Add(DamageEntry entry)
    {
        Damage += entry.Damage;

        Duration = entry.Timestamp;

        DPS = Duration == 0 ? Damage : (float)Damage / Duration * 1000;

        Log.Add(entry);
        DamageLog.Add(entry);
    }

    public void CreateDamageBreakdown()
    {
        DamageBreakdown.Clear();

        foreach (DamageEntry entry in DamageLog)
        {

            if (DamageBreakdown.ContainsKey(entry.Source))
                DamageBreakdown[entry.Source].Add(entry);

            else
                DamageBreakdown.Add(entry.Source, new List<DamageEntry> { entry });
        }
    }

    public void CreateAuraBreakDown()
    {
        AuraBreakdown.Clear();

        foreach (LogEntry entry in Log)
        {
            if (entry is BuffEntry buff)
            {
                if (AuraBreakdown.ContainsKey(buff.Source))
                    AuraBreakdown[buff.Source].Add(buff);

                else
                    AuraBreakdown.Add(buff.Source, new List<BuffEntry> { buff });
            }
        }
    }
}