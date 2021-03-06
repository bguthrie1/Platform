﻿/*
 * Idmr.Platform.dll, X-wing series mission library file, XW95-XWA
 * Copyright (C) 2009-2018 Michael Gaisser (mjgaisser@gmail.com)
 * This file authored by "JB" (Random Starfighter) (randomstarfighter@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * Full notice in ../help/Idmr.Platform.chm
 * Version: 3.0
 */

/* CHANGELOG
 * v3.0, 180309
 * [NEW] created [JB]
 */

//TODO: throughout, check access and redo capitalization as required

using System;
using System.Collections.Generic;

namespace Idmr.Platform.Xwing
{
    /// <summary>Briefing object for XWING95</summary>
    /// <remarks>Default settings: 45 seconds, map to (0,0), zoom to 48</remarks>
    public class Briefing : BaseBriefing
	{
		//TODO: check access
		/// <summary>Collection of Briefing pages</summary>
        public List<BriefingPage> pages;
		/// <summary>Collection of window settings </summary>
        public List<BriefingUIPage> windowSettings;

		/// <summary>Known briefing event types unique to XWING</summary>
		new public enum EventType : byte {
			/// <summary>No type defined</summary>
			None = 0,
			/// <summary>Waits for the user to click the page to proceed.  Used to hide the special hints text.</summary>
			WaitForClick = 1,
			/// <summary>Clears both the title and caption text.</summary>
			ClearText = 10, 
			/// <summary>Displays the specified text at the top of the briefing (param:textID)</summary>
			TitleText = 11,
			/// <summary>Displays the specified text at the bottom of the briefing (param:textID)</summary>
			CaptionText = 12,
            /// <summary>Alternate command used in some briefings.  If at tick 700, it's an end marker.  (param:textID)</summary>
            CaptionText2 = 14,
            /// <summary>Change the focal point of the map (param:x,y)</summary>
			MoveMap = 15,
			/// <summary>Change the zoom distance of the map (param:xFactor,yFactor)</summary>
			ZoomMap = 16,
			/// <summary>Erase all FlightGroup tags from view</summary>
			ClearFGTags = 21,
			/// <summary>Apply a FlightGroup tag using slot 1 (param:objectID)</summary>
			FGTag1 = 22,
			/// <summary>Apply a FlightGroup tag using slot 2 (param:objectID)</summary>
			FGTag2 = 23,
			/// <summary>Apply a FlightGroup tag using slot 3 (param:objectID)</summary>
			FGTag3 = 24,
			/// <summary>Apply a FlightGroup tag using slot 4 (param:objectID)</summary>
			FGTag4 = 25,
			/// <summary>Erase all text tags from view</summary>
			ClearTextTags = 26,
			/// <summary>Apply a text tag using slot 1 (param:tagID,x,y)</summary>
			TextTag1 = 27,
			/// <summary>Apply a text tag using slot 2 (param:tagID,x,y)</summary>
			TextTag2 = 28,
			/// <summary>Apply a text tag using slot 3 (param:tagID,x,y)</summary>
			TextTag3 = 29,
			/// <summary>Apply a text tag using slot 4 (param:tagID,x,y)</summary>
			TextTag4 = 30,
			/// <summary>End of briefing marker</summary>
			EndBriefing = 41
		};

        Dictionary<EventType, string> _eventTypeStringMap = new Dictionary<EventType, string> {
            {EventType.None, "None"},
            {EventType.WaitForClick, "Wait For Click"},
            {EventType.ClearText, "Clear Text"},
            {EventType.TitleText, "Title Text"},
            {EventType.CaptionText, "Caption Text"},
            {EventType.CaptionText2, "* Caption Text 2"},
            {EventType.MoveMap, "Move Map"},
            {EventType.ZoomMap, "Zoom Map"},
            {EventType.ClearFGTags, "Clear FG Tags"},
            {EventType.FGTag1, "FG Tag 1"},
            {EventType.FGTag2, "FG Tag 2"},
            {EventType.FGTag3, "FG Tag 3"},
            {EventType.FGTag4, "FG Tag 4"},
            {EventType.ClearTextTags, "Clear Text Tags"},
            {EventType.TextTag1, "Text Tag 1"},
            {EventType.TextTag2, "Text Tag 2"},
            {EventType.TextTag3, "Text Tag 3"},
            {EventType.TextTag4, "Text Tag 4"},
            {EventType.EndBriefing, "End Briefing"}
        };

