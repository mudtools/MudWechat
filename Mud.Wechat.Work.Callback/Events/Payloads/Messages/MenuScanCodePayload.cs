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
/// 扫码菜单事件载荷（<c>scancode_push</c> / <c>scancode_waitmsg</c> 两键；官方 path 90240）。
/// </summary>
/// <remarks>
/// <b>结构族</b>：两事件的官方报文字段集合一致（信封 + <c>EventKey</c> + <c>ScanCodeInfo</c> + <c>AgentID</c>），
/// 差异仅在触发时机（<c>scancode_waitmsg</c> 额外弹出「消息接收中」提示框，不体现在报文上）。
/// <see cref="ScanCodeInfo"/> 为单对象嵌套（G-ADR-17 <c>Object</c> 通道，内层字段声明化）；
/// 节点缺失 ⇒ <c>null</c>（处理器不得假设必有值）。
/// </remarks>
/// <para>
/// <b>官方文档（核对字段以此为准）</b>：
/// <see href="https://developer.work.weixin.qq.com/document/path/90240">path 90240 接收消息与事件（企业内部开发）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/90376">path 90376 接收消息与事件（第三方）</see>/
/// <see href="https://developer.work.weixin.qq.com/document/path/96468">path 96468 接收消息与事件（服务商代开发）</see>
/// （三份正文逐字一致，ADR-14）。
/// </para>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] {
        WechatCallbackEventTypes.ScanCodePush,
        WechatCallbackEventTypes.ScanCodeWaitMsg })]
public sealed partial class MenuScanCodePayload : WechatCallbackPayload
{
    /// <summary>事件 KEY 值（官方 <c>EventKey</c>，与自定义菜单接口中 KEY 值对应）。</summary>
    [PayloadField("EventKey")]
    public string? EventKey { get; set; }

    /// <summary>扫描信息（官方 <c>ScanCodeInfo</c>：<c>ScanType</c> + <c>ScanResult</c>）。</summary>
    [PayloadField("ScanCodeInfo")]
    public WechatCallbackScanCodeInfo? ScanCodeInfo { get; set; }

    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页面查看）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }
}
