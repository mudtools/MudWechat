// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>微盘容量不足事件载荷（<c>wedrive_insufficient_capacity</c>；官方 path 97898/97972/97932）。</summary>
/// <remarks>
/// <para>
/// 触发时机（官方原文）：「当企业微盘容量使用率超过90%，且企业有授权安装了具有微盘权限的第三方应用，
/// 则企业微信向第三方应用回调通知该事件」。信封外无业务字段（纯通知型事件，与安全管理
/// <c>change_domain_ip</c> 同形态）。
/// </para>
/// <para>
/// <b>官方业务限制（不得弱化）</b>：该回调<b>不是实时回调</b>，而是每天定时检测触发回调，
/// 且单个授权企业每天最多回调一次 —— 处理器不得假设容量状态实时。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/97898">path 97898 微盘容量不足事件（企业自建）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97972">path 97972（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/97932">path 97932（服务商代开发）</see>
/// （三份正文逐字一致，ADR-14）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.WedriveInsufficientCapacity })]
public sealed partial class WedriveInsufficientCapacityPayload : WechatCallbackPayload
{
}
