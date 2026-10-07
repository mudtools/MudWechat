// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「素材管理」域下载通道（响应为二进制流的端点专用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>I3 裁决（独立请求形态，勿并入生成管线）</b>：下载端点的正常响应<b>不是 JSON</b>，
/// 错误又以「HTTP 200 + JSON errcode」形态出现 ⇒ 本服务走
/// <see cref="Mud.HttpUtils.IBaseHttpClient.SendRawAsync"/> 原始响应直读，按 <b>Content-Type</b> 分支：
/// </para>
/// <list type="bullet">
/// <item><b>JSON + errcode != 0</b>：抛 <see cref="Abstractions.Exceptions.MpException"/>；令牌失效码
/// （<c>{40001, 40014, 42001}</c>，与 <c>MpTokenInvalidationDetector</c> 集合同源）
/// 先失效本应用 access_token 并<b>重试一次</b>（镜像票据管理器的 40001 自愈先例）。</item>
/// <item><b>JSON + video_url</b>（<c>media/get</c> 对视频素材的官方形态）：
/// 返回 <see cref="MpMediaDownloadResult"/>（<see cref="MpMediaDownloadResult.VideoUrl"/> 承载下载地址）。</item>
/// <item><b>其余 Content-Type</b>：按文件流返回（<see cref="MpMediaDownloadResult.Content"/>，
/// 文件名取 <c>Content-Disposition</c>，官方未携带则为 <c>null</c>）。</item>
/// </list>
/// <para>
/// <b>多应用路由</b>：随当前 <see cref="IMpAppContext"/>（<see cref="IAppContextHolder.Current"/>）解析，
/// 与声明式客户端同源；令牌经 <see cref="IMpAppContext.AccessTokenManager"/> 显式获取（Query 注入）。
/// </para>
/// <para>
/// <b>SDK 不做本地落盘</b>：流交宿主消费与释放（<see cref="MpMediaDownloadResult"/> 实现
/// <see cref="IDisposable"/>）。下载 URL 含 access_token Query（MUD005 同源风险，脱敏词表已覆盖）。
/// </para>
/// </remarks>
public interface IMpMediaDownloadService
{
    /// <summary>
    /// 获取临时素材（文件流 / 视频 <c>video_url</c> 双形态）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/temporary/api_getmedia.html"/>
    /// （官方接口英文名 <c>mediaGet</c>）。
    /// </summary>
    /// <param name="mediaId">媒体文件 ID（<see cref="IMpMediaService.UploadTempMediaAsync"/> 返回的 <c>media_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>下载结果（文件流形态或视频 <c>video_url</c> 形态）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，Query <c>media_id</c>（必填）+ <c>access_token</c>。
    /// </para>
    /// <para>
    /// <b>视频素材为 JSON 形态</b>（逐页核验 2026-10-07）：官方示例返回
    /// <c>{"video_url":"DOWN_URL"}</c>，<b>不是文件流也非 302 跳转</b>（方案文档 I3 的
    /// 「视频类可能重定向」假设已被核验修正）——调用方须按 <see cref="MpMediaDownloadResult.VideoUrl"/>
    /// 是否为空分流处理。图片 / 语音 / 缩略图为文件流。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40007</c>（invalid media_id，无效的媒体 ID）。
    /// </para>
    /// <para>
    /// 临时素材 3 天有效（官方「注意事项」）；本页「注意事项」原文为「本接口无特殊注意事项」。
    /// </para>
    /// </remarks>
    Task<MpMediaDownloadResult> DownloadTemporaryMediaAsync(
        string mediaId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高清语音素材（JSSDK <c>uploadVoice</c> 上传的 speex 语音）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/temporary/api_gethdvoice.html"/>
    /// （官方接口英文名 <c>getHDVoice</c>）。
    /// </summary>
    /// <param name="mediaId">媒体文件 ID（JSSDK <c>uploadVoice</c> 接口返回的 serverID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>下载结果（speex 文件流）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，Query <c>media_id</c>（必填）+ <c>access_token</c>。
    /// </para>
    /// <para>
    /// 官方原文：「本接口用于获取从 JSSDK 的 uploadVoice 接口上传的临时语音素材，格式为 speex，
    /// 16K 采样率。该音频比临时素材获取接口（格式为 amr，8K 采样率）更加清晰，适合用作语音识别等
    /// 对音质要求较高的业务」；「需使用 speex 解码库进行转码处理」（解码库见官方文档链接）。
    /// </para>
    /// <para>官方错误码：<c>40007</c>（invalid media_id）。</para>
    /// <para>
    /// <b>适用范围（本页字段表逐页核验）</b>：公众号 / 服务号 —— <b>仅认证</b>
    /// （仅允许企业主体已认证账号调用）。
    /// </para>
    /// </remarks>
    Task<MpMediaDownloadResult> DownloadJssdkVoiceAsync(
        string mediaId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取永久素材（图文 / 视频 / 文件流三形态统一信封）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_getmaterial.html"/>
    /// （官方接口英文名 <c>getMaterial</c>）。
    /// </summary>
    /// <param name="mediaId">要获取的素材的 media_id（官方 <c>media_id</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>永久素材结果（图文 news_item / 视频 down_url / 图片语音文件流三形态之一）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约（逐页核验 2026-10-07）：<b>POST</b> + 请求体 <c>{"media_id":…}</c>；
    /// <b>响应形态按素材类型分流</b>（官方原文「除图文、视频之外，其他类型的素材消息，则响应的
    /// 直接为素材的内容，开发者可以自行保存为文件」）：
    /// </para>
    /// <list type="bullet">
    /// <item><b>图文</b>：JSON <c>news_item</c> 数组（8 字段，逐页核验——本页无
    /// <c>need_open_comment</c>/<c>only_fans_can_comment</c>，旧版文档有，按新版页面建模）。</item>
    /// <item><b>视频</b>：JSON <c>title</c>/<c>description</c>/<c>down_url</c>（down_url 为下载地址，
    /// 由调用方自行下载；官方示例 JSON 末尾尾逗号/裸占位符系页面原文缺陷，照录）。</item>
    /// <item><b>图片 / 语音</b>：二进制文件流。</item>
    /// </list>
    /// <para>
    /// <b>为何本端点在下载通道而非生成管线</b>：同一端点对图片 / 语音素材返回二进制，
    /// JSON 反序列化管线无法承载三形态（I3 双通道裁决的延用；JSON 形态的解释经
    /// <c>MediaJsonContext</c> 源生成反序列化完成）。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>40007</c>（invalid media_id）。</para>
    /// <para>适用范围：小程序 ✔ / 公众号 ✔ / 服务号 ✔ / 小游戏 ✔（本页字段表）。</para>
    /// </remarks>
    Task<MpPermanentMaterialResult> GetPermanentMaterialAsync(
        string mediaId,
        CancellationToken cancellationToken = default);
}
