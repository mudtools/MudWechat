// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>
/// 批量增加 / 批量删除覆盖区域（<c>coverlocations/add</c> 与 <c>coverlocations/del</c>）共用请求体。
/// </summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：两端点请求体字段集完全一致（<c>out_warehouse_id</c> + <c>cover_locations</c>，
/// 元素结构同为 <see cref="ChannelsWarehouseLocation"/>），共用同一请求 DTO；语义演化如官方分裂再按端点分建。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehouseCoverLocationsRequest
{
    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;

    /// <summary>获取或设置覆盖区域（官方 <c>cover_locations</c>，必填；可从获取区域仓库接口获得）。</summary>
    [JsonPropertyName("cover_locations")]
    public List<ChannelsWarehouseLocation> CoverLocations { get; set; } = new();
}
