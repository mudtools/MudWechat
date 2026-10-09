// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using System.Text;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.Fapiao;

namespace Mud.Wechat.Pay.Tests.Fapiao;

/// <summary>
/// 电子发票文件通道（<see cref="WechatPayFapiaoFileService"/>）的输入闸、表单形态与失败分类测试。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组锁定的三件事</b>：
/// ① 输入非法时<b>在发送之前</b> fail-fast（与账单下载同款纪律：不要把错误交给网络层）；
/// ② <c>multipart</c> 的表单字段名与 <c>meta</c> 的 JSON 字段名<b>照官方原文</b>
/// （尤其是官方拼写 <c>digest_alogrithm</c> —— 少一个 r，写对了才算照录）；
/// ③ 2xx 但拿不到 <c>fapiao_media_id</c> 这种「看起来成功却不可用」的应答必须显式失败。
/// </para>
/// </remarks>
public class WechatPayFapiaoFileServiceTests
{
    /// <summary>文件名空白 ⇒ 参数错误（不得发出一个官方必然判错的请求）。</summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Upload_ShouldReject_WhenFileNameBlank(string fileName)
    {
        var sut = CreateSut();

        var act = async () => await sut.UploadFapiaoFileAsync(new MemoryStream(new byte[] { 1 }), fileName, "abc");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    /// <summary>
    /// 摘要空白 ⇒ 参数错误，且消息须点明「SM3 不在 BCL」这一真因
    /// （否则调用方只会看到「摘要不能为空」而不知该从哪来）。
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Upload_ShouldReject_WhenDigestBlank(string digest)
    {
        var sut = CreateSut();

        var act = async () => await sut.UploadFapiaoFileAsync(new MemoryStream(new byte[] { 1 }), "a.pdf", digest);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*SM3*");
    }

    /// <summary>内容流为 null ⇒ <see cref="ArgumentNullException"/>。</summary>
    [Fact]
    public async Task Upload_ShouldReject_WhenContentNull()
    {
        var sut = CreateSut();

        var act = async () => await sut.UploadFapiaoFileAsync(null!, "a.pdf", "abc");

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>输入非法时不得触达 HTTP 客户端（逐条证明「拒绝发生在发送之前」）。</summary>
    [Fact]
    public async Task Upload_ShouldNotSendRequest_WhenInputRejected()
    {
        var client = new Mock<IWechatPayHttpClient>(MockBehavior.Strict);
        var sut = new WechatPayFapiaoFileService(client.Object, NullLogger<WechatPayFapiaoFileService>.Instance);

        var act = async () => await sut.UploadFapiaoFileAsync(new MemoryStream(new byte[] { 1 }), "a.pdf", " ");

        await act.Should().ThrowAsync<ArgumentException>();
        client.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <b>表单形态锁定</b>：<c>multipart/form-data</c>，两部分名为 <c>file</c> / <c>meta</c>，
    /// 且 <c>meta</c> 里的字段名照官方原文（含 <c>digest_alogrithm</c> 这一处官方拼写）。
    /// </summary>
    [Fact]
    public async Task Upload_ShouldSendOfficialMultipartShape()
    {
        HttpMethod? method = null;
        string? uri = null;
        string? contentTypeName = null;
        string? formText = null;

        var client = new Mock<IWechatPayHttpClient>(MockBehavior.Loose);
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            // ⚠️ **必须在回调里即时读取请求内容**：被测实现用 using 释放了 request / content，
            // 若只存下 HttpRequestMessage 引用、事后再读，会读到已释放的内容而假红
            // （本轮实测踩到：表单断言全部落空）。守卫/测试的可观测点要在**生命周期内**取。
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                method = request.Method;
                uri = request.RequestUri?.ToString();
                contentTypeName = request.Content?.GetType().Name;
                formText = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"fapiao_media_id\":\"MEDIA-1\"}", Encoding.UTF8, "application/json"),
            });

        var sut = new WechatPayFapiaoFileService(client.Object, NullLogger<WechatPayFapiaoFileService>.Instance);

        var result = await sut.UploadFapiaoFileAsync(
            new MemoryStream(Encoding.UTF8.GetBytes("pdf-bytes")), "invoice.pdf", "aa11bb22");

        result.FapiaoMediaId.Should().Be("MEDIA-1");

        method.Should().Be(HttpMethod.Post);
        uri.Should().Be(
            "https://api.mch.weixin.qq.com/v3/new-tax-control-fapiao/fapiao-applications/upload-fapiao-file",
            "上传走**主接入点**（与发票下载的 pay.wechatpay.cn 文件域名不同）");
        contentTypeName.Should().Be(nameof(MultipartFormDataContent), "官方契约要求 multipart/form-data");

        formText.Should().NotBeNull();

        // ⚠️ 表单段名用**正则容许带/不带引号**：.NET 的 MultipartFormDataContent 写出的是
        // 无引号形态（name=file），而 RFC 与部分实现用带引号形态（name="file"）——
        // 锁「段名到底是什么」即可，不必把 .NET 当前的引号风格一起锁死（否则升级 .NET 会假红）。
        Regex.IsMatch(formText!, "name=\"?file\"?").Should().BeTrue("官方表单字段名就是 file");
        Regex.IsMatch(formText!, "name=\"?meta\"?").Should().BeTrue("官方表单字段名就是 meta");
        formText.Should().Contain(WechatPayFapiaoFileService.DigestAlgorithmFieldName,
            "⚠️ 官方拼写少一个 r（digest_alogrithm）—— 纠正即错");
        formText.Should().Contain(WechatPayFapiaoFileService.Sm3DigestAlgorithm, "官方该字段唯一可选值");
        formText.Should().Contain(WechatPayFapiaoFileService.PdfFileType, "官方 file_type 该字段表给出的唯一可选值");
        formText.Should().Contain("aa11bb22", "调用方传入的摘要必须原样提交");
    }

