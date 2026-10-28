// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2RiskService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写</b>：官方「是否需要证书」为「是」（错误码表给出 <c>CERT_ERROR</c>），
/// 而证书按商户绑定 ⇒ 声明式管道无从拿到。落 <see cref="WechatPayV2SecureInvoker"/> 后，
/// 签名、应答验签、空值不入签名串等口径与其余手写族完全同源。
/// </para>
/// <para>
/// <b>域名是 <c>api.mch</c></b>：本页官方 URL 无 <c>api2.mch</c> ⇒ 取 <see cref="WechatPayV2HttpClientNames.BaseAddress"/>。
/// </para>
/// <para>
/// <b>记录原样交回</b>：<c>violation_records</c> 是整段 JSON 或 CSV 字符串，本类不解析；
/// 该字段含公司名称与违规子商户号，异常与日志一律不回显（PAY-B7 同线口径）。
/// </para>
/// </remarks>
internal sealed class WechatPayV2RiskService : IWechatPayV2RiskService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建风控侧服务（由 DI 解析）。</summary>
    public WechatPayV2RiskService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2ViolationRecordQueryResponse> GetViolationRecordAsync(
        V2ViolationRecordQueryRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2ViolationRecordQueryRequest, V2ViolationRecordQueryResponse>(
            "/risk/getviolation",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}
