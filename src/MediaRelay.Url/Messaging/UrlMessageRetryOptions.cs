namespace MediaRelay.Url.Messaging;

internal sealed record class UrlMessageRetryOptions
{
    public int CurrentCount { get; private set; }
    public required int MaxCount { get; init; }


    public static UrlMessageRetryOptions FromCount(int maxCount, int initCount = 0)
    {
        return new UrlMessageRetryOptions() { MaxCount = maxCount, CurrentCount = initCount };
    }


    public void Increment()
    {
        if (CurrentCount < MaxCount)
        {
            CurrentCount++;
            return;
        }

        throw new InvalidOperationException("已到最大重试次数");
    }

    public void Recover()
    {
        CurrentCount = 0;
    }

    public override string ToString() => $"{CurrentCount}/{MaxCount}";
}