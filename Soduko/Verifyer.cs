using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Soduko
{
    public class Verifyer : INotifyPropertyChanged
    {
        private bool m_Verifyes = true;

        private List<SodukoSquare> m_List;

        public Verifyer(IGet9List squares)
        {
            m_List = squares.To9List();
            if (m_List.Count != 9)
                throw new System.Exception("Here should be nine squares");
        }

        public IEnumerable<string> GetValues() =>
            m_List.Where(x => x.Value != SodukoSet.EmptyValue).Select(x => x.Value);

        public bool Verifyes
        {
            get { return m_Verifyes; }
            private set
            {
                bool changed = m_Verifyes != value;
                m_Verifyes = value;
                if (changed)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Verifyes)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Verify()
        {
            var xx = m_List.GroupBy(x => x.Value);
            var yy = xx.Any(x => x.Count() > 1 && x.Key != SodukoSet.EmptyValue);
            Verifyes = !yy;
            return Verifyes;
        }
    }
}
