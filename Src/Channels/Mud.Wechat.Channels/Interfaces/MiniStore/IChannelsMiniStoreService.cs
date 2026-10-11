// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.MiniStore;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「连接小程序」域 SDK（10 端点：文件下载链接 / 资料上传 /
/// 礼物活动查询 / 授权送礼礼物单全链路）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域定位（设计方案 v1「连接小程序」）</b>：本域是小程序侧消费小店能力的开放面
/// （<c>/channels/ec/open/*</c>、<c>/channels/ec/b2c/*</c>、<c>/channels/ec/order/present*</c>），
/// 令牌仍按小店线统一为 <c>ChannelsTokenTypes.AccessToken</c>（官方 upload 端点原文亦接受
/// authorizer_access_token 代调用）。
/// </para>
/// <para>
/// <b>礼物单子单状态契约</b>：见 <see cref="ChannelsMiniStorePresentSubOrderStatuses"/>
/// （12 待收下 → 20/21/30 → 100 完成；180 过期 / 200 全部售后后取消）。
/// </para>
/// <para>
/// <b>祝福语契约</b>：创建礼物单时 <c>wishmessage</c> 最大 100 字符；改写祝福语
/// （<c>wishmessage/set</c>）仅可在「指定礼物收礼者」之前，且中文 ≤ 20 字 / 英文 ≤ 40 字符（6080091）。
/// </para>
/// <para>
/// <b>与 Order 域边界</b>：小店订单主链路（<c>/channels/ec/order/*</c> 下单 / 查单 / 发货等）落
/// <see cref="IChannelsOrderService"/>；本域仅承载「合作商家为小程序发放礼物」族的礼物单端点与
/// 连接小程序基础端点，两域无路由交叠。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniStore", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsMiniStoreService
{
    /// <summary>
    /// 获取文件下载链接（<c>get_download_url</c>，定制商品用户上传资料下载）。
    /// </summary>
    /// <param name="request">查询请求（<c>openid</c> / <c>order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件下载链接（<c>url</c>）与到期时间（<c>expire_time</c>，秒级时间戳，有效期 10 分钟）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/basic/api_getdownloadurl"/></para>
    /// <para>
    /// 官方业务限制：①频率限制为 <b>order_id 维度 10 次/天</b>；②<b>订单已结算或超过售后期 7 天后，
    /// 不可获取对应的文件下载链接</b>；③链接有效期 10 分钟（官方变更记录：原 download_url 字段已修正为 url）。
    /// </para>
    /// <para>官方错误码：<c>10005</c>（订单已取消，禁止下载文件）/ <c>10006</c>（订单已结束，禁止下载文件）/
    /// <c>10009</c>（下载文件次数超过日上限）等。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/open/get_download_url")]
    Task<ChannelsMiniStoreGetDownloadUrlResponse> GetDownloadUrlAsync(
        [Body] ChannelsMiniStoreGetDownloadUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传资料（<c>upload</c>，下单后传给商家进行定制商品发货）。
    /// </summary>
    /// <param name="uploadType">上传方式（官方 <c>upload_type</c>，Query 参数，见 <see cref="ChannelsMiniStoreUploadTypes"/>）——
    /// SDK 建模官方 JSON 形态（upload_type=1 文件 url 上传）；<c>upload_type=0</c> 二进制上传官方为
    /// multipart/form-data，不在本 JSON 方法语义内。</param>
    /// <param name="request">上传请求（<c>openid</c> 必填——upload_type=1 时走请求体；<c>file_url</c>
    /// 必填，支持 &lt;50M 的文件）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文件 media_id（用于后续下发商家）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/basic/api_uploadec"/></para>
    /// <para>
    /// 官方业务限制：①官方提供两种形态——upload_type=0 二进制上传（multipart/form-data，文件 &lt;10M、
    /// openid 走 Query）与 upload_type=1 URL 上传（JSON 请求体，文件 &lt;50M）；SDK 按 JSON 形态建模；
    /// ②支持格式见官方表（图片 / CAD / 3DMax / 压缩包 / 普通文件），压缩包中只能包含支持的文件类型；
    /// ③适用范围官方标注「小程序」账号类型调用。
    /// </para>
    /// <para>官方错误码：<c>10004</c>（上传文件存在风险）/ <c>10007</c>（上传文件格式不支持）/
    /// <c>10008</c>（上传文件的大小超过最大值）/ <c>10010</c>（上传 url 无效）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>180</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/open/upload")]
    Task<ChannelsMiniStoreUploadMaterialResponse> UploadMaterialAsync(
        [Query("upload_type")] int uploadType,
        [Body] ChannelsMiniStoreUploadMaterialRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询小店礼物活动列表（<c>activity/list/promoter/get</c>，授权送礼活动）。
    /// </summary>
    /// <param name="request">查询请求（<c>offset</c> / <c>limit</c> 必填，limit 最大 100）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>礼物活动列表（<c>list</c>：小店 appid / 昵称 / 活动 id / 状态 / 类型）与总数（<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_list_present_activity"/></para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；活动状态枚举见
    /// <see cref="ChannelsMiniStoreActivityStatuses"/>，类型枚举见 <see cref="ChannelsMiniStoreActivityTypes"/>。</para>
    /// <para>官方错误码：<c>40097</c>（参数错误）/ <c>10020002</c>（非法的 limit 参数，应该小于等于 100）/
    /// <c>10028004</c>（推广者账号送礼被封禁）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/b2c/activity/list/promoter/get")]
    Task<ChannelsMiniStoreGetActivityListResponse> GetPresentActivityListAsync(
        [Body] ChannelsMiniStoreGetActivityListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询礼物活动详情（<c>activity/info/promoter/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>shop_appid</c> / <c>activity_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>活动信息（<c>activity</c>：活动 id / 状态 / 基础信息（起止时间 + 小店信息）/ 礼物商品信息列表）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_get_activity"/></para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；<c>can_use_stock_num</c>（剩余库存）
    /// 仅小店授权类型活动才有，<c>present_order_id</c>（礼物订单号）仅自购类型才有。</para>
    /// <para>官方错误码：<c>40097</c>（参数错误）/ <c>10028003</c>（活动 id 不存在）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/b2c/activity/info/promoter/get")]
    Task<ChannelsMiniStoreGetActivityResponse> GetPresentActivityAsync(
        [Body] ChannelsMiniStoreGetActivityRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定礼物收礼者（<c>presentorder/receiver/set</c>，按子单指定收礼者 openid）。
    /// </summary>
    /// <param name="request">指定请求（<c>present_order_id</c> / <c>sub_order_list</c> 必填，子单不可为空）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_set_present_receiver"/></para>
    /// <para>
    /// 官方业务限制：①指定收礼者后不允许再修改祝福语（与 <c>wishmessage/set</c> 的时序契约）；
    /// ②多笔子单不可指定同样的收礼者（6080079）；③同一子单不可重复指定（6080076）。
    /// </para>
    /// <para>官方错误码：<c>6080073</c>（礼物单状态错误）/ <c>6080074</c>（当前礼物单暂不支持此操作）/
    /// <c>6080075</c>（子单号错误）/ <c>6080076</c>（当前子单已经指定过收礼者）/ <c>6080077</c>（礼物单号错误）/
    /// <c>6080078</c>（子单列表为空）/ <c>6080079</c>（多笔子单指定了同样的收礼者）/ <c>6080095</c>（外部 APP 收礼场景传参错误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/presentorder/receiver/set")]
    Task<ChannelsResponse> SetPresentReceiverAsync(
        [Body] ChannelsMiniStoreSetPresentReceiverRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建并发送礼物（<c>presentorder/create</c>，发送授权送礼商品给指定用户）。
    /// </summary>
    /// <param name="request">创建请求（<c>activity_id</c> / <c>openid</c> / <c>shop_appid</c> / <c>wishmessage</c> /
    /// <c>product_id</c> / <c>sku_id</c> 必填；祝福语最大 100 字符）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>礼物单 id（<c>present_order_id</c>）与外部应用跳转字段（business_type / query，仅外部应用身份调用返回）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_create_present_order"/></para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；送礼商品价格不得超过 2 万元（608021）；
    /// 传入幂等 id（idempotent_id）后相同幂等 id 的并发请求返回 6011104（请求处理中）。</para>
    /// <para>官方错误码：<c>108001</c>（库存不足）/ <c>108002</c>（商品状态异常）/ <c>608010</c>（平台风控）/
    /// <c>608016</c>（该商品不在礼物池）/ <c>608021</c>（商品送礼价格过高）/ <c>6011100</c>（活动类型错误）/
    /// <c>6011102</c>（祝福语长度过长）/ <c>6011103</c>（活动侧全 sku 库存不足）/ <c>6011104</c>（请求处理中）/
    /// <c>6080060</c>（送礼份数超出预设活动库存）/ <c>6080061</c>（送礼商品非预设活动商品）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/presentorder/create")]
    Task<ChannelsMiniStoreCreatePresentOrderResponse> CreatePresentOrderAsync(
        [Body] ChannelsMiniStoreCreatePresentOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询礼物订单列表（<c>presentlist/get</c>，当前小程序发放的礼物单 id 列表）。
    /// </summary>
    /// <param name="request">查询请求（<c>shop_appid</c> / <c>page_size</c> 必填；创建 / 更新时间范围至少填一个，
    /// 跨度均不可超过 7 天；游标翻页经 <c>next_key</c>，首次置空）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>礼物单列表（<c>present_order_list</c>，元素含礼物单 id）与翻页游标（<c>next_key</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_get_present_list"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 不超过 100；②时间范围（创建或更新时间二选一填写）跨度不可超过 7 天；
    /// ③官方参数表将 <c>present_order_list</c> 声明为 objarray、返回示例以对象形式（含 present_order_id）
    /// 展示——SDK 按参数表元素形态（对象）建模。
    /// </para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/presentlist/get")]
    Task<ChannelsMiniStoreGetPresentListResponse> GetPresentListAsync(
        [Body] ChannelsMiniStoreGetPresentListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询收礼者订单列表（<c>receiverorderlist/get</c>，发放给某收礼者的订单）。
    /// </summary>
    /// <param name="request">查询请求（<c>shop_appid</c> / <c>openid</c> / <c>page_size</c> 必填；时间范围约束同礼物单列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单列表（<c>order_list</c>：礼物单 id + 订单 id）与翻页游标（<c>next_key</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_get_receiver_order_list"/></para>
    /// <para>官方业务限制：①时间范围跨度不可超过 7 天；②<c>page_size</c> 不超过 100；③游标翻页（next_key）。</para>
    /// <para>官方错误码：<c>40003</c>（openid 错误）/ <c>40097</c>（请求体参数不正确）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/receiverorderlist/get")]
    Task<ChannelsMiniStoreGetReceiverOrderListResponse> GetReceiverOrderListAsync(
        [Body] ChannelsMiniStoreGetReceiverOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询礼物订单详情（<c>present/get</c>，礼物单全量信息）。
    /// </summary>
    /// <param name="request">查询请求（<c>present_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>礼物单结构（<c>present</c>：礼物单 id / 创建时间 / 价格与支付信息 / 祝福语 /
    /// 子单列表（子单状态见 <see cref="ChannelsMiniStorePresentSubOrderStatuses"/>）/ 是否无需付款 / 关联活动 id）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_get_present"/></para>
    /// <para>
    /// 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；<b>官方文档矛盾照录</b>：
    /// <c>is_b2c_free_present</c> 官方声明 boolean 类型但描述为「0：已付款 1：无需付款」——SDK 按声明
    /// 类型 boolean 建模，0/1 描述照录。
    /// </para>
    /// <para>官方错误码：<c>6080077</c>（礼物单号错误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/present/get")]
    Task<ChannelsMiniStoreGetPresentResponse> GetPresentAsync(
        [Body] ChannelsMiniStoreGetPresentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定礼物单祝福语（<c>presentorder/wishmessage/set</c>，改写礼物单祝福语）。
    /// </summary>
    /// <param name="request">改写请求（<c>present_order_id</c> / <c>wish_message</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/miniandstore/cooperation_gift/api_set_present_wishmessage"/></para>
    /// <para>
    /// 官方业务限制：①仅可在调用「指定礼物收礼者」接口<b>之前</b>修改祝福语，指定收礼者后不允许修改（6080076）；
    /// ②长度限制：纯中文最多 20 个汉字，纯英文最多 40 个字符（6080091）；③祝福语内容安全校验未通过报 6080085。
    /// </para>
    /// <para>官方错误码：<c>6080073</c>（礼物单状态错误）/ <c>6080074</c>（当前礼物单暂不支持此操作）/
    /// <c>6080076</c>（礼物子单已指定收礼人，不允许修改祝福语）/ <c>6080077</c>（礼物单号错误）/
    /// <c>6080085</c>（祝福语内容安全校验未通过）/ <c>6080091</c>（祝福语长度超过限制）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/presentorder/wishmessage/set")]
    Task<ChannelsResponse> SetPresentWishMessageAsync(
        [Body] ChannelsMiniStoreSetPresentWishMessageRequest request,
        CancellationToken cancellationToken = default);
}
