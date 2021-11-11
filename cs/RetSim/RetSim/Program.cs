﻿global using System;
global using System.Collections.Generic;

using RetSim.Items;
using RetSim.Misc;
using RetSim.Misc.Loggers;
using RetSim.Simulation;
using RetSim.Simulation.Tactics;
using RetSim.Spells;
using RetSim.Units.Enemy;
using RetSim.Units.Player;
using RetSim.Units.Player.Static;
using RetSim.Units.UnitStats;
using System.Diagnostics;

namespace RetSim;

class Program
{
    public static AbstractLogger Logger = new ConsoleLogger();

    public static int HunterAgility = 1000;
    //TODO: Find a better way to handle this

    static void Main(string[] args)
    {
        Data.Manager.InstantiateData();

        var equipment = Data.Manager.GetEquipment();        

        var playerTalents = Talent.GetTalents(20121, 31868, 20113, 20218, 31870, 20059, 31883, 35397, 20193, 20266);
        var playerBuffs = Spell.GetSpells(25580, 27141, 25898, 2048, 25528, 25359, 25570, 30811, 26991, 25392, 32999, 27127, 28878);
        var playerDebuffs = Spell.GetSpells(20337, 14169, 33602, 27226, 30070, 32484, 17800, 33200, 15258, 22959, 14325, 34501);

        Logger.Log("Press Enter to run a single, detailed sim, or any other key to run many, non-detailed sims.");

        bool once = Console.ReadKey(false).Key == ConsoleKey.Enter;

        Console.Clear();

        Logger.DisableInput();

        if (once)
            RunOnce(equipment, playerTalents, playerBuffs, playerDebuffs);

        else
            RunMany(equipment, playerTalents, playerBuffs, playerDebuffs);

        //TODO: Add more debuffs, consumables

        //PrintEquipment(equipment);

        Logger.EnableInput();
    }

    static void PrintEquipment(Equipment equipment)
    {
        Logger.Log("");

        Logger.Log($"╔═══════════╦═══════╦═══════════════════════════╦══════╦═════════════════════════════════════════════════════════════╗");
        Logger.Log($"║   Slot    ║  ID   ║           Item            ║ Gems ║                          Gem Names                          ║");
        Logger.Log($"╠═══════════╬═══════╬═══════════════════════════╬══════╬═════════════════════════════════════════════════════════════╣");

        foreach (EquippableItem item in equipment.PlayerEquipment)
        {
            if (item != null)
                Logger.Log($"{item}");
        }

        Logger.Log($"╚═══════════╩═══════╩═══════════════════════════╩══════╩═════════════════════════════════════════════════════════════╝");
    }

    static void RunOnce(Equipment equipment, List<Talent> talents, List<Spell> buffs, List<Spell> debuffs)
    {
        FightSimulation fight = new(new Player("Brave Hero", Data.Collections.Races["Human"], equipment, talents), new Enemy("Magtheridon", CreatureType.Demon, ArmorCategory.Warrior), new EliteTactic(), buffs, debuffs, 180000, 200000);

        PrintStats(fight.Player.Stats.All);

        fight.Run();

        fight.Output();
    }

