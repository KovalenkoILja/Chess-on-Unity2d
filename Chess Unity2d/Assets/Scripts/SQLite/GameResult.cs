// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable once CheckNamespace

namespace SQLiteTypes
{
    public class GameResult
    {
        [NotNull]
        [PrimaryKey]
        [AutoIncrement]
        [Unique]
        public int ResultId { get; set; }

        [NotNull]
        // ReSharper disable once InconsistentNaming
        public string PGN { get; set; }

        [NotNull] public int Score { get; set; }

        [NotNull] public float Playtime { get; set; }

        [NotNull] public float WhiteTime { get; set; }

        [NotNull] public float BlackTime { get; set; }

        [NotNull] public int EndGameStateId { get; set; }

        [NotNull] public int GameSetupId { get; set; }

        public override string ToString()
        {
            return $"[GameResult: Id={ResultId}," +
                   $"PGN={PGN}," +
                   $"Score={Score}," +
                   $"Playtime={Playtime}," +
                   $"WhiteTime={WhiteTime}" +
                   $"BlackTime={BlackTime}" +
                   $"BlackTime={BlackTime}" +
                   $"EndGameStateId={EndGameStateId}" +
                   $"GameSetupId={GameSetupId}]";
        }
    }
}