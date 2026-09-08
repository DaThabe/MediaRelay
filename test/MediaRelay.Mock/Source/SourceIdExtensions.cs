namespace MediaRelay.Source;

public static class SourceIdExtensions
{
    extension(SourceId)
    {
        public static SourceId Test => SourceId.Create("Test");
    }
}