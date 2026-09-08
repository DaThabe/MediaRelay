namespace MediaRelay.Publish;

[Obsolete]
public readonly record struct PublishJobId
{
    public static PublishId Empty => default;

    private readonly Guid _value;
    private PublishJobId(Guid value) => _value = value;


    public static PublishJobId Create()
    {
        return new(Guid.CreateVersion7());
    }
    public static PublishJobId Create(Guid id)
    {
        return new(id);
    }

    public override string ToString() => _value.ToString("N");
}