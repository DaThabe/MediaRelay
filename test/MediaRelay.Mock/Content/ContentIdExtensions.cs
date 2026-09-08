namespace MediaRelay.Content;

public static class ContentIdExtensions
{
    extension(ContentId)
    {
        public static ContentId Test => ContentId.Create("Test");
    }
}