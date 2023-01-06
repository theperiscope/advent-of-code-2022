using shared;

namespace day02
{
    /// <summary>
    /// Rock, paper, scissors
    /// 
    /// Rock beats Scissors and loses to Paper.
    /// Paper beats Rock and loses to Scissors.
    /// Scissors beats Paper and loses to Rock.
    /// Equal moves are a Draw.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Game value represents points
        /// </summary>
        private enum Game { Loss = 0, Draw = 3, Win = 6 }

        /// <summary>
        /// Move value represents points
        /// </summary>
        private enum Move { Rock = 1, Paper = 2, Scissors = 3 }

        private static void Main(string[] args)
        {
            if (args.Length == 0) {
                Console.WriteLine("Usage: {0} <file>", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
                return;
            }

            // optimization: file is read only once, and line-by-line to avoid storing all lines in memory
            var results = File.ReadLines(args[0])
                .Select(line => {
                    var part1 = ScorePart1(ToMove(line[0]), ToMove(line[2]));
                    var part2 = ScorePart2(ToMove(line[0]), ToGame(line[2]));
                    return (part1, part2);
                });

            var total1 = results.Select(x => x.part1.ourScore).Sum();
            var total2 = results.Select(x => x.part2.ourScore).Sum();

            Console.WriteLine("Total Part1: {0}", total1);
            Console.WriteLine("Total Part2: {0}", total2);
        }

        private static Move ToMove(char c)
        {
            return c switch {
                'A' or 'X' => Move.Rock,
                'B' or 'Y' => Move.Paper,
                'C' or 'Z' => Move.Scissors,
                _ => throw new NotSupportedException()
            };
        }

        private static Game ToGame(char c)
        {
            return c switch {
                'X' => Game.Loss,
                'Y' => Game.Draw,
                'Z' => Game.Win,
                _ => throw new NotSupportedException()
            };
        }

        /// <summary>
        /// The score for a single round is the score for the shape you selected (1 for Rock, 2 for Paper, and 3 for Scissors)
        /// plus the score for the outcome of the round (0 if you lost, 3 if the round was a draw, and 6 if you won)
        /// </summary>
        private static (int opponentScore, int ourScore) ScorePart1(Move opponentMove, Move ourMove)
        {
            var (g1, g2) = opponentMove == ourMove
                ? (Game.Draw, Game.Draw)
                : opponentMove switch {
                    Move.Rock => ourMove == Move.Paper ? (Game.Loss, Game.Win) : (Game.Win, Game.Loss),
                    Move.Paper => ourMove == Move.Scissors ? (Game.Loss, Game.Win) : (Game.Win, Game.Loss),
                    Move.Scissors => ourMove == Move.Rock ? (Game.Loss, Game.Win) : (Game.Win, Game.Loss),
                    _ => throw new NotSupportedException(),
                };

            return ((int)g1 + (int)opponentMove, (int)g2 + (int)ourMove);
        }

        private static (int opponentScore, int ourScore) ScorePart2(Move opponentMove, Game ourGameGoal)
        {
            var ourMove = ourGameGoal switch {
                Game.Draw => opponentMove,
                Game.Win =>
                    opponentMove switch {
                        Move.Rock => Move.Paper,
                        Move.Paper => Move.Scissors,
                        Move.Scissors => Move.Rock,
                        _ => throw new NotSupportedException()
                    },
                Game.Loss =>
                    opponentMove switch {
                        Move.Rock => Move.Scissors,
                        Move.Paper => Move.Rock,
                        Move.Scissors => Move.Paper,
                        _ => throw new NotSupportedException()
                    },
                _ => throw new NotSupportedException()
            };

            return ScorePart1(opponentMove, ourMove);
        }
    }
}