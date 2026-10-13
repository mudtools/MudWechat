// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「直播 · 小助手与主播副号」域 SDK（8 端点：小助手增删改查 4 + 主播副号增删改查 4；
/// 官方 livebroadcast 分区 → studio-management）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成</b>：官方 livebroadcast 分区全集 38 端点的六分之一，按「协作者身份」职责归档 ——
/// 小助手（房间级协助管理成员）与主播副号（房间级替身主播）在官方页面分列两节，但请求形状高度对称
/// （均为 <c>{roomId, username}</c> 一族），共用注册组 <c>Live</c> 与同一接口便于宿主一次注入。
/// 直播间本体见 <see cref="IWxaLiveRoomService"/>，商品面见 <see cref="IWxaLiveRoomGoodsService"/>
/// 与 <see cref="IWxaLiveGoodsService"/>，成员角色见 <see cref="IWxaLiveRoleService"/>。
/// </para>
/// <para>
/// <b>小助手 ≠ 成员角色</b>：小助手是<b>单房间</b>的协助管理身份（上限官方以 <c>maxCount</c> 逐房间返回，
/// 示例值 10），角色（管理员 / 运营者 / 主播）是<b>账号级</b>权限，走 <see cref="IWxaLiveRoleService"/>；
/// 两者不可互相替代，也不共享路由。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 52；第三方代调用形态由宿主以小程序应用配置供给令牌，本 SDK 不声明
/// 组件 / 授权令令牌，P3-R2）。域级取证与「适用范围字段异常」留档见 <see cref="IWxaLiveRoomService"/> 注释。
/// </para>
/// <para>
/// <b>GET 端点带「请求体」表</b>：<c>getassistantlist</c> 与 <c>getsubanchor</c> 两页把 <c>roomId</c>
/// 列在「请求体 Request Payload」表内，而请求地址行只带 <c>access_token</c> ⇒ 本 SDK 按 <c>[Query]</c>
/// 落地（裁决同 <see cref="IWxaLiveRoomService.GetPushUrlAsync"/>）。
/// </para>
/// <para>
/// <b>调用限制</b>：八端点中除 <c>deletesubanchor</c> / <c>getsubanchor</c> 官方标注「本接口无特殊注意事项」外，
/// 其余六端点调用额度均为 <b>10000 次/天</b>（逐页登记于各端点注释）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Live", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaLiveAssistantService
{
    /// <summary>
    /// 添加管理直播间小助手（<c>POST /wxaapi/broadcast/room/addassistant</c>，云调用 <c>liveBroadcast.addAssistant</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_addveassistant.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>users[]</c>，元素为 <c>{username, nickname}</c>），见 <see cref="WxaLiveAddAssistantRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> ＋ JSON 请求体；调用额度 <b>10000 次/天</b>。官方错误码：<c>-1</c>（系统繁忙）。
    /// </para>
    /// <para>
    /// <b>页面名与路由交叉（照录官方）</b>：官方文件名为 <c>api_addveassistant</c>、接口英文名
    /// <c>addveAssistant</c>，而路由与云调用方法为 <c>addassistant</c> / <c>liveBroadcast.addAssistant</c>
    /// ⇒ 路由与 URL 一律照录官方原文，方法名取 <see cref="AddAssistantAsync"/>。
    /// </para>
    /// <para><b>批量形态</b>：<c>users</c> 为对象数组（本域唯一批量入口），单元素两字段均必填。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/addassistant")]
    Task<WxaResponse> AddAssistantAsync(
        [Body] WxaLiveAddAssistantRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询直播间小助手（<c>GET /wxaapi/broadcast/room/getassistantlist</c>，云调用 <c>liveBroadcast.getAssistantList</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getassistantlist.html"/>。
    /// </summary>
    /// <param name="roomId">房间 ID（<c>roomId</c>，官方必填；官方把该参数列在「请求体」表内，本 SDK 按 Query 落地）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>小助手列表 + <c>count</c> + <c>maxCount</c>，见 <see cref="WxaLiveAssistantListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>；调用额度 <b>10000 次/天</b>。官方错误码：<c>-1</c>（系统繁忙）。</para>
    /// <para>
    /// <b>响应字段名陷阱</b>：列表元素用 <c>alias</c> 表达微信号（<b>不是</b> <c>username</c>），
    /// 而三支写端点的请求字段名为 <c>username</c> ⇒ 读回后再写须做字段名映射，见 <see cref="WxaLiveAssistantItem"/>。
    /// </para>
    /// <para><b><c>maxCount</c> 是逐房间值</b>：官方同时返回 <c>count</c> 与 <c>maxCount</c>，宿主侧加人前以此为闸。</para>
    /// </remarks>
    [Get("/wxaapi/broadcast/room/getassistantlist")]
    Task<WxaLiveAssistantListResponse> GetAssistantListAsync(
        [Query("roomId")] long roomId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改直播间小助手（<c>POST /wxaapi/broadcast/room/modifyassistant</c>，云调用 <c>liveBroadcast.modifyAssistant</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_modifyassistant.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>username</c> + <c>nickname</c> 三字段均必填），见 <see cref="WxaLiveModifyAssistantRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。官方错误码：<c>-1</c>（系统繁忙）。</para>
    /// <para><b>逐成员操作</b>：与 <see cref="AddAssistantAsync"/> 的数组形态不同，本端点一次只改一人，且须同时传昵称（覆盖语义）。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/modifyassistant")]
    Task<WxaResponse> ModifyAssistantAsync(
        [Body] WxaLiveModifyAssistantRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除直播间小助手（<c>POST /wxaapi/broadcast/room/removeassistant</c>，云调用 <c>liveBroadcast.removeAssistant</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_removeassistant.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>username</c>；与 <see cref="AddSubAnchorAsync"/> / <see cref="ModifySubAnchorAsync"/> 共用 <see cref="WxaLiveRoomUserRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。官方错误码表只列 <c>40001</c>
    /// （access_token 无效）⇒ 删不存在的小助手无专用错误码，须按幂等语义处置。
    /// </para>
    /// <para><b>动词命名照官方</b>：小助手族用 <c>remove</c>，副号族用 <c>delete</c>（官方两页原文如此），SDK 不做统一。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/removeassistant")]
    Task<WxaResponse> RemoveAssistantAsync(
        [Body] WxaLiveRoomUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加主播副号（<c>POST /wxaapi/broadcast/room/addsubanchor</c>，云调用 <c>liveBroadcast.addSubAnchor</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_addsubanchor.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>username</c>，共用 <see cref="WxaLiveRoomUserRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。</para>
    /// <para>官方错误码：<c>500004</c>（不能设置重复的副号）/ <c>500005</c>（副号不能和主号重复）。</para>
    /// <para><b>副号语义</b>：副号是可替主播开播的微信号，房间维度唯一。创建直播间时以 <c>subAnchorWechat</c> 指定，
    /// 而编辑直播间官方参数表<b>不含</b>该字段 ⇒ 建房后的副号变更只能走本族三支端点（添加 / 修改 / 删除）。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/addsubanchor")]
    Task<WxaResponse> AddSubAnchorAsync(
        [Body] WxaLiveRoomUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改主播副号（<c>POST /wxaapi/broadcast/room/modifysubanchor</c>，云调用 <c>liveBroadcast.modifySubAnchor</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_modifysubanchor.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>username</c>，共用 <see cref="WxaLiveRoomUserRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。</para>
    /// <para>官方错误码：<c>500002</c>（副号未实名）/ <c>500004</c>（不能设置重复的副号）——与 <see cref="AddSubAnchorAsync"/> 的错误码集<b>不同</b>（本端点独有 <c>500002</c>，添加端点独有 <c>500005</c>）。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/modifysubanchor")]
    Task<WxaResponse> ModifySubAnchorAsync(
        [Body] WxaLiveRoomUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除主播副号（<c>POST /wxaapi/broadcast/room/deletesubanchor</c>，云调用 <c>liveBroadcast.deleteSubAnchor</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deletesubanchor.html"/>。
    /// </summary>
    /// <param name="request">请求体（<b>只有 <c>roomId</c> 一字段</b>，官方原文如此，见 <see cref="WxaLiveRoomIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；本接口官方标注「无特殊注意事项」。官方错误码：<c>-1</c>（系统繁忙）。</para>
    /// <para>
    /// <b>请求体不含 <c>username</c></b>：官方参数表只有房间 ID ⇒ 语义为「清空该房间副号」，
    /// 不得沿用 <see cref="WxaLiveRoomUserRequest"/> 造出带微信号的形态（守卫 LB4 锁定 DTO 归属）。
    /// </para>
    /// </remarks>
    [Post("/wxaapi/broadcast/room/deletesubanchor")]
    Task<WxaResponse> DeleteSubAnchorAsync(
        [Body] WxaLiveRoomIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取主播副号（<c>GET /wxaapi/broadcast/room/getsubanchor</c>，云调用 <c>liveBroadcast.getSubAnchor</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getsubanchor.html"/>。
    /// </summary>
    /// <param name="roomId">房间 ID（<c>roomId</c>，官方必填；按 Query 落地，理由同 <see cref="GetAssistantListAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>username</c>（主播微信号），见 <see cref="WxaLiveSubAnchorResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>；本接口官方标注「无特殊注意事项」。官方错误码：<c>-1</c>（系统繁忙）。</para>
    /// <para><b>官方响应示例不含 <c>roomId</c></b>：只有 <c>username</c> + <c>errcode</c>（<c>errmsg</c> 列在参数表内）⇒ DTO 按参数表建模。</para>
    /// </remarks>
    [Get("/wxaapi/broadcast/room/getsubanchor")]
    Task<WxaLiveSubAnchorResponse> GetSubAnchorAsync(
        [Query("roomId")] long roomId,
        CancellationToken cancellationToken = default);
}
