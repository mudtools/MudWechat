// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 投诉处理「文本 / 图片二选一」请求体（<c>content</c> + <c>complaintOrderId</c> + <c>mediaIdList</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决（两页参数表字段集完全一致）</b>：官方「商家补充凭证」<c>POST /wxaapi/minishop/bussiSupplyProof</c>
/// 与「商家申诉」<c>POST /wxaapi/minishop/busiAppeal</c> 的 Request Payload 三个字段名、类型、必填项
/// 逐字相同（<c>content</c> string、<c>complaintOrderId</c> number、<c>mediaIdList</c> array）
/// ⇒ 本类型由两端点共用，不再各造一份（对齐 Vip 域 <c>WxaVipOpenidRequest</c> 的共用先例）。
/// </para>
/// <para>
/// <b>二选一语义（官方原文）</b>：「图片数组 mediaIdList 跟内容 content 二选一」⇒ 两者虽在参数表同标「必填」，
/// 实际按业务语义至少提供其一；官方错误码 <c>2</c>（仅 busiAppeal 列出）即「传入 mediaIdList 或 content 格式有误」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaComplaintProofRequest
{
    /// <summary>回应的内容（<c>content</c>，与 <see cref="MediaIdList"/> 二选一）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>投诉单号（<c>complaintOrderId</c>，官方类型 number；由查询投诉单详情接口取得）。</summary>
    [JsonPropertyName("complaintOrderId")]
    public long? ComplaintOrderId { get; set; }

    /// <summary>
    /// 图片 id 列表（<c>mediaIdList</c>，官方类型 array；元素为素材 id，需先经「新增素材」获得，
    /// 与 <see cref="Content"/> 二选一）。
    /// </summary>
    [JsonPropertyName("mediaIdList")]
    public List<string>? MediaIdList { get; set; }
}
