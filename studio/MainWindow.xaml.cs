using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace studio
{
    public partial class MainWindow : Window
    {
        private Point start = new Point(0, 0);
        private Point end = new Point(0, 0);
        private Color strokeColor = Colors.Black;
        private Color fillColor = Colors.Transparent;
        private int strokeThickness = 1;
        private string shapeType = "Line";

        public MainWindow()
        {
            InitializeComponent();

            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;

            strokeColorPicker.SelectedColorChanged += strokeColorPicker_SelectedColorChanged;
            fillColorPicker.SelectedColorChanged += fillColorPicker_SelectedColorChanged;
        }

        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton button)
            {
                shapeType = button.Tag.ToString();
                DisplayStatus();
            }
        }

        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                strokeColor = e.NewValue.Value;
                DisplayStatus();
            }
        }

        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                fillColor = e.NewValue.Value;
                DisplayStatus();
            }
        }

        private void DisplayStatus()
        {
            if (statusLabel != null)
                statusLabel.Content = $"Start:({start.X},{start.Y}) End:({end.X},{end.Y})";

            if (colorLabel != null)
                colorLabel.Content = $"筆刷色彩: {strokeColor} 填滿色彩: {fillColor} 筆刷粗細: {strokeThickness}";

            if (shapeLabel != null)
                shapeLabel.Content = $"形狀: {shapeType}";
        }

        private void strokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = (int)strokeThicknessSlider.Value;
            DisplayStatus();
        }
    }
}
