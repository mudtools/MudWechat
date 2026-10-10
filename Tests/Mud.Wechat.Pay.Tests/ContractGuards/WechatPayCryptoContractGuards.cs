// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 支付密码学与签名守卫（方案 v2 §2.8，P0-b 出口门禁：<b>PAY-B2 / B3 / B4 / B6 / B7</b>）。
/// </summary>
/// <remarks>
/// <para>
/// 这一组守卫的共同特征是「**错了不会编译失败**」：签名串换行位置、验签 fail-closed 分支、
/// GCM 认证失败的收敛方式、异常消息是否夹带密文 —— 全部只在**线上真实交易**时才暴露，
/// 且第一次暴露就是资金侧故障。故一律用黄金向量 + 反向用例锁死。
/// </para>
/// <para>
/// 黄金向量来源见 <see cref="WechatPayGoldenVectors"/>：拼接规则逐字对齐官方 SDK
/// <c>WechatPay2Credential#buildMessage</c> / <c>WechatPay2Validator#validate</c>。
/// </para>
/// </remarks>
public class WechatPayCryptoContractGuards
{
    // ==================== PAY-B2：签名黄金向量 ====================

    /// <summary>
    /// PAY-B2：**请求**签名串逐字节对齐官方 <c>WechatPay2Credential#buildMessage</c>。
    /// </summary>
    /// <remarks>
    /// 三处最容易被「顺手优化」掉的细节，本断言全部覆盖：
    /// ① 每段之间是 <c>\n</c>（不是 <c>Environment.NewLine</c>，Windows 下会变成 <c>\r\n</c> ⇒ 全量验签失败）；
    /// ② 请求体之后**还有一个** <c>\n</c>（结尾换行）；③ 空请求体取空串而非 <c>null</c> 字面量。
    /// </remarks>
    [Fact]
    public void RequestMessage_ShouldMatchOfficialByteLayout_WhenGoldenVector()
    {
        var built = WechatPaySignatureMessages.BuildRequestMessage(
            WechatPayGoldenVectors.RequestMethod,
            WechatPaySignatureMessages.BuildCanonicalUrl(WechatPayGoldenVectors.RequestCanonicalUrl, null),
            WechatPayGoldenVectors.RequestTimestamp,
            WechatPayGoldenVectors.RequestNonce,
            WechatPayGoldenVectors.RequestBody);

        var golden = DecodeUtf8(WechatPayGoldenVectors.RequestMessageB64);

        built.Should().Be(golden,
            "签名串是 PAY-B2 的核心契约：任一字节漂移都会让线上验签 100% 失败，且只在真实交易时暴露");

        built.Should().EndWith("\n",
            "官方 buildMessage 每段（含请求体后）都以 \\n 收尾，缺尾换行即逐字节不符");
        built.Should().NotEndWith("\n\n", "结尾只能有恰好一个换行");

        built.Split('\n').Should().Equal(
            new[]
            {
                WechatPayGoldenVectors.RequestMethod,
                WechatPayGoldenVectors.RequestCanonicalUrl,
                "1729040000",
                WechatPayGoldenVectors.RequestNonce,
                WechatPayGoldenVectors.RequestBody,
                string.Empty,
            },
            "because 五段 + 尾换行切出的空段；段数或顺序漂移说明换行位置被改动");
    }

    /// <summary>PAY-B2：**验签**串逐字节对齐官方 <c>WechatPay2Validator#validate</c>（应答与回调同形态）。</summary>
    [Fact]
    public void VerifyMessage_ShouldMatchOfficialByteLayout_WhenGoldenVector()
    {
        var built = WechatPaySignatureMessages.BuildVerifyMessage(
            WechatPayGoldenVectors.VerifyTimestamp,
            WechatPayGoldenVectors.VerifyNonce,
            WechatPayGoldenVectors.VerifyBody);

        var golden = DecodeUtf8(WechatPayGoldenVectors.VerifyMessageB64);

        built.Should().Be(golden,
            "验签串 = 时间戳\\n随机串\\n报文体\\n；报文体必须是**原文**，反序列化后重排即验签失败");
        built.Should().EndWith("\n", "官方 validate 同样以 \\n 收尾");
        built.Split('\n').Should().HaveCount(4, "三段 + 尾换行切出的空段");
    }