		/// <summary>The types available to a <see cref="BriefingPage"/>.</summary>
        public enum PageType : short
        {
			/// <summary>Renders a briefing map</summary>
            Map = 0,
			/// <summary>Renders text</summary>
            Text = 1
        };

        //This data table assists in converting X-wing briefing events to TIE Fighter briefing events for use in conversion.
		//Disp:  Index in the briefing editor events dropdown list.
		//XWID:  Event ID in Xwing
		//TIEID: Event ID in TIE Fighter if it exists (zero for no TIE equivalent)
		//PARAMS: Event parameter count in X-wing
		//-- Credits to the XWVM team for providing documentation of these events.
		//TODO: see if this needs to be const, maybe [][] or [,]
		/// <summary>Array to convert X-wing and TIE Briefing events</summary>
		public short[] EventMapper = {  //19 events, 4 columns each
		// DISP  XWID  TIEID  PARAMS       NOTES
			 0,    0,    0,   0,
			 1,    1,    0,   0,  //01: Wait For Click. (No params)   --> none (Doesn't exist in TIE)
			 2,   10,  0x11,   0,  //10: Clear Texts Boxes (No params) --> Clear Text Tags
			 3,   11,  0x04,   1,  //11: Display Title (textId)        --> Title Text (textId)
			 4,   12,  0x05,   1,  //12: Display Main Text (textId)    --> Caption Text (textId)
			 5,   14,  0x05,   1,  //14: Display Main Text 2 (textId)  --> Caption Text (textId)
			 6,   15,  0x06,   2,  //15: Center Map (x, y)             --> Move Map (X,Y)
			 7,   16,  0x07,   2,  //16: Zoom Map (xFactor, yFactor)   --> Zoom Map (X,Y)
			 8,   21,  0x08,   0,  //21: Clear Highlights (No params)  --> Clear FG Tags
			 9,   22,  0x09,   1,  //22: Set highlight 1 (objectId)    --> FG Tag 1 (FGIndex)
			10,   23,  0x0A,   1,  //23: Set highlight 2 (objectId)    --> FG Tag 2 (FGIndex)
			11,   24,  0x0B,   1,  //24: Set highlight 3 (objectId)    --> FG Tag 3 (FGIndex)
			12,   25,  0x0C,   1,  //25: Set highlight 4 (objectId)    --> FG Tag 4 (FGIndex)
			13,   26,  0x11,   0,  //26: Clear Tags (No params)        --> Clear Text Tags
			14,   27,  0x12,   3,  //27: Create Tag 1 (tagId, x, y)    --> Text Tag 1 (tag, color, x, y)
			15,   28,  0x13,   3,  //28: Create Tag 2 (tagId, x, y)    --> Text Tag 2 (tag, color, x, y)
			16,   29,  0x14,   3,  //29: Create Tag 3 (tagId, x, y)    --> Text Tag 3 (tag, color, x, y)
			17,   30,  0x15,   3,  //30: Create Tag 4 (tagId, x, y)    --> Text Tag 4 (tag, color, x, y)
			18,   41,  0x22,   0   //41: End marker                    --> End Briefing
		};

        /// <summary>The number of parameters per event type</summary>
        new protected EventParameters _eventParameters;

		/// <summary>Initializes a blank Briefing</summary>
		public Briefing()
		{   //initialize
			MaxCoordSet = 2;
            pages = new List<BriefingPage>();
            pages.Add(new BriefingPage());
            pages[0].SetDefaultFirstPage();

            windowSettings = new List<BriefingUIPage>();
            ResetUISettings(2);

			_eventParameters = new EventParameters();
            _platform = MissionFile.Platform.Xwing;
            _events = new short[0x190];
			Length = 0x21C;	//default 45 seconds
			_briefingTags = new string[0x20];
			_briefingStrings = new string[0x20];
			for (int i=0;i<0x20;i++)
			{
				_briefingTags[i] = "";
				_briefingStrings[i] = "";
			}
		}

