using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;


namespace SymbolMatchGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetUpCurrentSymbol();
            
        }

        private Brush SetGradient()
        {
            var rainbowFill = new GradientStopCollection
                {
                    new GradientStop(Colors.Red, 0.0),
                    new GradientStop(Colors.DarkOrange, 0.15),
                    new GradientStop(Colors.Yellow, 0.25),
                    new GradientStop(Colors.Green, 0.55),
                    new GradientStop(Colors.Blue, 0.75),
                    new GradientStop(Colors.Purple, 0.85),
                    new GradientStop(Colors.PaleGoldenrod, 1.0)
                };

            Brush b = new RadialGradientBrush(rainbowFill);
            return b;
        }


        //global variables
        private int _discardsLeft = 3;
        private int _score;
        private int _level = 1;
        private int _spacesCleared = 1;
       
        
        private void PlaceSymbol(Button name)
        {

            
            //check adjacent buttons for color and symbol
            //if placement is valid, change button color and symbol to currentSymbol
            if (IsValid(name))
            {
                RemoveHint();
                AddText(CurrentSymbol.Text, name);
                name.Foreground = CurrentSymbol.Foreground;
                var colorBrush = GetSolidColorBrush(name.Background);
                if (colorBrush!=null && colorBrush.Color != Colors.PaleGoldenrod)
                
                {
                    _spacesCleared++; //used to check if board is full
                    name.Background = new SolidColorBrush(Colors.PaleGoldenrod);
                }

                EnableAdjacents(name);

                if (colorBrush.Color != Colors.PaleGoldenrod)
                {
                    IncrementScore(10);
                }
                SetUpCurrentSymbol();

                if (_discardsLeft < 3)
                {
                    _discardsLeft++;
                    ShowDiscards();
                }

            }

            //check to see if entire row and/or column is filled
            if (RowFull(name))
            {
                ClearRow(name);
            }

            if (ColumnFull(name))
            {
                ClearColumn(name);
            }

            if (LevelComplete())
            {
                ClearBoard();
                IncrementLevel();
                _spacesCleared = 1;
                _discardsLeft = 3;
                ShowDiscards();
            }
            ShowDiscards();
        }

        private void ShowDiscards()
        {
            if (_discardsLeft == 3) DiscardsRemaining.Text = "SSS";
            else if (_discardsLeft == 2) DiscardsRemaining.Text = "SS";
            else if (_discardsLeft == 1) DiscardsRemaining.Text = "S";
            else if (_discardsLeft == 0) DiscardsRemaining.Text = "";
        }

        private void AddText(string s, Button b)
        {
            var textEffect = new TextBlock();
            textEffect.Text = s;
            textEffect.Width = 26;
            textEffect.TextAlignment = TextAlignment.Center;
            textEffect.VerticalAlignment = VerticalAlignment.Bottom;
            var d = new DropShadowEffect
                {
                    ShadowDepth = 4,
                    Direction = 330,
                    Color = Colors.Black,
                    Opacity = 0.8,
                    BlurRadius = 7
                };
            textEffect.Effect = d;
            b.Content = textEffect;
        }




        private bool LevelComplete()
        {
            if (_spacesCleared == 72)
            {
                MessageBox.Show("CONGRATULATIONS!!\nYou beat Level "+_level+ "! \n\nYour current score is "+_score+". Click OK for Level "+(_level+1)+".");
                return true;
            }
            return false;
        }

        private void ClearBoard()
        {
            foreach (Button button in GameGrid.Children)
            {
                button.Content = null;
                button.Background = new SolidColorBrush(Colors.LightGray);
                button.IsEnabled = false;
            }

            B32.Background = SetGradient();
            B23.IsEnabled = true;
            B31.IsEnabled = true;
            B32.IsEnabled = true;
            B33.IsEnabled = true;
            B41.IsEnabled = true;
        }

        private void ClearRow(Button name)
        {
            IncrementScore(100);
            _discardsLeft = 3;
            int row = Grid.GetRow(name);
            foreach (Button button in GameGrid.Children)
            {
                if (Grid.GetRow(button) == row)
                {
                    ResetButton(button);
                }
            }
        }

        private void ClearColumn(Button name)
        {
            IncrementScore(100);
            _discardsLeft = 3;
            int column = Grid.GetColumn(name);
            foreach (Button button in GameGrid.Children)
            {
                if (Grid.GetColumn(button) == column)
                {
                    ResetButton(button);
                }
            }
        }

        //shows the button has been cleared even if there's no content in it
        private void ResetButton(Button name)
        {
            name.Content = null;
            name.Background = new SolidColorBrush(Colors.PaleGoldenrod);
        }

        private static RadialGradientBrush GetRadialGradientBrush(Brush brush)
        {
            return brush as RadialGradientBrush;
        }

        private static SolidColorBrush GetSolidColorBrush(Brush brush)
        {
            return brush as SolidColorBrush;
        }
        
        private bool HasASymbol(Button name)
        {
            // Unable to cast object of type 'System.Windows.Media.RadialGradientBrush' to type 'System.Windows.Media.SolidColorBrush'.
            var colorBrush = GetSolidColorBrush(name.Background);
            var radialBrush = GetRadialGradientBrush(name.Background);
            if (colorBrush != null && colorBrush.Color == Colors.PaleGoldenrod && name.Content != null)
            {
                return true;
            }
            if (radialBrush != null)
            {
                return true;
            }

            return false;

        }

        private bool RowFull(Button name)
        {
            int rowFillCount = 0;
            int row = Grid.GetRow(name);
            foreach (Button button in GameGrid.Children)
            {
                if (Grid.GetRow(button) == row && HasASymbol(button))
                {
                    rowFillCount++;
                }
            }
            if (rowFillCount == 9)
            {
                return true;
            }
            return false;
        }

        private bool ColumnFull(Button name)
        {
            int columnFillCount = 0;
            int column = Grid.GetColumn(name);
            foreach (Button button in GameGrid.Children)
            {
                if (Grid.GetColumn(button) == column && HasASymbol(button))
                {
                    columnFillCount++;
                }
            }

            if (columnFillCount == 8)
            {
                return true;
            }
            return false;
        }

        bool IsValid(Button name)
        {
            IEnumerable<Button> al = GetAdjacents(name);
            var buttonsWithContent = new int();
            var freeButton = new int();
            
            //if button is already full
            if (HasASymbol(name)||GetRadialGradientBrush(name.Background)!=null)
            {
                return false;
            }

            foreach (Button button in al)
            {
                var tb = (TextBlock) button.Content;
                //if adjacent button has content
                if (!Equals(tb, null))
                {
                    //if there is even one adjacent button that doesn't match either in content or color, function should return false
                    if (!CurrentSymbol.Text.Equals(tb.Text) && 
                        !Equals(((SolidColorBrush)CurrentSymbol.Foreground).Color, ((SolidColorBrush)button.Foreground).Color))
                    {
                        return false;
                    }

                    buttonsWithContent++;   //incremented if adjacent button has matching symbol or color
                }

                //if adjacent button has rainbow inside ("free button")
                if (GetRadialGradientBrush(button.Background) != null)
                {
                    freeButton++;
                }
            }

            //at this point we know that any if any adjacent buttons have content, they match the current symbol, 
            //but we need to figure out if there is an adjacent button (we can only place a new symbol next to an existing one)
            //if both counts = 0, it is an invalid placement.

            if (buttonsWithContent == 0 && freeButton == 0)
            {
                return false;
            }

            //at this point we know there is one or more buttons with matching content and/or a free button in the adjacency list,
            //so the function should return true since it is a valid placement.
            return true;
        }

        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            RemoveHint();
            SetUpCurrentSymbol();
            _discardsLeft--;
            ShowDiscards();
            
            
            if (_discardsLeft <0)
            {
                CurrentSymbol.Text = null;
                if (MessageBox.Show("Your final score is " + _score + ".\n\n\nWould you like to play again?",
                "Game Over", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    Application curApp = Application.Current;
                    curApp.Shutdown();
                    
                }
                ClearBoard();
                _level = 1;
                _spacesCleared = 1;
                _discardsLeft = 3;
                _score = 0;
                ShowDiscards();
                SetUpCurrentSymbol();
                
                
            }

        }

        private IEnumerable<Button> GetAdjacents(Button name)
        {
            var adjacents = new List<Button>();
            int row = Grid.GetRow(name);
            int column = Grid.GetColumn(name);


            foreach (Button button in GameGrid.Children)
            {
                //find buttons to the right and left
                if (Grid.GetColumn(button) == column && (Grid.GetRow(button) == row - 1 || Grid.GetRow(button) == row + 1))
                {
                    adjacents.Add(button);
                }
                //enable buttons above and below
                if (Grid.GetRow(button) == row &&
                    (Grid.GetColumn(button) == column - 1 || Grid.GetColumn(button) == column + 1))
                {
                    adjacents.Add(button);
                }
            }
            return adjacents;

        }

        private void EnableAdjacents(Button name)
        {
            foreach (Button button in GetAdjacents(name))
            {
                button.IsEnabled = true;
            }

        }

        private void SetUpCurrentSymbol()
        {
            CurrentSymbol.Text = GetRandomSymbol();
            CurrentSymbol.Foreground = new SolidColorBrush(GetRandomColor());
        }

        private string GetRandomSymbol()
        {
            string[] symbols =
                {
                    "y", "s", "a", "c", "e", "p", "g", "h", "i", "j", "k", "m", "n", "o", 
                    "r", "t", "u", "v", "w"
                };

            int end = symbols.Length;

            if (_level <= 3) {end = 7;}    
            else if (_level == 4 || _level == 5) {end = 8;}
            else if (_level + 3 <= symbols.Length)
            {
                end = _level + 3;
            }

            int returnIndex = ri.Next(0, end);
            return symbols[returnIndex];
        }

        Random ri = new Random();
        Color[] colors = { Colors.Red, Colors.DarkOrange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Purple, Colors.DeepPink, Colors.Black };

        private Color GetRandomColor()
        {

            int end = colors.Length;

            if (_level <= 3)
            {
                end = 5;   
            }
            else if (_level + 2 <= colors.Length)
            {
                end = _level;
            }
            int returnIndex = ri.Next(0, end);

            return colors[returnIndex];

        }

        private void IncrementLevel()
        {
            _level++;
            Level.Content = _level;
        }

        private void IncrementScore(int a)
        {
            if (_level >= 4 && _level < 7)
            {
                a *= 2;
            }
            else if (_level >= 7)
            {
                a *= 3;
            }

            _score += a;
            Score.Content = _score;
        }
    
        private void Hint_Click(object sender, RoutedEventArgs e)
        {
           
            foreach (Button button in GameGrid.Children)
            {
                
                if (IsValid(button))
                {
                    button.Background = new SolidColorBrush(Colors.LightSkyBlue);
                }
            }
        }

        private void RemoveHint()
        {
            foreach (Button button in GameGrid.Children)
            {
                var colorBrush = GetSolidColorBrush(button.Background);
                if (colorBrush!=null && colorBrush.Color == Colors.LightSkyBlue)
                {
                    button.Background = new SolidColorBrush(Colors.LightGray);
                }
            }
        }











        //Button Event Handlers
        private void B1_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B1);

        }

        private void B2_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B2);
        }

        private void B3_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B3);
        }

        private void B4_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B4);
        }

        private void B5_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B5);
        }

        private void B6_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B6);
        }

        private void B7_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B7);
        }

        private void B8_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B8);
        }

        private void B9_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B9);
        }

        private void B10_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B10);
        }

        private void B11_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B11);
        }

        private void B12_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B12);
        }

        private void B13_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B13);
        }

        private void B14_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B14);
        }

        private void B15_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B15);
        }

        private void B16_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B16);
        }

        private void B17_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B17);
        }

        private void B18_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B18);
        }

        private void B19_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B19);
        }

        private void B20_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B20);
        }

        private void B21_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B21);
        }

        private void B22_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B22);
        }

        private void B23_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B23);
        }

        private void B24_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B24);
        }

        private void B25_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B25);
        }

        private void B26_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B26);
        }

        private void B27_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B27);
        }

        private void B28_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B28);
        }

        private void B29_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B29);
        }

        private void B30_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B30);
        }

        private void B31_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B31);
        }

        private void B32_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B32);
        }

        private void B33_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B33);
        }

        private void B34_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B34);
        }

        private void B35_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B35);
        }

        private void B36_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B36);
        }

        private void B37_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B37);
        }

        private void B38_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B38);
        }

        private void B39_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B39);
        }

        private void B40_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B40);
        }

        private void B41_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B41);
        }

        private void B42_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B42);
        }

        private void B43_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B43);
        }

        private void B44_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B44);
        }

        private void B45_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B45);
        }

        private void B46_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B46);
        }

        private void B47_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B47);
        }

        private void B48_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B48);
        }

        private void B49_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B49);
        }

        private void B50_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B50);
        }

        private void B51_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B51);
        }

        private void B52_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B52);
        }

        private void B53_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B53);
        }

        private void B54_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B54);
        }

        private void B55_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B55);
        }

        private void B56_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B56);
        }

        private void B57_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B57);
        }

        private void B58_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B58);
        }

        private void B59_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B59);
        }

        private void B60_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B60);
        }

        private void B61_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B61);
        }

        private void B62_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B62);
        }

        private void B63_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B63);
        }

        private void B64_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B64);
        }

        private void B65_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B65);
        }

        private void B66_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B66);
        }

        private void B67_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B67);
        }

        private void B68_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B68);
        }

        private void B69_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B69);
        }

        private void B70_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B70);
        }

        private void B71_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B71);
        }

        private void B72_Click(object sender, RoutedEventArgs e)
        {
            PlaceSymbol(B72);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "HOW TO PLAY:\n\nThe object of this game is to clear the board by matching symbols. A tile may be placed on the board by clicking a square. \n\nA placement is valid if adjacent tiles have the same color or the same symbol. Any tile may be placed next to the center rainbow tile.\n\nA row or column will clear if every square in it has had a tile placed. The board will clear when every square has had a tile placed at least once.\n\nIf you need help finding a valid placement, click on the HINT BUTTON. If there are no valid placements, you may click on the DISCARD button to get a new symbol. You will have the opportunity to use at least three discards per level. You may replenish used discards by placing tiles or clearing rows and columns.\n\nClick OK to play.",
                "WELCOME TO SYMBOL MATCH");
        }

    }
}
