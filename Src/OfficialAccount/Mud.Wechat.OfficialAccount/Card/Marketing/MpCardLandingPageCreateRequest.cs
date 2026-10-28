// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 卡券落地页（POST /card/landingpage/create）

/// <summary>
/// 生成卡片或优惠券的落地页（<c>POST /card/landingpage/create</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>顶层平级形态</b>（对齐基准：SKIT <c>CardLandingPageCreateRequest</c>）：<c>banner</c> / <c>page_title</c> /
/// <c>can_share</c> / <c>scene</c> / <c>card_list</c> 同层，<b>无包装对象</b>。
/// </para>
/// <para><b>官方页面正文本次不可达</b> ⇒ 字段长度与 <c>scene</c> 取值表<b>待逐页核验</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardLandingPageCreateRequest
{
    /// <summary>获取或设置落地页头图 URL（官方 <c>banner</c>，选填）。</summary>
    [JsonPropertyName("banner")]
    public string? Banner { get; set; }

    /// <summary>获取或设置落地页标题（官方 <c>page_title</c>，选填）。</summary>
    [JsonPropertyName("page_title")]
    public string? PageTitle { get; set; }

    /// <summary>获取或设置是否允许分享（官方 <c>can_share</c>，选填）。</summary>
    [JsonPropertyName("can_share")]
    public bool? CanShare { get; set; }

    /// <summary>获取或设置使用场景（官方 <c>scene</c>，选填；取值表<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>获取或设置落地页承载的卡券列表（官方 <c>card_list</c>，必填）。</summary>
    [JsonPropertyName("card_list")]
    public List<MpCardLandingPageCardInfo>? CardList { get; set; }
}
