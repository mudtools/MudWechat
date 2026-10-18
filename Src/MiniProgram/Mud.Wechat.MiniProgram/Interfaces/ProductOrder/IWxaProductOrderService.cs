// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ProductOrder;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」·订单域（10 端点：订单详情 / 列表 / 搜索 /
/// 改价 / 改商家备注 / 售后单查询（单条 + 批量）/ 受理 / 拒绝退款 / 上传退款凭证；
/// 路由族 <c>/product/order/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域是 V1 面，与 V2 <c>/shop/order/*</c> 是两代契约</b>：本域订单状态值域为
/// <see cref="WxaProductOrderStatuses"/>（10/15/16/17/20/21/30/100/181/190/200/250），
/// V2 面见 <see cref="IWxaShopOrderService"/>；两族路由、状态枚举与售后单号语义<b>不得</b>互抄。
/// </para>
/// <para>
/// <b>售后三支的 <c>order_id</c> 是售后单号而非订单号（官方反直觉点，照录）</b>：
/// <c>acceptapply</c> / <c>rejectrefund</c> / <c>upload_refund_certificate</c> 三页参数表中
/// <c>order_id</c> 的说明均为「售后单号」，官方三页均只给请求示例、<b>无</b>参数表与字段说明。
/// </para>
/// <para>
/// <b>两套售后单并存</b>：<c>getaftersaleorder</c> / <c>batchgetaftersaleorder</c> 读到的
/// <see cref="WxaProductAfterSaleOrder"/> 是「退款 / 退货退款」工单（状态为
/// <c>MERCHANT_REFUND_SUCCESS</c> 一类<b>符号字符串</b>），与本域「基础交易工具售后」接口层
/// 的 <c>/shop/aftersale/*</c> 数值状态机不同源。
/// </para>
/// <para>
/// <b>官方两页 URL 互换（诚实留档）</b>：<c>getaftersaleorder.html</c> 正文写的请求地址是
/// <c>/product/order/batchgetaftersaleorder</c>，而 <c>batchgetaftersaleorder.html</c> 正文写的是
/// <c>/product/order/getaftersaleorder</c> ⇒ 本接口以<b>路由语义</b>为准（单条 → <c>getaftersaleorder</c>、
/// 批量 → <c>batchgetaftersaleorder</c>），守卫 SHPO-V1O2 锁定该取舍。
/// </para>
/// <para>
/// <b>金额单位一律为分</b>；<c>create_time</c> / <c>update_time</c> 为 <c>yyyy-MM-dd HH:mm:ss</c>
/// （与本仓 V2 域毫秒字符串口径不同）。
/// </para>
/// <para>
/// <b>个人信息红线</b>：订单含 <c>openid</c>、收货地址、手机号与昵称 ⇒ 响应数据禁入日志、遥测与异常消息。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：本域 10 页官方正文<b>未给</b>端点级频率数字、权限集编号与云调用方法名
/// （2026-10-18 逐页取证）⇒ 不得凭第三方 SDK 注释或记忆补数。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductOrder", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductOrderService
{
    /// <summary>
    /// 受理退款申请（<c>POST /product/order/acceptapply</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/acceptapply.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> 为<b>售后单号</b>、<c>address_id</c> 为退货地址），见 <see cref="WxaProductOrderAcceptApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方仅给示例、无参数表</b>：字段语义按示例键推断 —— <c>order_id</c> 在此接口指<b>售后单号</b>
    /// （不是订单号），<c>address_id</c> 为退货收货地址簿 id。
    /// </para>
    /// <para>
    /// <b>不可逆</b>：受理后进入退款 / 退货流程，撤销需走平台售后协商，本接口<b>无</b>反向操作
    /// ⇒ 不得自动重试，重复调用返回错误。
    /// </para>
    /// </remarks>
    [Post("/product/order/acceptapply")]
    Task<WxaResponse> AcceptRefundApplyAsync(
        [Body] WxaProductOrderAcceptApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取售后单（<c>POST /product/order/batchgetaftersaleorder</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/getaftersaleorder.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>after_sale_order_id_list</c>，官方上限 50 条），见 <see cref="WxaProductOrderBatchGetAfterSaleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>after_sale_order_list[]</c>，见 <see cref="WxaProductOrderBatchGetAfterSaleResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方 URL 与页名互换（照录）</b>：本路由的官方文档正文出现在 <c>getaftersaleorder.html</c> 页
    /// ⇒ 出处链指向该页，但请求地址以本路由为准。
    /// </para>
    /// <para>
    /// <b>批量上限 50</b>：超出即参数错误，宿主须自行分片；缺号 / 无权限的售后单<b>静默缺席</b>，
    /// 回包不保证与请求一一对应。
    /// </para>
    /// </remarks>
    [Post("/product/order/batchgetaftersaleorder")]
    Task<WxaProductOrderBatchGetAfterSaleResponse> BatchGetAfterSaleOrderAsync(
        [Body] WxaProductOrderBatchGetAfterSaleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改商家备注（<c>POST /product/order/change_merchant_notes</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/change_merchant_notes.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> + <c>merchant_notes</c>），见 <see cref="WxaProductOrderChangeMerchantNotesRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>只影响商家侧可见字段</b>：<c>merchant_notes</c> 为全量覆盖（传空串即清空），
    /// 用户侧不可见，也不进入订单状态机 ⇒ 可安全重试。
    /// </para>
    /// </remarks>
    [Post("/product/order/change_merchant_notes")]
    Task<WxaResponse> ChangeMerchantNotesAsync(
        [Body] WxaProductOrderChangeMerchantNotesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改订单价格（<c>POST /product/order/change_order_price</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/change_order_price.html"/>。
    /// </summary>
    /// <param name="request">请求体（订单定位 + 运费变更 + 商品改价明细），见 <see cref="WxaProductOrderChangePriceRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>仅待付款订单可改价</b>（官方限制）：<c>status</c> 已越过待付款即失败；金额单位分，
    /// 改价明细 <c>change_order_infos[]</c> 按 <c>sku_id</c> 逐项给出新价。
    /// </para>
    /// <para>
    /// <b>改价是「设置」不是「加减」</b>：重复提交同一价格集为幂等，但并发改价以最后写入者胜
    /// ⇒ 需要确定性时串行。
    /// </para>
    /// </remarks>
    [Post("/product/order/change_order_price")]
    Task<WxaResponse> ChangeOrderPriceAsync(
        [Body] WxaProductOrderChangePriceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单详情（<c>POST /product/order/get</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/get_order_detail.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> 必填），见 <see cref="WxaProductOrderGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>order</c> 单对象，见 <see cref="WxaProductOrderGetResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>单对象回包</b>：与 <c>get_list</c> / <c>search</c> 的列表回包不同源 ⇒ 不共用响应类型。
    /// </para>
    /// <para>
    /// <b>官方字段表与示例冲突一处（照录）</b>：<c>delivery_info.express_fee</c> 在参数表写作
    /// 「Object」、示例写作数组 ⇒ 本仓按示例取 <see cref="WxaProductOrderDeliveryInfo"/> 中的数组
    /// <c>express_fee</c>，守卫锁定该取舍。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c>、收货地址、手机号禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/product/order/get")]
    Task<WxaProductOrderGetResponse> GetOrderAsync(
        [Body] WxaProductOrderGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订单列表（<c>POST /product/order/get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/get_order_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（时间窗口二选一 + 状态 + 翻页），见 <see cref="WxaProductOrderGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>orders[]</c> + <c>total_num</c>，见 <see cref="WxaProductOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>时间窗口强制二选一</b>：<c>start_create_time</c>/<c>end_create_time</c> 与
    /// <c>start_update_time</c>/<c>end_update_time</c> 至少填一对，格式 <c>yyyy-MM-dd HH:mm:ss</c>；
    /// 扫描过多返回 <c>-31042</c> / <c>-31043</c>，宿主应缩短区间分段拉取。
    /// </para>
    /// <para>
    /// <b>翻页上限 10,000</b>（搜索页为 1,000）⇒ 两页不得共用同一上限口径；
    /// <c>status</c> 为<b>必填</b>单状态，不是状态数组。
    /// </para>
    /// </remarks>
    [Post("/product/order/get_list")]
    Task<WxaProductOrderListResponse> GetOrderListAsync(
        [Body] WxaProductOrderGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取售后单详情（<c>POST /product/order/getaftersaleorder</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/batchgetaftersaleorder.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>after_sale_order_id</c>），见 <see cref="WxaProductOrderGetAfterSaleRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>after_sale_order</c> 单对象，见 <see cref="WxaProductOrderGetAfterSaleResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方 URL 与页名互换（照录）</b>：本路由的官方文档正文出现在 <c>batchgetaftersaleorder.html</c> 页
    /// ⇒ 出处链指向该页，但请求地址以本路由为准。
    /// </para>
    /// <para>
    /// <b>状态是符号字符串</b>：<c>after_sale_order.status</c> 形如 <c>"MERCHANT_REFUND_SUCCESS"</c>，
    /// 官方同页又给出一套数值枚举（0~19）⇒ 两个字段并存、不得强转。
    /// </para>
    /// </remarks>
    [Post("/product/order/getaftersaleorder")]
    Task<WxaProductOrderGetAfterSaleResponse> GetAfterSaleOrderAsync(
        [Body] WxaProductOrderGetAfterSaleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拒绝退款（<c>POST /product/order/rejectrefund</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/rejectrefund.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> 为<b>售后单号</b> + <c>reject_reason</c>），见 <see cref="WxaProductOrderRejectRefundRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方仅给示例、无参数表</b>：字段语义按示例键推断 —— <c>order_id</c> 指<b>售后单号</b>。
    /// </para>
    /// <para>
    /// <b>拒绝理由会对用户可见</b>：<c>reject_reason</c> 进入用户侧售后详情，措辞由商家自拟；
    /// 拒绝后用户可申请平台介入，本接口无反向操作 ⇒ 不得自动重试。
    /// </para>
    /// </remarks>
    [Post("/product/order/rejectrefund")]
    Task<WxaResponse> RejectRefundAsync(
        [Body] WxaProductOrderRejectRefundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索订单（<c>POST /product/order/search</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/search_order.html"/>。
    /// </summary>
    /// <param name="request">请求体（支付时间窗口 / 商品标题 / 用户信息 / 状态与来源等复合过滤），见 <see cref="WxaProductOrderSearchRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>orders[]</c> + <c>total_num</c>，见 <see cref="WxaProductOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>默认只搜已支付订单</b>：<c>status</c> 缺省时官方按「已支付」过滤，要搜未支付须显式传值；
    /// <c>source</c> 取 0 表示不限来源（1 小商店 / 2 CPS 带货）。
    /// </para>
    /// <para>
    /// <b>翻页上限 1,000</b>（列表页为 10,000）；官方提醒 <c>title</c> / <c>sku_code</c> 等
    /// 模糊匹配面较宽，深翻页成本高，建议叠加时间窗口。
    /// </para>
    /// </remarks>
    [Post("/product/order/search")]
    Task<WxaProductOrderListResponse> SearchOrderAsync(
        [Body] WxaProductOrderSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传退款凭证（<c>POST /product/order/upload_refund_certificate</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/order/upload_refund_certificate.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> 为<b>售后单号</b> + 已上传素材的 <c>refund_certificates</c> + 说明），见 <see cref="WxaProductOrderUploadRefundCertificateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方仅给示例、无参数表</b>：字段语义按示例键推断 —— <c>order_id</c> 指<b>售后单号</b>，
    /// <c>refund_certificates</c> 为<b>已上传素材的 media_id 列表</b>（媒体上传走 V1 素材接口，
    /// 本接口只做绑定）。
    /// </para>
    /// <para>
    /// <b>覆盖语义</b>：重复提交按<b>全量替换</b>凭证集处理 ⇒ 追加凭证须携带旧集合并集。
    /// </para>
    /// </remarks>
    [Post("/product/order/upload_refund_certificate")]
    Task<WxaResponse> UploadRefundCertificateAsync(
        [Body] WxaProductOrderUploadRefundCertificateRequest request,
        CancellationToken cancellationToken = default);
}
