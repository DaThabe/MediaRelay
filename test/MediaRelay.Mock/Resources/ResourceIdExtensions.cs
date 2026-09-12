namespace MediaRelay.Resource;

public static class ResourceIdExtensions
{
    extension(ResourceId)
    {
        public static ResourceId TestId => ResourceId.Create("Test");
    }
}
