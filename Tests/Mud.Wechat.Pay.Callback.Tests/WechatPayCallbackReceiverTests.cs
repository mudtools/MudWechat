// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 支付通知接收器三道闸的 <b>fail-closed</b> 用例（方案 §2.6 / §2.8 PAY-B3）。
/// </summary>
/// <remarks>
/// 三闸任一处放宽都是安全事故（截获一次合法通知即可无限重放、或凭畸形报文绕过校验），
/// 故本组逐闸枚举失败形态，且全部使用真实 RSA 与真实 AES-256-GCM。
/// </remarks>
public class WechatPayCallbackReceiverTests
{
    /// <summary>合法通知：三闸全过并解出类型化交易载荷。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReturnDecryptedContext_WhenAllGatesPass()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var context = await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        context.MerchantKey.Should().Be(WechatPayCallbackTestFixture.MerchantKey);
        context.EventType.Should().Be("TRANSACTION.SUCCESS");
        context.NotificationId.Should().Be("EV-TEST-1");

        var transaction = context.GetTransaction();
        transaction.Should().NotBeNull("解密后的载荷必须可按交易类型化解析（AOT 源生成快车道）");
        transaction!.OutTradeNo.Should().Be("ORDER-1");
        transaction.TradeState.Should().Be("SUCCESS");
        transaction.Amount!.Total.Should().Be(100);
        transaction.Payer!.OpenId.Should().Be("o-test");
    }

    /// <summary>闸① fail-closed：报文体被改动 ⇒ 验签不匹配，拒绝。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenBodyTampered()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body + " ");

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    /// <summary>闸① fail-closed：伪造签名（换一段长度合法的 Base64）⇒ 拒绝。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenSignatureForged()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var forged = new WechatPayCallbackHeaders(
            headers.Timestamp, headers.Nonce, Convert.ToBase64String(new byte[256]), headers.SerialNumber);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, forged, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    /// <summary>闸① fail-closed：未知 <c>Wechatpay-Serial</c> ⇒ 拒绝（不得回落到任意证书）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenSerialUnknown()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(
            WechatPayCallbackTestFixture.TransactionResourceJson, serial: "DEADBEEFDEADBEEF");

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    /// <summary>
    /// 闸① fail-closed：公钥模式（<c>PUB_KEY_ID_</c>）被<b>显式点名</b>拒绝（方案 §2.5 待决项的裁决落地）。
    /// </summary>
    [Fact]
    public async Task ReceiveAsync_ShouldRejectPublicKeyMode_Explicitly()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(
            WechatPayCallbackTestFixture.TransactionResourceJson, serial: "PUB_KEY_ID_0123456789");

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        var exception = (await act.Should().ThrowAsync<WechatCallbackException>()).Which;
        exception.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
        exception.Message.Should().Contain("PUB_KEY_ID_", "必须点名公钥模式，而非留下「未知序列号」这种无从排查的形态");
    }

    /// <summary>闸② fail-closed：缺失时间戳 ⇒ 拒绝。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenTimestampMissing()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var stripped = new WechatPayCallbackHeaders(null, headers.Nonce, headers.Signature, headers.SerialNumber);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, stripped, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.MissingTimestamp);
    }

    /// <summary>闸② fail-closed：非数字时间戳 ⇒ 拒绝。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenTimestampNotNumeric()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var bad = new WechatPayCallbackHeaders("not-a-number", headers.Nonce, headers.Signature, headers.SerialNumber);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, bad, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.MissingTimestamp);
    }

    /// <summary>闸② fail-closed：超出 ±300s 窗口 ⇒ 拒绝（这正是 v1 遗漏、可致无限重放的那道闸）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenTimestampOutOfWindow()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        // 用一小时前的时间戳签名：签名合法，但时效不合 ⇒ 必须被闸②拦下。
        var staleTimestamp = (fixture.Clock.UtcNowEpochSeconds - 3600)
            .ToString(System.Globalization.CultureInfo.InvariantCulture);
        var (headers, body) = fixture.CreateNotification(
            WechatPayCallbackTestFixture.TransactionResourceJson, timestamp: staleTimestamp);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.TimestampOutOfRange);
    }

    /// <summary>闸③ fail-closed：同一密文重放 ⇒ 第二次拒绝（指纹已在保留窗口内消费）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenReplayed()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.ReplaySuspected);
    }

    /// <summary>配置关闭指纹闸时重放放行（仅排障语义；默认必开，由配置守卫锁定）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldAllowReplay_WhenReplayGuardDisabled_ForDiagnosticsOnly()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        fixture.Options.RequireReplayGuard = false;
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);
        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        await act.Should().NotThrowAsync();
    }

    /// <summary>
    /// 解密失败（APIv3 密钥不匹配）⇒ 拒绝，<b>且不消耗指纹</b>：换回正确密钥后可成功重入。
    /// </summary>
    /// <remarks>
    /// 这是「官方重试可重入」的关键性质：若指纹在解密前消费，一次密钥抖动就会把指纹吃掉，
    /// 官方重试将被判重放而永久丢单。
    /// </remarks>
    [Fact]
    public async Task ReceiveAsync_ShouldRejectOnDecryptFailure_WithoutConsumingFingerprint()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        fixture.Credentials.ApiKey = Encoding.UTF8.GetBytes("ffffffffffffffffffffffffffffffff");

        var failed = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);
        (await failed.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);

        // 密钥恢复后同一密文必须仍可处理（指纹未被消费）。
        fixture.Credentials.ApiKey = Encoding.UTF8.GetBytes(WechatPayCallbackTestFixture.ApiKeyText);
        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);
        context.GetTransaction()!.OutTradeNo.Should().Be("ORDER-1");
    }

    /// <summary>非法 Base64 密文 ⇒ 拒绝（不抛出密码学异常、不回显原文）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenCipherTextNotBase64()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(
            WechatPayCallbackTestFixture.TransactionResourceJson, cipherTextOverride: "!!!not-base64!!!");

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);
    }

    /// <summary>不支持的加密算法 ⇒ 拒绝（不得按 AEAD_AES_256_GCM 硬解）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenAlgorithmUnsupported()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(
            WechatPayCallbackTestFixture.TransactionResourceJson, algorithm: "AEAD_AES_128_GCM");

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);
    }

    /// <summary>报文缺少 <c>resource</c> 节点 ⇒ 拒绝。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenResourceMissing()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        const string body =
            "{\"id\":\"EV-1\",\"event_type\":\"TRANSACTION.SUCCESS\",\"summary\":\"支付成功\"}";

        // 合法签名 + 缺 resource：必须走到「缺节点」判定，而不是被闸①拦下。
        var (signedHeaders, signedBody) = fixture.WithSignedBody(body);
        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, signedHeaders, signedBody);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.MissingEncrypt);
    }

    /// <summary>未登记商户键 ⇒ 拒绝（不得回落默认商户）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenMerchantUnknown()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var act = async () => await receiver.ReceiveAsync("9999999999", headers, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.UnknownReceiver);
    }

    /// <summary>缺少签名头 ⇒ 拒绝（缺失即畸形）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenSignatureHeaderMissing()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var stripped = new WechatPayCallbackHeaders(
            headers.Timestamp, headers.Nonce, null, headers.SerialNumber);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, stripped, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.MissingSignature);
    }

    /// <summary>缺少序列号头 ⇒ 拒绝。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenSerialHeaderMissing()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var stripped = new WechatPayCallbackHeaders(
            headers.Timestamp, headers.Nonce, headers.Signature, null);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, stripped, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.MissingSignature);
    }

    /// <summary>
    /// 闸① 自愈路径：官方<b>轮换</b>平台证书后，新序列号先于本机拉取到达 ⇒
    /// 触发一次刷新后复验通过（设计方案 §2.6「未知 serial ⇒ 触发一次刷新后仍失败即拒」）。
    /// </summary>
    [Fact]
    public async Task ReceiveAsync_ShouldRecoverViaRefresh_WhenSerialJustRotated()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var refresher = new WechatPayCallbackTestFixture.FakeCertificateRefresher(fixture);
        var receiver = fixture.CreateReceiver(refresher);

        // 官方已换新钥并在通知头里给出新序列号，而本机存储里还没有这本证书。
        fixture.RotateSigningCertificate();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        fixture.Certificates.TryGetCertificate(headers.SerialNumber, out _)
            .Should().BeFalse("前置条件：刷新之前本机不得认识该新序列号");

        var context = await receiver.ReceiveAsync(WechatPayCallbackTestFixture.MerchantKey, headers, body);

        context.GetTransaction()!.OutTradeNo.Should().Be("ORDER-1");
        refresher.CallCount.Should().Be(1);
        refresher.DownloadCount.Should().Be(1, "未知序列号只允许触发一次真实下载");
    }

    /// <summary>刷新仍拿不到该序列号 ⇒ 照常拒绝（fail-closed，<b>不</b>因「刷过」而放宽）。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenRefreshCannotResolveSerial()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var refresher = new WechatPayCallbackTestFixture.FakeCertificateRefresher(fixture, canPublish: false);
        var receiver = fixture.CreateReceiver(refresher);

        fixture.RotateSigningCertificate();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
        refresher.CallCount.Should().Be(1, "只补一次，不得重试到成功");
    }

    /// <summary>未注册刷新端口（宿主只装回调包）⇒ 未知序列号直接拒绝，且不抛其它异常。</summary>
    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenNoRefresherRegistered()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var receiver = fixture.CreateReceiver();

        fixture.RotateSigningCertificate();
        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    /// <summary>
    /// <b>防放大</b>：序列号已知却签名不匹配（攻击者可随意构造的最廉价形态）时，
    /// 绝不会触发证书下载 —— 端口实现须以「已命中即短路」守住这一点。
    /// </summary>
    /// <remarks>
    /// 若改成「验签失败就刷新」，每个伪造请求都会变成一次证书仓库往返（放大型 DoS）。
    /// 判据分两层锁定：本用例锁「短路不下载」，<c>WechatPayPlatformCertificateRefresherTests</c>
    /// 另有「已命中 ⇒ 客户端零调用」的实现级用例。
    /// </remarks>
    [Fact]
    public async Task ReceiveAsync_ShouldNotDownload_WhenSerialKnownButSignatureTampered()
    {
        using var fixture = new WechatPayCallbackTestFixture();
        var refresher = new WechatPayCallbackTestFixture.FakeCertificateRefresher(fixture);
        var receiver = fixture.CreateReceiver(refresher);

        var (headers, body) = fixture.CreateNotification(WechatPayCallbackTestFixture.TransactionResourceJson);

        // 序列号仍是本机已知的那一本，但报文体被改过 ⇒ 验签必然失败。
        var act = async () => await receiver.ReceiveAsync(
            WechatPayCallbackTestFixture.MerchantKey, headers, body + " ");

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);

        refresher.CallCount.Should().Be(1, "端口会被征询一次（由它自行短路）");
        refresher.DownloadCount.Should().Be(0, "序列号已知 ⇒ 不得下载：否则伪造请求可驱动证书仓库流量");
    }
}
