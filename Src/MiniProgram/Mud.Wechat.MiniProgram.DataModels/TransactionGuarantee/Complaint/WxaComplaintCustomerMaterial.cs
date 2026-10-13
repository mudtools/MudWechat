// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>投诉信息（<c>complaintOrder.customerMaterial</c>）。</summary>
/// <remarks>
/// <b>图片链接有时效（官方原文）</b>：「出于安全性考虑，目前该图片 url 有过期时间，如需查看图片，
/// 需每次重新调用该接口获取，防止图片过期导致无法查看」⇒ <see cref="MediaIdList"/> 不得缓存复用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaComplaintCustomerMaterial
{
    /// <summary>投诉内容（<c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>投诉内容图片 cdn 列表（<c>mediaIdList</c>，官方类型 array；有过期时间，须每次重新获取）。</summary>
    [JsonPropertyName("mediaIdList")]
    public List<string>? MediaIdList { get; set; }
}
