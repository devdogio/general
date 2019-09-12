using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.General
{
    public partial class GeneralSettingsManager : ManagerBase<GeneralSettingsManager>
    {
        [Required]
        public GeneralSettings settings;


        protected override void Awake()
        {
            base.Awake();

            settings.defaultCursor.Enable();
            Assert.raiseExceptions = settings.useExceptionsForAssertions;
            DevdogLogger.minimaLog = settings.minimalLogType;
        }
    }
}
