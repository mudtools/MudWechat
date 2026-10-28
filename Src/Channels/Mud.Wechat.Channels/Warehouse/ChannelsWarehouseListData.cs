// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>查询区域仓库列表数据对象（官方 <c>data</c>，<c>list/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsWarehouseListData
{
    /// <summary>获取或设置外部仓库 ID 列表（官方 <c>out_warehouse_ids</c>）。</summary>
    [JsonPropertyName("out_warehouse_ids")]
    public List<string>? OutWarehouseIds { get; set; }

    /// <summary>
    /// 获取或设置本次翻页的上下文（官方 <c>next_key</c>）——用于请求下一页；
    /// 如果是空，则当前是最后一页。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
