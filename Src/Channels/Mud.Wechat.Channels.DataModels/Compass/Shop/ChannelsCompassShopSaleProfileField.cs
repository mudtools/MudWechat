// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>人群维度类别（官方 <c>field_list</c> 数组元素，<c>getshopsaleprofiledata</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopSaleProfileField
{
    /// <summary>获取或设置维度类别名（官方 <c>field_name</c>，如 年龄/性别/省级/策略人群）。</summary>
    [JsonPropertyName("field_name")]
    public string? FieldName { get; set; }

    /// <summary>获取或设置指标列表（官方 <c>data_list</c>）。</summary>
    [JsonPropertyName("data_list")]
    public List<ChannelsCompassShopSaleProfileDim>? DataList { get; set; }
}
