// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Transfer;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「商家转账」域 SDK（APIv3 商家转账，<b>首批 1 端点</b>：发起转账）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716434"/>
/// （发起转账，2026-10-09 逐字段核验；更新时间 2025.03.21）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>官方「商家转账」接口清单（共 8 项，照录自该页导航）</b>：① 发起转账 ✔本批已实现；
/// ② APP 调起用户确认收款（客户端 SDK）；③ JSAPI 调起用户确认收款（客户端 SDK）；
/// ④ 撤销转账；⑤ 商户单号查询转账单；⑥ 微信单号查询转账单；⑦ 商家转账回调通知（归回调包）；
/// ⑧ 获取电子回单（已核一条路由：商户单号查询电子回单
/// <c>GET /v3/fund-app/mch-transfer/elecsign/out-bill-no/{out_bill_no}</c>）。
/// </para>
/// <para>
/// <b>🔴 本域第一原则（官方原文）</b>：发起转账遇到错误码时<b>不得换单重试</b> ——
/// 必须先经『商户单号/微信单号查询转账单』确认原单结果，<b>明确为失败后</b>才可换号重试，
/// 否则有<b>重复转账的资金风险</b>。故本域<b>必须先补齐两个查询端点</b>才具备生产可用性；
/// 本批先落发起端点是**刻意的最小步**（见下条待办）。
/// </para>
/// <para>
/// <b>本批未覆盖</b>：撤销转账 · 商户单号查询转账单 · 微信单号查询转账单 · 获取电子回单。
/// 并且：各枚举字段（<c>state</c> / <c>transfer_scene_id</c> / <c>user_recv_perception</c> /
/// <c>user_recv_style.type</c>）的<b>值域本轮未核验</b> ⇒ SDK 不臆造常量。
/// <b>不要凭推断补路由或补取值</b>。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Transfer", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayTransferService
{
    /// <summary>
    /// 发起转账。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716434"/>。
    /// </summary>
    /// <param name="request">发起转账请求体，见 <see cref="TransferBillRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>转账单（含 <c>state</c> 与调起用户确认收款的 <c>package_info</c>），见 <see cref="TransferBillResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/fund-app/mch-transfer/transfer-bills</c>；无 path / query 参数。</para>
    /// <para>
    /// <b>🔴 调用方必须遵守（官方原文）</b>：遇到错误码时<b>不要换单重试</b> ——
    /// 先查清原单结果，明确失败后才换 <c>out_bill_no</c> 重试，否则有<b>重复转账资金风险</b>。
    /// </para>
    /// <para>
    /// <b>异步语义</b>：应答中的 <c>package_info</c> 只是<b>调起用户确认收款</b>的凭据，
    /// <b>不等于</b>转账成功；终态须经查询端点或回调通知确认（<c>state</c> 值域本轮未核验）。
    /// </para>
    /// </remarks>
    [Post("/v3/fund-app/mch-transfer/transfer-bills")]
    Task<TransferBillResponse> CreateTransferBillAsync(
        [Body] TransferBillRequest request,
        CancellationToken cancellationToken = default);
}
