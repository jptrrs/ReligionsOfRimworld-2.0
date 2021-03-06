﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ReligionsOfRimworld
{
    public class Pawn_ReligionCompability
    {
        private Pawn pawn;
        private Dictionary<Religion, float> compabilities;

        public Pawn_ReligionCompability(Pawn pawn)
        {
            this.pawn = pawn;
            compabilities = new Dictionary<Religion, float>();
            RecalculateCompabilities();
        }

        public IEnumerable<KeyValuePair<Religion, float>> Compabilities => compabilities;

        public void RecalculateCompabilities()
        {
            compabilities.Clear();

            IEnumerable<Religion> religions = ReligionExtensions.GetReligionManager().AllReligions;

            if (religions == null)
                religions = ReligionsBuffer.religions;

            if (religions != null && religions.Count() > 0)
            {
                foreach (Religion religion in religions)
                {
                    compabilities.Add(religion, CalculateCompabilityForReligion(religion.GetSettings<ReligionSettings_JoiningCriteria>(SettingsTagDefOf.JoiningCriteriaTag)));
                }
            }
        }

        public Religion MostSuitableReligion()
        {
            return compabilities.RandomElementByWeight(x => x.Value).Key;
        }

        private float CalculateCompabilityForReligion(ReligionSettings_JoiningCriteria settings)
        {
            float currentCompability = 1f;

            if (settings != null)
                foreach (JoiningCriteria permission in settings.Criteria)
                    currentCompability *= (1 - permission.PermissionRate(pawn));
            return currentCompability;
        }

        public float CompabilityFor(Religion religion)
        {
            return compabilities[religion];
        }
    }
}
