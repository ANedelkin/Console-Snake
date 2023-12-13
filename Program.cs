using System;
using System.Collections.Generic;
using System.Threading;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace Console_Snake
{
    public class Scores
    {
        static IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "Qmxkw4UVCR6xnNJ42ZqrVFsMZJitBukDF5oZodrG",
            BasePath = "https://scores-472cf-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        static IFirebaseClient client = new FireSharp.FirebaseClient(config);
        public static void Update(string n, int s)
        {
            if (n != null)
            {
                if (GetScores().ContainsKey(n) && GetScores()[n] < s) { var setter = client.Set("Scores/" + n, s); }
                else if (!GetScores().ContainsKey(n)) { client.Set("Scores/" + n, s); }
            }
        }
        public static Dictionary<string, int> GetScores()
        {
            FirebaseResponse resp = client.Get(@"Scores");
            Dictionary<string, int> scores = new Dictionary<string, int>();
            string[] parts = resp.Body.Split('\"');
            for (int i = 1; i < parts.Length - 1; i+=2)
            {
                scores.Add(parts[i], int.Parse(parts[i + 1].Remove(0, 1).Remove(parts[i + 1].Length - 2)));
            }
            return scores;
        }
    }
    class Program
    {
        static Random rng = new Random();
        static int w = 72, h = 20;
        static List<int[]> snake;
        static int[] apple = new int[2];
        static int[] pLast;
        static byte dir;
        static string name = null;
        static List<string> toshibas = new List<string>()
        { "azshibam", "tishibash", "toyshiba", "tyashiba", "tqshiba", "toshiba", "nieshibame", "vieshibate", "teshibat" };
        static void Main(string[] args)
        {
            Console.Title = "Console Snake";
            Console.SetWindowSize(w, h);
            Console.SetBufferSize(w, h);
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < w; i++)
            {
                Console.WriteLine();
            }
            while (true)
            {
                StartScreen();
            }
        }
        static void StartScreen()
        {
            Console.Clear();
            int buffer;
            int opt = 0;
            string[] logo = { "    ###### ##   #  ####  #   ## ######",
                              "    #      ###  # ##  ## # ###  #",
                              "    ###### # ## # #    # ###    ######",
                              "         # #  ### ###### # ###  #",
                              "########## #   ## #    # #   ## ######",};
            for (int i = 0; i < logo.Length; i++)
            {
                Console.SetCursorPosition(16, 3 + i);
                for (int j = 0; j < logo[i].Length; j++)
                {
                    if (logo[i][j] == '#') { Console.BackgroundColor = ConsoleColor.Green; }
                    else { Console.BackgroundColor = ConsoleColor.Black; }
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            while (true)
            {
                Console.SetWindowSize(w, h);
                Console.SetCursorPosition(0, 11);
                int yOpt1, yOpt2, yOpt3;
                string opt1, opt2, opt3;
                opt3 = "Scores list";
                yOpt3 = 15;
                if (name == null)
                {
                    opt1 = "Enter name";
                    opt2 = "Play as guest";
                    yOpt1 = 11; yOpt2 = 13;
                }
                else
                {
                    opt1 = $"Change name from \"{name}\"";
                    opt2 = "Play";
                    yOpt1 = 13; yOpt2 = 11;
                }
                buffer = (w - Math.Max(opt1.Length, Math.Max(opt3.Length, opt2.Length))) / 2 + 2;
                Console.SetCursorPosition(0, yOpt1);
                Console.WriteLine(new string(' ', buffer) + opt1);
                Console.SetCursorPosition(0, yOpt2);
                Console.WriteLine(new string(' ', buffer) + opt2);
                Console.SetCursorPosition(0, yOpt3);
                Console.WriteLine(new string(' ', buffer) + opt3);
                if (opt == 0)
                {
                    Console.SetCursorPosition(buffer - 2, 11);
                    Console.Write('>');
                    Console.SetCursorPosition(buffer - 2, 13);
                    Console.Write(' ');
                    Console.SetCursorPosition(buffer - 2, yOpt3);
                    Console.Write(' ');
                }
                else if (opt == 1)
                {
                    Console.SetCursorPosition(buffer - 2, 11);
                    Console.Write(' ');
                    Console.SetCursorPosition(buffer - 2, 13);
                    Console.Write('>');
                    Console.SetCursorPosition(buffer - 2, yOpt3);
                    Console.Write(' ');
                }
                else
                {
                    Console.SetCursorPosition(buffer - 2, 11);
                    Console.Write(' ');
                    Console.SetCursorPosition(buffer - 2, 13);
                    Console.Write(' ');
                    Console.SetCursorPosition(buffer - 2, yOpt3);
                    Console.Write('>');
                }
                int l = Math.Min(opt1.Length, Math.Min(opt2.Length, opt3.Length));
                Console.SetCursorPosition(opt1.Length + buffer, yOpt1);
                Console.Write(new string(' ', 72 - l));
                Console.SetCursorPosition(opt2.Length + buffer, yOpt2);
                Console.Write(new string(' ', 72 - l));
                Console.SetCursorPosition(opt3.Length + buffer, yOpt3);
                Console.Write(new string(' ', 72 - l));
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        opt--;
                        break;
                    case ConsoleKey.DownArrow:
                        opt++;
                        break;
                    case ConsoleKey.Enter:
                        if (11 + opt * 2 == yOpt1)
                        {
                            string pName = name;
                            for (int i = buffer - 1 + opt1.Length; i > buffer - 2; i--)
                            {
                                Console.SetCursorPosition(i, yOpt1);
                                Console.Write(' ');
                                Thread.Sleep(10);
                            }
                            Console.CursorVisible = true;
                            name = Console.ReadLine();
                            Console.CursorVisible = false;
                            bool animate = true;
                            if (name.Length > 15)
                            {
                                name = name.Remove(15, name.Length - 15) + "...";
                                Console.SetCursorPosition(buffer - 1 + name.Length, yOpt1);
                                Console.Write(new string(' ', 73 - name.Length - buffer));
                            }
                            opt1 = $"Change name from \"{name}\"";
                            if (pName == null)
                            {
                                opt2 = "Play            ";
                                opt = 0;
                                for (int i = buffer - 1 + Math.Max(name.Length, opt2.Length); i > buffer - 2 && animate; i--)
                                {
                                    Console.SetCursorPosition(i, yOpt1);
                                    Console.Write(' ');
                                    Console.SetCursorPosition(i, yOpt2);
                                    Console.Write(' ');
                                    Console.SetCursorPosition(i, yOpt3);
                                    Console.Write(' ');
                                    Thread.Sleep(10);
                                    if (Console.KeyAvailable) { animate = false; }
                                }
                                Console.SetCursorPosition(buffer - 2, 11);
                                Console.Write(' ');
                                yOpt1 = 13; yOpt2 = 11;
                                buffer = (w - opt1.Length) / 2 + 2;
                                Console.SetCursorPosition(buffer - 2, 11);
                                Console.Write('>');
                                for (int i = 0; i < Math.Max(opt1.Length, opt2.Length) && animate; i++)
                                {
                                    if (i < opt2.Length)
                                    {
                                        Console.SetCursorPosition(i + buffer, yOpt2);
                                        Console.Write(opt2[i]);
                                    }
                                    if (i < opt1.Length)
                                    {
                                        Console.SetCursorPosition(i + buffer, yOpt1);
                                        Console.Write(opt1[i]);
                                    }
                                    if (i < opt3.Length)
                                    {
                                        Console.SetCursorPosition(i + buffer, yOpt3);
                                        Console.Write(opt3[i]);
                                    }
                                    Thread.Sleep(10);
                                    if (Console.KeyAvailable) { animate = false; }
                                }
                            }
                            else
                            {
                                for (int i = buffer - 1 + name.Length; i > buffer - 2 && animate; i--)
                                {
                                    Console.SetCursorPosition(i, yOpt1);
                                    Console.Write(' ');
                                    Thread.Sleep(10);
                                    if (Console.KeyAvailable) { animate = false; }
                                }
                                for (int i = buffer; i < opt1.Length + buffer && animate; i++)
                                {
                                    Console.SetCursorPosition(i, yOpt1);
                                    Console.Write(opt1[i - buffer]);
                                    Thread.Sleep(10);
                                    if (Console.KeyAvailable) { animate = false; }
                                }
                            }
                        }
                        else if (11 + opt * 2 == yOpt2) { GamePlay(); break; }
                        else if (11 + opt * 2 == yOpt3) { ScoresScreen(); break; }
                        break;
                    default:
                        break;
                }
                if (opt < 0) { opt = 2; }
                opt = opt % 3;
            }
        }
        static void ScoresScreen()
        {
            Console.Clear();
            int opt = 1;
            bool order = false;
            Console.SetCursorPosition(17, 1);
            Console.WriteLine("Back" + new string(' ', 21) + "Score    " + "Name");
            Console.Write(new string('_', w));
            Console.SetCursorPosition(31, 8);
            Console.Write("Loading...");
            Console.SetCursorPosition(40, 1);
            Dictionary<string, int> scores = Scores.GetScores();
            List<string> scoreNames = new List<string>(scores.Keys);
            Console.SetCursorPosition(31, 8);
            Console.Write("          ");
            while (true)
            {
                bool pOrder = order;
                if (opt == 0)
                {
                    Console.SetCursorPosition(15, 1);
                    Console.Write('>');
                    Console.SetCursorPosition(40, 1);
                    Console.Write(' ');
                    Console.SetCursorPosition(49, 1);
                    Console.Write(' ');
                }
                else if (opt == 1)
                {
                    Console.SetCursorPosition(15, 1);
                    Console.Write(' ');
                    Console.SetCursorPosition(40, 1);
                    Console.Write('>');
                    Console.SetCursorPosition(49, 1);
                    Console.Write(' ');
                    SortByScore(scores, scoreNames);
                    order = true;
                }
                else if (opt == 2)
                {
                    Console.SetCursorPosition(15, 1);
                    Console.Write(' ');
                    Console.SetCursorPosition(40, 1);
                    Console.Write(' ');
                    Console.SetCursorPosition(49, 1);
                    Console.Write('>');
                    scoreNames.Sort();
                    order = false;
                }
                for (int i = 0; i < Math.Min(scoreNames.Count, 8) && opt != 0 && (order != pOrder); i += 1)
                {
                    Console.SetCursorPosition(17, 4 + i * 2);
                    Console.WriteLine(new string(' ', 38));
                    Thread.Sleep(50);
                    Console.SetCursorPosition(17, 4 + i * 2);
                    Console.Write(scoreNames[i] + new string(' ', 38 - scoreNames[i].Length - scores[scoreNames[i]].ToString().Length)
                                  + scores[scoreNames[i]]);
                }
                Console.SetCursorPosition(0, h - 1);
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow:
                        opt--;
                        break;
                    case ConsoleKey.RightArrow:
                        opt++;
                        break;
                    case ConsoleKey.Enter:
                        if (opt == 0) { StartScreen(); }
                        break;
                }
                if (opt < 0) { opt = 2; }
                opt = opt % 3;
            }
        }
        static void SortByScore(Dictionary<string, int> scores, List<string> scoreNames)
        {
            for (int i = 0; i < scores.Count; i++)
            {
                for (int j = i; j < scores.Count; j++)
                {
                    if (scores[scoreNames[i]] < scores[scoreNames[j]])
                    {
                        scoreNames.Insert(i, scoreNames[j]);
                        scoreNames.RemoveAt(j + 1);
                        i = -1;
                        break;
                    }
                }
            }
        }
        static void GamePlay()
        {
            dir = 4;
            snake = new List<int[]>()
            {
                new int[] { 4, 8 },
                new int[] { 4, 10 },
                new int[] { 4, 12 },
            };
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Green;
            Thread.Sleep(150);
            foreach (int[] item in snake)
            {
                Thread.Sleep(50);
                Console.SetCursorPosition(item[1], item[0]);
                Console.Write(' ');
                Thread.Sleep(50);
                Console.SetCursorPosition(item[1] + 1, item[0]);
                Console.Write(' ');
            }
            Console.BackgroundColor = ConsoleColor.Black;
            CreateApple();
            Console.SetCursorPosition(apple[1], apple[0]);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("  ");
            Console.BackgroundColor = ConsoleColor.Black;
            Score();
            while (true)
            {
                if (snake[snake.Count - 1][0] == apple[0] && snake[snake.Count - 1][1] == apple[1])
                {
                    UpdateSnake(true); CreateApple(); Score();
                    Console.SetCursorPosition(apple[1], apple[0]);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("  ");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                if (name != null && toshibas.Contains(name.ToLower()))
                {
                    Score();
                    Console.SetCursorPosition(apple[1], apple[0]);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("  ");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Thread.Sleep(100); Console.Clear(); Thread.Sleep(35);
                }
                else { Thread.Sleep(75); }
                CheckKeys();
                UpdateSnake(false);
                if (IsDead()) { break; }
            }
        }
        static void Score()
        {
            Console.SetCursorPosition(0, 1);
            Console.WriteLine(" Score: " + (snake.Count - 3));
            Console.WriteLine(new string('_', w));
        }
        static void UpdateSnake(bool elongate)
        {
            int i = snake.Count - 1;
            if (dir != 4)
            {
                if (!elongate)
                {
                    pLast = snake[0];
                    if (dir == 0)
                    {
                        snake.Add(new int[] { snake[i][0], snake[i][1] + 2 });
                    }
                    else if (dir == 1)
                    {
                        snake.Add(new int[] { snake[i][0], snake[i][1] - 2 });
                    }
                    else if (dir == 2)
                    {
                        snake.Add(new int[] { snake[i][0] - 1, snake[i][1] });
                    }
                    else if (dir == 3)
                    {
                        snake.Add(new int[] { snake[i][0] + 1, snake[i][1] });
                    }
                    if (snake[0][1] > snake[1][1])
                    {
                        Console.SetCursorPosition(snake[0][1] + 1, snake[0][0]);
                        Console.Write(' ');
                        Console.SetCursorPosition(snake[0][1], snake[0][0]);
                        Console.Write(' ');
                    }
                    else
                    {
                        Console.SetCursorPosition(snake[0][1], snake[0][0]);
                        Console.Write("  ");
                    }
                    snake.RemoveAt(0);
                }
                else
                {
                    snake.Insert(0, pLast);
                }
            }
            if (!IsDead())
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(snake[i][1], snake[i][0]);
                Console.Write("  ");
                if (toshibas.Contains(name))
                {
                    Console.SetCursorPosition(0, 4);
                    foreach (int[] item in snake)
                    {
                        Console.SetCursorPosition(item[1], item[0]);
                        Console.Write(' ');
                        Console.SetCursorPosition(item[1] + 1, item[0]);
                        Console.Write(' ');
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else { Die(); }

        }
        static void CheckKeys()
        {
            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        if (dir == 0) { UpdateSnake(false); }
                        if (dir != 1) { dir = 0; }
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        if (dir == 1) { UpdateSnake(false); }
                        if (dir != 0 && dir != 4) { dir = 1; }
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (dir == 2) { UpdateSnake(false); }
                        if (dir != 3) { dir = 2; }
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (dir == 3) { UpdateSnake(false); }
                        if (dir != 2) { dir = 3; }
                        break;
                    default:
                        break;
                }
            }
        }
        static void CreateApple()
        {
            while (true)
            {
                bool f = false;
                apple[0] = rng.Next(2, h / 2) * 2; apple[1] = rng.Next(0, (w - 1) / 2) * 2;
                for (int i = 0; i < snake.Count && !f; i++)
                {
                    f = (apple[0] == snake[i][0] && apple[1] == snake[i][1]);
                }
                if (!f) { break; }
            }
        }
        static bool IsDead()
        {
            int[] lastPart = snake[snake.Count - 1];
            bool hitItself = false;
            for (int i = 0; i < snake.Count - 1 && !hitItself; i++)
            {
                if (snake[i][0] == lastPart[0] && snake[i][1] == lastPart[1]) { hitItself = true; }
            }
            if (lastPart[0] < 3 || lastPart[0] >= h || lastPart[1] + 1 >= w || lastPart[1] < 0 || hitItself) { return true; }
            else { return false; }
        }
        static void Die()
        {
            Console.Clear();
            Score();
            int buffer;
            int opt = 0;
            string[] gameOver =
            {
                " ####   ####  ##  ## ######     ####  #    # ###### #####  ",
                "##     ##  ## ###### #         ##  ## #    # #      #   ## ",
                "#   ## #    # # ## # ######    #    # ##  ## ###### #####  ",
                "##   # ###### #    # #         ##  ##  ####  #      # ##   ",
                " ##### #    # #    # ######     ####    ##   ###### #  ### "
            };
            for (int i = 0; i < gameOver.Length; i++)
            {
                Console.SetCursorPosition(7, 6 + i);
                for (int j = 0; j < gameOver[i].Length; j++)
                {
                    if (gameOver[i][j] == '#') { Console.BackgroundColor = ConsoleColor.Green; }
                    else { Console.BackgroundColor = ConsoleColor.Black; }
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            while (true)
            {
                Console.SetWindowSize(w, h);
                Console.SetCursorPosition(0, 11);
                int yOpt1, yOpt2;
                string opt1, opt2;
                if (name == null)
                {
                    opt1 = "Enter name";
                    opt2 = "Continue";
                    yOpt1 = 14; yOpt2 = 16;
                }
                else
                {
                    opt1 = $"Change name from \"{name}\"";
                    opt2 = "Continue";
                    yOpt1 = 16; yOpt2 = 14;
                }
                buffer = (w - Math.Max(opt1.Length, opt2.Length)) / 2 + 1;
                Console.SetCursorPosition(0, yOpt1);
                Console.WriteLine(new string(' ', buffer) + opt1);
                Console.SetCursorPosition(0, yOpt2);
                Console.WriteLine(new string(' ', buffer) + opt2);
                if (opt == 0)
                {
                    Console.SetCursorPosition(buffer - 2, 14);
                    Console.Write('>');
                    Console.SetCursorPosition(buffer - 2, 16);
                    Console.Write(' ');
                }
                else
                {
                    Console.SetCursorPosition(buffer - 2, 14);
                    Console.Write(' ');
                    Console.SetCursorPosition(buffer - 2, 16);
                    Console.Write('>');
                }
                int l = Math.Min(opt1.Length, opt2.Length);
                Console.SetCursorPosition(opt1.Length + buffer, yOpt1);
                Console.Write(new string(' ', 72 - l));
                Console.SetCursorPosition(opt2.Length + buffer, yOpt2);
                Console.Write(new string(' ', 72 - l));
                Console.SetCursorPosition(buffer, 18);
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        opt--;
                        break;
                    case ConsoleKey.DownArrow:
                        opt++;
                        break;
                    case ConsoleKey.Enter:
                        if (14 + opt * 2 == yOpt1)
                        {
                            string pName = name;
                            for (int i = buffer - 1 + opt1.Length; i > buffer - 2; i--)
                            {
                                Console.SetCursorPosition(i, yOpt1);
                                Console.Write(' ');
                                Thread.Sleep(10);
                            }
                            Console.CursorVisible = true;
                            name = Console.ReadLine();
                            Console.CursorVisible = false;
                            bool animate = true;
                            if (name.Length > 15)
                            {
                                name = name.Remove(15, name.Length - 15) + "...";
                                Console.SetCursorPosition(buffer - 1 + name.Length, yOpt1);
                                Console.Write(new string(' ', 73 - name.Length - buffer));
                            }
                            opt1 = $"Change name from \"{name}\"";
                            if (pName == null)
                            {
                                opt2 = "Continue";
                                opt = 0;
                                for (int i = buffer - 1 + Math.Max(name.Length, opt2.Length); i > buffer - 2 && animate; i--)
                                {
                                    Console.SetCursorPosition(i, yOpt1);
                                    Console.Write(' ');
                                    Console.SetCursorPosition(i, yOpt2);
                                    Console.Write(' ');
                                    Thread.Sleep(10);
                                    if (Console.KeyAvailable) { animate = false; }
                                }
                                Console.SetCursorPosition(buffer - 2, 14);
                                Console.Write(' ');
                                yOpt1 = 16; yOpt2 = 14;
                                buffer = (w - opt1.Length) / 2 + 1;
                                Console.SetCursorPosition(buffer - 2, 14);
                                Console.Write('>');
                                for (int i = 0; i < Math.Max(opt1.Length, opt2.Length) && animate; i++)
                                {
                                    if (i < opt2.Length)
                                    {
                                        Console.SetCursorPosition(i + buffer, yOpt2);
                                        Console.Write(opt2[i]);
                                    }
                                    if (i < opt1.Length)
                                    {
                                        Console.SetCursorPosition(i + buffer, yOpt1);
                                        Console.Write(opt1[i]);
                                    }
                                    Thread.Sleep(10);
                                    if (Console.KeyAvailable) { animate = false; }
                                }
                            }
                            else
                            {
                                for (int i = buffer - 1 + name.Length; i > buffer - 2 && animate; i--)
                                {
                                    Console.SetCursorPosition(i, yOpt1);
                                    Console.Write(' ');
                                    Thread.Sleep(10);
                                    if (Console.KeyAvailable) { animate = false; }
                                }
                                for (int i = buffer; i < opt1.Length + buffer && animate; i++)
                                {
                                    Console.SetCursorPosition(i, yOpt1);
                                    Console.Write(opt1[i - buffer]);
                                    Thread.Sleep(10);
                                    if (Console.KeyAvailable) { animate = false; }
                                }
                            }
                        }
                        else if (14 + opt * 2 == yOpt2) { Scores.Update(name, snake.Count - 3); StartScreen(); break; }
                        break;
                    default:
                        break;
                }
                if (opt < 0) { opt = 1; }
                opt = opt % 2;
            }
        }
    }
}