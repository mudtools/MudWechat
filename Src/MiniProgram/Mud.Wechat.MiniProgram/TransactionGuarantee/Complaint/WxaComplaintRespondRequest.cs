// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 商家回应投诉请求体（<c>POST /wxaapi/minishop/bussiRespondComplaint</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_respondcomplaint.html"/>。
/// </para>
/// <para>
/// <b>与 <see cref="WxaComplaintProofRequest"/> 不共用的理由</b>：本端点在其三字段之外多一个
/// <see cref="BussiHandle"/>（官方参数表必填），字段集不等 ⇒ 独立成类型，
/// 不得为「少一个类」而把 <c>bussiHandle</c> 塞进共用请求体（那会让补充凭证 / 申诉两端点携带官方不接受的字段）。
/// </para>
/// <para>
/// <b>状态前置</b>：官方错误码 <c>309</c> 即「状态不对（可能已转到下一个状态）」，解决方案栏写「开发者自行校验投诉单状态」
/// ⇒ 回应前应先调查询投诉单详情，确认 <c>status</c> 处于待商家处理段（如 201 待处理）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaComplaintRespondRequest
{
    /// <summary>回应的内容（<c>content</c>，官方原文与 <see cref="MediaIdList"/> 二选一）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>投诉单号（<c>complaintOrderId</c>，官方类型 number）。</summary>
    [JsonPropertyName("complaintOrderId")]
    public long? ComplaintOrderId { get; set; }

    /// <summary>图片 id 列表（<c>mediaIdList</c>，官方类型 array；元素需先经「新增素材」获得，与 <see cref="Content"/> 二选一）。</summary>
    [JsonPropertyName("mediaIdList")]
    public List<string>? MediaIdList { get; set; }

    /// <summary>
    /// 商家处理态度（<c>bussiHandle</c>；官方原文「1 是同意和解，2 是拒绝和解」，
    /// 取值见 <see cref="WxaComplaintBussiHandle"/>）。
    /// </summary>
    [JsonPropertyName("bussiHandle")]
    public long? BussiHandle { get; set; }
}
