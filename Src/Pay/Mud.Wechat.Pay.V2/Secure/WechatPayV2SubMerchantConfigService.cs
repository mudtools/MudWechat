// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2SubMerchantConfigService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写</b>：三页官方「是否需要证书」栏均为「是」⇒ 证书按商户绑定，声明式管道无从拿到。
/// </para>
/// <para>
/// <b>域名是 <c>api.mch</c></b>：三页「接口说明」栏给的是 <c>api.mch.weixin.qq.com/secapi/mch/*</c> 与
/// <c>/secapi/mkt/*</c>（逐页核验 4014115404 / 4014115413 / 4014115399）⇒ 取
/// <see cref="WechatPayV2HttpClientNames.BaseAddress"/>，<b>不</b>用 <see cref="WechatPayV2HttpClientNames.SecureBaseAddress"/>：
/// <c>/secapi/</c> 前缀只说明「要证书」，不说明「挂在证书域名上」。
/// </para>
/// <para>
/// <b>签名面差异不在此收口</b>：目录两支无 <c>nonce_str</c>／<c>sign_type</c>（⇒ 默认 MD5），
/// 关注配置支的 <c>sign_type</c> 由 DTO 默认值钉成 HMAC-SHA256。差异全部体现在 DTO 字段面上，
/// <see cref="WechatPayV2Signer"/> 按字段表里是否出现 <c>sign_type=HMAC-SHA256</c> 自动选算法 ⇒
/// 本类不做任何分支，也就无从把某一支的算法写错。
/// </para>
/// <para>
/// <b>列表原样交回</b>：<c>jsapi_path_list</c>／<c>appid_config_list</c> 是装在 XML 字符串字段里的 JSON，
/// 本类不解析、不回显（PAY-B7 同线口径）。
/// </para>
/// </remarks>
internal sealed class WechatPayV2SubMerchantConfigService : IWechatPayV2SubMerchantConfigService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建子商户配置服务（由 DI 解析）。</summary>
    public WechatPayV2SubMerchantConfigService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2SubDevConfigAddResponse> AddSubDevConfigAsync(
        V2SubDevConfigAddRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2SubDevConfigAddRequest, V2SubDevConfigAddResponse>(
            "/secapi/mch/addsubdevconfig",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2SubDevConfigQueryResponse> QuerySubDevConfigAsync(
        V2SubDevConfigQueryRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2SubDevConfigQueryRequest, V2SubDevConfigQueryResponse>(
            "/secapi/mch/querysubdevconfig",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2RecommendConfigAddResponse> AddRecommendConfigAsync(
        V2RecommendConfigAddRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2RecommendConfigAddRequest, V2RecommendConfigAddResponse>(
            "/secapi/mkt/addrecommendconf",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}
