using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Power_Equipment_Handbook.src
{
    /// <summary>
    /// Абстрактный класс элемента Ячейки узла
    /// </summary>
    [XmlInclude(type:typeof(BreakerCell)), XmlInclude(type: typeof(DisconnectorCell)),
     XmlInclude(type: typeof(ShortCircuiterCell)), XmlInclude(type: typeof(TTCell)), XmlInclude(type: typeof(BusbarCell))]
    public abstract class Element : INotifyPropertyChanged
    {
        string name;
        double? inom, iotkl, iterm, tterm, bterm, iudar;
        double unom;        

        #region Properties

        [XmlAttribute] public string Name { get => name; set => SetProperty(ref name, value); }
        [XmlIgnore] public double? Inom { get => inom; set => SetProperty(ref inom, value); }   
        [XmlIgnore] public double? Iotkl { get => iotkl; set => SetProperty(ref iotkl, value); }
        [XmlIgnore] public double? Iterm { get => iterm; set => SetProperty(ref iterm, value); }
        [XmlIgnore] public double? Tterm { get => tterm; set => SetProperty(ref tterm, value); }
        [XmlIgnore] public double? Bterm { get => bterm; set => SetProperty(ref bterm, value); }
        [XmlIgnore] public double? Iudar { get => iudar; set => SetProperty(ref iudar, value); }
        [XmlAttribute] public double Unom { get => unom; set => SetProperty(ref unom, value); }

        #region Properties to Text (serialize)
        [XmlElement("Inom")] public string InomAsText { get => (Inom.HasValue) ? Inom.ToString() : null; set => Inom = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }
        [XmlElement("Iotkl")] public string IotklAsText { get => (Iotkl.HasValue) ? Iotkl.ToString() : null; set => Iotkl = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }
        [XmlElement("Iterm")] public string ItermAsText { get => (Iterm.HasValue) ? Iterm.ToString() : null; set => Iterm = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }
        [XmlElement("Tterm")] public string TtermAsText { get => (Tterm.HasValue) ? Tterm.ToString() : null; set => Tterm = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }
        [XmlElement("Bterm")] public string BtermAsText { get => (Bterm.HasValue) ? Bterm.ToString() : null; set => Bterm = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }
        [XmlElement("Iudar")] public string IudarAsText { get => (Iudar.HasValue) ? Iudar.ToString() : null; set => Iudar = !string.IsNullOrEmpty(value) ? double.Parse(value) : default(double?); }
        #endregion Properties to Text (serialize)

        #endregion Properties


        /// <summary>
        /// Конструктор пустой (для сериализации)
        /// </summary>
        public Element() { }

        /// <summary>
        /// Конструктор элемента ячейки
        /// </summary>
        protected Element(string name, double? inom, double unom,
                          double? iotkl, double? iterm, double? iudar,
                          double? tterm, double? bterm)
        {
            this.name = name;
            this.inom = inom;
            this.iotkl = iotkl;
            this.iterm = iterm;
            this.tterm = tterm;
            this.bterm = bterm;
            this.iudar = iudar;
            this.unom = unom;
        }

        

        #region INotifyPropertyChanged interface block

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value; OnPropertyChanged(propertyName); return true;
        }

        #endregion INotifyPropertyChanged interface block

    }
}
