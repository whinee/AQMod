﻿using Aequus.Common.Utilities;
using System;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Systems;

public class TimeSystem : ModSystem {
    public static int DaysPassed { get; private set; }

    public static int WeekDay { get; private set; }

    public static DayOfWeek DayOfTheWeek => (DayOfWeek)WeekDay;

    /// <param name="firstDay">A <see cref="DayOfWeek"/> to compare against <see cref="DayOfTheWeek"/>.</param>
    /// <param name="lastDay">A <see cref="DayOfWeek"/> to compare against <see cref="DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific time range between two days of the week.</returns>
    public static Condition ConditionBetweenDaysOfWeek(DayOfWeek firstDay, DayOfWeek lastDay) {
        LocalizedText text = ALanguage.GetText("Condition.BetweenDays")
            .WithFormatArgs(GetWeekText(firstDay), GetWeekText(lastDay));

        if (firstDay > lastDay) {
            // For example, if something were to be sold between Friday (5) and Monday (1)
            // We should instead check if the day is <= Monday and >= Friday
            // Making the valid days be 5, 6, 0, and 1.
            return new Condition(text, () => DayOfTheWeek <= lastDay && DayOfTheWeek >= firstDay);
        }

        // Otherwise, no special logic is needed, we can just check
        // if the current day is between the first and last days.
        return new Condition(text, () => DayOfTheWeek >= firstDay && DayOfTheWeek <= lastDay);
    }

    /// <param name="dayOfWeek">The <see cref="GetWeekText"/> to compare against <see cref="DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific day of the week.</returns>
    public static Condition ConditionByDayOfWeek(DayOfWeek dayOfWeek) {
        return new Condition(ALanguage.GetText("Condition.DayOfTheWeek").WithFormatArgs(GetWeekText(dayOfWeek)), () => DayOfTheWeek == dayOfWeek);
    }

    /// <returns>Localized name of a <see cref="DayOfWeek"/> value.</returns>
    public static LocalizedText GetWeekText(DayOfWeek dayOfWeek) {
        return ALanguage.GetText("Misc.DayOfTheWeek." + dayOfWeek.ToString());
    }

    public override void Load() {
        On_Main.UpdateTime_StartDay += On_Main_UpdateTime_StartDay;
    }

    private static void On_Main_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents) {
        DaysPassed++;
        orig(ref stopEvents);
    }

    public override void ClearWorld() {
        DaysPassed = 0;
    }

    public override void SaveWorldData(TagCompound tag) {
        tag["DaysPassed"] = DaysPassed;
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet("DaysPassed", out int value)) {
            DaysPassed = value;
        }
    }

    public override void PostUpdateTime() {
        if (Main.remixWorld) {
            if (Main.netMode == NetmodeID.Server) {
                WeekDay = (int)DateTime.Now.DayOfWeek;
            }
            return;
        }

        WeekDay = DaysPassed % 7;
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(DaysPassed);
        writer.Write(WeekDay);
    }

    public override void NetReceive(BinaryReader reader) {
        DaysPassed = reader.ReadInt32();
        WeekDay = reader.ReadInt32();
    }

    internal static void OnStartDay() {
        DaysPassed++;
    }
}