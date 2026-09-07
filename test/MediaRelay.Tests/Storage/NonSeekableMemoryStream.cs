namespace MediaRelay.Storage;

public sealed class NonSeekableMemoryStream(byte[] datas) : MemoryStream(datas)
{
    public override bool CanSeek => false;
}