		#region public methods
		/// <summary>Re-initialize <see cref="BaseBriefing.BriefingTag"/> to the given size.</summary>
		/// <param name="count">The new size, max <b>32</b></param>
		/// <remarks>All tags initialized as empty.</remarks>
		public void ResizeTagList(int count)
		{
			if(count < 32) count = 32;
			_briefingTags = new string[count];
			for(int i = 0; i < count; i++)
				_briefingTags[i] = "";
		}
		/// <summary>Re-initialize <see cref="BaseBriefing.BriefingString"/> to the given size.</summary>
		/// <param name="count">The new size, max <b>32</b></param>
		/// <remarks>All strings initialized as empty.</remarks>
		public void ResizeStringList(int count)
		{
			if(count < 32) count = 32;
			_briefingStrings = new string[count];
			for(int i = 0; i < count; i++)
				_briefingStrings[i] = "";
		}

		/// <summary>Adjust FG references as necessary.</summary>
		/// <param name="fgIndex">Original index</param>
		/// <param name="newIndex">Replacement index</param>
		public override void TransformFGReferences(int fgIndex, int newIndex)
        {
            bool deleteCommand = false;
            if (newIndex < 0)
                deleteCommand = true;

            for (int page = 0; page < pages.Count; page++)
            {
                BriefingPage pg = pages[page];
                int p = 0, advance = 0;
                int paramCount = 0;
                int briefLen = GetEventsLength(page);
                while (p < briefLen)
                {
                    int evt = pg.Events[p + 1];
                    if (IsEndEvent(evt))
                        break;

                    advance = 2 + EventParameterCount(evt);
                    if (IsFGTag(evt))
                    {
                        if (pg.Events[p + 2] == fgIndex)
                        {
                            if (deleteCommand == false)
                            {
                                pg.Events[p + 2] = (short)newIndex;
                            }
                            else
                            {
                                int len = GetEventsLength(page); //get() walks the event list, so cache the current value as the modifications will temporarily corrupt it
                                paramCount = 2 + EventParameterCount(evt);
                                for (int i = p; i < len - paramCount; i++)
                                    pg.Events[i] = pg.Events[i + paramCount];  //Drop everything down
                                for (int i = len - paramCount; i < len; i++)
                                    pg.Events[i] = 0;  //Erase the final space
                                advance = 0;
                            }
                        }
                        else if (pg.Events[p + 2] > fgIndex && deleteCommand == true)
                        {
                            pg.Events[p + 2]--;
                        }
                    }
                    p += advance;
                }
            }
        }

		/// <summary>Gets a string with highlight brackets inserted as necessary</summary>
		/// <param name="text">The text to process</param>
		/// <param name="highlight">The array of highlight information.</param>
		/// <returns>THe process string with "[" and "]" inserted</returns>
		/// <exception cref="ArgumentException"><i>text</i> and <i>highlight</i> are not the same length</exception>
		public string TranslateHighlightToString(string text, byte[] highlight)
		{
			if(highlight.Length != text.Length)
				throw new ArgumentException("String highlight data does not match string length.");

			string process = "";
			bool isHighlighted = false;
			int start = 0;
			int end = 0;
			
			for (int i = 0; i < highlight.Length; i++)
			{
				if (highlight[i] == 1)
				{
					if (!isHighlighted)
					{
						isHighlighted = true;
						start = i;
						process += text.Substring(end, start - end) + "["; //from the old end position
					}
				}
				else
				{
					if (isHighlighted)
					{
						isHighlighted = false;
						end = i;
						process += text.Substring(start, end - start) + "]";
					}
				}
			}
			if (isHighlighted) //Didn't terminate?
				process += text.Substring(start, text.Length - start) + "]";
			else
				process += text.Substring(end, text.Length - end); //Last known end position

			return process;
		}
		/// <summary>Gets an array denoting highlighting information</summary>
		/// <param name="text">The string including brackets</param>
		/// <returns>An array with the length of <i>text</i> stripped of the highlighting brackets.
		/// A value of <b>0</b> is regular text, <b>1</b> is highlighted</returns>
        public byte[] TranslateStringToHighlight(string text)
        {	//TODO: this is really a bool[]
            int len = text.Length;
            byte[] temp = new byte[len];
            char[] tchar = text.ToCharArray();
            bool isHighlighted = false;
            int pos = 0;
            for (int i = 0; i < len; i++)
            {
                if (tchar[i] == '[')
                    isHighlighted = true;
                else if (tchar[i] == ']')
                    isHighlighted = false;
                else
                    temp[pos++] = (byte)(isHighlighted ? 1 : 0);
            }
            byte[] ret = new byte[pos];
            Array.Copy(temp, ret, pos);
            return ret;
        }
		/// <summary>Gets a string without the highlighting brackets.</summary>
		/// <param name="text">The string to convert.</param>
		/// <returns><i>text</i> without the "[" or "]" characters.</returns>
		public string RemoveBrackets(string text)
        {
            return text.Replace("[", string.Empty).Replace("]", string.Empty);
        }

