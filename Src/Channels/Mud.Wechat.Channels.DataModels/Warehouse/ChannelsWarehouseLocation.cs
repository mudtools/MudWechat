// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>覆盖区域地址编码（官方 <c>cover_locations</c> 数组元素，<c>create</c> / <c>get</c> / <c>coverlocations/add</c> / <c>coverlocations/del</c> 共用）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：创建 / 获取 / 批量增 / 批量删四端点的覆盖区域元素字段集完全一致
/// （<c>address_id1</c> 必填 + 可选 2~4 级），共用同一元素 DTO。
/// 覆盖规则（A/B/C/D 均非 0）：A000=A 省份；AB00=A 省 B 市；ABC0=A 省 B 市 C 区；ABCD=A 省 B 市 C 区 D 街道。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehouseLocation
{
    /// <summary>获取或设置省份地址编码（官方 <c>address_id1</c>，必填）。</summary>
    [JsonPropertyName("address_id1")]
    public long AddressId1 { get; set; }

    /// <summary>获取或设置市地址编码（官方 <c>address_id2</c>，选填）。</summary>
    [JsonPropertyName("address_id2")]
    public long? AddressId2 { get; set; }

    /// <summary>获取或设置区地址编码（官方 <c>address_id3</c>，选填）。</summary>
    [JsonPropertyName("address_id3")]
    public long? AddressId3 { get; set; }

    /// <summary>获取或设置街道地址编码（官方 <c>address_id4</c>，选填）。</summary>
    [JsonPropertyName("address_id4")]
    public long? AddressId4 { get; set; }
}
