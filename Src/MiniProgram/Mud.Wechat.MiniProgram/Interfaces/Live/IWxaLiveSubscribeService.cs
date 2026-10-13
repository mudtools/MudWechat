// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「直播 · 长期订阅」域 SDK（2 端点：获取长期订阅用户 + 发送直播开始事件群发；
/// 官方 livebroadcast 分区 → subscribe-management，路由均在 <c>/wxa/business/</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域是「开播提醒」闭环的两端</b>：用户在小程序直播间点「长期订阅」后，商家先以
/// <see cref="GetFollowersAsync"/> 拉取订阅用户 <c>openid</c>，再以 <see cref="PushMessageAsync"/>
/// 向选定用户群发开播事件；群发的<b>最终结果</b>不在这两个端点的应答里，而在官方「长期订阅群发结果回调」
/// 中（按 <c>message_id</c> 对应），回调面属本线未覆盖的推送通道，宿主须自行接收。
/// </para>
/// <para>
/// <b>路由前缀例外（照录官方）</b>：两端点都在 <c>/wxa/business/</c> 而非 <c>/wxaapi/broadcast/</c>
/// ——与 <see cref="IWxaLiveRoomService.GetLiveInfoAsync"/>、<see cref="IWxaLiveGoodsService.GetGoodsWarehouseAsync"/>
/// 同族，构成本线直播域的四个 <c>/wxa/business/</c> 端点。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 52）。域级取证、「适用范围字段异常」留档见 <see cref="IWxaLiveRoomService"/> 注释。
/// </para>
/// <para>
/// <b>调用限制</b>：<c>get_wxa_followers</c> 官方标注「无特殊注意事项」（单次最多 2000 条）；
/// <c>push_message</c> 调用额度 <b>5000 次/天</b> 且<b>群发订阅用户个数限制 2000/次</b>（官方注意事项原文）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Live", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaLiveSubscribeService
{
    /// <summary>
    /// 获取长期订阅用户（<c>POST /wxa/business/get_wxa_followers</c>，云调用 <c>liveBroadcast.getFollowers</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_getfollowers.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>limit</c> 与 <c>page_break</c> 官方均选填），见 <see cref="WxaLiveFollowersRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订阅用户列表 + 下一页 <c>page_break</c>，见 <see cref="WxaLiveFollowersResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> ＋ JSON 请求体（本端点官方<b>动词为 POST</b>，参数表列在「请求体」内，
    /// 无 GET 歧义）；<c>limit</c> 默认 200、最大 2000。<b>游标分页</b>：第一页不带 <c>page_break</c>，
    /// 其后各页须回传上一页响应中的 <c>page_break</c>（官方原文如此，非 offset）。
    /// </para>
    /// <para>官方错误码表只列 <c>40001</c>（access_token 无效）。</para>
    /// <para>
    /// <b>官方类型列异常</b>：<c>followers</c> 列表元素的 <c>openid</c> 官方类型列写作
    /// <c>array</c>（语义为单值 OpenId），<c>subscribe_time</c> 类型列写作 <c>timestamp</c>
    /// ⇒ SDK 按语义建为 <c>string?</c> / <c>long?</c>，裁决留档于 <see cref="WxaLiveFollowerItem"/>
    /// 并由守卫 LB4 锁定。
    /// </para>
    /// </remarks>
    [Post("/wxa/business/get_wxa_followers")]
    Task<WxaLiveFollowersResponse> GetFollowersAsync(
        [Body] WxaLiveFollowersRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送直播开始事件（长期订阅群发，<c>POST /wxa/business/push_message</c>，云调用 <c>liveBroadcast.pushMessage</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_pushmessage.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>room_id</c> + <c>user_openid</c> 字符串数组，均必填），见 <see cref="WxaLivePushMessageRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群发消息标识 <c>message_id</c>，见 <see cref="WxaLivePushMessageResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> ＋ JSON 请求体；调用额度 <b>5000 次/天</b>，单次群发用户数上限 <b>2000</b>
    /// （官方注意事项原文）。字段名为 snake_case（<c>room_id</c> / <c>user_openid</c>），
    /// 与本域多数 <c>roomId</c> 形态不同，照录官方原文。
    /// </para>
    /// <para>
    /// <b><c>errcode = 0</c> 只代表「触发成功」</b>：官方返回表说明原文「若 errcode=0 仅表明触发接口成功，
    /// 最终群发结果请看【长期订阅群发结果回调】」⇒ 不得据此向用户承诺送达，且 <c>message_id</c> 须落库以便与回调对账。
    /// </para>
    /// <para>官方错误码：<c>9410005</c>（当日另外一个房间已推送过）/ <c>9410006</c>（推送的直播间非直播中状态）⇒ 群发为<b>日粒度</b>单房间配额。</para>
    /// </remarks>
    [Post("/wxa/business/push_message")]
    Task<WxaLivePushMessageResponse> PushMessageAsync(
        [Body] WxaLivePushMessageRequest request,
        CancellationToken cancellationToken = default);
}
