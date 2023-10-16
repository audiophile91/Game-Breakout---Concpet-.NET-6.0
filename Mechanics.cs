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
    public class Mechanics : INotifyPropertyChanged
    {
        #region Referentions

        private Info Info { get; }
        private Settings Settings { get; }
        private ObservableCollection<Ball> Balls { get; }
        private ObservableCollection<Block> Blocks { get; }
        private ObservableCollection<Board> Boards { get; }

        #endregion

        public Mechanics(Info info, Settings settings, ObservableCollection<Ball> balls, ObservableCollection<Block> blocks, ObservableCollection<Board> boards)
        {
            Info = info;
            Settings = settings;
            Balls = balls;
            Blocks = blocks;
            Boards = boards;
        }
        public void RunGame()
        {
            foreach (Ball ball in Balls)
            {
                ball.Move();
            }
            foreach (Board board in Boards)
            {
                board.Move();
            }
        }

        #region Fields

        private bool _boardGoLeft;
        private bool _boardGoRight;
        private bool _boardIsBraking;

        private double _ballSpeed = 0.30;
        private double _ballSpreadAngleRange = 45;
        private double _boardAccelerationDeterminant = 1;
        private double _boardMomentum;

        #endregion

        #region Properties

        public bool BoardGoLeft
        {
            get
            {
                return _boardGoLeft;
            }
            set
            {
                _boardGoLeft = value;
                OnPropertyChanged(nameof(BoardGoLeft));
            }
        }
        public bool BoardGoRight
        {
            get
            {
                return _boardGoRight;
            }
            set
            {
                _boardGoRight = value;
                OnPropertyChanged(nameof(BoardGoRight));
            }
        }
        public bool BoardIsBraking
        {
            get
            {
                return _boardIsBraking;
            }
            set
            {
                _boardIsBraking = value;
                OnPropertyChanged(nameof(BoardIsBraking));
            }
        }

        public double BallSpeed
        {
            get
            {
                return _ballSpeed;
            }
            set
            {
                _ballSpeed = value;
                OnPropertyChanged(nameof(BallSpeed));
            }
        }
        public double BallSpeadAngleRange
        {
            get
            {
                return _ballSpreadAngleRange;
            }
            set
            {
                _ballSpreadAngleRange = value;
                OnPropertyChanged(nameof(BallSpeadAngleRange));
            }
        }
        public double BoardAccelerationDeterminant
        {
            get
            {
                return _boardAccelerationDeterminant;
            }
            set
            {
                _boardAccelerationDeterminant = value;
                OnPropertyChanged(nameof(BoardAccelerationDeterminant));
            }
        }
        public double BoardMomentum
        {
            get
            {
                return _boardMomentum;
            }
            set
            {
                value = value > 1 ? 1 : value;
                value = value < -1 ? -1 : value;

                _boardMomentum = value;
                OnPropertyChanged(nameof(BoardMomentum));
            }
        }

        #endregion

        #region Getters

        public bool BoardAccelerate => BoardGoLeft || BoardGoRight;
        public bool BoardCanAccelerate => BoardAccelerate && !BoardIsBraking;
        public bool BoardGoingLeft => BoardMomentum < 0;
        public bool BoardGoingRight => BoardMomentum > 0;
        public bool BoardIsGoingInOppositeDirection => BoardGoingLeft && BoardGoRight || BoardGoingRight && BoardGoLeft;
        public bool BoardHasMomentum => BoardMomentum != 0;
        public bool BoardHasOneDirection => BoardGoLeft != BoardGoRight;
        public double BoardSpeedReduction => BoardIsBraking ? BoardSpeedReductionBrake : BoardSpeedReductionFriction;
        public double BoardSpeedReductionBrake => BoardSpeedReductionFriction * 2;
        public double BoardSpeedReductionFriction => BoardAccelerationDeterminant * 2 * Info.DeltaTime;

        #endregion

        #region Stage

        public void StageSetLayout(uint columns, uint rows)
        {
            Info.StageBlockColumns = columns;
            Info.StageBlockRows = rows;
        }
        public void StageGenerateBlock(uint column, uint row)
        {
            Blocks.Add(new Block(Info, Settings, new Point(column * Info.ProportionBlockWidth, row * Info.ProportionBlockHeight)));
        }

        #endregion

        public void BoardReduceSpeed()
        {
            if (BoardHasMomentum)
            {
                var newMomentum = BoardMomentum + BoardSpeedReduction * (BoardGoingLeft ? 1 : -1);

                BoardMomentum = (BoardGoingLeft && newMomentum > 0) || (BoardGoingRight && newMomentum < 0) ? 0 : newMomentum;
            }       
        }
        public void BallSpread()
        {
            foreach (Ball ball in Balls.ToList())
            {
                for (int i = 0; i < 2; i++)
                {
                    Balls.Add(new Ball(Info, this, Settings, Blocks, Boards, ball.Data.Position, ball.Direction));
                }
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
