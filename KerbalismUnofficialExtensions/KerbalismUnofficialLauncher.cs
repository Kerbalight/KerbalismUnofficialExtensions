using KerbalismUnofficialExtensions.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KerbalismUnofficialExtensions
{
    [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
    public class KerbalismUnofficialLauncher : MonoBehaviour
    {
        public void Start()
        {
            Lib.Log("[KerbalismUE] Started.");
        }
    }
}
