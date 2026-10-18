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
/// 分享员直播间带货数据汇总应答（<c>POST /shop/sharer/get_sharer_live_summary_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>列表键名 <c>lives</c></b>（官方原文）；本支是 <c>get_sharer_live_order_list</c> 的
/// <c>live_export_id</c> 供给方（订单支必填本支返回值）⇒ 两支构成「先汇总取 id、后逐场拉订单」的两段式。</para>
/// <para><b>敏感面</b>：直播间维度金额属经营数据，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerLiveSummaryListResponse : WxaResponse
{
    /// <summary>直播间列表（<c>lives</c>，object array），见 <see cref="WxaShopSharerLiveInfo"/>。</summary>
    [JsonPropertyName("lives")]
    public List<WxaShopSharerLiveInfo>? Lives { get; set; }

    /// <summary>分享员分享的直播间总数（<c>total_num</c>，number）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}
