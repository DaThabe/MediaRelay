namespace MediaRelay.Resources;

public static class ResourceIdExtensions
{
    extension(ResourceId)
    {
        public static ResourceId TestId => ResourceId.Create("Test");
    }
}
