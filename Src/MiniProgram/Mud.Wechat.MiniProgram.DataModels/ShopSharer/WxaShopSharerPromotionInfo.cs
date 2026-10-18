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
/// 直播间订单推广信息（<c>order_detail.promotion_info</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>live_start_time</c> 同名不同型陷阱</b>：本类里官方标 string 且示例为「2020-04-25 12:05:25」
/// 格式化时间文本；而 <c>get_sharer_live_summary_list</c> 的 <c>lives[].live_start_time</c> 官方标 number。
/// 两处<b>不得</b>共用类型、也不得都改成 DateTimeOffset（线格式会变，守卫 SHOPSH5 锁定）。</para>
/// <para><b>敏感面</b>：<c>sharer_openid</c>（注意键名带 <c>sharer_</c> 前缀，与请求侧裸 <c>openid</c> 不同）禁落日志。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerPromotionInfo
{
    /// <summary>推广员视频号用户名（<c>finder_username</c>，string）。</summary>
    [JsonPropertyName("finder_username")]
    public string? FinderUserName { get; set; }

    /// <summary>推广员视频号昵称（<c>finder_nickname</c>，string）。</summary>
    [JsonPropertyName("finder_nickname")]
    public string? FinderNickname { get; set; }

    /// <summary>分享员的 openid（<c>sharer_openid</c>，string）。禁落日志。</summary>
    [JsonPropertyName("sharer_openid")]
    public string? SharerOpenId { get; set; }

    /// <summary>直播开始时间（<c>live_start_time</c>，string，官方示例形如「2020-04-25 12:05:25」）。与直播汇总侧的 number 形态同名不同型。</summary>
    [JsonPropertyName("live_start_time")]
    public string? LiveStartTime { get; set; }
}
