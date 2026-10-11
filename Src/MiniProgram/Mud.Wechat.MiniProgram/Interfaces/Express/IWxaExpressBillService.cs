// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.Express;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「物流助手 · 商家寄件对账文件」通道（1 端点：<c>single_waybill/get_bill</c>）—— <b>独立请求形态</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不是 <c>[HttpClientApi]</c> 接口</b>：<c>get_bill</c> 成功时返回<b>对账文件内容（文本，非 JSON）</b>，
/// 失败时才返回 JSON 错误信封（<c>errcode</c>/<c>errmsg</c>）⇒ 必须走原始响应直读 + Content-Type 分支判错
/// （同 <see cref="IWxaFeedbackMediaService"/> / <see cref="IWxaCodeService"/> 的手动通道形态）。
/// </para>
/// <para>
/// <b>令牌</b>：手工取当前应用令牌拼 Query（<c>access_token</c>），<b>不进</b> Query 令牌白名单
/// （白名单只收 <c>[Token]</c> 声明式接口）；令牌失效（<c>40001</c> / <c>40014</c> / <c>42001</c>）时失效本应用令牌并重试一次。
/// </para>
/// <para>
/// <b>调用方</b>：面向<b>微信物流服务商</b>；请求体 <see cref="WxaExpressSingleWaybillGetBillRequest"/> 中
/// <c>date</c> 为对账日期（格式 YYYY-MM-DD）、<c>type</c> 为对账类型（ALL 全部 / SUCCESS 成功订单 / REFUND 退款订单）。
/// </para>
/// </remarks>
public interface IWxaExpressBillService
{
    /// <summary>
    /// 拉取商家寄件对账文件（<c>POST /cgi-bin/express/delivery/single_waybill/get_bill</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scattergetbill"/>。
    /// </summary>
    /// <param name="request">对账文件请求体（<c>date</c>/<c>type</c> 必填），见 <see cref="WxaExpressSingleWaybillGetBillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>对账文件文本内容（成功时）；失败时抛 <see cref="Abstractions.WxaException"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；成功响应为文本文件（非 JSON）。</para>
    /// <para>对账类型 <c>type</c>：ALL 全部 / SUCCESS 成功订单 / REFUND 退款订单。</para>
    /// </remarks>
    Task<string> GetBillAsync(
        WxaExpressSingleWaybillGetBillRequest request,
        CancellationToken cancellationToken = default);
}
