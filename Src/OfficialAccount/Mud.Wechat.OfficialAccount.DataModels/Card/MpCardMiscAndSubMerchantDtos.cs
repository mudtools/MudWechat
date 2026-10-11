// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 「获取卡券图文 HTML」请求（官方 <c>mpnews/gethtml</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMpNewsGetHtmlRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}

/// <summary>
/// 「获取卡券图文 HTML」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardMpNewsGetHtmlResponse : MpResponse
{
    /// <summary>获取或设置图文 HTML 内容（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 「查询门店小程序卡券」请求（官方 <c>storewxa/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardStoreWxaGetRequest
{
    /// <summary>获取或设置门店 POI ID（官方 <c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;
}

/// <summary>
/// 「查询门店小程序卡券」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardStoreWxaGetResponse : MpResponse
{
    /// <summary>获取或设置门店绑定的卡券 ID（官方 <c>card_id</c>，未绑定时缺省）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}

/// <summary>
/// 「设置门店小程序卡券」请求（官方 <c>storewxa/set</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardStoreWxaSetRequest
{
    /// <summary>获取或设置门店 POI ID（官方 <c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;

    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;
}

/// <summary>
/// 子商户二级类目（官方 <c>secondary_category</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantSecondaryCategory
{
    /// <summary>获取或设置二级类目 ID（官方 <c>secondary_category_id</c>）。</summary>
    [JsonPropertyName("secondary_category_id")]
    public int SecondaryCategoryId { get; set; }

    /// <summary>获取或设置二级类目名称（官方 <c>category_name</c>）。</summary>
    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    /// <summary>获取或设置需要资质证明文件（官方 <c>need_qualification_stuffs</c>，可选）。</summary>
    [JsonPropertyName("need_qualification_stuffs")]
    public string[]? NeedQualificationStuffs { get; set; }
}

/// <summary>
/// 子商户一级类目（官方 <c>category</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantPrimaryCategory
{
    /// <summary>获取或设置一级类目 ID（官方 <c>primary_category_id</c>）。</summary>
    [JsonPropertyName("primary_category_id")]
    public int PrimaryCategoryId { get; set; }

    /// <summary>获取或设置一级类目名称（官方 <c>category_name</c>）。</summary>
    [JsonPropertyName("category_name")]
    public string? CategoryName { get; set; }

    /// <summary>获取或设置二级类目列表（官方 <c>secondary_category</c>）。</summary>
    [JsonPropertyName("secondary_category")]
    public MpCardSubMerchantSecondaryCategory[]? SecondaryCategoryList { get; set; }
}

/// <summary>
/// 「查询子商户协议」响应（官方 <c>getapplyprotocol</c>；GET、无请求体——本域唯一 GET 端点）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGetApplyProtocolResponse : MpResponse
{
    /// <summary>获取或设置类目树（官方 <c>category</c>）。</summary>
    [JsonPropertyName("category")]
    public MpCardSubMerchantPrimaryCategory[]? CategoryList { get; set; }
}

/// <summary>
/// 子商户信息——提交 / 更新形态（官方 <c>info</c>；submit 8 字段，update 追加 merchant_id）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantSubmitInfo
{
    /// <summary>获取或设置子商户 ID（官方 <c>merchant_id</c>；仅 update 形态使用；官方返回数字，字符串承载）。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }

    /// <summary>获取或设置子商户 <c>appid</c>（官方 <c>app_id</c>，可选）。</summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    /// <summary>获取或设置商户品牌名（官方 <c>brand_name</c>，必填）。</summary>
    [JsonPropertyName("brand_name")]
    public string BrandName { get; set; } = string.Empty;

    /// <summary>获取或设置商户 Logo 的临时素材 ID（官方 <c>logo_url</c>，必填；经素材上传获得）。</summary>
    [JsonPropertyName("logo_url")]
    public string LogoUrl { get; set; } = string.Empty;

    /// <summary>获取或设置授权函（官方 <c>protocol</c>，必填；临时素材 ID 形态）。</summary>
    [JsonPropertyName("protocol")]
    public string Protocol { get; set; } = string.Empty;

    /// <summary>获取或设置授权函有效期（官方 <c>end_time</c>，秒级 Unix 时间戳，必填）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置授权函编号（官方 <c>agreement_media_id</c>，可选）。</summary>
    [JsonPropertyName("agreement_media_id")]
    public string? AgreementMediaId { get; set; }

    /// <summary>获取或设置操作员身份证（官方 <c>operator_media_id</c>，可选）。</summary>
    [JsonPropertyName("operator_media_id")]
    public string? OperatorMediaId { get; set; }

    /// <summary>获取或设置一级类目 ID（官方 <c>primary_category_id</c>，必填；取自 getapplyprotocol）。</summary>
    [JsonPropertyName("primary_category_id")]
    public int PrimaryCategoryId { get; set; }

    /// <summary>获取或设置二级类目 ID（官方 <c>secondary_category_id</c>，必填）。</summary>
    [JsonPropertyName("secondary_category_id")]
    public int SecondaryCategoryId { get; set; }
}

/// <summary>
/// 「创建子商户」请求（官方 <c>submerchant/submit</c>；仅第三方平台 / 代开发可用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantSubmitRequest
{
    /// <summary>获取或设置子商户信息（官方 <c>info</c>，必填）。</summary>
    [JsonPropertyName("info")]
    public MpCardSubMerchantSubmitInfo Info { get; set; } = new();
}

/// <summary>
/// 子商户信息——查询形态（官方 <c>info</c> 返回体；11 字段；与建卡面 MpCardSubMerchantInfo 同名异形，故改名 Detail 区分）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantDetail
{
    /// <summary>获取或设置子商户 ID（官方 <c>merchant_id</c>；官方返回数字，字符串承载）。</summary>
    [JsonPropertyName("merchant_id")]
    public string? MerchantId { get; set; }

    /// <summary>获取或设置子商户 <c>appid</c>（官方 <c>app_id</c>，可选）。</summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    /// <summary>获取或设置商户品牌名（官方 <c>brand_name</c>）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>获取或设置商户 Logo（官方 <c>logo_url</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    /// <summary>获取或设置审核状态（官方 <c>status</c>；CHECKING / APPROVED / REJECTED 等）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置授权函有效期起始（官方 <c>begin_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>获取或设置授权函有效期截止（官方 <c>end_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置一级类目 ID（官方 <c>primary_category_id</c>）。</summary>
    [JsonPropertyName("primary_category_id")]
    public int PrimaryCategoryId { get; set; }

    /// <summary>获取或设置二级类目 ID（官方 <c>secondary_category_id</c>）。</summary>
    [JsonPropertyName("secondary_category_id")]
    public int SecondaryCategoryId { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long UpdateTime { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }
}

/// <summary>
/// 子商户查询 / 提交 / 更新共用响应（官方 submit / update / get 三端点同构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantInfoResponse : MpResponse
{
    /// <summary>获取或设置子商户信息（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public MpCardSubMerchantDetail? Info { get; set; }
}

/// <summary>
/// 「更新子商户」请求（官方 <c>submerchant/update</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantUpdateRequest
{
    /// <summary>获取或设置子商户信息（官方 <c>info</c>，必填；须携带 merchant_id）。</summary>
    [JsonPropertyName("info")]
    public MpCardSubMerchantSubmitInfo Info { get; set; } = new();
}

/// <summary>
/// 「查询子商户」请求（官方 <c>submerchant/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantGetRequest
{
    /// <summary>获取或设置子商户 ID（官方 <c>merchant_id</c>，必填）。</summary>
    [JsonPropertyName("merchant_id")]
    public string MerchantId { get; set; } = string.Empty;
}

/// <summary>
/// 「批量查询子商户」请求（官方 <c>submerchant/batchget</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantBatchGetRequest
{
    /// <summary>获取或设置起始 ID（官方 <c>begin_id</c>；首个查询传 0 或不传）。</summary>
    [JsonPropertyName("begin_id")]
    public string BeginId { get; set; } = "0";

    /// <summary>获取或设置拉取数量（官方 <c>limit</c>；官方默认 10，最大 50）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 10;

    /// <summary>获取或设置审核状态过滤（官方 <c>status</c>，可选；不传返回全部）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

/// <summary>
/// 「批量查询子商户」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantBatchGetResponse : MpResponse
{
    /// <summary>获取或设置子商户列表（官方 <c>info_list</c>）。</summary>
    [JsonPropertyName("info_list")]
    public MpCardSubMerchantInfo[]? InfoList { get; set; }

    /// <summary>获取或设置下一页起始 ID（官方 <c>next_begin_id</c>；官方返回数字，字符串承载）。</summary>
    [JsonPropertyName("next_begin_id")]
    public string? NextBeginId { get; set; }
}
