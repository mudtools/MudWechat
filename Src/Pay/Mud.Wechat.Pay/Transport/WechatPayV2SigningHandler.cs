// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.V2.Credential;

namespace Mud.Wechat.Pay.V2.Transport;

/// <summary>
/// APIv2 签名 Handler：请求发出前把 <c>sign</c> 追加进 XML 报文体，应答回来后校验应答 <c>sign</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么签在 Handler 而不是序列化器里</b>：<c>sign</c> 的作用域是<b>报文体的全部非空顶层字段</b>
/// （含 <c>sign_type</c>，不含 <c>sign</c> 自身），所以必须在报文体<b>组装完成之后</b>计算；
/// 而密钥出自 <c>ISecretProvider</c>（异步），<c>ToHttpContent&lt;T&gt;</c> 却是同步方法。
/// 两难在 Handler 里同时解开：这里既拿得到已成形的体，又处在异步管道上。
/// 与 APIv3 把 <c>Authorization</c> 头放在 <c>WechatPayAuthorizationHandler</c> 是同一分工。
/// </para>
/// <para>
/// <b>商户取自环境态</b>（<see cref="IWechatPayMerchantContext.ResolveCurrent"/>）：与 APIv3 同理由 ——
/// 「先读体里的 mch_id 再决定用谁的密钥签」与「签的就是这份体」互相矛盾，且服务商形态下体里还有 sub_mchid。
/// </para>
/// <para>
/// <b>算法判定与应答双试</b>统一收在 <see cref="Security.WechatPayV2Signer"/>，
/// 与双向证书通道（<c>secapi</c> 侧手动收发）共用同一条规则。
/// </para>
/// <para>
/// <b>密钥按商户缓存</b>：APIv2 签名密钥是静态凭据，逐请求回源配置中心只会把每笔交易拖慢一次 IO。
/// 代价是<b>密钥轮换需重启进程</b>才生效（与 APIv3 按商户缓存签名提供者同一口径）。
/// </para>
/// <para>
/// <b>不记录任何密钥、签名串或报文原文</b>（PAY-B7 同线红线）；异常消息只给商户键与字段名。
/// </para>
/// </remarks>
internal sealed class WechatPayV2SigningHandler : DelegatingHandler
{
    private readonly IWechatPayMerchantContext _merchantContext;
    private readonly IWechatPayV2CredentialProvider _credentials;

    /// <summary>按商户键缓存的 APIv2 签名密钥（静态凭据，见类注释的轮换口径）。</summary>
    private readonly ConcurrentDictionary<string, string> _keys = new(StringComparer.Ordinal);