		private short getEventMapperIndex(int eventID)
		{
			for(short i = 0; i < EventMapper.Length / 4; i++)
				if(EventMapper[(i*4)+1] == eventID)  //+1 for Column[1]
					return (short)(i*4);
			return 0;
		}

		/// <summary>Reads an briefing event and returns an array with all the information for that event.</summary>
		/// <param name="page">The index in <see cref="pages"/></param>
		/// <param name="rawOffset">The offset within <see cref="BriefingPage.Events"/></param>
		/// <remarks>The returned array contains exactly as many shorts as used by the event: time, event command, and variable length parameter field.</remarks>
        /// <returns>A short[] array of equal size to the exact resulting command length.</returns>
		public short[] ReadBriefingEvent(int page, int rawOffset)
		{
			short evtTime = pages[page]._events[rawOffset++];
            short evtCommand = pages[page]._events[rawOffset++];
			short mapperOffset = getEventMapperIndex(evtCommand);
			int paramCount = EventMapper[mapperOffset+3]; //Column[3] is param counts
			short[] retEvent = new short[2 + paramCount];
			int writePos = 0;
			retEvent[writePos++] = evtTime;
			retEvent[writePos++] = evtCommand;
			for(int i = 0; i < paramCount; i++)
                retEvent[writePos++] = pages[page]._events[rawOffset++];
			return retEvent;
		}
		/// <summary>Takes an event from ReadBriefingEvent and translates it into a TIE95 compatible format, adjusting event IDs and parameter count as necessary.</summary>
		/// <param name="xwingEvent">The original Xwing events</param>
		/// <remarks>TextTag Color is a placeholder and may need to be modified by the calling platform.
        /// XvT and XWA viewports are larger than XWING95 and TIE95.  Zoom Map event parameters may need to be scaled.
        /// XWA Move Map event parameters may need to be scaled.</remarks>
        /// <returns>A short[] array of equal size to the exact resulting command length.</returns>
		public short[] TranslateBriefingEvent(short[] xwingEvent)
		{
			short rpos = 0, wpos = 0;
			short evtTime = xwingEvent[rpos++];
			short evtCommand = xwingEvent[rpos++];
			short tieCommand = 0;
			short xwParams = 0;
			short tieParams = 0;
			Tie.Briefing.EventParameters TIEEventParameters = new Tie.Briefing.EventParameters();
			short mapperOffset = getEventMapperIndex(evtCommand);
			tieCommand = EventMapper[mapperOffset + 2];
			tieParams = TIEEventParameters[tieCommand];
			xwParams = EventMapper[mapperOffset + 3];

			short[] retEvent = new short[2 + tieParams];
			retEvent[wpos++] = evtTime;
			retEvent[wpos++] = tieCommand;
			if(xwParams == tieParams)
			{
				for(int i = 0; i < xwParams; i++)
					retEvent[wpos++] = xwingEvent[rpos++];
			}
			else
			{
                if (evtCommand >= (int)EventType.TextTag1 && evtCommand <= (int)EventType.TextTag4)
                {
                    retEvent[wpos++] = xwingEvent[rpos++];  //tagId
                    retEvent[wpos++] = xwingEvent[rpos++];  //x
                    retEvent[wpos++] = xwingEvent[rpos++];  //y
                    retEvent[wpos++] = 0;                   //color (placeholder).  Acceptable values are platform-dependent and should be set in the converting function.
                }
                else throw new Exception("Unhandled instruction");
			}
			return retEvent;
		}

