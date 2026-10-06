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
/// 公共邮箱接收邮件事件载荷（<b>族事件值</b>：<c>public_email_change</c>，<c>ChangeType = receive_email</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 与 <see cref="AppEmailChangedPayload"/> 同理以族事件值为事件键（<c>receive_email</c> 跨族同名）；
/// 较应用邮箱报文多出 <see cref="Id"/>（公共邮箱 id）节点。
/// </para>
/// <para>
/// <b>开放面</b>：仅<b>企业自建应用</b>可接收 —— 官方第三方应用与服务商代开发均无「管理公共邮箱」
/// 回调事件，该约束由事件键级开放面声明承载（<c>SupportedAppTypes = Internal</c>）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/100180">path 100180 管理公共邮箱 回调通知（企业自建）</see>。
/// 注意本页 <c>Id</c>/<c>Amount</c> 为裸数字文本节点（无 CDATA），与应用邮箱页的 CDATA 形态不同，
/// 解析不得假定数值字段恒有 CDATA。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.PublicEmailChange })]
public sealed partial class PublicEmailChangedPayload : WechatCallbackPayload
{
    /// <summary>公共邮箱 id（官方 <c>Id</c>，数字文本；应用邮箱报文无此节点）。</summary>
    [PayloadField("Id")]
    public string? Id { get; set; }

    /// <summary>公共邮箱当前的新邮件数（官方 <c>Amount</c>，数值文本）。</summary>
    [PayloadField("Amount")]
    public long? Amount { get; set; }
}
