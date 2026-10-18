// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2SecureService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写而不是源生成</b>：见 <see cref="WechatPayV2SecureChannelProvider"/> 类注释 ——
/// 客户端证书按商户绑定，与「一条命名客户端一个连接池」的复用模型冲突。
/// </para>
/// <para>
/// <b>签名/收发不在本类</b>：报文规则（XML、字段表签名、应答验签、空值不入库）与声明式管道完全同源，
/// 且需证书的族不止本接口一家（红包 / 企业付款同样要证书），故统一委托
/// <see cref="WechatPayV2SecureInvoker"/>；本类只剩「路径 + 域名 + DTO」的映射。
/// 域名取 <see cref="WechatPayV2HttpClientNames.SecureBaseAddress"/>（<c>api2.mch</c>）：
/// 本接口的 5 支官方均给证书域名。
/// </para>
/// <para>
/// <b>不出密钥</b>（PAY-B7）：异常消息只含商户键、路由与官方返回字段，绝不包含密钥、<c>sign</c> 或证书材料。
/// </para>
/// </remarks>
internal sealed class WechatPayV2SecureService : IWechatPayV2SecureService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建安全侧服务（由 DI 解析）。</summary>
    public WechatPayV2SecureService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2RefundResponse> RefundAsync(
        V2RefundRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2RefundRequest, V2RefundResponse>(
            "/secapi/pay/refund",
            WechatPayV2HttpClientNames.SecureBaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2RefundV2Response> RefundV2Async(
        V2RefundV2Request request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2RefundV2Request, V2RefundV2Response>(
            "/secapi/pay/refundv2",
            WechatPayV2HttpClientNames.SecureBaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2ReverseResponse> ReverseAsync(
        V2ReverseRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2ReverseRequest, V2ReverseResponse>(
            "/secapi/pay/reverse",
            WechatPayV2HttpClientNames.SecureBaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<string> DownloadBillAsync(
        V2DownloadBillRequest request, CancellationToken cancellationToken = default)
        => _invoker.SendAsync(
            "/pay/downloadbill",
            WechatPayV2HttpClientNames.SecureBaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<string> DownloadFundFlowAsync(
        V2DownloadFundFlowRequest request, CancellationToken cancellationToken = default)
        => _invoker.SendAsync(
            "/pay/downloadfundflow",
            WechatPayV2HttpClientNames.SecureBaseAddress, request, cancellationToken);
}
