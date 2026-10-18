// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · V2 售后单」域接口（11 端点：创建 / 取消 / 详情 / 列表 / 更新 /
/// 同意退款 / 同意退货 / 拒绝 / 上传协商凭证 / 上传退货物流 / 超时模拟；
/// 官方 <c>minishopopencomponent2/API/aftersale</c> 分区的 <c>ecaftersale/*</c> 族）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 V1 售后的硬边界</b>：本族是视频号小程序自定义交易组件的售后面，单商品
/// <c>product_info</c>、微信侧 <c>aftersale_id</c>、金额字段叫 <c>orderamt</c>、时间为<b>毫秒字符串</b>、
/// 状态值域 1~25（含平台介入）⇒ 与 <see cref="IWxaShopAftersaleService"/> 的 V1 四支
/// <b>路由、字段、类型、状态机全不同</b>，守卫 SHPO-A2 锁定不得互抄。
/// </para>
/// <para>
/// <b>状态机（官方例外值域）</b>：<c>1</c> 用户取消 / <c>2</c> 商家处理退款中 / <c>4</c> 商家拒绝退款 /
/// <c>5</c> 商家拒绝退货 / <c>6</c> 待用户退货 / <c>7</c> 关闭 / <c>8</c> 待商家收货 /
/// <c>11</c> 平台退款中 / <c>13</c> 退款成功 / <c>21</c> 平台处理退款申请中 / <c>22</c> 废弃 /
/// <c>23</c> 商家处理退货中 / <c>24</c> 平台处理退货中 / <c>25</c> 平台退款失败；
/// <b>缺 3、9、10、12、14~20</b> ⇒ 不得按序数遍历。
/// </para>
/// <para>
/// <b>写口并发约束</b>：<see cref="AcceptRefundAsync"/> / <see cref="AcceptReturnAsync"/> /
/// <see cref="RejectAftersaleAsync"/> / <see cref="UpdateAftersaleAsync"/> 是<b>互斥的状态推进</b>
/// （同一售后单只能走其中一条分支）；全族写口<b>非幂等</b>，不得放进自动重试管道。
/// </para>
/// <para>
/// <b>官方文档残缺（照录）</b>：<c>cancel</c> 与 <c>uploadreturninfo</c> 的请求示例缺分隔逗号；
/// <c>merchanttest</c> 参数表把 <c>operation</c> 印成 <c>operatoin</c>；
/// <c>get_list</c> 回包示例键值全空。DTO 已按参数表 + 可解析示例建模并逐处留档。
/// </para>
/// <para>
/// <b>敏感面</b>：<c>openid</c>、<c>refund_reason</c>、<c>media_list[].url</c>、
/// 退货地址（姓名 / 电话 / 详细地址）、<c>waybill_id</c> 与 <c>refund_pay_detail.refund_id</c>
/// ⇒ 一律禁入日志、遥测与异常消息。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：11 页官方正文<b>未给</b>端点级频率数字、权限集编号与云调用方法名
/// （2026-10-18 逐页取证）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAfterSale", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaEcAftersaleService
{
    /// <summary>
    /// 创建售后（<c>POST /shop/ecaftersale/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/add_new.html"/>。
    /// </summary>
    /// <param name="request">请求体（单商品 + 原因 + 金额），见 <see cref="WxaEcAftersaleAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>微信侧 <c>aftersale_id</c>，见 <see cref="WxaEcAftersaleAddResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>订单二选一</b>：<c>order_id</c> 与 <c>out_order_id</c> 均选填但须给一支；
    /// <c>out_aftersale_id</c> 可留空，此时必须把应答 <c>aftersale_id</c> 落库作为后续定位键。
    /// </para>
    /// <para>
    /// <b>单商品</b>：<c>product_info</c> 是对象（不是数组），多商品须拆多张售后单。
    /// <b>金额单位分</b>；<c>refund_reason_type</c> 须与售后类型匹配，见
    /// <see cref="WxaEcAftersaleReasonTypes"/>。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> / <c>refund_reason</c> / <c>media_list[].url</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/ecaftersale/add")]
    Task<WxaEcAftersaleAddResponse> AddAftersaleAsync(
        [Body] WxaEcAftersaleAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消售后（<c>POST /shop/ecaftersale/cancel</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/cancel.html"/>。
    /// </summary>
    /// <param name="request">请求体（售后单号二选一 + <c>openid</c>），见 <see cref="WxaEcAftersaleCancelRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>商家侧取消</b>：把售后单推进到状态 <c>1</c>（用户取消）的语义，属不可逆写口；
    /// 官方请求示例缺分隔逗号，字段仍以参数表为准。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/ecaftersale/cancel")]
    Task<WxaResponse> CancelAftersaleAsync(
        [Body] WxaEcAftersaleCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取售后（<c>POST /shop/ecaftersale/get</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/get_new.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>aftersale_id</c> / <c>out_aftersale_id</c> 二选一），见 <see cref="WxaEcAftersaleGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>after_sales_order</c>（单对象），见 <see cref="WxaEcAftersaleGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>应答键是单数对象</b> <c>after_sales_order</c>，不是列表、也无 <c>data</c> 包装。</para>
    /// <para><b>敏感值</b>：应答含 <c>openid</c>、退货地址与退款 ID ⇒ 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/ecaftersale/get")]
    Task<WxaEcAftersaleGetResponse> GetAftersaleAsync(
        [Body] WxaEcAftersaleGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取售后列表（<c>POST /shop/ecaftersale/get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/get_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>offset</c> / <c>limit</c> 必填，时间窗口为毫秒），见 <see cref="WxaEcAftersaleGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>after_sales_orders</c> 单号数组 + <c>has_more</c>，见 <see cref="WxaEcAftersaleGetListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>回包是单号列表而非详情列表</b>：官方表类型「Array string」、说明「售后单号列表」⇒
    /// 须逐条再调 <see cref="GetAftersaleAsync"/>；<c>has_more=false</c> 才是末页。
    /// </para>
    /// <para>
    /// <b>官方回包示例残缺（照录）</b>：示例两键均无值 ⇒ 不得以示例推断对象形态。
    /// 时间窗口为<b>毫秒</b>（与纠纷单列表的秒不同）。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/ecaftersale/get_list")]
    Task<WxaEcAftersaleGetListResponse> GetAftersaleListAsync(
        [Body] WxaEcAftersaleGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新售后（<c>POST /shop/ecaftersale/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/aftersale_update.html"/>。
    /// </summary>
    /// <param name="request">请求体（金额 / 原因 / 凭证），见 <see cref="WxaEcAftersaleUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>不能改售后类型</b>：官方原文要求 <c>type</c> 与当前类型一致、不支持修改
    /// ⇒ 本端点只改金额、原因与凭证。金额单位<b>分</b>。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> / <c>refund_reason</c> / <c>media_list[].url</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/ecaftersale/update")]
    Task<WxaResponse> UpdateAftersaleAsync(
        [Body] WxaEcAftersaleUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同意退款（<c>POST /shop/ecaftersale/acceptrefund</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/acceptrefund.html"/>。
    /// </summary>
    /// <param name="request">请求体（售后单号二选一），见 <see cref="WxaEcAftersaleIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>仅适用于「仅退款」分支</b>：官方本页无地址参数；退货退款须走
    /// <see cref="AcceptReturnAsync"/> 并提交退货地址。两键均标选填、二选一，由宿主保证至少给一支。
    /// </para>
    /// <para><b>非幂等</b>：状态推进不可逆，不得自动重试。</para>
    /// </remarks>
    [Post("/shop/ecaftersale/acceptrefund")]
    Task<WxaResponse> AcceptRefundAsync(
        [Body] WxaEcAftersaleIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 同意退货（<c>POST /shop/ecaftersale/acceptreturn</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/acceptreturn.html"/>。
    /// </summary>
    /// <param name="request">请求体（售后单号 + 退货地址），见 <see cref="WxaEcAftersaleAcceptReturnRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>地址键是 <c>town</c></b>：<c>address_info.receiver_name</c> / <c>detailed_address</c> /
    /// <c>tel_number</c> / <c>province</c> / <c>city</c> / <c>town</c> 必填，仅 <c>country</c> 选填
    /// ⇒ 与公共结构 <see cref="WxaEcAftersaleReturnAddressInfo"/> 的 <c>county</c> 不是同一键。
    /// </para>
    /// <para><b>敏感值</b>：退货地址属个人信息，禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/ecaftersale/acceptreturn")]
    Task<WxaResponse> AcceptReturnAsync(
        [Body] WxaEcAftersaleAcceptReturnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拒绝售后（<c>POST /shop/ecaftersale/reject</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/reject.html"/>。
    /// </summary>
    /// <param name="request">请求体（售后单号二选一），见 <see cref="WxaEcAftersaleIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方本页无拒绝原因字段</b>：参数表只有售后单号二选一 ⇒ SDK 不臆造 <c>reject_reason</c>。
    /// 与同意退款 / 同意退货<b>互斥</b>（同一售后单只能走一条分支），非幂等。
    /// </para>
    /// </remarks>
    [Post("/shop/ecaftersale/reject")]
    Task<WxaResponse> RejectAftersaleAsync(
        [Body] WxaEcAftersaleIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传协商凭证（<c>POST /shop/ecaftersale/upload_certificates</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/upload_certificates.html"/>。
    /// </summary>
    /// <param name="request">请求体（单号 + 描述 + 图片 URL 列表），见 <see cref="WxaEcAftersaleUploadCertificatesRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b><c>certificates</c> 是字符串数组</b>（图片 URL），不是 <c>Media</c> 对象数组；
    /// <c>refund_desc</c> 与 <c>certificates</c> 官方均标选填。追加语义、非幂等。
    /// </para>
    /// </remarks>
    [Post("/shop/ecaftersale/upload_certificates")]
    Task<WxaResponse> UploadCertificatesAsync(
        [Body] WxaEcAftersaleUploadCertificatesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传退货物流（<c>POST /shop/ecaftersale/uploadreturninfo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/uploadreturninfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（单号 + <c>openid</c> + 快递三键），见 <see cref="WxaEcAftersaleUploadReturnInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>四支必填</b>：<c>openid</c> / <c>delivery_id</c> / <c>waybill_id</c> 必填，
    /// <c>delivery_name</c> 选填；官方请求示例缺逗号，字段以参数表为准。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> 与运单号禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/ecaftersale/uploadreturninfo")]
    Task<WxaResponse> UploadReturnInfoAsync(
        [Body] WxaEcAftersaleUploadReturnInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 售后超时模拟（<c>POST /shop/ecaftersale/merchanttest</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/merchanttest.html"/>。
    /// </summary>
    /// <param name="request">请求体（单号 + <c>operation</c>），见 <see cref="WxaEcAftersaleMerchantTestRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>联调模拟器</b>：强制触发五类超时流转（用户上传退货物流 / 用户申请平台介入 / 商家处理退货 /
    /// 商家处理退款 / 商家确认收货），生产环境须禁用。
    /// </para>
    /// <para>
    /// <b>官方字段名笔误（照录）</b>：参数表印 <c>operatoin</c>、示例写 <c>operation</c>
    /// ⇒ DTO 取示例拼写并注明，不提供别名。
    /// </para>
    /// </remarks>
    [Post("/shop/ecaftersale/merchanttest")]
    Task<WxaResponse> MerchantTestAsync(
        [Body] WxaEcAftersaleMerchantTestRequest request,
        CancellationToken cancellationToken = default);
}
