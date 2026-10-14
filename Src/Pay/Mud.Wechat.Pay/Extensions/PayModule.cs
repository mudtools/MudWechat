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

    /// <summary>
    /// 电子发票（<b>P2 首批 2 端点</b>）：开具 + 查询电子发票。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>。
    /// </summary>
    /// <remarks>
    /// 已核并更正设计方案 §2.5：<b>普通商户侧有</b>完整电子发票文档（非「无独立入口」）；
    /// 服务商侧的「开具通用行业电子发票」是另一路由（<c>issue-general</c>），本域不覆盖。
    /// </remarks>
    NewTaxControlFapiao,

    /// <summary>
    /// 代金券（<b>P2 表内最后一项，首批 2 端点</b>）：创建代金券批次 + 查询代金券详情。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534633"/>。
    /// </summary>
    /// <remarks>
    /// 官方流程：创建 → <b>激活</b>（未实现）→ 发放；<b>创建成功 ≠ 可发放</b>。
    /// </remarks>
    MarketingFavor,

    /// <summary>
    /// 特约商户进件（<b>P0-1，6 端点</b>）：提交进件申请单 + 按申请单号 / 业务编号查状态 +
    /// 修改结算账户 + 查询结算账户 + 查询结算账户修改申请状态。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012719997"/>。
    /// </summary>
    /// <remarks>
    /// <b>服务商形态</b>：签名主体是服务商商户号；<c>sub_mchid</c> 是<b>调用期参数</b>（path）而非商户作用域，
    /// 且进件阶段子商户号尚不存在 ⇒ 宿主应以<b>服务商自有商户号</b>的商户配置进入调用作用域。
    /// 提交申请单与修改结算账户<b>均为异步审核</b>，受理成功不等于入驻 / 改卡生效。
    /// </remarks>
    Apply4SubMerchant,

    /// <summary>
    /// 主体进件 / 商户开户意愿确认（<b>P0-2，5 端点</b>）：提交申请单 + 查询申请单审核结果 +
    /// 按业务编号 / 申请单号撤销（两个变体）+ 获取商户开户意愿确认状态。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012722388"/>。
    /// </summary>
    /// <remarks>
    /// <b>支持商户类型为渠道商 / 从业机构（银行、支付机构）</b>，不含普通服务商 ⇒
    /// 与 <see cref="PayModule.Apply4SubMerchant"/> 是两套契约（前缀、查询定位方式、状态取值、
    /// 主体类型枚举全部不同，<b>无状态映射表可写</b>）。签名主体是服务商 / 从业机构自有商户号，
    /// <c>sub_mchid</c> 与 <c>channel_id</c> 均为<b>调用期参数 / 请求体字段</b>而非商户作用域。
    /// </remarks>
    Apply4Subject,

    /// <summary>
    /// 服务商基础支付 / 分期（<b>P0-3，9 端点</b>）：下单五族（JSAPI/小程序、Native、APP、H5、扫码支付）+
    /// 双路查单 + 关单 + 分期曝光咨询。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714911"/>。
    /// </summary>
    /// <remarks>
    /// <b>服务商形态</b>：签名主体是<b>服务商商户号</b>（<c>sp_mchid</c>），<c>sub_mchid</c> 是
    /// <b>报文 / Query 参数</b>而非商户作用域 ⇒ 宿主应以服务商自有商户配置进入调用作用域，
    /// <b>不得</b>用子商户证书调本域。与 <see cref="PayModule.Transactions"/>（直连）是两套契约：
    /// 商户标识四分、<c>payer</c> 双 openid <b>二选一</b>、<c>support_fapiao</c> 位置按族分列。
    /// 官方 SKIT 清单里的 <c>codepay</c> / <c>reverse</c> 是 <b>APIv2</b> 面（现行正文无 V3 字段表）、
    /// <c>transactions/exposure</c> 页面<b>已下架</b> ⇒ 本域零端点，前两者归 APIv2 批次。
    /// </remarks>
    PartnerTransactions,

    /// <summary>
    /// 服务商支付分（<b>P0-4，23 端点</b>）：用户授权 7（预授权 ×2 + 风险评估 + 授权记录查 ×2 + 解除授权 ×2）+
    /// 服务订单 8（创建 / 查询 / 取消 / 完结 / 修改 / 未完结提醒 / 确认付款提醒 / 不同步支付结果）+
    /// 服务额度 1 + 支付分计划 3 + 计划型服务订单 1 + 用户签约计划 3。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012692136"/>。
    /// </summary>
    /// <remarks>
    /// <b>本域商户标识形态与 <see cref="PayModule.PartnerTransactions"/> 相反</b>：服务商支付分全部报文用
    /// <c>appid</c> / <c>mchid</c>（服务商侧）+ <c>sub_mchid</c> / <c>sub_appid</c>（子商户侧），
    /// <b>没有</b> <c>sp_</c> 前缀字段 ⇒ 照抄基础支付字段名会静默丢字段。
    /// 签名主体是<b>服务商商户号</b>，<c>sub_mchid</c> 按端点分列 query / body 两处。
    /// 与 <see cref="PayModule.PayScore"/>（直连）是两套契约：三套路由前缀
    /// （<c>/v3/payscore/partner/</c>、<c>/v3/payscore/plan/partner/</c>、<c>/v3/payscore/sign-plan/partner/</c>）、
    /// 两套时间格式并存、状态取值集合并非直连超集。
    /// 官方 SKIT 清单里的「商户收款」<c>POST /v3/payscore/serviceorder/{out_order_no}/pay</c>
    /// 现行目录<b>无该端点</b> ⇒ 本域零端点（守卫 PS-B2）。
    /// </remarks>
    PartnerPayScore,
}
