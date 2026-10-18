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
/// 查询审核结果请求（<c>POST /shop/audit/result</c>）。
/// </summary>
/// <remarks>
/// <para><b>本端点同时服务品牌审核与类目审核两张单</b>（官方原文「根据审核 id，查询品牌和类目的审核结果」）⇒
/// 传入 <c>audit_brand</c> 或 <c>audit_category</c> 返回的 <c>audit_id</c> 皆可，无第三支查询端点。</para>
/// <para><b><c>audit_id</c> 建模为字符串（官方本页类型列写 number，属官方自身的表内矛盾）</b>：
/// 提审侧 <c>audit_id</c> 官方类型为 string、示例 <c>"RQAAAPX1nuJsAAAAFTrfXW"</c> 是非定长串（无法数值化），
/// 本仓读写两侧一律 <see cref="string"/>，守卫 SHOPA5 锁定「不得回改成数值」。
/// 官方本页示例 <c>"HIDFUSHJRIYAAAAAAAA"</c> 同样是字符串字面量，可佐证类型列笔误。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAuditResultRequest
{
    /// <summary>提交审核时返回的审核单 id（<c>audit_id</c>，<b>必填</b>；取自 <see cref="WxaShopAuditIdResponse.AuditId"/>）。</summary>
    [JsonPropertyName("audit_id")]
    public string? AuditId { get; set; }
}