		/// <summary>Gets the indicated page.</summary>
		/// <param name="index">The page index</param>
		/// <returns>The indicated page.</returns>
		/// <exception cref="IndexOutOfRangeException"><i>index</i> is not valid.</exception>
        public BriefingPage GetBriefingPage(int index)
        {
            if(index < 0 || index >= pages.Count) throw new IndexOutOfRangeException("Briefing page index out of range.");
            return pages[index];
        }
		/// <summary>Re-initalizes the <see cref="BriefingPage"/> listing with the given size.</summary>
		/// <param name="count">The number of pages.</param>
        public void ResetPages(int count)
        {
			if (count < 0) count = 1;
            pages = new List<BriefingPage>();
			for (int i = 0; i < count; i++) { pages.Add(new BriefingPage()); }
			pages[0].SetDefaultFirstPage();
        }
		/// <summary>Gets the total number of values occupied in the indicated <see cref="BriefingPage"/>.</summary>
		/// <param name="page">The <see cref="BriefingPage"/> index</param>
		/// <returns>The number of values in the event listing up through <see cref="EventType.EndBriefing"/>.</returns>
        public int GetEventsLength(int page)
        {
            BriefingPage pg = GetBriefingPage(page);
            int pos = 0;
            while(pos < pg._events.Length)
            {
                pos++; //skip event time
                int evt = pg._events[pos++];
                if(evt == (int)EventType.None)
                    return pos - 2;
                else if(evt == (int)EventType.EndBriefing)
                    return pos;
                else
                    pos += EventParameterCount(evt);
            }
            return pos;
        }

		/// <summary>Resets the pages to default.</summary>
		/// <param name="pageTypeCount">The number of page types, must be at least <b>2</b>.</param>
		/// <remarks>The first will be a default <see cref="PageType.Map"/>, the second will be a default <see cref="PageType.Text"/>.</remarks>
        public void ResetUISettings(int pageTypeCount)
        {
            //Default to 2 page types, Map and Text.
            if (pageTypeCount < 2)
                pageTypeCount = 2;
            windowSettings.Clear();
            for (int i = 0; i < pageTypeCount; i++)
                windowSettings.Add(new BriefingUIPage());

            windowSettings[0].SetDefaultsToMapPage();
            windowSettings[1].SetDefaultsToTextPage();
        }

		/// <summary>Gets the index of the matching <see cref="BaseBriefing.BriefingString"/>.</summary>
		/// <param name="str">The exact text to search for.</param>
		/// <returns>The index if found, otherwise <b>-1</b>.</returns>
        public int FindStringList(string str)
        {
            for (int i = 0; i < 32; i++)
                if (_briefingStrings[i] == str)
                    return i;
            return -1;
        }
		/// <summary>Gets the index of the matching <see cref="BaseBriefing.BriefingString"/> or the next available empty position.</summary>
		/// <param name="str">The exact text to search for.</param>
		/// <returns>The index if found, otherwise the next empty string. If an empty position does not exist, returns <b>-1</b>.</returns>
		public int GetOrCreateString(string str)
        {
            int index = FindStringList(str);
            if (index != -1)
                return index;

            index = FindStringList("");
            if (index != -1)
               _briefingStrings[index] = str;

           return index;
        }

