﻿using System;
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


        List<Alien> aliens = new List<Alien>();
        List<Point> path = new List<Point>();
        SortedDictionary<char, Turret> turrets = new SortedDictionary<char, Turret>();


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

        }

        private void ComputeDamage()
        {
            foreach (var turret in turrets.Values)
            {
                foreach (var alien in aliens)
                {
                    if (alien.Health > 0)
                    {
                        var b = turret.IsInRange(alien.Location);
                        if (b) //shoot
                        {
                            for (int i = 0; i < turret.ShotFreq; i++)
                            {
                                alien.Health -= turret.Damage;
                            }

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
            for (int i = 0; i < Math.Min(turn, aliens.Count); i++)
            {
                var offset = turn - i - 1;

                if (offset >= path.Count)
                {
                    Debug.WriteLine("alien inside");
                    aliens.RemoveAt(i);
                }
                else
                {
                    aliens[i].Location = path[offset];
                }
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {

            aliens.Clear();
            path.Clear();

            int x = 0, y = 0;

            var turretsDefDict = turretsDef.Select(t => new Turret() { AttackRange = t.attackRange, Key = t.key, ShotFreq = t.shotFreq }).ToDictionary(k => k.Key);
            turrets = new SortedDictionary<char, Turret>(turretsDefDict);


            //build path

            path.Add(new Point() { X = x, Y = y });

            foreach (var (row, i) in battlefield.Select((v, i) => (v, i)))
            {
                foreach (var (field, j) in row.Select((c, j) => (c, j)))
                {
                    if (field == '1')
                    {
                        path.Add(new Point() { X = i, Y = j });
                    }
                    else
                    if (char.IsLetter(field))
                    {
                        turrets[field].Location = new Point() { X = i, Y = j };
                    }
                }

            }



            // init wave
            aliens = wave.Select(health => new Alien() { Health = health }).ToList();
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

            foreach (var (line, index) in battlefield.Select((line, index) => (line, index)))
            {
                RowDefinition rd = new RowDefinition();
                GameMap.RowDefinitions.Add(rd);

                for (int i = 0; i < columnCount; i++)
                {
                    Canvas c = new Canvas();
                    c.Background = i % 2 == 0 ? Brushes.Aquamarine : Brushes.Beige;


                    GameMap.Children.Add(c);
                    Grid.SetRow(c, index);
                    Grid.SetColumn(c, i);

                    //
                    Ellipse el = new Ellipse();
                    el.Fill = Brushes.Green;
                    //el.Width = 10;
                    // el.Height = 10;
                    //  el.Stretch = Stretch.Fill;
                    //  el.HorizontalAlignment = HorizontalAlignment.Center;
                    //  el.VerticalAlignment = VerticalAlignment.Center;

                    Binding widthBinding = new Binding("ActualWidth");
                    widthBinding.Source = c;
                    el.SetBinding(Ellipse.WidthProperty, widthBinding);

                    Binding heightBinding = new Binding("ActualHeight");
                    heightBinding.Source = c;
                    el.SetBinding(Ellipse.HeightProperty, heightBinding);

                    c.Children.Add(el);


                    Label lb = new Label();
                    lb.Content = (i * 2).ToString();
                    lb.HorizontalAlignment = HorizontalAlignment.Center;
                    lb.VerticalAlignment = VerticalAlignment.Top;
                    Grid.SetRow(lb, index);
                    Grid.SetColumn(lb, i);

                    GameMap.Children.Add(lb);


                }
            }


        }
    }
}
