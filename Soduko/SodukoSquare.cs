using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Soduko
{
    [DebuggerDisplay("{Value}")]
    public class SodukoSquare
    {
        private SodukoValue m_Value;
        private bool? m_Valid;
        private Index m_Index;
        private List<Verifyer> Verifyables = new List<Verifyer>();

        public SodukoSquare(Index index)
        {
            m_Value = new SodukoValue();
            m_Index = index;
        }

        internal void Verify()
        {
            bool valid = true;
            foreach (var verifyable in Verifyables)
            {
                if (!verifyable.Verify())
                    valid = false;
            }
            Valid = valid;
        }

        public List<string> GetFreeValues()
        {
            List<string> all = new List<string>();
            foreach (var item in Verifyables)
            {
                all.AddRange(item.GetValues());
            }

            return SodukoSet.AllValues().Except(all.Distinct()).ToList();
        }

        public bool? Valid
        {
            get { return m_Valid; }
            set { m_Valid = value; }
        }

        internal void AddVerifyer(Verifyer v)
        {
            Verifyables.Add(v);
        }

        public string Value
        {
            get { return m_Value.Value; }
            set { m_Value.Value = value; }
        }

        [Newtonsoft.Json.JsonIgnore]
        public Index Index => m_Index;

        public bool Locked
        {
            get { return m_Value.Locked; }
            set { m_Value.Locked = value; }
        }

        internal void Lock() => m_Value.Locked = true;

        internal void Unlock() => m_Value.Locked = false;
    }
}
