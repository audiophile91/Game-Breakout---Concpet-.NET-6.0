using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Breakout
{
    public class Info : INotifyPropertyChanged
    {
        #region Referentions

        private Settings Settings { get; }

        #endregion

        public Info(Settings settings)
        {
            Settings = settings;
        }

        #region Fields

        private bool _displayDesigner = true;
        private bool _isGameOver;
        private bool _isGamePaused;
        private double _deltaTime;
        private uint _fps;
        private uint _framesCounter;
        private uint _score;
        private uint _stage;
        private uint _stageBlockColumns;
        private uint _stageBlockRows;

        public DateTime LastFrameRendered { get; set; }
        public DateTime LastMenuClick { get; set; }

        #endregion

        #region Properties

        public bool DisplayDesigner
        {
            get
            {
                return _displayDesigner;
            }
            set
            {
                _displayDesigner = value;
                OnPropertyChanged(nameof(DisplayDesigner));
            }
        }
        public bool IsGameOver
        {
            get
            {
                return _isGameOver;
            }
            set
            {
                _isGameOver = value;
                OnPropertyChanged(nameof(IsGameOver));
            }
        }
        public bool IsGamePaused
        {
            get
            {
                return _isGamePaused;
            }
            set
            {
                _isGamePaused = value;
                OnPropertyChanged(nameof(IsGamePaused));
            }
        }
        public double DeltaTime
        {
            get
            {
                return _deltaTime;
            }
            set
            {
                _deltaTime = value;
                OnPropertyChanged(nameof(DeltaTime));
            }
        }
        public uint FPS
        {
            get
            {
                return _fps;
            }
            set
            {
                _fps = value;
                OnPropertyChanged(nameof(FPS));
            }
        }
        public uint FramesCounter
        {
            get
            {
                return _framesCounter;
            }
            set
            {
                _framesCounter = value;
                OnPropertyChanged(nameof(FramesCounter));
            }
        }
        public uint Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
                OnPropertyChanged(nameof(Score));
            }
        }
        public uint Stage
        {
            get
            {
                return _stage;
            }
            set
            {
                _stage = value;
                OnPropertyChanged(nameof(Stage));
            }
        }
        public uint StageBlockColumns
        {
            get
            {
                return _stageBlockColumns;
            }
            set
            {
                _stageBlockColumns = value;
                OnPropertyChanged(nameof(StageBlockColumns));
            }
        }
        public uint StageBlockRows
        {
            get
            {
                return _stageBlockRows;
            }
            set
            {
                _stageBlockRows = value;
                OnPropertyChanged(nameof(StageBlockRows));
            }
        }

        #endregion

        #region Getters

        public double BlocksSpaceEdge => ProportionBlockHeight * StageBlockRows; 
        public double ProportionBallHeight => ProportionBoardHeight;
        public double ProportionBallWidth => ProportionBallHeight * Settings.GameHeight / Settings.GameWidth;
        public double ProportionBlockHeight => 1.0 / StageBlockRows / 3;
        public double ProportionBlockWidth => 1.0 / StageBlockColumns;
        public double ProportionBoardHeight => 0.02;
        public double ProportionBoardWidth => 0.1;

        public Point InitialPointBall => new Point(0.5 - ProportionBallWidth / 2, 0.5 - ProportionBallHeight / 2);
        public Point InitialPointBoard => new Point(0.5 - ProportionBoardWidth / 2, 1 - ProportionBoardHeight * 2);

        public Vector2 InitialVectorBall => new Vector2(0, 1);

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
