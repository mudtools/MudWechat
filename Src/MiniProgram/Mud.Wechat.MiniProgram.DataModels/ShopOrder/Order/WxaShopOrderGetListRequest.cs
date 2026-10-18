// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 查询订单列表请求（<c>/shop/order/get_list</c>，偏移式为官方口径）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopOrderGetListRequest
{
    /// <summary>第几页（<b>必填</b>，官方要求 ≥ 1）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>每页订单数（<b>必填</b>，官方上限 <b>100</b>）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>排序（<b>必填</b>）：<c>1</c> 倒序 desc、<c>2</c> 正序 asc。</summary>
    [JsonPropertyName("sort_order")]
    public int? SortOrder { get; set; }

    /// <summary>起始创建时间，<c>yyyy-MM-dd HH:mm:ss</c>（选填）。</summary>
    [JsonPropertyName("start_create_time")]
    public string? StartCreateTime { get; set; }

    /// <summary>最终创建时间，<c>yyyy-MM-dd HH:mm:ss</c>（选填）。</summary>
    [JsonPropertyName("end_create_time")]
    public string? EndCreateTime { get; set; }
}
