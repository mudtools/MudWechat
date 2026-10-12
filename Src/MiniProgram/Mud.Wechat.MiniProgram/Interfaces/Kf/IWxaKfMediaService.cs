// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「客服」域临时素材通道（1 端点：<c>media/get</c>）—— <b>独立请求形态</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不是 <c>[HttpClientApi]</c> 接口</b>：本端点成功时返回<b>素材二进制流</b>（图片 / 语音等），
/// 失败时才返回 JSON —— 与小程序码、用户反馈图片同形，必须走
/// <c>IBaseHttpClient.SendRawAsync</c> 原始响应直读 + <b>Content-Type 分支判错</b>
/// （实现见 <see cref="WxaKfMediaService"/>，同 <see cref="IWxaCodeService"/> / <see cref="IWxaFeedbackMediaService"/> 口径）。
/// </para>
/// <para>
/// <b>为何本线要建这条端点（MP-X1 核对结论）</b>：公众号线<b>只有</b> <c>/cgi-bin/media/upload</c>，
/// <b>没有</b> <c>/cgi-bin/media/get</c> —— 本端点在公众号线<b>无重叠</b>，属小程序线的真实缺口
/// （2026-10-12 依官方清单核对）。
/// </para>
/// <para>
/// <b>令牌</b>：手工取当前应用令牌拼 Query（<c>access_token</c>），<b>不进</b> Query 令牌白名单
/// （白名单只收 <c>[Token]</c> 声明式接口）；令牌失效（<c>40001</c> / <c>40014</c> / <c>42001</c>）时
/// 失效本应用令牌并重试一次。
/// </para>
/// <para>
/// <b>视频素材是特例（官方原文）</b>：<c>media/get</c> 对<b>视频</b>素材返回 JSON
/// <c>{"video_url":"DOWN_URL"}</c> 而非二进制流；其余素材类型返回二进制。
/// 故调用方须按 <see cref="WxaCodeResult.ContentType"/> 自行分支：命中 <c>json</c> 时解析 <c>video_url</c>。
/// </para>
/// </remarks>
public interface IWxaKfMediaService
{
    /// <summary>
    /// 获取临时素材（<c>GET /cgi-bin/media/get?access_token=…&amp;media_id=…</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/kf-mgnt/kf-message/api_getmedia"/>。
    /// </summary>
    /// <param name="mediaId">媒体文件 ID（官方 <c>media_id</c>，必填；由 <c>/cgi-bin/media/upload</c> 上传后获得）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>
    /// 素材内容，见 <see cref="WxaCodeResult"/>：<b>非视频</b>为二进制流；<b>视频</b>为含 <c>video_url</c> 的 JSON
    /// （调用方按 <see cref="WxaCodeResult.ContentType"/> 分支解析）。失败时抛 <see cref="Abstractions.WxaException"/>。
    /// </returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，<b>无请求体</b>；Query 携带 <c>access_token</c> 与 <c>media_id</c>。
    /// </para>
    /// <para>
    /// 官方错误码要点：<c>40007</c>（invalid media_id，无效的媒体 ID）；其余见通用错误码。
    /// </para>
    /// </remarks>
    Task<WxaCodeResult> GetMediaAsync(
        string mediaId,
        CancellationToken cancellationToken = default);
}
