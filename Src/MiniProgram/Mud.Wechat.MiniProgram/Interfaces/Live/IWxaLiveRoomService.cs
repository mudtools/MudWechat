// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「直播 · 直播间管理」域 SDK（11 端点：创建 / 编辑 / 删除直播间 + 推流地址 + 分享二维码 +
/// 直播间列表与回放 + 官方收录 / 回放 / 客服 / 禁言四开关 + 导入商品；官方 livebroadcast 分区 → studio-management）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 2026-10-13 逐页核验，livebroadcast 分区全集 38 端点）</b>：本接口取其中「直播间本体」11 端点，
/// 其余按职责分到 <see cref="IWxaLiveAssistantService"/>（小助手与主播副号 8）、
/// <see cref="IWxaLiveRoomGoodsService"/>（直播间内商品操作 7）、<see cref="IWxaLiveGoodsService"/>（商品库管理 7）、
/// <see cref="IWxaLiveRoleService"/>（成员角色 3）、<see cref="IWxaLiveSubscribeService"/>（长期订阅群发 2）。
/// 六接口共用注册组 <c>Live</c>。
/// </para>
/// <para>
/// <b>P3-R1 第 2 项裁决的解除（同批变更，务必先读）</b>：本线曾按「小程序直播插件已停止接入」把
/// <c>wxaapi/broadcast</c> 前缀<b>整族零声明</b>。2026-10-13 逐页取证推翻该判断的前提：
/// ① 38 个服务端 API 页面全部完整渲染参数表，且每页均声明「权限集 id 52」「服务器端调用」「云调用方法
/// <c>liveBroadcast.*</c>」「可使用 <c>access_token</c>、<c>authorizer_access_token</c>」；
/// ② 现行 <c>OpenApiDoc/livebroadcast</c> 目录树与旧 <c>miniprogram/dev/server/API/livebroadcast</c> 树的
/// 路由 + 动词清单<b>逐条相同</b>（38 = 38）；③「页面不存在或已失效」的是
/// <c>dev/framework/activity/live-player-plugin</c>（直播<b>插件</b>，前端组件形态），不是服务端 API。
/// ⇒ 守卫 <c>MiniProgramP3RulingsContractGuards</c> 的 broadcast 条款已随之收窄为「清单式在册」。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>，不新增令牌类型常量（新增会让 <c>MpTokenManagerRegistry</c> 单槽 <c>Resolve</c>
/// 返回 <c>null</c> ⇒ errcode 令牌自愈静默失效）。第三方代调用形态（<c>authorizer_access_token</c>）
/// 由宿主以小程序应用配置供给令牌，本 SDK 不声明组件 / 授权令令牌（P3-R2）。
/// </para>
/// <para>
/// <b>适用范围字段异常（诚实留档）</b>：38 页的「适用范围」段官方统一写
/// 「本接口暂未明确可调用账号类型，或在业务中根据调用传参自行确定是否可调用，请以实际调用情况为准」
/// —— <b>没有</b>「小程序」字样。本线归属判据因此取「令牌可用 <c>access_token</c>（小程序应用级）
/// + 文档树位于 <c>miniprogram/dev/server/API/</c>」两条，而非适用范围段。
/// </para>
/// <para>
/// <b>GET 端点带「请求体」表（本域三处，裁决同交易保障域）</b>：<c>getpushurl</c> / <c>getsharedcode</c> /
/// <c>getassistantlist</c> / <c>getsubanchor</c> 四页在 GET 端点下把业务参数写进「请求体 Request Payload」表，
/// 但请求地址行只带 <c>access_token</c> ⇒ 本 SDK 一律按 <c>[Query]</c> 落地（全仓零 GET + <c>[Body]</c> 先例）。
/// </para>
/// <para>
/// <b>调用限制</b>：创建 / 编辑直播间 10000 次/天，删除直播间 10000 次/天，<c>getliveinfo</c> <b>100000 次/天</b>
/// （官方注意事项逐页登记于各端点注释）；图片字段一律填 mediaID 且 <b>mediaID 三天内有效</b>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Live", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaLiveRoomService
{
    /// <summary>
    /// 创建直播间（<c>POST /wxaapi/broadcast/room/create</c>，云调用 <c>liveBroadcast.createRoom</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_createroom.html"/>。
    /// </summary>
    /// <param name="request">请求体（官方 18 字段），见 <see cref="WxaLiveCreateRoomRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>房间 ID 与「小程序直播」小程序码，见 <see cref="WxaLiveCreateRoomResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ JSON 请求体；Query 仅携带 <c>access_token</c>（权限集 52）。</para>
    /// <para>
    /// <b>时间窗硬约束（官方原文）</b>：开播须在当前时间 10 分钟后且不超过 6 个月后；
    /// 场次时长不得短于 30 分钟、不得超过 24 小时。
    /// </para>
    /// <para>
    /// <b>成功后必须保存 <c>roomId</c></b>：本域其余端点全部以房间 ID 定位（官方示例返回
    /// <c>{ "roomId": 811, "qrcode_url": "..." }</c> 形态）。
    /// </para>
    /// <para>官方错误码 15 条，含 <c>300036</c>（主播微信号未实名认证）/ <c>300038</c>（未在小程序管理后台配置客服）。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/create")]
    Task<WxaLiveCreateRoomResponse> CreateRoomAsync(
        [Body] WxaLiveCreateRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑直播间（<c>POST /wxaapi/broadcast/room/editroom</c>，云调用 <c>liveBroadcast.editRoom</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_editroom.html"/>。
    /// </summary>
    /// <param name="request">请求体（官方 16 字段，定位键为 <c>id</c>），见 <see cref="WxaLiveEditRoomRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ JSON 请求体；调用额度 <b>10000 次/天</b>。</para>
    /// <para>
    /// <b>不可编辑项</b>：官方本页参数表<b>无</b> <c>type</c> / <c>subAnchorWechat</c> / <c>createrWechat</c>
    /// 三字段 ⇒ 直播间类型创建后不可改；副号改走 <see cref="IWxaLiveAssistantService"/> 的三支副号端点。
    /// </para>
    /// <para>官方错误码：<c>300022</c>（此房间号不存在）/ <c>300023</c>（房间状态拦截）/ <c>300030</c>（主播微信号不合法）等 10 条。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/editroom")]
    Task<WxaResponse> EditRoomAsync(
        [Body] WxaLiveEditRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除直播间（<c>POST /wxaapi/broadcast/room/deleteroom</c>，云调用 <c>liveBroadcast.deleteRoom</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deleteroom.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>id</c> 一字段，官方原文如此非 <c>roomId</c>），见 <see cref="WxaLiveDeleteRoomRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ JSON 请求体；调用额度 <b>10000 次/天</b>。</para>
    /// <para>
    /// <b>状态拦截</b>：<c>300023</c>「房间状态 拦截（当前房间状态不允许此操作）」⇒ 直播中的房间不可删除，
    /// 状态口径见 <see cref="DataModels.Live.WxaLiveRoomStatus"/>。
    /// </para>
    /// <para><b>破坏性操作</b>：删除后房间与回放不可恢复，宿主侧须自行做二次确认。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/deleteroom")]
    Task<WxaResponse> DeleteRoomAsync(
        [Body] WxaLiveDeleteRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播间推流地址（<c>GET /wxaapi/broadcast/room/getpushurl</c>，云调用 <c>liveBroadcast.getPushUrl</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getpushurl.html"/>。
    /// </summary>
    /// <param name="roomId">房间 ID（<c>roomId</c>，官方必填；官方把该参数列在「请求体」表内，本 SDK 按 Query 落地）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推流地址（<c>pushAddr</c>），见 <see cref="WxaLivePushUrlResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>（误用 POST 回 <c>43001</c> require GET method）；本接口无特殊注意事项。</para>
    /// <para>
    /// <b>仅推流型直播间有意义</b>：官方直播间类型 <c>type</c> 为 0（手机直播）时无需推流地址。
    /// </para>
    /// <para>
    /// <b>敏感值</b>：官方示例的推流地址含 <c>txSecret</c> / <c>txTime</c> 凭据参数 ⇒ 禁止写入日志、
    /// 遥测或异常消息（与 <c>permanent_code</c> 同级别处置）。
    /// </para>
    /// </remarks>
    [Get("/wxaapi/broadcast/room/getpushurl")]
    Task<WxaLivePushUrlResponse> GetPushUrlAsync(
        [Query("roomId")] long roomId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播间分享二维码（<c>GET /wxaapi/broadcast/room/getsharedcode</c>，云调用 <c>liveBroadcast.getSharedCode</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getsharedcode.html"/>。
    /// </summary>
    /// <param name="roomId">房间 ID（<c>roomId</c>，官方必填；按 Query 落地，理由同 <see cref="GetPushUrlAsync"/>）。</param>
    /// <param name="customParams">自定义参数（<c>params</c>，选填；官方字段名为 <c>params</c>，因 C# 关键字改用形参名）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码 / 分享路径 / 海报三字段，见 <see cref="WxaLiveSharedCodeResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>；本接口无特殊注意事项。官方错误码 <c>1001</c>（请求参数非法）。</para>
    /// <para><b>官方返回体表不含 <c>errmsg</c></b>（只列 <c>errcode</c> / <c>cdnUrl</c> / <c>pagePath</c> / <c>posterUrl</c>）⇒ 判成败只看 <c>errcode</c>。</para>
    /// </remarks>
    [Get("/wxaapi/broadcast/room/getsharedcode")]
    Task<WxaLiveSharedCodeResponse> GetSharedCodeAsync(
        [Query("roomId")] long roomId,
        [Query("params")] string? customParams = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播间列表和回放（<c>POST /wxa/business/getliveinfo</c>，云调用 <c>liveBroadcast.getLiveInfo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>start</c> / <c>limit</c> 必填；取回放时带 <c>action</c> + <c>room_id</c>），见 <see cref="WxaLiveGetLiveInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>房间列表或回放列表（两数组互斥），见 <see cref="WxaLiveInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> ＋ JSON 请求体，路由在 <c>/wxa/business/</c> 而非 <c>/wxaapi/broadcast/</c>
    /// ——本域四个 <c>/wxa/business/</c> 端点之一（另三个见 <see cref="IWxaLiveGoodsService.GetGoodsWarehouseAsync"/>
    /// 与 <see cref="IWxaLiveSubscribeService"/>）。
    /// </para>
    /// <para>调用额度：<b>100000 次/天</b>（官方注意事项，本域最高）。</para>
    /// <para>
    /// <b>一路由两形态</b>：<c>action</c> 缺省回 <c>room_info</c>（按 <c>start_time</c> 降序）；
    /// <c>action = "get_replay"</c> 回 <c>live_replay</c> 且<b>不返回</b> <c>room_info</c>，此时 <c>room_id</c> 必填。
    /// </para>
    /// <para><b>回放链接有时效</b>：官方回 <c>expire_time</c>，不得长期缓存 <c>media_url</c>。</para>
    /// </remarks>
    [Post("/wxa/business/getliveinfo")]
    Task<WxaLiveInfoResponse> GetLiveInfoAsync(
        [Body] WxaLiveGetLiveInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 官方收录管理（<c>POST /wxaapi/broadcast/room/updatefeedpublic</c>，云调用 <c>liveBroadcast.updateFeedPublic</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatefeedpublic.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>isFeedsPublic</c>），见 <see cref="WxaLiveFeedPublicRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。官方错误码：<c>-1</c>（系统繁忙）。</para>
    /// <para><b>开关语义相反于其他三兄弟</b>：<c>isFeedsPublic</c> 为「1 开启」，而回放 / 客服 / 禁言用 <c>close*</c> 或 <c>ban*</c> 表达「1 关闭」。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/updatefeedpublic")]
    Task<WxaResponse> UpdateFeedPublicAsync(
        [Body] WxaLiveFeedPublicRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 回放功能管理（<c>POST /wxaapi/broadcast/room/updatereplay</c>，云调用 <c>liveBroadcast.updateReplay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatereplay.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>closeReplay</c>），见 <see cref="WxaLiveReplaySwitchRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。官方错误码只有 <c>300023</c>（房间状态拦截）一条。</para>
    /// <para><b>开启后取内容</b>：回放内容须经 <see cref="GetLiveInfoAsync"/> 带 <c>action = "get_replay"</c> 拉取，本端点只切开关。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/updatereplay")]
    Task<WxaResponse> UpdateReplayAsync(
        [Body] WxaLiveReplaySwitchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 客服功能管理（<c>POST /wxaapi/broadcast/room/updatekf</c>，云调用 <c>liveBroadcast.updateKF</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatekf.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>closeKf</c>），见 <see cref="WxaLiveKfSwitchRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。官方错误码：<c>-1</c>（系统繁忙）。</para>
    /// <para><b>前置条件</b>：未在小程序管理后台配置客服时，创建直播间即报 <c>300038</c> ⇒ 本端点只在已配置的房间间切换。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/updatekf")]
    Task<WxaResponse> UpdateKfAsync(
        [Body] WxaLiveKfSwitchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 禁言管理（<c>POST /wxaapi/broadcast/room/updatecomment</c>，云调用 <c>liveBroadcast.updateComment</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatecomment.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>id</c> + <c>banComment</c>），见 <see cref="WxaLiveUpdateCommentRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。官方错误码：<c>300022</c> / <c>300023</c>。</para>
    /// <para>
    /// <b>官方参数表与官方示例字段名不一致</b>：参数表为 <c>id</c>，页内示例写 <c>roomId</c>
    /// ⇒ 本 SDK 按<b>参数表</b>建模为 <c>id</c>（裁决留档于 DTO 注释，守卫 LB 系列锁定）。
    /// </para>
    /// <para><b>与创建时的 <c>closeComment</c> 区分</b>：后者是开播前关闭评论入口且开播后不可开启，本端点是直播中禁言 / 解禁。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/updatecomment")]
    Task<WxaResponse> UpdateCommentAsync(
        [Body] WxaLiveUpdateCommentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入商品（把商品库商品加入直播间，<c>POST /wxaapi/broadcast/room/addgoods</c>，云调用 <c>liveBroadcast.addGoods</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_importgoods.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>ids</c> numarray + <c>roomId</c>），见 <see cref="WxaLiveImportGoodsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。<b>页面别名交叉</b>：官方文件名与接口英文名为
    /// <c>importGoods</c>，路由与云调用方法为 <c>addgoods</c> / <c>liveBroadcast.addGoods</c> ⇒ 路由照录，
    /// 方法名取 <see cref="ImportGoodsAsync"/>（与商品库侧 <see cref="IWxaLiveGoodsService.AddGoodsAsync" />「添加并提审」区分）。
    /// </para>
    /// <para>
    /// <b>前置：商品须审核通过</b>（官方 <c>300025</c> 商品审核未通过 / <c>300024</c> 商品不存在）；
    /// 数量超上限报 <c>300033</c>（官方页面未公布上限数字 ⇒ SDK 不做本地校验）。
    /// </para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/addgoods")]
    Task<WxaResponse> ImportGoodsAsync(
        [Body] WxaLiveImportGoodsRequest request,
        CancellationToken cancellationToken = default);
}
