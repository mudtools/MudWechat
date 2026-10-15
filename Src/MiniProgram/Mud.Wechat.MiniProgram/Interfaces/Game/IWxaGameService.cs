// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.Game;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「小游戏」域 SDK（<b>20 端点</b>：帧同步房间 4 + 用户数据存储 3 + 对局匹配 5 + 米大师支付 1 + 内容安全 2 + 米大师 V2 支付 5）。
/// </summary>
/// <remarks>
/// <para><b>官方文档族</b>（minigame 树，2026-10-14 逐页核验）：帧同步房间
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.createGameRoom.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameFrame.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameIdentityInfo.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameRoomInfo.html"/>、
/// 用户数据存储
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.removeUserStorage.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserInteractiveData.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserStorage.html"/>、
/// 对局匹配
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.createMatchRule.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.deleteMatchRule.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.getAllMatchRule.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.setMatchIdOpenState.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.updateMatchRule.html"/>、
/// 米大师支付
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/business.getPayForOrder.html"/>、
/// 内容安全
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.msgSecCheck.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.mediaCheck.html"/>、
/// 米大师 V2 支付
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.getBalance.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.pay.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.cancelPay.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.present.html"/> /
/// <see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.queryOrder.html"/>。</para>
/// <para><b>令牌面</b>：本域全部端点使用<b>小程序 access_token</b>（Query 注入，<c>MpTokenTypes.AccessToken</c>），
/// 与小程序基础线同构；非第三方平台代调用形态（component_access_token 归开放平台线）。</para>
/// <para><b>落位</b>：<c>Interfaces/Game/IMpGameService.cs</c>；<c>MiniProgramModule.Game</c>。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Game", TokenManage = nameof(IMpAppManager))]
public interface IMpGameService
{
    // ==================== 帧同步游戏房间（4 端点）====================

