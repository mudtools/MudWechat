// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「直播 · 商品库管理」域 SDK（7 端点：添加并提审 / 更新 / 重新提审 / 撤回审核 / 删除 +
/// 商品列表 + 商品信息审核状态；官方 livebroadcast 分区 → commodity-management，外加一支
/// <c>/wxa/business/</c> 路由）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与房间内商品域的分界</b>：本接口是「<b>商品库</b>（账号级）生命周期」，房间内的删除 / 推送 /
/// 上下架 / 排序 / 讲解视频见 <see cref="IWxaLiveRoomGoodsService"/>。官方把「从房间移除」
/// （<c>goods/deleteInRoom</c>）与「删除商品」 （<c>goods/delete</c>）分作两页两路由，
/// 后者不可逆且会让已导入的房间报 <c>300024</c>。
/// </para>
/// <para>
/// <b>必须保存商品 ID 与审核单 ID（官方原文强调）</b>：<c>add</c> 返回 <c>goodsId</c> + <c>auditId</c>，
/// 官方注意事项原文「开发者必须保存【商品ID】与【审核单ID】，如果丢失，则无法调用其他相关接口」
/// ⇒ <c>auditId</c> 是 <see cref="ResetAuditAsync"/> 的必填定位键，宿主侧须落库。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 52）。域级取证、「适用范围字段异常」留档与 GET 参数落地裁决见
/// <see cref="IWxaLiveRoomService"/> 注释。
/// </para>
/// <para>
/// <b>调用限制</b>：<c>add</c> 与 <c>audit</c>（重新提审）<b>共用 500 次/天</b>（官方注意事项逐页写明），
/// <c>resetaudit</c> 500 次/天，<c>delete</c> 与 <c>getapproved</c> 各 10000 次/天，
/// <c>update</c> 与 <c>getgoodswarehouse</c> 官方标注「本接口无特殊注意事项」。
/// </para>
/// <para>
/// <b>审核前置（错误码族）</b>：<c>300010</c>（商品审核状态不对）/ <c>300011</c>（操作非法，
/// API 不允许操作非 API 创建的商品）⇒ 直播控制台添加的商品不能由本域端点改删；
/// 商品来源判别字段为 <c>third_party_tag</c>，见 <see cref="WxaLiveApprovedGoodsItem"/>。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Live", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaLiveGoodsService
{
    /// <summary>
    /// 添加并提审商品（<c>POST /wxaapi/broadcast/goods/add</c>，云调用 <c>liveBroadcast.goodsAdd</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_addgoods.html"/>。
    /// </summary>
    /// <param name="request">请求体（唯一字段 <c>goodsInfo</c> 对象），见 <see cref="WxaLiveAddGoodsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品 ID 与审核单 ID，见 <see cref="WxaLiveAddGoodsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ <c>goodsInfo</c> 包装对象；调用额度 <b>500 次/天</b>（与 <see cref="ResubmitAuditAsync"/> 共用）。</para>
    /// <para>
    /// <b>图片字段填 mediaID 且三天内有效</b>（<c>coverImgUrl</c>，尺寸上限 300×300 像素）；
    /// 名称最长 14 个汉字（1 汉字按 2 字符计）；<c>priceType</c> 为 2 或 3 时 <c>price2</c> 必填，
    /// 见 <see cref="WxaLivePriceType"/>。
    /// </para>
    /// <para>官方错误码 9 条，含 <c>300021</c>（商品添加成功，审核失败）⇒ 该码是「已建档但未过审」而非失败，不得回滚本地记录。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/add")]
    Task<WxaLiveAddGoodsResponse> AddGoodsAsync(
        [Body] WxaLiveAddGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新商品（<c>POST /wxaapi/broadcast/goods/update</c>，云调用 <c>liveBroadcast.goodsUpdate</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_updategoodsinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>goodsInfo</c> 对象，内含 <c>goodsId</c>），见 <see cref="WxaLiveUpdateGoodsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；本接口官方标注「无特殊注意事项」。<b>部分更新语义（官方示例原文）</b>：
    /// 「需要更新哪个字段就传入哪个字段，goodsId 必传」⇒ 与 <c>add</c> 的 <c>goodsInfo</c> 差一个定位键，
    /// 故两 DTO 分立（<see cref="WxaLiveGoodsInfo"/> / <see cref="WxaLiveUpdateGoodsInfo"/>）。
    /// </para>
    /// <para>官方错误码：<c>300010</c>（商品审核状态不对，如审核中）/ <c>300011</c>（非 API 创建的商品不可操作）。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/update")]
    Task<WxaResponse> UpdateGoodsAsync(
        [Body] WxaLiveUpdateGoodsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重新提交商品审核（<c>POST /wxaapi/broadcast/goods/audit</c>，云调用 <c>liveBroadcast.goodsAudit</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_resubmitaudit.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>goodsId</c>；与 <see cref="DeleteGoodsAsync"/> 共用 <see cref="WxaLiveGoodsIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新审核单 ID（<c>auditId</c>），见 <see cref="WxaLiveResubmitAuditResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；调用额度 <b>500 次/天</b>（官方注意事项原文：与「添加并提审商品」接口共用 500 次限制）。
    /// <b>页面名与路由交叉</b>：官方文件名 <c>api_resubmitaudit</c>、接口英文名 <c>resubmitAudit</c>，
    /// 路由为 <c>goods/audit</c>、云调用为 <c>liveBroadcast.goodsAudit</c> ⇒ 路由照录，方法名取
    /// <see cref="ResubmitAuditAsync"/> 表达官方标题语义。
    /// </para>
    /// <para>官方错误码：<c>300010</c>（审核中不可重复提审）/ <c>300011</c>（非 API 商品）。</para>
    /// <para><b>返回新的 <c>auditId</c></b>：每次提审生成新审核单，旧的 <c>auditId</c> 失效 ⇒ 须覆盖本地记录。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/audit")]
    Task<WxaLiveResubmitAuditResponse> ResubmitAuditAsync(
        [Body] WxaLiveGoodsIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤回商品审核（<c>POST /wxaapi/broadcast/goods/resetaudit</c>，云调用 <c>liveBroadcast.goodsResetAudit</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_resetaudit.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>goodsId</c> + <c>auditId</c>），见 <see cref="WxaLiveResetAuditRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>500 次/天</b>。</para>
    /// <para>官方错误码：<c>300009</c>（商品审核撤回失败）/ <c>300017</c>（商品未提审）。</para>
    /// <para><b>本域唯一双定位键端点</b>：必须同时握有商品 ID 与审核单 ID ⇒ 呼应官方「丢失即无法调用其他相关接口」的警示。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/resetaudit")]
    Task<WxaResponse> ResetAuditAsync(
        [Body] WxaLiveResetAuditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除商品（从商品库删除，<c>POST /wxaapi/broadcast/goods/delete</c>，<b>无云调用</b>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_deletegoodsinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>goodsId</c>，共用 <see cref="WxaLiveGoodsIdRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；调用额度 <b>10000 次/天</b>。官方错误码表只列 <c>-1</c>（系统繁忙）。</para>
    /// <para>
    /// <b>破坏性 + 跨房间影响</b>：删的是商品库记录，已导入房间的商品随之失效（<c>300024</c>）；
    /// 只想让某房间不再展示时应改用 <see cref="IWxaLiveRoomGoodsService.DeleteInRoomAsync"/>。
    /// </para>
    /// </remarks>
    [Post("/wxaapi/broadcast/goods/delete")]
    Task<WxaResponse> DeleteGoodsAsync(
        [Body] WxaLiveGoodsIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品列表（<c>GET /wxaapi/broadcast/goods/getapproved</c>，云调用 <c>liveBroadcast.goodsList</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsinfo.html"/>。
    /// </summary>
    /// <param name="offset">分页起点（<c>offset</c>，官方<b>必填</b>）。</param>
    /// <param name="status">商品状态（<c>status</c>，官方<b>必填</b>），取值见 <see cref="WxaLiveGoodsAuditStatus"/>。</param>
    /// <param name="limit">分页大小（<c>limit</c>，选填，默认 30，不超过 100）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品列表 + <c>total</c>，见 <see cref="WxaLiveGoodsListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，三个业务参数写在<b>请求地址行</b>（<c>?access_token=…&amp;offset=…&amp;limit=…&amp;status=…</c>）
    /// ⇒ 与 <see cref="IWxaLiveRoomService.GetPushUrlAsync"/> 的「GET 但参数列表在请求体表」形态不同，本端点无歧义；
    /// 调用额度 <b>10000 次/天</b>。
    /// </para>
    /// <para>
    /// <b>页面名与路由交叉</b>：官方文件名 <c>api_getgoodsinfo</c>、接口英文名 <c>getGoodsInfo</c>，
    /// 路由为 <c>goods/getapproved</c> 且按 <c>status</c> 过滤（并非只回审核通过项）⇒ 方法名取
    /// <see cref="GetApprovedGoodsAsync"/> 表达路由，标题语义在注释留档。
    /// </para>
    /// <para><b>列表元素 <c>goodsId</c> 官方标注 string</b>（与写端点的 number 不同）⇒ 见 <see cref="WxaLiveApprovedGoodsItem"/>。</para>
    /// </remarks>
    [Get("/wxaapi/broadcast/goods/getapproved")]
    Task<WxaLiveGoodsListResponse> GetApprovedGoodsAsync(
        [Query("offset")] long offset,
        [Query("status")] long status,
        [Query("limit")] long? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品的信息与审核状态（<c>POST /wxa/business/getgoodswarehouse</c>，云调用 <c>liveBroadcast.goodsInfo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsauditinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>goods_ids</c> numarray，官方必填），见 <see cref="WxaLiveGoodsWarehouseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品列表（含 <c>audit_status</c>）+ <c>total</c>，见 <see cref="WxaLiveGoodsWarehouseResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> ＋ JSON 请求体，路由在 <c>/wxa/business/</c>（本域唯一的 <c>/wxa/business/</c>
    /// 端点，另三支见 <see cref="IWxaLiveRoomService.GetLiveInfoAsync"/> 与 <see cref="IWxaLiveSubscribeService"/>）；
    /// 本接口官方标注「无特殊注意事项」。
    /// </para>
    /// <para>
    /// <b>页面名与路由交叉 + 拼写陷阱</b>：官方文件名 <c>api_getgoodsauditinfo</c>（接口英文名
    /// <c>getGoodsAuditInfo</c>），路由为 <c>getgoodswarehouse</c>；仓库内既有的零声明守卫禁止的是
    /// <b>误拼形态</b> <c>/wxaapi/broadcast/getgoodswarehouse</c>，与本路由不同 ⇒ 见守卫 LB1 与
    /// <c>MiniProgramP3RulingsContractGuards</c> 的 broadcast 条款。
    /// </para>
    /// <para><b>本域唯一 snake_case 响应元素族</b>：<c>goods_id</c> / <c>cover_img_url</c> / <c>audit_status</c> 与同页的 <c>priceType</c> / <c>thirdPartyAppid</c> 混排，照官方原文命名，见 <see cref="WxaLiveWarehouseGoodsItem"/>。</para>
    /// <para><b><c>audit_status</c> 官方类型列为 string</b>（<c>getapproved</c> 的 <c>status</c> 入参却是 number）⇒ 类型差异照录，不做归一。</para>
    /// </remarks>
    [Post("/wxa/business/getgoodswarehouse")]
    Task<WxaLiveGoodsWarehouseResponse> GetGoodsWarehouseAsync(
        [Body] WxaLiveGoodsWarehouseRequest request,
        CancellationToken cancellationToken = default);
}
