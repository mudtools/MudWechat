// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantRiskManage;

/// <summary>
/// 「查询风险订单」服务商形态包体（<b>POST /v3/merchant-risk-manage/ec-trade-risk/query-risk-trades</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013894221"/>（支持商户【普通服务商】），2026-10-14 逐字段核验。</para>
/// <para><b>本域同一路径有两个商户形态、两套字段表</b>：服务商面用 <c>sp_mchid</c> + <c>sub_mchid</c>，
/// 普通商户面（<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894247"/>）只有 <c>mchid</c> ⇒
/// 按「表不同则分建」建两套请求与两套应答类型，跨形态复用会静默丢 <c>sp_mchid</c>（守卫 MRB-B6）。</para>
/// <para><b>官方把全部字段标选填</b>（含 <c>sp_mchid</c> / <c>sub_mchid</c>），但描述内注明
/// 「若为子商户调用，须传入对应的服务商号」⇒ 条件必填由官方判定，SDK 不做客户端校验。</para>
/// <para><b>分页是 <c>page_index</c> / <c>page_size</c> 整型页码</b>，不是本仓支付线常见的
/// <c>offset</c> / <c>limit</c> 游标形态，也不是 <c>cursor</c> ⇒ 照抄别域分页参数名会整批丢参数。</para>
/// <para><b>时间为 <c>string(128)</c> 的自由文本</b>（官方未写格式与跨度上限）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManagePartnerRiskTradeQueryRequest
{
    /// <summary>服务商商户号（<c>sp_mchid</c>，官方标选填 string(32)）：子商户调用时须传对应服务商号。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>特约商户号（<c>sub_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>开始时间（<c>start_time</c>，选填 string(128)）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>结束时间（<c>end_time</c>，选填 string(128)）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>页码（<c>page_index</c>，选填 integer）：本域分页参数名，非 <c>offset</c>。</summary>
    [JsonPropertyName("page_index")]
    public int? PageIndex { get; set; }

    /// <summary>页大小（<c>page_size</c>，选填 integer）：本域分页参数名，非 <c>limit</c>。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

/// <summary>
/// 「查询风险订单」普通商户形态包体：<b>与服务商形态同一路由</b>，标识字段换成 <c>mchid</c>。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894247"/>（支持商户【普通商户】），2026-10-14 逐字段核验。</para>
/// <para>本页请求表<b>没有</b> <c>sp_mchid</c> / <c>sub_mchid</c> 两行 ⇒ 与
/// <see cref="MerchantRiskManagePartnerRiskTradeQueryRequest"/> 不可互换（守卫 MRB-B6）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageMchRiskTradeQueryRequest
{
    /// <summary>商户号（<c>mchid</c>，官方标选填 string(32)）：普通商户形态的商户标识。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>开始时间（<c>start_time</c>，选填 string(128)）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>结束时间（<c>end_time</c>，选填 string(128)）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>页码（<c>page_index</c>，选填 integer）。</summary>
    [JsonPropertyName("page_index")]
    public int? PageIndex { get; set; }

    /// <summary>页大小（<c>page_size</c>，选填 integer）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

/// <summary>
/// 「查询风险订单」服务商形态应答：总数 + 风险订单列表（列表项<b>含 <c>sp_mchid</c></b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013894221"/>，2026-10-14 逐字段核验。</para>
/// <para><b>计数字段名是 <c>total_num</c></b>：与本仓服务商批量转账域一致，而银行面用 <c>total_count</c> ⇒ 跨域复制会丢字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManagePartnerRiskTradeQueryResponse : WechatPayResponse
{
    /// <summary>总风险订单数（<c>total_num</c>，选填 integer）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>风险订单列表（<c>risk_trade_list</c>，选填 array）：见 <see cref="MerchantRiskManagePartnerRiskTradeItem"/>。</summary>
    [JsonPropertyName("risk_trade_list")]
    public List<MerchantRiskManagePartnerRiskTradeItem>? RiskTradeList { get; set; }
}

/// <summary>
/// 「查询风险订单」普通商户形态应答：与服务商形态<b>同字段名同结构，但列表项内没有 <c>sp_mchid</c></b>。
/// </summary>
/// <remarks><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894247"/>，2026-10-14 两页比对。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageMchRiskTradeQueryResponse : WechatPayResponse
{
    /// <summary>总风险订单数（<c>total_num</c>，选填 integer）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }

    /// <summary>风险订单列表（<c>risk_trade_list</c>，选填 array）：见 <see cref="MerchantRiskManageMchRiskTradeItem"/>。</summary>
    [JsonPropertyName("risk_trade_list")]
    public List<MerchantRiskManageMchRiskTradeItem>? RiskTradeList { get; set; }
}

/// <summary>
/// 风险订单条目（服务商形态，官方 <c>risk_trade_list[]</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>risk_type</c> / <c>risk_level</c> 在本域是 integer</b>：官方行内枚举
/// <c>risk_type</c> = <c>1-涉嫌赌博,2-涉嫌欺诈,3-涉嫌色情,4-涉嫌洗钱</c>、
/// <c>risk_level</c> = <c>1-明确风险,2-高风险,3-高可疑</c>（各 4 值 / 3 值）。</para>
/// <para><b>与风险共建面的同名 <c>risk_type</c>（string，5 值）是两套体系</b> ⇒ 不得引用
/// <see cref="MerchantRiskManageTradeRiskTypes"/>，也不得共用 DTO（守卫 MRB-B3 逐类型锁定）。</para>
/// <para><b><c>risk_discove_time</c> 官方即此拼写</b>（缺 discover 的 r），且类型为<b>无长度约束的 string</b>、
/// 格式为 rfc3339 ⇒ 修正拼写会静默丢字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManagePartnerRiskTradeItem
{
    /// <summary>商户号（<c>mchid</c>，选填 string(32)）：风险订单对应的商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，选填 string(64)）：数字/大小写字母 <c>_-*</c>，同商户号下唯一。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>风险类型（<c>risk_type</c>，选填 <b>integer</b>）：官方 1-涉嫌赌博,2-涉嫌欺诈,3-涉嫌色情,4-涉嫌洗钱。</summary>
    [JsonPropertyName("risk_type")]
    public int? RiskType { get; set; }

    /// <summary>风险等级（<c>risk_level</c>，选填 <b>integer</b>）：官方 1-明确风险,2-高风险,3-高可疑。</summary>
    [JsonPropertyName("risk_level")]
    public int? RiskLevel { get; set; }

    /// <summary>服务商商户号（<c>sp_mchid</c>，选填 string）：<b>服务商形态应答独有</b>，官方类型未标长度。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>风险发现时间（<c>risk_discove_time</c>，选填 string）：rfc3339；<b>官方拼写即缺 r</b>，勿"顺手修正"。</summary>
    [JsonPropertyName("risk_discove_time")]
    public string? RiskDiscoveTime { get; set; }
}

/// <summary>
/// 风险订单条目（普通商户形态）：<b>比服务商形态少 <c>sp_mchid</c> 一行</b>，其余逐字段相同。
/// </summary>
/// <remarks><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894247"/>，2026-10-14 逐字段核验。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageMchRiskTradeItem
{
    /// <summary>商户号（<c>mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，选填 string(64)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>风险类型（<c>risk_type</c>，选填 <b>integer</b>）：官方 1-涉嫌赌博,2-涉嫌欺诈,3-涉嫌色情,4-涉嫌洗钱。</summary>
    [JsonPropertyName("risk_type")]
    public int? RiskType { get; set; }

    /// <summary>风险等级（<c>risk_level</c>，选填 <b>integer</b>）：官方 1-明确风险,2-高风险,3-高可疑。</summary>
    [JsonPropertyName("risk_level")]
    public int? RiskLevel { get; set; }

    /// <summary>风险发现时间（<c>risk_discove_time</c>，选填 string）：rfc3339；<b>官方拼写即缺 r</b>。</summary>
    [JsonPropertyName("risk_discove_time")]
    public string? RiskDiscoveTime { get; set; }
}

/// <summary>
/// 「回传处置结果」服务商形态包体（<b>POST /v3/merchant-risk-manage/ec-trade-risk/recv-dispose-result</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013894227"/>（支持商户【普通服务商】），2026-10-14 逐字段核验。</para>
/// <para><b><c>out_trade_no</c> 是本域四支里唯一的必填字段</b>（string(64)）⇒ 缺它即 <c>PARAM_ERROR</c>。</para>
/// <para><b><c>disp_conclusion</c> 是 <c>array[integer]</c></b>（官方 1-未处置,2-拦截订单,3-关闭店铺），
/// 不是 string 枚举 ⇒ 建成字符串列表会被官方拒。</para>
/// <para><b><c>refund_amount</c> 官方只写「退款金额」，未印单位与取值区间</b>（两轮定向复核一致）⇒
/// 本 SDK 按 integer 原样透传，<b>注释不得臆造「单位为分」</b>（守卫 MRB-B5 记录该空缺是官方正文口径）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManagePartnerDisposeResultRequest
{
    /// <summary>服务商商户号（<c>sp_mchid</c>，官方标选填 string(32)）：子商户调用时须传对应服务商号。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>特约商户号（<c>sub_mchid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，<b>必填</b> string(64)）：本域唯一必填字段。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>最终风险判定（<c>final_risk_result</c>，选填 integer）：官方 1-有风险,2-无风险。</summary>
    [JsonPropertyName("final_risk_result")]
    public int? FinalRiskResult { get; set; }

    /// <summary>最终风险类型（<c>final_risk_type</c>，选填 integer）：官方 1-涉嫌赌博,2-涉嫌欺诈,3-涉嫌色情,4-涉嫌洗钱。</summary>
    [JsonPropertyName("final_risk_type")]
    public int? FinalRiskType { get; set; }

    /// <summary>处置结论（<c>disp_conclusion</c>，选填 <b>array[integer]</b>）：官方 1-未处置,2-拦截订单,3-关闭店铺。</summary>
    [JsonPropertyName("disp_conclusion")]
    public List<int>? DispConclusion { get; set; }

    /// <summary>处置补充说明（<c>disp_addition</c>，选填 string(1024)）。</summary>
    [JsonPropertyName("disp_addition")]
    public string? DispAddition { get; set; }

    /// <summary>是否存在投诉（<c>exist_complaint</c>，选填 boolean）：官方 false-无，true-有。</summary>
    [JsonPropertyName("exist_complaint")]
    public bool? ExistComplaint { get; set; }

    /// <summary>退款金额（<c>refund_amount</c>，选填 integer）：<b>官方未印单位与区间</b>，原样透传。</summary>
    [JsonPropertyName("refund_amount")]
    public int? RefundAmount { get; set; }
}

/// <summary>
/// 「回传处置结果」普通商户形态包体：与服务商形态同一路由，标识字段换成 <c>mchid</c>。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894250"/>（支持商户【普通商户】），2026-10-14 逐字段核验。</para>
/// <para>两页<b>除商户标识行外逐字段一致</b>（含 <c>exist_complaint</c> 的行内 false/true 说明），
/// 应答类型共用 <see cref="MerchantRiskManageDisposeResultResponse"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageMchDisposeResultRequest
{
    /// <summary>商户号（<c>mchid</c>，官方标选填 string(32)）：普通商户形态的商户标识。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，<b>必填</b> string(64)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>最终风险判定（<c>final_risk_result</c>，选填 integer）：官方 1-有风险,2-无风险。</summary>
    [JsonPropertyName("final_risk_result")]
    public int? FinalRiskResult { get; set; }

    /// <summary>最终风险类型（<c>final_risk_type</c>，选填 integer）：官方 1-涉嫌赌博,2-涉嫌欺诈,3-涉嫌色情,4-涉嫌洗钱。</summary>
    [JsonPropertyName("final_risk_type")]
    public int? FinalRiskType { get; set; }

    /// <summary>处置结论（<c>disp_conclusion</c>，选填 <b>array[integer]</b>）：官方 1-未处置,2-拦截订单,3-关闭店铺。</summary>
    [JsonPropertyName("disp_conclusion")]
    public List<int>? DispConclusion { get; set; }

    /// <summary>处置补充说明（<c>disp_addition</c>，选填 string(1024)）。</summary>
    [JsonPropertyName("disp_addition")]
    public string? DispAddition { get; set; }

    /// <summary>是否存在投诉（<c>exist_complaint</c>，选填 boolean）：官方 false-无，true-有。</summary>
    [JsonPropertyName("exist_complaint")]
    public bool? ExistComplaint { get; set; }

    /// <summary>退款金额（<c>refund_amount</c>，选填 integer）：<b>官方未印单位与区间</b>，原样透传。</summary>
    [JsonPropertyName("refund_amount")]
    public int? RefundAmount { get; set; }
}

/// <summary>
/// 「回传处置结果」应答：<b>两支共用</b>，官方两页应答表同为单字段 <c>rsp_msg</c>。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013894227"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013894250"/>，2026-10-14 两页比对：应答表零差异 ⇒ 归并共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantRiskManage")]
public class MerchantRiskManageDisposeResultResponse : WechatPayResponse
{
    /// <summary>回传结果说明（<c>rsp_msg</c>，选填 string(1024)）。</summary>
    [JsonPropertyName("rsp_msg")]
    public string? RspMsg { get; set; }
}
