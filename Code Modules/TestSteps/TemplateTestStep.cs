/*************************************************************
Boilerplate  template for creating a new custom test module.
Feel free to paste this when starting a new test method,
but make sure to update and rename the method signature,
as well as the documentation. The method signature should 
match the file name.
*************************************************************/

using NationalInstruments.SemiconductorTestLibrary.Common;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace QPF4659_0325
{
    /// <summary>  
    /// Partial class containing all test steps for the project.  
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.  
    /// </summary>  
    public static partial class TestSteps
    {
        /// <summary>  
        /// Call this method from the test executive to execute the test code.  
        /// </summary>  
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object reference.</param>  
        /// <param name="ppmuPinNames"></param>  
        /// <param name="dcPinNames"></param>  
        /// <param name="pmuForceCurrentHi"></param>  
        /// <param name="pmuForceCurrentLo"></param>  
        /// <param name="pmuApertureTime"></param>  
        /// <param name="dcApertureTime"></param>  
        public static void Continuity(ISemiconductorModuleContext tsmContext, string[] ppmuPinNames, string[] dcPinNames, double pmuForceCurrentHi = 100e-6, double pmuForceCurrentLo = -100e-6, double pmuApertureTime = 0.001, double dcApertureTime = 0.001)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var pmuPins = sessionManager.Digital(ppmuPinNames);
            pmuPins.ForceVoltage(0, 0.002, pmuApertureTime);
            var dcPins = sessionManager.DCPower(dcPinNames);
            var dcMeasureSettings = new DCPowerMeasureSettings
            {
                ApertureTime = 0.0167,
                MeasureWhen = DCPowerMeasurementWhen.OnDemand,
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.Seconds,
                RecordLength = 1,
                Sense = DCPowerMeasurementSense.Remote,
            };
            dcPins.ConfigureMeasureSettings(dcMeasureSettings);
            var dcSourceSettings = new DCPowerSourceSettings
            {
                LevelRange = 10,
                LimitHigh = 0.1,
                LimitLow = -0.8,
                Level = 0,
                Limit = 0.1,
                LimitRange = 0.1,
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                SourceDelayInSeconds = 0.001,
                TransientResponse = DCPowerSourceTransientResponse.Normal,
            };
            dcPins.ConfigureSourceSettings(dcSourceSettings);
            dcPins.ForceVoltage(0, 0.002);

            foreach (var pin in ppmuPinNames)
            {
                var pinSessionBundle = sessionManager.Digital(pin);
                pinSessionBundle.ForceCurrent(pmuForceCurrentLo);
                pinSessionBundle.MeasureAndPublishVoltage(pin + "_Lo");
                pinSessionBundle.ForceVoltage(0, 0.002);
            }
        }
    }
}