    /// <summary>
    /// 「创建帧同步游戏房间」：为小游戏创建帧同步房间，返回房间 ID。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.createGameRoom.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameCreateRoomRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>房间 ID，见 <see cref="GameCreateRoomResponse"/>。</returns>
    [Post("/wxa/createwxagameroom")]
    Task<GameCreateRoomResponse> CreateGameRoomAsync(
        GameCreateRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取帧数据」：获取指定房间的帧数据。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameFrame.html"/>。
    /// </summary>
    /// <param name="roomId">房间 ID。</param>
    /// <param name="beginFrameId">起始帧 ID。</param>
    /// <param name="count">帧数量。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>帧数据列表，见 <see cref="GameGetFrameResponse"/>。</returns>
    [Get("/wxa/getwxagameframe")]
    Task<GameGetFrameResponse> GetGameFrameAsync(
        [Query("room_id")] string roomId,
        [Query("begin_frame_id")] long beginFrameId,
        [Query("count")] int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取对局玩家位次信息」：获取房间内玩家的位次信息。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameIdentityInfo.html"/>。
    /// </summary>
    /// <param name="roomId">房间 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>玩家位次信息，见 <see cref="GameGetIdentityInfoResponse"/>。</returns>
    [Get("/wxa/getwxagameidentityinfo")]
    Task<GameGetIdentityInfoResponse> GetGameIdentityInfoAsync(
        [Query("room_id")] string roomId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取指定房间信息」：获取房间详细信息。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameRoomInfo.html"/>。
    /// </summary>
    /// <param name="roomId">房间 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>房间信息，见 <see cref="GameGetRoomInfoResponse"/>。</returns>
    [Get("/wxa/getwxagameroominfo")]
    Task<GameGetRoomInfoResponse> GetGameRoomInfoAsync(
        [Query("room_id")] string roomId,
        CancellationToken cancellationToken = default);

    // ==================== 用户数据存储（3 端点）====================

    /// <summary>
    /// 「删除已上报KV数据」：删除用户的 KV 数据。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.removeUserStorage.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameRemoveUserStorageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>空响应。</returns>
    [Post("/wxa/remove_user_storage")]
    Task RemoveUserStorageAsync(
        GameRemoveUserStorageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「写用户关系链互动数据存储」：写入用户关系链互动数据。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserInteractiveData.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameSetUserInteractiveDataRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>空响应。</returns>
    [Post("/wxa/setuserinteractivedata")]
    Task SetUserInteractiveDataAsync(
        GameSetUserInteractiveDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「上报用户数据」：上报用户的游戏数据。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserStorage.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameSetUserStorageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>空响应。</returns>
    [Post("/wxa/set_user_storage")]
    Task SetUserStorageAsync(
        GameSetUserStorageRequest request,
        CancellationToken cancellationToken = default);

    // ==================== 对局匹配规则（5 端点）====================

    /// <summary>
    /// 「创建对局匹配规则」：创建新的匹配规则。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.createMatchRule.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameCreateMatchRuleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>匹配规则 ID，见 <see cref="GameCreateMatchRuleResponse"/>。</returns>
    [Post("/wxa/business/gamematch/creatematchrule")]
    Task<GameCreateMatchRuleResponse> CreateMatchRuleAsync(
        GameCreateMatchRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除对局匹配规则」：删除指定的匹配规则。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.deleteMatchRule.html"/>。
    /// </summary>
    /// <param name="matchId">匹配规则 ID。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>空响应。</returns>
    [Post("/wxa/business/gamematch/deletematchrule")]
    Task DeleteMatchRuleAsync(
        [Query("match_id")] string matchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取对局匹配规则」：获取所有匹配规则。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.getAllMatchRule.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>匹配规则列表，见 <see cref="GameGetAllMatchRuleResponse"/>。</returns>
    [Post("/wxa/business/gamematch/getallmatchrule")]
    Task<GameGetAllMatchRuleResponse> GetAllMatchRuleAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「修改对局规则打开状态」：修改匹配规则的开启/关闭状态。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.setMatchIdOpenState.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameSetMatchOpenStateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>空响应。</returns>
    [Post("/wxa/business/gamematch/setmatchopenstate")]
    Task SetMatchOpenStateAsync(
        GameSetMatchOpenStateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「修改对局匹配规则」：更新匹配规则配置。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.updateMatchRule.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameUpdateMatchRuleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>空响应。</returns>
    [Post("/wxa/business/gamematch/updatematchrule")]
    Task UpdateMatchRuleAsync(
        GameUpdateMatchRuleRequest request,
        CancellationToken cancellationToken = default);

    // ==================== 米大师虚拟支付 business 族（1 端点）====================

    /// <summary>
    /// 「查询订单支付信息」：查询米大师订单支付信息。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/business.getPayForOrder.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameGetPayForOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>支付信息，见 <see cref="GameGetPayForOrderResponse"/>。</returns>
    [Post("/wxa/business/getpayfororder")]
    Task<GameGetPayForOrderResponse> GetPayForOrderAsync(
        GameGetPayForOrderRequest request,
        CancellationToken cancellationToken = default);

    // ==================== 内容安全（2 端点）====================

    /// <summary>
    /// 「文本内容安全识别」：检测文本内容是否违规。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.msgSecCheck.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameMsgSecCheckRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>检测结果，见 <see cref="GameMsgSecCheckResponse"/>。</returns>
    [Post("/wxa/game/content_spam/msg_sec_check")]
    Task<GameMsgSecCheckResponse> MsgSecCheckAsync(
        GameMsgSecCheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「图片内容安全识别」：检测图片内容是否违规。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.mediaCheck.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameMediaCheckRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>检测结果，见 <see cref="GameMediaCheckResponse"/>。</returns>
    [Post("/wxa/game/content_spam/media_check_sync")]
    Task<GameMediaCheckResponse> MediaCheckAsync(
        GameMediaCheckRequest request,
        CancellationToken cancellationToken = default);

    // ==================== 米大师 V2 虚拟支付 game 族（5 端点）====================

    /// <summary>
    /// 「查询余额」：查询用户在游戏中的虚拟货币余额。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.getBalance.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameGetBalanceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>余额信息，见 <see cref="GameGetBalanceResponse"/>。</returns>
    [Post("/wxa/game/getbalance")]
    Task<GameGetBalanceResponse> GetBalanceAsync(
        GameGetBalanceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「支付」：发起虚拟货币支付。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.pay.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GamePayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>支付结果，见 <see cref="GamePayResponse"/>。</returns>
    [Post("/wxa/game/pay")]
    Task<GamePayResponse> PayAsync(
        GamePayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「取消支付」：取消未完成的支付订单。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.cancelPay.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameCancelPayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>取消结果，见 <see cref="GameCancelPayResponse"/>。</returns>
    [Post("/wxa/game/cancelpay")]
    Task<GameCancelPayResponse> CancelPayAsync(
        GameCancelPayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「赠送」：赠送虚拟货币给用户。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.present.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GamePresentRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>赠送结果，见 <see cref="GamePresentResponse"/>。</returns>
    [Post("/wxa/game/present")]
    Task<GamePresentResponse> PresentAsync(
        GamePresentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询订单状态」：查询米大师 V2 订单状态。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.queryOrder.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="GameQueryOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单信息，见 <see cref="GameQueryOrderResponse"/>。</returns>
    [Post("/wxa/game/queryorderinfo")]
    Task<GameQueryOrderResponse> QueryOrderAsync(
        GameQueryOrderRequest request,
        CancellationToken cancellationToken = default);
}
