using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Breakout
{
    /// <summary>
    /// Interaction logic for Ball.xaml
    /// </summary>
    public partial class Ball : UserControl, INotifyPropertyChanged
    {
        #region Referentions

        private Info Info { get; }
        private Mechanics Mechanics { get; }
        private Settings Settings { get; }
        private ObservableCollection<Block> Blocks { get; }
        private ObservableCollection<Board> Boards { get; }

        #endregion

        private SpatialDatasetRectangle _data;
        private Point _positionTarget;
        private Vector2 _direction;

        public Vector2 PostionDelta { get; set; }

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
        public Point PositionTarget
        {
            get
            {
                return _positionTarget;
            }
            set
            {
                _positionTarget = value;
                OnPropertyChanged(nameof(PositionTarget));
            }
        }
        public Vector2 Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                Mechanics.BallSpeed *= 1.0005;
                OnPropertyChanged(nameof(Direction));
                OnPropertyChanged(nameof(VectorLength));
            }
        }

        public double DeltaX { get; set; }
        public double DeltaY { get; set; }

        public bool BlockCollisionImpossible => Data.EdgeTop > Info.BlocksSpaceEdge && PositionTarget.Y > Info.BlocksSpaceEdge;

        public double VectorLength => Direction.Length();

        //public bool Intersects(Block block)
        //{
        //    bool intersectsX = Data.Position.X + Data.Radius > block.X && X - Radius < block.X + block.Width;
        //    bool intersectsY = Data.Position.Y + Data.Radius > block.Y && Y - Radius < block.Y + block.Height;

        //    // Jeśli obie warunki są spełnione, to nastąpiło zderzenie
        //    return intersectsX && intersectsY;
        //}



        public Ball(Info info, Mechanics mechanics, Settings settings, ObservableCollection<Block> blocks, ObservableCollection<Board> boards, Point position, Vector2 initialVector)
        {
            InitializeComponent();
            DataContext = this;
            Info = info;
            Mechanics = mechanics;
            Settings = settings;
            Blocks = blocks;
            Boards = boards;
            Data = new(Info, Settings, this, position);
            Direction = XMath.GetRandomOffsetVector2(initialVector, 45);
            Settings.GameSpace.Children.Add(this);
        }
        public void Move()
        {
            PositionTarget = GetPositionTarget(Info.DeltaTime, Data.Position);

            ValidateCollisionBoards();
            ValidateCollisionBlock();

            Data.Position = PositionTarget;

            ValidateCollisionCanvas();
        }
        public void BounceFromBoard(Board board)
        {
            var hitPoint = XMath.GetValueRangePercent(Data.PointCenterEdgeBottom.X, board.Data.EdgeLeft, board.Data.EdgeRight);
            var angle = 190 + hitPoint * 1.6;

            Direction = XMath.SetVectorAngle(Direction, angle);
        }
        public void ValidateCollisionCanvas()
        {
            // Doesnt need extra logic checks other than this because SpatialDataset handles all within space boundaries

            if (Data.IsOnEdgeAxisX)
            {
                Direction = new Vector2(-Direction.X, Direction.Y);
            }
            if (Data.IsOnEdgeAxisY)
            {
                Direction = new Vector2(Direction.X, -Direction.Y);
            }
        }
        public void ValidateCollisionBoards()
        {
            foreach (Board board in Boards)
            {
                // If target position of ball bottom edge is lesser than board top edge, collision cannot occur
                if (PositionTarget.Y + Info.ProportionBallHeight < board.Data.EdgeTop)
                {
                    return;
                }
                // If balls bottom edge is already beneath boards top edge, ball already missed
                else if (Data.EdgeBottom > board.Data.EdgeTop)
                {
                    return;
                }
                // Calculate if ball hits the board with current movement vector
                else
                {
                    var bottomCenterTargetPosition = GetPositionTarget(Info.DeltaTime, Data.PointCenterEdgeBottom);

                    var crossPoint = XMath.GetCrossPoint(Data.PointCenterEdgeBottom, bottomCenterTargetPosition, board.Data.EdgeTop, true);

                    if (XMath.InBoundaries(crossPoint.X, board.Data.EdgeLeft, board.Data.EdgeRight))
                    {
                        PositionTarget = new Point(crossPoint.X - Info.ProportionBallWidth / 2, crossPoint.Y - Info.ProportionBallHeight);
                        BounceFromBoard(board);
                    }
                }
            }
        }
        public void ValidateCollisionBlock()
        {
            if (BlockCollisionImpossible)
            {
                return;
            }

            foreach (Block block in Blocks)
            {
                if (Direction.Y <= 0)
                {
                    if (XMath.DoesSegmentIntersect(Data.EdgeTop, DeltaY, block.Data.EdgeBottom))
                    {
                        var pointTested = Direction.X <= 0 ? Data.PointCornerLeftTop : Data.PointCornerRightTop;
                        var crossPoint = XMath.GetCrossPoint(pointTested, new Point(pointTested.X + DeltaX, pointTested.Y + DeltaY), block.Data.EdgeBottom, true);

                        if (XMath.InBoundaries(crossPoint.X, block.Data.EdgeLeft, block.Data.EdgeRight))
                        {
                            PositionTarget = new Point(crossPoint.X - (Direction.X <= 0 ? 0 : Info.ProportionBallWidth), crossPoint.Y);
                            Blocks.Remove(block);
                            Settings.GameSpace.Children.Remove(block);
                            Direction = new Vector2(Direction.X, -Direction.Y);
                            Info.Score += 100;
                            break;
                        }
                    }
                }
                if (Direction.Y > 0)
                {
                    if (XMath.DoesSegmentIntersect(Data.EdgeBottom, DeltaY, block.Data.EdgeTop))
                    {
                        var pointTested = Direction.X <= 0 ? Data.PointCornerLeftBottom : Data.PointCornerRightBottom;
                        var crossPoint = XMath.GetCrossPoint(pointTested, new Point(pointTested.X + DeltaX, pointTested.Y + DeltaY), block.Data.EdgeTop, true);

                        if (XMath.InBoundaries(crossPoint.X, block.Data.EdgeLeft, block.Data.EdgeRight))
                        {
                            PositionTarget = new Point(crossPoint.X - (Direction.X <= 0 ? 0 : Info.ProportionBallWidth), crossPoint.Y - Info.ProportionBallHeight);
                            Blocks.Remove(block);
                            Settings.GameSpace.Children.Remove(block);
                            Direction = new Vector2(Direction.X, -Direction.Y);
                            Info.Score += 100;
                            break;
                        }

                    }
                }
                if (Direction.X <= 0)
                {
                    if (XMath.DoesSegmentIntersect(Data.EdgeLeft, DeltaX, block.Data.EdgeRight))
                    {
                        var pointTested = Direction.Y <= 0 ?  Data.PointCornerLeftTop : Data.PointCornerLeftBottom;
                        var crossPoint = XMath.GetCrossPoint(pointTested, new Point(pointTested.X + DeltaX, pointTested.Y + DeltaY), block.Data.EdgeRight, false);

                        if (XMath.InBoundaries(crossPoint.Y, block.Data.EdgeTop, block.Data.EdgeBottom))
                        {
                            PositionTarget = new Point(crossPoint.X, crossPoint.Y - (Direction.Y <= 0 ? 0 : Info.ProportionBallHeight));
                            Blocks.Remove(block);
                            Settings.GameSpace.Children.Remove(block);
                            Direction = new Vector2(-Direction.X, Direction.Y);
                            Info.Score += 100;
                            break;
                        }
                    }
                }
                if (Direction.X > 0)
                {
                    if (XMath.DoesSegmentIntersect(Data.EdgeRight, DeltaX, block.Data.EdgeLeft))
                    {
                        var pointTested = Direction.Y <= 0 ? Data.PointCornerRightTop : Data.PointCornerRightBottom;
                        var crossPoint = XMath.GetCrossPoint(pointTested, new Point(pointTested.X + DeltaX, pointTested.Y + DeltaY), block.Data.EdgeLeft, false);

                        if (XMath.InBoundaries(crossPoint.Y, block.Data.EdgeTop, block.Data.EdgeBottom))
                        {
                            PositionTarget = new Point(crossPoint.X - Info.ProportionBallWidth, crossPoint.Y - (Direction.Y <= 0 ? 0 : Info.ProportionBallHeight));
                            Blocks.Remove(block);
                            Settings.GameSpace.Children.Remove(block);
                            Direction = new Vector2(-Direction.X, Direction.Y);
                            Info.Score += 100;
                            break;
                        }
                    }
                }
            }
        }

        private Point GetPositionTarget(double deltaTime, Point point)
        {
            DeltaX = Direction.X * Mechanics.BallSpeed * deltaTime;
            DeltaY = Direction.Y * Mechanics.BallSpeed * deltaTime;

            PostionDelta = new Vector2((float)DeltaX, (float)DeltaY);

            return new Point(point.X + DeltaX, point.Y + DeltaY);
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
