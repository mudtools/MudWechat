// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2FundService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写</b>：官方「是否需要证书」为「需要。详见证书使用。」，错误码表另有 <c>CA_ERROR</c>
/// ⇒ 证书按商户绑定，声明式管道无从拿到。
/// </para>
/// <para>
/// <b>域名是 <c>api.mch</c></b>：「要证书」与「挂在证书域名上」是两个互相独立的官方事实，本支属前者
/// （与 <see cref="IWechatPayV2RiskService"/> 同形）⇒ 通道按 (商户, 域名) 双键缓存，
/// 与同一商户的 <c>api2.mch</c> 通道互不复用连接。
/// </para>
/// <para>
/// <b>应答带签</b>：官方返回表同时给出 <c>sign</c> 与 <c>sign_type</c> ⇒ 走 <c>PostAsync</c> 的常规验签，
/// 与反欺诈族「应答无签即放行」是同一出口的两个不同报文事实。
/// </para>
/// </remarks>
internal sealed class WechatPayV2FundService : IWechatPayV2FundService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建资金侧服务（由 DI 解析）。</summary>
    public WechatPayV2FundService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2AutoWithdrawQueryResponse> QueryAutoWithdrawByDateAsync(
        V2AutoWithdrawQueryRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2AutoWithdrawQueryRequest, V2AutoWithdrawQueryResponse>(
            "/fund/queryautowithdrawbydate",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}
