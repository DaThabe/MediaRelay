using Apigen.Immich.Client;

namespace MediaRelay.Immich;


[TestClass]
public sealed class ImmichApiClientTest
{
    [TestMethod]
    public async Task SendPixivArtworksRequestAsync()
    {
        var api = ImmichApiClient.WithApiKey("cKN6aX28ZHdl7D6gi0aOefwfBrTUfBU5DmDMgvObw", "http://localhost:2283/api/");

        var my = await api.Users.GetMyUserAsync(TestContext.CancellationToken);

        var all = await api.Albums.GetAllAlbumsAsync(cancellationToken: TestContext.CancellationToken);
        all.FirstOrDefault(x => x.AlbumName == "PixivUser@123456");

        await api.Albums.CreateAsync(new()
        {
            AlbumName = "Test",
            Description = "这是一个描述"
        });

        

        System.Console.WriteLine(my);
    }

    public TestContext TestContext { get; set; }
}
