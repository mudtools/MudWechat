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
/// 企业微信域名IP变更事件载荷（<c>change_domain_ip</c>；官方 100080）。
/// </summary>
/// <remarks>
/// <para>
/// <b>信封外无业务字段</b>：官方报文即「标准信封 + <c>Event = security</c> +
/// <c>ChangeType = change_domain_ip</c>」，参数表未列出任何业务节点
/// —— 具体变更类别由信封 <c>ChangeType</c> 判别，本载荷仅承载契约登记与开放面声明。
/// 处理器收到后应刷新本地缓存的域名/IP 白名单（可用官方
/// <see href="https://developer.work.weixin.qq.com/document/path/100079">path 100079 获取企业微信域名IP信息</see>
/// 接口拉取最新值）。
/// </para>
/// <para>
/// <b>官方开放面</b>：<b>仅企业自建应用</b>可配置接收 —— 须将应用配置到
/// 「我的企业 - 设置 - 域名IP - 可调用API的应用」；<b>第三方应用 / 代开发应用暂不支持</b>
/// （官方页面权限表明示，v2.2 ADR-15 以事件键级开放面声明承载：
/// <c>SupportedAppTypes = Internal</c>，见 <c>OfficialPayloadContracts</c>）。
/// </para>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/100080">path 100080 企业微信域名IP变更事件（安全管理·回调通知，仅自建）</see>。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredEvent = WechatCallbackEventTypes.Security,
    RequiredFamily = WechatCallbackEventFamily.SecurityChange,
    SupportedAppTypes = WechatAppTypeSet.Internal,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.ChangeDomainIp })]
public sealed partial class SecurityDomainIpChangedPayload : WechatCallbackPayload
{
}
