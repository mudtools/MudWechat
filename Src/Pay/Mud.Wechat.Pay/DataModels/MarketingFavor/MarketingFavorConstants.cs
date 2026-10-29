// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.MarketingFavor;

/// <summary>
/// 代金券<b>批次状态</b>（官方 <c>status</c>，5 值，<b>全小写</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方来源</b>：<see href="https://pay.weixin.qq.com/docs/merchant/apis/cash-coupons/stock/query-stock.html"/>
/// （查询批次详情，2026-10-09 逐字核验；更新时间 2025.03.25）。
/// </para>
/// <para>
/// <b>⚠️ 官方拼写照录：<c>stoped</c>（少一个 p）</b>：正确英文是 <c>stopped</c>，
/// 但官方页面逐字为 <c>stoped</c> ⇒ 取值<b>必须</b>照录，否则永远匹配不上。
/// 本仓已有同款先例（如 <c>digest_alogrithm</c>、<c>ture</c>）：<b>官方拼写优先于拼写正确性</b>。
/// </para>
/// <para>
/// <b>⚠️ 全小写</b>：与支付分 / 电子发票的 <c>event_type</c>（<b>全大写</b>）风格相反 ——
/// 本仓不同产品线的取值风格确实不统一，<b>不得</b>按「微信都用大写」类推。
/// </para>
/// </remarks>
public static class CouponStockStatuses
{
    /// <summary>未激活（官方 <c>unactivated</c>）：<b>不可</b>发放。</summary>
    public const string Unactivated = "unactivated";

    /// <summary>审核中（官方 <c>audit</c>）：<b>不可</b>发放。</summary>
    public const string Audit = "audit";

    /// <summary>运行中（官方 <c>running</c>）：<b>唯一</b>可发放的状态（官方发券接口错误码：非运行中将报 <c>param_error</c>）。</summary>
    public const string Running = "running";

    /// <summary>已停止（官方 <c>stoped</c>，<b>官方拼写照录，少一个 p</b>）。</summary>
    public const string Stoped = "stoped";

    /// <summary>暂停发放（官方 <c>paused</c>）。</summary>
    public const string Paused = "paused";
}

/// <summary>
/// 代金券类型（官方 <c>coupon_type</c>）。
/// </summary>
/// <remarks>
/// <b>官方来源</b>：同查询批次详情页。本类只覆盖该页明确列出的两个取值；
/// 创建批次页（<c>4012534633</c>）的取值表本轮未单独复核 ⇒ 若发现不同须分建常量。
/// </remarks>
public static class CouponTypes
{
    /// <summary>满减券（官方 <c>NORMAL</c>）。</summary>
    public const string Normal = "NORMAL";

    /// <summary>减至券（官方 <c>CUT_TO</c>）。</summary>
    public const string CutTo = "CUT_TO";
}

/// <summary>
/// 代金券批次类型（官方 <c>stock_type</c>）。
/// </summary>
/// <remarks><b>官方来源</b>：同查询批次详情页（取值 <c>NORMAL</c> / <c>DISCOUNT_CUT</c> / <c>OTHER</c>）。</remarks>
public static class CouponStockTypes
{
    /// <summary>代金券批次（官方 <c>NORMAL</c>）。</summary>
    public const string Normal = "NORMAL";

    /// <summary>立减与折扣（官方 <c>DISCOUNT_CUT</c>）。</summary>
    public const string DiscountCut = "DISCOUNT_CUT";

    /// <summary>其他（官方 <c>OTHER</c>）。</summary>
    public const string Other = "OTHER";
}

/// <summary>
/// 代金券指定支付模式（官方 <c>stock_use_rule.trade_type</c>，6 值）。
/// </summary>
/// <remarks>
/// <b>官方来源</b>：同查询批次详情页；官方注明「默认不限制」。
/// <b>⚠️ 注意 <c>PPAY</c> 是免密支付</b>（不是「预支付」之类），且 6 个取值拼写风格不统一。
/// </remarks>
public static class CouponTradeTypes
{
    /// <summary>小程序支付（官方 <c>MICROAPP</c>）。</summary>
    public const string MicroApp = "MICROAPP";

    /// <summary>APP 支付（官方 <c>APPPAY</c>）。</summary>
    public const string AppPay = "APPPAY";

    /// <summary>免密支付（官方 <c>PPAY</c>）。</summary>
    public const string Ppay = "PPAY";

    /// <summary>付款码支付 / 刷卡支付（官方 <c>CARD</c>）。</summary>
    public const string Card = "CARD";

    /// <summary>人脸支付（官方 <c>FACE</c>）。</summary>
    public const string Face = "FACE";

    /// <summary>其他支付（公众号、扫码等，官方 <c>OTHER</c>）。</summary>
    public const string Other = "OTHER";
}

/// <summary>
/// 消费金可用地域级别（官方 <c>available_region_list[].type</c>，4 值）。
/// </summary>
/// <remarks>
/// <b>官方来源</b>：同查询批次详情页；该列表<b>仅当</b> <c>business_type = MULTIUSE</c>（消费金）时返回。
/// </remarks>
public static class CouponRegionTypes
{
    /// <summary>国家级别可用（官方 <c>COUNTRY</c>）。</summary>
    public const string Country = "COUNTRY";

    /// <summary>省级（官方 <c>PROVINCE</c>）。</summary>
    public const string Province = "PROVINCE";

    /// <summary>市级（官方 <c>CITY</c>）。</summary>
    public const string City = "CITY";

    /// <summary>区级（官方 <c>DISTRICT</c>）。</summary>
    public const string District = "DISTRICT";
}

/// <summary>
/// 代金券业务类型（官方 <c>business_type</c>）。
/// </summary>
/// <remarks>
/// <b>官方来源</b>：同查询批次详情页；官方原文「<b>仅当</b> <c>business_type = MULTIUSE</c> 时返回」，
/// 故该字段与 <c>available_region_list</c> / <c>available_industry_list</c> 三者同进同出。
/// </remarks>
public static class CouponBusinessTypes
{
    /// <summary>消费金类型（官方 <c>MULTIUSE</c>）。</summary>
    public const string MultiUse = "MULTIUSE";
}
