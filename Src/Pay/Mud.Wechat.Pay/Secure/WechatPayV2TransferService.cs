// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2TransferService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>域名与红包族同为 <c>api.mch</c></b>：官方给企业付款族的域名是
/// <see cref="WechatPayV2HttpClientNames.BaseAddress"/> 而非 <c>api2.mch</c> ——
/// 「要证书」与「在证书域名上」是两件独立的事，本族的组合是「要证书 + 普通域名」。
/// </para>
/// <para>
/// <b>双签只有一支</b>：<see cref="PayWorkWxPocketAsync"/> 走
/// <see cref="WechatPayV2WorkWxSigner.PayPocketWhitelist"/>，其余三支只算 <c>sign</c>。
/// </para>
/// <para><b>出资操作</b>：本类不做任何自动重试 —— 幂等键 <c>partner_trade_no</c> 的重试策略属宿主业务
/// （换单号即重复付款），SDK 只把官方应答原样交还。</para>
/// <para><b>不出敏感信息</b>（PAY-B7）：异常文本不含密钥、<c>sign</c>、<c>openid</c> 或 <c>risk_info</c>。</para>
/// </remarks>
internal sealed class WechatPayV2TransferService : IWechatPayV2TransferService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建企业付款侧服务（由 DI 解析）。</summary>
    public WechatPayV2TransferService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2TransferToBalanceResponse> TransferToBalanceAsync(
        V2TransferToBalanceRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2TransferToBalanceRequest, V2TransferToBalanceResponse>(
            "/mmpaymkttransfers/promotion/transfers",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2GetTransferInfoResponse> GetTransferInfoAsync(
        V2GetTransferInfoRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2GetTransferInfoRequest, V2GetTransferInfoResponse>(
            "/mmpaymkttransfers/gettransferinfo",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2PayWorkWxPocketResponse> PayWorkWxPocketAsync(
        V2PayWorkWxPocketRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostWorkWxAsync<V2PayWorkWxPocketRequest, V2PayWorkWxPocketResponse>(
            "/mmpaymkttransfers/promotion/paywwsptrans2pocket",
            WechatPayV2HttpClientNames.BaseAddress, request,
            WechatPayV2WorkWxSigner.PayPocketWhitelist, cancellationToken);

    /// <inheritdoc />
    public Task<V2QueryWorkWxPocketResponse> QueryWorkWxPocketAsync(
        V2QueryWorkWxPocketRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2QueryWorkWxPocketRequest, V2QueryWorkWxPocketResponse>(
            "/mmpaymkttransfers/promotion/querywwsptrans2pocket",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}
