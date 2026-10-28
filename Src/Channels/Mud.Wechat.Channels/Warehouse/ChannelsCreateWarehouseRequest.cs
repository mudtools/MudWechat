// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>
/// 创建区域仓库（<c>createwarehouse</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>out_warehouse_id</c> 命名规则为字母数字 <c>[0-9, a-z, A-Z]</c> 加特殊字符 <c>[-_.]</c>，
/// 最大 128 字符，一个店铺下同一个外部 ID 只能创建一个仓库。
/// </para>
/// <para>
/// 覆盖规则（A/B/C/D 均非 0）：仅填 <c>address_id1</c>=覆盖 A 省份，填 1、2=覆盖 A 省 B 市，
/// 填 1~3=覆盖 A 省 B 市 C 区，填 1~4=覆盖 A 省 B 市 C 区 D 街道。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsCreateWarehouseRequest
{
    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填；字母数字加 <c>[-_.]</c>，最大 128 字符，店铺内唯一）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;

    /// <summary>获取或设置仓库名称（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置仓库介绍（官方 <c>intro</c>，必填）。</summary>
    [JsonPropertyName("intro")]
    public string Intro { get; set; } = string.Empty;

    /// <summary>获取或设置覆盖区域（官方 <c>cover_locations</c>，选填，可在创建后经「批量增加覆盖区域」添加）。</summary>
    [JsonPropertyName("cover_locations")]
    public List<ChannelsWarehouseLocation>? CoverLocations { get; set; }
}
