using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lunar_Lander
{

    [DataContract(Name = "Score")]
    public class Score : IComparable<Score>
    {
        /// <summary>
        /// Have to have a default constructor for the XmlSerializer.Deserialize method
        /// </summary>
        public Score()
        {
        }

        /// <summary>
        /// Overloaded constructor used to create an object for long term storage
        /// </summary>
        /// <param name="value"></param>
        /// <param name="poles"></param>
        public Score(long value, int poles)
        {
            Value = value;
            Poles = poles;
            TimeStamp = DateTime.Now;

            keys.Add(1, "one");
            keys.Add(2, "two");
        }

        [DataMember()]
        public long Value { get; set; }

        [DataMember()]
        public int Poles { get; set; }
        [DataMember()]
        public DateTime TimeStamp { get; set; }

        [DataMember()]
        public Dictionary<int, string> keys = new Dictionary<int, string>();

        
        public int CompareTo(Score other)
        {
            if (other == null) return 1;
            if (this == other) return 0;
            // Higher score wins
            return (int)(this.Value - other.Value);
        }

        // Define the is greater than operator.
        public static bool operator >(Score operand1, Score operand2)
        {
            return operand1.CompareTo(operand2) > 0;
        }

        // Define the is less than operator.
        public static bool operator <(Score operand1, Score operand2)
        {
            return operand1.CompareTo(operand2) < 0;
        }
    }
}
