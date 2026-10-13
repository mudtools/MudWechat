// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「短剧媒资 · 媒资管理」域 SDK（4 端点：获取媒资列表 / 获取媒资详细信息 / 获取媒资播放链接 /
/// 删除媒资；官方 minidrama 分区 → vod_media）。
/// </summary>
/// <remarks>
/// <para>
/// <b>媒资（media）与剧目（drama）是两层对象</b>：媒资是「一集」的视频文件（<c>media_id</c>），
/// 剧目是「一部剧」的审核主体（<c>drama_id</c>，见 <see cref="IWxaVodDramaService"/>）。
/// 媒资列表里的 <c>drama_id</c> 为 0 表示该剧集尚未归属任何剧目。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 153）。域构成、适用范围段异常留档与全域 20 错误码共用表的说明见
/// <see cref="IWxaVodUploadService"/>。
/// </para>
/// <para>
/// <b>临时链接不得落库</b>（官方逐页原文「本接口返回的视频或图片链接均为临时链接，不应将其保存下来」）：
/// 四端点里 <c>cover_url</c> / <c>original_url</c> / <c>mp4_url</c> / <c>hls_url</c> 全部是临时形态，
/// 需要可重放定位时只存 <c>media_id</c>，播放链接每次现取（<see cref="GetMediaLinkAsync"/>）。
/// </para>
/// <para><b>调用限制</b>：四端点官方均未公布额度数字。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Vod", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaVodMediaService
{
    /// <summary>
    /// 获取媒资列表（<c>POST /wxa/sec/vod/listmedia</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_listmedia.html"/>。
    /// </summary>
    /// <param name="request">请求体（七个过滤参数<b>全选填</b>），见 <see cref="WxaVodListMediaRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒资信息列表（<c>media_info_list</c>，元素与详细信息端点共用 <see cref="WxaVodMediaInfo"/>），见 <see cref="WxaVodListMediaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；<c>limit</c> 官方上限 <b>100</b>。</para>
    /// <para>
    /// <b>两种模糊匹配，官方推荐后者</b>：<c>media_name</c> 支持精确/模糊（模糊须在前后加 <c>%</c>，
    /// 如 <c>"test%"</c>），官方原文警告「文件太多时使用该参数进行模糊匹配可能无法得到结果」；
    /// <c>media_name_fuzzy</c> 仅支持模糊且<b>不带</b> <c>%</c> ⇒ 二者语义相反（一个默认精确、一个默认模糊），
    /// <b>不得</b>合并成一个属性，守卫 DV4 锁定该字段面对。
    /// </para>
    /// <para>
    /// <b>逐集驳回理由与证据截图在本域取</b>（官方原文）：剧目审核失败/驳回时，每一集的具体理由及截图
    /// 只能通过本接口或 <see cref="GetMediaAsync"/> 的 <c>audit_detail</c> 获取，剧目侧接口只给整剧结论。
    /// </para>
    /// </remarks>
    [Post("/wxa/sec/vod/listmedia")]
    Task<WxaVodListMediaResponse> ListMediaAsync(
        [Body] WxaVodListMediaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取媒资详细信息（<c>POST /wxa/sec/vod/getmedia</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedia.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>media_id</c> 一字段，必填），见 <see cref="WxaVodMediaIdRequest"/>（与删除端点共用）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>媒资详细信息（<c>media_info</c>），见 <see cref="WxaVodGetMediaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>
    /// <b>审核状态的两层口径</b>（官方原文）：<c>media_info.audit_detail.status</c> 是<b>本集</b>的审核状态
    /// （取值见 <see cref="WxaVodMediaAuditStatuses"/>），可能与剧目整体结论不一致 ⇒ 「本集审核通过」
    /// 不等于「可以播放」，播放可用性判据在 <see cref="IWxaVodDramaService"/> 侧。
    /// </para>
    /// <para><b><c>expire_time</c> 是媒资本身的过期时间</b>（官方类型 <c>number</c>，时间戳）：过期后播放链接不可用，须重新上传。</para>
    /// <para><b>审核证据截图 <c>evidence_material_id_list</c> 可回读</b>（官方原文）：截图 id 可用作「获取永久素材」接口的参数来获得截图内容。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getmedia")]
    Task<WxaVodGetMediaResponse> GetMediaAsync(
        [Body] WxaVodMediaIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取媒资播放链接（<c>POST /wxa/sec/vod/getmedialink</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedialink.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>media_id</c> + <c>t</c> 必填，<c>us</c> / <c>exper</c> / <c>rlimit</c> / <c>whref</c> / <c>bkref</c> 选填），见 <see cref="WxaVodGetMediaLinkRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>播放信息（<c>media_info.mp4_url</c> / <c>hls_url</c>），见 <see cref="WxaVodGetMediaLinkResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；六个参数官方全部写在请求体表内（本端点为 POST，按 <c>[Body]</c> 落地，
    /// 与直播域「GET 端点带请求体表」的 <c>[Query]</c> 裁决无关）。
    /// </para>
    /// <para>
    /// <b><c>t</c> 上限 2 小时</b>（官方原文「有效的时间最长不能超过 2 小时后的」）：入参是播放地址的过期时间戳，
    /// 传超上限值不会得到更长的有效期。
    /// </para>
    /// <para>
    /// <b>能否取到链接取决于剧目审核状态</b>（官方原文）：可能出现「单集审核通过但剧目整体未通过」，
    /// 这种情况<b>同样拿不到播放链接</b> ⇒ 宿主不要把「本接口失败」直接归因为单集问题。
    /// </para>
    /// <para>
    /// <b><c>us</c> 是渠道统计的唯一抓手</b>（官方原文）：最终值为「开发者标识-平台标识」，
    /// 该标识同时出现在 CDN 日志（<see cref="IWxaVodCdnService.GetCdnLogsAsync"/>）的「文件路径」列 ⇒
    /// 结合「字节数」列可估算各渠道流量；另需注意日志流量与计费流量的差异（官方口径约 110%）。
    /// </para>
    /// <para>
    /// <b>限制项互斥风险</b>：<c>rlimit</c>（最多允许多少个不同 IP 终端播放，官方上限 9）官方建议「限制单人播放时不要严格限制成 1，
    /// 可设置为 3」，因为移动端断网重连后 IP 可能改变；<c>whref</c> / <c>bkref</c> 各支持 1~10 条域名、
    /// 域名前<b>不带</b>协议名且为前缀匹配（支持通配符）。
    /// </para>
    /// <para><b>不得假定播放链接域名固定</b>（官方原文「平台可能使用多个域名分发」）⇒ 白名单校验须按域名集合而非单域名硬编码。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getmedialink")]
    Task<WxaVodGetMediaLinkResponse> GetMediaLinkAsync(
        [Body] WxaVodGetMediaLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除媒资（<c>POST /wxa/sec/vod/deletemedia</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_deletemedia.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>media_id</c> 一字段，必填），见 <see cref="WxaVodMediaIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方返回体表除通用两字段外无业务字段）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>
    /// <b>破坏性操作</b>：删除后媒资不可恢复，且已提审剧目若引用该 <c>media_id</c> 会成为悬空引用 ⇒
    /// 宿主侧须自行做二次确认与剧集替换编排（替换见 <see cref="IWxaVodDramaService.ReplaceDramaMediaAsync"/>）。
    /// </para>
    /// <para><b>与 <see cref="GetMediaAsync"/> 共用请求体</b>：官方两页参数表逐字段相同（<c>media_id</c>，均必填）⇒ 守卫 DV4 锁定该共用关系。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/deletemedia")]
    Task<WxaResponse> DeleteMediaAsync(
        [Body] WxaVodMediaIdRequest request,
        CancellationToken cancellationToken = default);
}
