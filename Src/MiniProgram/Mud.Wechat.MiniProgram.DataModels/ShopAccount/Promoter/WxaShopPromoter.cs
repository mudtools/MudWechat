// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 推广员条目（官方 <c>/shop/promoter/list</c> 回包 <c>promoters[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>三支全是字符串</b>：官方把 <c>promoter_id</c> 示例写成 <c>a-****…po</c> 形态的<b>非定长串</b>
/// （不是 number，不得数值化）；<c>finder_nickname</c> 是<b>视频号</b>昵称 ⇒ 推广员身份体系属视频号侧，
/// 与小程序 openid 不同源。</para>
/// <para><b>敏感面</b>：<c>promoter_openid</c> 为用户表示标识，<b>不得</b>写入日志、遥测或异常消息；
/// 官方示例已把它写成字面量 <c>"openid"</c>，实际返回是真实 openid。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopPromoter
{
    /// <summary>推广员视频号昵称（<c>finder_nickname</c>，string）。</summary>
    [JsonPropertyName("finder_nickname")]
    public string? FinderNickname { get; set; }

    /// <summary>推广员唯一 ID（<c>promoter_id</c>，string，非定长串；按推广员查订单等下游端点用此值）。</summary>
    [JsonPropertyName("promoter_id")]
    public string? PromoterId { get; set; }

    /// <summary>推广员 openid（<c>promoter_openid</c>，string）。敏感，禁落日志。</summary>
    [JsonPropertyName("promoter_openid")]
    public string? PromoterOpenid { get; set; }
}
