// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Combine;

/// <summary>
/// 合单商品单交易状态（官方 <c>sub_orders[].trade_state</c>，<b>3 值</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013462574"/>
/// （合单订单支付成功回调通知页的明文字段表，2026-10-09 逐字核验；更新时间 2025.01.16）。
/// 官方逐字给出：<c>trade_state</c> 为 <c>SUCCESS</c> / <c>NOTPAY</c> / <c>CLOSED</c>。
/// </para>
/// <para>
/// <b>⚠️ 核验边界（紧记）</b>：本类取值取自<b>回调通知页</b>；合单<b>查询订单</b>页的
/// <c>trade_state</c> 取值表本轮<b>未</b>单独核验（该页此前只确认了「字段名 + 必填性」）。
/// 两者是<b>同名字段、同域语义</b>，故本类对查询侧仅作<b>交叉引用</b>而非「该页逐字确认」。
/// </para>
/// <para>
/// <b>为何不能在回调里假定成功</b>：即便通知类型是「支付成功」，<c>trade_state</c>
/// 仍须<b>显式判定为 <see cref="Success"/></b>（<c>NOTPAY</c> / <c>CLOSED</c> 亦在官方取值表内）。
/// </para>
/// </remarks>
public static class CombineTradeStates
{
    /// <summary>支付成功（官方 <c>SUCCESS</c>）：<b>只有</b>此值才可判定该商品单已支付。</summary>
    public const string Success = "SUCCESS";

    /// <summary>未支付（官方 <c>NOTPAY</c>）。</summary>
    public const string NotPay = "NOTPAY";

    /// <summary>已关闭（官方 <c>CLOSED</c>）。</summary>
    public const string Closed = "CLOSED";
}

/// <summary>
/// 合单商品单交易类型（官方 <c>sub_orders[].trade_type</c>，<b>4 值</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源</b>：同 <see cref="CombineTradeStates"/>（合单支付成功通知页明文字段表）。
/// 官方逐字给出：<c>JSAPI</c> / <c>NATIVE</c> / <c>APP</c> / <c>MWEB</c>
/// —— 注意 <c>MWEB</c> 是 H5 场景的官方取值（<b>不是</b> <c>H5</c>）。
/// </para>
/// <para>
/// <b>与下单接口的对应关系</b>：本域四个下单端点分别对应
/// <c>JSAPI</c>（<c>/jsapi</c>）、<c>NATIVE</c>（<c>/native</c>）、<c>APP</c>（<c>/app</c>）、
/// <c>MWEB</c>（<c>/h5</c>）—— 最后一个的<b>端点名与取值名不同</b>，回调用 <c>MWEB</c> 匹配时勿写 <c>H5</c>。
/// </para>
/// </remarks>
public static class CombineTradeTypes
{
    /// <summary>公众号 / 小程序支付（官方 <c>JSAPI</c>）：对应下单端点 <c>/v3/combine-transactions/jsapi</c>。</summary>
    public const string JsApi = "JSAPI";

    /// <summary>扫码支付（官方 <c>NATIVE</c>）：对应下单端点 <c>/v3/combine-transactions/native</c>。</summary>
    public const string Native = "NATIVE";

    /// <summary>APP 支付（官方 <c>APP</c>）：对应下单端点 <c>/v3/combine-transactions/app</c>。</summary>
    public const string App = "APP";

    /// <summary>H5 支付（官方 <c>MWEB</c>）：对应下单端点 <c>/v3/combine-transactions/h5</c>（取值名与端点名不同）。</summary>
    public const string MWeb = "MWEB";
}
