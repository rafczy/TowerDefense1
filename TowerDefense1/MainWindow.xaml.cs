using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TowerDefense1Logic;


//https://stackoverflow.com/questions/18296889/add-rows-columns-to-a-grid-dynamically
//https://www.codemag.com/Article/1008021/Centering-Text-on-a-WPF-Shape-Using-a-User-Control
//https://markheath.net/post/creating-resizable-shape-controls-in resizable ui controls


namespace TowerDefense1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] battlefield = {
                               "0111111",
                               "  A  B1",
                               " 111111",
                               " 1     ",
                               " 1C1111",
                               " 111 D1",
                               "      1"/**/ };
        int[] wave = { 30, 14, 27 };//, 21, 13, 0, 15, 17, 0, 18, 26 };



        (char key, int attackRange, int shotFreq)[] turretsDef = { ('A', 3, 2), ('B', 1, 4), ('C', 2, 2), ('D', 1, 3) };


        List<Alien> _aliens = new List<Alien>();
        List<Point> _path = new List<Point>();
        SortedDictionary<char, Turret> _turrets = new SortedDictionary<char, Turret>();


        int turn = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void nextTurn_Click(object sender, RoutedEventArgs e)
        {
            turn++;
            UpdateAlienLocation();
            ComputeDamage();
            UpdateUI();

        }

        private void UpdateUI()
        {

            GameMap.Children.Clear();

            foreach (var alien in _aliens)
            {

                DrawAlien((int)alien.Location.X, (int)alien.Location.Y, alien.Health);

            }

            foreach (var turret in _turrets.Values)
            {

                DrawTurret((int)turret.Location.X, (int)turret.Location.Y);
            }

        }


        //jam on
        private void ComputeDamage()
        {
            foreach (var turret in _turrets.Values)
            {
                foreach (var alien in _aliens)
                {
                    if (alien.Health > 0)
                    {
                        var b = turret.IsInRange(alien.Location);
                        if (b) //shoot
                        {
                            for (int i = 0; i < turret.ShotFreq; i++)
                            {
                                //  alien.Health -= turret.Damage;
                            }

                            //calculate angle
                            turret.Angle = turret.AngleBetween(alien.Location);

                            //DrawTurret((int)turret.Location.X, (int)turret.Location.Y);
                            break;
                        }
                    }
                    else
                    {

                        //remove alien?
                    }
                }
            }
        }

        private void UpdateAlienLocation()
        {
            for (int i = 0; i < Math.Min(turn, _aliens.Count); i++)
            {
                var offset = turn - i - 1;

                if (offset >= _path.Count)
                {
                    Debug.WriteLine("alien inside");
                    _aliens.RemoveAt(i);
                }
                else
                {
                    _aliens[i].Location = _path[offset];
                }
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {

            _aliens.Clear();
            _path.Clear();

            int x = 0, y = 0;

            var turretsDefDict = turretsDef.Select(t => new Turret() { AttackRange = t.attackRange, Key = t.key, ShotFreq = t.shotFreq }).ToDictionary(k => k.Key);
            _turrets = new SortedDictionary<char, Turret>(turretsDefDict);


            //build path

            _path.Add(new Point() { X = x, Y = y });

            foreach (var (row, i) in battlefield.Select((v, i) => (v, i)))
            {
                foreach (var (field, j) in row.Select((c, j) => (c, j)))
                {
                    if (field == '1')
                    {
                        _path.Add(new Point() { X = i, Y = j });
                    }
                    else
                    if (char.IsLetter(field))
                    {
                        _turrets[field].Location = new Point() { X = i, Y = j };
                    }
                }

            }



            // init wave
            _aliens = wave.Select(health => new Alien() { Health = health }).ToList();
            turn = 0;

            //reset  ui

            GameMap.ShowGridLines = true;
            GameMap.RowDefinitions.Clear();


            int columnCount = (battlefield.FirstOrDefault() ?? string.Empty).Length;

            for (int i = 0; i < columnCount; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                GameMap.ColumnDefinitions.Add(cd);
            }

            foreach (var line in battlefield)
            {
                RowDefinition rd = new RowDefinition();
                GameMap.RowDefinitions.Add(rd);
            }


            for (int i = 0; i < columnCount; i++)
            {
                //  DrawAlien(index, i);
                //   DrawTurret(index, i);
            }


            //foreach (var mapObject in _aliens.Concat<MapObject>(_turrets.Values))
            foreach (var alien in _aliens)
            {

                DrawAlien((int)alien.Location.X, (int)alien.Location.Y,alien.Health);

            }

            foreach (var turret in _turrets.Values)
            {

                DrawTurret((int)turret.Location.X, (int)turret.Location.Y);
            }


        }

        private void DrawTurret(int index, int i)
        {
            if (index < 0 || i < 0)
            {
                return;
            }

            var panel = new Canvas();
            //panel.ClipToBounds = true; //not needed
            panel.Width = 125;
            panel.Height = 125;

            Border b = new Border();
            b.BorderThickness = new Thickness(2);
            b.BorderBrush = Brushes.Black;
            b.Child = panel;

            Viewbox viewbox = new Viewbox();
            viewbox.Child = b;

            Polyline polyline1 = new Polyline();
            polyline1.Points.Add(new Point(25, 25));
            polyline1.Points.Add(new Point(0, 50));
            polyline1.Points.Add(new Point(25, 75));
            polyline1.Points.Add(new Point(50, 50));
            polyline1.Points.Add(new Point(25, 25));
            polyline1.Points.Add(new Point(25, 0));
            polyline1.Stroke = Brushes.Blue;
            polyline1.StrokeThickness = 10;
            // polyline1.Margin = new Thickness(25);
            polyline1.RenderTransform = new RotateTransform(45 * i);
            polyline1.RenderTransformOrigin = new Point(.5, .5); //centering the transform

            polyline1.HorizontalAlignment = HorizontalAlignment.Center;
            polyline1.VerticalAlignment = VerticalAlignment.Center;


            //centering on canvas
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Converter = new HalfValueConverter();
            multiBinding.Bindings.Add(new Binding("ActualWidth") { Source = panel });
            multiBinding.Bindings.Add(new Binding("ActualWidth") { Source = polyline1 });
            multiBinding.NotifyOnSourceUpdated = true;//this is important. 
            polyline1.SetBinding(Canvas.LeftProperty, multiBinding);

            MultiBinding multiBinding2 = new MultiBinding();
            multiBinding2.Converter = new HalfValueConverter();
            multiBinding2.Bindings.Add(new Binding("ActualHeight") { Source = panel });
            multiBinding2.Bindings.Add(new Binding("ActualHeight") { Source = polyline1 });
            multiBinding2.NotifyOnSourceUpdated = true;//this is important. 
            polyline1.SetBinding(Canvas.TopProperty, multiBinding2);

            panel.Children.Add(polyline1);

            GameMap.Children.Add(viewbox);
            Grid.SetRow(viewbox, index);
            Grid.SetColumn(viewbox, i);
        }

        private void DrawAlien(int index, int i, int health)
        {
            if (index < 0 || i < 0)
            {
                return;
            }

            //
            Ellipse el = new Ellipse();
            el.Fill = Brushes.Green;
            el.VerticalAlignment = VerticalAlignment.Stretch;
            el.HorizontalAlignment = HorizontalAlignment.Stretch;

            GameMap.Children.Add(el);
            Grid.SetRow(el, index);
            Grid.SetColumn(el, i);

            Label lb = new Label();
            lb.Content = health.ToString();
            lb.HorizontalAlignment = HorizontalAlignment.Center;
            lb.VerticalAlignment = VerticalAlignment.Top;

            GameMap.Children.Add(lb);
            Grid.SetRow(lb, index);
            Grid.SetColumn(lb, i);


            Ellipse eyeLeft = new Ellipse();
            eyeLeft.Fill = Brushes.Black;
            eyeLeft.VerticalAlignment = VerticalAlignment.Center;
            eyeLeft.HorizontalAlignment = HorizontalAlignment.Left;
            eyeLeft.Width = 30;
            eyeLeft.Height = 15;
            // eyeLeft.RenderTransform = new SkewTransform(0, 45);


            Binding widthBindingLeft = new Binding("ActualWidth");
            widthBindingLeft.Source = el;
            widthBindingLeft.Converter = new MarginConverter() { BaseThickness = new Thickness(.15, 0, 0, 0) };
            eyeLeft.SetBinding(Ellipse.MarginProperty, widthBindingLeft);


            GameMap.Children.Add(eyeLeft);
            Grid.SetRow(eyeLeft, index);
            Grid.SetColumn(eyeLeft, i);



            Ellipse eyeRight = new Ellipse();
            eyeRight.Fill = Brushes.Black;
            eyeRight.VerticalAlignment = VerticalAlignment.Center;
            eyeRight.HorizontalAlignment = HorizontalAlignment.Right;
            eyeRight.Width = 30;
            eyeRight.Height = 15;
            // eyeRight.RenderTransform = new SkewTransform(0, -45);



            Binding widthBindingRight = new Binding("ActualWidth");
            widthBindingRight.Source = el;
            widthBindingRight.Converter = new MarginConverter() { BaseThickness = new Thickness(0, 0, .15, 0) };
            eyeRight.SetBinding(Ellipse.MarginProperty, widthBindingRight);



            GameMap.Children.Add(eyeRight);
            Grid.SetRow(eyeRight, index);
            Grid.SetColumn(eyeRight, i);





            /*
                                Binding widthBinding = new Binding("ActualWidth");
                                widthBinding.Source = panel;
                                el.SetBinding(Ellipse.WidthProperty, widthBinding);

                                Binding heightBinding = new Binding("ActualHeight");
                                heightBinding.Source = panel;
                                el.SetBinding(Ellipse.HeightProperty, heightBinding);
                                */
        }
    }
}
