// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

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