    /// <summary>
    /// PAY-B2：规范 URL = <c>rawPath</c> +（有查询时）<c>?rawQuery</c>，与官方 <c>uri.getRawPath()</c> 语义一致。
    /// </summary>
    [Fact]
    public void CanonicalUrl_ShouldAppendRawQuery_WhenQueryPresent()
    {
        WechatPaySignatureMessages.BuildCanonicalUrl("/v3/pay/transactions/jsapi", null)
            .Should().Be("/v3/pay/transactions/jsapi", "无查询时不得追加问号");

        WechatPaySignatureMessages.BuildCanonicalUrl("/v3/pay/transactions/jsapi", "foo=bar&baz=1")
            .Should().Be("/v3/pay/transactions/jsapi?foo=bar&baz=1",
                "有查询时必须并入签名串（官方 buildMessage 显式拼接 ? + rawQuery）");

        // 空串与 null 同义：避免 "?"" 这种会让签名串多出两个字节的形态。
        WechatPaySignatureMessages.BuildCanonicalUrl("/v3/x", string.Empty)
            .Should().Be("/v3/x");
    }

    /// <summary>PAY-B2：<c>Authorization</c> 头照官方字段顺序组装。</summary>
    [Fact]
    public void Authorization_ShouldFollowOfficialFieldOrder_WhenBuilt()
    {
        var header = WechatPaySignatureMessages.BuildAuthorization(
            "1900000000", "4946B705E468CEC8", 1729040000,
            "MUDWECHATPAYNONCE0123456789ABCDEF", "SIGNATURE");

        header.Should().Be(
            "WECHATPAY2-SHA256-RSA2048 " +
            "mchid=\"1900000000\"," +
            "nonce_str=\"MUDWECHATPAYNONCE0123456789ABCDEF\"," +
            "timestamp=\"1729040000\"," +
            "serial_no=\"4946B705E468CEC8\"," +
            "signature=\"SIGNATURE\"",
            "字段顺序照官方原文（mchid → nonce_str → timestamp → serial_no → signature），便于抓包排障对齐");

        header.Should().StartWith(WechatPaySignatureMessages.AuthorizationSchema + " ",
            "schema 前缀 WECHATPAY2-SHA256-RSA2048 是官方固定值");
    }

    /// <summary>
    /// PAY-B2：RSA-SHA256 + PKCS#1 v1.5 对「固定私钥 + 固定消息」**确定性**——黄金签名必须原样复现。
    /// </summary>
    /// <remarks>
    /// 这是唯一能挡住「换填充 / 换哈希 / 换签名入参」的断言：这些改动**都能编译通过**，
    /// 且单测若只做「签名后自验」也会全绿（自验只证明内部自洽，不证明与官方一致）。
    /// </remarks>
    [Fact]
    public void Sign_ShouldReproduceGoldenSignature_WhenFixedKeyAndMessage()
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(WechatPayGoldenVectors.MerchantPrivateKeyPem);

        using var cache = CreatePlatformStore();
        var provider = new WechatPaySignatureProvider(
            WechatPayGoldenVectors.MerchantSerialNumber, rsa, cache);

        var message = DecodeUtf8(WechatPayGoldenVectors.RequestMessageB64);

        var first = provider.Sign(message);
        var second = provider.Sign(message);

