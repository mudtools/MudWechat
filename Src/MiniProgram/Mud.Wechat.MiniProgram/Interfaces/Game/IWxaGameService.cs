// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.Game;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「小游戏」域 SDK（<b>20 端点</b>：帧同步房间 4 + 用户数据存储 3 + 对局匹配规则 5 + 米大师索要单 1 + 内容安全 2 + 米大师 V2 游戏币 5）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：本域全部端点使用<b>小程序 access_token</b>（Query 注入，<c>MpTokenTypes.AccessToken</c>），
/// 与小程序基础线同构；官方 20 页「第三方调用」一节<b>逐页写明不支持第三方平台调用</b> ⇒ 本域<b>没有</b>
/// <c>component_access_token</c> 代调用形态（与云开发域恰好相反）。</para>
/// <para><b>第二重凭据面</b>：官方另有 8 支端点在 <c>access_token</c> 之外强制把<b>宿主计算</b>的签名放 Query ——
/// 数据存储三支（<c>openid</c> + <c>signature</c> + <c>sig_method</c>）与米大师 V2 五支
/// （<c>signature</c> + <c>sig_method</c> + <c>pay_sig</c>）。三者都是方法级 <c>[Query]</c> 业务参数、
/// 不属令牌注入面 ⇒ 不改 <c>[Token]</c> 声明。算法口径（官方签名页
/// <see href="https://developers.weixin.qq.com/minigame/dev/guide/open-ability/virtual-payment/signature.html"/>）：
/// <c>signature = hex(hmac_sha256(session_key, post_body))</c>；
/// <c>pay_sig = hex(hmac_sha256(app_key, uri + '&amp;' + post_body))</c>，其中 <c>uri</c> <b>只取路径、
/// 必须舍去 <c>?</c> 及之后部分</b>，<c>app_key</c> 取与请求 <c>env</c> 对应的「mp-支付基础配置」AppKey，
/// 且 <c>post_body</c> 必须与实际发出的请求体<b>逐字节一致</b>（补一个空格即 <c>90010</c>/<c>90011</c>）；
/// <c>sig_method</c> 官方唯一取值 <see cref="WxaSignatureMethods.HmacSha256"/>。
/// <b>敏感值</b>：<c>signature</c> / <c>pay_sig</c> / <c>session_key</c> / 支付 <c>app_key</c> 一律
/// <b>不得进日志、遥测或异常消息</b>。<c>pay_sig</c> 不在组件精确匹配脱敏词表内 ⇒ 由 <c>AddGameApi()</c> 的注册委托
/// 复用 <c>XPaySensitiveQueryKeys.RegisterAll()</c> 登记（键集单一来源，守卫 GM6 锁定该委托体真的调用登记）。</para>
/// <para><b>定位键</b>：帧同步房间面<b>没有 room_id 入参</b> —— 四支接口里三支以 <c>access_info</c>
/// （<c>createGameRoom</c> 返回的 <c>data.access_info</c>，即 <c>onGameEnd</c> 侧的 gameAccessInfo）定位对局。</para>
/// <para><b>官方文档笔误留档</b>（本域按「逐字照抄 + 不臆造」处置，均由守卫 GM1/GM4 锁定）：
/// ① <c>setMatchIdOpenState</c> 的接口名与实际路径不一致，实际为 <c>/wxa/business/gamematch/setmatchopenstate</c>；
/// ② <c>getAllMatchRule</c> 返回表把 <c>need_room_service_info</c> 那行名字误写成第二个 <c>team_member_count</c>
/// （返回示例用前者 ⇒ DTO 取 <c>need_room_service_info</c>）；
/// ③ <c>setMatchIdOpenState</c> / <c>updateMatchRule</c> 的 <c>match_id</c> 类型列标 number，
/// 而请求示例是 44 位字符串 ⇒ 按线格式取 string；
/// ④ <c>updateMatchRule</c> 页面正文参数表只有 <c>match_id</c> + <c>open_state</c> 两支（与「修改匹配规则」的语义
/// 不吻合，疑为复用状态页表格）⇒ 不臆造 <c>team_count</c> / <c>game_room_info</c> 等未出现的参数，
/// 与 <c>setMatchIdOpenState</c> 共用同一请求类型；
/// ⑤ <c>getGameFrame</c> 请求体表内重复列出一行「access_token / cloudbase_access_token」，与查询参数同义 ⇒
/// HTTP 形态只走 Query 的 <c>access_token</c>；
/// ⑥ <c>business.getPayForOrder</c> 页面顶部标注<b>「接口已废弃」</b>但正文完整可读 ⇒ 判为在册、按 20 端点全量落地，
/// 废弃状态在本注释与守卫 remarks 留档（不因此删除端点：官方未下架页面，且清单口径是 minigame 树全量）。</para>
/// <para><b>频率限制</b>：本域 20 页里只有内容安全两支给出端点级数字（文本 <b>1 万/分钟、1000 万/天</b>，
/// 同步耗时约 500ms、建议超时 1s；图片 <b>2 千/分钟、20 万/天</b>、单文件 ≤10M，同步耗时约 3s、建议超时 5s；
/// 超限官方回 <c>750032</c> / <c>750033</c>）。其余 18 页「注意事项」写明无特殊注意事项 ⇒ 注释留档该空缺。</para>
/// <para><b>落位</b>：<c>Interfaces/Game/IWxaGameService.cs</c>；<c>MiniProgramModule.Game</c>；
/// 注册组 <c>Game</c>（守卫 GM1~GM7）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Game", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaGameService
{
    // ==================== 帧同步游戏房间（4 端点）====================

    /// <summary>
    /// 「创建帧同步游戏房间」（<c>POST /wxa/createwxagameroom</c>，不支持第三方平台调用与云调用之外的形态）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.createGameRoom.html"/>。
    /// </summary>
    /// <param name="request">请求体（必填四支 <c>game_tick</c> / <c>open_id_list</c> / <c>udp_reliability_strategy</c> / <c>start_percent</c>），见 <see cref="GameCreateRoomRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>房间访问凭证 <c>data.access_info</c>，见 <see cref="GameCreateRoomResponse"/>。</returns>
    /// <remarks>
    /// <para><b>返回的是 access_info，不是房间号</b>：本域房间面三支查询接口都以它定位对局。</para>
    /// <para><b>官方必填口径</b>：<c>game_tick</c> 不得小于 33ms；<c>room_ext_info</c> ≤32 字节（超长回 <c>4013</c>，
    /// 请求数据非法回 <c>4003</c>）；<c>need_user_info</c> 为 true 时未授权用户<b>加入房间直接失败</b>（不是「加入但不下发昵称」）。</para>
    /// <para><b>官方请求示例未列 <c>open_id_list</c></b>（参数表列其为必填）⇒ 以参数表为准。</para>
    /// </remarks>
    [Post("/wxa/createwxagameroom")]
    Task<GameCreateRoomResponse> CreateGameRoomAsync(
        [Body] GameCreateRoomRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「分片拉取对局游戏帧」（<b>POST</b> <c>/wxa/getwxagameframe</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameFrame.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>access_info</c> + 帧区间 <c>begin_frame_id</c> / <c>end_frame_id</c>，官方全部必填），见 <see cref="GameGetFrameRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>帧列表与 <c>has_more</c>，见 <see cref="GameGetFrameResponse"/>。</returns>
    /// <remarks>
    /// <para><b>动词是 POST</b>：本域「读」类端点里最易想当然的一支——官方请求体是 JSON 对象，
    /// <c>begin_frame_id</c> / <c>end_frame_id</c> 落 Body 而非 Query；帧区间是<b>左闭右开</b>。</para>
    /// <para><b>帧号从 1 开始</b>；<c>pkg_list[].b</c> 为 true 表示空帧，<c>dp_id</c> 是微信内部字段
    /// （官方原文「无需理解，为了更快速返回数据没有去掉该字段内容」）⇒ 不得据此做业务判断。</para>
    /// <para>错误码：<c>1</c> 数据不存在 / <c>5</c> 非法的请求数据 / <c>6000</c> 获取数据失败。页面标注「本接口无代码示例」。</para>
    /// </remarks>
    [Post("/wxa/getwxagameframe")]
    Task<GameGetFrameResponse> GetGameFrameAsync(
        [Body] GameGetFrameRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取对局玩家位次信息」（<c>GET /wxa/getwxagameidentityinfo</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameIdentityInfo.html"/>。
    /// </summary>
    /// <param name="accessInfo">房间/对局访问凭证（官方 Query 参数 <c>access_info</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>每名玩家的 <c>client_id</c> 与 <c>open_id</c>，见 <see cref="GameGetIdentityInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>请求体为「无」</b>：官方请求体表明确写「无」⇒ <c>access_info</c> 只能是 Query 参数，
    /// 不得为它造 request DTO。</para>
    /// <para><b><c>client_id</c> 不是座位号</b>：官方原文「一旦分配不能修改」，座位号在
    /// <see cref="GetGameRoomInfoAsync"/> 的 <c>member_list[].pos_num</c>。</para>
    /// <para>错误码：<c>1</c> 数据不存在。</para>
    /// </remarks>
    [Get("/wxa/getwxagameidentityinfo")]
    Task<GameGetIdentityInfoResponse> GetGameIdentityInfoAsync(
        [Query("access_info")] string accessInfo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取指定房间信息」（<c>GET /wxa/getwxagameroominfo</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameRoomInfo.html"/>。
    /// </summary>
    /// <param name="accessInfo">房间/对局访问凭证（官方 Query 参数 <c>access_info</c>，必填；官方原文「不同于 access_token」）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>房间状态 + 对局参数 + 成员列表（本域最密集的返回体），见 <see cref="GameGetRoomInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>请求体为「无」</b>：同上，只有 Query 的 <c>access_token</c> + <c>access_info</c>。</para>
    /// <para><b>房间状态四值</b>：1 组队中 / 2 对局已开始 / 3 对局已结束 / 4 房间已销毁 ⇒ 轮询须按 4 终止。</para>
    /// <para><b><c>headimg</c> / <c>nickname</c> 仅用户授权才下发</b>（与 <c>need_user_info</c> 联动）⇒
    /// 空值不是数据缺失，不得当成错误重试。</para>
    /// <para>页面标注「本接口无代码示例」；错误码只有 <c>-1</c> / <c>0</c>。</para>
    /// </remarks>
    [Get("/wxa/getwxagameroominfo")]
    Task<GameGetRoomInfoResponse> GetGameRoomInfoAsync(
        [Query("access_info")] string accessInfo,
        CancellationToken cancellationToken = default);

    // ==================== 用户数据存储（3 端点）====================

    /// <summary>
    /// 「删除已上报KV数据」（<c>POST /wxa/remove_user_storage</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.removeUserStorage.html"/>。
    /// </summary>
    /// <param name="request">请求体（官方参数名是<b>单数</b> <c>key</c>、类型为字符串数组），见 <see cref="GameRemoveUserStorageRequest"/>。</param>
    /// <param name="openid">用户 openid（官方强制 Query，必填）。</param>
    /// <param name="signature">用户登录态签名（官方 Query 参数 <c>signature</c>，必填，由宿主以 <c>session_key</c> 计算）。</param>
    /// <param name="sigMethod">签名哈希方法（官方 Query 参数 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>只有 errcode / errmsg 的通用应答，见 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>三支存储端点的 Query 形状完全相同</b>（<c>openid</c> + <c>signature</c> + <c>sig_method</c> 均官方强制），
    /// 且都是<b>路由带下划线</b>的 <c>remove_user_storage</c> / <c>set_user_storage</c>，与同族的 <c>setuserinteractivedata</c> 连写不同 ⇒
    /// 照抄原文不得顺手统一。</para>
    /// <para><b>参数名单数 <c>key</c></b>：官方请求示例 <c>["gold", "score"]</c>，不是 <c>keys</c>。</para>
    /// </remarks>
    [Post("/wxa/remove_user_storage")]
    Task<WxaResponse> RemoveUserStorageAsync(
        [Body] GameRemoveUserStorageRequest request,
        [Query("openid")] string openid,
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「写用户关系链互动数据存储」（<c>POST /wxa/setuserinteractivedata</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserInteractiveData.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>kv_list[].key</c> 目前可为 '1' - '50'），见 <see cref="GameSetUserInteractiveDataRequest"/>。</param>
    /// <param name="openid">用户 openid（官方强制 Query，必填）。</param>
    /// <param name="signature">用户登录态签名（官方 Query 参数 <c>signature</c>，必填）。</param>
    /// <param name="sigMethod">签名哈希方法（官方 Query 参数 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>只有 errcode / errmsg 的通用应答，见 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>请求体与 <see cref="SetUserStorageAsync"/> 同形</b>（<c>kv_list</c> / <c>key</c> / <c>value</c>），
    /// 但路由与语义是「关系链互动数据」，两支不得互相替代。</para>
    /// <para>错误码：<c>87081</c> 非法的 openid / <c>87082</c> 非法的 key。</para>
    /// </remarks>
    [Post("/wxa/setuserinteractivedata")]
    Task<WxaResponse> SetUserInteractiveDataAsync(
        [Body] GameSetUserInteractiveDataRequest request,
        [Query("openid")] string openid,
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「上报用户数据」（<c>POST /wxa/set_user_storage</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/data/storage.setUserStorage.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>kv_list[].key</c> / <c>value</c>），见 <see cref="GameSetUserStorageRequest"/>。</param>
    /// <param name="openid">用户 openid（官方强制 Query，必填）。</param>
    /// <param name="signature">用户登录态签名（官方 Query 参数 <c>signature</c>，必填）。</param>
    /// <param name="sigMethod">签名哈希方法（官方 Query 参数 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>只有 errcode / errmsg 的通用应答，见 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方四条托管数据硬限制</b>（本域唯一给出实质「注意事项」的一页）：每个 openid 托管数据
    /// <b>不超过 128 个 key-value 对</b>（超限 <c>87018</c>）；上报列表里<b>每一项 key+value 长度不超过 1K（1024）字节</b>
    /// （超限 <c>87016</c>）；<b>每个 key 长度不超过 128 字节</b>（超限 <c>87019</c>）；用户存储数据量超限 <c>87017</c>。</para>
    /// <para><b>排行榜形态</b>：要把数据显示在小游戏中心排行榜时，<c>value</c> 须是顶层含 <c>wxgame</c> 的 JSON Object
    /// 序列化字符串，<c>wxgame</c> 内为 <c>score</c>（Int32，必填）与 <c>update_time</c>（Int64，必填），
    /// 且 <c>wxgame</c> 下<b>不可自定义字段</b>（同级可自定义）⇒ 与参数表的 number 标注并存，
    /// 故 <see cref="GameSetUserStorageRequestKvListItem.Value"/> 取 <c>JsonElement?</c>。</para>
    /// </remarks>
    [Post("/wxa/set_user_storage")]
    Task<WxaResponse> SetUserStorageAsync(
        [Body] GameSetUserStorageRequest request,
        [Query("openid")] string openid,
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        CancellationToken cancellationToken = default);

    // ==================== 对局匹配规则（5 端点）====================

    /// <summary>
    /// 「创建对局匹配规则」（<c>POST /wxa/business/gamematch/creatematchrule</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.createMatchRule.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>team_count</c> / <c>team_member_count</c> 必填，可带 <c>need_room_service_info</c> + <c>game_room_info</c>），见 <see cref="GameCreateMatchRuleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>新建的 <c>match_id</c>，见 <see cref="GameMatchRuleIdResponse"/>。</returns>
    /// <remarks>
    /// <para><b>规模上限</b>：每个小游戏持有的 matchid 上限 <b>20</b>（超限 <c>500014</c>）；
    /// <c>need_room_service_info</c> 填 1 时要求 <c>team_count × team_member_count</c> <b>不超过 10 人</b>
    /// （超限 <c>500008</c>）⇒ 这两条是调用方须自证的规模约束，官方不静默截断。</para>
    /// <para>返回的 <c>match_id</c> 是加入匹配时必须携带的匹配池 id；请求体无签名类计算参数。</para>
    /// </remarks>
    [Post("/wxa/business/gamematch/creatematchrule")]
    Task<GameMatchRuleIdResponse> CreateMatchRuleAsync(
        [Body] GameCreateMatchRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「删除对局匹配规则」（<c>POST /wxa/business/gamematch/deletematchrule</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.deleteMatchRule.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>match_id</c> 必填，官方放 Body 而非 Query），见 <see cref="GameDeleteMatchRuleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>只有 errcode / errmsg 的通用应答，见 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para><b>删除即释放匹配池</b>：官方原文提示线上正在使用的 matchid 须谨慎 ⇒ 这是不可逆操作，
    /// 调用方应自行做「在用」校验。</para>
    /// <para>错误码：<c>1</c> matchid 不存在 / <c>500005</c> 无效的 matchid / <c>200000</c> 无效的请求数据。</para>
    /// </remarks>
    [Post("/wxa/business/gamematch/deletematchrule")]
    Task<WxaResponse> DeleteMatchRuleAsync(
        [Body] GameDeleteMatchRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取对局匹配规则」（<c>POST /wxa/business/gamematch/getallmatchrule</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.getAllMatchRule.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>本小游戏全部 matchid 的规则与打开状态，见 <see cref="GameGetAllMatchRuleResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方是 POST 且请求体为「无」</b>：本域「查询」类端点同样照官方写 POST，且不得凭空造 request DTO。</para>
    /// <para><b>官方返回表笔误</b>：<c>team_member_count</c> 出现两次，第二行描述实为
    /// <c>need_room_service_info</c>，返回示例亦用后者 ⇒ <see cref="GameGetAllMatchRuleResponseMatchRuleListItem.NeedRoomServiceInfo"/>
    /// 取真实字段名（守卫 GM4 锁定）。</para>
    /// </remarks>
    [Post("/wxa/business/gamematch/getallmatchrule")]
    Task<GameGetAllMatchRuleResponse> GetAllMatchRuleAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「修改对局规则打开状态」（<c>POST /wxa/business/gamematch/setmatchopenstate</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.setMatchIdOpenState.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>match_id</c> + <c>open_state</c> 0 关闭 / 1 打开），见 <see cref="GameMatchOpenStateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回显 <c>match_id</c>，见 <see cref="GameMatchRuleIdResponse"/>。</returns>
    /// <remarks>
    /// <para><b>实际路径与接口英文名不一致</b>：页面/接口名是 <c>setMatchIdOpenState</c>，
    /// 官方请求地址正文写 <c>setmatchopenstate</c> ⇒ 路由照抄请求地址，不得按英文名拼路径。</para>
    /// <para><b>设为 0（关闭）会释放匹配池</b>，官方原文提示线上使用中的 matchid 需谨慎。</para>
    /// <para>错误码：<c>500012</c> open_state 的值不合法 / <c>500005</c> 无效的 matchid。</para>
    /// </remarks>
    [Post("/wxa/business/gamematch/setmatchopenstate")]
    Task<GameMatchRuleIdResponse> SetMatchIdOpenStateAsync(
        [Body] GameMatchOpenStateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「修改对局匹配规则」（<c>POST /wxa/business/gamematch/updatematchrule</c>，不支持第三方平台调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.updateMatchRule.html"/>。
    /// </summary>
    /// <param name="request">请求体（官方本页参数表只有 <c>match_id</c> + <c>open_state</c>，与 <see cref="SetMatchIdOpenStateAsync"/> 共用类型），见 <see cref="GameMatchOpenStateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>回显 <c>match_id</c>，见 <see cref="GameMatchRuleIdResponse"/>。</returns>
    /// <remarks>
    /// <para><b>只能改状态为关闭的 matchid</b>（状态经 <see cref="SetMatchIdOpenStateAsync"/> 变更）。</para>
    /// <para><b>官方文档疑似复用</b>：本页「请求体 Request Payload」表实际只列 <c>match_id</c> 与 <c>open_state</c> 两支，
    /// 与页面标题「修改匹配规则」的语义不吻合 ⇒ 按逐字照抄原则<b>不臆造</b> <c>team_count</c> /
    /// <c>game_room_info</c> 等正文未出现的参数，并与状态页共用同一请求类型（守卫 GM4 登记该共用裁决）。
    /// 若日后官方补齐该页参数表，须新建独立请求类型而非改共用的这支。</para>
    /// </remarks>
    [Post("/wxa/business/gamematch/updatematchrule")]
    Task<GameMatchRuleIdResponse> UpdateMatchRuleAsync(
        [Body] GameMatchOpenStateRequest request,
        CancellationToken cancellationToken = default);

    // ==================== 米大师索要单（1 端点）====================

    /// <summary>
    /// 「查询索要单」（<c>POST /wxa/business/getpayfororder</c>，官方页面顶部标注<b>接口已废弃</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment/business.getPayForOrder.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>appid</c> / <c>order_no</c> / <c>out_trade_no</c> 三支官方全部必填），见 <see cref="GameGetPayForOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>索要单信息（状态七值 + 沙箱标记 + 支付时间），见 <see cref="GameGetPayForOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>废弃但在册</b>：页面标注「接口已废弃」，但文档正文完整渲染、参数表与错误码俱在，且本域口径是
    /// minigame 树 20 端点全量 ⇒ 保留建模（裁决见接口 remarks 第 ⑥ 条）。索要单不存在官方回 <c>-10000404</c>。</para>
    /// <para><b>状态七值</b>：1 待支付 / 2 已支付 / 3 已超时 / 4 米大师已发放游戏币 / 5 游戏业务已发货 /
    /// 6 游戏业务发货失败 ⇒ <c>4</c> 只代表米大师侧完成，<c>5</c> 才是业务侧发货成功；<c>pay_time</c> 仅支付后存在。</para>
    /// <para><c>order_no</c>（微信索要单号）与 <c>out_trade_no</c>（游戏业务单号）是两支不同的单号，均必填。</para>
    /// </remarks>
    [Post("/wxa/business/getpayfororder")]
    Task<GameGetPayForOrderResponse> GetPayForOrderAsync(
        [Body] GameGetPayForOrderRequest request,
        CancellationToken cancellationToken = default);

    // ==================== 内容安全（2 端点）====================

    /// <summary>
    /// 「文本内容安全识别」（<c>POST /wxa/game/content_spam/msg_sec_check</c>，不支持第三方平台调用；云调用 <c>wxa.game.contentSpam.msgSecCheck</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.msgSecCheck.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>version</c> 固定 2 / <c>scene</c> 1~5 / <c>content</c> ≤2500 字），见 <see cref="GameMsgSecCheckRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>综合结果 + 明细策略列表，见 <see cref="GameMsgSecCheckResponse"/>。</returns>
    /// <remarks>
    /// <para><b>频率与耗时（本域仅有的端点级数字之一）</b>：单 AppID <b>1 万/分钟、1000 万/天</b>
    /// （可申请上调），同步返回、一般耗时 500ms 内 ⇒ <b>建议超时 1s</b>；超限回 <c>750032</c> / <c>750033</c>。</para>
    /// <para><b>与公众号 / 小程序内容安全线互不替代</b>：本支是游戏侧路由 <c>/wxa/game/content_spam/</c>，
    /// <c>version</c> 固定 2（错回 <c>750030</c>）、场景值错回 <c>40129</c>、非 POST 回 <c>43002</c>、
    /// appid 与 openid 不匹配回 <c>43104</c>、该游戏未开通回 <c>750031</c>。</para>
    /// <para><b>明细项级 errcode</b>：<c>detail[].errcode</c> 为 0 时该项才有效 ⇒ 不能只看接口级 errcode。
    /// 官方返回示例出现表格未登记的 <c>level</c> 字段 ⇒ 按参数表建模、不凭空补字段。</para>
    /// </remarks>
    [Post("/wxa/game/content_spam/msg_sec_check")]
    Task<GameMsgSecCheckResponse> MsgSecCheckAsync(
        [Body] GameMsgSecCheckRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「图片内容安全识别」（<c>POST /wxa/game/content_spam/media_check_sync</c>，不支持第三方平台调用；云调用 <c>wxa.game.contentSpam.mediaCheckSync</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.mediaCheck.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>version</c> 固定 2 / <c>scene</c> / <c>media_url</c> / <c>media_type</c> 固定 2 图片），见 <see cref="GameMediaCheckRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>综合结果 + 明细策略列表（相对文本支无 replaced_content / keyword），见 <see cref="GameMediaCheckResponse"/>。</returns>
    /// <remarks>
    /// <para><b>接口英文名与路由不同形</b>：英文名 <c>gamesecurity.mediaCheck</c>，实际路径是
    /// <c>media_check_sync</c> ⇒ 照抄请求地址。</para>
    /// <para><b>频率与耗时</b>：单 AppID <b>2 千/分钟、20 万/天</b>，单个文件 ≤10M（可申请上调），
    /// 同步耗时约 3s ⇒ <b>建议超时 5s</b>；<c>media_type</c> 非法回 <c>750034</c>。</para>
    /// <para><b>图片链接必须可被检测服务器下载</b>；官方另建议资料场景的头像定期重检（头像可被换）。</para>
    /// </remarks>
    [Post("/wxa/game/content_spam/media_check_sync")]
    Task<GameMediaCheckResponse> MediaCheckAsync(
        [Body] GameMediaCheckRequest request,
        CancellationToken cancellationToken = default);

    // ==================== 米大师 V2 游戏币（5 端点）====================

    /// <summary>
    /// 「查询游戏币余额」（<c>POST /wxa/game/getbalance</c>，不支持第三方平台调用；云调用 <c>wxa.game.getBalance</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.getBalance.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>offer_id</c> / <c>ts</c> / <c>zone_id</c> / <c>env</c> / <c>user_ip</c> 全部必填，<c>coin_type</c> 选填），见 <see cref="GameGetBalanceRequest"/>。</param>
    /// <param name="signature">用户登录态签名（官方 Query 参数 <c>signature</c>，必填；<c>hex(hmac_sha256(session_key, post_body))</c>）。</param>
    /// <param name="sigMethod">签名哈希方法（官方 Query 参数 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="paySig">支付请求签名（官方 Query 参数 <c>pay_sig</c>，必填；<c>hex(hmac_sha256(app_key, uri + '&amp;' + post_body))</c>，<c>uri</c> 不带 <c>?</c> 之后部分）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>总余额 + 赠送余额 + 四项累计值 + 首充标记，见 <see cref="GameGetBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方是 POST</b>（「查询」二字极易写成 GET）；请求体<b>没有</b> amount / description 类字段，
    /// <c>balance</c> 是响应字段。</para>
    /// <para><b>签名即业务前置</b>：<c>90010</c> signature 错误 / <c>90011</c> pay_sig 错误 /
    /// <c>90016</c> sessionkey 过期须重走登录流程 ⇒ 三支 Query 签名缺一或 <c>post_body</c> 与实际发出的 body
    /// 不逐字节一致即失败。</para>
    /// <para><b>到账延迟</b>：官方说明某些情况下支付后余额延迟到账，需按间隔定期查询 ⇒ 不得把一次查询为 0 当失败。</para>
    /// <para><b>适用范围</b>：官方标「小游戏（仅认证）」。</para>
    /// </remarks>
    [Post("/wxa/game/getbalance")]
    Task<GameGetBalanceResponse> GetBalanceAsync(
        [Body] GameGetBalanceRequest request,
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「扣除游戏币」（<c>POST /wxa/game/pay</c>，不支持第三方平台调用；云调用 <c>wxa.game.pay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.pay.html"/>。
    /// </summary>
    /// <param name="request">请求体（六支公共字段 + <c>amount</c> &gt; 0 + <c>bill_no</c>，可选 <c>payitem</c> / <c>remark</c>），见 <see cref="GamePayRequest"/>。</param>
    /// <param name="signature">用户登录态签名（官方 Query 参数 <c>signature</c>，必填）。</param>
    /// <param name="sigMethod">签名哈希方法（官方 Query 参数 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="paySig">支付请求签名（官方 Query 参数 <c>pay_sig</c>，必填；<c>uri</c> 取 <c>/wxa/game/pay</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单号 + 扣款后余额 + 本次消耗的赠送币，见 <see cref="GamePayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>本端点是「扣币」而非下单收款</b>：收款走充值链路，本支只消耗用户已充值的代币余额。</para>
    /// <para><b>幂等是官方给的恢复手段</b>：<c>bill_no</c> 在 openid + zoneid 下唯一；同一 <c>bill_no</c>
    /// 48 小时内幂等；接口超时或 <c>errcode = -1</c> 时订单状态未知 ⇒ <b>必须用相同 bill_no 重试</b>，
    /// 直到拿到 <c>90012</c>（订单号重复，即已扣成功）或非 -1 的明确失败。换单号重试就是重复扣款。</para>
    /// <para><c>bill_no</c> 约束（2.0 新增）：长度 ≤63，仅限数字 / 大小写字母 / <c>*</c> / <c>-</c>，且不得以 <c>*</c> 开头。</para>
    /// <para><c>90013</c> 余额不足是业务失败而非系统错误 ⇒ 重试无意义。</para>
    /// </remarks>
    [Post("/wxa/game/pay")]
    Task<GamePayResponse> PayAsync(
        [Body] GamePayRequest request,
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「退回扣除游戏币」（<c>POST /wxa/game/cancelpay</c>，不支持第三方平台调用；云调用 <c>wxa.game.cancelPay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.cancelPay.html"/>。
    /// </summary>
    /// <param name="request">请求体（公共六支 + 本次 <c>amount</c> + 本次 <c>bill_no</c> + 原扣除单 <c>pay_bill_no</c>），见 <see cref="GameCancelPayRequest"/>。</param>
    /// <param name="signature">用户登录态签名（官方 Query 参数 <c>signature</c>，必填）。</param>
    /// <param name="sigMethod">签名哈希方法（官方 Query 参数 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="paySig">支付请求签名（官方 Query 参数 <c>pay_sig</c>，必填；<c>uri</c> 取 <c>/wxa/game/cancelpay</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>订单号 + 变动后余额 + 本次赠送币数量，见 <see cref="GamePayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>两个单号同时必填，极易漏</b>：<c>bill_no</c> 是本次退回单号、<c>pay_bill_no</c> 是被退回的原扣除单号。</para>
    /// <para><b>部分退可多次</b>：须在原扣除订单有效时间内，且累计退回额不超过原扣除额。</para>
    /// <para><b>官方文档复用件留档</b>：本页请求/返回示例是从扣除页复制的（示例体含 <c>payitem</c> / <c>remark</c>
    /// 且缺 <c>pay_bill_no</c>），与本页参数表不一致 ⇒ 以参数表为准；应答表亦与扣除页同文（含「扣款后的余额」措辞），
    /// 故与 <see cref="PayAsync"/> 共用 <see cref="GamePayResponse"/>。</para>
    /// </remarks>
    [Post("/wxa/game/cancelpay")]
    Task<GamePayResponse> CancelPayAsync(
        [Body] GameCancelPayRequest request,
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「给用户赠送游戏币」（<c>POST /wxa/game/present</c>，不支持第三方平台调用；云调用 <c>wxa.game.present</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.present.html"/>。
    /// </summary>
    /// <param name="request">请求体（公共六支 + <c>amount</c> 不能为 0 + <c>bill_no</c>），见 <see cref="GamePresentRequest"/>。</param>
    /// <param name="signature">用户登录态签名（官方 Query 参数 <c>signature</c>，必填）。</param>
    /// <param name="sigMethod">签名哈希方法（官方 Query 参数 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="paySig">支付请求签名（官方 Query 参数 <c>pay_sig</c>，必填；<c>uri</c> 取 <c>/wxa/game/present</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>赠送订单号 + 赠送后余额（无 used_present_amount），见 <see cref="GamePresentResponse"/>。</returns>
    /// <remarks>
    /// <para><b>与扣币页的两处差异</b>：参数表<b>没有</b> <c>payitem</c> / <c>remark</c>（故赠送语义无处放备注），
    /// 应答只有 <c>bill_no</c> + <c>balance</c> ⇒ 单独建模，不与 <see cref="GamePayResponse"/> 共用。</para>
    /// <para><c>90012</c> 在本页官方描述为「订单已存在」（与扣币页同码不同措辞）⇒ 按「已赠送成功」处理。</para>
    /// <para>本页错误码表<b>没有</b> <c>90000</c>（订单不存在）与 <c>90013</c>（余额不足）。</para>
    /// </remarks>
    [Post("/wxa/game/present")]
    Task<GamePresentResponse> PresentAsync(
        [Body] GamePresentRequest request,
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「查询订单状态」（<c>POST /wxa/game/queryorderinfo</c>，不支持第三方平台调用，且<b>本域五支 pay_v2 中唯一不支持云调用</b>的一支）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/midas-payment-v2/pay_v2.queryOrder.html"/>。
    /// </summary>
    /// <param name="request">请求体（公共五支 + <c>out_trade_no</c> + <c>biz_id</c>；<b>官方参数表无 <c>user_ip</c></b>），见 <see cref="GameQueryOrderRequest"/>。</param>
    /// <param name="signature">用户登录态签名（官方 Query 参数 <c>signature</c>，必填）。</param>
    /// <param name="sigMethod">签名哈希方法（官方 Query 参数 <c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</param>
    /// <param name="paySig">支付请求签名（官方 Query 参数 <c>pay_sig</c>，必填；<c>uri</c> 取 <c>/wxa/game/queryorderinfo</c>——官方签名页枚举列表未单列本支，按同规则取路径部分）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>支付状态 / 发货状态 / 完成时间 / 双侧单号，见 <see cref="GameQueryOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>接口英文名与路径不同形</b>：<c>pay_v2.queryOrder</c> → 实际路径 <c>queryorderinfo</c>；
    /// 返回是<b>扁平结构</b>，无 <c>data.order_info[]</c> 包装。</para>
    /// <para><b>定位键是 <c>out_trade_no</c> + <c>biz_id</c></b>（不是 <c>bill_no</c>）；
    /// <c>biz_id</c> 必填，1 代币 / 2 道具直购；未找到回 <c>90019</c>。</para>
    /// <para><b><c>pay_state</c> 与 <c>deliver_state</c> 是两个维度</b>：已支付（2）不等于已发货（2）⇒
    /// 补发货判定须看 <c>deliver_state</c>。官方请求示例缺 <c>zone_id</c> 等必填项，以参数表为准。</para>
    /// </remarks>
    [Post("/wxa/game/queryorderinfo")]
    Task<GameQueryOrderResponse> QueryOrderAsync(
        [Body] GameQueryOrderRequest request,
        [Query("signature")] string signature,
        [Query("sig_method")] string sigMethod,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);
}
