namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	using FTN.Common;

	/// <summary>
	/// PowerTransformerConverter has methods for populating
	/// ResourceDescription objects using PowerTransformerCIMProfile_Labs objects.
	/// </summary>
	public static class PowerTransformerConverter
	{
		#region Populate ResourceDescription
		public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
		{
			if ((cimIdentifiedObject != null) && (rd != null))
			{
				if (cimIdentifiedObject.MRIDHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
				}
				if (cimIdentifiedObject.NameHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
				}
			}
		}
		public static void PopulatePowerSystemResourceProperties(FTN.PowerSystemResource cimPowerSystemResource, ResourceDescription rd)
		{
			if ((cimPowerSystemResource != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimPowerSystemResource, rd);
			}
		}
		public static void PopulateTerminalProperties(FTN.Terminal cimTerminal, ResourceDescription rd)
		{
			if ((cimTerminal != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimTerminal, rd);
			}
		}
		public static void PopulateCurveProperties(FTN.Curve cimCurve, ResourceDescription rd)
		{
			if ((cimCurve != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimCurve, rd);
			}
		}
		public static void PopulateControlProperties(FTN.Control cimControl, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimControl != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateIdentifiedObjectProperties(cimControl, rd);

				if (cimControl.RegulatingCondEqHasValue)
				{
					long gid = importHelper.GetMappedGID(cimControl.RegulatingCondEq.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Control ").Append(cimControl.GetType().ToString()).Append(" rdfID = \"").Append(cimControl.ID);
						report.Report.Append("\" - Failed to set reference to Control: rdfID \"").Append(cimControl.RegulatingCondEq.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.CONTROL_REGULATINGCONDEQ, gid));
				}
			}
		}
		public static void PopulateEquipmentProperties(FTN.Equipment cimEquipment, ResourceDescription rd)
		{
			if ((cimEquipment != null) && (rd != null))
			{
				PowerTransformerConverter.PopulatePowerSystemResourceProperties(cimEquipment, rd);

				if (cimEquipment.AggregateHasValue)
				{
					rd.AddProperty(new Property(ModelCode.EQUIPMENT_AGGREGATE, cimEquipment.Aggregate));
				}
				if (cimEquipment.NormallyInServiceHasValue)
				{
					rd.AddProperty(new Property(ModelCode.EQUIPMENT_NORMALLYNSERVICE, cimEquipment.NormallyInService));
				}
			}
		}
		public static void PopulateConductingEquipmentProperties(FTN.ConductingEquipment cimConductingEquipment, ResourceDescription rd)
		{
			if ((cimConductingEquipment != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateEquipmentProperties(cimConductingEquipment, rd);
			}
		}
		public static void PopulateRegulatingControlProperties(FTN.RegulatingControl cimRegulatingControl, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimRegulatingControl != null) && (rd != null))
			{
				PowerTransformerConverter.PopulatePowerSystemResourceProperties(cimRegulatingControl, rd);

				if (cimRegulatingControl.DiscreteHasValue)
				{
					rd.AddProperty(new Property(ModelCode.REGCONTROL_DISCRETE, cimRegulatingControl.Discrete));
				}
				if (cimRegulatingControl.ModeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.REGCONTROL_MODE, (short)GetDMSRegulatingControlModeKind(cimRegulatingControl.Mode)));
				}
				if (cimRegulatingControl.MonitoredPhaseHasValue)
				{
					rd.AddProperty(new Property(ModelCode.REGCONTROL_MONITOREDPAHSE, (short)GetDMSPhaseCode(cimRegulatingControl.MonitoredPhase)));
				}
				if (cimRegulatingControl.TargetRangeHasValue)
				{
					rd.AddProperty(new Property(ModelCode.REGCONTROL_TARGET_RANGE, cimRegulatingControl.TargetRange));
				}
				if (cimRegulatingControl.TargetValueHasValue)
				{
					rd.AddProperty(new Property(ModelCode.REGCONTROL_TARGET_VALUE, cimRegulatingControl.TargetValue));
				}
				if (cimRegulatingControl.TerminalHasValue)
				{
					long gid = importHelper.GetMappedGID(cimRegulatingControl.Terminal.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Terminal ").Append(cimRegulatingControl.GetType().ToString()).Append(" rdfID = \"").Append(cimRegulatingControl.ID);
						report.Report.Append("\" - Failed to set reference to Terminal: rdfID \"").Append(cimRegulatingControl.Terminal.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.REGCONTROL_TERMINAL, gid));
				}
			}
		}
		public static void PopulateRegulatingCondEqProperties(FTN.RegulatingCondEq cimRegulatingCondEq, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimRegulatingCondEq != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateConductingEquipmentProperties(cimRegulatingCondEq, rd);

				if (cimRegulatingCondEq.RegulatingControlHasValue)
				{
					long gid = importHelper.GetMappedGID(cimRegulatingCondEq.RegulatingControl.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: Terminal ").Append(cimRegulatingCondEq.GetType().ToString()).Append(" rdfID = \"").Append(cimRegulatingCondEq.ID);
						report.Report.Append("\" - Failed to set reference to Terminal: rdfID \"").Append(cimRegulatingCondEq.RegulatingControl.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.REGCONDEQ_REGCONTROL, gid));
				}
			}

		}
		public static void PopulateShuntCompensatorProperties(FTN.ShuntCompensator cimShuntCompensator, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimShuntCompensator != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateRegulatingCondEqProperties(cimShuntCompensator, rd, importHelper, report);
			}
		}
		public static void PopulateStaticVarCompensatorProperties(FTN.StaticVarCompensator cimStaticVarCompensator, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimStaticVarCompensator != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateRegulatingCondEqProperties(cimStaticVarCompensator, rd, importHelper, report);
			}
		}
		public static void PopulateRotatingMachineProperties(FTN.RotatingMachine cimRotatingMachine, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimRotatingMachine != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateRegulatingCondEqProperties(cimRotatingMachine, rd, importHelper, report);
			}
		}
		public static void PopulateSynchronousMachineProperties(FTN.SynchronousMachine cimSynchronousMachine, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
		{
			if ((cimSynchronousMachine != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateRotatingMachineProperties(cimSynchronousMachine, rd, importHelper, report);

				if (cimSynchronousMachine.ReactiveCapabilityCurvesHasValue)
				{
					long gid = importHelper.GetMappedGID(cimSynchronousMachine.ReactiveCapabilityCurves.ID);
					if (gid < 0)
					{
						report.Report.Append("WARNING: SynchronousMachine ").Append(cimSynchronousMachine.GetType().ToString()).Append(" rdfID = \"").Append(cimSynchronousMachine.ID);
						report.Report.Append("\" - Failed to set reference to SynchronousMachine: rdfID \"").Append(cimSynchronousMachine.ReactiveCapabilityCurves.ID).AppendLine(" \" is not mapped to GID!");
					}
					rd.AddProperty(new Property(ModelCode.SYNCMACHINE_REACTIVECAPCURVE, gid));
				}
			}
		}
		public static void PopulateReactiveCapabilityCurveProperties(FTN.ReactiveCapabilityCurve cimReactiveCapabilityCurve, ResourceDescription rd)
		{
			if ((cimReactiveCapabilityCurve != null) && (rd != null))
			{
				PowerTransformerConverter.PopulateCurveProperties(cimReactiveCapabilityCurve, rd);
			}
		}
		
		#endregion Populate ResourceDescription

		#region Enums convert
		public static PhaseCode GetDMSPhaseCode(FTN.PhaseCode phases)
		{
			switch (phases)
			{
				case FTN.PhaseCode.A:
					return PhaseCode.A;
				case FTN.PhaseCode.AB:
					return PhaseCode.AB;
				case FTN.PhaseCode.ABC:
					return PhaseCode.ABC;
				case FTN.PhaseCode.ABCN:
					return PhaseCode.ABCN;
				case FTN.PhaseCode.ABN:
					return PhaseCode.ABN;
				case FTN.PhaseCode.AC:
					return PhaseCode.AC;
				case FTN.PhaseCode.ACN:
					return PhaseCode.ACN;
				case FTN.PhaseCode.AN:
					return PhaseCode.AN;
				case FTN.PhaseCode.B:
					return PhaseCode.B;
				case FTN.PhaseCode.BC:
					return PhaseCode.BC;
				case FTN.PhaseCode.BCN:
					return PhaseCode.BCN;
				case FTN.PhaseCode.BN:
					return PhaseCode.BN;
				case FTN.PhaseCode.C:
					return PhaseCode.C;
				case FTN.PhaseCode.CN:
					return PhaseCode.CN;
				case FTN.PhaseCode.N:
					return PhaseCode.N;
				case FTN.PhaseCode.s12N:
					return PhaseCode.ABN;
				case FTN.PhaseCode.s1N:
					return PhaseCode.AN;
				case FTN.PhaseCode.s2N:
					return PhaseCode.BN;
				default: return PhaseCode.Unknown;
			}
		}

		public static RegulatingControlModeKind GetDMSRegulatingControlModeKind(FTN.RegulatingControlModeKind regulatingControlModeKind)
		{
			switch (regulatingControlModeKind)
			{
				case FTN.RegulatingControlModeKind.activePower:
					return RegulatingControlModeKind.activePower;
				case FTN.RegulatingControlModeKind.admittance:
					return RegulatingControlModeKind.admittance;
				case FTN.RegulatingControlModeKind.currentFlow:
					return RegulatingControlModeKind.currentFlow;
				case FTN.RegulatingControlModeKind.@fixed:
					return RegulatingControlModeKind.@fixed;
				case FTN.RegulatingControlModeKind.powerFactor:
					return RegulatingControlModeKind.powerFactor;
				case FTN.RegulatingControlModeKind.reactivePower:
					return RegulatingControlModeKind.reactivePower;
				case FTN.RegulatingControlModeKind.temperature:
					return RegulatingControlModeKind.temperature;
				case FTN.RegulatingControlModeKind.timeScheduled:
					return RegulatingControlModeKind.timeScheduled;
				case FTN.RegulatingControlModeKind.voltage:
					return RegulatingControlModeKind.voltage;

				default:
					return RegulatingControlModeKind.voltage;
			}
		}
		#endregion Enums convert
	}
}
