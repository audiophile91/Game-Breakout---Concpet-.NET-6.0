using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Breakout
{
    public class Designer : INotifyPropertyChanged
    {
        #region Referentions

        private Info Info { get; }
        private Mechanics Mechanics { get; }
        private Settings Settings { get; }
        private ObservableCollection<Ball> Balls { get; }
        private ObservableCollection<Block> Blocks { get; }
        private ObservableCollection<Board> Boards { get; } 

        #endregion

        public Designer(Info info, Mechanics mechanics, Settings settings, ObservableCollection<Ball> balls, ObservableCollection<Block> blocks, ObservableCollection<Board> boards)
        {
            Info = info;
            Mechanics = mechanics;
            Settings = settings;
            Balls = balls;
            Blocks = blocks;
            Boards = boards;
        }




        public void UpdateDesigner()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
