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
using System.Windows.Threading;

namespace Fireball_Run
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(10000);
            jumpTimer.Interval = TimeSpan.FromSeconds(1.2);

            jumpTimer.Tick += JumpTimer_Tick;
            timer.Tick += Timer_Tick;
        }

        private const int ENEMY_START_POS = 610;
        private const int GAME_FIELD_HEIGHT = 90;

        private const int FIREBALL_WIDTH = 60;
        private const int FIREBALL_HEIGHT = 50;

        private const int CHARACTER_POS = 50;

        private int enemySpeed = 2;

        private int scores = 0;

        private List<Entity> enemies = new();

        private DispatcherTimer timer = new();
        private DispatcherTimer jumpTimer = new();

        private Random random = new();

        private Entity character = new()
        {
            Body = new Image
            {
                Source = new BitmapImage(new Uri("Images/BlueMage.png", UriKind.Relative)),
                Width = 60,
                Height = 80
            },
            Position = new()
            {
                X = CHARACTER_POS,
                Y = GAME_FIELD_HEIGHT
            }
        };

        private void StartNewGame()
        {
            GameField.Children.Add(new Image
            {
                Source = new BitmapImage(new Uri("Images/FireMage.png", UriKind.Relative)),
                Height = 150
            });

            Canvas.SetBottom(GameField.Children[0], GAME_FIELD_HEIGHT);
            Canvas.SetRight(GameField.Children[0], 20);

            scores = 0;
            enemies.Clear();
            GameField.Children.Add(character.Body);            

            Canvas.SetBottom(character.Body, GAME_FIELD_HEIGHT);
            Canvas.SetLeft(character.Body, CHARACTER_POS);

            SpawnEnemy();

            timer.Start();
        }
        private void StopGame()
        {
            GameField.Children.Clear();

            MenuPanel.Visibility = Visibility.Visible;

            timer.Stop();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (random.Next(0, 500) == 0 || enemies.Count == 0)
                SpawnEnemy();            

            foreach (var enemy in enemies.ToList())
            {
                MoveEnemy(enemy);
                CollisionCheck(enemy);
            }

            tbScores.Text = $"Score: {scores++}";
        }

        private void CollisionCheck(Entity enemy)
        {
            if (enemy.Position.X < character.Position.X + CHARACTER_POS && enemy.Position.X + 60 > character.Position.X
                && enemy.Position.Y == character.Position.Y)
            {
                StopGame();
            }
        }

        private void MoveEnemy(Entity enemy)
        {
            enemy.Position.X -= enemySpeed;
            Canvas.SetLeft(enemy.Body, enemy.Position.X);

            if (enemy.Position.X < -20)
                RemoveEnemy(enemy);
        }

        private void SpawnEnemy()
        {
            enemies.Add(new Entity
            {
                Body = new Image
                {
                    Source = new BitmapImage(new Uri("Images/fireball-pixel-art.png", UriKind.Relative)),
                    Width = FIREBALL_WIDTH,
                    Height = FIREBALL_HEIGHT
                },
                Position = new()
                {
                    X = ENEMY_START_POS,
                    Y = GAME_FIELD_HEIGHT
                }
            });

            GameField.Children.Add(enemies[^1].Body);

            Canvas.SetBottom(enemies[^1].Body, GAME_FIELD_HEIGHT);
            Canvas.SetLeft(enemies[^1].Body, ENEMY_START_POS);

        }
        private void RemoveEnemy(Entity enemy)
        {
            enemies.Remove(enemy);
            GameField.Children.Remove(enemy.Body);
        }

        private void SetCharacterHeight(int newHeight)
        {
            character.Position.Y = newHeight;
            Canvas.SetBottom(character.Body, newHeight);
        }

        private void Jump()
        {
            jumpTimer.Start();
            character.Position.Y += 70;
            Canvas.SetBottom(character.Body, character.Position.Y);
        }

        private void JumpTimer_Tick(object? sender, EventArgs e)
        {
            character.Position.Y = GAME_FIELD_HEIGHT;
            Canvas.SetBottom(character.Body, GAME_FIELD_HEIGHT);

            jumpTimer.Stop();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && character.Position.Y == GAME_FIELD_HEIGHT)
                Jump();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GameField.Children.Clear();
            StartNewGame();
            MenuPanel.Visibility = Visibility.Collapsed;
        }

        private void ExitImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
