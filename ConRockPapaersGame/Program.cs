using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RockPaperScissorsLizardSpock
{
    public enum Choice
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3,
        Lizard = 4,
        Spock = 5
    }

    public enum Opponent
    {
        None,
        Computer,
        Human,
    }

    class Rule
    {
        public Choice roundWinner;
        public Choice roundLoser;
        public string verb;

        public Rule(Choice roundWinner_in, string verb_in, Choice roundLoser_in)
        {
            roundWinner = roundWinner_in;
            verb = verb_in;
            roundLoser = roundLoser_in;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", roundWinner, verb, roundLoser);
        }
    }

    static class CompareMoves
    {
        private static Rule winningRule;
        private static bool? HasPlayerWon;

        public static Rule FindRulePlayer(HumanPlayer p, ComputerPlayer cpu)
        {
            return Game.Rules.FirstOrDefault(rule => rule.roundWinner == p.Move_Enum && rule.roundLoser == cpu.Move_Enum);
        }

        public static Rule FindRuleCPU(ComputerPlayer cpu, HumanPlayer p)
        {
            return Game.Rules.FirstOrDefault(rule => rule.roundWinner == cpu.Move_Enum && rule.roundLoser == p.Move_Enum);
        }

        public static Rule FindRule2P(HumanPlayer p1, HumanPlayer p2)
        {
            return Game.Rules.FirstOrDefault(rule => rule.roundWinner == p1.Move_Enum && rule.roundLoser == p2.Move_Enum);
        }

        public static void Compare(HumanPlayer p, ComputerPlayer cpu)
        {
            Rule rule1 = FindRulePlayer(p, cpu);
            Rule rule2 = FindRuleCPU(cpu, p);

            if (rule1 != null)
            {
                HasPlayerWon = true;
                winningRule = rule1;
                p.Score++;
            }

            else if (rule2 != null)
            {
                HasPlayerWon = false;
                winningRule = rule2;
                cpu.Score++;
            }

            else
            {
                HasPlayerWon = null;
            }
        }

        public static void Compare(HumanPlayer p1, HumanPlayer p2)
        {
            Rule rule1 = FindRule2P(p1, p2);
            Rule rule2 = FindRule2P(p2, p1);


            if (rule1 != null)
            {
                HasPlayerWon = true;
                winningRule = rule1;
                p1.Score++;
            }

            else if (rule2 != null)
            {
                HasPlayerWon = false;
                winningRule = rule2;
                p2.Score++;
            }

            else
            {
                HasPlayerWon = null;
            }
        }

        public static string WhoWonTheRound(HumanPlayer p, ComputerPlayer cpu)
        {
            string msg = string.Empty;

            if (HasPlayerWon == null)
            {
                msg = "\nTie.";
            }

            if (HasPlayerWon == true)
            {
                msg = string.Format("\n{0} wins this round. {1}", p.Name, winningRule);
            }

            if (HasPlayerWon == false)
            {
                msg = string.Format("\nComputer wins this round. {0}", winningRule);
            }

            return msg;
        }

        public static string WhoWonTheRound(HumanPlayer p1, HumanPlayer p2)
        {
            string msg = string.Empty;

            if (HasPlayerWon == null)
            {
                msg = "\nTie.";
            }

            if (HasPlayerWon == true)
            {
                msg = string.Format("\n{0} wins this round.{1}", p1.Name, winningRule);
            }

            if (HasPlayerWon == false)
            {
                msg = string.Format("\n{0} wins this round.{1}", p2.Name, winningRule);
            }

            return msg;
        }
    }

    class Player
    {
        public int Score { get; set; }
        public int Move_Int;
        public Choice Move_Enum;
    }

    class HumanPlayer : Player
    {
        public string Name { get; set; }
        public static void SetPlayerName(HumanPlayer p)
        {
            Console.Write("Enter name --> ");
            p.Name = Console.ReadLine();
        }

        public static void SetPlayerName(HumanPlayer p1, HumanPlayer p2)
        {
            Console.Write("Player 1 - Enter name --> ");
            p1.Name = Console.ReadLine();
            Console.Clear();
            Console.Write("Player 2 - Enter name --> ");
            p2.Name = Console.ReadLine();
        }

        public Choice PlayerMove(HumanPlayer p)
        {
            do
            {
                Console.Write("\n\n{0} - Rock [1], Paper [2], Scissors [3], Lizard [4], Spock? [5] --> ", p.Name);
                p.Move_Int = int.Parse(Console.ReadLine());
            } while (p.Move_Int != 1 && p.Move_Int != 2 && p.Move_Int != 3 && p.Move_Int != 4 && p.Move_Int != 5);

            p.Move_Enum = (Choice)p.Move_Int;

            return p.Move_Enum;
        }
    }

    class ComputerPlayer : Player
    {
        public Choice ComputerRandomMove(ComputerPlayer cpu)
        {
            Random random = new Random();
            cpu.Move_Int = random.Next(1, 6);

            cpu.Move_Enum = (Choice)cpu.Move_Int;

            return cpu.Move_Enum;
        }
    }

    class Display
    {
        public static void MainMenu()
        {
            Console.WriteLine("Welcome to the \"Rock Paper Scissors Lizard Spock\".\n");
            Thread.Sleep(500);
            Console.Write("Do you really want to play the game? [Y/N] --> ");
        }

        public static void Settings(HumanPlayer p)
        {
            Console.Clear();
            Game.winScore = Game.HowManyPoints(ref Game.scr);
            Console.Clear();
            Console.Write("Play against the computer or an another player? [C/H] --> ");
            Game.opponent = Game.ChooseOpponent(ref Game.opponentStr);
            Console.Clear();
        }

        public static void Board(HumanPlayer p, ComputerPlayer cpu)
        {
            Console.WriteLine("\n\t\t{0}: {1}\n\n\t\tComputer: {2}\n", p.Name, p.Score, cpu.Score);
        }

        public static void Board(HumanPlayer p1, HumanPlayer p2)
        {
            Console.WriteLine("\n\t\t{0}: {1}\n\n\t\t{2}: {3}\n", p1.Name, p1.Score, p2.Name, p2.Score);
        }

        public static void Rules()
        {
            Console.WriteLine("\n  Remember:\n");
            foreach (Rule item in Game.Rules)
            {
                Console.WriteLine(item);
            }
        }

        public static void HumanVsCPU(HumanPlayer p, ComputerPlayer cpu)
        {
            Display.Board(p, cpu);
            Display.Rules();
            p.PlayerMove(p);
            cpu.ComputerRandomMove(cpu);
            Console.Clear();
            CompareMoves.Compare(p, cpu);
            Display.ShowMoves(p, cpu);
            Display.ShowTheRoundWinner(p, cpu);
        }

        public static void HumanVsHuman(HumanPlayer p1, HumanPlayer p2)
        {
            Display.Board(p1, p2);
            Display.Rules();
            p1.PlayerMove(p1);
            Console.Clear();
            Display.Board(p1, p2);
            Display.Rules();
            p2.PlayerMove(p2);
            Console.Clear();
            CompareMoves.Compare(p1, p2);
            Display.ShowMoves(p1, p2);
            Display.ShowTheRoundWinner(p1, p2);
        }

        public static void ShowMoves(HumanPlayer p, ComputerPlayer cpu)
        {
            Console.WriteLine("{0} chose {1}.", p.Name, p.Move_Enum);
            Console.WriteLine("Computer chose {0}", cpu.Move_Enum);
        }

        public static void ShowMoves(HumanPlayer p1, HumanPlayer p2)
        {
            Console.WriteLine("{0} chose {1}.", p1.Name, p1.Move_Enum);
            Console.WriteLine("{0} chose {1}", p2.Name, p2.Move_Enum);
        }

        public static void ShowTheRoundWinner(HumanPlayer p, ComputerPlayer cpu)
        {
            string message = CompareMoves.WhoWonTheRound(p, cpu);
            Console.WriteLine(message);
        }

        public static void ShowTheRoundWinner(HumanPlayer p1, HumanPlayer p2)
        {
            string message = CompareMoves.WhoWonTheRound(p1, p2);
            Console.WriteLine(message);
        }

        public static void AskForReplay(HumanPlayer p1, HumanPlayer p2, ComputerPlayer cpu)
        {
            Console.Write("\nReplay? [Y/N) --> ");
            Game.StartGameOrNot(ref Game.startgame);
            Game.Initialize(p1, p2, cpu);
            Console.Clear();
        }
    }

    static class Game
    {
        public static string startgame;
        public static bool play;
        public const int MAX_SCORE = 50;
        public static int winScore;
        public static int scr;
        public static string opponentStr;
        public static Opponent opponent;

        public static List<Rule> Rules = new List<Rule>
        {
            new Rule(Choice.Scissors, "cuts", Choice.Paper),
            new Rule(Choice.Paper, "covers", Choice.Rock),
            new Rule(Choice.Rock, "crushes", Choice.Lizard),
            new Rule(Choice.Lizard, "poisons", Choice.Spock),
            new Rule(Choice.Spock, "smashes", Choice.Scissors),
            new Rule(Choice.Scissors, "decapitates", Choice.Lizard),
            new Rule(Choice.Lizard, "eats", Choice.Paper),
            new Rule(Choice.Paper, "disproves", Choice.Spock),
            new Rule(Choice.Spock, "vaporizes", Choice.Rock),
            new Rule(Choice.Rock, "crushes", Choice.Scissors),
        };

        public static bool StartGameOrNot(ref string startgame_in)
        {

            bool play_in = false;

            do
            {
                startgame_in = Console.ReadLine().ToUpper();

                if (startgame_in == "Y")
                {
                    play_in = true;
                }
                else if (startgame_in == "N")
                {
                    play_in = false;
                    Console.WriteLine("\nOkay then, goodbye.");
                    Environment.Exit(0);
                }
                else
                {
                    Console.Write("\nInvalid. Write \"Y\" or \"N\" --> ");
                }
            } while (startgame_in != "Y" && startgame_in != "N");

            return play_in;
        }

        public static int HowManyPoints(ref int winScore_in)
        {
            do
            {
                Console.Write("How many points? [1-{0}] --> ", Game.MAX_SCORE);
                winScore_in = int.Parse(Console.ReadLine());
            } while (winScore_in <= 0 || winScore_in > MAX_SCORE);

            return winScore_in;
        }

        public static Opponent ChooseOpponent(ref string opponentStr_in)
        {
            Opponent opponent_in = Opponent.None;
            do
            {
                opponentStr_in = Console.ReadLine().ToUpper();
            } while (opponentStr_in != "C" && opponentStr_in != "H");

            switch (opponentStr_in)
            {
                case "C":
                    opponent_in = Opponent.Computer;
                    break;

                case "H":
                    opponent_in = Opponent.Human;
                    break;
            }

            return opponent_in;
        }

        public static void Initialize(HumanPlayer p1, HumanPlayer p2, ComputerPlayer cpu)
        {
            p1.Score = 0;
            p2.Score = 0;
            cpu.Score = 0;
        }

        public static bool WhoWins(HumanPlayer p1, HumanPlayer p2, ComputerPlayer cpu)
        {
            if (p1.Score == winScore)
            {
                Console.WriteLine("\n{0} wins the game.", p1.Name);
                return true;
            }

            else if (p2.Score == winScore)
            {
                Console.WriteLine("\n{0} wins the game.", p2.Name);
                return true;
            }

            else if (cpu.Score == winScore)
            {
                Console.WriteLine("\nComputer wins the game.");
                return true;
            }

            return false;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {

            Display.MainMenu();
            Game.play = Game.StartGameOrNot(ref Game.startgame);

            HumanPlayer player1 = new HumanPlayer();
            HumanPlayer player2 = new HumanPlayer();
            ComputerPlayer computer = new ComputerPlayer();

            Display.Settings(player1);
            Game.Initialize(player1, player2, computer);

            switch (Game.opponent)
            {
                case Opponent.Computer:
                    HumanPlayer.SetPlayerName(player1);
                    break;

                case Opponent.Human:
                    HumanPlayer.SetPlayerName(player1, player2);
                    break;
            }

            Console.Clear();

            while (Game.play)
            {
                switch (Game.opponent)
                {
                    case Opponent.Computer:

                        Display.HumanVsCPU(player1, computer);
                        break;

                    case Opponent.Human:

                        Display.HumanVsHuman(player1, player2);
                        break;
                }

                if (Game.WhoWins(player1, player2, computer))
                {
                    Display.AskForReplay(player1, player2, computer);
                }
            }
        }
    }
}