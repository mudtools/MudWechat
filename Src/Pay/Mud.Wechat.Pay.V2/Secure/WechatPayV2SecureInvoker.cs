// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.Wechat.Pay.V2.Credential;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// APIv2 <b>需商户证书</b>端点的通用收发器：字段表签名 → 逐商户证书通道 POST → 应答验签。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要抽这一层</b>：需证书的端点不止 <c>/secapi/pay/*</c> 一族 ——
/// 现金红包（<c>/mmpaymkttransfers/*</c>）、企业付款到零钱与付款到银行卡（<c>/mmpaysptrans/*</c>）
/// 同样要求 TLS 握手期呈现商户证书，且它们挂在 <c>api.mch</c> 域名而非 <c>api2.mch</c>。
/// 报文规则（XML、字段表签名、应答双试验签、空值不入签名串）在各族之间<b>完全同一</b>，
/// 唯一差异是路径与域名 ⇒ 收成一个引擎、各族服务只做「路径 + 域名 + DTO」的薄映射，
/// 避免同一套签名/验签代码在五个服务里各抄一遍（抄一遍就多一处漏验签的可能）。
/// </para>
/// <para>
/// <b>免反射</b>：序列化复用 <see cref="WechatPayV2ContentSerializer"/> 的源生成元数据路径，
/// 判定复用 <see cref="WechatPayV2Signer"/>，与声明式管道（<see cref="WechatPayV2SigningHandler"/>）
/// 同源 ⇒ 两条路产出的签名必然一致。
/// </para>
/// <para>
/// <b>不出密钥</b>（PAY-B7 同线红线）：异常消息只含商户键、域名、路由与官方返回字段名，
/// 绝不包含密钥值、<c>sign</c> 值、签名原文串或证书材料。
/// </para>
/// </remarks>
internal sealed class WechatPayV2SecureInvoker
{
    private readonly IWechatPayMerchantContext _merchantContext;
    private readonly IWechatPayV2CredentialProvider _credentials;
    private readonly WechatPayV2SecureChannelProvider _channels;
    private readonly WechatPayV2ContentSerializer _serializer;

    /// <summary>
    /// 惰性解析<b>可选</b>端口用（<see cref="IWechatPayV2WorkWxSecretProvider"/>）。
    /// 收发器本身是 Singleton，走 <c>IServiceProvider.GetService</c> 而非构造期强依赖：
    /// 未使用双签端点的宿主不该被强制注册企微凭据桥接。
    /// </summary>
    private readonly IServiceProvider _services;

    /// <summary>按商户键缓存的 APIv2 签名密钥（与签名 Handler 同口径：轮换需重启进程）。</summary>
    private readonly ConcurrentDictionary<string, string> _keys = new(StringComparer.Ordinal);

    /// <summary>创建通用收发器（由 DI 解析，随 APIv2 底座注册）。</summary>
    public WechatPayV2SecureInvoker(
        IWechatPayMerchantContext merchantContext,
        IWechatPayV2CredentialProvider credentials,
        WechatPayV2SecureChannelProvider channels,
        WechatPayV2ContentSerializer serializer,
        IServiceProvider services)
    {
        _merchantContext = merchantContext ?? throw new ArgumentNullException(nameof(merchantContext));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _channels = channels ?? throw new ArgumentNullException(nameof(channels));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>签名后 POST，返回<b>应答原文</b>（下载族与「原样交回调用方判别」的出口用此重载）。</summary>
    /// <param name="path">官方路径（相对域名，代码字面量）。</param>
    /// <param name="baseAddress">目标域名，取 <see cref="WechatPayV2HttpClientNames.BaseAddress"/> 或 <see cref="WechatPayV2HttpClientNames.SecureBaseAddress"/>。</param>
    /// <param name="request">请求 DTO。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public Task<string> SendAsync<TRequest>(
        string path, string baseAddress, TRequest request, CancellationToken cancellationToken)
        => SendInternalAsync(path, baseAddress, request, workWxWhitelist: null, cancellationToken);