    /// <summary>
    /// <b>2xx 但没有 <c>fapiao_media_id</c></b> ⇒ 必须显式失败：
    /// 这是「看起来成功却不可用」的形态（返回空壳会让调用方拿着 null 去插卡）。
    /// </summary>
    [Fact]
    public async Task Upload_ShouldFail_WhenResponseLacksMediaId()
    {
        var client = new Mock<IWechatPayHttpClient>(MockBehavior.Loose);
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json"),
            });

        var sut = new WechatPayFapiaoFileService(client.Object, NullLogger<WechatPayFapiaoFileService>.Instance);

        var act = async () => await sut.UploadFapiaoFileAsync(
            new MemoryStream(new byte[] { 1 }), "a.pdf", "aa");

        await act.Should().ThrowAsync<WechatPayException>()
            .WithMessage("*fapiao_media_id*");
    }

    /// <summary>官方非 2xx 且带 JSON 错误体 ⇒ 按 <c>code</c> 抛业务异常（与其余支付线的判错面一致）。</summary>
    [Fact]
    public async Task Upload_ShouldThrowBusinessException_WhenOfficialReturnsError()
    {
        var client = new Mock<IWechatPayHttpClient>(MockBehavior.Loose);
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(
                    "{\"code\":\"PARAM_ERROR\",\"message\":\"文件过大\"}", Encoding.UTF8, "application/json"),
            });

        var sut = new WechatPayFapiaoFileService(client.Object, NullLogger<WechatPayFapiaoFileService>.Instance);

        var act = async () => await sut.UploadFapiaoFileAsync(
            new MemoryStream(new byte[] { 1 }), "a.pdf", "aa");

        (await act.Should().ThrowAsync<WechatPayException>())
            .Which.PayErrorCode.Should().Be("PARAM_ERROR", "官方错误体里的 code 必须被保留（判错面不得降级为裸消息）");
    }

    /// <summary>
    /// <b>常量与裁决锁定</b>：下载域名 <c>pay.wechatpay.cn</c> 被显式留档，
    /// 而它<b>不在</b>进程级白名单内 —— 这条断言让「下载为何不在此实现」成为可校验的事实。
    /// </summary>
    [Fact]
    public void FileChannel_ShouldDocumentDownloadRuling()
    {
        WechatPayFapiaoFileService.FileDownloadHost.Should().Be("pay.wechatpay.cn",
            "官方《下载发票文件》示例 URL 的主机");

        WechatPayFapiaoFileService.PrimaryAccessPoint.Should().Be("https://api.mch.weixin.qq.com");
        WechatPayFapiaoFileService.UploadPath.Should().Be(
            "/v3/new-tax-control-fapiao/fapiao-applications/upload-fapiao-file");
        WechatPayFapiaoFileService.FileFieldName.Should().Be("file");
        WechatPayFapiaoFileService.MetaFieldName.Should().Be("meta");
        WechatPayFapiaoFileService.PdfFileType.Should().Be("PDF");
        WechatPayFapiaoFileService.Sm3DigestAlgorithm.Should().Be("SM3");

        WechatApiHosts.AllowedBaseUrlDomains.Should().NotContain(WechatPayFapiaoFileService.FileDownloadHost,
            "发票下载域名不在进程级白名单内（白名单被 MP-X8 锁定为两条新线零改动）⇒ "
            + "SDK 内请求该主机会被 URL 校验器拒绝，须由宿主显式决策后才可放开");
    }

    private static WechatPayFapiaoFileService CreateSut()
        => new(new Mock<IWechatPayHttpClient>(MockBehavior.Loose).Object,
               NullLogger<WechatPayFapiaoFileService>.Instance);
}
