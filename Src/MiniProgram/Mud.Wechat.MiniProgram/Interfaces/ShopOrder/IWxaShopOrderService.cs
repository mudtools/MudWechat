// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopOrder;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · 订单本体」域 SDK（8 端点：创建 / 关闭 / 详情 / 列表 / 按推广员列表 /
/// 按分享员列表 / 获取支付参数 / 上报支付结果；官方 <c>minishopopencomponent2/API/order</c> 分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// 订单面全部以小程序自身 <c>access_token</c> 调用，与小店线 <c>/channels/ec/*</c> 令牌链无关。
/// </para>
/// <para>
/// <b>订单状态机（官方 <c>get_order</c> 枚举）</b>：<c>10</c> 待付款、<c>11</c> 收银台支付完成（自动流转，
/// 官方要求与 10 同等对待）、<c>20</c> 待发货、<c>21</c> 部分发货、<c>30</c> 待收货、<c>100</c> 完成、
/// <c>181</c> 超时未支付取消、<c>200</c> 全部商品售后取消、<c>250</c> 用户取消/商家取消；
/// <b>注意</b>：<c>add_order</c> 页的状态枚举只列到 <c>250</c> 且<b>不含 21/181</b>，
/// 以 <c>get_order</c> 页的完整枚举为准（两页口径不一致，守卫锁定）。
/// </para>
/// <para>
/// <b>金额单位一律分</b>：<c>sale_price</c> / <c>real_price</c> / <c>price_info.*</c> / <c>final_price</c> 均为分。
/// <c>real_price</c> 是「扣除优惠后单件 sku 的分摊价格」，<c>sku_real_price</c> 是「sku 总实付价」，
/// 两者不可互换；<c>data.final_price = product_price + freight - discounted_price + additional_price</c>（官方公式）。
/// </para>
/// <para>
/// <b>时间同步要求</b>：<c>create_time</c> 官方要求与微信服务器时差不得超过 <b>5 秒</b>，格式
/// <c>yyyy-MM-dd HH:mm:ss</c>；<c>expire_time</c> 是秒级时间戳，到期后官方自动流转为 <c>181</c>。
/// </para>
/// <para>
/// <b>二选一与必填陷阱</b>：关闭 / 详情 / 上报的订单定位键为 <c>order_id</c> 与 <c>out_order_id</c> <b>二选一</b>，
/// 但 <c>openid</c> <b>恒必填</b>；<c>getpaymentparams</c> 的官方参数表把 <c>order_id</c> 与 <c>out_order_id</c>
/// <b>都</b>标必填（与其它三支口径不同，按官方原文留档）；该端点仅允许 <c>fund_type=1</c> 的二级商户单
/// （否则 <c>1010047</c>），价格为 0 的订单返回 <c>1010051</c>。
/// </para>
/// <para>
/// <b>幂等与不可逆</b>：<c>add</c> 以 <c>out_order_id</c> 为商家侧主键，重复调用属覆盖/冲突语义，<b>非幂等安全</b>；
/// <c>close</c> 与 <c>pay</c> 改变订单终态且<b>不可逆</b> ⇒ 三支均<b>不得</b>放进自动重试管道。
/// 列表三支（<c>get_list</c> / <c>get_list_by_finder</c> / <c>get_list_by_sharer</c>）只读，可安全重试。
/// </para>
/// <para>
/// <b>官方端点级频率数字空缺</b>：本域 8 页正文均未给「次/分钟」类频率数字，也未给权限集编号与云调用说明
/// ⇒ 注释留档该空缺，不得凭第三方 SDK 注释或记忆补数；「<c>page_size</c> 上限 100」是规模限制，与频率分列。
/// </para>
/// <para>
/// <b>应答包装不一致（官方原文自相矛盾）</b>：<c>get</c> 页「返回参数说明」写 <c>data</c>，同页回包示例却用
/// <c>order</c>；<c>get_list</c> 页参数表写 <c>orders[]</c>，示例却用 <c>data</c> ⇒ 两个响应类对这些键
/// <b>同时建模</b>并逐键留档，宿主按真实回包取值。
/// </para>
/// <para>
/// <b>敏感面（AGENTS §8）</b>：<c>openid</c> / <c>out_user_id</c> / <c>promoter_openid</c> / <c>sharer_openid</c>、
/// 收货地址（<c>receiver_name</c> / <c>tel_number</c> / <c>detailed_address</c>）、<c>prepay_id</c> /
/// <c>transaction_id</c>、快递单号与佣金/退款金额属个人敏感信息或个人金融信息
/// ⇒ <b>一律不得写入日志、遥测或异常消息</b>。
/// </para>
/// <para>
/// <b>与小店线零交叉</b>：本域 8 条全部是 <c>/shop/order/*</c> 小程序令牌面，与小店线 <c>/channels/ec/*</c>
/// 无交叠；本域<b>不含</b> V1 <c>/product/order/*</c> 面（V1 族另域承载）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopOrder", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopOrderService
{
    /// <summary>
    /// 创建订单（<c>POST /shop/order/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/add_order.html"/>。
    /// </summary>
    /// <param name="request">订单请求体，见 <see cref="WxaShopOrderAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>回包 <c>data</c> 层的订单结果（<c>order_id</c> / <c>ticket</c> / <c>final_price</c>），见 <see cref="WxaShopOrderAddResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体（平铺顶层，无包装键）；官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b><c>out_order_id</c> 是商家侧主键</b>：普通场景是外部订单 ID，合单支付场景是主外部订单 ID；
    /// 长度须小于 128 字符，重复提交同一 <c>out_order_id</c> 并非幂等重放语义 ⇒ <b>不得</b>放进自动重试管道。
    /// </para>
    /// <para>
    /// <b>必填集</b>：<c>create_time</c>（与微信服务器时差 ≤ 5 秒）、<c>out_order_id</c>、<c>openid</c>、<c>path</c>、
    /// <c>scene</c>、<c>order_detail</c>；<c>order_detail</c> 内 <c>product_infos[]</c> 八支、<c>pay_info.pay_method_type</c>、
    /// <c>price_info.order_price</c> 与 <c>freight</c> 均必填；<c>delivery_detail.delivery_type</c> 默认 1。
    /// <c>pay_method_type=0</c>（微信支付）时 <c>prepay_id</c> 与 <c>prepay_time</c> 必填。
    /// </para>
    /// <para>
    /// <b>回包 <c>data</c> 包装</b>：官方示例为 <c>{errcode, errmsg, data:{order_id, out_order_id, ticket, ticket_expire_time, final_price}}</c>
    /// ⇒ 订单结果不在顶层；<c>ticket</c> 是拉起收银台的敏感凭据。
    /// </para>
    /// </remarks>
    [Post("/shop/order/add")]
    Task<WxaShopOrderAddResponse> AddOrderAsync(
        [Body] WxaShopOrderAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭订单（<c>POST /shop/order/close</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/order_close.html"/>。
    /// </summary>
    /// <param name="request">订单定位键 + <c>openid</c>，见 <see cref="WxaShopOrderCloseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体 <c>{order_id | out_order_id, openid}</c>；两键<b>二选一</b>、<c>openid</c> 必填。
    /// 本域官方无端点级频率数字。
    /// </para>
    /// <para><b>状态不可逆</b>：关闭后订单进入取消终态，官方本域无「重新打开」端点 ⇒ 非幂等、不得自动重试。</para>
    /// </remarks>
    [Post("/shop/order/close")]
    Task<WxaResponse> CloseOrderAsync(
        [Body] WxaShopOrderCloseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单详情（<c>POST /shop/order/get</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/>。
    /// </summary>
    /// <param name="request">订单定位键 + <c>openid</c>，见 <see cref="WxaShopOrderGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单信息，见 <see cref="WxaShopOrderGetResponse"/>（<c>order</c> 与 <c>data</c> 两键均建模）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>order_id</c> 与 <c>out_order_id</c> 二选一、<c>openid</c> 必填；
    /// 本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>应答键名官方自相矛盾</b>：返回参数说明表写 <c>data | Order</c>，同页回包示例写 <c>order</c> ⇒
    /// 响应类同留两键；订单树另含 <c>default_receiving_address</c>（默认退货地址）、<c>settlement_info</c>、
    /// <c>refund_info</c>、<c>related_aftersale_info</c>、<c>fund_type</c>（0 普通单 / 1 二级商户单）等。
    /// </para>
    /// <para><b>只读</b>：可安全重试。</para>
    /// </remarks>
    [Post("/shop/order/get")]
    Task<WxaShopOrderGetResponse> GetOrderAsync(
        [Body] WxaShopOrderGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单列表（<c>POST /shop/order/get_list</c>，页码式分页）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order_list.html"/>。
    /// </summary>
    /// <param name="request">分页与时间范围，见 <see cref="WxaShopOrderGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单数组 + 总数，见 <see cref="WxaShopOrderGetListResponse"/>（<c>orders</c> 与 <c>data</c> 两键均建模）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>page</c> 最小 1、<c>page_size</c> 上限 <b>100</b>，
    /// <c>sort_order</c> 必填（1 倒序 / 2 正序）；本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>应答键名官方自相矛盾</b>：返回参数表写 <c>orders[]</c>，回包示例写 <c>data</c> ⇒ 响应类继承
    /// <see cref="WxaShopOrderListResponse.Orders"/> 之外额外保留 <c>data</c> 键。
    /// </para>
    /// <para><b>只读</b>：可安全重试；<c>start_create_time</c> / <c>end_create_time</c> 为 <c>yyyy-MM-dd HH:mm:ss</c>。</para>
    /// </remarks>
    [Post("/shop/order/get_list")]
    Task<WxaShopOrderGetListResponse> GetOrderListAsync(
        [Body] WxaShopOrderGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按推广员获取订单列表（<c>POST /shop/order/get_list_by_finder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order_list_by_finder.html"/>。
    /// </summary>
    /// <param name="request">推广员标识 + 时间范围 + 分页，见 <see cref="WxaShopOrderGetListByFinderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>orders[]</c> + <c>total_num</c>，见 <see cref="WxaShopOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>promoter_id</c> 与 <c>promoter_openid</c> <b>二选一</b>，
    /// <c>page</c> 最小 1、<c>page_size</c> 上限 <b>100</b>；本域官方无端点级频率数字。
    /// </para>
    /// <para><b>结算口径</b>：按<b>支付时间</b>（<c>start_pay_time</c> / <c>end_pay_time</c>）筛选，不是创建时间；
    /// 回包 <c>promotion_info</c> 含 <c>finder_nickname</c> / <c>promoter_openid</c>。</para>
    /// <para><b>只读</b>：可安全重试。</para>
    /// </remarks>
    [Post("/shop/order/get_list_by_finder")]
    Task<WxaShopOrderListResponse> GetOrderListByFinderAsync(
        [Body] WxaShopOrderGetListByFinderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按分享员获取订单列表（<c>POST /shop/order/get_list_by_sharer</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order_list_by_sharer.html"/>。
    /// </summary>
    /// <param name="request">分享员 <c>openid</c> + 时间范围 + 分页，见 <see cref="WxaShopOrderGetListBySharerRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>orders[]</c> + <c>total_num</c>，见 <see cref="WxaShopOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>sharer_openid</c> <b>必填</b>，<c>page</c> 最小 1、
    /// <c>page_size</c> 上限 <b>100</b>；本域官方无端点级频率数字。
    /// </para>
    /// <para><b>结算口径</b>：同样按<b>支付时间</b>筛选；回包 <c>promotion_info</c> 含 <c>live_start_time</c>（直播间开始时间）。</para>
    /// <para><b>只读</b>：可安全重试。</para>
    /// </remarks>
    [Post("/shop/order/get_list_by_sharer")]
    Task<WxaShopOrderListResponse> GetOrderListBySharerAsync(
        [Body] WxaShopOrderGetListBySharerRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单支付参数（<c>POST /shop/order/getpaymentparams</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/getpaymentparams.html"/>。
    /// </summary>
    /// <param name="request">订单定位键 + <c>openid</c>，见 <see cref="WxaShopOrderPaymentParamsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>payment_params</c>（<c>timeStamp</c> / <c>nonceStr</c> / <c>package</c> / <c>paySign</c> / <c>signType</c>），见 <see cref="WxaShopPaymentParamsResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；参数表把 <c>order_id</c>、<c>out_order_id</c>、<c>openid</c>
    /// <b>三支全标必填</b>（与同域其它订单定位端点的「二选一」口径不同，按官方原文留档）；
    /// 本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>准入与错误码</b>：仅 <c>fund_type=1</c> 的二级商户单允许调用（否则 <c>1010047</c>）；
    /// 订单不存在 <c>1010011</c>、状态不允许付款 <c>1010043</c>、请求支付失败 <c>1010044</c>、
    /// 计算参数失败 <c>1010045</c>、价格为 0 <c>1010051</c>、无权限 <c>48001</c>、系统异常 <c>-1</c>。
    /// </para>
    /// <para>
    /// <b>返回敏感凭据</b>：<c>package</c>（<c>prepay_id=***</c>）与 <c>paySign</c> 是拉起支付的签名材料
    /// ⇒ 不得进日志、遥测或异常消息。
    /// </para>
    /// <para><b>只读</b>：可安全重试；但每次调用会生成新的支付参数，宿主应按需缓存并校验 <c>expire_time</c>。</para>
    /// </remarks>
    [Post("/shop/order/getpaymentparams")]
    Task<WxaShopPaymentParamsResponse> GetPaymentParamsAsync(
        [Body] WxaShopOrderPaymentParamsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报支付结果（<c>POST /shop/order/pay</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/pay_order.html"/>。
    /// </summary>
    /// <param name="request">支付结果上报体，见 <see cref="WxaShopOrderPayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>order_id</c> 与 <c>out_order_id</c> <b>二选一</b>，
    /// <c>openid</c> 与 <c>action_type</c> 必填；<c>action_type</c> 取值
    /// <c>1</c> 支付成功 / <c>2</c> 支付失败 / <c>3</c> 用户取消 / <c>4</c> 超时未支付 / <c>5</c> 商家取消 /
    /// <c>10</c> 其他原因取消（默认 1）。官方本域无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>条件必填</b>：<c>action_type=1</c> 且下单时 <c>pay_method_type=0</c> ⇒ <c>transaction_id</c> 必填；
    /// <c>action_type=1</c> ⇒ <c>pay_time</c> 必填；<c>action_type=10</c> 建议传 <c>action_remark</c>。
    /// </para>
    /// <para>
    /// <b>状态不可逆</b>：本端点把订单推进到支付成功/取消终态，官方无回退端点 ⇒ 非幂等、
    /// <b>不得</b>放进自动重试管道；<c>transaction_id</c> 为敏感凭据，不得进日志。
    /// </para>
    /// </remarks>
    [Post("/shop/order/pay")]
    Task<WxaResponse> PayOrderAsync(
        [Body] WxaShopOrderPayRequest request,
        CancellationToken cancellationToken = default);
}
