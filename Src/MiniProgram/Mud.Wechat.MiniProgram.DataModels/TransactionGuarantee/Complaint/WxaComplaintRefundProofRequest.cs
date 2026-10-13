// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 商家提交退款凭证请求体（<c>POST /wxaapi/minishop/bussiSupplyRefund</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_submitrefund.html"/>。
/// </para>
/// <para>
/// <b>退货状态下才必填</b>：官方对 <see cref="AcceptReturn"/> 与 <see cref="ReturnId"/> 的说明栏均写
/// 「（退货状态下必填）」⇒ 仅当投诉单处于「平台已判定为商责，待用户退货中」（<c>status</c> 308 / 309）
/// 或进度节点 <c>itemType</c> 31 / 32 且 <c>blameResult == 1</c> 时才须携带；
/// 非退货场景应走「商家补充凭证」而非本端点。
/// </para>
/// <para>
/// <b>官方未公布 <c>acceptReturn</c> 的枚举取值</b>：参数表只给语义「确认是否收货」，本页无枚举表。
/// 因此本字段类型为 <c>long?</c> 且<b>不建常量类</b>——凭空造 0/1 即违反「字段与取值一律照官方原文」口径。
/// 接入方须以官方联调结果为准。
/// </para>
/// <para>
/// <b><see cref="ReturnId"/> 类型口径</b>：本页参数表标 <c>number</c>，而查询投诉单详情应答里
/// <c>returnBill.returnId</c> 标 <c>string</c> ⇒ 出参 <c>string</c> / 入参 <c>long?</c> 各自照本页原文，
/// 官方两页口径不一致已照录。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaComplaintRefundProofRequest
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

    /// <summary>确认是否收货（<c>acceptReturn</c>，官方类型 number，退货状态下必填；官方未公布枚举取值）。</summary>
    [JsonPropertyName("acceptReturn")]
    public long? AcceptReturn { get; set; }

    /// <summary>退货单号（<c>returnId</c>，官方类型 number，退货状态下必填；须由查询投诉单详情接口的 <c>returnBill.returnId</c> 取得）。</summary>
    [JsonPropertyName("returnId")]
    public long? ReturnId { get; set; }
}
