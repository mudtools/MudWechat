// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopSharer;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · 分享员（推客/分销族）」域 SDK（8 端点：批量邀请 / 批量解绑 /
/// 已绑定列表 / 单人查看 / 总带货数据 / 直播间订单汇总 / 直播间带货数据汇总 / 设置分享模式；
/// 官方 minishopopencomponent2「分享员接口」分区，8 页路由一律 <c>/shop/sharer/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// </para>
/// <para>
/// <b>「分享员」即业务口径的推客/分销员</b>：绑定动作官方语义是<b>邀请</b>——<c>bind</c> 的
/// <c>success_list</c> 只代表邀请模版消息下发成功（邀请中），用户同意后才绑定，终态由回调
/// <c>sharer_bind_staus_change</c>（官方页面拼写即 <c>staus</c>）推送；<c>unbind</c> 的 <c>success_list</c>
/// 同样以 callback 推送为准 ⇒ 两支都<b>不是</b>同步终态端点，宿主不得以 <c>errcode=0</c> 当作绑定/解绑完成。
/// </para>
/// <para>
/// <b>非幂等与冷却面</b>：<c>bind</c> 同一分享员一天最多邀请三次、10 分钟内最多一次（逐人 <c>reason_code=204</c>），
/// 已绑定再邀 <c>202</c>、用户拒绝再邀 <c>203</c>；绑定总数上限 10000（<c>200</c>）；
/// <c>unbind</c> 拒绝列官方例举「未到解绑时间」；<c>set_share_mode</c> 两次更改间隔不得短于 <b>30 天</b>
/// （<c>1310005</c>）⇒ 本域端点<b>一律不得</b>放进自动重试管道。
/// </para>
/// <para>
/// <b>端点级频率数字官方空缺</b>：8 页正文均无「次/秒 / 次/日」类频率数字，也未标注权限集编号、云调用与
/// 第三方平台代调用支持面 ⇒ 本仓只按自建 <c>access_token</c> 面落地，注释留档该空缺，不得凭第三方 SDK 或记忆补数；
/// 上文「三次 / 10 分钟 / 30 天 / 10000 / page_size 100 与 20」皆为<b>业务上限或规模上限</b>，不得当端点频率解读。
/// </para>
/// <para>
/// <b>金额单位一律分</b>：<c>gmv</c>、<c>sale_price</c> / <c>real_price</c>、<c>price_info</c> 五支金额字段官方
/// 逐行标注单位分 ⇒ 不得与元口径混算；<c>gmv</c> 是「有效带货额」（剔除退款等无效单），与商家自有统计不同源。
/// </para>
/// <para>
/// <b>敏感面（AGENTS §8）</b>：本域全族的 <c>openid</c> / <c>sharer_openid</c> / <c>nickname</c>、
/// 订单树内 <c>transaction_id</c> / <c>prepay_id</c> / 快递单号，以及带货额与佣金类金额属个人敏感信息或
/// 个人金融信息 ⇒ <b>禁止写入日志、遥测或异常消息</b>；宿主侧持久化与截图同样受此约束。
/// </para>
/// <para>
/// <b>share_attribute 一页的三分身</b>：官方该页同时承载前端接口 <c>wx.getChannelsShareKey()</c>、
/// 回调事件 <c>miniprogram_sharer_action</c> 与服务端端点 <c>set_share_mode</c>；本域在册的只有服务端一支
/// （路由 <c>/shop/sharer/set_share_mode</c>），另两支属前端与回调面、不在本接口声明。
/// </para>
/// <para>
/// <b>与小店线零交叉</b>：本域 8 条全部是小程序令牌面的 <c>/shop/sharer/*</c>，与小店线
/// <c>/channels/ec/*</c> 无任何交叠（守卫 SHOPSH7 锁定）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopSharer", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopSharerService
{
    /// <summary>
    /// 批量邀请分享员（<c>POST /shop/sharer/bind</c>，权限集：官方未标注）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/sharer/bind_sharer.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>openids</c> 一支键，必填），见 <see cref="WxaShopSharerBindRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>三分列表 + 逐人 <c>result_list</c>（<c>result_code</c> / <c>reason_code</c>），见 <see cref="WxaShopSharerBindResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：POST + JSON 请求体；官方本页无端点级频率数字（「一天三次 / 10 分钟一次」是逐人邀请业务上限）。</para>
    /// <para><b>成功 ≠ 绑定</b>：<c>success_list</c> 仅表示邀请模版消息下发成功（邀请中），绑定终态由回调
    /// <c>sharer_bind_staus_change</c> 推送；三列表与 <c>result_list</c> 并存（新旧两套口径同时回填），不得只解析一种。</para>
    /// <para><b>逐人错误码</b>：<c>200</c> 分享员总数达上限 10000、<c>201</c> openid 错误、<c>202</c> 已绑定、
    /// <c>203</c> 用户拒绝再次邀请、<c>204</c> 邀请过频；<c>fail_list</c> 可重试、<c>refuse_list</c> 不可重试。</para>
    /// <para><b>敏感面</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/sharer/bind")]
    Task<WxaShopSharerBindResponse> BindSharerAsync(
        [Body] WxaShopSharerBindRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量解绑分享员（<c>POST /shop/sharer/unbind</c>，权限集：官方未标注）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/sharer/unbind_sharer.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>openids</c> 一支键，必填），见 <see cref="WxaShopSharerUnbindRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>三分列表（无 <c>result_list</c>，与 bind 应答形态不同），见 <see cref="WxaShopSharerUnbindResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：POST + JSON 请求体；官方本页无端点级频率数字。</para>
    /// <para><b>解绑结果以 callback 推送为准</b>（官方原文「解除绑定后，通过 callback 推送结果」）；
    /// <c>refuse_list</c> 官方例举「未到解绑时间」⇒ 存在解绑时间窗约束，被拒条目不可重试。</para>
    /// <para><b>本支应答无 <c>result_list</c></b>：与 <see cref="BindSharerAsync"/> 应答不同形，两类型不共用（守卫 SHOPSH5 锁定）。</para>
    /// <para><b>敏感面</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/sharer/unbind")]
    Task<WxaShopSharerUnbindResponse> UnbindSharerAsync(
        [Body] WxaShopSharerUnbindRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已绑定分享员列表（<c>POST /shop/sharer/get_sharer_list</c>，权限集：官方未标注）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/sharer/get_sharer_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page</c> / <c>page_size</c> 两支均必填），见 <see cref="WxaShopSharerListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>sharers</c> 列表 + <c>total_num</c>，见 <see cref="WxaShopSharerListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：POST + JSON 请求体；翻页为 <c>page</c>（从 1 起）+ <c>page_size</c>（≤<b>100</b>），无游标分页。</para>
    /// <para><b>列表键名是 <c>sharers</c></b>（非 <c>list</c> / <c>data.list</c>）；本域三支列表键名各不同
    /// （<c>sharers</c> / <c>orders</c> / <c>lives</c>），守卫 SHOPSH5 逐条锁定。</para>
    /// <para><b>时间戳形态</b>：<c>invited_time</c> / <c>bind_time</c> 官方表标 number（unix 秒）、示例却为字符串 ⇒
    /// 本仓以表为准建 <c>long?</c> 并留档该冲突。</para>
    /// <para><b>敏感面</b>：<c>openid</c> / <c>nickname</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/sharer/get_sharer_list")]
    Task<WxaShopSharerListResponse> GetSharerListAsync(
        [Body] WxaShopSharerListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查看单个分享员（<c>POST /shop/sharer/search_sharer</c>，权限集：官方未标注）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/sharer/search_sharer.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>openid</c> 一支键，必填），见 <see cref="WxaShopSharerSearchRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>字段平铺于应答顶层（含 <c>bind_status</c>），见 <see cref="WxaShopSharerSearchResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：POST + JSON 请求体；官方本页无端点级频率数字。</para>
    /// <para><b><c>bind_status</c> 官方枚举</b>：<c>1</c> 已绑定、<c>2</c> 拒绝邀请、<c>3</c> 邀请中、<c>4</c> 邀请过期
    /// ——「邀请中/过期」仅本支可见，列表支只回已绑定者 ⇒ 排查邀请不落地须走本支而非翻页。</para>
    /// <para><b>应答平铺无包装</b>：与 <see cref="GetSharerListAsync"/> 的同名条目字段（无 <c>bind_status</c>）不同形，两类型不共用。</para>
    /// <para><b>敏感面</b>：<c>openid</c> / <c>nickname</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/sharer/search_sharer")]
    Task<WxaShopSharerSearchResponse> SearchSharerAsync(
        [Body] WxaShopSharerSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分享员总带货数据（<c>POST /shop/sharer/get_sharer_data_summary</c>，权限集：官方未标注）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/sharer/get_sharer_data_summary.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>openid</c> 一支键，必填），见 <see cref="WxaShopSharerDataSummaryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>gmv</c>（单位分）/ <c>order_cnt</c> / <c>user_cnt</c>，见 <see cref="WxaShopSharerDataSummaryResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：POST + JSON 请求体；官方本页无端点级频率数字。</para>
    /// <para><b><c>gmv</c> 单位分</b>（官方原文「分享员有效带货额（单位分）」）且口径为「有效」（剔除无效单）
    /// ⇒ 与商家自有统计不同源，不得直接对账。</para>
    /// <para><b>数据可见面受分享模式影响</b>：切到分享归因模式（<see cref="SetShareModeAsync"/> 取值 2）后
    /// 微信侧不再统计展示该小程序分享员带货数据 ⇒ 本支返回值可能停滞，先查模式再查数据。</para>
    /// <para><b>敏感面</b>：<c>openid</c> 与带货金额属个人敏感/金融信息，禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/sharer/get_sharer_data_summary")]
    Task<WxaShopSharerDataSummaryResponse> GetSharerDataSummaryAsync(
        [Body] WxaShopSharerDataSummaryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分享员的直播间订单汇总（<c>POST /shop/sharer/get_sharer_live_order_list</c>，权限集：官方未标注）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/sharer/get_sharer_live_order_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>live_export_id</c> / <c>page</c> / <c>page_size</c> 四支均必填），见 <see cref="WxaShopSharerLiveOrderListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>orders</c> 列表（含五段式 <c>order_detail</c>）+ <c>total_num</c>，见 <see cref="WxaShopSharerLiveOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：POST + JSON 请求体；翻页 <c>page</c> + <c>page_size</c>（≤<b>100</b>）。</para>
    /// <para><b><c>live_export_id</c> 必填</b>：取自 <see cref="GetSharerLiveSummaryListAsync"/> 的
    /// <c>lives[].live_export_id</c> ⇒ 两支构成「先汇总取 id、后逐场拉订单」的两段式调用序。</para>
    /// <para><b>官方表类型笔误三处</b>：<c>product_infos[]</c> 的 <c>path</c> / <c>head_image</c> / <c>title</c>
    /// 类型列写 number、说明与示例均为字符串 ⇒ 本仓按语义建 <c>string?</c>（守卫 SHOPSH5 锁定，不得按表回填数值类型）。</para>
    /// <para><b>金额单位分</b>：售卖价 / 均摊价 / 运费 / 优惠 / 附加金额一律分；<c>pay_method</c> 仅示例出现、
    /// 表未列 ⇒ 两支都建模并留档。</para>
    /// <para><b>敏感面</b>：订单树含 <c>transaction_id</c> / <c>prepay_id</c> / 快递单号 / <c>sharer_openid</c>
    /// 与全部金额，禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/sharer/get_sharer_live_order_list")]
    Task<WxaShopSharerLiveOrderListResponse> GetSharerLiveOrderListAsync(
        [Body] WxaShopSharerLiveOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分享员的直播间带货数据汇总（<c>POST /shop/sharer/get_sharer_live_summary_list</c>，权限集：官方未标注）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/sharer/get_sharer_live_summary_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> / <c>page</c> / <c>page_size</c> 三支均必填），见 <see cref="WxaShopSharerLiveSummaryListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>lives</c> 列表 + <c>total_num</c>，见 <see cref="WxaShopSharerLiveSummaryListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：POST + JSON 请求体；翻页 <c>page</c> + <c>page_size</c>（≤<b>20</b>，与另两支列表的 100 不同，
    /// 全族不得共用一个上限常量）。</para>
    /// <para><b><c>live_end_time</c> 直播中恒为 0</b>（官方原文）⇒ 判直播中用 <c>live_status==1</c>，不得把 0 当 1970 年时间解析。</para>
    /// <para><b><c>live_start_time</c> 同名不同型</b>：本支 number（unix 时间戳）、订单支 <c>promotion_info</c> 内 string
    /// （「2020-04-25 12:05:25」文本）⇒ 两类型不共用（守卫 SHOPSH5 锁定）。</para>
    /// <para><b>敏感面</b>：<c>openid</c> 与各直播间金额禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/sharer/get_sharer_live_summary_list")]
    Task<WxaShopSharerLiveSummaryListResponse> GetSharerLiveSummaryListAsync(
        [Body] WxaShopSharerLiveSummaryListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置小程序分享模式（<c>POST /shop/sharer/set_share_mode</c>，权限集：官方未标注；官方文档页名为 share_attribute）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/sharer/share_attribute.html"/>。
    /// </summary>
    /// <param name="request">请求体（分享模式一支键），见 <see cref="WxaShopSharerSetShareModeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：POST + JSON 请求体；应答只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</para>
    /// <para><b>键名官方自相矛盾（裁决留档）</b>：请求示例 <c>share_mode</c> vs 参数表 <c>sharer_mode</c> ⇒
    /// 本仓照示例钉 <c>share_mode</c>（守卫 SHOPSH5 锁定），表侧判官方笔误。</para>
    /// <para><b>30 天冷却</b>：两次更改间隔不得短于 30 天，违反返回 <c>1310005</c> ⇒ 严禁放进自动重试管道。</para>
    /// <para><b>非纯配置项</b>：切到分享归因模式（值 2）后小程序对分享员不可见、微信侧停止统计展示带货数据，
    /// 直接影响本域其余数据面的可见性；<c>1</c> 为默认分享员模式。</para>
    /// <para><b>路由名与文档页名不同</b>：页面 <c>share_attribute.html</c>、路由 <c>set_share_mode</c>，
    /// 且该页另载前端 <c>wx.getChannelsShareKey()</c> 与回调事件 <c>miniprogram_sharer_action</c>（不属本域 HTTP 端点）。</para>
    /// </remarks>
    [Post("/shop/sharer/set_share_mode")]
    Task<WxaResponse> SetShareModeAsync(
        [Body] WxaShopSharerSetShareModeRequest request,
        CancellationToken cancellationToken = default);
}
