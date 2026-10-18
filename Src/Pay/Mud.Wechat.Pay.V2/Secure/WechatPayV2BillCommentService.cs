// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2BillCommentService"/> 的实现：逐商户证书通道 + 应答原文出口。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写</b>：官方「是否需要证书」为「需要」，失败示例的 <c>return_msg</c> 即「商户证书错误」，
/// 而证书按商户绑定 ⇒ 声明式管道无从拿到。
/// </para>
/// <para>
/// <b>为什么走 <c>SendAsync</c> 而非 <c>PostAsync</c></b>：官方成功应答<b>不是 XML</b>
/// 而是文本表格（首行 offset + 每行一笔评论）⇒ 反序列化无处可去，
/// 只能原样交回；这与 <c>/pay/downloadbill</c> 的下载族同一口径。
/// 失败时官方才回 XML 信封，同样原样交回，由调用方按首字符是否 <c>&lt;</c> 自行分流 ——
/// SDK 不猜列序、不解析评论文本。
/// </para>
/// <para>
/// <b>域名是 <c>api.mch</c></b>：本页接口链接栏无 <c>api2.mch</c> ⇒ 取
/// <see cref="WechatPayV2HttpClientNames.BaseAddress"/>；证书通道仍按 (商户, 域名) 双键独立缓存。
/// </para>
/// <para>
/// <b>不出用户文本</b>（PAY-B7 同线口径）：返回的评论内容是终端用户自由文本，
/// 本类不解析、不改写、不进日志与异常消息。
/// </para>
/// </remarks>
internal sealed class WechatPayV2BillCommentService : IWechatPayV2BillCommentService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建账单评论服务（由 DI 解析）。</summary>
    public WechatPayV2BillCommentService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<string> BatchQueryCommentAsync(
        V2BatchQueryCommentRequest request, CancellationToken cancellationToken = default)
        => _invoker.SendAsync(
            "/billcommentsp/batchquerycomment",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}
