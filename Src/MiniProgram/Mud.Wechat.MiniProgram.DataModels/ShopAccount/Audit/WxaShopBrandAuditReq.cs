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
/// 品牌审核请求体（官方 <c>audit_req</c> 一层，<c>audit_brand</c> 侧）。
/// </summary>
/// <remarks>
/// <para><b>资质材料一次提交即成审核单</b>：官方原文「请求成功后将会创建一个审核单，单号将在回包中给出」
/// 「审核完成后会进行回调，告知审核结果，请接入品牌审核回调，如果审核成功，则在回调中给出 <c>brand_id</c>」
/// ⇒ 本端点<b>不等价于「品牌已开通」</b>，同步应答只有 <c>audit_id</c>。</para>
/// <para><c>"scene_group_list":[]</c> 的取值口径见 <see cref="SceneGroupList"/>。</para>
/// <para><b>敏感面（AGENTS §8）</b>：<c>license</c> 与 <c>brand_info</c> 内的资质材料地址、商标注册人 /
/// 商标申请人姓名（自然人场景含身份证扫描件）属个人信息 ⇒ 不得写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopBrandAuditReq
{
    /// <summary>
    /// 营业执照或组织机构代码证（<c>license</c>，string 数组，四种品牌类型均<b>必填</b>）。
    /// 官方原文：图片 url / media_id 皆可（media_id 经「上传图片」接口换取）；
    /// url 必须可正常打开、大小 2MB 以下、格式 jpg / jpeg / png，<b>图片打不开会直接导致审核驳回</b>。
    /// </summary>
    [JsonPropertyName("license")]
    public List<string>? License { get; set; }

    /// <summary>品牌信息（<c>brand_info</c>，object）：逐字段必填集取决于品牌类型四列矩阵，见 <see cref="WxaShopBrandAuditInfo"/>。</summary>
    [JsonPropertyName("brand_info")]
    public WxaShopBrandAuditInfo? BrandInfo { get; set; }

    /// <summary>
    /// 品牌使用场景（<c>scene_group_list</c>，enum 数组，四种品牌类型均标<b>必填</b>）。
    /// 官方取值：<c>1</c> 视频号、<c>3</c> 订单中心（非视频号订单中心）。
    /// <b>官方原文警示</b>：「未明确开通此场景的商家请勿传值」「组件开通流程中以及未接入场景时，请保持为空
    /// <c>"scene_group_list":[]</c>」⇒ 传空数组是合法且常见的形态，不得省略该键当作「未填」。
    /// </summary>
    [JsonPropertyName("scene_group_list")]
    public List<int>? SceneGroupList { get; set; }
}
