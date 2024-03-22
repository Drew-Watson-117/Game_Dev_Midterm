using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Midterm
{

    [DataContract(Name = "Controls")]
    public class Controls
    {
        public Controls()
        {
        }

        /// <summary>
        /// Overloaded constructor used to create an object for long term storage
        /// </summary>
        /// <param name="controlsDict"></param>
        public Controls(Dictionary<string, Keys> controlsDict)
        {
            ControlsDict = controlsDict;

            //keys.Add(1, "one");
            //keys.Add(2, "two");
        }

        [DataMember()]
        public Dictionary<string, Keys> ControlsDict { get; set;}
        //public (string,Keys)[] ControlsArray { get; set; }

    }
}
