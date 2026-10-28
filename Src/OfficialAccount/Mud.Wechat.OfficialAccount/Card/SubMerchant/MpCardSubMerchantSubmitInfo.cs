// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

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
