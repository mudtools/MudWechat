// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Qic;

/// <summary>送检配置模板（官方 <c>submit_config</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectSubmitConfig
{
    /// <summary>获取或设置快递公司列表（官方 <c>delivery_list</c>）。</summary>
    [JsonPropertyName("delivery_list")]
    public List<ChannelsInspectDelivery>? DeliveryList { get; set; }

    /// <summary>获取或设置质检机构列表（官方 <c>inspect_org_list</c>）。</summary>
    [JsonPropertyName("inspect_org_list")]
    public List<ChannelsInspectOrg>? InspectOrgList { get; set; }

    /// <summary>获取或设置资费标准（官方 <c>charge_url</c>）。</summary>
    [JsonPropertyName("charge_url")]
    public string? ChargeUrl { get; set; }
}
