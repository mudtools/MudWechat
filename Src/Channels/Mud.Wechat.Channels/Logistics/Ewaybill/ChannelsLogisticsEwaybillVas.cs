// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>保价等增值服务（官方 <c>order_vas_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillVas
{
    /// <summary>获取或设置增值服务类型（官方 <c>vas_type</c>，如 INSURE / VA002 等）。</summary>
    [JsonPropertyName("vas_type")]
    public string? VasType { get; set; }

    /// <summary>获取或设置增值服务值（官方 <c>vas_value</c>，涉及金额单位一律为分）。</summary>
    [JsonPropertyName("vas_value")]
    public string? VasValue { get; set; }

    /// <summary>获取或设置增值服务描述（官方 <c>vas_detail</c>，查单响应返回）。</summary>
    [JsonPropertyName("vas_detail")]
    public string? VasDetail { get; set; }
}
