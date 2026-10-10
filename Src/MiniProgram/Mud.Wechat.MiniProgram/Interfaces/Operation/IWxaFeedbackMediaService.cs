// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.Operation;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「运维中心」用户反馈图片通道（1 端点：<c>getfeedbackmedia</c>）—— <b>独立请求形态</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不是 <c>[HttpClientApi]</c> 接口</b>：<c>getfeedbackmedia</c> 成功时返回<b>图片二进制流</b>
/// （<c>Content-Type: image/*</c>），失败时才返回 JSON —— 与小程序码一样必须走
/// <c>IBaseHttpClient.SendRawAsync</c> 原始响应直读 + Content-Type 分支判错（见 <see cref="IWxaCodeService"/> 同款说明）。
/// </para>
/// <para>
/// <b>令牌</b>：手工取当前应用令牌拼 Query（<c>access_token</c>），<b>不进</b> Query 令牌白名单
/// （白名单只收 <c>[Token]</c> 声明式接口）；令牌失效（<c>40001</c> / <c>40014</c> / <c>42001</c>）时失效本应用令牌并重试一次。
/// </para>
/// <para>
/// <b>路由</b>：<c>/cgi-bin/media/getfeedbackmedia</c> —— 与公众号线素材端点（<c>/cgi-bin/media/{upload,get}</c>）<b>无重叠</b>（MP-X1 断言）。
/// </para>
/// </remarks>
public interface IWxaFeedbackMediaService
{
    /// <summary>
    /// 获取用户反馈图片（<c>POST /cgi-bin/media/getfeedbackmedia</c>）。
    /// 官方文档：<c>operation/api_getfeedbackmedia.html</c>。
    /// </summary>
    /// <param name="request">反馈图片媒体 ID（<c>media_id</c> 必填），见 <see cref="WxaFeedbackMediaRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>图片内容流，见 <see cref="WxaCodeResult"/>（失败时抛 <see cref="Abstractions.WxaException"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>；成功响应为图片二进制。</para>
    /// <para><c>media_id</c> 取自 <see cref="WxaFeedbackItem.MediaIds"/>（用户反馈图片的媒体 ID）。</para>
    /// </remarks>
    Task<WxaCodeResult> GetFeedbackMediaAsync(
        WxaFeedbackMediaRequest request,
        CancellationToken cancellationToken = default);
}