    /// <summary>
    /// <b>双签</b>出口：先按官方白名单算 <c>workwx_sign</c> 入表，再算 <c>sign</c>（官方明确
    /// <c>workwx_sign</c> 参与 <c>sign</c>）⇒ 顺序由本方法固化，调用方无从颠倒。
    /// </summary>
    /// <param name="path">官方路径。</param>
    /// <param name="baseAddress">目标域名。</param>
    /// <param name="request">请求 DTO（其 <c>agentid</c> 决定取哪个企微应用的 Secret）。</param>
    /// <param name="workWxWhitelist">该端点的官方固定白名单，取 <see cref="WechatPayV2WorkWxSigner"/> 的两组常量。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public Task<string> SendWorkWxAsync<TRequest>(
        string path, string baseAddress, TRequest request,
        IReadOnlyCollection<string> workWxWhitelist, CancellationToken cancellationToken)
        => SendInternalAsync(path, baseAddress, request, workWxWhitelist, cancellationToken);

    /// <summary>签名后 POST 并把 XML 应答反序列化成 DTO。</summary>
    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path, string baseAddress, TRequest request, CancellationToken cancellationToken)
    {
        var text = await SendAsync(path, baseAddress, request, cancellationToken).ConfigureAwait(false);
        return _serializer.Deserialize<TResponse>(text);
    }

    /// <summary><see cref="SendWorkWxAsync{TRequest}"/> 的「反序列化成 DTO」出口。</summary>
    public async Task<TResponse> PostWorkWxAsync<TRequest, TResponse>(
        string path, string baseAddress, TRequest request,
        IReadOnlyCollection<string> workWxWhitelist, CancellationToken cancellationToken)
    {
        var text = await SendWorkWxAsync(path, baseAddress, request, workWxWhitelist, cancellationToken)
            .ConfigureAwait(false);
        return _serializer.Deserialize<TResponse>(text);
    }

    /// <summary>
    /// <b>multipart/form-data</b> 出口（商户媒体上传一族用）：字段表照常签名后逐个落成表单分节，
    /// 文件作为<b>独立分节</b>附加 ⇒ 天然<b>不参与</b> <c>sign</c>（官方口径「文件内容不参与签名」）。
    /// </summary>
    /// <remarks>
    /// 应答仍是 <b>XML</b>（官方本页应答格式与请求格式不同：请求 multipart、应答 XML），
    /// 故收尾复用 <see cref="VerifyResponse"/> + <see cref="WechatPayV2ContentSerializer.Deserialize{T}"/>，
    /// 与 XML 出口同一套验签口径 —— 不因请求换了载体就把应答验签漏掉。
    /// </remarks>
    /// <param name="path">官方路径。</param>
    /// <param name="baseAddress">目标域名。</param>
    /// <param name="request">请求 DTO（其字段即参与签名的表单分节）。</param>
    /// <param name="fileFieldName">文件分节的字段名（官方原文拼写）。</param>
    /// <param name="fileContent">文件内容流（由调用方拥有，本方法不关闭）。</param>
    /// <param name="fileName">文件名（表单分节的 filename 属性）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public async Task<TResponse> PostMultipartAsync<TRequest, TResponse>(
        string path, string baseAddress, TRequest request,
        string fileFieldName, Stream fileContent, string fileName,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (fileContent is null)
        {
            throw new ArgumentNullException(nameof(fileContent));
        }

        var merchant = _merchantContext.ResolveCurrent();
        var apiKey = await GetKeyAsync(merchant, cancellationToken).ConfigureAwait(false);

        var fields = _serializer.ToFields(request);
        if (fields.Count == 0)
        {
            throw new InvalidOperationException(
                $"{merchant.MerchantKey}：APIv2 {path} 的表单字段序列化后为空，无法签名。");
        }

        fields.Add(new KeyValuePair<string, string?>(
            WechatPayV2SignatureBuilder.SignFieldName, WechatPayV2Signer.Sign(fields, apiKey)));

        var channel = await _channels.GetChannelAsync(baseAddress, cancellationToken).ConfigureAwait(false);

        using var content = new MultipartFormDataContent();
        foreach (var field in fields)
        {
            // 空值既不参与签名（WechatPayV2SignatureBuilder 的既有口径），也不该出现在表单里，
            // 否则官方按「分节存在」判定该字段已上传，与签名串覆盖的字段集不一致。
            if (string.IsNullOrEmpty(field.Value))
            {
                continue;
            }

            content.Add(new StringContent(field.Value!), field.Key);
        }

        var filePart = new StreamContent(fileContent);
        filePart.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        content.Add(filePart, fileFieldName, fileName);

        using var response = await channel.HttpClient.PostAsync(path, content, cancellationToken)
            .ConfigureAwait(false);

        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"{merchant.MerchantKey}：APIv2 {baseAddress}{path} 返回 HTTP {(int)response.StatusCode}，" +
                $"应答首 512 字符：{Truncate(text)}。");
        }

        VerifyResponse(merchant, apiKey, path, text);

        return _serializer.Deserialize<TResponse>(text);
    }

    private async Task<string> SendInternalAsync<TRequest>(
        string path, string baseAddress, TRequest request,
        IReadOnlyCollection<string>? workWxWhitelist, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var merchant = _merchantContext.ResolveCurrent();
        var apiKey = await GetKeyAsync(merchant, cancellationToken).ConfigureAwait(false);

        var fields = _serializer.ToFields(request);
        if (fields.Count == 0)
        {
            throw new InvalidOperationException(
                $"{merchant.MerchantKey}：APIv2 请求序列化后字段表为空，无法对 {path} 签名。" +
                "请检查请求对象是否至少填了必填字段。");
        }

        if (workWxWhitelist is not null)
        {
            await AppendWorkWxSignatureAsync(merchant, path, fields, workWxWhitelist, cancellationToken)
                .ConfigureAwait(false);
        }

        fields.Add(new KeyValuePair<string, string?>(
            WechatPayV2SignatureBuilder.SignFieldName, WechatPayV2Signer.Sign(fields, apiKey)));

        var channel = await _channels.GetChannelAsync(baseAddress, cancellationToken).ConfigureAwait(false);

        using var content = new StringContent(
            WechatPayV2XmlCodec.WriteXml(fields), Encoding.UTF8, WechatPayV2ContentSerializer.XmlMediaType);
        using var response = await channel.HttpClient.PostAsync(path, content, cancellationToken)
            .ConfigureAwait(false);

        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"{merchant.MerchantKey}：APIv2 {baseAddress}{path} 返回 HTTP {(int)response.StatusCode}，" +
                $"应答首 512 字符：{Truncate(text)}。");
        }

        VerifyResponse(merchant, apiKey, path, text);

        return text;
    }

    /// <summary>
    /// 追加 <c>workwx_sign</c>：Secret 经 <see cref="IWechatPayV2WorkWxSecretProvider"/> 取（可选注册），
    /// 取不到即 fail-fast —— 单签报文对这两支端点必然被拒，放行只会把配置缺失伪装成 SIGNERROR。
    /// </summary>
    private async Task AppendWorkWxSignatureAsync(
        WechatPayMerchantConfig merchant, string path,
        List<KeyValuePair<string, string?>> fields,
        IReadOnlyCollection<string> whitelist, CancellationToken cancellationToken)
    {
        // 幂等放行：调用方已自行签好则不重签（与声明式 Handler 对 sign 的口径一致）。
        if (WechatPayV2WorkWxSigner.HasWorkWxSignature(fields))
        {
            return;
        }

        var provider = _services.GetService<IWechatPayV2WorkWxSecretProvider>();
        if (provider is null)
        {
            throw new InvalidOperationException(
                $"{merchant.MerchantKey}：APIv2 {path} 需要企业微信侧签名 workwx_sign，" +
                $"但容器未注册 {nameof(IWechatPayV2WorkWxSecretProvider)}。" +
                "请由宿主注册该端口（把 agentid 换成对应企业微信自建应用的 Secret 后返回）。");
        }

        var agentId = fields.FirstOrDefault(f =>
            string.Equals(f.Key, "agentid", StringComparison.OrdinalIgnoreCase)).Value;

        var secret = await provider.GetSecretAsync(agentId, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException(
                $"{merchant.MerchantKey}：APIv2 {path} 未取得 workwx_sign 所需的企业微信应用 Secret" +
                $"（agentid 见请求，端口返回空）。异常消息不回显任何凭据内容。");
        }

        fields.Add(new KeyValuePair<string, string?>(
            WechatPayV2SignatureBuilder.WorkWxSignFieldName,
            WechatPayV2WorkWxSigner.Sign(fields, whitelist, secret)));
    }

    /// <summary>校验应答 <c>sign</c>；CSV 出口与非 XML 报文无 <c>sign</c>，直接放行。</summary>
    private static void VerifyResponse(
        WechatPayMerchantConfig merchant, string apiKey, string path, string text)
    {
        var fields = WechatPayV2XmlCodec.ReadFields(text);
        if (fields.Count == 0 || !WechatPayV2Signer.HasSignature(fields))
        {
            return;
        }

        if (!WechatPayV2Signer.Verify(fields, apiKey))
        {
            throw new InvalidOperationException(
                $"{merchant.MerchantKey}：APIv2 {path} 应答验签失败（sign 与本地按同一密钥重算的结果不一致）。" +
                "可能成因：应答被中间层改写、或 APIv2 密钥与 APIv3 密钥不同源却被配成了同一个" +
                "（配置面须区分 ApiV2KeySecretName 与 ApiKeySecretName）。");
        }
    }

    private async Task<string> GetKeyAsync(
        WechatPayMerchantConfig merchant, CancellationToken cancellationToken)
    {
        if (_keys.TryGetValue(merchant.MerchantKey, out var cached))
        {
            return cached;
        }

        var apiKey = await _credentials.LoadApiV2KeyAsync(merchant, cancellationToken).ConfigureAwait(false);
        _keys[merchant.MerchantKey] = apiKey;
        return apiKey;
    }

    private static string Truncate(string value)
        => value.Length <= 512 ? value : value.Substring(0, 512) + "...";
}
