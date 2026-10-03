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
/// 上报地理位置事件载荷（<c>LOCATION</c> 单键；官方 path 90240，官方键值为大写）。
/// </summary>
/// <remarks>
/// <para>
/// <b>结构族</b>：官方报文为「信封 + <c>Latitude</c> + <c>Longitude</c> + <c>Precision</c> + <c>AgentID</c> + <c>AppType</c>」，
/// 触发时机为成员同意上报地理位置后<b>每次进入应用会话</b>上报一次；企业可在管理端关闭该权限
/// ⇒ 报文可能整体不出现（无事件），字段级不做缺失假设即可。
/// </para>
/// <para>
/// <b>坐标为小数</b>（如 <c>23.104</c>/<c>113.320</c>/<c>65.000</c>）：生成器的 <c>Number&lt;T&gt;</c> 推断
/// 只解析整数，故三个字段改用 <c>WechatPayloadConverter.ParseReal</c>（<c>double?</c>）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
[WechatCallbackContract(
    RequiredFamily = WechatCallbackEventFamily.Unknown,
    SupportedAppTypes = WechatAppTypeSet.All,
    RequiredChannel = WechatCallbackChannel.App,
    EventTypes = new[] { WechatCallbackEventTypes.Location })]
public sealed partial class LocationReportedPayload : WechatCallbackPayload
{
    /// <summary>地理位置纬度（官方 <c>Latitude</c>，小数）。</summary>
    [PayloadField("Latitude", Method = nameof(WechatPayloadConverter.ParseReal))]
    public double? Latitude { get; set; }

    /// <summary>地理位置经度（官方 <c>Longitude</c>，小数）。</summary>
    [PayloadField("Longitude", Method = nameof(WechatPayloadConverter.ParseReal))]
    public double? Longitude { get; set; }

    /// <summary>地理位置精度（官方 <c>Precision</c>，小数）。</summary>
    [PayloadField("Precision", Method = nameof(WechatPayloadConverter.ParseReal))]
    public double? Precision { get; set; }

    /// <summary>企业应用 id（官方 <c>AgentID</c>，整型；可在应用设置页面查看）。</summary>
    [PayloadField("AgentID")]
    public string? AgentId { get; set; }

    /// <summary>来源应用类型（官方 <c>AppType</c>：企业微信内恒为 <c>wxwork</c>，微信端不返回该节点）。</summary>
    [PayloadField("AppType")]
    public string? AppType { get; set; }
}
