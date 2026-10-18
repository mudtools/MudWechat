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
/// 品牌审核的品牌信息（官方 <c>audit_req.brand_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>必填集是「四列矩阵」而非单一必填标记</b>（官方参数表按 自有品牌 R 标 / 自有品牌 TM 标 /
/// 代理品牌 R 标 / 代理品牌 TM 标 四列分别标注是否必填）：
/// 四种类型都必填的有 <c>license</c>、<c>brand_audit_type</c>、<c>trademark_type</c>、
/// <c>brand_management_type</c>、<c>commodity_origin_type</c>、<c>brand_wording</c>、
/// <c>trademark_registrant_nu</c>、<c>scene_group_list</c>；
/// 「商标注册证 <c>trademark_registration_certificate</c>」与「商标注册人 <c>trademark_registrant</c>」「商标有效期
/// <c>trademark_authorization_period</c>」为 R 标必填、TM 标不填；
/// 「商标注册申请受理通知书 <c>trademark_registration_application</c>」「商标申请人 <c>trademark_applicant</c>」
/// 「商标申请时间 <c>trademark_application_time</c>」为 TM 标必填；
/// 「销售授权书 <c>sale_authorization</c>」仅代理品牌两支必填。本类把全部字段建模为可空，
/// 必填集交由宿主按品牌类型组装（守卫 SHOPA5 只锁字段名与类型）。</para>
/// <para><b>官方类型列与示例列矛盾处（诚实留档）</b>：<c>brand_audit_type</c> / <c>brand_management_type</c> /
/// <c>commodity_origin_type</c> 官方类型列写 <c>uint32</c> 但说明列写「字符类型，最长不超过 N」⇒
/// 本仓按类型列建为数值；<c>trademark_type</c> 官方类型列写 <c>string</c> 且示例为 <c>"29"</c>（带引号）⇒ 建为字符串。
/// 另 <c>trademark_registrant</c> 官方说明为「商标注册人<b>姓名</b>」（最长 100 字符），
/// 但请求示例给的是图片 URL ⇒ 键名与类型照参数表（string），示例异常仅留档。</para>
/// <para><b>敏感面</b>：商标权人姓名、注册号与各类资质图片链接属实名/证照信息，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopBrandAuditInfo
{
    /// <summary>
    /// 认证审核类型（<c>brand_audit_type</c>，官方 RegisterType 枚举，四列均必填；官方类型列 uint32）。
    /// <c>1</c> 国内品牌申请-R 标 / <c>2</c> 国内品牌申请-TM 标 / <c>3</c> 海外品牌申请-R 标 / <c>4</c> 海外品牌申请-TM 标。
    /// </summary>
    [JsonPropertyName("brand_audit_type")]
    public long? BrandAuditType { get; set; }

    /// <summary>
    /// 商标分类（<c>trademark_type</c>，string，四列均必填）：官方 TrademarkType 为「1」~「45」共 45 个分类，
    /// 官方原文「如第 45 类商标，上传 45 即可」；官方类型列为 string 且示例带引号 ⇒ 建为字符串。
    /// </summary>
    [JsonPropertyName("trademark_type")]
    public string? TrademarkType { get; set; }

    /// <summary>
    /// 品牌经营类型（<c>brand_management_type</c>，官方 BrandManagementType，四列均必填）：
    /// <c>1</c> 自有品牌 / <c>2</c> 代理品牌 / <c>3</c> 无品牌。
    /// </summary>
    [JsonPropertyName("brand_management_type")]
    public long? BrandManagementType { get; set; }

    /// <summary>
    /// 商品产地是否进口（<c>commodity_origin_type</c>，官方 CommodityOriginType，四列均必填）：
    /// <c>1</c> 是 / <c>2</c> 否（官方枚举表即「是/否」两值，无 0）。
    /// </summary>
    [JsonPropertyName("commodity_origin_type")]
    public long? CommodityOriginType { get; set; }

    /// <summary>商标 / 品牌词（<c>brand_wording</c>，string，四列均必填，官方限最长 50 字符）。</summary>
    [JsonPropertyName("brand_wording")]
    public string? BrandWording { get; set; }

    /// <summary>
    /// 销售授权书（<c>sale_authorization</c>，string 数组：图片 url / media_id，最多 10 张）。
    /// 仅代理品牌两支必填；官方原文：商标持有人为自然人时还需提供有其签名的身份证正反面扫描件（敏感，禁落日志）。
    /// </summary>
    [JsonPropertyName("sale_authorization")]
    public List<string>? SaleAuthorization { get; set; }

    /// <summary>商标注册证书（<c>trademark_registration_certificate</c>，string 数组，<b>最多 1 张</b>；R 标必填）。</summary>
    [JsonPropertyName("trademark_registration_certificate")]
    public List<string>? TrademarkRegistrationCertificate { get; set; }

    /// <summary>商标变更证明（<c>trademark_change_certificate</c>，string 数组，最多 5 张；官方四列均标「否」）。</summary>
    [JsonPropertyName("trademark_change_certificate")]
    public List<string>? TrademarkChangeCertificate { get; set; }

    /// <summary>
    /// 商标注册人（<c>trademark_registrant</c>，string，最长 100 字符；R 标必填）。
    /// 官方说明列为「商标注册人姓名」，而请求示例给的是图片 URL ⇒ 键名与类型照参数表，示例异常已在类 remarks 留档。
    /// </summary>
    [JsonPropertyName("trademark_registrant")]
    public string? TrademarkRegistrant { get; set; }

    /// <summary>商标注册号 / 申请号（<c>trademark_registrant_nu</c>，string，最长 10 字符，四列均必填）。注意键名以 <c>_nu</c> 结尾，官方原文如此。</summary>
    [JsonPropertyName("trademark_registrant_nu")]
    public string? TrademarkRegistrantNu { get; set; }

    /// <summary>商标有效期（<c>trademark_authorization_period</c>，string，格式 <c>yyyy-MM-dd HH:mm:ss</c>，最长 30 字符；R 标必填）。</summary>
    [JsonPropertyName("trademark_authorization_period")]
    public string? TrademarkAuthorizationPeriod { get; set; }

    /// <summary>商标注册申请受理通知书（<c>trademark_registration_application</c>，string 数组，<b>最多 1 张</b>；TM 标必填）。</summary>
    [JsonPropertyName("trademark_registration_application")]
    public List<string>? TrademarkRegistrationApplication { get; set; }

    /// <summary>商标申请人姓名（<c>trademark_applicant</c>，string，最长 100 字符；TM 标必填）。敏感，禁落日志。</summary>
    [JsonPropertyName("trademark_applicant")]
    public string? TrademarkApplicant { get; set; }

    /// <summary>商标申请时间（<c>trademark_application_time</c>，string，格式 <c>yyyy-MM-dd HH:mm:ss</c>，最长 30 字符；TM 标必填）。</summary>
    [JsonPropertyName("trademark_application_time")]
    public string? TrademarkApplicationTime { get; set; }

    /// <summary>中华人民共和国海关进口货物报关单（<c>imported_goods_form</c>，string 数组，最多 5 张；官方四列均标「否」）。</summary>
    [JsonPropertyName("imported_goods_form")]
    public List<string>? ImportedGoodsForm { get; set; }
}
