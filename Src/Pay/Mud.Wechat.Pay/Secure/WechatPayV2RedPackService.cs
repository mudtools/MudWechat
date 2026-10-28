// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2RedPackService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>域名口径</b>：本族五支官方全部给的是 <see cref="WechatPayV2HttpClientNames.BaseAddress"/>
/// （<c>api.mch</c>）而<b>非</b> <c>api2.mch</c> —— 与 <see cref="WechatPayV2SecureService"/>
/// 的 <c>/secapi/pay/*</c> 五支分属两个域名，「都要证书」不等于「都走证书域名」。
/// 通道提供者按「商户 + 域名」缓存，故同一商户在这里是<b>另一条</b>独立通道。
/// </para>
/// <para>
/// <b>双签只有一支</b>：仅 <see cref="SendWorkWxRedPackAsync"/> 需 <c>workwx_sign</c>，
/// 白名单与「先 <c>workwx_sign</c> 后 <c>sign</c>」的顺序由
/// <see cref="WechatPayV2WorkWxSigner.SendRedPackWhitelist"/> 与收发器固化，本类只负责选它。
/// 查询族三支不传白名单 ⇒ 未注册企微 Secret 端口的宿主也能正常发放普通红包与查询。
/// </para>
/// <para><b>不出敏感信息</b>（PAY-B7）：异常文本只含商户键、路由与官方字段名，
/// 不含密钥、<c>sign</c>、<c>re_openid</c> 或 <c>risk_info</c>。</para>
/// </remarks>
internal sealed class WechatPayV2RedPackService : IWechatPayV2RedPackService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建红包侧服务（由 DI 解析）。</summary>
    public WechatPayV2RedPackService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2SendRedPackResponse> SendRedPackAsync(
        V2SendRedPackRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2SendRedPackRequest, V2SendRedPackResponse>(
            "/mmpaymkttransfers/sendredpack",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2SendMiniProgramRedPackResponse> SendMiniProgramRedPackAsync(
        V2SendMiniProgramRedPackRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2SendMiniProgramRedPackRequest, V2SendMiniProgramRedPackResponse>(
            "/mmpaymkttransfers/sendminiprogramhb",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2GetRedPackInfoResponse> GetRedPackInfoAsync(
        V2GetRedPackInfoRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2GetRedPackInfoRequest, V2GetRedPackInfoResponse>(
            "/mmpaymkttransfers/gethbinfo",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2SendWorkWxRedPackResponse> SendWorkWxRedPackAsync(
        V2SendWorkWxRedPackRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostWorkWxAsync<V2SendWorkWxRedPackRequest, V2SendWorkWxRedPackResponse>(
            "/mmpaymkttransfers/sendworkwxredpack",
            WechatPayV2HttpClientNames.BaseAddress, request,
            WechatPayV2WorkWxSigner.SendRedPackWhitelist, cancellationToken);

    /// <inheritdoc />
    public Task<V2QueryWorkWxRedPackResponse> QueryWorkWxRedPackAsync(
        V2QueryWorkWxRedPackRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2QueryWorkWxRedPackRequest, V2QueryWorkWxRedPackResponse>(
            "/mmpaymkttransfers/queryworkwxredpack",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}