		/// <summary>Returns the string name of a particular <see cref="EventType"/>.</summary>
		/// <param name="eventCommand">The event</param>
		/// <returns>The event type, otherwise <b>"Unknown (<i>eventCommand</i>)"</b>.</returns>
		public string GetEventTypeAsString(EventType eventCommand)
		{
			if (_eventTypeStringMap.ContainsKey(eventCommand))
				return _eventTypeStringMap[eventCommand];
			return "Unknown(" + eventCommand + ")";
		}
		/// <summary>Gets the possible <see cref="EventType">EventTypes</see>, excluding the first entry (None).</summary>
		/// <returns>An array of the possible values.</returns>
		public string[] GetUsableEventTypeStrings()
		{
			string[] ret = new string[_eventTypeStringMap.Count - 1];
			int pos = -1;
			foreach (KeyValuePair<EventType, string> dat in _eventTypeStringMap)
			{
				if (pos == -1)
				{
					pos = 0;
					continue;
				}
				ret[pos++] = dat.Value;
			}
			return ret;
		}
		/// <summary>Gets the index from the string value of the <see cref="EventType"/></summary>
		/// <param name="name">The event's string value.</param>
		/// <returns>The raw index value.</returns>
		public int GetEventTypeByName(string name)
		{
			foreach (KeyValuePair<EventType, string> dat in _eventTypeStringMap)
				if (dat.Value == name)
					return (int)dat.Key;
			return (int)EventType.None;
		}

		/// <summary>Gets if the selected <see cref="BriefingPage"/> has a visible <see cref="BriefingUIPage.Elements.Map"/></summary>
		/// <param name="page">The selected page</param>
		/// <returns><b>true</b> is it's visible. Any errors or otherwise returns <b>false</b>.</returns>
		public bool IsMapPage(int page)
		{
			if (page < 0 || page >= pages.Count) return false;
			if (pages[page].PageType >= 0 && pages[page].PageType < windowSettings.Count)
				return (windowSettings[pages[page].PageType].GetElement(BriefingUIPage.Elements.Map).visible == 1);
			return false;
		}

		/// <summary>Enumerates a list of caption strings found in a briefing page.</summary>
		/// <param name="page">The index of <see cref="pages"/></param>
		/// <param name="captionText">The collection of captions</param>
		/// <remarks>This is a helper function for use in converting missions.</remarks>
		public void GetCaptionText(int page, out List<string> captionText)
		{
			captionText = new List<string>();
			if (page < 0 || page >= pages.Count) return;

			BriefingPage pg = pages[page];
			int length = pg.EventsLength;
			int rpos = 0;
			while (rpos < length)
			{
				short[] xwevt = ReadBriefingEvent(page, rpos);
				rpos += xwevt.Length;
				if (xwevt[1] == (short)EventType.CaptionText)
					captionText.Add(BriefingString[xwevt[2]]);
				else if (xwevt[1] == 0 || xwevt[1] == (short)EventType.EndBriefing)
					break;
			}
		}

		/// <summary>Determines if the given caption text is a potential hint page.</summary>
		/// <param name="captionText">The text to check</param>
		/// <returns><b>true</b> if <i>captionText</i> contains ">STRATEGY AND TACTICS".</returns>
		/// <remarks>This is a helper function for use in converting missions.  Intended for use with strings extracted via GetCaptionText().</remarks>
		public bool ContainsHintText(string captionText)
		{
			return (captionText.ToUpper().IndexOf(">STRATEGY AND TACTICS") >= 0);
		}

		/// <summary>Gets if the specified event denotes the end of the briefing.</summary>
		/// <param name="evt">The event index</param>
		/// <returns><b>true</b> if <i>evt</i> is <see cref="EventType.EndBriefing"/> or <see cref="EventType.None"/>.</returns>
		public override bool IsEndEvent(int evt)
        {
            return (evt == (int)EventType.EndBriefing || evt == (int)EventType.None);
        }
		/// <summary>Gets if the specified event denotes one of the FlightGroup Tag events.</summary>
		/// <param name="evt">The event index</param>
		/// <returns><b>true</b> if <i>evt</i> is <see cref="EventType.FGTag1"/> through <see cref="EventType.FGTag4"/>.</returns>
		public override bool IsFGTag(int evt)
        {
            return (evt >= (int)EventType.FGTag1 && evt <= (int)EventType.FGTag4);
        }

