// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「短剧媒资 · 用量与流量包」域 SDK（3 端点：查询 CDN 用量数据 / 查询 CDN 日志下载链接列表 /
/// 查询流量包详情；官方 minidrama 分区 → usagedata）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三端点是同一份数据的三个口径，不可互相替代</b>：用量数据（<see cref="GetCdnUsageDataAsync"/>）是聚合曲线、
/// 日志（<see cref="GetCdnLogsAsync"/>）是逐条明细、流量包（<see cref="ListPackagesAsync"/>）是配额余量。
/// </para>
/// <para>
/// <b>日志流量 ≠ 计费流量</b>（官方原文，本域最重要的口径差异）：日志字节数是<b>应用层</b>数据，未计协议包头与重传开销，
/// 实际网络流量比纯应用层多 5%~15%，媒资管理服务<b>计费的加速流量约为日志计算加速流量的 110%</b> ⇒
/// 不得用日志流量做扣费对账，只能用于渠道归因与用量分析。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 153）。域构成、适用范围段异常留档与全域 20 错误码共用表的说明见
/// <see cref="IWxaVodUploadService"/>。
/// </para>
/// <para>
/// <b><c>query_type</c> 三端点各自成套</b>：用量与日志侧取 <see cref="WxaVodCdnQueryTypes"/>
/// （0 通用播放流量 + 短剧播放器定向流量 / 1 定向 / 2 通用），流量包侧取 <see cref="WxaVodPackageQueryTypes"/>
/// （0 通用流量 / 1 定向流量）⇒ 两张官方枚举表的值集不同，分立建模，守卫 DV5 锁定。
/// </para>
/// <para><b>调用限制</b>：三端点官方均未公布额度数字，但给出了逐页的时间窗硬约束（见各端点注释）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Vod", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaVodCdnService
{
    /// <summary>
    /// 查询 CDN 用量数据（<c>POST /wxa/sec/vod/getcdnusagedata</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnusagedata.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>start_time</c> / <c>end_time</c> / <c>data_interval</c> 必填，<c>query_type</c> 选填），见 <see cref="WxaVodGetCdnUsageDataRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>时间粒度 + 明细项列表（<c>item_list</c>，见 <see cref="WxaVodCdnItem"/>），见 <see cref="WxaVodGetCdnUsageDataResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>
    /// <b>时间窗三重约束（官方原文）</b>：可查询最近 <b>365 天</b>内的用量数据，单次查询跨度<b>不超过 90 天</b>，
    /// 并可指定时间粒度（<see cref="WxaVodCdnDataIntervals"/>：5 分钟 / 1 小时 / 1 天，官方默认天粒度）。
    /// </para>
    /// <para><b>流量为「查询时间粒度内的总流量」</b>（官方原文），粒度越细返回项越多 ⇒ 365 天全量务必用天粒度。</para>
    /// <para><b>流量进制是十进制</b>（官方原文）：<c>1GB = 1000MB</c>、<c>1MB = 1000KB</c> ⇒ 不得按 1024 换算，否则与官方账单口径偏差约 7%。</para>
    /// <para>
    /// <b><c>data_interval</c> 官方参数表列作 <c>string</c>、官方请求示例写作数字</b>（<c>"data_interval": 1440</c>）
    /// ⇒ 线格式按示例取 <c>long</c>（同域 <c>copyright_verification</c> 相反处置：那里无示例推翻表格）。裁决留档见 DTO 注释，守卫 DV4 锁定。
    /// </para>
    /// </remarks>
    [Post("/wxa/sec/vod/getcdnusagedata")]
    Task<WxaVodGetCdnUsageDataResponse> GetCdnUsageDataAsync(
        [Body] WxaVodGetCdnUsageDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 CDN 日志下载链接列表（<c>POST /wxa/sec/vod/getcdnlogs</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnlogs.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>start_time</c> / <c>end_time</c> 必填；<c>limit</c> / <c>offset</c> 官方已废弃；<c>query_type</c> 选填），见 <see cref="WxaVodGetCdnLogsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>链接总数 + 国内 CDN 日志列表（<c>domestic_cdn_logs</c>，见 <see cref="WxaVodCdnLogInfo"/>），见 <see cref="WxaVodGetCdnLogsResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体。<b>时间窗比用量数据紧两个量级</b>（官方原文）：只能查最近 <b>30 天</b>，
    /// 单次跨度<b>不超过 48 小时</b>，且<b>日志下载链接本身有效期 24 小时</b> ⇒ 取到链接必须当天下载。
    /// </para>
    /// <para>
    /// <b><c>limit</c> / <c>offset</c> 自 2024 年 3 月 29 日起废弃</b>（官方原文照录；官方参数表内这两个字段名前带
    /// U+200C 零宽不连字符，是文档排版痕迹而非字段名的一部分）⇒ 属性保留以兼容既有请求，新接入方不得依赖其分页语义。
    /// </para>
    /// <para>
    /// <b>一小时可能多个文件、每文件对应一个域名</b>（官方原文）：平台可能多域名分发，某小时无访问则不生成日志文件
    /// ⇒ 不得假定「30 天 × 24 = 720 条」，也不得按小时做连续性校验。
    /// </para>
    /// <para><b>日志数据打包有延迟</b>（官方原文）：正常情况下 3 小时后数据包趋于完整 ⇒ 紧邻当前时间窗的查询结果不完整属预期。</para>
    /// <para>
    /// <b>日志列序固定 15 列</b>（官方原文依次）：请求时间、客户端 IP、访问域名、文件路径、字节数、省级编码、运营商编码、
    /// HTTP 状态码、referer、Request-Time、UA、range、HTTP Method、协议标识、缓存 HIT / MISS ⇒ 解析下载的日志文件须按该列序，
    /// 「文件路径」列含 <c>us</c> 渠道标识（与 <see cref="IWxaVodMediaService.GetMediaLinkAsync"/> 联动做渠道归因）。
    /// </para>
    /// <para><b>敏感值</b>：日志含客户端 IP ⇒ 不得写进日志、遥测或异常消息（与 <c>permanent_code</c> 同级别处置）。</para>
    /// <para>官方错误码：<c>10093030</c>（资源不存在）。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getcdnlogs")]
    Task<WxaVodGetCdnLogsResponse> GetCdnLogsAsync(
        [Body] WxaVodGetCdnLogsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询流量包详情（<c>POST /wxa/sec/vod/listpackages</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_listpackages.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>status</c> / <c>offset</c> / <c>limit</c> <b>三支全部必填</b>，<c>query_type</c> 选填），见 <see cref="WxaVodListPackagesRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流量包总数 + 明细列表（<c>package_list</c>，见 <see cref="WxaVodPackageInfo"/>），见 <see cref="WxaVodListPackagesResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；本端点是本域唯一「分页参数必填」的端点（<c>status</c> / <c>offset</c> / <c>limit</c> 三支官方均标注必填）。</para>
    /// <para><b><c>status</c> 用 0 表示不过滤</b>（<see cref="WxaVodPackageStatuses"/>：1 有效 / 2 无效 / 3 过期）⇒ 「剩余可用流量」按官方口径是<b>有效（status=1）流量包的余额之和</b>。</para>
    /// <para><b>流量包单位为 MB、进制为十进制</b>（官方原文 <c>1GB = 1000MB</c>），与用量数据侧一致。</para>
    /// <para><b>有效期是硬约束</b>（官方原文「流量包是存在有效期限制的，需要注意流量包是否即将过期」）⇒ 配额告警须同时看 <c>end_time</c>，只看余额会在到期日突然失血。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/listpackages")]
    Task<WxaVodListPackagesResponse> ListPackagesAsync(
        [Body] WxaVodListPackagesRequest request,
        CancellationToken cancellationToken = default);
}
