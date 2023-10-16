using System.ComponentModel;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;

namespace Breakout
{
    /// <summary>
    /// Stores and returns important data about UIElement on Canvas. 
    /// <para>Handles element layout position on Canvas when updating Position Property.</para>
    /// <para>Watch over boundaries of element on Canvas.</para>
    /// </summary>
    public class SpatialDatasetRectangle : INotifyPropertyChanged
    {
        #region References

        private Info Info { get; }
        private Settings Settings { get; }

        /// <summary>
        /// Reference to object storing this dataset
        /// </summary>
        private UIElement Self { get; }

        #endregion

        public SpatialDatasetRectangle(Info info, Settings settings, UIElement self, Point position)
        {
            Info = info;
            Settings = settings;
            Self = self;
            Position = position;
            UpdateLayout();
        }

        #region Properties

        private double _height;
        private double _proportionHeight;
        private double _proportionWidth;
        private double _scale = 1;
        private double _width;
        private Point _position;

        private double _radius;
        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                OnPropertyChanged(nameof(Radius));
            }
        }

        /// <summary>
        /// Elements actual height in pixels
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        /// <summary>
        /// Represents elements heights scaled size in comparition to parent.
        /// </summary>     
        public double ProportionHeight
        {
            get
            {
                return _proportionHeight;
            }
            set
            {
                _proportionHeight = value;
                OnPropertyChanged(nameof(ProportionHeight));

                Height = Settings.GameHeight * ProportionHeight * Scale;
            }
        }

        /// <summary>
        /// Represents elements widths scaled size in comparition to parent.
        /// </summary>
        public double ProportionWidth
        {
            get
            {
                return _proportionWidth;
            }
            set
            {
                _proportionWidth = value;
                OnPropertyChanged(nameof(ProportionWidth));

                Width = Settings.GameWidth * ProportionWidth * Scale;
            }
        }

        /// <summary>
        /// Default value is 1. Allows to alter element size.
        /// </summary>
        public double Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                OnPropertyChanged(nameof(Scale));
                UpdateLayout();
            }
        }

        /// <summary>
        /// Elements actual width in pixels
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        /// <summary>
        /// Represents scaled from 0 to 1 dimensions X and Y offset from top-left edge of parent.
        /// </summary>
        public Point Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = ValidateBoundaries(value);
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PointCornerRightTop));
                OnPropertyChanged(nameof(PointCornerLeftTop));
                OnPropertyChanged(nameof(PointCenterEdgeBottom));
                OnPropertyChanged(nameof(PointCenterEdgeTop));
                OnPropertyChanged(nameof(PointCenterEdgeLeft));
                OnPropertyChanged(nameof(PointCenterEdgeRight));
                UpdateCanvasPosition();
            }
        }

        #endregion

        #region Getters

        public bool IsOnEdgeAxisX => IsOnEdgeLeft || IsOnEdgeRight;
        public bool IsOnEdgeAxisY => IsOnEdgeTop || IsOnEdgeBottom;
        public bool IsOnEdgeBottom => Position.Y == PositionMaxY;
        public bool IsOnEdgeLeft => Position.X == PositionMinX;
        public bool IsOnEdgeRight => Position.X == PositionMaxX;
        public bool IsOnEdgeTop => Position.Y == PositionMinY;

        public double EdgeBottom => Position.Y + ProportionHeight;
        public double EdgeTop => Position.Y;
        public double EdgeLeft => Position.X;
        public double EdgeRight => Position.X + ProportionWidth;

        /// <summary>
        /// Returns elements 0 -> 1 X axis maximal position
        /// </summary>
        public double PositionMaxX => 1 - ProportionWidth;

        /// <summary>
        /// Returns elements 0 -> 1 X axis minimal position
        /// </summary>
        public double PositionMinX => 0;

        /// <summary>
        /// Returns elements 0 -> 1 Y axis maximal position
        /// </summary>
        public double PositionMaxY => 1 - ProportionHeight;

        /// <summary>
        /// Returns elements 0 -> 1 Y axis minimal position
        /// </summary>
        public double PositionMinY => 0;

        public Point PointCenterEdgeBottom => new Point(EdgeLeft + ProportionWidth / 2, EdgeBottom);
        public Point PointCenterEdgeTop => new Point(EdgeLeft + ProportionWidth / 2, EdgeTop);
        public Point PointCenterEdgeLeft => new Point(EdgeLeft, EdgeTop +  ProportionHeight / 2);
        public Point PointCenterEdgeRight => new Point(EdgeRight, EdgeTop +  ProportionHeight / 2);

        public Point PointCornerLeftTop => new Point(EdgeLeft, EdgeTop);
        public Point PointCornerLeftBottom => new Point(EdgeLeft, EdgeBottom);
        public Point PointCornerRightTop => new Point(EdgeRight, EdgeTop);
        public Point PointCornerRightBottom => new Point(EdgeRight, EdgeBottom);
       

        #endregion

        #region Functions

        /// <summary>
        /// Returns elements proportion from settings class based on elements type.
        /// </summary>
        /// <returns></returns>
        private double GetProportionHeight()
        {
            if (Self is Ball)
            {
                return Info.ProportionBallHeight;
            }
            if (Self is Block)
            {
                return Info.ProportionBlockHeight;         
            }
            if (Self is Board)
            {
                return Info.ProportionBoardHeight;
            }
            return 0;
        }

        /// <summary>
        /// Returns elements proportion from settings class based on elements type.
        /// </summary>
        /// <returns></returns>
        private double GetProportionWidth()
        {
            if (Self is Ball)
            {
                return Info.ProportionBallWidth;
            }
            if (Self is Block)
            {
                return Info.ProportionBlockWidth;
            }
            if (Self is Board)
            {
                return Info.ProportionBoardWidth;
            }
            return 0;
        }

        /// <summary>
        /// Public function which triggers reset of elements layout setup.
        /// </summary>
        public void UpdateLayout()
        {
            ProportionHeight = GetProportionHeight();
            ProportionWidth = GetProportionWidth();
            UpdateCanvasPosition();
        }

        /// <summary>
        /// Sets the element position on parent Canvas
        /// </summary>
        private void UpdateCanvasPosition()
        {
            Canvas.SetLeft(Self, EdgeLeft * Settings.GameWidth);
            Canvas.SetTop(Self, EdgeTop * Settings.GameHeight);
        }

        /// <summary>
        /// Checks if position is within canvas boundaries, if not, corrects it to edge. It is used later to trigger bounce.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>     
        private Point ValidateBoundaries(Point point)
        {
            if (point.X < PositionMinX)
            {
                point.X = PositionMinX;
            }
            if (point.X > PositionMaxX)
            {
                point.X = PositionMaxX;
            }
            if (point.Y < PositionMinY)
            {
                point.Y = PositionMinY;
            }
            if (point.Y > PositionMaxY)
            {
                point.Y = PositionMaxY;
            }
            return point;
        } 
        
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