    static void RunMany(Equipment equipment, List<Talent> talents, List<Spell> buffs, List<Spell> debuffs)
    {
        float iterations = 10000;
        int outerIterations = 10;

        (float AverageDPS, TimeSpan TimeSpan)[] results = new (float, TimeSpan)[outerIterations];


        float dps = 0;
        TimeSpan time = TimeSpan.Zero;

        float maxDPS = 0;
        float minDPS = float.MaxValue;

        TimeSpan maxTime = TimeSpan.Zero;
        TimeSpan minTime = TimeSpan.MaxValue;

        Logger.Log($"╔═════╦═══════════════╦════════╦════════════╦══════════════════╗");
        Logger.Log($"║  #  ║   Progress    ║   %    ║    DPS     ║   Elapsed Time   ║");
        Logger.Log($"╠═════╬═══════════════╬════════╬════════════╬══════════════════╣");

        for (int i = 0; i < outerIterations; i++)
        {
            Logger.Log($"║ {i + 1,3} ║ {$"0/{iterations}",13} ║     0% ║          0 ║ 00:00:00.0000000 ║");
        }

        Logger.Log($"╠═════╩═══════════════╩════════╬════════════╬══════════════════╣");
        Logger.Log($"║           Average            ║          0 ║ 00:00:00.0000000 ║");
        Logger.Log($"╠══════════════════════════════╬════════════╬══════════════════╣");
        Logger.Log($"║           Maximum            ║          0 ║ 00:00:00.0000000 ║");
        Logger.Log($"╠══════════════════════════════╬════════════╬══════════════════╣");
        Logger.Log($"║          Mininimum           ║          0 ║ 00:00:00.0000000 ║");
        Logger.Log($"╚══════════════════════════════╩════════════╩══════════════════╝");

        for (int i = 0; i < outerIterations; i++)
        {
            results[i] = Run(iterations, equipment, talents, buffs, debuffs, i, false);

            dps += results[i].AverageDPS;
            time += results[i].TimeSpan;

            if (results[i].AverageDPS > maxDPS)
            {
                maxDPS = results[i].AverageDPS;
            }

            if (results[i].AverageDPS < minDPS)
            {
                minDPS = results[i].AverageDPS;
            }

            if (results[i].TimeSpan.TotalMilliseconds > maxTime.TotalMilliseconds)
            {
                maxTime = results[i].TimeSpan;
            }

            if (results[i].TimeSpan.TotalMilliseconds < minTime.TotalMilliseconds)
            {
                minTime = results[i].TimeSpan;
            }
        }

        Logger.Log($"╠═════╩═══════════════╩════════╬════════════╬══════════════════╣");
        Logger.Log($"║           Average            ║ {dps / outerIterations,10} ║ {time / outerIterations,16} ║");
        Logger.Log($"╠══════════════════════════════╬════════════╬══════════════════╣");
        Logger.Log($"║           Maximum            ║ {maxDPS,10} ║ {maxTime,16} ║");
        Logger.Log($"╠══════════════════════════════╬════════════╬══════════════════╣");
        Logger.Log($"║          Mininimum           ║ {minDPS,10} ║ {minTime,16} ║");
        Logger.Log($"╚══════════════════════════════╩════════════╩══════════════════╝");
    }

    static (float AverageDPS, TimeSpan span) Run(float iterations, Equipment equipment, List<Talent> talents, List<Spell> buffs, List<Spell> debuffs, int outer, bool log)
    {
        var dps = 0f;
        var row = outer + 3;

        Stopwatch watch = new();

        watch.Start();

        for (int i = 0; i < iterations; i++)
        {
            FightSimulation fight = new(new Player("Brave Hero", Data.Collections.Races["Human"], equipment, talents), new Enemy("Magtheridon", CreatureType.Demon, ArmorCategory.Warrior), new EliteTactic(), buffs, debuffs, 180000, 190000);

            fight.Run();

            var index = i + 1;

            var progress = Math.Round(index / iterations * 100, 2);

            dps += fight.CombatLog.DPS;

            Console.SetCursorPosition(0, row);
            Logger.Log($"║ {outer + 1,3} ║ {$"{index}/{iterations}",13} ║ {progress,5}% ║ {dps / index,10} ║ {watch.Elapsed,16} ║");
        }

        watch.Stop();

        return (dps / iterations, watch.Elapsed);
    }

    static void PrintStats(Stat[] stats)
    {
        Logger.Log($"╔══════════════════╦═══════╦══════╦═══════╦═══════╦═══════╦═══════╦═══════╗");
        Logger.Log($"║    Stat Name     ║ Total ║ Mod  ║ Base  ║ Gear  ║ Buffs ║ Ratng ║ Suprt ║");
        Logger.Log($"╠══════════════════╬═══════╬══════╬═══════╬═══════╬═══════╬═══════╬═══════╣");

        foreach (Stat stat in stats)
        {
            if (stat != null)
                Logger.Log($"║ {stat.Name,-16} ║ {$"{stat.Value.Rounded()}",5} ║ {stat.Modifier.Rounded(),4} ║ {stat.Race.Rounded(),5} ║ {stat.Gear.Rounded(),5} ║ {stat.Bonus.Rounded(),5} ║ {stat.RatingValue.Rounded(),5} ║ {stat.SupportValue.Rounded(),5} ║");
        }

        Logger.Log($"╚══════════════════╩═══════╩══════╩═══════╩═══════╩═══════╩═══════╩═══════╝");
    }
}