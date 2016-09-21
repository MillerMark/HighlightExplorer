using System;
using System.Collections.Generic;
using System.Linq;
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

namespace HighlightExplorer
{
	/// <summary>
	/// Interaction logic for ForegroundBackgroundPicker.xaml
	/// </summary>
	public partial class ForegroundBackgroundPicker : UserControl
	{
		public static readonly DependencyProperty InnerPaddingProperty = DependencyProperty.Register("InnerPadding", typeof(int), typeof(ForegroundBackgroundPicker), new PropertyMetadata(0, new PropertyChangedCallback(OnInnerPaddingChanged)));

		private static void OnInnerPaddingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ForegroundBackgroundPicker highlightTextPicker = o as ForegroundBackgroundPicker;
			if (highlightTextPicker != null)
				highlightTextPicker.OnInnerPaddingChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnInnerPaddingChanged(int oldValue, int newValue)
		{
			SliderHelper.PadAbove(ctlBackground, InnerPadding);
			SliderHelper.PadAbove(ctlForeground, InnerPadding);
		}
		public int InnerPadding
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (int)GetValue(InnerPaddingProperty);
			}
			set
			{
				SetValue(InnerPaddingProperty, value);
			}
		}

		public static readonly DependencyProperty DraggableControlsProperty = DependencyProperty.Register("DraggableControls", typeof(bool), typeof(ForegroundBackgroundPicker), new PropertyMetadata(true, new PropertyChangedCallback(OnDraggableControlsChanged)));

		private static void OnDraggableControlsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ForegroundBackgroundPicker foregroundBackgroundPicker = o as ForegroundBackgroundPicker;
			if (foregroundBackgroundPicker != null)
				foregroundBackgroundPicker.OnDraggableControlsChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		protected virtual void OnDraggableControlsChanged(bool oldValue, bool newValue)
		{
			if (DraggableControls)
			{
				rctBackground.Visibility = Visibility.Hidden;
				rctForeground.Visibility = Visibility.Hidden;
				ArrowGray.Visibility = Visibility.Hidden;
				SliderHelper.EnableDrag(ctlForeground);
                SliderHelper.EnableDrag(ctlBackground);
                ctlForeground.MouseUp += SliderMouseUp;
                ctlBackground.MouseUp += SliderMouseUp;
                ctlForeground.MouseDown += CtlForeground_MouseDown;
                ctlBackground.MouseDown += CtlBackground_MouseDown;
                SliderHelper.PositionChanged += SliderHelper_PositionChanged;
			}
			else
			{
				ArrowGray.Visibility = Visibility.Visible;
				rctBackground.Visibility = Visibility.Visible;
				rctForeground.Visibility = Visibility.Visible;
				SliderHelper.DisableDrag(ctlForeground);
				ctlForeground.MouseUp -= SliderMouseUp;
                ctlForeground.MouseDown -= CtlForeground_MouseDown;
                SliderHelper.DisableDrag(ctlBackground);
				ctlBackground.MouseUp -= SliderMouseUp;
                ctlBackground.MouseDown -= CtlForeground_MouseDown;
                SliderHelper.PositionChanged -= SliderHelper_PositionChanged;
			}
		}

        private void CtlBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnBeforeBackgroundChanged(ctlBackground, (double)ctlBackground.Tag);
        }

        private void CtlForeground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnBeforeForegroundChanged(ctlForeground, (double)ctlForeground.Tag);
        }

        void PositionBackgroundForegroundArrow()
		{
			SliderHelper.PositionArrow(ArrowGray, BackgroundValue, ForegroundValue, ArrowShaftGray, tbGrayPercentDistance);
		}

		public bool DraggableControls
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (bool)GetValue(DraggableControlsProperty);
			}
			set
			{
				SetValue(DraggableControlsProperty, value);
			}
		}

		public event EventHandler<double> BeforeForegroundChanged;
        public event EventHandler<double> ForegroundChanged;
        public event EventHandler<double> AfterForegroundChanged;
        public event EventHandler<double> BeforeBackgroundChanged;
        public event EventHandler<double> BackgroundChanged;
        public event EventHandler<double> AfterBackgroundChanged;
        protected virtual void OnForegroundChanged(object sender, double e)
		{
			ForegroundChanged?.Invoke(sender, e);
		}
		protected virtual void OnBackgroundChanged(object sender, double e)
		{
			BackgroundChanged?.Invoke(sender, e);
		}

        protected virtual void OnBeforeForegroundChanged(object sender, double e)
        {
            BeforeForegroundChanged?.Invoke(sender, e);
        }
        protected virtual void OnBeforeBackgroundChanged(object sender, double e)
        {
            BeforeBackgroundChanged?.Invoke(sender, e);
        }

        protected virtual void OnAfterForegroundChanged(object sender, double e)
        {
            AfterForegroundChanged?.Invoke(sender, e);
        }
        protected virtual void OnAfterBackgroundChanged(object sender, double e)
        {
            AfterBackgroundChanged?.Invoke(sender, e);
        }

        #region Dependency Properties...
        public static readonly DependencyProperty BackgroundValueProperty = DependencyProperty.Register("BackgroundValue", typeof(double), typeof(ForegroundBackgroundPicker), new PropertyMetadata(0.0, new PropertyChangedCallback(OnBackgroundValueChanged)));
		
		private static void OnBackgroundValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ForegroundBackgroundPicker foregroundBackgroundPicker = o as ForegroundBackgroundPicker;
			if (foregroundBackgroundPicker != null)
				foregroundBackgroundPicker.OnBackgroundValueChanged((double)e.OldValue, (double)e.NewValue);
		}

		protected virtual void OnBackgroundValueChanged(double oldValue, double newValue)
		{
			if (DraggableControls)
				SliderHelper.SetPosition(ctlBackground, newValue);
			else
				SliderHelper.SetPosition(ctlBackground, rctBackground, newValue);
			SliderHelper.MoveUpIfSpaceAvailable(ctlBackground, ctlForeground, InnerPadding);
			if (!DraggableControls)
				PositionBackgroundForegroundArrow();
		}
		public double BackgroundValue
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(BackgroundValueProperty);
			}
			set
			{
				SetValue(BackgroundValueProperty, value);
			}
		}

		public static readonly DependencyProperty ForegroundValueProperty = DependencyProperty.Register("ForegroundValue", typeof(double), typeof(ForegroundBackgroundPicker), new PropertyMetadata(0.0, new PropertyChangedCallback(OnForegroundValueChanged)));

		private static void OnForegroundValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			ForegroundBackgroundPicker foregroundBackgroundPicker = o as ForegroundBackgroundPicker;
			if (foregroundBackgroundPicker != null)
				foregroundBackgroundPicker.OnForegroundValueChanged((double)e.OldValue, (double)e.NewValue);
		}

		protected virtual void OnForegroundValueChanged(double oldValue, double newValue)
		{
			if (DraggableControls)
				SliderHelper.SetPosition(ctlForeground, newValue);
			else
				SliderHelper.SetPosition(ctlForeground, rctForeground, newValue);
			
			SliderHelper.MoveUpIfSpaceAvailable(ctlForeground, ctlBackground, InnerPadding);

			if (!DraggableControls)
				PositionBackgroundForegroundArrow();
		}
		public double ForegroundValue
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(ForegroundValueProperty);
			}
			set
			{
				SetValue(ForegroundValueProperty, value);
			}
		}
		#endregion

		public ForegroundBackgroundPicker()
		{
			InitializeComponent();
			if (DraggableControls)
			{
				SliderHelper.EnableDrag(ctlBackground);
				ctlBackground.MouseUp += SliderMouseUp;
				SliderHelper.EnableDrag(ctlForeground);
				ctlForeground.MouseUp += SliderMouseUp;
				SliderHelper.PositionChanged += SliderHelper_PositionChanged;
                ctlForeground.MouseDown += CtlForeground_MouseDown;
                ctlBackground.MouseDown += CtlBackground_MouseDown;
            }
		}

		private void SliderMouseUp(object sender, MouseButtonEventArgs e)
		{
			var element = (UIElement)sender;
			PopupIfNeeded(element);
		}

		void PopupIfNeeded(FrameworkElement element1, FrameworkElement element2)
		{
			if (!SliderHelper.DidPopup(element1, element2, InnerPadding))
				SliderHelper.MoveUp(element1, InnerPadding);
			SliderHelper.MoveUpIfSpaceAvailable(element2, element1, InnerPadding);
		}


		void PopupIfNeeded(UIElement uIElement)
		{
			if (uIElement == ctlBackground)
				PopupIfNeeded(ctlBackground, ctlForeground);
			else if (uIElement == ctlForeground)
				PopupIfNeeded(ctlForeground, ctlBackground);
		}

		private void SliderHelper_PositionChanged(object sender, PositionChangedEventArgs ea)
		{
      SetPosition(ea.Element, ea.Position, "Moving");
		}

		void SetPosition(UIElement uIElement, double position, string logMessage)
		{
			if (uIElement == ctlBackground)
			{
				BackgroundValue = position;
				ctlBackground.Tag = position;
				OnBackgroundChanged(this, position);
			}
			else if (uIElement == ctlForeground)
			{
				ForegroundValue = position;
				ctlForeground.Tag = position;
				OnForegroundChanged(this, position);
			}
		}
	}
}
