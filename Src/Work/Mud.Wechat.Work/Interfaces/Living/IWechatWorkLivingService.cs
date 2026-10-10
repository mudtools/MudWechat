// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Living;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「直播」模块直播管理域公共 SDK（创建预约直播 + 修改预约直播 + 取消预约直播 + 删除直播回放
/// + 获取微信观看直播凭证 + 获取成员直播 ID 列表 + 获取直播详情 + 获取直播观看明细
/// + 获取跳转小程序商城的直播观众信息）。
/// <para>
/// 官方对三类应用开放完全一致的 9 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalLivingService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyLivingService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderLivingService"/>。
/// </para>
/// <para>
/// 注意与「家校沟通·上课直播域」（School 模块）的路由交叠：获取成员直播 ID 列表与删除直播回放两条路由
/// 官方在直播与家校沟通两处分文档承载同一端点，本域按直播文档页独立声明接口与 DTO。
/// 获取直播详情官方即 GET（livingid 走 Query），其余 8 个端点官方即 POST。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 企业自建应用消费应用自身 access_token；第三方 / 服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// 官方权限口径：自建应用须配置到「上课直播/直播 - 可调用接口的应用」中，第三方 / 代开发应用须具有「直播」权限；
/// 全域仅能获取 / 操作本应用创建的直播。自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口
/// （存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkLivingService
{
    /// <summary>
    /// 创建预约直播
    /// <para>创建一个预约直播，返回直播 id（可通过此 id 调用「进入直播」接口——包括小程序接口和 JS-SDK 接口——
    /// 实现主播到点后的开播操作，以及观众进入直播详情预约和观看直播）。</para>
    /// <para>官方限制：直播标题最多支持 20 个 utf8 字符；直播简介最多 100 个 utf8 字符，
    /// 仅对通用直播、小班课、大班课、企业培训生效；大班课和小班课仅 k12 学校和 IT 行业类型能够发起；
    /// 活动直播附图最多支持传 5 张，超过五张取前五张。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateLivingRequest"/>：anchor_userid / theme / living_start / living_duration / type / description / agentid / remind_time / activity_cover_mediaid / activity_share_mediaid / activity_detail）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播 id（livingid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93637"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93717"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96837"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/create")]
    Task<CreateLivingResponse> CreateLivingAsync(
        [Body] CreateLivingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改预约直播
    /// <para>修改一个预约直播的信息。</para>
    /// <para>官方限制：仅允许修改当前应用创建的直播，且仅允许修改预约状态下的直播 id；
    /// 直播标题最多支持 60 个字节，直播简介最多支持 300 个字节。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ModifyLivingRequest"/>：livingid / theme / living_start / living_duration / type / description / remind_time）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93640"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93720"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96839"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/modify")]
    Task<WechatWorkResponse> ModifyLivingAsync(
        [Body] ModifyLivingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消预约直播
    /// <para>取消一个指定的预约直播。</para>
    /// <para>官方限制：仅允许取消当前应用创建的直播，且仅允许取消预约状态下的直播 id。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CancelLivingRequest"/>：livingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93638"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93718"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96838"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/cancel")]
    Task<WechatWorkResponse> CancelLivingAsync(
        [Body] CancelLivingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除直播回放
    /// <para>删除指定直播的回放数据。</para>
    /// <para>官方限制：仅允许删除当前应用自己创建的直播。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeleteLivingReplayDataRequest"/>：livingid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93874"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93719"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96841"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/delete_replay_data")]
    Task<WechatWorkResponse> DeleteReplayDataAsync(
        [Body] DeleteLivingReplayDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取微信观看直播凭证（在微信中观看直播或直播回放）
    /// <para>获取微信观看直播凭证 living_code；获取后可在微信 H5（开放标签 username 固定为 <c>gh_25e071b83ee0</c>）
    /// 或小程序（appId 固定为 <c>wx7424030d69bde86e</c>，路径 <c>pages/watch/index?living_code=LIVING_CODE</c>，
    /// 回放页追加 <c>&amp;replay=1</c>）中唤起直播小程序观看直播或直播回放。</para>
    /// <para>官方限制：仅允许获取当前应用创建的微信观看直播凭证；living_code 5 分钟内可以重复使用，且仅能在微信上使用。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetLivingCodeRequest"/>：livingid / openid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>微信观看直播凭证（living_code）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93641"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93721"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96840"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/get_living_code")]
    Task<GetLivingCodeResponse> GetLivingCodeAsync(
        [Body] GetLivingCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取成员直播 ID 列表
    /// <para>获取指定成员的所有直播 ID。</para>
    /// <para>官方限制：只能获取本应用创建的直播；limit 建议填 20，默认值和最大值都为 100；
    /// 以 next_cursor 分页，返回空字符串代表已经是最后一页。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetUserAllLivingIdRequest"/>：userid / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分页游标（next_cursor，返回空字符串代表已经是最后一页）与直播 ID 列表（livingid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93634"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93714"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96834"/></para>
    /// <para>本路由与「家校沟通·上课直播域」的获取老师直播 ID 列表为官方同一端点（分文档承载）。</para>
    /// </remarks>
    [Post("/cgi-bin/living/get_user_all_livingid")]
    Task<GetUserAllLivingIdResponse> GetUserAllLivingIdAsync(
        [Body] GetUserAllLivingIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播详情（官方即 GET，livingid 走 Query）
    /// <para>获取直播详情（主题、开播时间、时长、状态、预约信息、描述、主播、观看/评论/连麦人数、回放开关与状态、
    /// 直播类型、推流地址、在线人数与预约人数）。</para>
    /// <para>官方限制：只能获取本应用创建的直播；push_stream_url 仅活动直播且状态待开播时返回；
    /// replay_status 仅 open_replay 为 1 时返回。</para>
    /// </summary>
    /// <param name="livingid">直播 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播详情信息（<see cref="LivingDetail"/>：theme / living_start / living_duration / status / reserve_start / reserve_living_duration / description / anchor_userid / main_department / viewer_num / comment_num / mic_num / open_replay / replay_status / type / push_stream_url / online_count / subscribe_count）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93635"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93715"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96835"/></para>
    /// </remarks>
    [Get("/cgi-bin/living/get_living_info")]
    Task<GetLivingInfoResponse> GetLivingInfoAsync(
        [Query("livingid")] string livingid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播观看明细
    /// <para>获取所有观看直播的人员统计（企业成员与外部成员）。</para>
    /// <para>官方限制：只能获取本应用创建的直播；以响应 ending 字段判断是否拉完
    /// （0 表示还有更多数据需继续拉取，1 表示已拉完），next_key 分页，初次调用可填 "0"；
    /// 邀请人字段（invitor_userid / invitor_external_userid）仅「推广产品」直播支持。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetLivingWatchStatRequest"/>：livingid / next_key）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否结束（ending）、分页 key（next_key）与统计信息列表（stat_info.users / stat_info.external_users）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93636"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93716"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96836"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/get_watch_stat")]
    Task<GetLivingWatchStatResponse> GetWatchStatAsync(
        [Body] GetLivingWatchStatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取跳转小程序商城的直播观众信息
    /// <para>「推广产品」直播观众跳转小程序商城时会在小程序 path 中带上 ww_share_code 参数，
    /// 通过该参数换取观众与邀请人身份信息。</para>
    /// <para>官方限制：ww_share_code 五分钟内有效；跳转的小程序需要与企业有绑定关系。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetLivingShareInfoRequest"/>：ww_share_code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播 id（livingid）、观众 userid / external_userid（viewer_userid / viewer_external_userid）与邀请人 userid / external_userid（invitor_userid / invitor_external_userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94442"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94578"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96843"/></para>
    /// </remarks>
    [Post("/cgi-bin/living/get_living_share_info")]
    Task<GetLivingShareInfoResponse> GetLivingShareInfoAsync(
        [Body] GetLivingShareInfoRequest request,
        CancellationToken cancellationToken = default);
}
