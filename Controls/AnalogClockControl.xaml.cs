using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ShopOffice.Controls
{
    /// <summary>
    /// Interaction logic for AnalogClockControl.xaml
    /// </summary>
    public partial class AnalogClockControl : UserControl, INotifyPropertyChanged
    {
        #region - Construtor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public AnalogClockControl()
        {
            // initialize clock
            this.InternalInitializeClock();

            // initialize control
            this.InitializeComponent();

            // set context
            this.DataContext = this;

            // invalidate UI
            this.InternalInvalidate();

            // initialize timer
            this.InternalInitializeTimer();
        }
        #endregion

        #region - Public Class -
        /// <summary>
        /// Clock part
        /// </summary>
        public class ClockPart
        {
            /// <summary>
            /// Number in string format
            /// </summary>
            public string Number { get; set; } = default!;
            /// <summary>
            /// Angle for this number
            /// </summary>
            public decimal Angle { get; set; }
        }
        #endregion
 
        #region - Events -
        /// <summary>
        /// Envent for property change
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region - Properties -
        /// <summary>
        /// Hour parts
        /// </summary>
        public List<ClockPart>? HourParts { get; set; } = default;
        /// <summary>
        /// Second parts
        /// </summary>
        public List<ClockPart>? SecondParts { get; set; } = default;
        /// <summary>
        /// Angle for hour hand
        /// </summary>
        public double AngleHour
        {
            get { return this.mAngleHour; }
            set
            {
                this.mAngleHour = value;
                this.InternalNotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Angle for minute hand
        /// </summary>
        public double AngleMin
        {
            get { return this.angleMin; }
            set
            {
                this.angleMin = value;
                this.InternalNotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Angle for second hand
        /// </summary>
        public double AngleSec
        {
            get { return this.angleSec; }
            set
            {
                this.angleSec = value;
                this.InternalNotifyPropertyChanged();
            }
        }
        #endregion

        #region - Variables -
        /// <summary>
        /// Timer for invalidating data for user
        /// </summary>
        private DispatcherTimer mTimer { get; set; } = new DispatcherTimer();
        /// <summary>
        /// Angle for hour hand
        /// </summary>
        private double mAngleHour = default;
        /// <summary>
        /// Angle for minute hand
        /// </summary>
        private double angleMin = default;
        /// <summary>
        /// Angle for second hand
        /// </summary>
        private double angleSec = default;
        #endregion

        #region - Private Methods -
        /// <summary>
        /// This method initializes timer
        /// </summary>
        private void InternalInitializeTimer()
        {
            this.mTimer.Interval = TimeSpan.FromSeconds(1);
            this.mTimer.Tick += InternalTimer_Tick;
            this.mTimer.Start();
        }
        /// <summary>
        /// This method initializes clock and its parts
        /// </summary>
        private void InternalInitializeClock()
        {
            this.HourParts = new List<ClockPart>();
            for (int x = 0; x < 12; x++)
            {
                this.HourParts.Add(new ClockPart()
                {
                    Number = (x + 1).ToString(),
                    Angle = (x + 1) * (360M / 12M),
                });
            }

            this.SecondParts = new List<ClockPart>();
            for (int x = 0; x < 60; x++)
            {
                this.SecondParts.Add(new ClockPart()
                {
                    Number = (x + 1).ToString(),
                    Angle = (x + 1) * (360M / 60M),
                });
            }
        }
        /// <summary>
        /// This method executes change in clock
        /// </summary>
        /// <param name="sender">Sender data</param>
        /// <param name="e">Arguments</param>
        private void InternalTimer_Tick(object? sender, EventArgs e)
        {
            this.InternalInvalidate();
        }
        /// <summary>
        /// This method invalidates data for clock
        /// </summary>
        private void InternalInvalidate()
        {
            DateTime time = DateTime.Now;
            AngleHour = (double)(time.Hour * (360M / 12M) + (((360M / 12M) / 60M) * time.Minute));
            AngleMin = (double)(time.Minute * (360M / 60M));
            AngleSec = (double)(time.Second * (360M / 60M));
        }
        /// <summary>
        /// This method is called by the Set accessor of each property.  
        /// </summary>
        /// <param name="propertyName">Property name</param>
        private void InternalNotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
