using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Converters;
using System.Windows;

namespace Breakout
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl, INotifyPropertyChanged
    {
        #region Referention

        private Info Info { get; }
        private Mechanics Mechanics { get; }
        private Settings Settings { get; }

        #endregion

        private SpatialDatasetRectangle _data;
        public SpatialDatasetRectangle Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        public bool DirectionIsFree => !(Mechanics.BoardGoLeft && Data.IsOnEdgeLeft || Mechanics.BoardGoRight && Data.IsOnEdgeRight);

        public Board(Info info, Mechanics mechanics, Settings settings)
        {
            InitializeComponent();
            DataContext = this;
            Info = info;
            Mechanics = mechanics;
            Settings = settings;
            Data = new(Info, Settings, this, Info.InitialPointBoard);
            Settings.GameSpace.Children.Add(this);
        }

        public void Move()
        {
            // Move if momentum is present
            if (Mechanics.BoardHasMomentum)
            {
                Data.Position = new Point(Data.Position.X + Mechanics.BoardMomentum * Info.DeltaTime, Data.Position.Y);
            }
            // Wall crash test, reverse and recude momentum or stop
            if (Data.IsOnEdgeAxisX)
            {
                Mechanics.BoardMomentum = Math.Abs(Mechanics.BoardMomentum) > 0.25 ? -Mechanics.BoardMomentum * 0.75 : 0;
            }
            // if Acceleration is active
            if (Mechanics.BoardCanAccelerate)
            {
                // Force brake if momentum pulls the other way
                if (Mechanics.BoardIsGoingInOppositeDirection)
                {
                    Mechanics.BoardIsBraking = true;
                    Mechanics.BoardReduceSpeed();
                    Mechanics.BoardIsBraking = false;
                }
                // Accelerate if one direction is selected
                else if (Mechanics.BoardHasOneDirection && DirectionIsFree)
                {
                    Mechanics.BoardMomentum += (Mechanics.BoardGoLeft ? -1 : 1) * Mechanics.BoardAccelerationDeterminant * Info.DeltaTime;
                }
            }
            else
            {
                Mechanics.BoardReduceSpeed();
            }
        }

        #region Property Changed

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
