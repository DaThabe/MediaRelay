namespace MediaRelay.Source;

public static class SourceIdExtensions
{
    extension(SourceId)
    {
        public static SourceId TestId => SourceId.Create("Test");
    }
}