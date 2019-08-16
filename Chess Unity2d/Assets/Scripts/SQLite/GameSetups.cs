// ReSharper disable once CheckNamespace

namespace SQLiteTypes
{
    public class GameSetups
    {
        [NotNull]
        [PrimaryKey]
        [AutoIncrement]
        [Unique]
        // ReSharper disable once IdentifierTypo
        public int SetipId { get; set; }

        [NotNull] [Unique] public string Setup { get; set; }
    }
}