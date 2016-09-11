using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace HighlightExplorer
{
	public delegate void PositionChangedHandler(object sender, PositionChangedEventArgs ea);
	public static class SliderHelper
	{
		const double DBL_MaxValue = 524;
		const double DBL_TopPosition = 0;
		const double DBL_BottomPosition = 31;
		const double DBL_MinValue = 54;
		const double DBL_Span = DBL_MaxValue - DBL_MinValue;
		const double DBL_ArrowHeadLength = 17;
		static Nullable<Point> dragStart = null;
		static PositionChangedEventArgs eventArgs;
		public static event PositionChangedHandler PositionChanged;
		public static void OnPositionChanged(object sender, PositionChangedEventArgs ea)
		{
			PositionChanged?.Invoke(sender, ea);
		}
		static void OnPositionChanged(FrameworkElement frameworkElement, double position)
		{
			if (eventArgs == null)
				eventArgs = new PositionChangedEventArgs(frameworkElement, position);
			else
			{
				eventArgs.SetElement(frameworkElement);
				eventArgs.SetPosition(position);
			}
			OnPositionChanged(null, eventArgs);
		}
		public static void EnableDrag(FrameworkElement element)
		{
			element.Tag = 0.0;
			element.MouseDown += ShapeMouseDown;
			element.MouseMove += ShapeDrag;
			element.MouseUp += ShapeMouseUp;
			element.Cursor = Cursors.ScrollWE;
		}

		public static void DisableDrag(FrameworkElement element)
		{
			element.MouseDown -= ShapeMouseDown;
			element.MouseMove -= ShapeDrag;
			element.MouseUp -= ShapeMouseUp;
			element.Cursor = Cursors.Arrow;
		}

		public static void ShapeMouseUp(object sender, MouseButtonEventArgs e)
		{
			var element = (UIElement)sender;
			dragStart = null;
			element.ReleaseMouseCapture();
		}

		static bool IsCloseTo(FrameworkElement element1, FrameworkElement element2)
		{
			var pos1 = (double)(element1.Tag);
			var pos2 = (double)(element2.Tag);
			return Math.Abs(pos1 - pos2) < 0.1;
		}

		public static void MoveUpIfSpaceAvailable(FrameworkElement element1, FrameworkElement element2, int padding)
		{
			if (!IsCloseTo(element1, element2))
				MoveUp(element1, padding);
			if (!IsCloseTo(element2, element1))
				MoveUp(element2, padding);

			if (IsCloseTo(element1, element2) && OnSameLevel(element1, element2))
				MoveUp(element1, padding);
		}

		public static bool DidPopup(FrameworkElement element1, FrameworkElement element2, int padding)
		{
			if (IsCloseTo(element1, element2))
			{
				if (IsUp(element2))
					MoveDown(element1, padding);
				else
					MoveUp(element1, padding);
				return true;
			}
			return false;
		}
	
		// TODO: PositionChanged event that sends the element and the new position.
		static void ShapeDrag(object sender, MouseEventArgs e)
		{
			if (dragStart != null && e.LeftButton == MouseButtonState.Pressed)
			{
				var element = (FrameworkElement)sender;
				var p2 = e.GetPosition((Canvas)element.Parent);

				double newX = p2.X - dragStart.Value.X;

				var halfWidth = ((FrameworkElement)element).ActualWidth / 2;
				double centerX = newX + halfWidth;

				if (centerX < DBL_MinValue)
				{
					centerX = DBL_MinValue;
					newX = centerX - halfWidth;
				}

				if (centerX > DBL_MaxValue)
				{
					centerX = DBL_MaxValue;
					newX = centerX - halfWidth;
				}

				var span = DBL_MaxValue - DBL_MinValue;
				var value = centerX - DBL_MinValue;

				var position = value / span;

				OnPositionChanged(element, position);

				Canvas.SetLeft(element, newX);
				//Canvas.SetTop(element, p2.Y - dragStart.Value.Y);
			}
		}

		public static void MoveDownIfSpaceAvailable(FrameworkElement element1, FrameworkElement element2, int padding)
		{
			if (!IsCloseTo(element1, element2))
				MoveDown(element1, padding);
			if (!IsCloseTo(element2, element1))
				MoveDown(element2, padding);

			if (IsCloseTo(element1, element2) && OnSameLevel(element1, element2))
				MoveDown(element1, padding);
		}
		static bool OnSameLevel(FrameworkElement element1, FrameworkElement element2)
		{
			return Canvas.GetTop(element1) == Canvas.GetTop(element2);
		}

		static bool IsUp(FrameworkElement element)
		{
			return Canvas.GetTop(element) == DBL_TopPosition;
		}

		public static void MoveDown(FrameworkElement element, int padding)
		{
			Canvas.SetTop(element, DBL_BottomPosition - padding);
		}

		public static void MoveUp(FrameworkElement element, int padding)
		{
			Canvas.SetTop(element, DBL_TopPosition + padding);
		}

		public static void PadBelow(FrameworkElement element, int innerPadding)
		{
			Canvas.SetTop(element, DBL_BottomPosition - innerPadding);
		}

		public static void PadAbove(FrameworkElement element, int innerPadding)
		{
			Canvas.SetTop(element, DBL_TopPosition + innerPadding);
		}

		public static void ShapeMouseDown(object sender, MouseButtonEventArgs e)
		{
			var element = (UIElement)sender;
			dragStart = e.GetPosition(element);
			element.CaptureMouse();
			BringToFront(element);
		}

		public static void SetPosition(FrameworkElement element, Rectangle rect, double newValue)
		{
			SetPosition(element, newValue);
			SetPosition(rect, newValue);
		}

		public static void SetPosition(FrameworkElement element, double newValue)
		{
			element.Tag = newValue;

			var halfWidth = element.ActualWidth / 2;
			double leftX = DBL_MinValue - halfWidth + newValue * DBL_Span;

			Canvas.SetLeft(element, leftX);
		}

		public static void PositionArrow(StackPanel stackPanel, double value1, double value2, Rectangle arrowShaft, TextBlock textBlock)
		{
			double left = DBL_MinValue + value1 * DBL_Span;
			double right = DBL_MinValue + value2 * DBL_Span;
			if (left > right)
			{
				double temp = left;
				left = right;
				right = temp;
			}
			Canvas.SetLeft(stackPanel, left);
			var width = right - left;
			double arrowShaftWidth = width - DBL_ArrowHeadLength - 5;
			if (arrowShaftWidth < 0)
			{
				stackPanel.Visibility = Visibility.Hidden;
			}
			else
			{
				stackPanel.Visibility = Visibility.Visible;
				stackPanel.Width = width;
				arrowShaft.Width = arrowShaftWidth;
				double distance = Math.Abs(value2 - value1) * 100.0;
				textBlock.Text = string.Format("{0:0.00}%", distance);
			}
		}

		static void BringToFront(UIElement element)
		{
			if (element == null)
				return;

			FrameworkElement frameworkElement = element as FrameworkElement;

			if (frameworkElement == null)
				return;

			Panel parent = frameworkElement.Parent as Panel;
			if (parent == null)
				return;

			var maxZ = parent.Children.OfType<UIElement>()
				.Where(x => x != element)
				.Select(x => Panel.GetZIndex(x))
				.Max();
			Panel.SetZIndex(element, maxZ + 1);
		}
	}
}
