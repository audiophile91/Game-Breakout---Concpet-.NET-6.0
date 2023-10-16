using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Breakout
{
    /// <summary>
    /// Interaction logic for BreakOut
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            GameDataClock = new();
        }

        #region Game Data

        private DispatcherTimer GameDataClock { get; }

        private Designer _designer;
        private Info _info;
        private Mechanics _mechanics;
        private Settings _settings;

        public Designer Designer
        {
            get
            {
                return _designer;
            }
            set
            {
                _designer = value;
                OnPropertyChanged(nameof(Designer));
            }
        }
        public Info Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
                OnPropertyChanged(nameof(Info));
            }
        }
        public Mechanics Mechanics
        {
            get
            {
                return _mechanics;
            }
            set
            {
                _mechanics = value;
                OnPropertyChanged(nameof(Mechanics));
            }
        }
        public Settings Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                OnPropertyChanged(nameof(Settings));
            }
        }

        private void UpdateData(object? sender, EventArgs e)
        {
            UpdateFPS();
        }
        private void UpdateFPS()
        {
            Info.FPS = Info.FramesCounter;
            Info.FramesCounter = 0;
        }

        #endregion

        #region Objects

        private readonly ObservableCollection<Ball> _balls = new();
        private readonly ObservableCollection<Block> _blocks = new();
        private readonly ObservableCollection<Board> _boards = new();

        public ObservableCollection<Ball> Balls => _balls;
        public ObservableCollection<Block> Blocks => _blocks;
        public ObservableCollection<Board> Boards => _boards;

        #endregion

        #region Events Mechanics

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                Mechanics.BoardGoLeft = true;
            }
            if (e.Key == Key.Right)
            {
                Mechanics.BoardGoRight = true;
            }
            if (e.Key == Key.LeftShift)
            {
                Mechanics.BoardIsBraking = true;
            }
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.D)
            {
                Info.DisplayDesigner ^= true;
            }
            if (e.Key == Key.Left)
            {
                Mechanics.BoardGoLeft = false;
            }
            if (e.Key == Key.Right)
            {
                Mechanics.BoardGoRight = false;
            }
            if (e.Key == Key.LeftShift)
            {
                Mechanics.BoardIsBraking = false;
            }
            if (e.Key == Key.A)
            {
                Mechanics.BallSpeed *= 2;
            }
            if (e.Key == Key.S)
            {
                Mechanics.BallSpeed /= 2;
            }
            if (e.Key == Key.Space)
            {
                Info.IsGamePaused ^= true;
            }
            if (e.Key == Key.R)
            {
                InitializeGameNew();
            }
            if (e.Key == Key.B)
            {
                Mechanics.BallSpread();
            }
            if (e.Key == Key.M)
            {
                foreach (Ball ball in Balls)
                {
                    ball.Data.Scale *= 2;
                }
            }
            if (e.Key == Key.Enter)
            {
                StageLoadNext();
            }
        }

        #endregion

        #region Game Clock

        private void GameTick_Tick(object? sender, EventArgs e)
        {
            Info.DeltaTime = (DateTime.Now - Info.LastFrameRendered).TotalSeconds;
            Info.LastFrameRendered = DateTime.Now;
            Info.FramesCounter++;

            if (Info.IsGamePaused)
            {
                return;
            }

            Mechanics.RunGame();
            Designer.UpdateDesigner();
        }

        #endregion

        #region Stages

        private void StageLoadNext()
        {
            InitializeNextStage();

            switch (++Info.Stage)
            {
                case 1: StageLoad_1(); break;
            }
        }

        private void StageLoad_1()
        {
            Mechanics.StageSetLayout(30, 15);

            for (uint i = 0; i < Info.StageBlockRows; i++)
            {
                for (uint j = 0; j < Info.StageBlockColumns; j++)
                {
                    Mechanics.StageGenerateBlock(j, i);
                }
            }
        }



        #endregion

        #region Events Application

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGameNew();
            SizeChanged += ValidateLayout;
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
        private void ShutDown_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Menu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - Info.LastMenuClick;

            if (elapsed.TotalMilliseconds < 300)
            {
                Maximize_Click(sender, e);
            }
            Info.LastMenuClick = DateTime.Now;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion

        #region Layout Control

        private void ValidateLayout(object sender, SizeChangedEventArgs e)
        {
            foreach (Ball ball in Balls)
            {
                ball.Data.UpdateLayout();
            }
            foreach (Block block in Blocks)
            {
                block.Data.UpdateLayout();
            }
            foreach (Board board in Boards)
            {
                board.Data.UpdateLayout();
            }
        }

        #endregion

        #region Initialization

        private void InitializeGameNew()
        {
            InitializeGameData();
            InitializeGameRendering();
            StageLoadNext();
        }
        private void InitializeGameData()
        {
            Settings = new(GameSpace);
            Info = new(Settings);
        }
        private void InitializeGameRendering()
        {
            CompositionTarget.Rendering += GameTick_Tick;
            GameDataClock.Interval = TimeSpan.FromSeconds(1);
            GameDataClock.Tick += UpdateData;
            GameDataClock.Start();
        }
        private void InitializeNextStage()
        {
            InitializeGameSpace();

            Mechanics = new(Info, Settings, Balls, Blocks, Boards);
            Designer = new(Info, Mechanics, Settings, Balls, Blocks, Boards);

            Boards.Add(new Board(Info, Mechanics, Settings));
            Balls.Add(new Ball(Info, Mechanics, Settings, Blocks, Boards, Info.InitialPointBall, Info.InitialVectorBall));
        }
        private void InitializeGameSpace()
        {
            GameSpace.Children.Clear();
            Balls.Clear();
            Blocks.Clear();
            Boards.Clear();
            Info.IsGamePaused = true;
        }

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }

        #endregion
    }
}