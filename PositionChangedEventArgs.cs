using System;
using System.Windows;

namespace HighlightExplorer
{
	public class PositionChangedEventArgs: EventArgs
	{
		public UIElement Element { get; private set; }
		public double Position { get; private set; }
		public PositionChangedEventArgs(UIElement element, double position)
		{
			Element = element;
			Position = position;
		}

		internal void SetPosition(double position)
		{
			Position = position;
		}

		internal void SetElement(UIElement element)
		{
			Element = element;
		}
	}
}