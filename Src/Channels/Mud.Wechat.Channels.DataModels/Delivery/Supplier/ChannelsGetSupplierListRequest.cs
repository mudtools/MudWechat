// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>获取供货商列表（<c>get_supplier_list</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + JSON 请求体；<c>offset</c>/<c>limit</c> 均选填。所属权限集 id：131。</remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetSupplierListRequest
{
    /// <summary>获取或设置偏移量（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置单页数量（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
