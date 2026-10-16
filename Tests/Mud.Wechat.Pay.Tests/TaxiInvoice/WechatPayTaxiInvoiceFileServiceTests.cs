// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using System.Text;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.TaxiInvoice;

namespace Mud.Wechat.Pay.Tests.TaxiInvoice;

/// <summary>
/// 出租车电子发票文件通道（<see cref="WechatPayTaxiInvoiceFileService"/>）的输入闸、表单形态与失败分类测试。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组锁定的三件事</b>：
/// ① 输入非法时<b>在发送之前</b> fail-fast（与电子发票文件通道同款纪律：不把错误交给网络层）；
/// ② <c>meta</c> 的四个字段名与取值<b>照本页官方原文</b> —— 尤其是
/// <c>digest_algorithm</c>（本页拼写<b>正常</b>）与 <c>DIGEST_ALGORITHM_SM3</c>，
/// 与电子发票域的 <c>digest_alogrithm</c> / <c>SM3</c> <b>不是同一契约</b>；
/// ③ 2xx 但拿不到 <c>fapiao_media_id</c> 这种「看起来成功却不可用」的应答必须显式失败。
/// </para>
/// </remarks>
public class WechatPayTaxiInvoiceFileServiceTests
{
    /// <summary>文件名或商户号空白 ⇒ 参数错误（不得发出一个官方必然判错的请求）。</summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Upload_ShouldReject_WhenFileNameOrCompanyBlank(string blank)
    {
        var sut = CreateSut();

        var byName = async () => await sut.UploadTaxiInvoiceFileAsync(
            new MemoryStream(new byte[] { 1 }), blank, "1900000109", 310100, "aa11bb22");
        var byCompany = async () => await sut.UploadTaxiInvoiceFileAsync(
            new MemoryStream(new byte[] { 1 }), "taxi.pdf", blank, 310100, "aa11bb22");

        await byName.Should().ThrowAsync<ArgumentException>();
        await byCompany.Should().ThrowAsync<ArgumentException>();
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

        var act = async () => await sut.UploadTaxiInvoiceFileAsync(
            new MemoryStream(new byte[] { 1 }), "taxi.pdf", "1900000109", 310100, digest);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*SM3*");
    }

