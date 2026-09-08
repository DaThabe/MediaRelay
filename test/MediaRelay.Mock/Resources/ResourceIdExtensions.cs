namespace MediaRelay.Resources;

public static class ResourceIdExtensions
{
    extension(ResourceId)
    {
        public static ResourceId Test => ResourceId.Create("Test");
    }
}