		/// <summary>Gets the number of parameters for the specified event type</summary>
		/// <param name="eventCommand">The briefing event</param>
		/// <exception cref="IndexOutOfRangeException">Invalid <i>eventCommand</i> value</exception>
		/// <returns>The number of parameters</returns>
		public int EventCount(short eventCommand)
		{
			//TODO: this is OBS, since it's just using EPC()
			//byte convertCommand = EventMap(eventCommand);
			//if(convertCommand == 1) return 0;
			//if(convertCommand == 2) return 2;
			return EventParameterCount(eventCommand);
		}

		/// <summary>Gets the number of parameters for the specified event type</summary>
		/// <param name="eventType">The briefing event</param>
		/// <exception cref="IndexOutOfRangeException">Invalid <i>eventType</i> value</exception>
		/// <returns>The number of parameters</returns>
		override public byte EventParameterCount(int eventType) { return _eventParameters[eventType]; }
		#endregion public methods

		/// <summary>DO NOT USE. Will always throw an exception</summary>
		/// <exception cref="InvalidOperationException">Throws on any get attempt.</exception>
		new public short EventsLength
		{
			get
			{
				throw new InvalidOperationException("Warning! EventsLength is not used for X-wing briefings. If you see this message, please file a bug report.");
			}
		}

		/// <summary>Frames per second for briefing animation</summary>
		/// <remarks>Value is <b>8 (0x8)</b></remarks>
		public const int TicksPerSecond = 0x8;
		/// <summary>Maximum number of events that can be held</summary>
		/// <remarks>Value is <b>200 (0xC8)</b></remarks>
		public const int EventQuantityLimit = 0xC8;

		/// <summary>The number of CoordinateSets in the Briefing.</summary>
		/// <remarks>Defaults to <b>2</b>.</remarks>
		public short MaxCoordSet { get; set; }
		/// <summary>Gets or sets where the mission takes place.</summary>
		/// <remarks>Same as <see cref="Mission.Location"/>. Will affect the briefing background.</remarks>
		public short MissionLocation { get; set; }

		/// <summary>Object to maintain a read-only array</summary>
		new public class EventParameters
		{
			//0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41
			byte[] _counts = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 2, 2, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

			/// <summary>Gets a parameter count</summary>
			/// <param name="eventType">The briefing event</param>
			/// <exception cref="IndexOutOfRangeException">Invalid <i>eventType</i> value</exception>
			public byte this[int eventType] { get { return _counts[eventType]; } }

			/// <summary>Gets a parameter count</summary>
			/// <param name="eventType">The briefing event</param>
			public byte this[EventType eventType] { get { return _counts[(int)eventType]; } }
		}
	}

	//Thanks to the XWVM team for providing documentation on this block.
	/// <summary>Object for element location and dimensions</summary>
	public class BriefingUIItem
	{	// TODO: should this be a struct instead?
		/// <summary>Gets or sets the top pixel location</summary>
		public short top;
		/// <summary>Gets or sets the left pixel location</summary>
		public short left;
		/// <summary>Gets or sets the bottom pixel location</summary>
		public short bottom;
		/// <summary>Gets or sets the right pixel location</summary>
		public short right;
		/// <summary>Gets or sets the visibility</summary>
		/// <remarks>Visible is <b>01</b>, not used (invisible) is <b>00</b>.</remarks>
		public short visible;

		/// <summary>Gets the visibility</summary>
		/// <returns><b>true></b> if <see cref="visible"/> is <b>00</b>.</returns>
		public bool IsVisible()
		{
			return (visible != 0);
		}
	}

	/// <summary>Object for the window settings</summary>
	public class BriefingUIPage
	{
		/// <summary>Gets or sets the viewport settings array</summary>
		public BriefingUIItem[] items;	//TODO: check access

		/// <summary>The valid viewport elements</summary>
		public enum Elements
		{
			/// <summary>The title text</summary>
			Title = 0,
			/// <summary>The caption text</summary>
			Text,
			/// <summary>Unused</summary>
			Unused1,
			/// <summary>Unused</summary>
			Unused2,
			/// <summary>The briefing map</summary>
			Map
		}

		/// <summary>Initializes a new window</summary>
		public BriefingUIPage()
		{
			items = new BriefingUIItem[5];
			for (int i = 0; i < 5; i++)
				items[i] = new BriefingUIItem();

		}

