// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Combine;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「合单支付」域 SDK（APIv3 合单，<b>6 端点</b>：JSAPI 合单下单 / Native 合单下单 /
/// <b>APP 合单下单</b> / <b>H5 合单下单</b> / 合单关闭订单 / 合单查询订单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（<b>合作伙伴</b>文档中心，2026-10-09 逐页核验；本域按<b>服务商面</b>建模）：
/// JSAPI/小程序合单下单 <see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml"/>
/// （更新时间 2025.01.16）、Native 合单下单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012758240"/>
/// （更新时间 2025.01.16）、合单关闭订单 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012709095"/>
/// （更新时间 2024.10.24）、合单查询订单
/// <see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter7_3_11.shtml"/>。
/// </para>
/// <para>
/// <b>⚠️ 合单支付在官方文档中同时存在「普通商户」与「普通服务商」两个面</b>，本域按<b>服务商面</b>建模
/// —— 各页均标注<b>支持商户：【普通服务商】</b>，服务商模式支持 <b>1–50 笔</b>商品单。
/// 普通商户面亦有对应页（Native 合单下单 <c>…/wiki/doc/apiv3/apis/chapter5_1_5.shtml</c>、
/// App 合单下单 <c>…/wiki/doc/apiv3/open/pay/chapter2_9_3.shtml</c>，均标注【普通商户】），
/// 差异是<b>只支持 2–10 笔</b>、且子单字段表<b>无</b> <c>sub_mchid</c> / <c>sub_appid</c>。
/// <b>路由两面共用</b>，故本域 DTO 以服务商面为准；<b>普通商户接入时勿填 <c>sub_mchid</c> / <c>sub_appid</c></b>。
/// 本仓商户基座已支持服务商（<c>{sp_mchid}:{sub_mchid}</c> 分槽），故两面均可使用。
/// </para>
/// <para>
/// <b>✅ 四个下单场景现已齐全</b>（JSAPI / Native / APP / H5）—— 原「APP / H5 尚未覆盖」留档已随增量关闭。
/// 四个接口<b>顶层字段表各不相同</b>，故各自独立 DTO；<b>下单应答也有两种</b>
/// （JSAPI / APP 给 <c>prepay_id</c>，Native 给 <c>code_url</c>，H5 给 <c>h5_url</c>）。
/// </para>
/// <para>
/// <b>本域仅剩「非 HTTP 能力」未覆盖</b>：调起支付（客户端 SDK）。
/// <b>✅ 通知面已完成</b>：合单支付成功通知载荷见回调包 <c>GetCombineTransaction</c>
/// （⚠️ 其信封 <c>event_type</c> / <c>original_type</c> 与普通支付成功通知<b>完全同值</b>，
/// 判别只能靠<b>解密后的载荷形态</b>）；合单退款通知复用标准 <c>REFUND.*</c> 事件
/// （退款逐子单走退款域，<b>无</b>专属事件类型）。
/// <b>不要凭推断补路由</b>。
/// </para>
/// <para>
/// <b>⚠️「合单退款」不是一个接口</b>：官方《订单退款》开发指引
/// （<see href="https://pay.weixin.qq.com/doc/v3/partner/4013080623"/>，更新 2026.06.10）明示
/// 「对于合单支付的订单，<b>无法通过合单支付总单号 <c>combine_out_trade_no</c> 退款，
/// 只能根据单个子单进行退款</b>」，且 <c>transaction_id</c> 须填 <c>sub_orders.transaction_id</c>、
/// <c>out_trade_no</c> 须填 <c>sub_orders.out_trade_no</c> ⇒ 退款<b>逐子单</b>调用退款域的
/// <see cref="IWechatPayRefundService"/> 即可，本域<b>不</b>另设端点（守卫 CB6 固化该裁决，
/// 防后来者照第三方博客臆造的 <c>/v3/combine-transactions/refunds</c> 去补一个不存在的路由）。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "CombineTransactions", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayCombineService
{
    /// <summary>
    /// JSAPI / 小程序合单下单。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml"/>。
    /// </summary>
    /// <param name="request">合单下单请求体，见 <see cref="CombinePrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预支付交易会话标识（2 小时有效），见 <see cref="CombinePrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/combine-transactions/jsapi</c>；无 path / query 参数。</para>
    /// <para><b>子单约束</b>：<c>sub_orders</c> <b>1–50 笔</b>；<c>sub_appid</c> <b>仅允许一笔</b>商品单填写。</para>
    /// <para><b>支付者约束</b>：<c>combine_payer_info</c> 的 <c>openid</c> 与 <c>sub_openid</c> <b>二选一必填</b>，
    /// 且传 <c>sub_openid</c> 时 <c>sub_appid</c> 必填。</para>
    /// </remarks>
    [Post("/v3/combine-transactions/jsapi")]
    Task<CombinePrepayResponse> CreateJsapiOrderAsync(
        [Body] CombinePrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 合单关闭订单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012709095"/>。
    /// </summary>
    /// <param name="combineOutTradeNo">合单商户订单号（官方 path <c>combine_out_trade_no</c>，必填 string(32)）。</param>
    /// <param name="request">关单请求体（<c>combine_appid</c> + <c>sub_orders</c>），见 <see cref="CombineCloseOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/combine-transactions/out-trade-no/{combine_out_trade_no}/close</c>；
    /// path 必带 <c>combine_out_trade_no</c>。</para>
    /// <para>
    /// <b>三条硬约束（官方原文）</b>：① 合单支付订单<b>只能</b>用本接口关单；
    /// ② <b>不支持关闭部分子单</b>；③ 主单商户号 / 单号 / 子单个数 / 子单商户号 / 子单单号
    /// <b>必须与下单时完全一致</b>。
    /// </para>
    /// <para>
    /// <b>返回类型为何是 <c>Task</c></b>：官方明确「无应答包体」，成功状态码 <b>204 No Content</b>
    /// （与交易域关单、支付分解除授权同款处置）。
    /// </para>
    /// </remarks>
    [Post("/v3/combine-transactions/out-trade-no/{combineOutTradeNo}/close")]
    Task CloseOrderAsync(
        [Path] string combineOutTradeNo,
        [Body] CombineCloseOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 合单查询订单。官方文档：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter7_3_11.shtml"/>。
    /// </summary>
    /// <param name="combineOutTradeNo">合单商户订单号（官方 path <c>combine_out_trade_no</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>合单订单（含各商品单的交易状态与实付金额），见 <see cref="CombineQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/combine-transactions/out-trade-no/{combine_out_trade_no}</c>；
    /// <b>无 query 参数</b>。</para>
    /// <para>
    /// <b>查询方式只有一种</b>：官方本页仅提供按<b>合单商户订单号</b>查询，
    /// <b>没有</b>「按微信支付订单号」的等价入口（与单笔交易域的两种入口不同）—— 勿照搬交易域的直觉。
    /// </para>
    /// <para>
    /// <b>状态判定</b>：<c>sub_orders[].trade_state</c>（必填）是<b>每个商品单各自</b>的交易状态，
    /// 须逐单显式判定；<c>amount.payer_amount</c> 才是实付金额（<c>total_amount</c> 是标价）。
    /// </para>
    /// </remarks>
    [Get("/v3/combine-transactions/out-trade-no/{combineOutTradeNo}")]
    Task<CombineQueryResponse> QueryOrderAsync(
        [Path] string combineOutTradeNo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Native 合单下单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012758240"/>。
    /// </summary>
    /// <param name="request">Native 合单下单请求体，见 <see cref="CombineNativePrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>支付二维码链接（<c>code_url</c>，<b>有效期 2 小时</b>），见 <see cref="CombineNativePrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/combine-transactions/native</c>；无 path / query 参数。</para>
    /// <para>
    /// <b>与 JSAPI 下单的三点差异</b>（勿混用 DTO）：① 请求<b>无支付者信息</b>
    /// （无 <c>combine_payer_info</c>）；② 应答<b>只有 <c>code_url</c></b>（无 <c>prepay_id</c>）；
    /// ③ 支付由用户<b>扫码</b>发起，服务端须自行把 <c>code_url</c> 生成二维码展示。
    /// </para>
    /// <para>
    /// <b><c>code_url</c> 有效期 2 小时</b>（官方原文）：失效后须<b>重新调用本接口</b>取新链接，
    /// 不得对旧链接做拼接或改写。
    /// </para>
    /// </remarks>
    [Post("/v3/combine-transactions/native")]
    Task<CombineNativePrepayResponse> CreateNativeOrderAsync(
        [Body] CombineNativePrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// APP 合单下单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015973099"/>。
    /// </summary>
    /// <param name="request">APP 合单下单请求体，见 <see cref="CombineAppPrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预支付交易会话标识（<c>prepay_id</c>，<b>有效期 2 小时</b>），见 <see cref="CombinePrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/combine-transactions/app</c>；无 path / query 参数。
    /// 支持商户<b>【普通服务商】【平台商户】</b>（更新时间 2025.09.02）。</para>
    /// <para>
    /// <b>⚠️ 请求里的 <c>prepay_id</c> 是「追加订单」专用</b>：官方中文说明为
    /// 「当<b>发起追加订单</b>，需传入目标追加的合单单号对应的预支付交易会话标识」
    /// ⇒ 首次下单<b>不传</b>；<b>勿与应答里的 <c>prepay_id</c> 混淆</b>
    /// （同名、同意义，但一个是入参、一个是回参）。
    /// </para>
    /// <para>
    /// <b>应答类型与 JSAPI 共用</b>：APP 应答<b>同样只有 <c>prepay_id</c></b>（两页应答表一致）⇒
    /// 复用 <see cref="CombinePrepayResponse"/>，由「调起支付」的客户端 SDK 场景决定怎么用。
    /// </para>
    /// <para>
    /// <b>子单表与 JSAPI <b>不</b>共用</b>：APP 页的子单<b>多</b> <c>time_start</c> / <c>time_expire</c>，
    /// 且 <c>sub_mchid</c> 被标为<b>选填</b>（JSAPI / H5 页为必填）⇒ 见 <see cref="CombineAppSubOrder"/> 的 remarks。
    /// </para>
    /// </remarks>
    [Post("/v3/combine-transactions/app")]
    Task<CombinePrepayResponse> CreateAppOrderAsync(
        [Body] CombineAppPrepayRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// H5 合单下单。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012758208"/>。
    /// </summary>
    /// <param name="request">H5 合单下单请求体，见 <see cref="CombineH5PrepayRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>H5 支付跳转链接（<c>h5_url</c>），见 <see cref="CombineH5PrepayResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/combine-transactions/h5</c>；无 path / query 参数。
    /// 服务商模式支持 <b>1–50 笔</b>（更新时间 2025.01.16）。</para>
    /// <para>
    /// <b>⚠️ 服务商面与普通商户面字段不同</b>：官方<b>普通商户</b>面同路由页
    /// （<c>…/doc/v3/merchant/4012545879</c>，产品线标注「指定身份支付」）<b>有</b> <c>combine_payer_info</c>
    /// （含实名 <c>identity</c>）与 <c>time_start</c>；而<b>服务商面本页无</b>这两处
    /// ⇒ 本接口按<b>服务商面</b>建模（普通商户接入须以自身页面字段为准）。
    /// </para>
    /// <para>
    /// <b>⚠️ H5 场景下最容易漏的一处</b>：<see cref="CombineH5SceneInfo"/> 的 <c>device_id</c>
    /// 在<b>本页标注必填</b>（其它下单页均选填）⇒ 从别的下单接口复制请求体会漏掉它。
    /// </para>
    /// <para>
    /// <b>拿到 <c>h5_url</c> 后</b>须按官方《H5 调起支付》指引跳转（含 <c>Referer</c> 等要求），
    /// <b>严禁</b>自行改写或拼装该链接。
    /// </para>
    /// </remarks>
    [Post("/v3/combine-transactions/h5")]
    Task<CombineH5PrepayResponse> CreateH5OrderAsync(
        [Body] CombineH5PrepayRequest request,
        CancellationToken cancellationToken = default);
}
