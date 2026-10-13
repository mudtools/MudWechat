// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「直播 · 直播间内商品」域 SDK（7 端点：房内商品删除 / 推送 / 上下架 / 排序 + 讲解视频 +
/// 直播挂件全局 KEY 读写；官方 livebroadcast 分区 → studio-management 的商品小节）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与商品库域的分界（官方目录二分，勿合并）</b>：本接口是「<b>某房间内</b>的商品操作」
/// （路由 <c>/wxaapi/broadcast/goods/*</c> 但全部以 <c>roomId</c> 定位），商品库侧的添加 / 更新 /
/// 审核 / 删除 / 列表见 <see cref="IWxaLiveGoodsService"/>（commodity-management 小节）。
/// 官方把 <c>deleteInRoom</c>（房内移除）与 <c>delete</c>（删商品库）分成两页两个路由，
/// 语义完全不同，是本域最易误用的一处。
/// </para>
/// <para>
/// <b>导入商品不在本域</b>：把商品库商品加入房间的动作（<c>/wxaapi/broadcast/room/addgoods</c>）
/// 归 <see cref="IWxaLiveRoomService.ImportGoodsAsync"/>（官方页面位于 studio-management 的直播间小节）。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 52）。域级取证、「适用范围字段异常」留档与 GET 参数落地裁决见
/// <see cref="IWxaLiveRoomService"/> 注释。
/// </para>
/// <para>
/// <b>调用限制</b>：五端点 10000 次/天（deleteInRoom / getVideo / push / sort），
/// <c>onsale</c> 与两支 KEY 端点官方分别标注「本接口无特殊注意事项」和「500 次/天」
/// （<c>getkey</c> / <c>setkey</c> 各 500 次/天，本域最低额度）。
/// </para>
/// <para>
/// <b>串行约束（来自官方错误码族）</b>：<c>300047</c>「已有商品正在推送，请稍后再试」与
/// <c>300049</c>「商品推送过程中不允许上下架」⇒ 同一房间的 <c>push</c> 与 <c>onsale</c> /
/// <c>deleteInRoom</c> 不得并发发起，宿主侧须按房间串行化。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Live", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaLiveRoomGoodsService
{
    /// <summary>
    /// 删除直播间商品（把商品移出房间，<c>POST /wxaapi/broadcast/goods/deleteInRoom</c>，云调用 <c>liveBroadcast.goodsDelete</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deletedoods.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>goodsId</c>；与 <see cref="PushGoodsAsync"/> / <see cref="GetGoodsVideoAsync"/> 共用 <see cref="WxaLiveRoomGoodsRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。</para>
    /// <para>官方错误码：<c>300022</c>（此房间号不存在）/ <c>300047</c>（已有商品正在推送）。</para>
    /// <para>
    /// <b>页面名照录</b>：官方文件名为 <c>api_deletedoods</c>、接口英文名 <c>deleteDoods</c>
    /// （官方原文拼写即缺 <c>g</c>），路由为 <c>goods/deleteInRoom</c> ⇒ URL 照录官方，方法名按路由语义取
    /// <see cref="DeleteInRoomAsync"/>，不与商品库侧 <see cref="IWxaLiveGoodsService.DeleteGoodsAsync"/> 混用。
    /// </para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/deleteInRoom")]
    Task<WxaResponse> DeleteInRoomAsync(
        [Body] WxaLiveRoomGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 推送商品（向观众端弹出商品，<c>POST /wxaapi/broadcast/goods/push</c>，云调用 <c>liveBroadcast.goodsPush</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_pushgoods.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>goodsId</c>，共用 <see cref="WxaLiveRoomGoodsRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。</para>
    /// <para>
    /// 官方错误码 7 条，含 <c>300052</c>（已下架的商品无法推送）/ <c>300053</c>（直播间未添加此商品）
    /// ⇒ 推送前须同时满足「已导入本房间」+「已上架」两个前置，分别对应
    /// <see cref="IWxaLiveRoomService.ImportGoodsAsync"/> 与 <see cref="OnSaleAsync"/>。
    /// </para>
    /// <para><b>独占</b>：<c>300047</c> 表明同房间推送为单飞语义，不得并发。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/push")]
    Task<WxaResponse> PushGoodsAsync(
        [Body] WxaLiveRoomGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上下架商品（<c>POST /wxaapi/broadcast/goods/onsale</c>，云调用 <c>liveBroadcast.goodsSale</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_salegoods.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>goodsId</c> + <c>onSale</c>），见 <see cref="WxaLiveOnSaleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；本接口官方标注「无特殊注意事项」（未给额度）。</para>
    /// <para>官方错误码：<c>300024</c>（商品不存在）/ <c>300048</c>（拉取商品列表失败）/ <c>300049</c>（商品推送过程中不允许上下架）。</para>
    /// <para><b>页面名与路由交叉</b>：官方文件名 <c>api_salegoods</c>、接口英文名 <c>SaleGoods</c>（首字母大写），路由 <c>goods/onsale</c> ⇒ 路由与 URL 照录。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/onsale")]
    Task<WxaResponse> OnSaleAsync(
        [Body] WxaLiveOnSaleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 直播间商品排序（<c>POST /wxaapi/broadcast/goods/sort</c>，云调用 <c>liveBroadcast.goodsSort</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_sortgoods.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>goods[]</c> 对象数组），见 <see cref="WxaLiveSortGoodsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。官方错误码：<c>300023</c>（房间状态拦截）/ <c>300048</c>（拉取商品列表失败）。</para>
    /// <para>
    /// <b>全量覆盖语义</b>：官方参数表为「商品 ID 列表」而非相邻对调 ⇒ 提交的数组即最终顺序，
    /// <b>未列出的商品不保留位次</b>，宿主须先取房内商品全集再排（与 <see cref="IWxaLiveRoomService.UpdateCommentAsync"/>
    /// 一类「局部开关」端点的语义分野）。
    /// </para>
    /// <para><b>官方示例把 <c>goodsId</c> 写成字符串</b>（<c>{"goodsId": "123"}</c>）而参数表标注 number ⇒ 按参数表建模为数值，见 <see cref="WxaLiveSortGoodsItem"/>。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/sort")]
    Task<WxaResponse> SortGoodsAsync(
        [Body] WxaLiveSortGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下载商品讲解视频（<c>POST /wxaapi/broadcast/goods/getVideo</c>，云调用 <c>liveBroadcast.goodsVideo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_downloadgoodsvideo.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>roomId</c> + <c>goodsId</c>，共用 <see cref="WxaLiveRoomGoodsRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>讲解视频链接（<c>url</c>），见 <see cref="WxaLiveGoodsVideoResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。<b>页面名与路由交叉</b>：官方文件名
    /// <c>api_downloadgoodsvideo</c>、接口英文名 <c>downloadGoodsVideo</c>，路由为 <c>goods/getVideo</c>
    /// 且返回的是<b>链接</b>而非视频流 ⇒ 走 JSON 管线，不需要二进制通道。
    /// </para>
    /// <para>官方错误码：<c>300044</c>（商品没有讲解视频）/ <c>300045</c>（讲解视频未生成）⇒ 两者皆「取不到内容」而非「调用失败」，宿主侧须区分处置。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/getVideo")]
    Task<WxaLiveGoodsVideoResponse> GetGoodsVideoAsync(
        [Body] WxaLiveRoomGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 直播挂件获取全局 KEY（<c>GET /wxaapi/broadcast/goods/getkey</c>，<b>无云调用</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getdefault_goodskey.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>vendorGoodsKey</c>（全局 KEY 数组），见 <see cref="WxaLiveGoodsKeyResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b> 且<b>无请求参数</b>（除 Query 上的 <c>access_token</c>）⇒ SDK 不声明任何入参；
    /// 调用额度 <b>500 次/天</b>。本域两支 KEY 端点均<b>无云调用</b>形态。
    /// </para>
    /// <para><b>响应字段名与请求字段名不同</b>：读回为 <c>vendorGoodsKey</c>，写入为 <c>goodsKey</c>（见 <see cref="SetGoodsKeyAsync"/>）⇒ 两 DTO 不可共用。</para>
    /// </remarks>
    [Get("/wxaapi/broadcast/goods/getkey")]
    Task<WxaLiveGoodsKeyResponse> GetGoodsKeyAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 直播挂件设置全局 KEY（<c>POST /wxaapi/broadcast/goods/setkey</c>，<b>无云调用</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_setdefault_goodskey.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>goodsKey</c> 字符串数组），见 <see cref="WxaLiveGoodsKeyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方成功示例的 <c>errmsg</c> 为空串）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>500 次/天</b>。官方注意事项原文：<b>「key 必须为字符串数组」</b>
    /// ⇒ 请求体 DTO 用 <c>List&lt;string&gt;</c>（守卫 LB4 锁定该类型，不得建数值数组）。</para>
    /// <para><b>覆盖语义</b>：全局 KEY 是账号级唯一配置，本端点为整体替换而非追加，取值口径与 <see cref="GetGoodsKeyAsync"/> 配对使用。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/setkey")]
    Task<WxaResponse> SetGoodsKeyAsync(
        [Body] WxaLiveGoodsKeyRequest request,
        CancellationToken cancellationToken = default);
}
