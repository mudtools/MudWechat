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
/// 品牌审核请求（<c>POST /shop/audit/audit_brand</c>，官方文档 <c>platform-capabilities/shop-1/API/audit/audit_brand.html</c>）。
/// </summary>
/// <remarks>
/// <para><b>请求体是 <c>audit_req</c> 单键包装</b>：官方请求示例与参数表均以 <c>audit_req.</c> 为前缀
/// （<c>audit_req.license</c> / <c>audit_req.brand_info.*</c> / <c>audit_req.scene_group_list</c>）⇒
/// 扁平直传会命中 <c>errcode=40097</c>（参数错误）一类失败，守卫 SHOPA5 锁定包装键名。</para>
/// <para><b>与类目审核侧 <see cref="WxaShopCategoryAuditRequest"/> 同层不同形</b>：两支包装键同名（<c>audit_req</c>），
/// 内层第三支字段分别是 <c>brand_info</c> 与 <c>category_info</c> ⇒ 两个请求类型不得合并，
/// 元素类也不得互抄（<see cref="WxaShopBrandAuditReq"/> ↔ <see cref="WxaShopCategoryAuditReq"/>）。</para>
/// <para><b>本端点不是幂等更新</b>：官方原文「请求成功后将会创建一个审核单，单号将在回包中给出」⇒
/// 每次调用产生一张新审核单，同步应答只有 <c>audit_id</c>；结果经品牌审核回调或 <c>/shop/audit/result</c> 取得，
/// 「<c>errcode=0</c>」绝不等于「品牌已开通」。</para>
/// <para><b>敏感面（AGENTS §8）</b>：营业执照、商标注册证、销售授权书、海关进口货物报关单等资质材料地址，
/// 以及商标注册人 / 商标申请人姓名（自然人场景含身份证扫描件）⇒ 不得写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopBrandAuditRequest
{
    /// <summary>品牌审核请求体（<c>audit_req</c>，object，<b>必填</b>），见 <see cref="WxaShopBrandAuditReq"/>。</summary>
    [JsonPropertyName("audit_req")]
    public WxaShopBrandAuditReq? AuditReq { get; set; }
}
