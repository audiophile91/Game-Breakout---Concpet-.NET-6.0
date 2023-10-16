using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Breakout
{
    public class Settings : INotifyPropertyChanged
    {
        #region Referentions

        public Canvas GameSpace { get; }

        #endregion

        public Settings(Canvas gameSpace)
        {
            GameSpace = gameSpace;
        }

        #region Properties






        #endregion

        #region Getters

        public double GameHeight => GameSpace.ActualHeight;
        public double GameWidth => GameSpace.ActualWidth;

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        #endregion
    }
}
