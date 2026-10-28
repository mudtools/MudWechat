// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Warehouse;

/// <summary>修改区域仓库详情（<c>detail/update</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Warehouse")]
public class ChannelsUpdateWarehouseDetailRequest
{
    /// <summary>获取或设置外部仓库 ID（官方 <c>out_warehouse_id</c>，必填）。</summary>
    [JsonPropertyName("out_warehouse_id")]
    public string OutWarehouseId { get; set; } = string.Empty;

    /// <summary>获取或设置仓库名称（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置仓库介绍（官方 <c>intro</c>，必填）。</summary>
    [JsonPropertyName("intro")]
    public string Intro { get; set; } = string.Empty;
}
