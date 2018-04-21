using Soduko;
using System;
using System.ComponentModel;
using System.Linq;

namespace SodukoGui
{
    public class SingleSquareViewModel : INotifyPropertyChanged
    {
        SodukoSquare m_model;

        public SingleSquareViewModel()
        { }

        public string ToolTip => $"X: {m_model.Index.X + 1}, Y: {m_model.Index.Y + 1}, Z: {m_model.Index.Z + 1}";

        public string Value
        {
            get { return m_model.Value; }
            set
            {
                try
                {
                    bool changed = m_model.Value == value;
                    m_model.Value = value;
                    if (changed)
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(value)));
                }
                catch (ArgumentOutOfRangeException)
                { }
            }
        }

        public bool Locked => m_model.Locked;

        public bool Valid => m_model.Valid ?? true;

        internal void SetModel(SodukoSquare sodukoSquare)
        {
            m_model = sodukoSquare;
            FireAll();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void FireAll()
        {
            FirePropertyChanged(nameof(Value));
            FirePropertyChanged(nameof(Valid));
            FirePropertyChanged(nameof(ToolTip));
            FirePropertyChanged(nameof(Locked));
        }

        internal void FirePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