        first.Signature.Should().Be(WechatPayGoldenVectors.RequestSignature,
            "RSA-SHA256 + Pkcs1 无随机性 ⇒ 同钥同消息恒定；漂移即算法/填充/入参被改（PAY-B2）");
        first.Signature.Should().Be(second.Signature, "重复签名必须得到同一结果");
        first.SerialNumber.Should().Be(WechatPayGoldenVectors.MerchantSerialNumber,
            "serial_no 必须来自商户证书，供官方反查公钥");
    }

    // ==================== PAY-B3：验签 fail-closed ====================

    /// <summary>PAY-B3 基准：证书 ↔ 公钥 ↔ 签名三者必须自洽，否则本组反向用例失去意义。</summary>
    [Fact]
    public void PlatformCertificate_ShouldBindToGoldenSignature_WhenImportedFromPem()
    {
        using var cert = X509Certificate2.CreateFromPem(WechatPayGoldenVectors.PlatformCertificatePem);

        cert.SerialNumber.Should().Be(WechatPayGoldenVectors.PlatformSerialNumber,
            "序列号是验签查表键，与 fixture 必须一致");

        using var publicKey = cert.GetRSAPublicKey();
        publicKey.Should().NotBeNull("平台证书必须携带 RSA 公钥（WECHATPAY2-SHA256-RSA2048）");
        publicKey!.KeySize.Should().Be(2048, "官方平台证书为 RSA-2048");

        var message = DecodeUtf8(WechatPayGoldenVectors.VerifyMessageB64);
        publicKey.VerifyData(
                Encoding.UTF8.GetBytes(message),
                Convert.FromBase64String(WechatPayGoldenVectors.VerifySignature),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1)
            .Should().BeTrue("fixture 证书公钥必须能验过 fixture 签名，否则 PAY-B3 的「通过」基准本身是坏的");
    }

    /// <summary>PAY-B3：合法签名 + 正确序列号 ⇒ 通过（反向用例的对照组）。</summary>
    [Fact]
    public void Verify_ShouldAcceptAuthenticSignature_WhenSerialMatches()
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(WechatPayGoldenVectors.MerchantPrivateKeyPem);

        using var cache = CreatePlatformStore();
        var provider = new WechatPaySignatureProvider(
            WechatPayGoldenVectors.MerchantSerialNumber, rsa, cache);

        var message = DecodeUtf8(WechatPayGoldenVectors.VerifyMessageB64);

        provider.Verify(WechatPayGoldenVectors.PlatformSerialNumber, message,
                WechatPayGoldenVectors.VerifySignature)
            .Should().BeTrue("平台证书 + 正确序列号 + 原文验签串必须通过，否则守卫会把正确实现判死");
    }

    /// <summary>PAY-B3：**伪造签名一律拒绝**（重要性等同企微抗重放双闸）。</summary>
    [Fact]
    public void Verify_ShouldRejectTamperedSignature_WhenForged()
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(WechatPayGoldenVectors.MerchantPrivateKeyPem);

        using var cache = CreatePlatformStore();
        var provider = new WechatPaySignatureProvider(
            WechatPayGoldenVectors.MerchantSerialNumber, rsa, cache);

        var message = DecodeUtf8(WechatPayGoldenVectors.VerifyMessageB64);
        var forged = FlipLastByte(WechatPayGoldenVectors.VerifySignature);

        forged.Should().NotBe(WechatPayGoldenVectors.VerifySignature, "用例前置：篡改必须真的改变了签名");

        provider.Verify(WechatPayGoldenVectors.PlatformSerialNumber, message, forged)
            .Should().BeFalse("篡改签名必须拒绝；放行即等于任何人都能伪造支付成功通知");

        // 报文被改一个字节 —— 签名覆盖原文，同样必须拒绝。
        var tamperedMessage = message[..^1] + (message[^1] == '}' ? 'x' : '}');
        provider.Verify(WechatPayGoldenVectors.PlatformSerialNumber, tamperedMessage,
                WechatPayGoldenVectors.VerifySignature)
            .Should().BeFalse("验签串必须逐字节覆盖原始报文，改一个字符即失效");
    }

    /// <summary>PAY-B3：**未知 / 缺失** <c>Wechatpay-Serial</c> ⇒ 拒绝（证书轮换后的查表键不可用即拒）。</summary>
    [Fact]
    public void Verify_ShouldRejectUnknownOrMissingSerial_WhenNotRegistered()
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(WechatPayGoldenVectors.MerchantPrivateKeyPem);

        using var cache = CreatePlatformStore();
        var provider = new WechatPaySignatureProvider(
            WechatPayGoldenVectors.MerchantSerialNumber, rsa, cache);

        var message = DecodeUtf8(WechatPayGoldenVectors.VerifyMessageB64);

        provider.Verify("00F5140D349A73226C-UNKNOWN", message, WechatPayGoldenVectors.VerifySignature)
            .Should().BeFalse("未知序列号 ⇒ 端口查不到证书 ⇒ 拒绝，不得回落到「任意证书」");
        provider.Verify(null, message, WechatPayGoldenVectors.VerifySignature)
            .Should().BeFalse("序列号缺失即畸形报文，必须拒绝而非跳过校验");
        provider.Verify("   ", message, WechatPayGoldenVectors.VerifySignature)
            .Should().BeFalse("空白序列号同缺失处理（fail-closed）");
        provider.Verify(WechatPayGoldenVectors.PlatformSerialNumber, message, null)
            .Should().BeFalse("签名缺失必须拒绝");
        provider.Verify(WechatPayGoldenVectors.PlatformSerialNumber, null, WechatPayGoldenVectors.VerifySignature)
            .Should().BeFalse("验签串缺失必须拒绝");
        provider.Verify(WechatPayGoldenVectors.PlatformSerialNumber, message, "not-base64!!")
            .Should().BeFalse("非法 Base64 必须拒绝且**不得抛出**（抛出会把签名原文带进异常消息）");
    }

    /// <summary>
    /// PAY-B3：**时间戳窗**三态（缺失 / 非数字 / 超窗）全部拒绝 —— 只验签不校验时效 = 可无限重放。
    /// </summary>
    /// <remarks>
    /// 窗口边界取 <c>&lt;= 300</c> 放行，与本仓 <c>MpCallbackReceiver</c> 的
    /// <c>Math.Abs(now - ts) &gt; AllowClockSkewSeconds</c> 同形；官方 SDK 用 <c>toMinutes() &gt;= 5</c>
    /// 即 <c>&gt;= 300</c> 拒绝，两者仅在**恰好 300 秒**这一秒上有差异，均为 fail-closed（见实现类注释）。
    /// </remarks>
    [Fact]
    public void TimestampGate_ShouldRejectMissingNonNumericAndOutOfWindow_WhenValidated()
    {
        const long now = 1729040000L;

        WechatPayTimestampGate.Validate(null, now)
            .Should().Be(WechatPayTimestampGate.Verdict.Missing, "时间戳缺失必须拒绝");
        WechatPayTimestampGate.Validate(string.Empty, now)
            .Should().Be(WechatPayTimestampGate.Verdict.Missing);
        WechatPayTimestampGate.Validate("   ", now)
            .Should().Be(WechatPayTimestampGate.Verdict.Missing, "纯空白不得被当成 0 解析后放行");
        WechatPayTimestampGate.Validate("abc", now)
            .Should().Be(WechatPayTimestampGate.Verdict.NotNumeric, "非数字必须拒绝，不得静默取 0");
        WechatPayTimestampGate.Validate("1729040000.0", now)
            .Should().Be(WechatPayTimestampGate.Verdict.NotNumeric, "小数形态不得放行");
        WechatPayTimestampGate.Validate(" 1729040000 ", now)
            .Should().Be(WechatPayTimestampGate.Verdict.NotNumeric,
                "NumberStyles.None 不接受首尾空白，避免不同网关补空格后绕过窗口");
        WechatPayTimestampGate.Validate("-1", now)
            .Should().Be(WechatPayTimestampGate.Verdict.NotNumeric, "负时间戳必须拒绝");

        WechatPayTimestampGate.Validate((now - 301).ToString(), now)
            .Should().Be(WechatPayTimestampGate.Verdict.OutOfWindow, "早于窗口必须拒绝（重放）");
        WechatPayTimestampGate.Validate((now + 301).ToString(), now)
            .Should().Be(WechatPayTimestampGate.Verdict.OutOfWindow, "晚于窗口必须拒绝（未来时间戳）");
        WechatPayTimestampGate.Validate("1", now)
            .Should().Be(WechatPayTimestampGate.Verdict.OutOfWindow);

        WechatPayTimestampGate.Validate((now - 300).ToString(), now)
            .Should().Be(WechatPayTimestampGate.Verdict.Valid, "窗口内放行（与本仓回调闸同形）");
        WechatPayTimestampGate.IsAccepted((now + 300).ToString(), now)
            .Should().BeTrue();
        WechatPayTimestampGate.IsAccepted((now - 301).ToString(), now)
            .Should().BeFalse("IsAccepted 必须与 Validate 同源，不得出现两套判定");
    }

    // ==================== PAY-B4：AES-256-GCM ====================

    /// <summary>PAY-B4：黄金向量解密 —— 固定 key/nonce/aad 必须解出固定明文。</summary>
    [Fact]
    public void AesGcm_ShouldDecryptGoldenVector_WhenAuthenticated()
    {
        var key = Convert.FromBase64String(WechatPayGoldenVectors.AesKeyB64);
        var nonce = Convert.FromBase64String(WechatPayGoldenVectors.AesNonceB64);
        var aad = Convert.FromBase64String(WechatPayGoldenVectors.AesAssociatedDataB64);
        var ciphertext = Convert.FromBase64String(WechatPayGoldenVectors.AesCiphertextB64);
        var tag = Convert.FromBase64String(WechatPayGoldenVectors.AesTagB64);

        key.Length.Should().Be(WechatPayAesGcmCodec.KeySizeBytes, "APIv3 密钥为 32 字节");
        nonce.Length.Should().Be(WechatPayAesGcmCodec.NonceSizeBytes, "GCM nonce 为 12 字节");
        tag.Length.Should().Be(WechatPayAesGcmCodec.TagSizeBytes, "GCM 认证标签为 16 字节");

        var ok = WechatPayAesGcmCodec.TryDecrypt(key, nonce, ciphertext, tag, aad, out var plaintext);

        ok.Should().BeTrue("合法 AEAD_AES_256_GCM 载荷必须解得开，否则回调 resource 全量不可用");
        plaintext.Should().NotBeNull();
        Convert.ToBase64String(plaintext!)
            .Should().Be(WechatPayGoldenVectors.AesPlaintextB64, "解密结果必须逐字节等于黄金明文");
    }

    /// <summary>PAY-B4：往返一致 + **nonce 不得复用**（GCM 复用 nonce 会直接泄露明文异或值）。</summary>
    [Fact]
    public void AesGcm_ShouldRoundtripWithFreshNonce_WhenEncrypting()
    {
        var key = Convert.FromBase64String(WechatPayGoldenVectors.AesKeyB64);
        var aad = Convert.FromBase64String(WechatPayGoldenVectors.AesAssociatedDataB64);
        var expected = Convert.FromBase64String(WechatPayGoldenVectors.AesPlaintextB64);

        var first = WechatPayAesGcmCodec.Encrypt(key, expected, aad);
        var second = WechatPayAesGcmCodec.Encrypt(key, expected, aad);

        Convert.ToBase64String(first.Nonce)
            .Should().NotBe(Convert.ToBase64String(second.Nonce),
                "同一明文两次加密必须产出不同 nonce；GCM nonce 复用是**密码学级**事故");

        WechatPayAesGcmCodec.TryDecrypt(key, first.Nonce, first.Ciphertext, first.Tag, aad, out var roundtrip)
            .Should().BeTrue();
        Convert.ToBase64String(roundtrip!).Should().Be(Convert.ToBase64String(expected),
            "往返必须一致");

        // 关联数据不匹配 ⇒ 认证失败（GCM 的 AAD 同样被认证）。
        WechatPayAesGcmCodec.TryDecrypt(key, first.Nonce, first.Ciphertext, first.Tag,
                Encoding.UTF8.GetBytes("other-associated-data"), out var wrongAad)
            .Should().BeFalse("AAD 变化必须导致认证失败");
        wrongAad.Should().BeNull();
    }

    /// <summary>
    /// PAY-B4 + PAY-B7：解密失败**不抛出、不泄漏明文**——异常消息会夹带密文字节，
    /// 等于把 <c>resource.ciphertext</c> 洒进日志与遥测。
    /// </summary>
    [Fact]
    public void AesGcm_ShouldFailClosedWithoutLeaking_WhenAuthenticationFails()
    {
        var key = Convert.FromBase64String(WechatPayGoldenVectors.AesKeyB64);
        var nonce = Convert.FromBase64String(WechatPayGoldenVectors.AesNonceB64);
        var aad = Convert.FromBase64String(WechatPayGoldenVectors.AesAssociatedDataB64);
        var ciphertext = Convert.FromBase64String(WechatPayGoldenVectors.AesCiphertextB64);
        var tag = Convert.FromBase64String(WechatPayGoldenVectors.AesTagB64);

        var corruptTag = (byte[])tag.Clone();
        corruptTag[0] ^= 0xFF;

        var corruptCiphertext = (byte[])ciphertext.Clone();
        corruptCiphertext[0] ^= 0xFF;

        Action actBadTag = () => WechatPayAesGcmCodec.TryDecrypt(
            key, nonce, ciphertext, corruptTag, aad, out _);
        Action actBadCipher = () => WechatPayAesGcmCodec.TryDecrypt(
            key, nonce, corruptCiphertext, tag, aad, out _);

        actBadTag.Should().NotThrow(
            "认证失败必须收敛为 false，抛出 CryptographicException 会把载荷细节带进异常消息（PAY-B7）");
        actBadCipher.Should().NotThrow();

        WechatPayAesGcmCodec.TryDecrypt(key, nonce, ciphertext, corruptTag, aad, out var plaintext)
            .Should().BeFalse("标签被改必须拒绝");
        plaintext.Should().BeNull("认证失败时缓冲区不可信，必须清零且不外泄");

        WechatPayAesGcmCodec.TryDecrypt(null, nonce, ciphertext, tag, aad, out _)
            .Should().BeFalse("密钥缺失必须拒绝");
        WechatPayAesGcmCodec.TryDecrypt(new byte[16], nonce, ciphertext, tag, aad, out _)
            .Should().BeFalse("密钥长度非 32 字节必须拒绝，不得降级为 AES-128");
        WechatPayAesGcmCodec.TryDecrypt(key, new byte[13], ciphertext, tag, aad, out _)
            .Should().BeFalse("nonce 长度非 12 字节必须拒绝");
        WechatPayAesGcmCodec.TryDecrypt(key, nonce, ciphertext, new byte[15], aad, out _)
            .Should().BeFalse("标签长度非 16 字节必须拒绝");
    }

    /// <summary>PAY-B4：入参校验抛出的异常**不得回显密钥或明文材料**（PAY-B7 同批断言）。</summary>
    [Fact]
    public void AesGcm_ShouldNotEchoKeyMaterialInExceptions_WhenKeyHasWrongLength()
    {
        var badKey = new byte[16];
        var plaintext = Convert.FromBase64String(WechatPayGoldenVectors.AesPlaintextB64);

        Action act = () => WechatPayAesGcmCodec.Encrypt(badKey, plaintext);

        var exception = act.Should().Throw<ArgumentException>().Which;

        exception.Message.Should().NotContain(Convert.ToBase64String(badKey),
            "异常消息不得回显密钥材料（会被日志/遥测采集）");
        exception.Message.Should().NotContain(Convert.ToBase64String(plaintext),
            "异常消息不得回显明文");
    }

    // ==================== PAY-B6：AOT 净零 ====================

    /// <summary>
    /// PAY-B6：支付线源码**无反射版 JSON 序列化**、**无反射发现**，RSA/AesGcm 全部 BCL 直调。
    /// </summary>
    /// <remarks>
    /// 本包目标含 <c>net8.0</c>/<c>net10.0</c> 且 <c>AotStrictMode=true</c>，
    /// 反射 <c>Serialize&lt;T&gt;</c>/<c>Deserialize&lt;T&gt;</c> 会在 AOT strict 步骤报 IL2026/IL3050；
    /// 但「能编译、只在 AOT 发布时炸」的形态必须**在源码层**就拦下，不能依赖门禁跑完全构建才发现。
    /// </remarks>
    [Fact]
    public void Source_ShouldAvoidReflectionJsonAndReflectionDiscovery_WhenAotStrict()
    {
        var files = CollectPaySourceFiles();
        files.Should().NotBeEmpty("支付线必须有源文件，否则本源码扫描空跑假绿（AGENTS §6 防枚举空跑纪律）");

        foreach (var file in files)
        {
            var code = StripComments(File.ReadAllText(file));
            var name = Path.GetFileName(file);

            code.Should().NotContain("JsonSerializer.Serialize<",
                $"{name}：AOT 下必须走 JsonTypeInfo，禁反射 Serialize<T>");
            code.Should().NotContain(".Deserialize<",
                $"{name}：AOT 下必须走 JsonTypeInfo，禁反射 Deserialize<T>");
            code.Should().NotContain("Activator.CreateInstance",
                $"{name}：反射实例化在 AOT/剪裁下不可用");
            code.Should().NotContain("Assembly.Load",
                $"{name}：按名加载程序集在 AOT 下不可用，且属硬编码 metadata name 反模式");
        }

        // 算法实现必须显式落在 System.Security.Cryptography（BCL 直调，无第三方密码学包）。
        var signatureProvider = File.ReadAllText(SourcePath(
            "Mud.Wechat.Pay.Abstractions", "Credential", "WechatPaySignatureProvider.cs"));
        signatureProvider.Should().Contain("System.Security.Cryptography",
            "RSA 签名必须 BCL 直调（PAY-B6），不得引入 BouncyCastle 等第三方包");

        var aesCodec = File.ReadAllText(SourcePath(
            "Mud.Wechat.Pay.Abstractions", "Credential", "WechatPayAesGcmCodec.cs"));
        aesCodec.Should().Contain("System.Security.Cryptography",
            "AesGcm 必须 BCL 直调（PAY-B6）");

        // 第三方密码学包一旦引入，AOT/剪裁与授权面都要重新评估 ⇒ 显式禁止。
        foreach (var csproj in Directory.GetFiles(Root, "Mud.Wechat.Pay*.csproj", SearchOption.AllDirectories)
                     .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                 && !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                 && !p.StartsWith(Path.Combine(Root, "Tests") + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)))
        {
            // 必须剔除 XML 注释：本仓 PAY-B9 的**说明注释里**就写了 "BouncyCastle"（记录被否决的方案 c），
            // 不剔除会把「文档提及」当成「引入了该包」⇒ 恒红且永远修不好。
            var xml = StripXmlComments(File.ReadAllText(csproj));
            foreach (var banned in new[] { "BouncyCastle", "Portable.BouncyCastle", "Org.BouncyCastle" })
            {
                xml.Should().NotContain(banned,
                    $"{Path.GetFileName(csproj)}：禁止第三方密码学包（PAY-B6，须走 BCL 直调）");
            }
        }
    }

    // ==================== PAY-B7：敏感项不入日志 ====================

    /// <summary>
    /// PAY-B7：凭据与密码学源码中，**敏感标识符不得出现在日志调用或插值异常消息**里。
    /// </summary>
    /// <remarks>
    /// 约束对象 = 私钥 / APIv3 密钥 / <c>Authorization</c> 头 / <c>resource.ciphertext</c> 明文。
    /// 这些值一旦进日志或异常消息，就会随日志平台、APM、错误上报二次扩散，
    /// 且**不可撤回**（密钥泄露无法「删日志」补救）。规则按「同一行同时出现敏感标识符与下沉点」判定：
    /// 下沉点 = 日志 API，或带插值字符串的 <c>throw</c>（<c>nameof(...)</c> 只是名字、不是值，故不配插值即不触发）。
    /// </remarks>
    [Fact]
    public void Source_ShouldNeverEmitSecretsIntoLogsOrExceptions_WhenScanned()
    {
        var files = CollectPaySourceFiles();
        files.Should().NotBeEmpty("支付线必须有源文件，否则本源码扫描空跑假绿");

        string[] secretMarkers =
        {
            "PrivateKey", "Secret", "ApiKey", "AesKey",
            "Authorization", "Ciphertext", "PlainText", "Plaintext",
        };

        string[] logSinks =
        {
            "ILogger", "LogTrace", "LogDebug", "LogInformation", "LogWarning",
            "LogError", "LogCritical", "Console.Write", "Debug.Write",
            "Trace.Write", "EventSource", "ActivitySource", "Telemetry",
        };

        foreach (var file in files)
        {
            var lines = StripComments(File.ReadAllText(file))
                .Split('\n')
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToArray();

            foreach (var line in lines)
            {
                var hasSecret = secretMarkers.Any(m =>
                    line.Contains(m, StringComparison.OrdinalIgnoreCase));

                if (!hasSecret)
                {
                    continue;
                }

                var logs = logSinks.Any(s => line.Contains(s, StringComparison.Ordinal));
                var throwsInterpolated = line.Contains("throw", StringComparison.Ordinal)
                                         && line.Contains("$\"", StringComparison.Ordinal);

                (logs || throwsInterpolated).Should().BeFalse(
                    $"{Path.GetFileName(file)}：敏感标识符不得进入日志或插值异常消息（PAY-B7）。\n> {line}");
            }
        }
    }

    /// <summary>PAY-B7：签名结果只携带**可公开**的两个字段（签名与序列号本就进明文头）。</summary>
    [Fact]
    public void SignatureResult_ShouldExposeNoKeyMaterial_WhenInspected()
    {
        var properties = typeof(WechatPaySignatureResult)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        properties.Should().Equal(new[] { "SerialNumber", "Signature" },
            "because 签名结果若多暴露任何字段，都可能是把私钥/密钥外带的口子");

        typeof(WechatPaySignatureProvider)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Should().BeEmpty("签名提供器不得以可读属性暴露私钥（私钥只在构造期注入，不落属性）");
    }

    // ---- helpers -------------------------------------------------------------

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }

    /// <summary>
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    /// <summary>支付线四个工程的源码目录（按 csproj 定位，不硬编码层级）。</summary>
    private static string[] PayProjectDirs()
        => new[]
        {
            "Mud.Wechat.Pay",
            "Mud.Wechat.Pay.Abstractions",
            "Mud.Wechat.Pay.DataModels",
            "Mud.Wechat.Pay.Callback",
        }.Select(SourceProjectDir).ToArray();

    /// <summary>支付线源码（不含 bin/obj 与测试工程）——PAY-B6/B7 源码扫描的枚举集。</summary>
    private static string[] CollectPaySourceFiles()
        => PayProjectDirs()
            .SelectMany(dir => Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories))
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

    /// <summary>剔除 <c>//</c>、<c>/* */</c> 与 XML 文档注释，避免「文档提及」被当成实现（AB-G9 已踩过此坑）。</summary>
    private static string StripComments(string text)
        => Regex.Replace(text, @"//.*?$|/\*.*?\*/", string.Empty,
            RegexOptions.Multiline | RegexOptions.Singleline);

    /// <summary>剔除 csproj 的 <c>&lt;!-- --&gt;</c> 块注释——被否决方案的**记录性提及**不得触发包禁令。</summary>
    private static string StripXmlComments(string text)
        => Regex.Replace(text, @"<!--.*?-->", string.Empty, RegexOptions.Singleline);

    private static string DecodeUtf8(string base64)
        => Encoding.UTF8.GetString(Convert.FromBase64String(base64));

    /// <summary>翻转 Base64 签名的末字节（保持长度不变 ⇒ 只会因内容不符被拒，而非因长度被拒）。</summary>
    private static string FlipLastByte(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        bytes[bytes.Length - 1] ^= 0xFF;
        return Convert.ToBase64String(bytes);
    }

    /// <summary>构造只装平台证书的存取端口（证书所有权交给缓存，由 <c>using</c> 释放）。</summary>
    private static WechatPayPlatformCertificateCache CreatePlatformStore()
    {
        var cache = new WechatPayPlatformCertificateCache();
        var certificate = X509Certificate2.CreateFromPem(WechatPayGoldenVectors.PlatformCertificatePem);
        cache.Set(WechatPayGoldenVectors.PlatformSerialNumber, certificate);
        return cache;
    }
}
