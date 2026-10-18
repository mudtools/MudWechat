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
/// 提审应答（<c>POST /shop/audit/audit_brand</c> 与 <c>POST /shop/audit/audit_category</c> 共用，回包顶层 <c>audit_id</c>）。
/// </summary>
/// <remarks>
/// <para><b>两支提审端点的应答形状完全相同</b>（官方逐页回包示例均为 <c>{errcode, errmsg, audit_id}</c>）⇒ 建一支共用类型，
/// 而非两支各一个空壳（守卫 SHOPA5 锁定复用关系）。</para>
/// <para><b><c>audit_id</c> 在本域内类型两处不一致（官方原文差异，非本仓建模选择）</b>：
/// 提审回包参数表写作 string 且示例为 <c>"RQAAAPX1nuJsAAAAFTrfXW"</c>（非定长串），
/// 而 <c>audit/result</c> 的请求参数表把同名键写作 number ⇒ 本仓两处一律建为 <see cref="string"/>
/// （数值化无法承载官方示例值，且会让轮询永远传不回去），差异留档于 <see cref="WxaShopAuditResultRequest"/>。</para>
/// <para><b>拿到 <c>audit_id</c> 不等于审核通过</b>：结果须走「品牌审核回调 / 类目审核回调」或
/// <c>POST /shop/audit/result</c> 轮询（守卫 SHOPA5 记录该「成功仅代表受理」语义）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAuditIdResponse : WxaResponse
{
    /// <summary>审核单 id（<c>audit_id</c>，官方类型列 string，示例 <c>RQAAAPX1nuJsAAAAFTrfXW</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public string? AuditId { get; set; }
}
