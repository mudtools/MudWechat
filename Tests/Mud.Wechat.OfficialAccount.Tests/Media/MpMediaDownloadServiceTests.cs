// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.DataModels.Media;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Media;

/// <summary>
/// 素材下载通道（<see cref="MpMediaDownloadService"/>）的 Content-Type 分支判错行为：
/// 文件流 / 视频 video_url / 业务错误 / 令牌失效自愈重试四分支（I3 裁决的落地验证）。
/// </summary>
public class MpMediaDownloadServiceTests
{
    /// <summary>D1：二进制响应（图片）→ 文件流结果，Content-Type / 文件名取自官方响应头。</summary>
    [Fact]
    public async Task DownloadAsync_ShouldReturnFileStream_WhenResponseIsBinary()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(new byte[] { 0x89, 0x50, 0x4E, 0x47 }),
        };
        response.Content.Headers.TryAddWithoutValidation("Content-Type", "image/jpeg");
        response.Content.Headers.TryAddWithoutValidation(
            "Content-Disposition", "attachment; filename=\"abc.jpg\"");

        using var harness = new Harness(response);
        using var result = await harness.Service.DownloadTemporaryMediaAsync("MEDIA_ID");

        result.VideoUrl.Should().BeNull("二进制形态无 video_url");
        result.ContentType.Should().Be("image/jpeg");
        result.FileName.Should().Be("abc.jpg", "文件名取自 Content-Disposition（官方携带时）");
        result.Content.Should().NotBeNull();
        var buffer = new byte[4];
        (await result.Content!.ReadAsync(buffer)).Should().Be(4);
        buffer.Should().Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 });
    }

    /// <summary>D2：官方未携带 Content-Disposition 时 FileName 为 null（SDK 不推断）。</summary>
    [Fact]
    public async Task DownloadAsync_ShouldNotInferFileName_WhenDispositionAbsent()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(new byte[] { 0x01 }),
        };
        response.Content.Headers.TryAddWithoutValidation("Content-Type", "audio/speex");

        using var harness = new Harness(response);
        using var result = await harness.Service.DownloadJssdkVoiceAsync("MEDIA_ID");

        result.ContentType.Should().Be("audio/speex");
        result.FileName.Should().BeNull();
    }

    /// <summary>D3：JSON 错误体（40007 invalid media_id）→ 抛 MpException，请求 URI 已交给脱敏基类。</summary>
    [Fact]
    public async Task DownloadAsync_ShouldThrowMpException_WhenJsonErrorBody()
    {
        var response = JsonResponse("""{"errcode":40007,"errmsg":"invalid media_id"}""");
        using var harness = new Harness(response);

        var act = () => harness.Service.DownloadTemporaryMediaAsync("BAD_ID");

        (await act.Should().ThrowAsync<MpException>())
            .Which.ErrorCode.Should().Be(40007);
        harness.TokenGetCalls.Should().Be(1, "非令牌失效码不重试");
    }

    /// <summary>D4：视频素材的官方 JSON 形态（video_url）→ VideoUrl 结果、无内容流。</summary>
    [Fact]
    public async Task DownloadAsync_ShouldReturnVideoUrl_WhenJsonCarriesVideoUrl()
    {
        var response = JsonResponse("""{"video_url":"DOWN_URL"}""");
        using var harness = new Harness(response);
        using var result = await harness.Service.DownloadTemporaryMediaAsync("VIDEO_ID");

        result.VideoUrl.Should().Be("DOWN_URL");
        result.Content.Should().BeNull("视频素材官方返回 JSON（video_url），不返回文件流");
        result.ContentType.Should().Be("application/json");
    }

    /// <summary>D5：令牌失效码（42001）→ 重试一次；重试成功则返回文件流（自愈链路）。</summary>
    [Fact]
    public async Task DownloadAsync_ShouldRetryOnce_WhenTokenInvalidErrcode()
    {
        var first = JsonResponse("""{"errcode":42001,"errmsg":"access_token expired"}""");
        var second = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(new byte[] { 0x41 }),
        };
        second.Content.Headers.TryAddWithoutValidation("Content-Type", "image/jpeg");

        using var harness = new Harness(first, second);
        using var result = await harness.Service.DownloadTemporaryMediaAsync("MEDIA_ID");

        result.Content.Should().NotBeNull("重试后拿到文件流");
        harness.TokenGetCalls.Should().Be(2, "令牌失效码 ⇒ 重新取令牌并重试一次");
        harness.SentRequests.Should().HaveCount(2);
        harness.SentRequests[0].Should().Contain("access_token=TOKEN-1");
        harness.SentRequests[1].Should().Contain("access_token=TOKEN-2", "重试请求必须携带重新获取的令牌");
    }

    /// <summary>D6：重试仍失败 → 抛业务异常（不对确定失效的场景空转）。</summary>
    [Fact]
    public async Task DownloadAsync_ShouldThrowAfterRetry_WhenRetryAlsoFails()
    {
        var first = JsonResponse("""{"errcode":40001,"errmsg":"invalid credential"}""");
        var second = JsonResponse("""{"errcode":40001,"errmsg":"invalid credential"}""");

        using var harness = new Harness(first, second);

        (await FluentActions.Awaiting(() => harness.Service.DownloadTemporaryMediaAsync("MEDIA_ID"))
                .Should().ThrowAsync<MpException>())
            .Which.ErrorCode.Should().Be(40001);
        harness.TokenGetCalls.Should().Be(2, "恰重试一次（首轮 + 重试），不得无限自愈");
    }

    /// <summary>D7：JSON 成功体但无 errcode / video_url（官方未定义形态）→ 防御性拒绝，不把错误体当流交出。</summary>
    [Fact]
    public async Task DownloadAsync_ShouldRejectUndefinedJsonShape()
    {
        var response = JsonResponse("""{"unexpected":"payload"}""");
        using var harness = new Harness(response);

        await FluentActions.Awaiting(() => harness.Service.DownloadTemporaryMediaAsync("MEDIA_ID"))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    /// <summary>D8：mediaId 空白 → 参数异常（fail-fast，不发起请求）。</summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task DownloadAsync_ShouldRejectBlankMediaId(string mediaId)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        using var harness = new Harness(response);

        await FluentActions.Awaiting(() => harness.Service.DownloadTemporaryMediaAsync(mediaId))
            .Should().ThrowAsync<ArgumentException>();
        harness.SentRequests.Should().BeEmpty();
    }

    /// <summary>D9：当前无应用上下文 → 明确错误（与生成客户端的上下文语义一致）。</summary>
    [Fact]
    public async Task DownloadAsync_ShouldThrow_WhenAppContextMissing()
    {
        var holder = new Mock<IAppContextHolder>();
        holder.SetupGet(h => h.Current).Returns((IMudAppContext?)null);
        var service = new MpMediaDownloadService(
            holder.Object,
            NullLogger<MpMediaDownloadService>.Instance);

        await FluentActions.Awaiting(() => service.DownloadTemporaryMediaAsync("MEDIA_ID"))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    /// <summary>D11：get_material 图文形态（news_item JSON）→ NewsItems 信封。</summary>
    [Fact]
    public async Task GetPermanentMaterialAsync_ShouldReturnNewsItems_WhenJsonCarriesNewsShape()
    {
        var response = JsonResponse(
            """{"news_item":[{"title":"TITLE","thumb_media_id":"THUMB","show_cover_pic":1,"author":"A","digest":"","content":"C","url":"U","content_source_url":"S"}]}""");
        using var harness = new Harness(response);
        using var result = await harness.Service.GetPermanentMaterialAsync("MEDIA_ID");

        result.NewsItems.Should().HaveCount(1);
        result.NewsItems![0].Title.Should().Be("TITLE");
        result.VideoDownUrl.Should().BeNull();
        result.Content.Should().BeNull();
        harness.SentRequests.Should().Contain(r => r.Contains("/cgi-bin/material/get_material"));
    }

    /// <summary>D12：get_material 视频形态（down_url JSON）→ VideoDownUrl 信封。</summary>
    [Fact]
    public async Task GetPermanentMaterialAsync_ShouldReturnVideoDownUrl_WhenJsonCarriesVideoShape()
    {
        var response = JsonResponse("""{"title":"T","description":"D","down_url":"DOWN_URL"}""");
        using var harness = new Harness(response);
        using var result = await harness.Service.GetPermanentMaterialAsync("MEDIA_ID");

        result.VideoDownUrl.Should().Be("DOWN_URL");
        result.VideoTitle.Should().Be("T");
        result.VideoDescription.Should().Be("D");
        result.NewsItems.Should().BeNull();
        result.Content.Should().BeNull();
    }

    /// <summary>D13：get_material 图片/语音形态（官方原文「响应的直接为素材的内容」）→ 文件流信封。</summary>
    [Fact]
    public async Task GetPermanentMaterialAsync_ShouldReturnStream_WhenResponseIsBinary()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(new byte[] { 0xFF, 0xD8 }),
        };
        response.Content.Headers.TryAddWithoutValidation("Content-Type", "image/jpeg");
        using var harness = new Harness(response);
        using var result = await harness.Service.GetPermanentMaterialAsync("MEDIA_ID");

        result.Content.Should().NotBeNull("图片/语音素材直接返回文件内容（官方原文）");
        result.NewsItems.Should().BeNull();
        result.VideoDownUrl.Should().BeNull();
    }

    /// <summary>D14：get_material 请求为 POST 且体为 {"media_id":…}（官方契约）。</summary>
    [Fact]
    public async Task GetPermanentMaterialAsync_ShouldPostMediaIdBody()
    {
        var response = JsonResponse("""{"news_item":[{"title":"T"}]}""");
        using var harness = new Harness(response);
        using var result = await harness.Service.GetPermanentMaterialAsync("MEDIA-ID-1");

        result.NewsItems.Should().NotBeNull();
        harness.SentRequests.Should().Contain(r => r.Contains("access_token=TOKEN-1"));
        harness.SentRequests.Should().Contain(r => r.Contains("media_id=MEDIA-ID-1"));
    }

    /// <summary>D15：get_material 的 40007 → MpException（与下载端点同一判错通路）。</summary>
    [Fact]
    public async Task GetPermanentMaterialAsync_ShouldThrowMpException_WhenJsonErrorBody()
    {
        var response = JsonResponse("""{"errcode":40007,"errmsg":"invalid media_id"}""");
        using var harness = new Harness(response);

        (await FluentActions.Awaiting(() => harness.Service.GetPermanentMaterialAsync("BAD"))
            .Should().ThrowAsync<MpException>())
            .Which.ErrorCode.Should().Be(40007);
    }

    /// <summary>D16：AOT 快车道——宿主序列化器实现 <see cref="IAotJsonContentSerializer"/> 时，
    /// 永久素材 JSON 走源生成 <c>JsonTypeInfo</c> 反序列化、请求体走 <c>ToHttpContent(T, JsonTypeInfo)</c>，
    /// <b>不落</b> <c>IHttpContentSerializer</c> 的 options 解析路径（AGENTS §3 AOT 红线）。</summary>
    [Fact]
    public async Task GetPermanentMaterialAsync_ShouldUseAotFastLane_WhenSerializerSupportsIt()
    {
        var response = JsonResponse("""{"news_item":[{"title":"T"}]}""");

        var serializer = new Mock<IHttpContentSerializer>();
        serializer.Setup(s => s.Deserialize<MpPermanentMaterialResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .Throws(new InvalidOperationException("下载通道不得落 options 解析路径（须走 IAotJsonContentSerializer 快车道）"));
        serializer.Setup(s => s.ToHttpContent(It.IsAny<MpMediaIdRequest>(), It.IsAny<object>()))
            .Throws(new InvalidOperationException("下载通道不得落 options 解析路径（须走 IAotJsonContentSerializer 快车道）"));

        var aot = serializer.As<IAotJsonContentSerializer>();
        aot.Setup(s => s.Deserialize(It.IsAny<string>(), It.IsAny<JsonTypeInfo<MpPermanentMaterialResponse>>()))
            .Returns(new MpPermanentMaterialResponse { NewsItems = new List<MpMaterialNewsItem> { new() { Title = "T" } } });
        aot.Setup(s => s.ToHttpContent(It.IsAny<MpMediaIdRequest>(), It.IsAny<JsonTypeInfo<MpMediaIdRequest>>()))
            .Returns(new StringContent("""{"media_id":"M"}""", System.Text.Encoding.UTF8, "application/json"));

        using var harness = new Harness(serializer.Object, response);
        using var result = await harness.Service.GetPermanentMaterialAsync("MEDIA-ID-1");

        result.NewsItems.Should().NotBeNull();
        result.NewsItems!.Should().ContainSingle();
        aot.Verify(s => s.Deserialize(It.IsAny<string>(), It.IsAny<JsonTypeInfo<MpPermanentMaterialResponse>>()),
            Times.Once, "永久素材 JSON 必须经源生成 JsonTypeInfo 反序列化");
        aot.Verify(s => s.ToHttpContent(It.IsAny<MpMediaIdRequest>(), It.IsAny<JsonTypeInfo<MpMediaIdRequest>>()),
            Times.Once, "get_material 请求体必须经源生成 JsonTypeInfo 序列化");
        serializer.Verify(s => s.Deserialize<MpPermanentMaterialResponse>(It.IsAny<string>(), It.IsAny<object>()),
            Times.Never, "反射/options 路径不得被调用");
    }

    /// <summary>D10：DI——下载服务随 AddMediaApi 注册为单例（ValidateScopes=true 下可解析且同实例）。</summary>
    [Fact]
    public void DownloadService_ShouldBeSingletonViaMediaModule()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(new List<MpAppConfig>
        {
            new() { AppKey = "mp-default", AppId = "wx-a", AppSecret = "s-a", IsDefault = true },
        });
        services.AddMpServices(builder => builder.AddMediaApi());

        using var provider = services.BuildServiceProvider(
            new ServiceProviderOptions { ValidateScopes = true });

        var first = provider.GetRequiredService<IMpMediaDownloadService>();
        var second = provider.GetRequiredService<IMpMediaDownloadService>();
        ReferenceEquals(first, second).Should().BeTrue("下载服务为无状态单例（随上下文逐调用解析应用）");
        first.Should().BeOfType<MpMediaDownloadService>();
    }

    /// <summary>测试夹具：伪应用上下文 + 令牌管理器 + 伪 HTTP 客户端（按序回放响应）。</summary>
    private sealed class Harness : IDisposable
    {
        private static readonly string[] Tokens = { "TOKEN-1", "TOKEN-2", "TOKEN-3", "TOKEN-4" };

        public Harness(params HttpResponseMessage[] responses)
            : this(null, responses)
        {
        }

        public Harness(IHttpContentSerializer? contentSerializer, params HttpResponseMessage[] responses)
        {
            var queue = new Queue<HttpResponseMessage>(responses);
            var httpClient = new Mock<IEnhancedHttpClient>();
            httpClient
                .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
                {
                    SentRequests.Add(request.RequestUri!.ToString());
                    return queue.Dequeue();
                });

            var tokenIndex = 0;
            var tokenManager = new Mock<IMpAccessTokenManager>();
            tokenManager
                .Setup(m => m.GetTokenAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken _) =>
                {
                    TokenGetCalls++;
                    return Task.FromResult(Tokens[Math.Min(tokenIndex++, Tokens.Length - 1)]);
                });

            var appContext = new Mock<IMpAppContext>();
            appContext.SetupGet(a => a.AppKey).Returns("mp-default");
            appContext.SetupGet(a => a.AccessTokenManager).Returns(tokenManager.Object);
            appContext.SetupGet(a => a.HttpClient).Returns(httpClient.Object);

            var holder = new Mock<IAppContextHolder>();
            holder.SetupGet(h => h.Current).Returns(appContext.Object);

            Service = new MpMediaDownloadService(
                holder.Object,
                NullLogger<MpMediaDownloadService>.Instance,
                contentSerializer);
        }

        public MpMediaDownloadService Service { get; }

        public List<string> SentRequests { get; } = new();

        /// <summary>取令牌次数（重试语义断言用：失效码恰重试一次）。</summary>
        public int TokenGetCalls { get; private set; }

        public void Dispose()
        {
        }
    }

    /// <summary>构造 application/json 形态的伪响应。</summary>
    private static HttpResponseMessage JsonResponse(string json)
        => new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
        };
}
