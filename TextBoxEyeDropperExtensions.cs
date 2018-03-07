using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace HighlightExplorer
{
	public static class TextBoxEyeDropperExtensions
	{
		public static void UpdatePreview(this TextBox textBox, Color color)
		{
			textBox.SetBackgroundColor(color);
		}

		public static void StopGrabbing(this TextBox textBox)
		{

		}

		public static void SetBackgroundColor(this TextBox textBox, Color color)
		{
			if (textBox == null)
				return;
			textBox.Background = new SolidColorBrush(color);
		}
	}
}