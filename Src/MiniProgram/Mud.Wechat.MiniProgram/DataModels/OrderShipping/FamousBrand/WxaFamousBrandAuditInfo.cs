// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「品牌申请材料」（请求侧 <c>Application.audit_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>条件必填三连</b>（官方原文）：<c>flagship_in_which_ec_platform</c> 与
/// <c>ec_platform_proof_list</c> 在「品牌类型 = 电商平台旗舰店（<c>4</c>）」时必填；
/// <c>authority_certified_proof_list</c> 在「有关部门认定的驰名商标、著名商标（<c>7</c>）」时必填。</para>
/// <para><b>三个材料数组的元素是 media_id 字符串</b>（官方示例逐字为临时素材 id 形态，非对象数组）
/// ⇒ 走「新增临时素材」上传后取 <c>media_id</c>。</para>
/// <para><b>与响应侧 audit_info 不是同一类型</b>：响应侧只有 <c>audit_reason</c>
/// （见 <see cref="WxaFamousBrandAuditResult"/>）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_famousbrandapply.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaFamousBrandAuditInfo
{
    /// <summary>brand_name（官方类型 <c>string</c>，必填）：品牌名称</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>brand_type（官方类型 <c>number</c>，必填）：品牌类型。取值见 <see cref="WxaFamousBrandTypeValues"/></summary>
    [JsonPropertyName("brand_type")]
    public long? BrandType { get; set; }

    /// <summary>flagship_in_which_ec_platform（官方类型 <c>string</c>）：当品牌类型是电商平台旗舰店为必填，表示该品牌是哪个电商平台的旗舰店</summary>
    [JsonPropertyName("flagship_in_which_ec_platform")]
    public string? FlagshipInWhichEcPlatform { get; set; }

    /// <summary>ec_platform_proof_list（官方类型 <c>array</c>）：当品牌类型是电商平台旗舰店为必填，表示该品牌为官方旗舰店的佐证材料，使用「新增临时素材」上传图片</summary>
    [JsonPropertyName("ec_platform_proof_list")]
    public List<string>? EcPlatformProofList { get; set; }

    /// <summary>other_material_list（官方类型 <c>array</c>）：其他补充说明材料，使用「新增临时素材」上传图片</summary>
    [JsonPropertyName("other_material_list")]
    public List<string>? OtherMaterialList { get; set; }

    /// <summary>authority_certified_proof_list（官方类型 <c>array</c>）：当品牌类型是有关部门认定的驰名商标时为必填，表示该品牌是有关部门认定的驰名商标、著名商标的佐证材料，使用「新增临时素材」上传图片</summary>
    [JsonPropertyName("authority_certified_proof_list")]
    public List<string>? AuthorityCertifiedProofList { get; set; }
}
