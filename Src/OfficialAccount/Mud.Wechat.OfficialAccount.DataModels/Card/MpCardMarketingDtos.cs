// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 卡券二维码（POST /card/qrcode/create）

/// <summary>
/// 创建卡券 / 礼品码二维码（<c>POST /card/qrcode/create</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>与公众号「带参二维码」域无重叠</b>：本端点路由为 <c>/card/qrcode/create</c>，
/// 与 <c>IMpQrcodeService</c> 的 <c>/cgi-bin/qrcode/create</c> 是两个不同 URI ⇒
/// 不受 <c>MpRouteCountGuard</c> RC3「同一路由不得跨接口」约束，但两域职责在接口 XML 中互相点名。
/// </para>
/// <para><b>顶层三字段</b>：<c>action_name</c>（官方动作枚举，取值<b>待逐页核验</b>）+ <c>action_info</c> + <c>expire_seconds</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeCreateRequest
{
    /// <summary>获取或设置二维码有效秒数（官方 <c>expire_seconds</c>，选填；上限数值<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("expire_seconds")]
    public long? ExpireSeconds { get; set; }

    /// <summary>获取或设置动作类型（官方 <c>action_name</c>，必填；取值表<b>待官方逐页核验</b>，SDK 不建枚举常量）。</summary>
    [JsonPropertyName("action_name")]
    public string ActionName { get; set; } = string.Empty;

    /// <summary>获取或设置动作信息（官方 <c>action_info</c>，必填）。</summary>
    [JsonPropertyName("action_info")]
    public MpCardQrcodeActionInfo? ActionInfo { get; set; }
}

/// <summary>
/// 二维码动作信息（官方 <c>action_info</c>）—— <c>card</c>（单卡）与 <c>multiple_card</c>（多卡）<b>互斥</b>。
/// </summary>
/// <remarks>由 <c>action_name</c> 判别取哪一支；SDK <b>不做本地互斥校验</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeActionInfo
{
    /// <summary>获取或设置单卡信息（官方 <c>card</c>）。</summary>
    [JsonPropertyName("card")]
    public MpCardQrcodeCardInfo? Card { get; set; }

    /// <summary>获取或设置多卡信息（官方 <c>multiple_card</c>）。</summary>
    [JsonPropertyName("multiple_card")]
    public MpCardQrcodeMultipleCardInfo? MultipleCard { get; set; }
}

/// <summary>二维码单卡信息（官方 <c>action_info.card</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeCardInfo
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置券码（官方 <c>code</c>，选填；礼品码场景使用）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置指定领取用户的 openid（官方 <c>openid</c>，选填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置是否唯一码（官方 <c>is_unique_code</c>，选填）。</summary>
    [JsonPropertyName("is_unique_code")]
    public bool? IsUniqueCode { get; set; }

    /// <summary>获取或设置外部自定义 id（官方 <c>outer_id</c>，选填）。</summary>
    [JsonPropertyName("outer_id")]
    public string? OuterId { get; set; }

    /// <summary>获取或设置外部自定义串（官方 <c>outer_str</c>，选填）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }
}

/// <summary>二维码多卡信息（官方 <c>action_info.multiple_card</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeMultipleCardInfo
{
    /// <summary>获取或设置卡券列表（官方 <c>card_list</c>，多卡领取场景）。</summary>
    [JsonPropertyName("card_list")]
    public List<MpCardQrcodeCardInfo>? CardList { get; set; }
}

/// <summary>
/// 创建卡券 / 礼品码二维码（<c>POST /card/qrcode/create</c>）应答。
/// </summary>
/// <remarks>
/// <para>
/// <b>四字段全建模</b>：<c>ticket</c> / <c>url</c> / <c>show_qrcode_url</c> / <c>expire_seconds</c>。
/// 其中 <c>show_qrcode_url</c> 为「可直接展示的二维码图片地址」——取回该图需自行下载，
/// SDK <b>不做图片通道</b>（对齐小程序码手工通道的既存口径）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeCreateResponse : MpResponse
{
    /// <summary>获取或设置二维码有效秒数（官方 <c>expire_seconds</c>）。</summary>
    [JsonPropertyName("expire_seconds")]
    public long? ExpireSeconds { get; set; }

    /// <summary>获取或设置二维码票据（官方 <c>ticket</c>）。</summary>
    [JsonPropertyName("ticket")]
    public string? Ticket { get; set; }

    /// <summary>获取或设置二维码跳转链接（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置可直接展示的二维码图片地址（官方 <c>show_qrcode_url</c>）。</summary>
    [JsonPropertyName("show_qrcode_url")]
    public string? ShowQrCodeUrl { get; set; }
}

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

/// <summary>落地页卡券项（官方 <c>card_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardLandingPageCardInfo
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置卡券缩略图 URL（官方 <c>thumb_url</c>，选填）。</summary>
    [JsonPropertyName("thumb_url")]
    public string? ThumbUrl { get; set; }
}

/// <summary>生成落地页（<c>POST /card/landingpage/create</c>）应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardLandingPageCreateResponse : MpResponse
{
    /// <summary>获取或设置落地页编号（官方 <c>page_id</c>）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置落地页链接（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

// ---------------------------------------------------------------- 支付即会员 / 自助核销 / 测试白名单

/// <summary>
/// 开通或关闭「支付后开卡」组件（<c>POST /card/paycell/set</c>）请求体。
/// </summary>
/// <remarks><b>官方键名为 <c>is_open</c></b>（布尔开关），两字段即全部请求面。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayCellSetRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置是否开通（官方 <c>is_open</c>，必填）。</summary>
    [JsonPropertyName("is_open")]
    public bool IsOpen { get; set; }
}

/// <summary>
/// 开通或关闭「自助核销」组件（<c>POST /card/selfconsumecell/set</c>）请求体。
/// </summary>
/// <remarks>
/// <b>官方键名照录 <c>need_verify_cod</c></b>（SKIT 对齐基准如此）—— 该拼写<b>缺尾部 <c>e</c></b>，
/// 与常见 <c>need_verify_code</c> 不同。此为<b>待官方逐页核验</b>的高风险点：若官方页确为
/// <c>need_verify_code</c>，守卫 CD6 与本片段的「照录而非修拼写」判断需同步更正。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSelfConsumeCellSetRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置是否开通自助核销（官方 <c>is_open</c>，必填）。</summary>
    [JsonPropertyName("is_open")]
    public bool IsOpen { get; set; }

    /// <summary>获取或设置核销时是否需要验证券码（官方 <c>need_verify_cod</c>，选填；拼写<b>待逐页核验</b>）。</summary>
    [JsonPropertyName("need_verify_cod")]
    public bool? NeedVerifyCode { get; set; }

    /// <summary>获取或设置核销时是否需要录入金额（官方 <c>need_remark_amount</c>，选填）。</summary>
    [JsonPropertyName("need_remark_amount")]
    public bool? NeedRemarkAmount { get; set; }
}

/// <summary>
/// 设置测试白名单（<c>POST /card/testwhitelist/set</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>用途</b>：卡券功能在未通过官方审核前仅对白名单用户可见，本端点即该灰度通道。
/// </para>
/// <para>
/// <b>两键二选一</b>：<c>openid</c>（用户）与 <c>username</c>（公众号原始 id / 微信号方向）由官方按场景取用；
/// SDK <b>不做本地互斥校验</b>。对齐基准 SKIT 两字段均为可空 <c>string</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardTestWhiteListSetRequest
{
    /// <summary>获取或设置测试用户 openid（官方 <c>openid</c>，选填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置测试账号名（官方 <c>username</c>，选填；语义<b>待官方逐页核验</b>）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
