// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>设置指定地址下区域仓库的优先级（<c>address/prioritysort/set</c>）请求体。</summary>
/// <remarks>
/// 官方契约：未设置优先级时默认按区域仓库创建时间作为优先级（越早创建优先级越高）；
/// <c>priority_sort</c> 列表内 <c>out_warehouse_id</c> 不得重复（官方错误码 10020206）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsSetAddressPrioritySortRequest
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

    /// <summary>获取或设置按优先级从高到低排列的外部仓库 ID 列表（官方 <c>priority_sort</c>，选填）。</summary>
    [JsonPropertyName("priority_sort")]
    public List<string>? PrioritySort { get; set; }
}
