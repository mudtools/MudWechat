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
/// 类目审核请求（<c>POST /shop/audit/audit_category</c>）。
/// </summary>
/// <remarks>
/// <para>与 <see cref="WxaShopBrandAuditRequest"/> 同为 <c>audit_req</c> 一层包装，仅内层对象不同
/// （品牌侧 <c>brand_info</c> ↔ 类目侧 <c>category_info</c>）⇒ 两个请求类型不合并。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopCategoryAuditRequest
{
    /// <summary>审核请求体（<c>audit_req</c>，object，必填）。</summary>
    [JsonPropertyName("audit_req")]
    public WxaShopCategoryAuditReq? AuditReq { get; set; }
}