    /// <summary>内容流为 null ⇒ <see cref="ArgumentNullException"/>。</summary>
    [Fact]
    public async Task Upload_ShouldReject_WhenContentNull()
    {
        var sut = CreateSut();

        var act = async () => await sut.UploadTaxiInvoiceFileAsync(
            null!, "taxi.pdf", "1900000109", 310100, "aa11bb22");

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>输入非法时不得触达 HTTP 客户端（证明「拒绝发生在发送之前」）。</summary>
    [Fact]
    public async Task Upload_ShouldNotSendRequest_WhenInputRejected()
    {
        var client = new Mock<IWechatPayHttpClient>(MockBehavior.Strict);
        var sut = new WechatPayTaxiInvoiceFileService(client.Object, NullLogger<WechatPayTaxiInvoiceFileService>.Instance);

        var act = async () => await sut.UploadTaxiInvoiceFileAsync(
            new MemoryStream(new byte[] { 1 }), "taxi.pdf", "1900000109", 310100, " ");

        await act.Should().ThrowAsync<ArgumentException>();
        client.VerifyNoOtherCalls();
    }

    /// <summary>
    /// <b>表单形态锁定</b>：<c>multipart/form-data</c>，两部分名为 <c>file</c> / <c>meta</c>，
    /// 且 <c>meta</c> 内四个字段名与取值照本页官方原文（<c>digest_algorithm</c> 拼写<b>正常</b>、
    /// 取值 <c>DIGEST_ALGORITHM_SM3</c>、<c>region_id</c> 为 JSON 数字）。
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
            // 若只存下 HttpRequestMessage 引用、事后再读，会读到已释放的内容而假红。
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
                    "{\"fapiao_media_id\":\"ASNFZ4mrze/+3LqYdlQyEA==\"}", Encoding.UTF8, "application/json"),
            });

        var sut = new WechatPayTaxiInvoiceFileService(client.Object, NullLogger<WechatPayTaxiInvoiceFileService>.Instance);

        var result = await sut.UploadTaxiInvoiceFileAsync(
            new MemoryStream(Encoding.UTF8.GetBytes("pdf-bytes")), "taxi.pdf", "1900000109", 310100, "aa11bb22");

        result.FapiaoMediaId.Should().Be("ASNFZ4mrze/+3LqYdlQyEA==");

        method.Should().Be(HttpMethod.Post);
        uri.Should().Be("https://api.mch.weixin.qq.com/v3/taxi-invoice/cards/upload-file",
            "上传走**主接入点**（本域官方另设备域名 api2，由宿主配置切换、不在通道里硬编码）");
        contentTypeName.Should().Be(nameof(MultipartFormDataContent), "官方契约要求 multipart/form-data");

        formText.Should().NotBeNull();

        // ⚠️ 表单段名用**正则容许带/不带引号**：.NET 写出的是无引号形态（name=file），
        // 锁「段名到底是什么」即可，不必把 .NET 当前的引号风格一起锁死。
        Regex.IsMatch(formText!, "name=\"?file\"?").Should().BeTrue("官方表单字段名就是 file");
        Regex.IsMatch(formText!, "name=\"?meta\"?").Should().BeTrue("官方表单字段名就是 meta");
        formText.Should().Contain(WechatPayTaxiInvoiceFileService.CompanyMchidFieldName);
        formText.Should().Contain("1900000109");
        formText.Should().Contain(WechatPayTaxiInvoiceFileService.DigestFieldName);
        formText.Should().Contain("aa11bb22", "调用方传入的摘要必须原样提交");
        formText.Should().Contain(WechatPayTaxiInvoiceFileService.DigestAlgorithmFieldName,
            "⚠️ 本页官方拼写是 digest_algorithm（正常），不是电子发票域的 digest_alogrithm");
        formText.Should().Contain(WechatPayTaxiInvoiceFileService.Sm3DigestAlgorithm,
            "本页该字段唯一可选值 DIGEST_ALGORITHM_SM3（电子发票域那页是裸 SM3）");
        formText.Should().Contain(WechatPayTaxiInvoiceFileService.RegionIdFieldName);
        Regex.IsMatch(formText!, "\"region_id\"\\s*:\\s*310100").Should().BeTrue(
            "官方字段表把 region_id 定为 integer ⇒ 以 JSON 数字提交（页内示例出现字符串形态属示例本身的口径不一）");

        // 跨域红线的反面对照：本通道不得出现电子发票域的那套键名。
        formText.Should().NotContain("digest_alogrithm", "少一个 r 的拼写属电子发票域，写进本域即官方 PARAM_ERROR");
        formText.Should().NotContain("\"file_type\"", "file_type 是电子发票域 meta 的字段，本页官方未印发");
    }

    /// <summary>
    /// <b>2xx 但没有 <c>fapiao_media_id</c></b> ⇒ 必须显式失败：
    /// 返回空壳会让调用方拿着 null 去插卡。
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

        var sut = new WechatPayTaxiInvoiceFileService(client.Object, NullLogger<WechatPayTaxiInvoiceFileService>.Instance);

        var act = async () => await sut.UploadTaxiInvoiceFileAsync(
            new MemoryStream(new byte[] { 1 }), "taxi.pdf", "1900000109", 310100, "aa");

        await act.Should().ThrowAsync<WechatPayException>().WithMessage("*fapiao_media_id*");
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

        var sut = new WechatPayTaxiInvoiceFileService(client.Object, NullLogger<WechatPayTaxiInvoiceFileService>.Instance);

        var act = async () => await sut.UploadTaxiInvoiceFileAsync(
            new MemoryStream(new byte[] { 1 }), "taxi.pdf", "1900000109", 310100, "aa");

        (await act.Should().ThrowAsync<WechatPayException>())
            .Which.PayErrorCode.Should().Be("PARAM_ERROR", "官方错误体里的 code 必须被保留（判错面不得降级为裸消息）");
    }

    /// <summary><b>常量锁定</b>：路由与表单键名/取值照官方页原文，任一漂移即在此 fail。</summary>
    [Fact]
    public void FileChannelConstants_ShouldMatchOfficialPage()
    {
        WechatPayTaxiInvoiceFileService.PrimaryAccessPoint.Should().Be("https://api.mch.weixin.qq.com");
        WechatPayTaxiInvoiceFileService.UploadPath.Should().Be("/v3/taxi-invoice/cards/upload-file");
        WechatPayTaxiInvoiceFileService.FileFieldName.Should().Be("file");
        WechatPayTaxiInvoiceFileService.MetaFieldName.Should().Be("meta");
        WechatPayTaxiInvoiceFileService.CompanyMchidFieldName.Should().Be("company_mchid");
        WechatPayTaxiInvoiceFileService.DigestFieldName.Should().Be("digest");
        WechatPayTaxiInvoiceFileService.DigestAlgorithmFieldName.Should().Be("digest_algorithm");
        WechatPayTaxiInvoiceFileService.Sm3DigestAlgorithm.Should().Be("DIGEST_ALGORITHM_SM3");
        WechatPayTaxiInvoiceFileService.RegionIdFieldName.Should().Be("region_id");
        WechatPayTaxiInvoiceFileService.MediaIdFieldName.Should().Be("fapiao_media_id");
    }

    private static WechatPayTaxiInvoiceFileService CreateSut()
        => new(new Mock<IWechatPayHttpClient>(MockBehavior.Loose).Object,
               NullLogger<WechatPayTaxiInvoiceFileService>.Instance);
}
