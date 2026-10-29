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
/// 审核结果内容体（官方 <c>audit/result</c> 回包 <c>data</c>）。
/// </summary>
/// <remarks>
/// <para><b>审核成功后拿到的 <c>brand_id</c> 才是可上架商品的凭据</b>：官方本页回包表原文
/// 「如果是品牌审核，返回 <c>brand_id</c>」，与「品牌审核回调中给出 <c>brand_id</c>」互为两条通路。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAuditResultInfo
{
    /// <summary>
    /// 审核状态（<c>status</c>，number）。官方取值：<c>0</c> 审核中 / <c>1</c> 审核成功 / <c>9</c> 审核拒绝
    /// （注意与 <c>/shop/register/check</c> 的场景状态取值表不同）。
    /// </summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>品牌 ID（<c>brand_id</c>，number；仅品牌审核单回填，类目审核单官方示例回 <c>0</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>拒绝原因（<c>reject_reason</c>，string；官方原文「如果审核拒绝，返回拒绝原因」，示例「请重新提交审核」）。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}
