// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 查询券码状态（<c>POST /card/code/get</c>）应答。
/// </summary>
/// <remarks>
/// <para>
/// <b>字段名三处易错</b>（对齐基准：SKIT <c>CardCodeGetResponse</c>，守卫 CD7 锁定）：
/// 券有效期用 <c>begin_time</c> / <c>end_time</c>（<b>非</b>建卡侧的 <c>begin_timestamp</c>）；
/// 会员积分/余额在<b>顶层</b>用 <c>bonus</c> / <c>balance</c>（与 <c>card</c> 内的同名键是两码事，
/// 故 SDK 属性名区分 <see cref="MpCardCodeInfo.Bonus"/> 与 <see cref="MemberBonus"/>）；
/// 用户信息容器为 <c>user_info</c>（内层是 <c>common_field_list</c> / <c>custom_field_list</c> 两张 name/value 表）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeGetResponse : MpResponse
{
    /// <summary>获取或设置券码与卡券信息（官方 <c>card</c>）。</summary>
    [JsonPropertyName("card")]
    public MpCardCodeInfo? Card { get; set; }

    /// <summary>获取或设置券码状态（官方 <c>user_card_status</c>；状态枚举<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("user_card_status")]
    public string? CardStatus { get; set; }

    /// <summary>获取或设置持券用户 openid（官方 <c>openid</c>）。</summary>
    /// <remarks><b>官方键名为全小写 <c>openid</c></b>（非公众号用户域惯用的 <c>openid</c> 之外的形态）。</remarks>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置持券用户昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置会员卡编号（官方 <c>membership_number</c>，仅会员卡）。</summary>
    [JsonPropertyName("membership_number")]
    public string? MembershipNumber { get; set; }

    /// <summary>获取或设置会员信息（官方 <c>user_info</c>，仅会员卡）。</summary>
    [JsonPropertyName("user_info")]
    public MpCardMemberUserInfo? UserInfo { get; set; }

    /// <summary>获取或设置会员积分（官方 <c>bonus</c>，顶层会员维度）。</summary>
    [JsonPropertyName("bonus")]
    public int? MemberBonus { get; set; }

    /// <summary>获取或设置会员余额（官方 <c>balance</c>，顶层会员维度，单位：分）。</summary>
    [JsonPropertyName("balance")]
    public int? MemberBalance { get; set; }

    /// <summary>获取或设置订单号（官方 <c>order_id</c>，礼品卡购买场景）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置卡面背景图 URL（官方 <c>background_pic_url</c>）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置当前是否可核销（官方 <c>can_consume</c>，须请求侧 <c>check_consume</c> 为真）。</summary>
    [JsonPropertyName("can_consume")]
    public bool? CanConsume { get; set; }

    /// <summary>获取或设置外部自定义串（官方 <c>outer_str</c>，建码时传入的回显）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }
}
