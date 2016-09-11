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
	/// Interaction logic for HighlightTextPicker.xaml
	/// </summary>
	public partial class HighlightTextPicker : UserControl
	{
		public static readonly DependencyProperty InnerPaddingProperty = DependencyProperty.Register("InnerPadding", typeof(int), typeof(HighlightTextPicker), new PropertyMetadata(0, new PropertyChangedCallback(OnInnerPaddingChanged)));
		
		private static void OnInnerPaddingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			HighlightTextPicker highlightTextPicker = o as HighlightTextPicker;
			if (highlightTextPicker != null)
				highlightTextPicker.OnInnerPaddingChanged((int)e.OldValue, (int)e.NewValue);
		}

		protected virtual void OnInnerPaddingChanged(int oldValue, int newValue)
		{
			SliderHelper.PadBelow(ctlHighlight, InnerPadding);
			SliderHelper.PadBelow(ctlText, InnerPadding);
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

		public static readonly DependencyProperty DraggableControlsProperty = DependencyProperty.Register("DraggableControls", typeof(bool), typeof(HighlightTextPicker), new PropertyMetadata(true, new PropertyChangedCallback(OnDraggableControlsChanged)));

		private static void OnDraggableControlsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			HighlightTextPicker highlightTextPicker = o as HighlightTextPicker;
			if (highlightTextPicker != null)
				highlightTextPicker.OnDraggableControlsChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		protected virtual void OnDraggableControlsChanged(bool oldValue, bool newValue)
		{
			if (DraggableControls)
			{
				BackgroundMarker.Visibility = Visibility.Hidden;
				rctHighlight.Visibility = Visibility.Hidden;
				rctText.Visibility = Visibility.Hidden;
				ArrowBlue.Visibility = Visibility.Hidden;
				ArrowRed.Visibility = Visibility.Hidden;
				SliderHelper.EnableDrag(ctlHighlight);
				ctlHighlight.MouseUp += SliderMouseUp;
				SliderHelper.EnableDrag(ctlText);
				ctlText.MouseUp += SliderMouseUp;
				SliderHelper.PositionChanged += SliderHelper_PositionChanged;
			}
			else
			{
				BackgroundMarker.Visibility = Visibility.Visible;
				ArrowBlue.Visibility = Visibility.Visible;
				ArrowRed.Visibility = Visibility.Visible;
				rctHighlight.Visibility = Visibility.Visible;
				rctText.Visibility = Visibility.Visible;
				SliderHelper.DisableDrag(ctlHighlight);
				ctlHighlight.MouseUp -= SliderMouseUp;
				SliderHelper.DisableDrag(ctlText);
				ctlText.MouseUp -= SliderMouseUp;
				SliderHelper.PositionChanged -= SliderHelper_PositionChanged;
			}
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

		public event EventHandler<double> HighlightChanged;
		public event EventHandler<double> TextChanged;
		protected virtual void OnTextChanged(object sender, double e)
		{
			TextChanged?.Invoke(sender, e);
		}
		protected virtual void OnHighlightChanged(object sender, double e)
		{
			HighlightChanged?.Invoke(sender, e);
		}

		#region Dependency Properties...
		public static readonly DependencyProperty BackgroundValueProperty = DependencyProperty.Register("BackgroundValue", typeof(double), typeof(HighlightTextPicker), new PropertyMetadata(0.0, new PropertyChangedCallback(OnBackgroundValueChanged)));

		private static void OnBackgroundValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			HighlightTextPicker highlightTextPicker = o as HighlightTextPicker;
			if (highlightTextPicker != null)
				highlightTextPicker.OnBackgroundValueChanged((double)e.OldValue, (double)e.NewValue);
		}

		void PositionBackgroundHighlightArrow()
		{
			CheckOverlap();
			SliderHelper.PositionArrow(ArrowRed, BackgroundValue, HighlightValue, ArrowShaftRed, tbRedPercentDistance);
		}

		protected virtual void OnBackgroundValueChanged(double oldValue, double newValue)
		{
			if (!DraggableControls)
			{
				SliderHelper.SetPosition(BackgroundMarker, newValue);
				PositionBackgroundHighlightArrow();
			}
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

		public static readonly DependencyProperty TextValueProperty = DependencyProperty.Register("TextValue", typeof(double), typeof(HighlightTextPicker), new PropertyMetadata(0.0, new PropertyChangedCallback(OnTextValueChanged)));

		private static void OnTextValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			HighlightTextPicker highlightTextPicker = o as HighlightTextPicker;
			if (highlightTextPicker != null)
				highlightTextPicker.OnTextValueChanged((double)e.OldValue, (double)e.NewValue);
		}

		protected virtual void OnTextValueChanged(double oldValue, double newValue)
		{
			if (DraggableControls)
				SliderHelper.SetPosition(ctlText, newValue);
			else
				SliderHelper.SetPosition(ctlText, rctText, newValue);
			
			SliderHelper.MoveDownIfSpaceAvailable(ctlText, ctlHighlight, InnerPadding);

			if (!DraggableControls)
				PositionHighlightTextArrow();
		}
		public double TextValue
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(TextValueProperty);
			}
			set
			{
				SetValue(TextValueProperty, value);
			}
		}

		public static readonly DependencyProperty HighlightValueProperty = DependencyProperty.Register("HighlightValue", typeof(double), typeof(HighlightTextPicker), new PropertyMetadata(0.0, new PropertyChangedCallback(OnHighlightValueChanged)));

		private static void OnHighlightValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			HighlightTextPicker highlightTextPicker = o as HighlightTextPicker;
			if (highlightTextPicker != null)
				highlightTextPicker.OnHighlightValueChanged((double)e.OldValue, (double)e.NewValue);
		}

		protected virtual void OnHighlightValueChanged(double oldValue, double newValue)
		{
			//SetPosition(ctlHighlight, newValue);
			if (DraggableControls)
				SliderHelper.SetPosition(ctlHighlight, newValue);
			else
				SliderHelper.SetPosition(ctlHighlight, rctHighlight, newValue);

			SliderHelper.MoveDownIfSpaceAvailable(ctlHighlight, ctlText, InnerPadding);
			if (!DraggableControls)
			{
				PositionBackgroundHighlightArrow();
				PositionHighlightTextArrow();
			}
		}

		void PositionHighlightTextArrow()
		{
			CheckOverlap();
			SliderHelper.PositionArrow(ArrowBlue, TextValue, HighlightValue, ArrowShaftBlue, tbBluePercentDistance);
		}

		private void CheckOverlap()
		{
			ArrowsOverlap = TextValue < HighlightValue && BackgroundValue < HighlightValue || TextValue > HighlightValue && BackgroundValue > HighlightValue;
		}

		public double HighlightValue
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (double)GetValue(HighlightValueProperty);
			}
			set
			{
				SetValue(HighlightValueProperty, value);
			}
		}

		bool arrowsOverlap;

		public bool ArrowsOverlap
		{
			get
			{
				return arrowsOverlap;
			}

			set
			{
				if (arrowsOverlap == value)
					return;
				arrowsOverlap = value;
				if (arrowsOverlap)
				{
					Canvas.SetTop(ArrowBlue, 30);
					InnerPadding = 28;
				}
				else
				{
					Canvas.SetTop(ArrowBlue, 50);
					InnerPadding = 12;
				}
			}
		}
		#endregion

		public HighlightTextPicker()
		{
			InitializeComponent();
			if (DraggableControls)
			{
				SliderHelper.EnableDrag(ctlHighlight);
				ctlHighlight.MouseUp += SliderMouseUp;
				SliderHelper.EnableDrag(ctlText);
				ctlText.MouseUp += SliderMouseUp;
				SliderHelper.PositionChanged += SliderHelper_PositionChanged;
			}
		}

		private void SliderHelper_PositionChanged(object sender, PositionChangedEventArgs ea)
		{
			SetPosition(ea.Element, ea.Position);
		}

		private void SliderMouseUp(object sender, MouseButtonEventArgs e)
		{
			var element = (UIElement)sender;
			PopupIfNeeded(element);
		}

		bool OnSameLevel(FrameworkElement element1, FrameworkElement element2)
		{
			return Canvas.GetTop(element1) == Canvas.GetTop(element2);
		}




		void PopupIfNeeded(FrameworkElement element1, FrameworkElement element2)
		{
			if (!SliderHelper.DidPopup(element1, element2, InnerPadding))
				SliderHelper.MoveDown(element1, InnerPadding);
			SliderHelper.MoveDownIfSpaceAvailable(element1, element2, InnerPadding);
		}



		void PopupIfNeeded(UIElement uIElement)
		{
			if (uIElement == ctlHighlight)
				PopupIfNeeded(ctlHighlight, ctlText);
			else if (uIElement == ctlText)
				PopupIfNeeded(ctlText, ctlHighlight);
		}

		void SetPosition(UIElement uIElement, double position)
		{
			if (uIElement == ctlHighlight)
			{
				HighlightValue = position;
				ctlHighlight.Tag = position;
				OnHighlightChanged(this, position);
			}
			else if (uIElement == ctlText)
			{
				TextValue = position;
				ctlText.Tag = position;
				OnTextChanged(this, position);
			}
		}
	}
}
