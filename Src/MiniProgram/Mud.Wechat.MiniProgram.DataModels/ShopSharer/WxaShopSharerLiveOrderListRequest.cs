// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 获取分享员直播间订单汇总请求（<c>POST /shop/sharer/get_sharer_live_order_list</c>，
/// <c>openid</c> / <c>live_export_id</c> / <c>page</c> / <c>page_size</c> 四支均必填）。
/// </summary>
/// <remarks>
/// <para><b><c>live_export_id</c> 必填是本端点与另一支列表端点的关键差异</b>：<c>get_sharer_live_summary_list</c>
/// 只要 <c>openid</c> + 分页，本端点还须先取直播间 export_id ⇒ 正确调用序为「live_summary_list 拿 id → 本端点逐直播间拉订单」。</para>
/// <para><b>敏感面</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerLiveOrderListRequest
{
    /// <summary>分享员 openid（<c>openid</c>，string，必填）。禁落日志。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>直播间 export_id（<c>live_export_id</c>，string，必填）：取自 <c>get_sharer_live_summary_list</c> 的 <c>lives[].live_export_id</c>。</summary>
    [JsonPropertyName("live_export_id")]
    public string? LiveExportId { get; set; }

    /// <summary>第几页（<c>page</c>，number，必填，最小填 1）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>每页数量（<c>page_size</c>，number，必填，官方上限 100）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}
