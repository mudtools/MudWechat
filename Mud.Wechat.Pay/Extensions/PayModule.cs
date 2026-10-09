// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Extensions;

/// <summary>
/// 微信支付 API 模块枚举（对齐 <c>WechatModule</c>，三段式注册的第一段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>命名红线</b>：本枚举的成员即源生成器的 <c>RegistryGroupName</c>，因此<b>绝不能叫 <c>Pay</c></b> ——
/// 本仓已有「企业支付」产品线占用了 <c>WechatModule.Pay</c> 与生成的 <c>AddPayWebApiHttpClient()</c>。
/// 若两条线用同一个组名，两个程序集会各产出一个同名扩展方法，跨程序集导入即歧义。
/// 实测全仓既有 59 个组名，<c>Transactions</c> / <c>Refund</c> / <c>Bill</c> / <c>Certificates</c> 均未被占用。
/// </para>
/// <para>
/// <b>成员只列已落地域</b>：没有对应 <c>Add{X}WebApiHttpClient()</c> 的成员会落进
/// <see cref="PayServiceBuilder.AddModules"/> 的「查不到注册器 ⇒ 静默不注册」分支，
/// 是一个无声陷阱。故随域增量加入，不预先铺空值。
/// </para>
/// <para>模块注册是三段式：本枚举值 + <c>Add{域}Api()</c>（见 <see cref="PayServiceBuilder"/>）+ 源生成器产出的 <c>Add{域}WebApiHttpClient()</c>。</para>
/// </remarks>
public enum PayModule
{
    /// <summary>
    /// 基础交易（下单 / 查单 / 关单，4 端点）：JSAPI·小程序下单 + 微信支付订单号查单 + 商户订单号查单 + 关闭订单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791897"/>。
    /// </summary>
    Transactions,

    /// <summary>
    /// 退款（3 端点）：申请退款 + 查询单笔退款 + 发起异常退款。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791903"/>。
    /// </summary>
    Refund,

    /// <summary>
    /// 账单（2 端点）：申请交易账单 + 申请资金账单（账单文件本身经 <c>IWechatPayBillDownloadService</c> 下载）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791907"/>。
    /// </summary>
    Bill,

    /// <summary>
    /// 平台证书（1 端点）：获取平台证书列表（用于应答 / 回调验签与 <c>Wechatpay-Serial</c> 轮换）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551764"/>。
    /// </summary>
    Certificates,

    /// <summary>
    /// 分账（<b>P2 首批 3 端点</b>）：添加分账接收方 + 请求分账 + 查询分账结果。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524936"/>。
    /// </summary>
    /// <remarks>其余分账端点（回退 / 解冻 / 待分金额 / 删除接收方 / 分账账单）待逐页核验后增量。</remarks>
    ProfitSharing,

    /// <summary>
    /// 支付分（<b>P2 首批 3 端点</b>）：创建 / 查询 / 取消服务订单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587900"/>。
    /// </summary>
    /// <remarks>完结 / 收款 / 同步 / 授权与回调等待逐页核验后增量。</remarks>
    PayScore,

    /// <summary>
    /// 合单支付（<b>P2 首批 2 端点</b>，<b>仅服务商</b>）：JSAPI/小程序合单下单 + 合单关闭订单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml"/>。
    /// </summary>
    /// <remarks>合单查询 / Native 下单 / 合单退款等待逐页核验后增量。</remarks>
    CombineTransactions,

    /// <summary>
    /// 商家转账（<b>P2 首批 2 端点</b>）：发起转账 + 商户单号查询转账单。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012716434"/>。
    /// </summary>
    /// <remarks>
    /// <b>「不换单重试」红线的最低闭环已具备</b>：发起转账遇错后可按商户单号查单，再依 <c>state</c>
    /// （<c>ACCEPTED</c>/<c>PROCESSING</c> ⇒ <b>原单重试</b>；<c>FAIL</c> ⇒ 才可重新生成单据）处置。
    /// <b>仍缺</b>：微信单号查询转账单 · 撤销转账 · 获取电子回单（商户单号/微信单号）。
    /// </remarks>
    Transfer,
}
