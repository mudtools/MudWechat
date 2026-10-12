// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>运力拉取商家寄件对账文件请求体（<c>POST /cgi-bin/express/delivery/single_waybill/get_bill</c>，散单寄件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scattergetbill"/>。</para>
/// <para>
/// <b>成功时返回对账文件内容（非 JSON），失败时才返回 JSON</b>（错误信封 <c>errcode</c>/<c>errmsg</c>）⇒
/// 本端点走独立请求形态 <see cref="Mud.Wechat.MiniProgram.IWxaExpressBillService"/>（不进入 JSON 反序列化管线）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillGetBillRequest
{
    /// <summary>对账日期（<c>date</c>，必填；格式 YYYY-MM-DD）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>对账类型（<c>type</c>，必填；ALL 全部 / SUCCESS 成功订单 / REFUND 退款订单）。</summary>
    [JsonPropertyName("type")]
    public string? BillType { get; set; }
}
