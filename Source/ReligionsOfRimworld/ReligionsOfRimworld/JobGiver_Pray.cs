﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace ReligionsOfRimworld
{
    public class JobGiver_Pray : ThinkNode_JobGiver
    {
        public override float GetPriority(Pawn pawn)
        {
            return pawn.GetReligionComponent().PietyTracker.Piety.CurCategoryIntWithoutZero * 19f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (this.GetPriority(pawn) == 0f)
                return (Job)null;

            CompReligion comp = pawn.GetReligionComponent();
            if (comp.Religion.PrayingSettings != null && comp.ReligionRestrictions.MayPray && comp.Religion.PrayingSettings.PrayIntervalHours * 2500 + comp.PrayTracker.LastPrayTick <= Find.TickManager.TicksGame)
            {
                Building placeToPray = GetRightPlaceToPray(pawn);
                if (placeToPray == null)
                    return (Job)null;

                if (!WatchBuildingUtility.TryFindBestWatchCell(placeToPray, pawn, true, out IntVec3 result, out Building chair))
                    WatchBuildingUtility.TryFindBestWatchCell(placeToPray, pawn, false, out result, out chair);

                return new Job(comp.Religion.PrayingSettings.PrayJob, placeToPray, (LocalTargetInfo)result, (LocalTargetInfo)((Thing)chair));
            }
            return (Job)null;
        }

        private Building GetRightPlaceToPray(Pawn pawn)
        {
            return pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_ReligiousBuildingMain>().Where(x => x.IsComplete && x.AssignedReligion == pawn.GetReligionComponent().Religion && pawn.CanReach(x, PathEndMode.ClosestTouch, Danger.None)).RandomElement();
        }
    }
}