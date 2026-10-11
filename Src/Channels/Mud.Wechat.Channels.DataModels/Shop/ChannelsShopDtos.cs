// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Shop;

/// <summary>
/// 获取店铺基本信息（<c>mmecapi_basicinfo</c>）响应。
/// </summary>
/// <remarks>官方契约：<b>GET</b> + 空请求体（官方参数表仅 <c>access_token</c> 一个 URL 参数）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopBasicInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置店铺信息（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public ChannelsShopBasicInfo? Info { get; set; }
}

/// <summary>店铺基本信息（官方 <c>info</c> 对象，<c>mmecapi_basicinfo</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopBasicInfo
{
    /// <summary>获取或设置店铺名称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置店铺头像 URL（官方 <c>headimg_url</c>）。</summary>
    [JsonPropertyName("headimg_url")]
    public string? HeadimgUrl { get; set; }

    /// <summary>获取或设置店铺类型（官方 <c>subject_type</c>，见 <see cref="ChannelsShopSubjectTypes"/>）。</summary>
    [JsonPropertyName("subject_type")]
    public string? SubjectType { get; set; }

    /// <summary>获取或设置店铺状态（官方 <c>status</c>，见 <see cref="ChannelsShopStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置店铺原始 ID（官方 <c>username</c>）。</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>获取或设置是否本地生活小店（官方 <c>is_local_life</c>：0 否，1 是）。</summary>
    [JsonPropertyName("is_local_life")]
    public int? IsLocalLife { get; set; }

    /// <summary>获取或设置开店时间戳（官方 <c>open_timestamp</c>；店铺状态为 open_finished 时返回）。</summary>
    [JsonPropertyName("open_timestamp")]
    public long? OpenTimestamp { get; set; }
}

/// <summary>
/// 店铺传播物料企业微信参数请求体（<c>h5url/get</c> 与 <c>taglink/get</c> 请求体字段集一致 ⇒ 共用；
/// 裁决理由见守卫 <c>ChannelsShopContractGuards</c> CS5）。
/// </summary>
/// <remarks>
/// 官方契约：两字段均选填，经「获取关联账号企微id」接口获取；带企微参数的链接 / 口令下单后，
/// 订单会在成交来源展示该企业。二维码端点额外多一个必填 <c>qrcode_type</c> ⇒ 独立 DTO 不共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopWecomLinkRequest
{
    /// <summary>获取或设置小店通过关联账号绑定的企业微信 id（官方 <c>wecom_corp_id</c>，选填）。</summary>
    [JsonPropertyName("wecom_corp_id")]
    public string? WecomCorpId { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信下的成员 id（官方 <c>wecom_user_id</c>，选填）。</summary>
    [JsonPropertyName("wecom_user_id")]
    public string? WecomUserId { get; set; }
}

/// <summary>获取店铺二维码（<c>qrcode/get</c>）请求体。</summary>
/// <remarks>官方契约：<c>qrcode_type</c> 必填；企微参数与 H5 链接 / 口令同语义但字段集不同 ⇒ 独立 DTO。</remarks>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopQrcodeRequest
{
    /// <summary>获取或设置小店通过关联账号绑定的企业微信 id（官方 <c>wecom_corp_id</c>，选填）。</summary>
    [JsonPropertyName("wecom_corp_id")]
    public string? WecomCorpId { get; set; }

    /// <summary>获取或设置小店通过关联账号绑定的企业微信下的成员 id（官方 <c>wecom_user_id</c>，选填）。</summary>
    [JsonPropertyName("wecom_user_id")]
    public string? WecomUserId { get; set; }

    /// <summary>获取或设置二维码类型（官方 <c>qrcode_type</c>，必填，见 <see cref="ChannelsShopQrcodeTypes"/>）。</summary>
    [JsonPropertyName("qrcode_type")]
    public int QrcodeType { get; set; }
}

/// <summary>获取店铺 H5 链接（<c>h5url/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopH5UrlResponse : ChannelsResponse
{
    /// <summary>获取或设置店铺 H5 链接（官方 <c>shop_h5url</c>）。</summary>
    [JsonPropertyName("shop_h5url")]
    public string? ShopH5Url { get; set; }
}

/// <summary>获取店铺二维码（<c>qrcode/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopQrcodeResponse : ChannelsResponse
{
    /// <summary>获取或设置店铺二维码链接（官方 <c>shop_qrcode</c>）。</summary>
    [JsonPropertyName("shop_qrcode")]
    public string? ShopQrcode { get; set; }
}

/// <summary>获取店铺口令（<c>taglink/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Shop")]
public class ChannelsShopTagLinkResponse : ChannelsResponse
{
    /// <summary>获取或设置店铺微信口令（官方 <c>shop_taglink</c>，形如 <c>#微信小店://微信小店/xxx</c>）。</summary>
    [JsonPropertyName("shop_taglink")]
    public string? ShopTagLink { get; set; }
}

/// <summary>店铺状态常量（官方 <c>status</c> 枚举值，<c>mmecapi_basicinfo</c>）。</summary>
public static class ChannelsShopStatuses
{
    /// <summary>开店中。</summary>
    public const string Opening = "opening";

    /// <summary>已开店。</summary>
    public const string OpenFinished = "open_finished";

    /// <summary>关店中。</summary>
    public const string Closing = "closing";

    /// <summary>已关店。</summary>
    public const string CloseFinished = "close_finished";
}

/// <summary>店铺类型常量（官方 <c>subject_type</c> 枚举值，<c>mmecapi_basicinfo</c>；值照抄官方中文原文）。</summary>
public static class ChannelsShopSubjectTypes
{
    /// <summary>企业类型。</summary>
    public const string Enterprise = "企业";

    /// <summary>个体工商户类型。</summary>
    public const string IndividualBusiness = "个体工商户";
}

/// <summary>店铺二维码类型常量（官方 <c>qrcode_type</c> 枚举值，<c>qrcode/get</c>）。</summary>
public static class ChannelsShopQrcodeTypes
{
    /// <summary>二维码。</summary>
    public const int Qrcode = 1;

    /// <summary>标准物料。</summary>
    public const int StandardMaterial = 2;

    /// <summary>送礼物物料。</summary>
    public const int GiftMaterial = 3;
}
