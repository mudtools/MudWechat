// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>获取分配方式（<c>get_distribute</c>）响应。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）；
/// 分配类型枚举见 <see cref="ChannelsDistributeTypes"/>。所属权限集 id：131。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetDistributeResponse : ChannelsResponse
{
    /// <summary>获取或设置分配类型（官方 <c>distribute_type</c>，见 <see cref="ChannelsDistributeTypes"/>）。</summary>
    [JsonPropertyName("distribute_type")]
    public int? DistributeType { get; set; }

    /// <summary>获取或设置全店订单自动分配的供货商 appid（官方 <c>all_supplier_appid</c>）。</summary>
    [JsonPropertyName("all_supplier_appid")]
    public string? AllSupplierAppid { get; set; }

    /// <summary>获取或设置全店订单自动分配的供货商名称（官方 <c>all_distribute_supplier_name</c>）。</summary>
    [JsonPropertyName("all_distribute_supplier_name")]
    public string? AllDistributeSupplierName { get; set; }
}
