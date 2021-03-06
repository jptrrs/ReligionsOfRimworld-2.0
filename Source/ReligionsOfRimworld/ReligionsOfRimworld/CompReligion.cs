﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class CompReligion : ThingComp
    {
        private Religion religion;
        private Pawn_ReligionCompability religionCompability;
        private Pawn_PietyTracker pietyTracker;
        private Pawn_ReligionRestrictions religionRestrictions;
        private Pawn_PrayTracker prayTracker;

        public Religion Religion => religion;
        public Pawn_ReligionCompability ReligionCompability => religionCompability;
        public Pawn_PietyTracker PietyTracker => pietyTracker;
        public Pawn_ReligionRestrictions ReligionRestrictions => religionRestrictions;
        public Pawn_PrayTracker PrayTracker => prayTracker;

        public bool TryChangeReligion(Religion religion)
        {
            if (religionCompability.CompabilityFor(religion) != 0f && religion != this.religion)
            {
                ChangeReligion(religion);
                return true;
            }
            return false;
        }

        private void ChangeReligion(Religion religion)
        {
            this.religion = religion;
            pietyTracker = new Pawn_PietyTracker((Pawn)parent, religion);
            prayTracker = new Pawn_PrayTracker((Pawn)parent, religion);
            religionCompability.RecalculateCompabilities();
            religionRestrictions.RestoreToDefault();
            BillUtility.Notify_ColonistUnavailable((Pawn)this.parent);
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            religionCompability = new Pawn_ReligionCompability((Pawn)parent);
            religionRestrictions = new Pawn_ReligionRestrictions();
            ChangeReligion(religionCompability.MostSuitableReligion());
        }

        public void Refresh()
        {
            religionCompability.RecalculateCompabilities();
        }

        public override void CompTick()
        {
            base.CompTick();
            pietyTracker.TrackerTick();
            prayTracker.TrackerTick();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look<Religion>(ref this.religion, "religionOfPawn");
            Scribe_Deep.Look<Pawn_PietyTracker>(ref this.pietyTracker, "pietyTracker", (Pawn)parent, religion);
            Scribe_Deep.Look<Pawn_ReligionRestrictions>(ref this.religionRestrictions, "religionRestrictions");
            Scribe_Deep.Look<Pawn_PrayTracker>(ref this.prayTracker, "prayTracker", (Pawn)parent, religion);
            if (religion == null)
                Initialize(null);
        }
    }
}
