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
/// 三级类目及其资质（官方 <c>/shop/cat/get</c> 回包 <c>third_cat_list[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>两支「资质类型」字段是本端点的核心判据</b>（官方取值表：0 不需要 / 1 必填 / 2 选填）：
/// 类目资质必填 ⇒ 新增商品前必须先走 <c>/shop/audit/audit_category</c> 提审；
/// 类目资质不需要 ⇒ 该类目自动拥有，<b>如依然调用提审接口会报错 <c>1050011</c></b>（官方原文）；
/// 商品资质必填 ⇒ 新增商品时必须带上商品资质字段。</para>
/// <para><b>审核结果不在本接口里</b>：官方原文「接入类目审核回调，才可获取审核结果」（轮询侧另有
/// <c>/shop/audit/result</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopThirdCategory
{
    /// <summary>三级类目 ID（<c>third_cat_id</c>，number；即 <c>audit_category</c> 的 <c>level3</c>）。</summary>
    [JsonPropertyName("third_cat_id")]
    public long? ThirdCatId { get; set; }

    /// <summary>三级类目名称（<c>third_cat_name</c>，string）。</summary>
    [JsonPropertyName("third_cat_name")]
    public string? ThirdCatName { get; set; }

    /// <summary>类目资质说明文本（<c>qualification</c>，string；官方示例为空串 ⇒ 空串与缺省皆可能出现）。</summary>
    [JsonPropertyName("qualification")]
    public string? Qualification { get; set; }

    /// <summary>
    /// 类目资质要求（<c>qualification_type</c>，number）：<c>0</c> 不需要 / <c>1</c> 必填 / <c>2</c> 选填。
    /// 为 <c>0</c> 时再调 <c>audit_category</c> 会报 <c>1050011</c>。
    /// </summary>
    [JsonPropertyName("qualification_type")]
    public long? QualificationType { get; set; }

    /// <summary>商品资质说明文本（<c>product_qualification</c>，string；官方示例「《国家强制性产品认证证书》（CCC安全认证证书）」）。</summary>
    [JsonPropertyName("product_qualification")]
    public string? ProductQualification { get; set; }

    /// <summary>商品资质要求（<c>product_qualification_type</c>，number）：<c>0</c> 不需要 / <c>1</c> 必填 / <c>2</c> 选填。</summary>
    [JsonPropertyName("product_qualification_type")]
    public long? ProductQualificationType { get; set; }

    /// <summary>一级类目 ID（<c>first_cat_id</c>，number）。</summary>
    [JsonPropertyName("first_cat_id")]
    public long? FirstCatId { get; set; }

    /// <summary>一级类目名称（<c>first_cat_name</c>，string；官方示例「玩具乐器」）。</summary>
    [JsonPropertyName("first_cat_name")]
    public string? FirstCatName { get; set; }

    /// <summary>二级类目 ID（<c>second_cat_id</c>，number）。</summary>
    [JsonPropertyName("second_cat_id")]
    public long? SecondCatId { get; set; }

    /// <summary>二级类目名称（<c>second_cat_name</c>，string；官方示例「健身玩具」）。</summary>
    [JsonPropertyName("second_cat_name")]
    public string? SecondCatName { get; set; }
}
