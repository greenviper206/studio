using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point start = new Point { X = 0, Y = 0 };
        Point end = new Point { X = 0, Y = 0 };
        Color strokeColor = Colors.Black;
        Color fillColor = Colors.Transparent;
        int strokeThickness = 1;
        string shapeType = "Line";
        string actionType = "Draw";
        public MainWindow()
        {
            InitializeComponent();
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
        }

        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = strokeColorPicker.SelectedColor.Value;
            DisplayStatus();
        }

        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = fillColorPicker.SelectedColor.Value;
            DisplayStatus();
        }

        private void DisplayStatus()
        {
            if (statusLabel != null)
            {
                if (actionType == "Draw")
                {
                    statusLabel.Content = $"圖形：{shapeType}   座標：({start.X},{start.Y}) - ({end.X},{end.Y})";
                }
                else
                {
                    statusLabel.Content = $"工作模式：{actionType}";
                }
            }
            if (colorLabel != null) colorLabel.Content = $"筆刷色彩: {strokeColor} 填滿色彩: {fillColor} 筆刷粗細: {strokeThickness}";
            if (shapeLabel != null)
            {
                int shapeCount = MyCanvas.Children.Count;
                shapeLabel.Content = $"目前形狀: {shapeType}，總共有{shapeCount}個形狀";
            }
        }

        private void ShapeButton_Checked(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;
            shapeType = targetRadioButton.Tag.ToString();
            actionType = "Draw";
            DisplayStatus();
        }

        private void strokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = (int)strokeThicknessSlider.Value;
            DisplayStatus();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "Clear";
            MyCanvas.Children.Clear();
            DisplayStatus();
            actionType = "Draw";
        }

        private void EraserButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "Eraser";
            DisplayStatus();
        }

        private void MyCanvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MyCanvas.Cursor = Cursors.Pen;
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyCanvas.Cursor = Cursors.Cross;
            start = e.GetPosition(MyCanvas);

            if (actionType == "Draw")
            {
                switch (shapeType)
                {
                    case "Line":
                        Line line = new Line
                        {
                            X1 = start.X,
                            Y1 = start.Y,
                            X2 = end.X,
                            Y2 = end.Y,
                            Stroke = Brushes.Gray,
                            StrokeThickness = 1
                        };
                        MyCanvas.Children.Add(line);
                        break;

                    case "Rectangle":
                        Rectangle rect = new Rectangle
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        MyCanvas.Children.Add(rect);
                        rect.SetValue(Canvas.LeftProperty, start.X);
                        rect.SetValue(Canvas.TopProperty, start.Y);
                        break;

                    case "Ellipse":
                        Ellipse ellipse = new Ellipse
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        MyCanvas.Children.Add(ellipse);
                        ellipse.SetValue(Canvas.LeftProperty, start.X);
                        ellipse.SetValue(Canvas.TopProperty, start.Y);
                        break;

                    case "Polyline":
                        var polyline = new Polyline
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray,
                        };
                        MyCanvas.Children.Add(polyline);
                        break;
                }
            }

            DisplayStatus();
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            end = e.GetPosition(MyCanvas);
            DisplayStatus();

            switch (actionType)
            {
                case "Draw": //繪圖模式
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point origin;
                        origin.X = Math.Min(start.X, end.X);
                        origin.Y = Math.Min(start.Y, end.Y);
                        double width = Math.Abs(end.X - start.X);
                        double height = Math.Abs(end.Y - start.Y);

                        switch (shapeType)
                        {
                            case "Line":
                                var line = MyCanvas.Children.OfType<Line>().LastOrDefault();
                                line.X2 = end.X;
                                line.Y2 = end.Y;
                                break;

                            case "Rectangle":
                                var rect = MyCanvas.Children.OfType<Rectangle>().LastOrDefault();
                                rect.Width = width;
                                rect.Height = height;
                                rect.SetValue(Canvas.LeftProperty, origin.X);
                                rect.SetValue(Canvas.TopProperty, origin.Y);
                                break;

                            case "Ellipse":
                                var ellipse = MyCanvas.Children.OfType<Ellipse>().LastOrDefault();
                                ellipse.Width = width;
                                ellipse.Height = height;
                                ellipse.SetValue(Canvas.LeftProperty, origin.X);
                                ellipse.SetValue(Canvas.TopProperty, origin.Y);
                                break;

                            case "Polyline":
                                var polyline = MyCanvas.Children.OfType<Polyline>().LastOrDefault();
                                polyline.Points.Add(end);
                                break;
                        }
                    }
                    break;

                case "Eraser": //橡皮擦模式
                    MyCanvas.Cursor = Cursors.Hand;
                    var shape = e.OriginalSource as Shape;
                    MyCanvas.Children.Remove(shape);
                    if (MyCanvas.Children.Count == 0) MyCanvas.Cursor = Cursors.Arrow;
                    break;
            }
            DisplayStatus();
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Brush strokeBrush = new SolidColorBrush(strokeColor);
            Brush fillBrush = new SolidColorBrush(fillColor);

            switch (shapeType)
            {
                case "Line":
                    var line = MyCanvas.Children.OfType<Line>().LastOrDefault();
                    if (line != null)
                    {
                        line.Stroke = strokeBrush;
                        line.StrokeThickness = strokeThickness;
                    }
                    break;

                case "Rectangle":
                    var rect = MyCanvas.Children.OfType<Rectangle>().LastOrDefault();
                    if (rect != null)
                    {
                        rect.Stroke = strokeBrush;
                        rect.Fill = fillBrush;
                        rect.StrokeThickness = strokeThickness;
                    }
                    break;

                case "Ellipse":
                    var ellipse = MyCanvas.Children.OfType<Ellipse>().LastOrDefault();
                    if (ellipse != null)
                    {
                        ellipse.Stroke = strokeBrush;
                        ellipse.Fill = fillBrush;
                        ellipse.StrokeThickness = strokeThickness;
                    }
                    break;

                case "Polyline":
                    var polyline = MyCanvas.Children.OfType<Polyline>().LastOrDefault();
                    if (polyline != null)
                    {
                        polyline.Stroke = strokeBrush;
                        polyline.Fill = fillBrush;
                        polyline.StrokeThickness = strokeThickness;
                    }
                    break;
            }
        }
        private void SaveCanvasButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Canvas as Image",
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|Canvas File(*.xml)|*.xml|所有檔案(*.*)|*.*",
                DefaultExt = "png"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                int w = Convert.ToInt32(MyCanvas.ActualWidth);
                int h = Convert.ToInt32(MyCanvas.ActualHeight);

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(w, h, 96d, 96d, PixelFormats.Pbgra32);
                renderBitmap.Render(MyCanvas);

                BitmapEncoder? encoder = null;
                string ext = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                switch (ext) 
                { 
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;
                    case ".xml":
                        //Canvas tempCanvas = new Canvas();

                        //var shapes = MyCanvas.Children.Cast<Shape>().ToList();

                        //foreach (var shape in shapes)
                        //{
                        //    tempCanvas.Children.Add(shape);
                        //}

                        string xmlString = XamlWriter.Save(MyCanvas);
                        File.WriteAllText(saveFileDialog.FileName, xmlString);
                        break;
                    default:
                        break;
                }

                if (encoder != null)
                {
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream outStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(outStream);
                    }
                    MessageBox.Show("File saved successfully!");
                }
            }
        }
        private void OpenCanvasButton_Click(object sender, RoutedEventArgs e) 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Canvas from File",
                Filter = "Canvas File(*.xml)|*.xml|所有檔案(*.*)|*.*",
                DefaultExt = "xml"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string canvasXaml = File.ReadAllText(filePath);

                Canvas tempCanvas = XamlReader.Parse(canvasXaml) as Canvas;

                var canvasChildren = tempCanvas.Children.Cast<Shape>().ToList();

                foreach (var child in canvasChildren)
                {
                    tempCanvas.Children.Remove(child);
                    MyCanvas.Children.Add(child);
                }
                MessageBox.Show("Canvas loaded successfully!");
            }
        }
    }
}