// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>获取商品列表（<c>getshopproductlist</c>）请求体。</summary>
/// <remarks><c>limit</c>/<c>offset</c> 不填默认只返回 10 条商品数据（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopProductListRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置分页参数（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
}
