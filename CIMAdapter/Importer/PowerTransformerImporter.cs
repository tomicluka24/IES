﻿using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    /// <summary>
    /// PowerTransformerImporter
    /// </summary>
    public class PowerTransformerImporter
    {
        /// <summary> Singleton </summary>
        private static PowerTransformerImporter ptImporter = null;
        private static object singletoneLock = new object();

        private ConcreteModel concreteModel;
        private Delta delta;
        private ImportHelper importHelper;
        private TransformAndLoadReport report;


        #region Properties
        public static PowerTransformerImporter Instance
        {
            get
            {
                if (ptImporter == null)
                {
                    lock (singletoneLock)
                    {
                        if (ptImporter == null)
                        {
                            ptImporter = new PowerTransformerImporter();
                            ptImporter.Reset();
                        }
                    }
                }
                return ptImporter;
            }
        }

        public Delta NMSDelta
        {
            get
            {
                return delta;
            }
        }
        #endregion Properties


        public void Reset()
        {
            concreteModel = null;
            delta = new Delta();
            importHelper = new ImportHelper();
            report = null;
        }

        public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
        {
            LogManager.Log("Importing PowerTransformer Elements...", LogLevel.Info);
            report = new TransformAndLoadReport();
            concreteModel = cimConcreteModel;
            delta.ClearDeltaOperations();

            if ((concreteModel != null) && (concreteModel.ModelMap != null))
            {
                try
                {
                    // convert into DMS elements
                    ConvertModelAndPopulateDelta();
                }
                catch (Exception ex)
                {
                    string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
                    LogManager.Log(message);
                    report.Report.AppendLine(ex.Message);
                    report.Success = false;
                }
            }
            LogManager.Log("Importing PowerTransformer Elements - END.", LogLevel.Info);
            return report;
        }

        /// <summary>
        /// Method performs conversion of network elements from CIM based concrete model into DMS model.
        /// </summary>
        private void ConvertModelAndPopulateDelta()
        {
            LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            //// import all concrete model types (DMSType enum)
            ImportTerminals();
            ImportRegulatingControls();
            ImportReactiveCapabilityCurves();
            ImportSynchronousMachines();
            ImportShuntCompensators();
            ImportStaticVarCompensator();
            ImportControls();

            LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
        }

        #region Import

        private void ImportTerminals()
        {
            SortedDictionary<string, object> cimTerminals = concreteModel.GetAllObjectsOfType("FTN.Terminal");
            if (cimTerminals != null)
            {
                foreach (KeyValuePair<string, object> cimTerminalPair in cimTerminals)
                {
                    FTN.Terminal cimTerminal = cimTerminalPair.Value as FTN.Terminal;

                    ResourceDescription rd = CreateTerminalResourceDescription(cimTerminal);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Terminal ID = ").Append(cimTerminal.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Terminal ID = ").Append(cimTerminal.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }
        private ResourceDescription CreateTerminalResourceDescription(FTN.Terminal cimTerminal)
        {
            ResourceDescription rd = null;
            if (cimTerminal != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TERMINAL, importHelper.CheckOutIndexForDMSType(DMSType.TERMINAL));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimTerminal.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateTerminalProperties(cimTerminal, rd);
            }
            return rd;
        }

        private void ImportRegulatingControls()
        {
            SortedDictionary<string, object> cimRegulatingControls = concreteModel.GetAllObjectsOfType("FTN.RegulatingControl");
            if (cimRegulatingControls != null)
            {
                foreach (KeyValuePair<string, object> cimRegulatingControlPair in cimRegulatingControls)
                {
                    FTN.RegulatingControl cimRegulatingControl = cimRegulatingControlPair.Value as FTN.RegulatingControl;

                    ResourceDescription rd = CreateRegulatingControlResourceDescription(cimRegulatingControl);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("RegulatingControl ID = ").Append(cimRegulatingControl.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("RegulatingControl ID = ").Append(cimRegulatingControl.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }
        private ResourceDescription CreateRegulatingControlResourceDescription(FTN.RegulatingControl cimRegulatingControl)
        {
            ResourceDescription rd = null;
            if (cimRegulatingControl != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.REGULATINGCONTROL, importHelper.CheckOutIndexForDMSType(DMSType.REGULATINGCONTROL));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimRegulatingControl.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateRegulatingControlProperties(cimRegulatingControl, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportReactiveCapabilityCurves()
        {
            SortedDictionary<string, object> cimReactiveCapabilityCurves = concreteModel.GetAllObjectsOfType("FTN.ReactiveCapabilityCurve");
            if (cimReactiveCapabilityCurves != null)
            {
                foreach (KeyValuePair<string, object> cimReactiveCapabilityCurvePair in cimReactiveCapabilityCurves)
                {
                    FTN.ReactiveCapabilityCurve cimReactiveCapabilityCurve = cimReactiveCapabilityCurvePair.Value as FTN.ReactiveCapabilityCurve;

                    ResourceDescription rd = CreateReactiveCapabilityCurveResourceDescription(cimReactiveCapabilityCurve);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("ReactiveCapabilityCurve ID = ").Append(cimReactiveCapabilityCurve.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("ReactiveCapabilityCurve ID = ").Append(cimReactiveCapabilityCurve.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }
        private ResourceDescription CreateReactiveCapabilityCurveResourceDescription(FTN.ReactiveCapabilityCurve cimReactiveCapabilityCurve)
        {
            ResourceDescription rd = null;
            if (cimReactiveCapabilityCurve != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.REACTIVECAPABILITYCURVE, importHelper.CheckOutIndexForDMSType(DMSType.REACTIVECAPABILITYCURVE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimReactiveCapabilityCurve.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateReactiveCapabilityCurveProperties(cimReactiveCapabilityCurve, rd);
            }
            return rd;
        }

        private void ImportSynchronousMachines()
        {
            SortedDictionary<string, object> cimSynchronousMachines = concreteModel.GetAllObjectsOfType("FTN.SynchronousMachine");
            if (cimSynchronousMachines != null)
            {
                foreach (KeyValuePair<string, object> cimSynchronousMachinePair in cimSynchronousMachines)
                {
                    FTN.SynchronousMachine cimSynchronousMachine = cimSynchronousMachinePair.Value as FTN.SynchronousMachine;

                    ResourceDescription rd = CreateSynchronousMachineResourceDescription(cimSynchronousMachine);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("SynchronousMachine ID = ").Append(cimSynchronousMachine.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("SynchronousMachine ID = ").Append(cimSynchronousMachine.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }
        private ResourceDescription CreateSynchronousMachineResourceDescription(FTN.SynchronousMachine cimSynchronousMachine)
        {
            ResourceDescription rd = null;
            if (cimSynchronousMachine != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SYNCHRONOUSMACHINE, importHelper.CheckOutIndexForDMSType(DMSType.SYNCHRONOUSMACHINE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSynchronousMachine.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateSynchronousMachineProperties(cimSynchronousMachine, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportShuntCompensators()
        {
            SortedDictionary<string, object> cimShuntCompensators = concreteModel.GetAllObjectsOfType("FTN.ShuntCompensator");
            if (cimShuntCompensators != null)
            {
                foreach (KeyValuePair<string, object> cimShuntCompensatorPair in cimShuntCompensators)
                {
                    FTN.ShuntCompensator cimShuntCompensator = cimShuntCompensatorPair.Value as FTN.ShuntCompensator;

                    ResourceDescription rd = CreateShuntCompensatorResourceDescription(cimShuntCompensator);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("ShuntCompensator ID = ").Append(cimShuntCompensator.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("ShuntCompensator ID = ").Append(cimShuntCompensator.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }
        private ResourceDescription CreateShuntCompensatorResourceDescription(FTN.ShuntCompensator cimShuntCompensator)
        {
            ResourceDescription rd = null;
            if (cimShuntCompensator != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SHUTNCOMPENSATOR, importHelper.CheckOutIndexForDMSType(DMSType.SHUTNCOMPENSATOR));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimShuntCompensator.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateShuntCompensatorProperties(cimShuntCompensator, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportStaticVarCompensator()
        {
            SortedDictionary<string, object> cimStaticVarCompensators = concreteModel.GetAllObjectsOfType("FTN.StaticVarCompensator");
            if (cimStaticVarCompensators != null)
            {
                foreach (KeyValuePair<string, object> cimStaticVarCompensatorPair in cimStaticVarCompensators)
                {
                    FTN.StaticVarCompensator cimStaticVarCompensator = cimStaticVarCompensatorPair.Value as FTN.StaticVarCompensator;

                    ResourceDescription rd = CreateStaticVarCompensatorResourceDescription(cimStaticVarCompensator);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("StaticVarCompensator ID = ").Append(cimStaticVarCompensator.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Static    VarCompensator ID = ").Append(cimStaticVarCompensator.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }
        private ResourceDescription CreateStaticVarCompensatorResourceDescription(FTN.StaticVarCompensator cimStaticVarCompensator)
        {
            ResourceDescription rd = null;
            if (cimStaticVarCompensator != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.STATICVARCOMPENSATOR, importHelper.CheckOutIndexForDMSType(DMSType.STATICVARCOMPENSATOR));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimStaticVarCompensator.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateStaticVarCompensatorProperties(cimStaticVarCompensator, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportControls()
        {
            SortedDictionary<string, object> cimControls = concreteModel.GetAllObjectsOfType("FTN.Control");
            if (cimControls != null)
            {
                foreach (KeyValuePair<string, object> cimControlPair in cimControls)
                {
                    FTN.Control cimControl = cimControlPair.Value as FTN.Control;

                    ResourceDescription rd = CreateControlResourceDescription(cimControl);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Control ID = ").Append(cimControl.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Control ID = ").Append(cimControl.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }
        private ResourceDescription CreateControlResourceDescription(FTN.Control cimControl)
        {
            ResourceDescription rd = null;
            if (cimControl != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CONTROL, importHelper.CheckOutIndexForDMSType(DMSType.CONTROL));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimControl.ID, gid);

                ////populate ResourceDescription
                PowerTransformerConverter.PopulateControlProperties(cimControl, rd, importHelper, report);
            }
            return rd;
        }

        #endregion Import
    }
}

