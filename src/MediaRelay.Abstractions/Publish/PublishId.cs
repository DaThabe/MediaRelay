namespace MediaRelay.Publish;


/// <summary>
/// 转发任务的唯一标识
/// </summary>
public readonly record struct PublishId
{
    public static PublishId Empty => default;

    private readonly Guid _value;
    private PublishId(Guid value) => _value = value;


    public static PublishId Create()
    {
        return new(Guid.CreateVersion7());
    }
    public static PublishId Create(Guid id)
    {
        return new(id);
    }

    public override string ToString() => _value.ToString("N");
}