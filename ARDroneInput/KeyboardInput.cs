/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SlimDX.DirectInput;
using ARDrone.Input.InputMappings;

namespace ARDrone.Input
{
	public class KeyboardInput : DirectInputInput
    {
        public enum Axis
        {
            // "Normal" axes
            Axis_X, Axis_Y, Axis_Z,
        }

        protected ArrayList keysPressedBefore = new ArrayList();

		public static List<GenericInput> GetNewInputDevices(IntPtr windowHandle, List<GenericInput> currentDevices)
        {
            List<GenericInput> newDevices = new List<GenericInput>();
			DirectInput di = new DirectInput();
			List<DeviceInstance> keyboardControllerList = (List<DeviceInstance>)di.GetDevices(DeviceClass.Keyboard, DeviceEnumerationFlags.AttachedOnly);
            for (int i = 0; i < keyboardControllerList.Count; i++)
            {
                //keyboardControllerList.MoveNext();
                //DeviceInstance deviceInstance = (DeviceInstance)keyboardControllerList.Current;

				Keyboard kb = new Keyboard(di);
				Device device = (Device)kb;
			

				if (!CheckIfDirectInputDeviceExists(keyboardControllerList[i], currentDevices))
                {
                    AcquireDirectInputDevice(windowHandle, device, DeviceDataFormat.Keyboard);
                    KeyboardInput input = new KeyboardInput(device);

                    newDevices.Add(input);
                }
            }

            return newDevices;
        }

        public KeyboardInput(Device device) : base()
        {
            this.device = device;

            DetermineMapping();
        }

        protected override InputMapping GetStandardMapping()
        {
            ButtonBasedInputMapping mapping = new ButtonBasedInputMapping(GetValidButtons(), GetValidAxes());

			mapping.SetAxisMappings("A-D", "W-S", "LeftArrow-RightArrow", "DownArrow-UpArrow");
            mapping.SetButtonMappings("C", "Return", "Return", "NumPad0", "Space", "F", "X");

            return mapping;
        }

        private List<String> GetValidButtons()
        {
            List<String> validButtons = new List<String>();
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (!validButtons.Contains(key.ToString()))
                {
                    validButtons.Add(key.ToString());
                }
            }

            return validButtons;
        }

        private List<String> GetValidAxes()
        {
            return new List<String>();
        }

        public override List<String> GetPressedButtons()
        {
			Keyboard kbx = (Keyboard)device;
			KeyboardState state = kbx.GetCurrentState();
			
            List<String> buttonsPressed = new List<String>();
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
				if (state.IsPressed (key))
                {
                    if (!buttonsPressed.Contains(key.ToString()))
                    {
                        buttonsPressed.Add(key.ToString());
                    }
                }
            }

            return buttonsPressed;
        }

        public override Dictionary<String, float> GetAxisValues()
        {
            return new Dictionary<String, float>();
        }

        public override bool IsDevicePresent
        {
            get
            {
                try
                {
					Keyboard xkb = (Keyboard)device;
					KeyboardState currentState = xkb.GetCurrentState();
                    return true;
                }
				catch (DirectInputException)
                {
                    return false;
                }
            }
        }

        public override String DeviceName
        {
            get
            {
                if (device == null) { return string.Empty; }
                else { return "Keyboard"; }
            }
        }

        public override String FilePrefix
        {
            get
            {
                if (device == null) { return string.Empty; }
                else { return "KB"; }
            }
        }
    }
}