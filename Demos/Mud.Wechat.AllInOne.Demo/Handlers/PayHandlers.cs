// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Callback;

namespace Mud.Wechat.AllInOne.Demo.Handlers;

/// <summary>
/// 支付通知审计处理器（通配桶）：任何过了三道闸的通知都留一条流水。
/// </summary>
/// <remarks>
/// <para>
/// <b>本线的闸门与其余各线不同源</b>：支付 APIv3 通知的合法性靠<b>平台证书</b>验签 +
/// <b>APIv3 密钥</b>解密 <c>resource</c>，没有 <c>msg_signature</c> 那套三参/四参签名，
/// 也没有 EncodingAESKey —— 故 <c>SelfTest</c> 无法离线伪造本线报文（规划方案 R-3），
/// 本处理器在同一宿主里只承担「路由装上了、闸门的拒绝形态是 401/403 JSON」这一层的共存证明。
/// </para>
/// <para>
/// <b>刻意不读 <c>context.ResourceJson</c></b>（解密明文，含 <c>openid</c>，红线 PAY-B7）：
/// 明文不入日志、不入遥测、不入异常消息，同样不入台账。
/// </para>
/// </remarks>
internal sealed class PayAuditHandler(AllInOneLedger ledger) : IWechatPayNotificationHandler
{
    /// <inheritdoc />
    public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
    {
        ledger.Record("pay", context.MerchantKey, context.EventType ?? "(无 event_type)", $"通知 {context.NotificationId ?? "(无 id)"}/通配审计");

        return Task.CompletedTask;
    }
}