		/// <summary>Gets the item via the enumerated value</summary>
		/// <param name="item">The specified element value</param>
		/// <returns>The requested item</returns>
		public BriefingUIItem GetElement(Elements item)
		{
			return items[(int)item];
		}
		/// <summary>Gets the type of page</summary>
		/// <returns>If the <see cref="Elements.Map"/> viewport is visible, "Map", otherwise "Text".</returns>
		public string GetPageDesc()
		{
			return (items[(int)Elements.Map].visible == 1) ? "Map" : "Text";
		}
		/// <summary>Assigns default map data</summary>
		/// <remarks>Sets the <see cref="Elements.Map"/> visibility to <b>1</b>.</remarks>
		public void SetDefaultsToMapPage()
		{
			short[,] defData = { {0, 0, 12, 212, 1},
								 {115, 0, 138, 212, 1},
								 {0, 0, 0, 0, 0},
								 {0, 0, 0, 0, 0},
								 {12, 0, 115, 212, 1} };
			setRawData(defData);
		}
		/// <summary>Assigns default text data</summary>
		/// <remarks>Sets the <see cref="Elements.Map"/> visibility to <b>0</b>.</remarks>
		public void SetDefaultsToTextPage()
		{
			short[,] defData = { {0, 0, 12, 212, 1},
								 {12, 0, 138, 212, 1},
								 {0, 0, 0, 0, 0},
								 {0, 0, 0, 0, 0},
								 {12, 0, 114, 212, 0} };
			setRawData(defData);
		}

		/// <summary>Saves the array data to the items</summary>
		/// <param name="data">A [5,5] array of viewport settings</param>
		/// <exception cref="ArgumentException"><i>data</i>'s dimensions are not at least [5,5].</exception>
		void setRawData(short[,] data)
		{
			if (data.GetLength(0) < 5 || data.GetLength(1) < 5)	//~MG was 0 and 0
				throw new ArgumentException("Not enough data elements.");
			for (int i = 0; i < 5; i++)
			{
				BriefingUIItem item = items[i];
				item.top = data[i, 0];
				item.left = data[i, 1];
				item.bottom = data[i, 2];
				item.right = data[i, 3];
				item.visible = data[i, 4];
			}
		}
	}

	/// <summary>Represents the instructions used to display the briefing</summary>
	[Serializable]
	public class BriefingPage
	{
		/// <summary>The raw events</summary>
		internal short[] _events;

		/// <summary>Initalizes a new object</summary>
		/// <remarks><see cref="Events"/> is initalized to a length of 0x190</remarks>
		public BriefingPage()
		{
			_events = new short[0x190];
		}

		/// <summary>Set the initial events and <see cref="Briefing.EventType.EndBriefing"/></summary>
		/// <remarks><see cref="CoordSet"/> is set to <b>1</b>, duration is set to <b>45 seconds</b>.
		/// Initial events center map on (0,0), move map to (0x30, 0x30) and apply the end marker at time 9999</remarks>
		public void SetDefaultFirstPage()
		{
			CoordSet = 1;
			Length = 0x21C; //default 45 seconds
			_events[1] = 15;  //Center map
			_events[5] = 16;  //Move
			_events[6] = 0x30;
			_events[7] = 0x30;
			_events[8] = 9999;
			_events[9] = 41;  //End marker
		}
		
		/// <summary>Gets the raw even data</summary>
		public short[] Events { get { return _events; } }
		/// <summary>Gets or sets the briefing length in ticks</summary>
		/// <remarks>Xwing uses 8 ticks per second</remarks>
		public short Length { get; set; }
		/// <summary>Gets or sets the total number of values occupied in <see cref="Events"/>.</summary>
		public short EventsLength { get; set; }
		/// <summary>Gets or sets the applicable Waypoint coordinate set index</summary>
		public short CoordSet { get; set; }
		/// <summary>Gets or sets if the page is <see cref="Briefing.PageType.Map"/> or <see cref="Briefing.PageType.Text"/></summary>
		public short PageType { get; set; }  //TODO: this should be handled as the enum
	}
}