    /// <summary>创建 Handler（由 DI 解析，挂在 APIv2 命名客户端的管道上）。</summary>
    /// <param name="merchantContext">环境商户上下文。</param>
    /// <param name="credentials">APIv2 凭据取用端口。</param>
    public WechatPayV2SigningHandler(
        IWechatPayMerchantContext merchantContext, IWechatPayV2CredentialProvider credentials)
    {
        _merchantContext = merchantContext ?? throw new ArgumentNullException(nameof(merchantContext));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var signed = await SignRequestAsync(request, cancellationToken).ConfigureAwait(false);

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        // 只有请求侧确实签过名才校验应答（CSV 下载等非 XML 出口没有 sign 可比对）。
        if (signed && response.Content is not null && response.IsSuccessStatusCode)
        {
            await VerifyResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    /// <summary>把 <c>sign</c> 追加进出站报文；返回是否确实执行了签名。</summary>
    private async Task<bool> SignRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => request.Content is not null
            ? await SignBodyAsync(request, cancellationToken).ConfigureAwait(false)
            : await SignQueryAsync(request, cancellationToken).ConfigureAwait(false);

    /// <summary>对<b>无报文体</b>的出站请求（GET + Query 传参）签名，把 <c>sign</c> 追加进查询串。</summary>
    /// <remarks>
    /// <para>
    /// <b>为什么必须支持</b>：APIv2 的 H5 预签约（<c>/papay/h5entrustweb</c>、
    /// <c>/papay/partner/h5entrustweb</c>）官方就是 <b>GET</b>，参数全部在查询串里，
    /// 且 <c>sign</c> 同样以<b>查询参数</b>呈现。只签体 ⇒ 这两支会把未签名报文发出去，
    /// 官方回 <c>SIGNERROR</c>（症状在远端，本地无任何提示）。
    /// </para>
    /// <para>
    /// <b>参与签名的值取解码后的原文</b>：签名串是 <c>k=v&amp;</c> 拼接，微信侧比对的是参数值本身
    /// 而非其百分号编码形态；若拿编码值参与签名，含中文的 <c>redirect_url</c> 必然对不上。
    /// </para>
    /// </remarks>
    private async Task<bool> SignQueryAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri;
        if (uri is null || string.IsNullOrEmpty(uri.Query))
        {
            return false;
        }

        var fields = ReadQuery(uri.Query);
        if (fields.Count == 0 || WechatPayV2Signer.HasSignature(fields))
        {
            return false;
        }

        var merchant = _merchantContext.ResolveCurrent();
        var apiKey = await GetKeyAsync(merchant, cancellationToken).ConfigureAwait(false);

        fields.Add(new KeyValuePair<string, string?>(
            WechatPayV2SignatureBuilder.SignFieldName, WechatPayV2Signer.Sign(fields, apiKey)));

        request.RequestUri = new UriBuilder(uri)
        {
            Query = WriteQuery(fields),
        }.Uri;

        return true;
    }

    /// <summary>解析查询串为字段表（值取 <c>Uri.UnescapeDataString</c> 后的原文，空值保留给排序侧剔除）。</summary>
    private static List<KeyValuePair<string, string?>> ReadQuery(string query)
    {
        var trimmed = query.Length > 0 && query[0] == '?' ? query.Substring(1) : query;
        if (trimmed.Length == 0)
        {
            return new List<KeyValuePair<string, string?>>();
        }

        var fields = new List<KeyValuePair<string, string?>>();
        foreach (var pair in trimmed.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            var name = separator < 0
                ? Uri.UnescapeDataString(pair)
                : Uri.UnescapeDataString(pair.Substring(0, separator));
            if (name.Length == 0)
            {
                continue;
            }

            fields.Add(new KeyValuePair<string, string?>(
                name, separator < 0 ? null : Uri.UnescapeDataString(pair.Substring(separator + 1))));
        }

        return fields;
    }

    /// <summary>把字段表写回查询串（每个值单独百分号编码；<c>sign</c> 恒置末位，与报文侧同一约定）。</summary>
    private static string WriteQuery(IReadOnlyList<KeyValuePair<string, string?>> fields)
    {
        var parts = new string[fields.Count];
        var index = 0;
        foreach (var field in fields)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                parts[index++] = Uri.EscapeDataString(field.Key) + "=" + Uri.EscapeDataString(field.Value!);
            }
        }

        return string.Join("&", parts, 0, index);
    }

    /// <summary>对<b>有报文体</b>的出站请求（POST + XML）签名，把 <c>sign</c> 追加进体末尾。</summary>
    private async Task<bool> SignBodyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 体要先入缓冲区：读出来签的那一份与随后发出去的那一份必须是同一份字节。
        await request.Content!.LoadIntoBufferAsync().ConfigureAwait(false);
        var body = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var fields = WechatPayV2XmlCodec.ReadFields(body);
        if (fields.Count == 0)
        {
            return false;
        }

        // 幂等放行：重试 / 内部转发若已带 sign 就不重签（字段表口径一旦有差就会产出
        // 「体与 sign 不一致」的自我矛盾报文，宁可不碰）。
        if (WechatPayV2Signer.HasSignature(fields))
        {
            return false;
        }

        var merchant = _merchantContext.ResolveCurrent();
        var apiKey = await GetKeyAsync(merchant, cancellationToken).ConfigureAwait(false);

        var signature = WechatPayV2Signer.Sign(fields, apiKey);

        // 官方示例把 sign 放末位；顺序不参与签名（签名串自己排序），仅为抓包比对方便。
        fields.Add(new KeyValuePair<string, string?>(
            WechatPayV2SignatureBuilder.SignFieldName, signature));

        request.Content = new StringContent(
            WechatPayV2XmlCodec.WriteXml(fields), Encoding.UTF8, WechatPayV2ContentSerializer.XmlMediaType);

        return true;
    }

    /// <summary>校验应答 <c>sign</c>；不带 sign 的应答（如纯 <c>return_code</c> 失败报文）直接放行。</summary>
    private async Task VerifyResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await response.Content.LoadIntoBufferAsync().ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var fields = WechatPayV2XmlCodec.ReadFields(text);
        if (fields.Count == 0 || !WechatPayV2Signer.HasSignature(fields))
        {
            return;
        }

        var merchant = _merchantContext.ResolveCurrent();
        var apiKey = await GetKeyAsync(merchant, cancellationToken).ConfigureAwait(false);

        if (!WechatPayV2Signer.Verify(fields, apiKey))
        {
            throw new InvalidOperationException(
                $"{merchant.MerchantKey}：APIv2 应答验签失败（sign 与本地按同一密钥重算的结果不一致）。" +
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
}
