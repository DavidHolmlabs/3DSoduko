using System;
using System.ComponentModel;

namespace Soduko
{
    public class SodukoValue
    {
        private string m_Value = SodukoSet.EmptyValue;

        public string Value
        {
            get { return m_Value; }
            set
            {
                if (Locked)
                    throw new Exception("Locked value can't be changed");
                if (!SodukoSet.IsValidValue(value))
                    throw new ArgumentOutOfRangeException($"{value} is not a valid soduko value");
                m_Value = value;
            }
        }

        public bool Locked { get; set; } = false;
    }
}
