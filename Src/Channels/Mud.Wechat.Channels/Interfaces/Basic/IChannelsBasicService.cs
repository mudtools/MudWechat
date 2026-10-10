// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「基础接口」域 SDK（8 端点：获取微信 API 服务器 IP +
/// 获取微信推送服务器 IP + 网络通信检测 + openApi 管理 4 端点 + 免令牌清额 1 端点；
/// 令牌签发 2 端点由 Abstractions 的 Authentication 注册组承载）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域范围边界（设计方案 v1 §4.5）</b>：本域 8 个端点与公众号线<b>云端同路由</b>，但令牌凭据
/// 体系不同（小店 AppID vs 公众号 AppID）→ 本线自建 <c>AddBasicApi</c>，不抽共享、不并入公众号线
/// （对齐小程序线「同一 /cgi-bin 端点不新增令牌类型」与「跨线路由重复零回潮」纪律——本线是
/// <b>新令牌类型 + 同路由</b>，两线各自声明各自消费）。守卫 <c>ChannelsRouteContractGuards</c>
/// （CH-R1）白名单锁定该交叠。
/// </para>
/// <para>
/// <b>双接口分工（对齐公众号 OpenApi 域先例）</b>：<c>clear_quota/v2</c> 是官方为「access_token 耗尽
/// 无法调用重置接口」设计的<b>应急逃生端点</b>（免 access_token，appid + appsecret 入请求体）。
/// 若并入带 [Token] 的接口，SDK 会在调用前强制取令牌——恰好复刻 v2 要解救的故障场景。
/// <b>拆为双接口、同注册组</b>：本接口（7 端点带令牌）消费 <see cref="ChannelsTokenTypes.AccessToken"/>
/// 并以 Query 注入 <c>access_token</c>；<see cref="IChannelsBasicTokenFreeService"/>（1 端点）
/// <b>无 [Token] 特性</b>（不进令牌注入管线）。
/// </para>
/// <para>
/// <b>令牌路由键</b> <see cref="ChannelsTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多小店基座按当前应用上下文（AppKey）路由；走「普通通道」还是「稳定版通道」由
/// <c>ChannelsAppConfig.UseStableToken</c> 在注册期决定，<b>不体现在本接口声明上</b>。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Basic", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsBasicService
{
    /// <summary>
    /// 获取微信 API 服务器 IP。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>微信 API 服务器 IP 列表（失败时官方返回 <c>errcode</c>/<c>errmsg</c> 且 <c>ip_list</c> 缺省）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_getapidomainip"/></para>
    /// <para>
    /// 官方业务约束：出口 IP 及入口 IP 可能变动，官方<b>建议每天请求 1 次</b>以更新 IP 列表；
    /// 不建议长期使用旧 IP 列表作为 <c>api.weixin.qq.com</c> 的访问入口（单点故障）。
    /// </para>
    /// <para>官方错误码：<c>40013</c>（invalid appid）+ 通用错误码。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/get_api_domain_ip")]
    Task<ChannelsGetApiDomainIpResponse> GetApiDomainIpAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取微信推送服务器 IP。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>微信推送服务器 IP 列表（失败时官方返回 <c>errcode</c>/<c>errmsg</c> 且 <c>ip_list</c> 缺省）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_getcallbackip"/></para>
    /// <para>响应形态与约束同 <see cref="GetApiDomainIpAsync"/>（官方两页正文逐项一致）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/cgi-bin/getcallbackip")]
    Task<ChannelsGetCallbackIpResponse> GetCallbackIpAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 网络通信检测（排查回调连接失败——对开发者 URL 做域名解析，然后对所有 IP 各 ping 一次）。
    /// </summary>
    /// <param name="request">检测请求（<c>action</c> 与 <c>check_operator</c> 均为官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>DNS 解析与 PING 检测结果。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_callbackcheck"/></para>
    /// <para>
    /// 功能定位：排查<b>回调连接失败</b>——对开发者 URL 做域名解析，然后对所有 IP 各 ping 一次，
    /// 得到丢包率与耗时。取值常量见 <c>ChannelsCallbackCheckActions</c> / <c>ChannelsCallbackCheckOperators</c>。
    /// </para>
    /// <para>
    /// 官方说明：<c>package_loss</c> 因仅发送一个 ping 包，取值只有 <c>0%</c> / <c>100%</c> 两种，
    /// 不宜作为精细化网络质量指标；官方「注意事项」章节原文为「本接口无特殊注意事项」。
    /// </para>
    /// <para>官方错误码：<c>40201</c>（未设置回调 URL）/ <c>40202</c>（非法 action）/ <c>40203</c>（非法运营商参数）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/callback/check")]
    Task<ChannelsCallbackCheckResponse> CheckCallbackAsync(
        [Body] ChannelsCallbackCheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重置 API 调用次数（清空账号配额，可用于配额耗尽后的恢复）。
    /// </summary>
    /// <param name="request">清零请求（<c>appid</c> 为要被清空的账号的 appid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_clearquota"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"appid":…}</c>（access_token 在 Query——官方明确「不在 body 中」）。
    /// </para>
    /// <para>
    /// <b>每月 10 次</b>（与 clear_quota/v2 合计；官方原文「每个账号每月共 10 次清零操作机会，
    /// 清零生效一次即用掉一次机会」）。清空哪类账号的 quota 就需用该账号对应的 token。
    /// </para>
    /// <para>
    /// 官方错误码：<c>0</c> / <c>40013</c>（invalid appid）/ <c>48006</c>
    /// （forbid to clear quota because of reaching the limit，清零次数达到上限）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/clear_quota")]
    Task<ChannelsResponse> ClearQuotaAsync(
        [Body] ChannelsClearQuotaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 API 调用额度（<c>quota/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>cgi_path</c> 以 / 开头、不带 https://api.weixin.qq.com 前缀）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>额度详情（<c>quota</c> 当日额度 + <c>rate_limit</c> 频率限制 + <c>component_rate_limit</c> 代调用限制）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_getapiquota"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"cgi_path":…}</c>；官方原文「不要前缀
    /// https://api.weixin.qq.com，也不要漏了 /，否则都会 76003 的报错」。
    /// </para>
    /// <para>
    /// 官方错误码：<c>0</c> / <c>76021</c>（cgi_path not found，cgi_path 填错了）/
    /// <c>76022</c>（could not use this cgi_path, no permission——token 与 api 所属账号不符；
    /// 「/xxx/sns/xxx」类接口亦报此码）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/openapi/quota/get")]
    Task<ChannelsGetApiQuotaResponse> GetApiQuotaAsync(
        [Body] ChannelsCgiPathRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重置指定 API 调用次数（<c>quota/clear</c>）。
    /// </summary>
    /// <param name="request">清零请求（<c>cgi_path</c> 为目标 api 的请求地址）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_clearapiquota"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"cgi_path":…}</c>（官方示例以 /channels/ec/ 开头）。
    /// </para>
    /// <para><b>每月 50 次</b>（官方原文「每个账号每月共 50 次清零操作机会」——与 clear_quota 的 10 次独立）。</para>
    /// <para>
    /// 官方错误码：<c>40001</c> / <c>41001</c> / <c>42001</c> / <c>44002</c>（POST 数据包为空）/
    /// <c>45009</c> / <c>50002</c> / <c>76021</c> / <c>76022</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/openapi/quota/clear")]
    Task<ChannelsResponse> ClearApiQuotaAsync(
        [Body] ChannelsCgiPathRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 rid 信息（排障利器，<c>rid/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>rid</c> 为调用接口报错返回的 rid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>请求详情（<c>request</c>：invoke_time / cost_in_ms / request_url / request_body / response_body / client_ip）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_getridinfo"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"rid":…}</c>。
    /// </para>
    /// <para>
    /// <b>有效期与同账号语义（官方「注意事项」原文）</b>：①「rid 有效期仅 7 天，超过 7 天的 rid
    /// 查询会报错（76001）」；②「查询 rid 属于开发者私密行为，仅支持同账号查询——例如 rid 由账号 A
    /// 调用产生，需用账号 A 的 access_token 查询，用账号 B 查询会报错（76003）」；
    /// ③「/xxx/sns/xxx 这类接口不支持本接口查询（76022）」。
    /// </para>
    /// <para>官方错误码：<c>0</c> / <c>76001</c>（rid 不存在）/ <c>76002</c>（rid 为空或格式错误）/
    /// <c>76003</c>（无权查询，rid 属其他账号）/ <c>76004</c>（rid 过期，仅支持 7 天内）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/openapi/rid/get")]
    Task<ChannelsGetRidInfoResponse> GetRidInfoAsync(
        [Body] ChannelsGetRidInfoRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 微信小店 / 视频号「基础接口」域的<b>免令牌</b>端点（使用 AppSecret 重置 API 调用次数）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_clearquotabyappsecret"/>
/// （官方接口英文名 <c>clearQuotaByAppSecret</c>；路径 <c>/cgi-bin/clear_quota/v2</c>）。
/// </para>
/// <para>
/// <b>为何独立接口（I4 裁决，对齐公众号 OpenApi 域先例）</b>：官方注意事项原文「该接口通过 appsecret
/// 调用，解决了 access_token 耗尽无法调用『重置 API 调用次数』的问题」——它是令牌耗尽时的
/// <b>应急逃生端点</b>。若声明 [Token]，SDK 会在调用前强制取令牌，恰好复刻它要解救的故障场景 ⇒
/// 本接口<b>不带 [Token]</b>、不进令牌注入管线；appid/appsecret 走<b>请求体</b>（官方参数表权威；
/// 官方代码示例把两参数放 Query String 系文档矛盾——appsecret 进 URL 会扩大泄露面，SDK 不采用该形态）。
/// </para>
/// <para>
/// <b>仅支持 POST</b>；不支持第三方平台调用；每月与 clear_quota <b>合计 10 次</b>清零机会。
/// 本端点调用结果不走令牌恢复链路（无令牌可恢复；48006 = 清零次数达到上限，为业务终态）。
/// </para>
/// </remarks>
// HTTPCLIENT018：生成器要求显式声明 TokenManagerKey/TokenType。本接口是 I4 裁决的**免令牌端点**
// ——clear_quota/v2 官方设计为「access_token 耗尽无法调用 clear_quota」的应急逃生通道
//（appid + appsecret 入请求体），刻意不声明 [Token]；声明之恰好复刻它要解救的故障场景。
// 守卫锁定该裁决（带令牌的 IChannelsBasicService 7 端点仍显式声明 [Token]）。
#pragma warning disable HTTPCLIENT018
[HttpClientApi(RegistryGroupName = "Basic", TokenManage = nameof(IChannelsAppManager))]
public interface IChannelsBasicTokenFreeService
{
    /// <summary>
    /// 使用 AppSecret 重置 API 调用次数（免 access_token）。
    /// </summary>
    /// <param name="request">清零请求（<c>appid</c> + <c>appsecret</c>，均必填；走请求体，不进 URL）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_clearquotabyappsecret"/></para>
    /// <para>
    /// 官方契约：<b>POST</b>；请求体 <c>{"appid":…,"appsecret":…}</c>
    /// （参数位置裁决见类型 remarks——官方文档矛盾照录）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40013</c> / <c>41002</c>（appid missing）/
    /// <c>41004</c>（appsecret missing）/ <c>48006</c>（清零次数达到上限）。
    /// </para>
    /// <para>
    /// <b>安全边界</b>：<c>appsecret</c> 经请求体提交、绝不进 URL/日志（AGENTS §8；
    /// 宿主侧亦不得将其写入自己的日志/遥测）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/clear_quota/v2")]
    Task<ChannelsResponse> ClearQuotaV2Async(
        [Body] ChannelsClearQuotaV2Request request,
        CancellationToken cancellationToken = default);
}
#pragma warning restore HTTPCLIENT018
