// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2FraudService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写</b>：官方「是否需要证书」为「是」，而证书按商户绑定 ⇒ 声明式管道无从拿到。
/// </para>
/// <para>
/// <b>域名是 <c>fraud.mch</c></b>：本支是本线唯一挂在 <see cref="WechatPayV2HttpClientNames.FraudBaseAddress"/>
/// 上的端点。同一 <c>/risk/</c> 前缀的违规记录查询挂在 <c>api.mch</c> ⇒ 前缀不能推域名，
/// 通道按 (商户, 域名) 双键缓存正是为这种跨域复用准备的。
/// </para>
/// <para>
/// <b>应答无签</b>：官方返回表没有 <c>sign</c> 行，<c>PostAsync</c> 的应答验签在「应答不带 sign」时放行 ⇒
/// 本支走同一条出口而不需要任何特判（放行条件是报文事实，不是本族的例外声明）。
/// </para>
/// </remarks>
internal sealed class WechatPayV2FraudService : IWechatPayV2FraudService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建防欺诈侧服务（由 DI 解析）。</summary>
    public WechatPayV2FraudService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2GetPublicKeyResponse> GetPublicKeyAsync(
        V2GetPublicKeyRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2GetPublicKeyRequest, V2GetPublicKeyResponse>(
            "/risk/getpublickey",
            WechatPayV2HttpClientNames.FraudBaseAddress, request, cancellationToken);
}
