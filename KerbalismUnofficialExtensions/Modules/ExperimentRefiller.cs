using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KERBALISM;
using KSP.Localization;

namespace KerbalismUnofficialExtensions.Modules
{
    /// <summary>
    /// Allows to refill experiment samples given you have a Scientist EVAing nearby.
    /// </summary>
    public class ExperimentRefiller : PartModule
    {
        private Experiment experiment;
        public bool isRefillable = false;

        public const string SAMPLE_REFILL_PART_NAME = "sampleRefillCan";

        public override string GetModuleDisplayName()
        {
            return Localizer.Format("#KERBALISMUE_experiment_refiller");
        }

        public override string GetInfo()
        {
            return Localizer.Format("#KERBALISMUE_experiment_refiller_desc");
        }

        public override void OnStart(StartState state)
        {
            experiment = part.Modules.OfType<Experiment>().SingleOrDefault();
            // If we don't find the experiment (a misconfiguration), or if generates samples, or it
            // does not require samples at all (like a temperature reading), we skip.
            if (experiment == null || experiment.sample_collecting || !experiment.ExpInfo.IsSample) {
                isRefillable = false;
                return;
            }

            isRefillable = true;
        }

        /// <summary>
        /// Experiments are refillable only when they're depleted, there is still some science to do (ExpInfo)
        /// and sample mass on the equipment is empty.
        /// 
        /// Kerbalism uses a strange mechanism, where some experiments (like Material Bay and Mystery Goo)
        /// are shipped with some "samples" (like, some goo samples) that need to be tested in outer space.
        /// Actually, it makes pretty much sense, however I didn't get it while playing, so I'm leaving
        /// this note here just to remember.
        /// </summary>
        public virtual void Update()
        {
            if (!isRefillable) return;
            if (!HighLogic.LoadedSceneIsFlight) return;

            VesselData vd = vessel.KerbalismData();
            if (!vd.IsSimulated) return;

            if (experiment.Status == Experiment.ExpStatus.Issue && experiment.ExpInfo.SampleMass > 0.0 && experiment.remainingSampleMass <= 0.0)
            {
                Events["OnRefillExperiment"].active = true;
                Events["OnRefillExperiment"].guiName = HasActiveEvaSampleRefill()
                        ? Localizer.Format("#KERBALISMUE_experiment_refill_action")
                        : Localizer.Format("#KERBALISMUE_experiment_refill_action_nosample");
            }
            else
            {
                Events["OnRefillExperiment"].active = false;
            }
        }

        /// <summary>
        /// Refilling si available only if current vessel in a Kerbal (EVA), and he brings
        /// with himself some Refill Samples.
        /// </summary>
        /// <returns></returns>
        protected bool HasActiveEvaSampleRefill()
        {
            Vessel v = FlightGlobals.ActiveVessel;

            if (v == null || !v.isEVA || EVA.IsDead(v)) return false;

            var inventory = v.FindPartModuleImplementing<ModuleInventoryPart>();
            return inventory != null && inventory.ContainsPart(SAMPLE_REFILL_PART_NAME);
        }

        [KSPEvent(guiActiveUnfocused = true, active = false, guiName = "_", groupName = "Science", groupDisplayName = "#KERBALISM_Group_Science")]
        public void OnRefillExperiment()
        {
            Vessel v = FlightGlobals.ActiveVessel;

            if (!HasActiveEvaSampleRefill()) {               
                ScreenMessages.PostScreenMessage(Localizer.Format("#KERBALISMUE_experiment_refill_missing", experiment.ExpInfo.Title), 3.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            double maxSampleMass = experiment.ExpInfo.SampleMass * experiment.sample_amount;
            experiment.remainingSampleMass = maxSampleMass;

            var inventory = v.FindPartModuleImplementing<ModuleInventoryPart>();
            inventory.RemoveNPartsFromInventory(SAMPLE_REFILL_PART_NAME, 1, true);

            ScreenMessages.PostScreenMessage(Localizer.Format("#KERBALISMUE_experiment_refill_success", experiment.ExpInfo.Title), 3.0f, ScreenMessageStyle.UPPER_CENTER);
        }
    }
}
