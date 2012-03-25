﻿/*
 * Idmr.Platform.dll, X-wing series mission library file, TIE95-XWA
 * Copyright (C) 2009-2012 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the GPL v3.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 2.0
 */

/* CHANGELOG
 * 310112 - removed Goal.AndOrIndexer, Goal.AndOr to bool[]
 * *** v2.0 ***
 */

using System;
using Idmr.Common;

namespace Idmr.Platform.Xvt
{
	/// <summary>Object for individual Team's mission-wide goals</summary>
	[Serializable] public class Globals
	{
		Goal[] _goals = new Goal[3];

		/// <summary>Goal indexes</summary>
		public enum GoalIndex : byte
		{
			/// <summary>Primary goals</summary>
			Primary,
			/// <summary>Prevent goals</summary>
			Prevent,
			/// <summary>Secondary goals</summary>
			Secondary
		}
		/// <summary>Goal status</summary>
		public enum GoalState : byte
		{
			/// <summary>Goals that have not yet been completed.<br/>Does not apply to Secondary goals</summary>
			Incomplete,
			/// <summary>Goals that have been completed</summary>
			Complete,
			/// <summary>Goals that cannot be completed due to gameplay (mission design may permit inability to complete goals without direct failure).<br/>Does not apply to Prevent or Secondary goals</summary>
			Failed
		}
		
		/// <summary>Creates a new Globals object</summary>
		/// <remarks>Three <see cref="Goals"/>, each with four <see cref="Goal.Triggers"/> all set to <b>"never (FALSE)"</b></remarks>
		public Globals() { for (int i = 0; i < 3; i++) _goals[i] = new Goal(); }
		
		/// <summary>Gets the Global Goals</summary>
		/// <remarks>Use the <see cref="GoalIndex"/> enumeration for indexes</remarks>
		public Goal[] Goals { get { return _goals; } }
		
		/// <summary>Container for a single Global Goal</summary>
		public class Goal
		{
			Mission.Trigger[] _triggers = new Mission.Trigger[4];
			string[,] _goalStrings = new string[4, 3];	// No PreventFailed, SecondaryIncomplete, SecondaryFailed
			GoalStringIndexer _goalStringsIndexer;
			bool[] _andOrs = new bool[3];

			/// <summary>Initializes a new Goal</summary>
			/// <remarks>All <see cref="Triggers"/> set to <b>10</b>, "never (FALSE)". <see cref="AndOr"/> values set to <b>true</b>, "OR"</remarks>
			public Goal()
			{
				for (int i = 0; i < 12; i++) _goalStrings[i / 3, i % 3] = "";
				for (int i = 0; i < 4; i++)
				{
					_triggers[i] = new Mission.Trigger();
					_triggers[i].Condition = 10;
				}
				for (int i = 0; i < 3; i++) _andOrs[i] = true;
				_goalStringsIndexer = new GoalStringIndexer(this);
			}
			
			/// <summary>Raw value stored in file</summary>
			public sbyte RawPoints { get; set; }
			
			/// <summary>Gets or sets if both Triggers must be met</summary>
			public bool T1AndOrT2
			{
				get { return _andOrs[0]; }
				set { _andOrs[0] = value; }
			}
			/// <summary>Gets or sets if both Triggers must be met</summary>
			public bool T3AndOrT4
			{
				get { return _andOrs[1]; }
				set { _andOrs[1] = value; }
			}
			/// <summary>Gets or sets if both Trigger pairs must be met</summary>
			public bool T12AndOrT34
			{
				get { return _andOrs[2]; }
				set { _andOrs[2] = value; }
			}

			/// <summary>Gets the array for the AndOr values</summary>
			public bool[] AndOr { get { return _andOrs; } }

			/// <summary>Gets or sets the points awarded or subtracted after Goal completion</summary>
			/// <remarks>Equal to <see cref="RawPoints"/> * 250, limited from <b>-32000</b> to <b>+31750</b></remarks>
			public short Points
			{
				get { return (short)(RawPoints * 250); }
				set { RawPoints = (sbyte)(value > 31750 ? 31750 : (value < -32000 ? -32000 : value) / 250); }
			}
			
			/// <summary>Gets the array accessor for the GoalString values</summary>
			public GoalStringIndexer GoalStrings { get { return _goalStringsIndexer; } }
			
			/// <summary>The Triggers that define the Goal</summary>
			/// <remarks>Array length is 4</remarks>
			public Mission.Trigger[] Triggers { get { return _triggers; } }
			
			/// <summary>Object to provide array access to the GoalString values</summary>
			public class GoalStringIndexer
			{
				Goal _owner;
				
				/// <summary>Initializes the indexer</summary>
				/// <param name="parent">Parent Global Goal</param>
				public GoalStringIndexer(Goal parent) { _owner = parent; }
				
				/// <summary>Gets the size of the array</summary>
				public int Length { get { return _owner._goalStrings.Length; } }
				
				/// <summary>Gets or sets the Trigger Status strings</summary>
				/// <remarks>Limited to 63 characters</remarks>
				/// <param name="trigger">Trigger index, 0-3</param>
				/// <param name="state">GoalState value, 0-2</param>
				/// <exception cref="IndexOutOfRangeException">Invalid <i>trigger</i> or <i>state</i> value</exception>
				public string this[int trigger, int state]
				{
					get { return _owner._goalStrings[trigger, state]; }
					set { _owner._goalStrings[trigger, state] = StringFunctions.GetTrimmed(value, 63); }
				}
				
				/// <summary>Gets or sets the Trigger Status strings</summary>
				/// <remarks>Limited to 63 characters</remarks>
				/// <param name="trigger">Trigger index, 0-3</param>
				/// <param name="state">GoalState value</param>
				/// <exception cref="IndexOutOfRangeException">Invalid <i>trigger</i> value</exception>
				public string this[int trigger, GoalState state]
				{
					get { return _owner._goalStrings[trigger, (int)state]; }
					set { _owner._goalStrings[trigger, (int)state] = StringFunctions.GetTrimmed(value, 63); }
				}
			}
		}
	}
}
