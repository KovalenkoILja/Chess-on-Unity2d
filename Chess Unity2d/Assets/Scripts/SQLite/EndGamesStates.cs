// ReSharper disable once CheckNamespace

namespace SQLiteTypes
{
    public class EndGamesStates
    {
        [NotNull]
        [PrimaryKey]
        [AutoIncrement]
        [Unique]
        public int EndGameStateId { get; set; }

        [NotNull] [Unique] public string EndGameState { get; set; }
    }
}