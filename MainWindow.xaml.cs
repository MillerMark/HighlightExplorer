using StepDiagrammer;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		double highlightBrightness;
		double highlightHue;
		double highlightSaturation;
		double textBrightness;
		double textHue;
		double textSaturation;
		double backgroundBrightness;
		double backgroundHue;
		double backgroundSaturation;
		double foregroundBrightness;
		double foregroundHue;
		bool changingInternally;
		bool maintainHighlightBrightness;
		double lastHighlightBrightness;
		double foregroundSaturation;
		public MainWindow()
		{
			InitializeComponent();
			BuildHueBar();
			StyleW3Warnings();
		}
		void StyleW3Warnings()
		{
			StyleWarning(tbBackForeWCAGWarningAA);
			StyleWarning(tbBackForeWCAGWarningAALarge);
			StyleWarning(tbBackForeWCAGWarningAAA);
			StyleWarning(tbBackForeWCAGWarningAAALarge);
			StyleWarning(tbHighTextWCAGWarningAA);
			StyleWarning(tbHighTextWCAGWarningAALarge);
			StyleWarning(tbHighTextWCAGWarningAAA);
			StyleWarning(tbHighTextWCAGWarningAAALarge);
		}

		private void BuildHueBar()
		{
			var fill = new LinearGradientBrush();
			for (double i = 0; i < 1; i += 0.01)
			{
				var hueSatLight = new HueSatLight(i, 1.0, 0.5);
				var stop = new GradientStop();
				stop.Color = hueSatLight.AsRGB;
				stop.Offset = i;
				fill.GradientStops.Add(stop);
			}

			this.barHue.Fill = fill;
		}

		void HighlightChanged()
		{
			HueSatLight hueSatLight = GetHighlightColor();
			clrHighlight.Fill = new SolidColorBrush(hueSatLight.AsRGB);

			var newGrayScale = hueSatLight.AsGrayScale;
			clrHighlightGrayscale.Fill = new SolidColorBrush(newGrayScale);

			ctlPerceivedBrightnessHighlightText.HighlightValue = newGrayScale.GetBrightness();
			UpdateIssues();
			UpdateColorTextBoxes();
		}

		private HueSatLight GetHighlightColor()
		{
			var hueSatLight = new HueSatLight();
			hueSatLight.Hue = highlightHue;
			hueSatLight.Saturation = highlightSaturation;
			hueSatLight.Lightness = highlightBrightness;

			//if (maintainHighlightBrightness)
			//{
			// TODO: Implement ChangeLightPerceivedBrightnessTo.
			//	hueSatLight.ChangeLightPerceivedBrightnessTo(lastHighlightBrightness);
			//	highlightSaturation = hueSatLight.Saturation;
			//	highlightBrightness = hueSatLight.Lightness;
			//}
			return hueSatLight;
		}

		void TextChanged()
		{
			HueSatLight hueSatLight = GetTextColor();
			clrHighlightTextColor.Foreground = new SolidColorBrush(hueSatLight.AsRGB);

			var newGrayScale = hueSatLight.AsGrayScale;
			clrHighlightTextGrayscale.Foreground = new SolidColorBrush(newGrayScale);

			ctlPerceivedBrightnessHighlightText.TextValue = newGrayScale.GetBrightness();
			UpdateIssues();
			UpdateColorTextBoxes();
		}

		private HueSatLight GetTextColor()
		{
			var hueSatLight = new HueSatLight();
			hueSatLight.Hue = textHue;
			hueSatLight.Saturation = textSaturation;
			hueSatLight.Lightness = textBrightness;
			return hueSatLight;
		}

		void ForegroundChanged()
		{
			HueSatLight hueSatLight = GetForegroundColor();
			var newColor = hueSatLight.AsRGB;
			this.clrForegroundColor1.Foreground = new SolidColorBrush(newColor);
			this.clrForegroundColor2.Foreground = new SolidColorBrush(newColor);
			this.clrForegroundColor3.Foreground = new SolidColorBrush(newColor);

			var newGrayScale = hueSatLight.AsGrayScale;
			this.clrForegroundGrayscale1.Foreground = new SolidColorBrush(newGrayScale);
			this.clrForegroundGrayscale2.Foreground = new SolidColorBrush(newGrayScale);
			this.clrForegroundGrayscale3.Foreground = new SolidColorBrush(newGrayScale);

			ctlPerceivedBrightnessBackgroundForeground.ForegroundValue = newGrayScale.GetBrightness();
			UpdateIssues();
			UpdateColorTextBoxes();
		}

		private HueSatLight GetForegroundColor()
		{
			var hueSatLight = new HueSatLight();
			hueSatLight.Hue = foregroundHue;
			hueSatLight.Saturation = foregroundSaturation;
			hueSatLight.Lightness = foregroundBrightness;
			return hueSatLight;
		}

		void BackgroundChanged()
		{
			HueSatLight hueSatLight = GetBackgroundColor();
			clrBackground.Fill = new SolidColorBrush(hueSatLight.AsRGB);

			var newGrayScale = hueSatLight.AsGrayScale;

			clrBackgroundGrayscale.Fill = new SolidColorBrush(newGrayScale);

			ctlPerceivedBrightnessBackgroundForeground.BackgroundValue = newGrayScale.GetBrightness();
			ctlPerceivedBrightnessHighlightText.BackgroundValue = newGrayScale.GetBrightness();
			UpdateIssues();
			UpdateColorTextBoxes();
		}

		private HueSatLight GetBackgroundColor()
		{
			var hueSatLight = new HueSatLight();
			hueSatLight.Hue = backgroundHue;
			hueSatLight.Saturation = backgroundSaturation;
			hueSatLight.Lightness = backgroundBrightness;
			return hueSatLight;
		}

		void UpdateColorTextBoxes()
		{
			if (changingInternally)
				return;
			changingInternally = true;
			try
			{
				tbxBackground.Text = GetBackgroundColor().AsHtml;
				tbxForeground.Text = GetForegroundColor().AsHtml;
				tbxHighlight.Text = GetHighlightColor().AsHtml;
				tbxHighlightText.Text = GetTextColor().AsHtml;
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void UpdateIssues()
		{
			var backgroundToForegroundDistance = Math.Abs(ctlPerceivedBrightnessBackgroundForeground.BackgroundValue - ctlPerceivedBrightnessBackgroundForeground.ForegroundValue) * 100;
			ShowIssue(backgroundToForegroundDistance, 25, "Background and foreground colors should be more distinct.", "Background and foreground colors are close and could be more distinct.", tbDistanceBackgroundToForeground, ctlPerceivedBrightnessBackgroundForeground.tbGrayPercentDistance);

			var highlightDistance = Math.Abs(ctlPerceivedBrightnessBackgroundForeground.BackgroundValue - ctlPerceivedBrightnessHighlightText.HighlightValue) * 100;
			ShowIssue(highlightDistance, 5, "Background and highlight colors should be more distinct.", "Background and highlight colors are close and could be more distinct.", tbDistanceBackgroundToHighlight, ctlPerceivedBrightnessHighlightText.tbRedPercentDistance);

			var textDistance = Math.Abs(ctlPerceivedBrightnessHighlightText.TextValue - ctlPerceivedBrightnessHighlightText.HighlightValue) * 100;
			
			ShowIssue(textDistance, 20, "Text and highlight colors should be more distinct.", "Text and highlight colors are close and could be more distinct.", tbDistanceHighlightToText, ctlPerceivedBrightnessHighlightText.tbBluePercentDistance);

			var backgroundColor = GetBackgroundColor();
			var foregroundColor = GetForegroundColor();
			var highlightColor = GetHighlightColor();
			var textColor = GetTextColor();
			ShowWcagWarnings(backgroundColor, foregroundColor, highlightColor, textColor);

			double backgroundSaturationLevel = GetSaturationLevel(backgroundColor);
			double foregroundSaturationLevel = GetSaturationLevel(foregroundColor);
			double highlightSaturationLevel = GetSaturationLevel(highlightColor);
			double textSaturationLevel = GetSaturationLevel(textColor);

			ShowSaturationIssue(backgroundSaturationLevel, 0.8, 0.3, tbBackgroundSaturationWarning, "Background appears to be too saturated.", "Background may be too saturated.");
			ShowSaturationIssue(foregroundSaturationLevel, 0.9, 0.8, tbForegroundSaturationWarning, "Foreground text appears to be too saturated.", "Foreground text may be too saturated.");
			ShowSaturationIssue(highlightSaturationLevel, 2.0 /* no warning for highlight saturation */, 0.85, tbHighlightSaturationWarning, "", "Highlight color may be too saturated.");

			if (tbDistanceBackgroundToForeground.Visibility == Visibility.Visible ||
				tbDistanceBackgroundToHighlight.Visibility == Visibility.Visible ||
				tbDistanceHighlightToText.Visibility == Visibility.Visible ||
				tbBackgroundSaturationWarning.Visibility == Visibility.Visible ||
				tbForegroundSaturationWarning.Visibility == Visibility.Visible ||
				tbHighlightSaturationWarning.Visibility == Visibility.Visible ||
				tbBackForeWCAGWarningAA.Visibility == Visibility.Visible ||
				tbBackForeWCAGWarningAALarge.Visibility == Visibility.Visible ||
				tbBackForeWCAGWarningAAA.Visibility == Visibility.Visible ||
				tbBackForeWCAGWarningAAALarge.Visibility == Visibility.Visible ||
				tbHighTextWCAGWarningAA.Visibility == Visibility.Visible ||
				tbHighTextWCAGWarningAALarge.Visibility == Visibility.Visible ||
				tbHighTextWCAGWarningAAA.Visibility == Visibility.Visible ||
				tbHighTextWCAGWarningAAALarge.Visibility == Visibility.Visible)
				tbIssues.Visibility = Visibility.Visible;
			else
				tbIssues.Visibility = Visibility.Hidden;

			//Title = string.Format("B: {0:0.000}, F: {1:0.000}, Highlight: {2:0.000}, Text: {3:0.000}", backgroundSaturationLevel, foregroundSaturationLevel, highlightSaturationLevel, textSaturationLevel);
		}

		void ShowBackForeContrastValues(double contrastRatio)
		{
			SetContrastValue(contrastRatio >= 3, icoBackForeAaLargeYes, icoBackForeAaLargeNo);
			SetContrastValue(contrastRatio >= 4.5, icoBackForeAaaLargeYes, icoBackForeAaaLargeNo);
			SetContrastValue(contrastRatio >= 4.5, icoBackForeAaYes, icoBackForeAaNo);
			SetContrastValue(contrastRatio >= 7, icoBackForeAaaYes, icoBackForeAaaNo);
		}

		void ShowHighTextContrastValues(double contrastRatio)
		{
			SetContrastValue(contrastRatio >= 3, icoHighTextAaLargeYes, icoHighTextAaLargeNo);
			SetContrastValue(contrastRatio >= 4.5, icoHighTextAaaLargeYes, icoHighTextAaaLargeNo);
			SetContrastValue(contrastRatio >= 4.5, icoHighTextAaYes, icoHighTextAaNo);
			SetContrastValue(contrastRatio >= 7, icoHighTextAaaYes, icoHighTextAaaNo);
		}

		private void SetContrastValue(bool isSet, Viewbox yes, Viewbox no)
		{
			if (isSet)
			{
				yes.Visibility = Visibility.Visible;
				no.Visibility = Visibility.Hidden;
			}
			else
			{
				yes.Visibility = Visibility.Hidden;
				no.Visibility = Visibility.Visible;
			}
		}

		private void ShowWcagWarnings(HueSatLight backgroundColor, HueSatLight foregroundColor, HueSatLight highlightColor, HueSatLight textColor)
		{
			if (tbBackForeWCAGWarningAA == null)
				return;
			double backForeContrastRatio = GetContrastRatio(backgroundColor, foregroundColor);
			ShowBackForeContrastValues(backForeContrastRatio);
			//ShowContrastRatioWarnings("Background/Foreground", backForeContrastRatio, tbBackForeWCAGWarningAA, tbBackForeWCAGWarningAALarge, tbBackForeWCAGWarningAAA, tbBackForeWCAGWarningAAALarge);

			double highlightTextContrastRatio = GetContrastRatio(highlightColor, textColor);
			ShowHighTextContrastValues(highlightTextContrastRatio);
			//ShowContrastRatioWarnings("Highlight/Text", highlightTextContrastRatio, tbHighTextWCAGWarningAA, tbHighTextWCAGWarningAALarge, tbHighTextWCAGWarningAAA, tbHighTextWCAGWarningAAALarge);

			tbBackForeContrastRatio.Text = string.Format("{0:0.##} : 1", backForeContrastRatio);
			tbHighTextContrastRatio.Text = string.Format("{0:0.##} : 1", highlightTextContrastRatio);
		}

		//private void ShowContrastRatioWarnings(string ratioName, double contrastRatio, TextBlock warningAA, TextBlock warningAALarge, TextBlock warningAAA, TextBlock warningAAALarge)
		//{
		//	warningAA.Visibility = Visibility.Collapsed;
		//	warningAALarge.Visibility = Visibility.Collapsed;
		//	warningAAA.Visibility = Visibility.Collapsed;
		//	warningAAALarge.Visibility = Visibility.Collapsed;
		//	return;

		//	if (contrastRatio < 7)
		//	{
		//		if (contrastRatio < 3)
		//		{
		//			//icoBackForeAaaLargeYes
		//			//warningAALarge.Text = string.Format("{1} contrast ratio ({0:0.##}:1) fails to meet WCAG 2.0 1.4.3 (Level AA) guidelines for large text.", contrastRatio, ratioName);
		//			//warningAALarge.Visibility = Visibility.Visible;
		//		}

		//		if (contrastRatio < 4.5)
		//		{
		//			warningAA.Text = string.Format("{1} contrast ratio ({0:0.##}:1) fails to meet WCAG 2.0 1.4.3 (Level AA) guidelines for normal text.", contrastRatio, ratioName);
		//			warningAAALarge.Text = string.Format("{1} contrast ratio ({0:0.##}:1) fails to meet WCAG 2.0 1.4.6 (Level AAA) guidelines for large text.", contrastRatio, ratioName);
		//			warningAA.Visibility = Visibility.Visible;
		//			warningAAALarge.Visibility = Visibility.Visible;
		//		}
		//		warningAAA.Text = string.Format("{1} contrast ratio ({0:0.##}:1) fails to meet WCAG 2.0 1.4.6 (Level AAA) guidelines for normal text.", contrastRatio, ratioName);
		//		warningAAA.Visibility = Visibility.Visible;
		//	}
		//}

		private static double GetContrastRatio(HueSatLight backgroundColor, HueSatLight foregroundColor)
		{
			double backForeContrastRatio = (backgroundColor.GetRelativeLuminance() + 0.05) / (foregroundColor.GetRelativeLuminance() + 0.05);
			if (backForeContrastRatio < 1)
				backForeContrastRatio = 1 / backForeContrastRatio;
			return backForeContrastRatio;
		}

		private void ShowSaturationIssue(double saturationLevel, double warningThreshold, double hintThreshold, TextBlock textBlock, string warningMsg, string hintMsg)
		{
			if (saturationLevel >= hintThreshold)
			{
				textBlock.Visibility = Visibility.Visible;
				if (saturationLevel >= warningThreshold)
				{
					textBlock.Text = "WARNING: " + warningMsg;
					StyleWarning(textBlock);
				}
				else
				{
					textBlock.Text = "HINT: " + hintMsg;
					StyleHint(textBlock);
				}
			}
			else
				textBlock.Visibility = Visibility.Collapsed;
		}

		private void ShowIssue(double value, int minValue, string warningMsg, string hintMsg, TextBlock issueText, TextBlock percentText)
		{
			if (value < 2 * minValue)
			{
				if (value < minValue)
				{
					StyleWarning(issueText);
					StyleWarning(percentText);
					issueText.Text = "WARNING: " + warningMsg;
				}
				else
				{
					StyleHint(issueText);
					StyleHint(percentText);
					issueText.Text = "HINT: " + hintMsg;
				}
				issueText.Visibility = Visibility.Visible;
			}
			else
			{
				issueText.Visibility = Visibility.Collapsed;
				StyleNormal(percentText);
			}
		}

		private static double GetSaturationLevel(HueSatLight backgroundColor)
		{
			return 2 * backgroundColor.Saturation * (0.5 - Math.Abs(backgroundColor.Lightness - 0.5));
		}

		//double GetColorDifference(Luv luv1, Luv luv2)
		//{
		//	var deltaL = luv1.L - luv2.L;
		//	var deltaU = luv1.U - luv2.U;
		//	var deltaV = luv1.V - luv2.V;
		//	double difference = Math.Sqrt(deltaL * deltaL + deltaU * deltaU + deltaV * deltaV);
		//	return difference;
		//}

		//private void ShowDistance(double distance, TextBlock textBlock1, TextBlock textBlock2, double minPercent)
		//{
		//	textBlock1.Text = string.Format("{0:0.00}%", distance);
		//	if (distance < minPercent)
		//	{
		//		textBlock1.Text += " (WARNING: Distance Not Safe!)";
		//		StyleHint(textBlock1);
		//		StyleHint(textBlock2);
		//	}
		//	else if (distance < minPercent * 2)
		//	{
		//		textBlock1.Text += " (HINT: Distance could be wider)";
		//		StyleWarning(textBlock1);
		//		StyleWarning(textBlock2);
		//	}
		//	else
		//	{
		//		StyleNormal(textBlock1);
		//		StyleNormal(textBlock2);
		//	}
		//}

		private static void StyleNormal(TextBlock textBlock1)
		{
			textBlock1.Background = new SolidColorBrush(Colors.White);
			textBlock1.Foreground = new SolidColorBrush(Colors.Black);
		}

		private static void StyleWarning(TextBlock textBlock1)
		{
			textBlock1.Background = new SolidColorBrush(Color.FromRgb(255, 209, 209));
			textBlock1.Foreground = new SolidColorBrush(Color.FromRgb(76, 0, 0));
		}

		private static void StyleHint(TextBlock textBlock1)
		{
			textBlock1.Background = new SolidColorBrush(Color.FromRgb(209, 235, 255));
			textBlock1.Foreground = new SolidColorBrush(Color.FromRgb(0, 58, 102));
		}

		private void ctlBrightnessHighlightText_TextChanged(object sender, double e)
		{
			textBrightness = e;
			TextChanged();
		}

		private void ctlBrightnessHighlightText_HighlightChanged(object sender, double e)
		{
			highlightBrightness = e;
			HighlightChanged();
		}

		private void ctlBrightnessBackgroundForeground_ForegroundChanged(object sender, double e)
		{
			foregroundBrightness = e;
			ForegroundChanged();
		}

		private void ctlBrightnessBackgroundForeground_BackgroundChanged(object sender, double e)
		{
			backgroundBrightness = e;
			BackgroundChanged();
		}

		private void ctlHueHighlightText_HighlightChanged(object sender, double e)
		{
			highlightHue = e;
			HighlightChanged();
		}

		private void ctlHueHighlightText_TextChanged(object sender, double e)
		{
			textHue = e;
			TextChanged();
		}

		private void ctlHueBackgroundForeground_BackgroundChanged(object sender, double e)
		{
			backgroundHue = e;
			BackgroundChanged();
		}

		private void ctlHueBackgroundForeground_ForegroundChanged(object sender, double e)
		{
			foregroundHue = e;
			ForegroundChanged();
		}

		private void ctlSaturationHighlightText_HighlightChanged(object sender, double e)
		{
			highlightSaturation = e;
			HighlightChanged();
		}

		private void ctlSaturationHighlightText_TextChanged(object sender, double e)
		{
			textSaturation = e;
			TextChanged();
		}

		private void ctlSaturationBackgroundForeground_BackgroundChanged(object sender, double e)
		{
			backgroundSaturation = e;
			BackgroundChanged();
		}

		private void ctlSaturationBackgroundForeground_ForegroundChanged(object sender, double e)
		{
			foregroundSaturation = e;
			ForegroundChanged();
		}

		void SetColors(string backStr, string foreStr, string highlightStr, string textStr)
		{
			tbxBackground.Text = backStr;
			tbxForeground.Text = foreStr;
			tbxHighlight.Text = highlightStr;
			tbxHighlightText.Text = textStr;
		}

		private void tbxBackground_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (changingInternally)
				return;
			changingInternally = true;
			try
			{
				var hueSatLight = new HueSatLight(tbxBackground.Text);
				ctlBrightnessBackgroundForeground.BackgroundValue = hueSatLight.Lightness;
				ctlHueBackgroundForeground.BackgroundValue = hueSatLight.Hue;
				ctlSaturationBackgroundForeground.BackgroundValue = hueSatLight.Saturation;

				backgroundHue = hueSatLight.Hue;
				backgroundSaturation = hueSatLight.Saturation;
				backgroundBrightness = hueSatLight.Lightness;

				BackgroundChanged();
			}
			catch
			{
				return;
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void tbxHighlight_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (changingInternally)
				return;

			changingInternally = true;
			try
			{
				var hueSatLight = new HueSatLight(tbxHighlight.Text);
				ctlBrightnessHighlightText.HighlightValue = hueSatLight.Lightness;
				ctlHueHighlightText.HighlightValue = hueSatLight.Hue;
				ctlSaturationHighlightText.HighlightValue = hueSatLight.Saturation;


				highlightHue = hueSatLight.Hue;
				highlightSaturation = hueSatLight.Saturation;
				highlightBrightness = hueSatLight.Lightness;

				HighlightChanged();
			}
			catch
			{
				return;
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void tbxHighlightText_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (changingInternally)
				return;

			changingInternally = true;
			try
			{
				var hueSatLight = new HueSatLight(tbxHighlightText.Text);
				ctlBrightnessHighlightText.TextValue = hueSatLight.Lightness;
				ctlHueHighlightText.TextValue = hueSatLight.Hue;
				ctlSaturationHighlightText.TextValue = hueSatLight.Saturation;


				textHue = hueSatLight.Hue;
				textSaturation = hueSatLight.Saturation;
				textBrightness = hueSatLight.Lightness;

				TextChanged();
			}
			catch
			{
				return;
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void tbxForeground_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (changingInternally)
				return;

			changingInternally = true;
			try
			{
				var hueSatLight = new HueSatLight(tbxForeground.Text);
				ctlBrightnessBackgroundForeground.ForegroundValue = hueSatLight.Lightness;
				ctlHueBackgroundForeground.ForegroundValue = hueSatLight.Hue;
				ctlSaturationBackgroundForeground.ForegroundValue = hueSatLight.Saturation;

				foregroundHue = hueSatLight.Hue;
				foregroundSaturation = hueSatLight.Saturation;
				foregroundBrightness = hueSatLight.Lightness;

				ForegroundChanged();
			}
			catch
			{
				return;
			}
			finally
			{
				changingInternally = false;
			}
		}

		private void rbnGreen_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#FEFFFF", "#54514F", "#48EC32", "#213A1E");
		}

		private void rbnReverse_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#FFFFFF", "#000000", "#18529F", "#FFFFFF");
		}

		private void rbnDarkTheme_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#000000", "#DDDDDD", "#A2275C", "#FFFFFF");
		}

		private void rbnSameBrightness_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#595FB5", "#686816", "#696B00", "#4D55FE");
		}

		private void rbnWideLightness_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#8E95FC", "#737300", "#A9AC00", "#C8CBFF");
		}

		private void rbnMinDistances_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#FFFFFF", "#BEBEBE", "#FAEFF1", "#D89F9F");
		}

		private void rbnMinDistances_Loaded(object sender, RoutedEventArgs e)
		{
			SetColors("#000000", "#000000", "#000000", "#000000");
			SetColors("#FFFFFF", "#33559C", "#A8F6E9", "#002A23");
		}

		private void rbnVSRename_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#77c177", "#006400", "#5bb1ab", "#006400");
		}

		private void rbnHighlightYellow_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#FFFFFF", "#000000", "#FDD64F", "#000000");
		}

		private void rbnBlues_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#000E29", "#B0CCFF", "#1A4BA4", "#EEF4FF");
		}

		private void rbnH1_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#e8f0fe", "#000000", "#8FB1EB", "#FFFFFF");
		}

		private void rbnH2_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#E7EFFE", "#000000", "#266BDA", "#000000");
		}

		private void rbnGray_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#737373", "#FFFFFF", "#FFFFFF", "#141414");
		}

		private void rbnTeal_Checked(object sender, RoutedEventArgs e)
		{
			SetColors("#FFFCFC", "#000000", "#63C5E7", "#000000");
		}

		private void tbxBackground_GotFocus(object sender, RoutedEventArgs e)
		{
			tbxBackground.SelectAll();
		}

		private void tbxForeground_GotFocus(object sender, RoutedEventArgs e)
		{
			tbxForeground.SelectAll();
		}

		private void tbxHighlight_GotFocus(object sender, RoutedEventArgs e)
		{
			tbxHighlight.SelectAll();
		}

		private void tbxHighlightText_GotFocus(object sender, RoutedEventArgs e)
		{
			tbxHighlightText.SelectAll();
		}

		private void chkShowWcagWarnings_Checked(object sender, RoutedEventArgs e)
		{
			UpdateWcagWarnings();
		}

		private void UpdateWcagWarnings()
		{
			var backgroundColor = GetBackgroundColor();
			var foregroundColor = GetForegroundColor();
			var highlightColor = GetHighlightColor();
			var textColor = GetTextColor();
			ShowWcagWarnings(backgroundColor, foregroundColor, highlightColor, textColor);
		}

		private void chkShowWcagWarnings_Unchecked(object sender, RoutedEventArgs e)
		{
			UpdateWcagWarnings();
		}

		private void ctlHueHighlightText_BeforeHighlightChanged(object sender, double e)
		{
			// TODO: Implement shift+Hue slider dragging to maintain perceived brightness
			//if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			//{
			//	maintainHighlightBrightness = true;
			//	var highlightColor = GetHighlightColor();
			//	lastHighlightBrightness = highlightColor.AsGrayScale.GetBrightness();
			//}
			//else
			//	maintainHighlightBrightness = false;
		}

		private void ctlHueHighlightText_BeforeTextChanged(object sender, double e)
		{
			// TODO: Implement shift+Hue slider dragging to maintain perceived brightness
		}

		private void ctlHueHighlightText_AfterHighlightChanged(object sender, double e)
		{
			// TODO: Implement shift+Hue slider dragging to maintain perceived brightness
			//maintainHighlightBrightness = false;
		}

		private void ctlHueHighlightText_AfterTextChanged(object sender, double e)
		{
			// TODO: Implement shift+Hue slider dragging to maintain perceived brightness
		}

		private void ctlHueBackgroundForeground_AfterBackgroundChanged(object sender, double e)
		{
			// TODO: Implement shift+Hue slider dragging to maintain perceived brightness
		}

		private void ctlHueBackgroundForeground_AfterForegroundChanged(object sender, double e)
		{
			// TODO: Implement shift+Hue slider dragging to maintain perceived brightness
		}

		private void ctlHueBackgroundForeground_BeforeBackgroundChanged(object sender, double e)
		{
			// TODO: Implement shift+Hue slider dragging to maintain perceived brightness
		}

		private void ctlHueBackgroundForeground_BeforeForegroundChanged(object sender, double e)
		{
			// TODO: Implement shift+Hue slider dragging to maintain perceived brightness
		}
	}
	
}