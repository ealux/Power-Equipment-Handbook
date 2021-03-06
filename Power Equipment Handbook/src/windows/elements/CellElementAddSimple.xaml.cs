﻿using System.Windows;
using System.Windows.Controls;

namespace Power_Equipment_Handbook.src.windows
{
    /// <summary>
    /// Логика взаимодействия для CellElementAddSimple.xaml
    /// </summary>
    public partial class CellElementAddSimple : Window
    {
        readonly Cell cell;

        /// <summary>
        /// Конструктор окна упрощенного добавления оборудования
        /// </summary>
        /// <param name="cell">Передаваемый элемент ячейки для добавления</param>
        public CellElementAddSimple(ref Cell cell)
        {
            InitializeComponent();
            this.cell = cell;
        }

        /// <summary>
        /// Выбор типа оборудования для добавления
        /// </summary>
        private void btnAddElemSimple_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;

            if (btn == this.btnBreaker)
            {
                var added = new BreakerCell(tvkl: null, totkl: null,
                                            name: "_Выключатель_ячейки_", inom: null, unom: this.cell.Unom,
                                            iotkl: null, iterm: null, iudar: null,
                                            tterm: null, bterm: null);
                this.cell.CellElements.Add(added);
                this.Close();
            }
            else if (btn == this.btnDisconnector)
            {
                var added = new DisconnectorCell(name: "_Разъединитель_ячейки_", inom: null, unom: this.cell.Unom,
                                                iotkl: null, iterm: null, iudar: null,
                                                tterm: null, bterm: null);
                this.cell.CellElements.Add(added);
                this.Close();
            }
            else if (btn == this.btnSC)
            {
                var added = new ShortCircuiterCell(totkl:null,
                                                   name: "_Отдел./Короткозамык._ячейки_", inom: null, unom: this.cell.Unom,
                                                   iotkl: null, iterm: null, iudar: null,
                                                   tterm: null, bterm: null);
                this.cell.CellElements.Add(added);
                this.Close();
            }
            else if (btn == this.btnTT)
            {
                var added = new TTCell(iperv:0, ivtor:0,
                                       name: "_Трансформатор_тока_ячейки_", inom: null, unom: this.cell.Unom,
                                       iotkl: null, iterm: null, iudar: null,
                                       tterm: null, bterm: null);
                this.cell.CellElements.Add(added);
                this.Close();
            }
            else if (btn == this.btnBusbar)
            {
                var added = new BusbarCell(name: "_Ошиновка_ячейки_", inom: null, unom: this.cell.Unom,
                                           iotkl: null, iterm: null, iudar: null,
                                           tterm: null, bterm: null);
                this.cell.CellElements.Add(added);
                this.Close();
            }
        }
    }
}
