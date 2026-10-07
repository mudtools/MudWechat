// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「openApi 管理」域 SDK（5 端点：令牌形态 4 + 免令牌形态 1，双接口同注册组）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 → openApi 管理（apimanage/ 前缀）。深链均为逐页核验过的真实页面
/// （2026-10-07，subscription 订阅号域命中）。
/// </para>
/// <para>
/// <b>双接口分工（I4 裁决，勿合并）</b>：声明式 <c>[Token]</c> 管线为<b>接口级</b>特性——
/// <c>clear_quota/v2</c> 是官方为「access_token 耗尽无法调用重置接口」设计的<b>应急逃生端点</b>
/// （免 access_token，appid + appsecret 入请求体）。若并入带 [Token] 的接口，SDK 会在调用前
/// 强制取令牌——恰好复刻 v2 要解救的故障场景。<b>拆为双接口、同注册组</b>：
/// </para>
/// <list type="bullet">
/// <item><see cref="IMpOpenApiService"/>（本接口，4 端点）：clear_quota / quota/get / quota/clear / rid/get，
/// 消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入 <c>access_token</c>。</item>
/// <item><see cref="IMpOpenApiTokenFreeService"/>（1 端点）：clear_quota/v2，<b>无 [Token] 特性</b>
/// （不进令牌注入管线；G5 等价的 MP-TG8 Query 白名单不收录——它本就没有 Query 令牌）。</item>
/// </list>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>清零额度</b>：clear_quota 与 clear_quota/v2 <b>合计</b>每账号每月 <b>10 次</b>
/// （官方原文「清零生效一次即用掉一次机会」）；openapi/quota/clear 每账号每月 <b>50 次</b>。</item>
/// <item><b>同账号语义</b>：清空哪类账号的 quota 就需用该账号的 token；rid 查询仅支持同账号
/// （账号 A 产生的 rid 用账号 B 查询报 76003）。</item>
/// <item><b>sns 不支持</b>：官方原文「/xxx/sns/xxx 这类接口不支持该查询接口」（76022）。</item>
/// <item><b>第三方平台</b>：4 端点均支持 component_access_token 自调 / authorizer_access_token 代调用
/// （v2 不支持第三方平台调用）——按 M0-R3 裁决只在 XML 记录，不扩实现面。</item>
/// </list>
/// </remarks>
[HttpClientApi(RegistryGroupName = "OpenApi", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpOpenApiService
{
    /// <summary>
    /// 重置 API 调用次数。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clearquota.html"/>
    /// （官方接口英文名 <c>clearQuota</c>）。
    /// </summary>
    /// <param name="request">清零请求（<c>appid</c> 为要被清空的账号的 appid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
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
    Task<MpResponse> ClearQuotaAsync(
        [Body] MpClearQuotaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 API 调用额度。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_getapiquota.html"/>
    /// （官方接口英文名 <c>getApiQuota</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>cgi_path</c> 以 / 开头、不带 https://api.weixin.qq.com 前缀）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>额度详情（<c>quota</c> 当日额度 + <c>rate_limit</c> 频率限制 + <c>component_rate_limit</c> 代调用限制）。</returns>
    /// <remarks>
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
    Task<MpGetApiQuotaResponse> GetApiQuotaAsync(
        [Body] MpCgiPathRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重置指定 API 调用次数。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clearapiquota.html"/>
    /// （官方接口英文名 <c>clearApiQuota</c>）。
    /// </summary>
    /// <param name="request">清零请求（<c>cgi_path</c> 为目标 api 的请求地址）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
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
    Task<MpResponse> ClearApiQuotaAsync(
        [Body] MpCgiPathRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询 rid 信息（排障利器）。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_getridinfo.html"/>
    /// （官方接口英文名 <c>getRidInfo</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>rid</c> 为调用接口报错返回的 rid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>请求详情（<c>request</c>：invoke_time / cost_in_ms / request_url / request_body / response_body / client_ip）。</returns>
    /// <remarks>
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
    Task<MpGetRidInfoResponse> GetRidInfoAsync(
        [Body] MpGetRidInfoRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 微信公众号 / 服务号「openApi 管理」域的<b>免令牌</b>端点（使用 AppSecret 重置 API 调用次数）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clearquotabyappsecret.html"/>
/// （官方接口英文名 <c>clearQuotaByAppSecret</c>；路径 <c>/cgi-bin/clear_quota/v2</c>）。
/// </para>
/// <para>
/// <b>为何独立接口（I4 裁决）</b>：官方注意事项原文「该接口通过 appsecret 调用，解决了
/// access_token 耗尽无法调用『重置 API 调用次数』的问题」——它是令牌耗尽时的<b>应急逃生端点</b>。
/// 若声明 [Token]，SDK 会在调用前强制取令牌，恰好复刻它要解救的故障场景 ⇒ 本接口<b>不带 [Token]</b>、
/// 不进令牌注入管线；appid/appsecret 走<b>请求体</b>（官方参数表权威；官方代码示例把两参数放
/// Query String 系文档矛盾——appsecret 进 URL 会扩大泄露面，SDK 不采用该形态）。
/// </para>
/// <para>
/// <b>仅支持 POST</b>；不支持第三方平台调用；每月与 clear_quota <b>合计 10 次</b>清零机会。
/// 本端点调用结果不走令牌恢复链路（无令牌可恢复；48006 = 清零次数达到上限，为业务终态）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "OpenApi", TokenManage = nameof(IMpAppManager))]
public interface IMpOpenApiTokenFreeService
{
    /// <summary>
    /// 使用 AppSecret 重置 API 调用次数（免 access_token）。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/apimanage/api_clearquotabyappsecret.html"/>
    /// （官方接口英文名 <c>clearQuotaByAppSecret</c>）。
    /// </summary>
    /// <param name="request">清零请求（<c>appid</c> + <c>appsecret</c>，均必填；走请求体，不进 URL）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
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
    Task<MpResponse> ClearQuotaV2Async(
        [Body] MpClearQuotaV2Request request,
        CancellationToken cancellationToken = default);
}
