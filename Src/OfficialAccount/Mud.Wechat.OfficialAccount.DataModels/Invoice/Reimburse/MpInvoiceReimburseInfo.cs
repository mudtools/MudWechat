// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 报销发票信息（<c>getinvoiceinfo</c> 的<b>响应体</b>与 <c>getinvoicebatch</c> 的
/// <b><c>item_list[]</c> 条目</b>字段集一致 ⇒ <b>共用本类型</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决</b>：单张查询的响应为<b>平级</b>业务字段（+ 根 <c>errcode</c>/<c>errmsg</c>），
/// 批量查询的条目为<b>同一组业务字段</b>（无 <c>errcode</c>）——两者字段集<b>完全一致</b>，
/// 故本类型继承 <see cref="MpResponse"/>（条目位置的 <c>errcode</c> 缺省为 0，天然成立）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：批量返回示例中还出现未在字段表列出的 <c>order_id</c> ⇒ SDK <b>不建模</b>
/// （示例独有且无字段表形态说明）。
/// </para>
/// <para>官方错误码：<c>0</c>（官方把成功码也列入表，照录）/ <c>72015</c> / <c>72017</c> / <c>72023</c> / <c>72024</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseInfo : MpResponse
{
    /// <summary>获取或设置发票卡券 id（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置开始时间（官方 <c>begin_time</c>；官方未说明单位）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>；官方未说明单位）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置发票类型（官方 <c>type</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置收款方（官方 <c>payee</c>）。</summary>
    [JsonPropertyName("payee")]
    public string? Payee { get; set; }

    /// <summary>获取或设置发票详情（官方 <c>detail</c>；官方未给出结构说明）。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>获取或设置用户发票信息（官方 <c>user_info</c>；与插卡请求的 <c>invoice_user_data</c> 同构 ⇒ 共用 <see cref="MpInvoiceUserData"/>）。</summary>
    [JsonPropertyName("user_info")]
    public MpInvoiceUserData? UserInfo { get; set; }
}
