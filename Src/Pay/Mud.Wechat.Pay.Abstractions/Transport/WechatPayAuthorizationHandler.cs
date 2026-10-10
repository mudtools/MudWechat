// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Credential;

namespace Mud.Wechat.Pay.Abstractions.Transport;

/// <summary>
/// APIv3 请求签名 Handler：在请求<b>真正发出前</b>写入 <c>Authorization</c> 头。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须在这里签</b>：签名串含 <c>请求体原文</c> 与 <c>时间戳/随机串</c>，而这两个值又必须
/// 回填到同一个头里 ⇒ 只能在请求体<b>序列化完成之后</b>、<b>发出之前</b>计算。
/// 这也是支付线不能复用组件 <c>TokenInjectionMode</c>（见方案 §2.3 红框）的原因。
/// </para>
/// <para>
/// <b>商户从环境态取</b>（见 <see cref="Credential.IWechatPayMerchantContext"/>）：请求体里的
/// <c>mchid</c> 不能用来反推商户 —— 读体决定商户、再用该商户签名的顺序，与「签名串就是体原文」冲突。
/// </para>
/// <para>
/// <b>PAY-B7</b>：本类不记录任何日志，<c>Authorization</c> 头与签名串不出现在异常消息中。
/// </para>
/// </remarks>
internal sealed class WechatPayAuthorizationHandler : DelegatingHandler
{
    private readonly IWechatPayMerchantContext _merchantContext;
    private readonly IWechatPaySignatureProviderFactory _signatureProviders;

    /// <summary>
    /// 创建 Handler（由 DI 解析，随命名客户端的 Handler 管道每次发送时执行）。
    /// </summary>
    /// <param name="merchantContext">环境商户上下文。</param>
    /// <param name="signatureProviders">按商户缓存的签名提供者工厂。</param>
    public WechatPayAuthorizationHandler(
        IWechatPayMerchantContext merchantContext,
        IWechatPaySignatureProviderFactory signatureProviders)
    {
        _merchantContext = merchantContext ?? throw new ArgumentNullException(nameof(merchantContext));
        _signatureProviders = signatureProviders ?? throw new ArgumentNullException(nameof(signatureProviders));
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">请求缺少 <c>RequestUri</c>（无法构造签名用规范 URL）。</exception>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // 幂等放行：重试/转发若已带 Authorization 就不重签 —— 重签会产生新的时间戳与随机串，
        // 与调用方已按旧签名取证（或已发出）的请求不一致。
        if (request.Headers.Contains("Authorization"))
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        // 商户在读体**之前**就确定：见类注释「读体决定商户」不可行。
        var merchant = _merchantContext.ResolveCurrent();

        string? body = null;
        if (request.Content != null)
        {
            // 签名串取的是请求体**原文**，且签名后同一份内容还要发出去。
            // 流式内容读一次即耗尽 ⇒ 先入缓冲区，保证「读出来签」与「随后发送」是同一份字节。
            await request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            body = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        var requestUri = request.RequestUri
            ?? throw new InvalidOperationException("请求缺少 RequestUri，无法构造签名用规范 URL。");

        var canonicalUrl = WechatPaySignatureMessages.BuildCanonicalUrl(
            requestUri.AbsolutePath, StripLeadingQuestionMark(requestUri.Query));

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var nonce = WechatPaySignatureMessages.CreateNonce();

        var message = WechatPaySignatureMessages.BuildRequestMessage(
            request.Method.Method, canonicalUrl, timestamp, nonce, body);

        var provider = await _signatureProviders
            .GetOrCreateAsync(merchant, cancellationToken)
            .ConfigureAwait(false);

        var signature = provider.Sign(message);

        var authorization = WechatPaySignatureMessages.BuildAuthorization(
            merchant.AuthorizationMchId, signature.SerialNumber, timestamp, nonce, signature.Signature);

        request.Headers.TryAddWithoutValidation("Authorization", authorization);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 去掉查询串前导 <c>?</c>：<see cref="WechatPaySignatureMessages.BuildCanonicalUrl"/> 自己会补问号，
    /// 而 <see cref="Uri.Query"/> 本身就带问号 —— 直接透传会得到 <c>??a=b</c> 并逐字节偏离官方签名串。
    /// </summary>
    private static string StripLeadingQuestionMark(string query)
        => query.Length > 0 && query[0] == '?' ? query.Substring(1) : query;
}
