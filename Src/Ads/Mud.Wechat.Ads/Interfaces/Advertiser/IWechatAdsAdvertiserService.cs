// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Advertiser;

namespace Mud.Wechat.Ads;

/// <summary>
/// 腾讯广告（Marketing API v3.0）「客户账号」域 SDK（<c>advertiser</c>，3 端点）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（逐页核验 2026-10-10）：
/// 查询客户信息 <see href="https://developers.e.qq.com/v3.0/docs/api/advertiser/get"/>、
/// 更新客户信息 <see href="https://developers.e.qq.com/v3.0/docs/api/advertiser/update"/>、
/// 批量修改账户日预算 <see href="https://developers.e.qq.com/v3.0/docs/api/advertiser/update_daily_budget"/>。
/// 官方请求域为 <c>https://api.e.qq.com/v3.0/</c>，本线各路由按官方「请求地址」原文写为
/// <c>/v3.0/{resource}/{action}</c>（版本号进路由，基址只到主机名，见 <c>AdsHttpClientNames</c>）。</para>
/// <para><b>权限（官方「所属权限」原文，决定令牌是否具备调用资格）</b>：
/// <c>get</c> 为 <c>account_management,ads_management,ads_insights,audience_management,user_actions</c>（五者任一），
/// <c>update</c> 为 <c>account_management</c>，<c>update_daily_budget</c> 为 <c>ads_management</c>。
/// 权限不足时官方以应答 <c>code</c> 表达，SDK 不做本地权限预判。</para>
/// <para><b>无 <c>[Token]</c>、凭据在传输层成组注入</b>（守卫 ADS-B1）：v3.0「全局参数」表要求每个业务请求
/// 同时携带 <c>access_token</c> + <c>timestamp</c>（秒级，最大误差 300 秒，时区 GMT+8）+ <c>nonce</c>
/// （≤32 字符、全局唯一），三者必须同源同次现取 ⇒ 声明式 <c>[Token]</c> 只能注入其一、组不出合法请求。
/// 组装由 <c>AdsAuthorizationHandler</c> 完成，本接口的方法签名<b>不出现</b>这三个参数。</para>
/// <para><b>复合 Query 参数只能以 JSON 字符串承载</b>：官方 curl 把 <c>filtering</c> / <c>fields</c>
/// 写成单个键值对、值为 JSON 字面量（<c>-d 'filtering=[{"field":…}]'</c>、<c>-d 'fields=[]'</c>），
/// 而声明式客户端对数组参数走「重复同名参数」、对复杂类型走「逐属性展平」，两种都不是官方线格式 ⇒
/// 本域这两个参数类型为 <see cref="string"/>，构造入口只有 <see cref="AdsQueryJson"/>
/// （形态差异的完整推导见该类型 remarks）。</para>
/// <para><b>受限接口的 <c>user_token</c></b>：<c>update</c> 与 <c>update_daily_budget</c> 两页在「全局参数」
/// 之外另列 <c>user_token</c>（实名认证完成获取的令牌，官方指向「API 身份验证升级公告」对接文档），
/// 它是<b>逐请求</b>传入的操作者令牌、与账号级 <c>access_token</c> 不是一个东西 ⇒
/// 做成显式可选 Query 参数而非传输层注入。该参数名不在组件脱敏词表内，
/// 由模块注册期登记为强制掩码键（守卫 ADS-B5）。</para>
/// <para><b>应答信封</b>：<c>{code, message, message_cn, data}</c>，<c>code == 0</c> 为成功；
/// 全部端点带 <c>[AllowAnyStatusCode]</c>，业务失败一律落到信封后由 <c>WechatAdsException.ThrowIfFailed</c> 判定。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Advertiser", HttpClient = AdsHttpClientNames.TypeName)]
public interface IWechatAdsAdvertiserService
{
    /// <summary>
    /// 查询客户信息。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/advertiser/get"/>（页面更新时间见官方页面）。
    /// </summary>
    /// <param name="accountId">账户 id（Query <c>account_id</c>，官方标 <c>integer</c>）。官方原文：
    /// 「直客客户必须填写，代理商可不填写；如代理商不填写，则获取代理商下全部子客户的信息」⇒ 本参数可空。</param>
    /// <param name="fields">指定返回的字段列表（Query <c>fields</c>，官方 <c>string[]</c>、<b>必填</b>）。
    /// 值为 <b>JSON 数组字符串</b>，用 <see cref="AdsQueryJson.Fields"/> 构造。
    /// 官方限制：数组最小长度 1、最大长度 256，每项 1–64 字节；SDK 不做本地长度拦截
    /// （官方示例恰以 <c>fields=[]</c> 传空，拦截会挡掉合法请求）。</param>
    /// <param name="paginationMode">分页方式（Query <c>pagination_mode</c>，官方 <c>enum</c>、<b>必填</b>）：
    /// 可选值 <c>{ PAGINATION_MODE_NORMAL, PAGINATION_MODE_CURSOR }</c>。</param>
    /// <param name="pageSize">一页条数（Query <c>page_size</c>，官方 <c>integer</c>、<b>必填</b>，最小 1、最大 100）。</param>
    /// <param name="agencyId">服务商账号 id（Query <c>agency_id</c>，官方标 <c>integer</c>，最小值 1）。
    /// 官方原文：「服务商主体请求时必填、其他 token 忽略此参数」⇒ 条件必填，SDK 不预判。</param>
    /// <param name="filtering">过滤条件（Query <c>filtering</c>，官方 <c>struct[]</c>）。值为 <b>JSON 数组字符串</b>，
    /// 用 <see cref="AdsQueryJson.Filtering"/> 构造；官方原文「若此字段不传，或传空则视为无限制条件」。
    /// <b>本页官方限制</b>：数组长度为 1；<c>field</c> 可选值 <c>{ corporation_name }</c>；
    /// <c>operator</c> 当 <c>field</c> 取 <c>corporation_name</c> 时可选值 <c>{ EQUALS, CONTAINS }</c>；
    /// <c>values</c> 数组长度为 1，字段 1–120 字节。</param>
    /// <param name="page">搜索页码（Query <c>page</c>，官方 <c>integer</c>，最小 1、最大 1000）。
    /// 官方原文「<b>普通翻页模式必填</b>」⇒ 条件必填，SDK 不预判。</param>
    /// <param name="cursor">游标值（Query <c>cursor</c>，官方 <c>integer</c>，最小值 1）。官方原文：
    /// 「第一次拉取无需填写、后续拉取传递上一次返回的 cursor 数值」；游标有效期 24 小时，
    /// 且游标模式下除分页参数外的请求参数须与上一次调用<b>完全一致</b>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>客户信息列表（<c>data.list[]</c> + 两种分页元信息），见 <see cref="AdsAdvertiserGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3.0/advertiser/get</c>（官方 curl 用 <c>curl -G -d</c> ⇒ 参数进 Query）。</para>
    /// <para><b>翻页模式与参数配套</b>：<c>PAGINATION_MODE_NORMAL</c> 用 <paramref name="page"/> +
    /// <paramref name="pageSize"/> 并返回 <c>page_info</c>；<c>PAGINATION_MODE_CURSOR</c> 用
    /// <paramref name="cursor"/> + <paramref name="pageSize"/> 并返回 <c>cursor_page_info</c>，
    /// 其 <c>has_more</c> 官方原文「返回 false 表示已无下一页，此时务必停止拉取」。</para>
    /// <para><b>本域端点计数</b>：官方清单 <c>advertiser/*</c> 恰有
    /// <c>get</c> / <c>update</c> / <c>update_daily_budget</c> 三个端点，本接口三端点全覆盖（守卫 ADS-B2）。</para>
    /// </remarks>
    [Get("/v3.0/advertiser/get")]
    Task<AdsAdvertiserGetResponse> GetAsync(
        [Query("account_id")] long? accountId,
        [Query("fields")] string fields,
        [Query("pagination_mode")] string paginationMode,
        [Query("page_size")] long pageSize,
        [Query("agency_id")] long? agencyId = null,
        [Query("filtering")] string? filtering = null,
        [Query("page")] long? page = null,
        [Query("cursor")] long? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新客户信息。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/advertiser/update"/>。
    /// </summary>
    /// <param name="request">请求体（官方请求根字段表，仅 <c>account_id</c> 标必填，余者「不传即不改」）。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填）。本页「全局参数」之外另列该参数，
    /// 获取方式见官方「API 身份验证升级公告」对接文档；<b>属凭据</b> ⇒ 不得写进日志 / 遥测 / 异常消息。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>更新结果（官方 <c>data</c> 只回 <c>account_id</c>），见 <see cref="AdsAdvertiserUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/advertiser/update</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>日预算四条官方硬约束（<c>daily_budget</c>，原文要点，勿弱化）</b>：
    /// ① 单位为分，取值须介于 <b>5,000 分 – 4,000,000,000 分</b>（50 元 – 40,000,000 元），设为 <c>0</c> 表示不设预算；
    /// ② <b>每次修改幅度不能低于 5,000 分</b>；微信公众号平台小程序账户每次提高幅度不能低于 <b>50,000 分</b>；
    /// ③ 修改后不得<b>低于今日已消耗金额的 1.2 倍加上冻结金</b>；
    /// ④ 且不得<b>低于今日已消耗金额加上 5,000 分</b>。另：仅对竞价投放生效，合约投放不受影响。
    /// 数值判定依赖当日消耗与冻结金（SDK 不可见）⇒ <b>不做本地拦截</b>，越界由应答 <c>code</c> 表达。</para>
    /// <para><b><c>websites</c> 为整体提交</b>（数组 0–255，元素内 <c>website_domain</c> / <c>icp_image_id</c> 必填）：
    /// 传空数组与不传该字段在官方语义上不同（清空 / 不改），SDK 不做「空即忽略」的本地推断。</para>
    /// </remarks>
    [Post("/v3.0/advertiser/update")]
    Task<AdsAdvertiserUpdateResponse> UpdateAsync(
        AdsAdvertiserUpdateRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量修改账户日预算。
    /// 官方文档：<see href="https://developers.e.qq.com/v3.0/docs/api/advertiser/update_daily_budget"/>。
    /// </summary>
    /// <param name="request">请求体（官方根字段只有 <c>update_daily_budget_spec</c>，<b>必填</b>，数组最大长度 100）。</param>
    /// <param name="userToken">实名认证令牌（Query <c>user_token</c>，选填；语义同
    /// <see cref="UpdateAsync"/>，本页同样列出该参数）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>逐条结果（<c>data.list[]</c> 每条自带 <c>code</c>）+ 失败 id 集合（<c>data.fail_id_list</c>）。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3.0/advertiser/update_daily_budget</c>，<c>Content-Type: application/json</c>。</para>
    /// <para><b>双层失败语义（本端点最易误判之处）</b>：外层信封 <c>code == 0</c> 只表示请求被受理，
    /// <b>每一条</b>的成败看 <c>data.list[i].code</c>，部分失败时外层仍为 <c>0</c> 且另有 <c>fail_id_list</c> ⇒
    /// 判错必须两层都做，只判外层会把「半数失败」当成全成功。故本端点<b>不</b>走
    /// <c>WechatAdsException.ThrowIfFailed</c> 的单层判定即可返回，调用方须自行逐条判定。</para>
    /// <para><b><c>use_min_daily_budget</c> 可能出现「期望下调、实际上调」</b>（官方原文），
    /// 实际生效值以应答元素的 <c>daily_budget</c> 为准。预算区间与幅度约束同 <see cref="UpdateAsync"/>
    /// （本页原文另明确「允许从不限预算（0）修改为指定预算，以及从指定日预算修改为不限预算（0）」）。</para>
    /// </remarks>
    [Post("/v3.0/advertiser/update_daily_budget")]
    Task<AdsAdvertiserUpdateDailyBudgetResponse> UpdateDailyBudgetAsync(
        AdsAdvertiserUpdateDailyBudgetRequest request,
        [Query("user_token")] string? userToken = null,
        CancellationToken cancellationToken = default);
}
