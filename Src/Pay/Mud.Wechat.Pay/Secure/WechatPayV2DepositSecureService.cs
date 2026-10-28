// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2DepositSecureService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>域名是 <c>api.mch</c> 而非 <c>api2.mch</c></b>：押金族三支需证书端点的官方 URL
/// 全部落在 <see cref="WechatPayV2HttpClientNames.BaseAddress"/> 上，与 <c>/secapi/pay/*</c> 一族不同 ⇒
/// 本类显式传 <c>BaseAddress</c>，不复用 <see cref="WechatPayV2SecureService"/> 所用的证书域名。
/// </para>
/// <para>
/// <b>与免证书五支的分工</b>：同一域内 <c>/deposit/unifiedorder</c> 等五支走声明式生成，
/// 本三支走 <see cref="WechatPayV2SecureInvoker"/>（证书按商户绑定，声明式管道拿不到逐商户证书）。
/// 签名与验签口径两者同源，故不存在「同一笔业务换条路结果不同」的面。
/// </para>
/// <para><b>出资语义</b>：<c>/deposit/refund</c> 是退款（出资）操作 ⇒ SDK 不自动重试，
/// 换 <c>out_refund_no</c> 即重复退款；重试必须复用原单号（官方口径见接口注释）。</para>
/// <para><b>不出敏感信息</b>（PAY-B7 同线红线）：异常文本不含密钥、<c>sign</c>、签名原文串或证书材料；
/// <c>openid</c> / <c>sub_openid</c> 属用户身份，SDK 不落日志。</para>
/// </remarks>
internal sealed class WechatPayV2DepositSecureService : IWechatPayV2DepositSecureService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建押金需证书侧服务（由 DI 解析）。</summary>
    public WechatPayV2DepositSecureService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2DepositReverseResponse> ReverseAsync(
        V2DepositReverseRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2DepositReverseRequest, V2DepositReverseResponse>(
            "/deposit/reverse",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2DepositConsumeResponse> ConsumeAsync(
        V2DepositConsumeRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2DepositConsumeRequest, V2DepositConsumeResponse>(
            "/deposit/consume",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2DepositRefundResponse> RefundAsync(
        V2DepositRefundRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2DepositRefundRequest, V2DepositRefundResponse>(
            "/deposit/refund",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}
