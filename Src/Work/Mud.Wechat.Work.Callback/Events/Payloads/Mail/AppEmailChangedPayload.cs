// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 应用邮箱接收邮件事件载荷（<b>族事件值</b>：<c>app_email_change</c>，<c>ChangeType = receive_email</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>族事件值即事件键</b>：官方 <c>receive_email</c> 与公共邮箱族（<see cref="PublicEmailChangedPayload"/>）同名，
/// 逐 <c>ChangeType</c> 键无法消歧，故以族事件值为事件键、<c>ChangeType</c> 经信封判别
/// （与客户联系/获客族同理）。应用邮箱收到邮件后触发，<see cref="Amount"/> 表示应用邮箱当前的新邮件数。
/// </para>
/// <para>
/// <b>三模式无关性（ADR-14）</b>：官方 97495/97517/97506 三份文档的报文结构与参数表逐字一致
/// （含 <c>Amount</c> 的 CDATA 承载形态），差异只是「值是否出现」，一份载荷覆盖三类应用。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97495">path 97495 邮件 回调通知（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97517">path 97517（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97506">path 97506（服务商代开发）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.AppEmailChange })]
public sealed partial class AppEmailChangedPayload : WechatCallbackPayload
{
    /// <summary>应用邮箱当前的新邮件数（官方 <c>Amount</c>，数值文本；官方样例以 CDATA 承载）。</summary>
    [PayloadField("Amount")]
    public long? Amount { get; set; }
}
