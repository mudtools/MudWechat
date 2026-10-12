// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Qic;

/// <summary>送检快递产品（官方 <c>delivery_products</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectDeliveryProduct
{
    /// <summary>获取或设置快递公司产品 id（官方 <c>id</c>，绑定送检信息 <c>delivery_product_id</c> 的取值来源）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置快递公司产品名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置是否支持保价（官方 <c>enable_insure</c>，见 <see cref="ChannelsInspectInsureSupports"/>）。</summary>
    [JsonPropertyName("enable_insure")]
    public int? EnableInsure { get; set; }

    /// <summary>获取或设置保价类型列表（官方 <c>insure_type_list</c>）。</summary>
    [JsonPropertyName("insure_type_list")]
    public List<ChannelsInspectInsureType>? InsureTypeList { get; set; }
}
