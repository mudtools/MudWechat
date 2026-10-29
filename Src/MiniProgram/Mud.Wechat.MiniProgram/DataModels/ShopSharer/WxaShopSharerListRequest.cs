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
/// 获取已绑定分享员列表请求（<c>POST /shop/sharer/get_sharer_list</c>，<c>page</c> / <c>page_size</c> 两支均必填）。
/// </summary>
/// <remarks>
/// <para><b>翻页口径</b>：本域三支列表端点（<c>get_sharer_list</c> / <c>get_sharer_live_order_list</c> /
/// <c>get_sharer_live_summary_list</c>）一律 <c>page</c>（从 1 起）+ <c>page_size</c>，<b>无游标分页</b>；
/// 但 <c>page_size</c> 上限不统一（前两支 ≤100、直播间数据汇总 ≤20）⇒ 不得按单一默认值套全族。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerListRequest
{
    /// <summary>第几页（<c>page</c>，number，必填，最小填 1）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>每页数量（<c>page_size</c>，number，必填，官方上限 100）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}
