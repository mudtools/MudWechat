// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.PayScore;

namespace Mud.Wechat.Pay.DataModels.Combine;

/// <summary>
/// 合单支付·JSAPI / 小程序合单下单（<c>POST /v3/combine-transactions/jsapi</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter5_1_4.shtml"/>
/// （2026-10-09 逐字段核验；更新时间 2025.01.16）。<b>支持商户：【普通服务商】</b>
/// —— 合单支付是服务商能力，<b>普通商户不可用</b>（与支付线其它域不同，须注意）。
/// </para>
/// <para>
/// <b>官方要点</b>：① 服务商模式支持 <b>1–50 笔</b>订单合单支付（<c>sub_orders</c> 必填）；
/// ② <c>sub_appid</c> <b>仅允许一笔</b>商品单填写；③ <c>combine_payer_info</c> 中
/// <c>openid</c> 与 <c>sub_openid</c> <b>二选一必填</b>，且<b>传 <c>sub_openid</c> 则 <c>sub_appid</c> 必填</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombinePrepayRequest
{
    /// <summary>合单服务商 APPID（<c>combine_appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>合单服务商商户号（<c>combine_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>合单商户订单号（<c>combine_out_trade_no</c>，必填 string(32)）：商户系统内唯一，仅数字/大小写字母 <c>_-|*</c>。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填），见 <see cref="CombineSceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public CombineSceneInfo? SceneInfo { get; set; }

    /// <summary>商品单信息（<c>sub_orders</c>，必填，<b>1–50 笔</b>），见 <see cref="CombinePrepaySubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<CombinePrepaySubOrder>? SubOrders { get; set; }

    /// <summary>合单支付者信息（<c>combine_payer_info</c>，必填），见 <see cref="CombinePayerInfo"/>。</summary>
    [JsonPropertyName("combine_payer_info")]
    public CombinePayerInfo? CombinePayerInfo { get; set; }

    /// <summary>支付结束时间（<c>time_expire</c>，选填，rfc3339 格式）。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(255)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>场景信息（<c>scene_info</c>）。</summary>
/// <remarks><b>注意</b>：外层 <c>scene_info</c> 是选填，但其子字段 <c>payer_client_ip</c> 在官方表中标为<b>必填</b>（照录）。</remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineSceneInfo
{
    /// <summary>商户端设备号（<c>device_id</c>，选填 string(16)）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>用户终端 IP（<c>payer_client_ip</c>，<b>必填</b> string(45)，支持 IPv4/IPv6）。</summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }
}

/// <summary>下单的商品单条目（<c>sub_orders[]</c>，<c>UnionSubOrder</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombinePrepaySubOrder
{
    /// <summary>商品单商户号（<c>mchid</c>，必填 string(32)）：官方注明<b>填 <c>combine_mchid</c></b>。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户数据包（<c>attach</c>，必填 string(128)）：官方在本接口把它标为<b>必填</b>。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商品单金额信息（<c>amount</c>，必填），见 <see cref="CombineSubOrderAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public CombineSubOrderAmount? Amount { get; set; }

    /// <summary>商品单商户订单号（<c>out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，<b>必填</b> string(32)）：特约商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填），见 <see cref="CombineSettleInfo"/>。</summary>
    [JsonPropertyName("settle_info")]
    public CombineSettleInfo? SettleInfo { get; set; }

    /// <summary>子商户 APPID（<c>sub_appid</c>，选填 string(32)）：官方注明<b>仅允许一笔商品单填写</b>。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }
}

/// <summary>商品单金额信息（<c>amount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineSubOrderAmount
{
    /// <summary>标价金额（<c>total_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>标价币种（<c>currency</c>，必填 string(8)）：官方标注<b>固定 <c>CNY</c></b>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>结算信息（<c>settle_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineSettleInfo
{
    /// <summary>分账标识（<c>profit_sharing</c>，选填 bool）。</summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }
}

/// <summary>
/// 合单支付者信息（<c>combine_payer_info</c>）。
/// </summary>
/// <remarks>
/// <b>官方原文</b>：<c>openid</c> 与 <c>sub_openid</c> <b>二选一必填</b>；
/// <b>传 <c>sub_openid</c> 则 <c>sub_appid</c> 必填</b>（约束跨字段，无法用 DTO 表达，故在此留档）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombinePayerInfo
{
    /// <summary>用户服务商标识（<c>openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户子商户标识（<c>sub_openid</c>，选填 string(128)）：传此参数则 <c>sub_appid</c> 必填。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }
}

/// <summary>合单下单应答（<b>仅</b> <c>prepay_id</c> 一个字段）。</summary>
/// <remarks>官方注明 <c>prepay_id</c> <b>有效期 2 小时</b>，用于「调起支付」。</remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombinePrepayResponse : WechatPayResponse
{
    /// <summary>预支付交易会话标识（<c>prepay_id</c>，必填 string(64)，有效期 2 小时）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }
}

/// <summary>
/// 合单关闭订单（<c>POST /v3/combine-transactions/out-trade-no/{combine_out_trade_no}/close</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012709095"/>
/// （2026-10-09 逐字段核验；更新时间 2024.10.24）。<b>支持商户：【普通服务商】</b>。
/// </para>
/// <para>
/// <b>官方注意（原文，三条都是硬约束）</b>：① 合单支付订单<b>只能</b>使用此合单关单 API 完成关单；
/// ② <b>不支持关闭部分子单</b>；③ 关单的<b>主单商户号、单号、子单个数、子单商户号、子单单号
/// 必须与下单时完全一致</b> —— 即请求体要"重放"下单时的子单清单（但<b>字段集比下单少</b>，见
/// <see cref="CombineCloseSubOrder"/>）。
/// </para>
/// <para><b>无应答包体</b>：成功状态码 <b>204 No Content</b> ⇒ 接口方法返回 <c>Task</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineCloseOrderRequest
{
    /// <summary>合单 AppID（<c>combine_appid</c>，必填 string(32)）：合单发起方的 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>商品单信息（<c>sub_orders</c>，必填，最多 50 条），见 <see cref="CombineCloseSubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<CombineCloseSubOrder>? SubOrders { get; set; }
}

/// <summary>
/// 关单的商品单条目（<c>sub_orders[]</c>）—— <b>字段集与下单不同，故独立建模</b>。
/// </summary>
/// <remarks>
/// 与 <see cref="CombinePrepaySubOrder"/> 的差异（官方两页字段表不同）：
/// 关单<b>无</b> <c>attach</c> / <c>amount</c> / <c>description</c> / <c>settle_info</c> / <c>goods_tag</c>，
/// 且 <c>sub_mchid</c> / <c>sub_appid</c> 在本页是<b>选填</b>（下单页 <c>sub_mchid</c> 必填）。
/// 合并成一个类型会让两处各自带上对方才有的字段。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineCloseSubOrder
{
    /// <summary>商品单商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商品单订单号（<c>out_trade_no</c>，必填 string(32)）：须与下单时一致。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>二级商户号（<c>sub_mchid</c>，<b>选填</b> string(32)）：特约商户商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户绑定的 APPID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }
}

/// <summary>
/// 合单查询订单应答（<c>GET /v3/combine-transactions/out-trade-no/{combine_out_trade_no}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3_partner/apis/chapter7_3_11.shtml"/>
/// （2026-10-09 逐字段核验；更新时间 2024.10.24）。<b>支持商户：【普通服务商】</b>。
/// </para>
/// <para>
/// <b>查询方式只有一种</b>：官方本页仅给出按<b>合单商户订单号</b>查询（<c>out-trade-no/{combine_out_trade_no}</c>），
/// <b>无</b> query 参数、<b>无</b>「按微信支付订单号」的等价入口 —— 与单笔交易域（两种查询入口）<b>不同</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineQueryResponse : WechatPayResponse
{
    /// <summary>合单商户 AppID（<c>combine_appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>合单商户号（<c>combine_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>合单支付者信息（<c>combine_payer_info</c>，选填）：<b>本页只有 <c>openid</c></b>，见 <see cref="CombineQueryPayerInfo"/>。</summary>
    [JsonPropertyName("combine_payer_info")]
    public CombineQueryPayerInfo? CombinePayerInfo { get; set; }

    /// <summary>商品单列表（<c>sub_orders</c>，选填），见 <see cref="CombineQuerySubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<CombineQuerySubOrder>? SubOrders { get; set; }

    /// <summary>支付场景描述（<c>scene_info</c>，选填）：<b>本页只有 <c>device_id</c></b>，见 <see cref="CombineQuerySceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public CombineQuerySceneInfo? SceneInfo { get; set; }

    /// <summary>合单商户订单号（<c>combine_out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }
}

/// <summary>
/// 查询应答的支付者信息（<c>combine_payer_info</c>）—— <b>只有 <c>openid</c></b>。
/// </summary>
/// <remarks>
/// <b>为何不与下单的 <see cref="CombinePayerInfo"/> 共用</b>：官方查询页的 <c>combine_payer_info</c>
/// <b>没有</b> <c>sub_openid</c> 字段（下单页有）。共用会让调用方以为查询结果里能读到
/// <c>SubOpenId</c> 而永远拿到 <c>null</c> ——「永不返回的字段」比多一个类型更有害。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineQueryPayerInfo
{
    /// <summary>用户在商户 appid 下的唯一标识（<c>openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>
/// 查询应答的场景信息（<c>scene_info</c>）—— <b>只有 <c>device_id</c></b>。
/// </summary>
/// <remarks>官方查询页未列 <c>payer_client_ip</c>（下单页有，且为必填）⇒ 独立类型，不与下单共用（理由同 <see cref="CombineQueryPayerInfo"/>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineQuerySceneInfo
{
    /// <summary>商户端设备号（<c>device_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }
}

/// <summary>查询应答的商品单条目（<c>sub_orders[]</c>）。</summary>
/// <remarks>
/// <b>第三个子单形态</b>（下单 / 关单 / 查询各不相同）：本形态含<b>交易结果</b>字段
/// （<c>trade_state</c> 必填、<c>trade_type</c>/<c>bank_type</c>/<c>success_time</c>/<c>transaction_id</c>）
/// 与<b>实付金额</b>（<c>payer_amount</c> 必填），这些在下单/关单页都不存在 ⇒ 独立类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineQuerySubOrder
{
    /// <summary>商品单商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>交易类型（<c>trade_type</c>，选填 string）：取值见 <see cref="CombineTradeTypes"/>（H5 场景的取值是 <c>MWEB</c>）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>交易状态（<c>trade_state</c>，必填 string）：取值见 <see cref="CombineTradeStates"/>；须显式判定，勿假定查得到即已支付。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>付款银行（<c>bank_type</c>，选填 string(32)）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(128)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>success_time</c>，选填 string(32)）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>订单金额（<c>amount</c>，选填），见 <see cref="CombineQuerySubOrderAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public CombineQuerySubOrderAmount? Amount { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商品单订单号（<c>out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>特约商户商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户绑定的 Appid（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户 openid（<c>sub_openid</c>，选填 string(128)）：<c>sub_appid</c> 对应的 openid。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>优惠功能（<c>promotion_detail</c>，选填）：字段表与支付分域一致 ⇒ 复用 <c>PayScorePromotionDetail</c>（内含 <c>goods_detail</c>）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<PayScorePromotionDetail>? PromotionDetail { get; set; }
}

/// <summary>查询应答的商品单金额（<c>amount</c>）—— 含实付与汇率字段（下单页没有）。</summary>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineQuerySubOrderAmount
{
    /// <summary>标价金额（<c>total_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>用户支付金额（<c>payer_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("payer_amount")]
    public long? PayerAmount { get; set; }

    /// <summary>标价币种（<c>currency</c>，必填 string(16)）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>用户支付币种（<c>payer_currency</c>，必填 string(16)）。</summary>
    [JsonPropertyName("payer_currency")]
    public string? PayerCurrency { get; set; }

    /// <summary>结算汇率（<c>settlement_rate</c>，选填 integer）。</summary>
    [JsonPropertyName("settlement_rate")]
    public long? SettlementRate { get; set; }
}

/// <summary>
/// Native 合单下单请求（<c>POST /v3/combine-transactions/native</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012758240"/>
/// （服务商面，2026-10-09 逐字段核验；更新时间 2025.01.16）。<b>支持商户：【普通服务商】</b>。
/// </para>
/// <para>
/// <b>为何单独建类型而不复用 <see cref="CombinePrepayRequest"/></b>：两者顶层字段表<b>只差一项</b> ——
/// Native 页<b>没有</b> <c>combine_payer_info</c>（Native 支付二维码由用户扫码后自行确认，
/// 下单时<b>不需要</b>支付者标识）。若复用，调用方就能在 Native 请求里塞进官方<b>未定义</b>的
/// <c>combine_payer_info</c>，而该值对 Native 无意义（要么被忽略、要么被判参数错）。
/// 这正是本域 CB5 已确立的纪律：<b>表相同则共用，表不同则分建</b>。
/// </para>
/// <para>
/// <b>子单与场景信息则<b>复用</b> JSAPI 的类型</b>：官方两页的
/// <c>sub_orders[]</c>（含 <c>sub_mchid</c> 必填 / <c>sub_appid</c> 选填）与
/// <c>scene_info</c>（<c>device_id</c> 选填 / <c>payer_client_ip</c> 必填）字段表<b>逐项一致</b>
/// ⇒ 共用 <see cref="CombinePrepaySubOrder"/> / <see cref="CombineSceneInfo"/>（守卫 CB5 锁死该复用）。
/// </para>
/// <para>
/// <b>笔数限制</b>：官方原文「服务商模式支持 <b>1–50 笔</b>订单进行合单支付」；
/// (<b>普通商户</b>面另有对应页且「<b>只支持 2–10 笔</b>」，详见测试工程
/// <c>WechatPayCombineContractGuards</c> 的类注释 —— 该守卫跨程序集，故此处用 <c>&lt;c&gt;</c> 而非 cref)。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineNativePrepayRequest
{
    /// <summary>合单发起方的 AppID（<c>combine_appid</c>，必填 string(32)）：服务商的 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>合单商户订单号（<c>combine_out_trade_no</c>，必填 string(32)）：商户系统内部唯一，不超过 32 字符。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>合单发起方商户号（<c>combine_mchid</c>，必填 string(32)）：服务商的商户号。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填），见 <see cref="CombineSceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public CombineSceneInfo? SceneInfo { get; set; }

    /// <summary>商品单列表（<c>sub_orders</c>，必填 array，1–50 笔），见 <see cref="CombinePrepaySubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<CombinePrepaySubOrder>? SubOrders { get; set; }

    /// <summary>订单失效时间（<c>time_expire</c>，选填，rfc3339）：最短 1 分钟、最长 1 年。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>支付结果通知地址（<c>notify_url</c>，必填）：须为<b>可直接访问</b>的 https 地址，不允许携带参数。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>
/// Native 合单下单应答（<b>仅 <c>code_url</c> 一个字段</b>）。
/// </summary>
/// <remarks>
/// <b>为何不复用 <see cref="CombinePrepayResponse"/></b>：JSAPI 应答给的是 <c>prepay_id</c>
/// （供小程序 <c>wx.requestPayment</c> 调起），Native 给的是 <c>code_url</c>
/// （供服务端自行生成支付二维码）—— <b>二者不可互换</b>，官方两页应答表各自只有一个字段且字段名不同。
/// <para>
/// <b>有效期 2 小时</b>（官方原文）：失效后须<b>重新请求本接口</b>获取新的 <c>code_url</c>，
/// 不可对旧链接做任何拼接或改写。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineNativePrepayResponse : WechatPayResponse
{
    /// <summary>二维码链接（<c>code_url</c>，必填 string(512)）：有效期为 2 小时。</summary>
    [JsonPropertyName("code_url")]
    public string? CodeUrl { get; set; }
}

/// <summary>
/// APP 合单下单请求（<c>POST /v3/combine-transactions/app</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015973099"/>
/// （服务商面，2026-10-09 逐字段核验；更新时间 2025.09.02）。
/// <b>支持商户：【普通服务商】【平台商户】</b>；服务商模式支持 <b>1–50 笔</b>商品单。
/// </para>
/// <para>
/// <b>为何单独建类型而不复用 <see cref="CombinePrepayRequest"/> / <see cref="CombineNativePrepayRequest"/></b>：
/// 三个下单接口的顶层字段表<b>各不相同</b> ——
/// APP <b>无</b> <c>combine_payer_info</c>（APP 由客户端 SDK 调起，无需服务端传支付者标识），
/// 但<b>多</b> <c>time_start</c> 与 <c>prepay_id</c>；
/// Native 两者都没有。复用任一都会让调用方传官方<b>未定义</b>的字段（或传不了已定义的字段）。
/// </para>
/// <para>
/// <b>可复用的部分</b>：<c>scene_info</c> 与 JSAPI 页<b>逐项一致</b>（<c>device_id</c> 选填 /
/// <c>payer_client_ip</c> 必填，且<b>无</b> <c>h5_info</c>）⇒ 共用 <see cref="CombineSceneInfo"/>;
/// 应答与 JSAPI 同为<b>仅 <c>prepay_id</c></b> ⇒ 共用 <see cref="CombinePrepayResponse"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineAppPrepayRequest
{
    /// <summary>合单发起方的 AppID（<c>combine_appid</c>，必填 string(32)）：服务商的 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>合单商户订单号（<c>combine_out_trade_no</c>，必填 string(32)）：商户系统内部唯一，不超过 32 字符。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>合单发起方商户号（<c>combine_mchid</c>，必填 string(32)）：服务商的商户号。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填）：与 JSAPI 页同表 ⇒ 复用 <see cref="CombineSceneInfo"/>（<b>无</b> <c>h5_info</c>）。</summary>
    [JsonPropertyName("scene_info")]
    public CombineSceneInfo? SceneInfo { get; set; }

    /// <summary>商品单列表（<c>sub_orders</c>，必填 array，1–50 笔），见 <see cref="CombineAppSubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<CombineAppSubOrder>? SubOrders { get; set; }

    /// <summary>支付起始时间（<c>time_start</c>，选填，rfc3339）：又名「订单生效时间」。</summary>
    [JsonPropertyName("time_start")]
    public string? TimeStart { get; set; }

    /// <summary>支付结束时间（<c>time_expire</c>，选填，rfc3339）：超过 7 天未支付的订单无法再支付（官方原文）。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>
    /// 预支付交易会话标识（<c>prepay_id</c>，选填 string(64)）<b>—— 请求侧字段，勿与应答的 <c>prepay_id</c> 混淆</b>。
    /// </summary>
    /// <remarks>
    /// 官方中文说明：「【预支付交易会话标识】<b>当发起追加订单</b>，需传入目标追加的合单单号对应的
    /// 预支付交易会话标识」⇒ 只在<b>追加订单</b>场景填；普通首次下单<b>不传</b>。
    /// </remarks>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>支付结果通知地址（<c>notify_url</c>，必填）：须为可直接访问的 https 地址，不允许携带参数。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>
/// APP 合单下单的商品单条目（<c>sub_orders[]</c>，<b>APP 专用形态，11 字段</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不复用 <see cref="CombinePrepaySubOrder"/></b>：官方 APP 页的该表<b>多两个字段</b>
/// （<c>time_start</c> / <c>time_expire</c>，均选填）—— 复用会让 APP 调用方<b>无法</b>传这两个官方字段；
/// 反之把这两个字段加进 JSAPI 那支，JSAPI 调用方就能传官方该页<b>未定义</b>的字段。
/// 两条路都错，故分建（本域 CB5 纪律：<b>表相同则共用，表不同则分建</b>）。
/// </para>
/// <para>
/// <b>⚠️ 官方跨页不一致（照录，不「纠正」）</b>：<c>sub_mchid</c> 在本（APP）页标为<b>选填</b>，
/// 而在 JSAPI / H5 两页均标为<b>必填</b> ⇒ 本类按<b>本页</b>照录。两页事实<b>不得互推</b>：
/// 接入 APP 场景以 APP 页为准（至于实际调用是否放宽，由官方侧校验决定）。
/// </para>
/// <para>
/// <c>amount</c> 与 JSAPI 页同表（<c>total_amount</c> / <c>currency</c>）⇒ 共用
/// <see cref="CombineSubOrderAmount"/>；<c>settle_info</c> 同理共用 <see cref="CombineSettleInfo"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineAppSubOrder
{
    /// <summary>商品单商户号（<c>mchid</c>，必填 string(32)）：官方注明<b>填 <c>combine_mchid</c></b>。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户数据包（<c>attach</c>，必填 string(128)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商品单金额信息（<c>amount</c>，必填）：与 JSAPI 页同表 ⇒ 复用 <see cref="CombineSubOrderAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public CombineSubOrderAmount? Amount { get; set; }

    /// <summary>商品单商户订单号（<c>out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>）：<b>本（APP）页标注选填</b>，与 JSAPI / H5 页的「必填」不一致（见类型 remarks）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商品描述（<c>description</c>，必填 string(127)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>结算信息（<c>settle_info</c>，选填）：复用 <see cref="CombineSettleInfo"/>。</summary>
    [JsonPropertyName("settle_info")]
    public CombineSettleInfo? SettleInfo { get; set; }

    /// <summary>子商户 APPID（<c>sub_appid</c>，选填 string(32)）：官方注明<b>仅允许一笔商品单填写</b>。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>商品单支付起始时间（<c>time_start</c>，选填，rfc3339）<b>—— APP 页独有字段</b>（JSAPI 子单表没有）。</summary>
    [JsonPropertyName("time_start")]
    public string? TimeStart { get; set; }

    /// <summary>商品单支付结束时间（<c>time_expire</c>，选填，rfc3339）<b>—— APP 页独有字段</b>（JSAPI 子单表没有）。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }
}

/// <summary>
/// H5 合单下单请求（<c>POST /v3/combine-transactions/h5</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012758208"/>
/// （服务商面，2026-10-09 逐字段核验；更新时间 2025.01.16）。<b>支持商户：【普通服务商】</b>；
/// 服务商模式支持 <b>1–50 笔</b>商品单。
/// </para>
/// <para>
/// <b>与服务商面其它下单接口的字段差</b>：H5 <b>无</b> <c>combine_payer_info</c>、
/// <b>无</b> <c>time_start</c>，只有 <c>time_expire</c> ⇒ 单独建表。
/// </para>
/// <para>
/// <b>⚠️ 面（face）差异 —— 本域按服务商面建模，须留档</b>：<b>普通商户</b>面《合单下单-H5》
/// （<c>…/doc/v3/merchant/4012545879</c>，产品线标注为「<b>指定身份支付</b>」）的该接口
/// <b>有</b> <c>combine_payer_info</c>（含 <c>openid</c> 与 <c>identity</c> 实名子对象），
/// 且顶层另有 <c>time_start</c>；而<b>服务商面本页</b>明确<b>没有</b>这两处
/// ⇒ 本 DTO 按<b>服务商面</b>建模，普通商户接入时以自身页面字段为准（勿照本类硬编码）。
/// </para>
/// <para>
/// <b>可复用的部分</b>：<c>sub_orders</c> 与本域 JSAPI 页<b>同表</b>（9 字段，<c>sub_mchid</c> 必填，
/// 无 <c>time_start</c> / <c>time_expire</c>）⇒ 共用 <see cref="CombinePrepaySubOrder"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineH5PrepayRequest
{
    /// <summary>合单发起方的 AppID（<c>combine_appid</c>，必填 string(32)）：服务商的 AppID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>合单商户订单号（<c>combine_out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>合单发起方商户号（<c>combine_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填），见 <see cref="CombineH5SceneInfo"/>（<b>含</b> H5 专属 <c>h5_info</c>）。</summary>
    [JsonPropertyName("scene_info")]
    public CombineH5SceneInfo? SceneInfo { get; set; }

    /// <summary>商品单列表（<c>sub_orders</c>，必填 array，1–50 笔）：与本域 JSAPI 页同表 ⇒ 复用 <see cref="CombinePrepaySubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<CombinePrepaySubOrder>? SubOrders { get; set; }

    /// <summary>支付结束时间（<c>time_expire</c>，选填，rfc3339）。</summary>
    [JsonPropertyName("time_expire")]
    public string? TimeExpire { get; set; }

    /// <summary>支付结果通知地址（<c>notify_url</c>，必填）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }
}

/// <summary>
/// H5 合单下单的场景信息（<c>scene_info</c>，<b>H5 专用形态</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不复用 <see cref="CombineSceneInfo"/></b>：两点不同 ——
/// ① <c>device_id</c> 在本（H5）页标为<b>必填</b>，而 JSAPI / APP / Native 三页均为<b>选填</b>；
/// ② 本页多出 H5 专属的 <c>h5_info</c>（见 <see cref="CombineH5Info"/>）。
/// 服务商面 H5 页的 <c>device_id</c> 与 <c>payer_client_ip</c> <b>均为必填</b>（照录）。
/// </para>
/// <para>
/// <b>⚠️ 注意本页字段的必填性与其它页「正好相反」的一处</b>：H5 页把 <c>payer_client_ip</c> 标为<b>必填</b>
/// 而 <c>device_id</c> <b>也</b>是必填；其它页 <c>device_id</c> 是选填 ⇒ 从别的下单接口复制请求体时，
/// <b>最容易漏的正是 H5 场景下的 <c>device_id</c></b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineH5SceneInfo
{
    /// <summary>用户终端 IP（<c>payer_client_ip</c>，<b>必填</b> string(45)，支持 IPv4/IPv6）。</summary>
    [JsonPropertyName("payer_client_ip")]
    public string? PayerClientIp { get; set; }

    /// <summary>商户端设备号（<c>device_id</c>，<b>H5 页标注必填</b> string(32)）—— 与其它下单页的「选填」不一致（见类型 remarks）。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }

    /// <summary>H5 场景信息（<c>h5_info</c>，选填），见 <see cref="CombineH5Info"/>。</summary>
    [JsonPropertyName("h5_info")]
    public CombineH5Info? H5Info { get; set; }
}

/// <summary>
/// H5 支付场景信息（<c>h5_info</c>，<b>H5 合单下单专属</b>）。
/// </summary>
/// <remarks>
/// <b>官方来源</b>：服务商面《H5合单下单》页（<c>…/doc/v3/partner/4012758208</c>，更新 2025.01.16）。
/// 官方原文：<c>type</c> 为「H5 合单支付场景」<b>且给出取值 <c>Wap</c> / <c>iOS</c> / <c>Android</c></b>；
/// 其余四项（<c>app_name</c> / <c>app_url</c> / <c>bundle_id</c> / <c>package_name</c>）为选填。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineH5Info
{
    /// <summary>H5 支付场景类型（<c>type</c>，必填 string(32)）：取值 <c>Wap</c> / <c>iOS</c> / <c>Android</c>（官方给出）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>应用名（<c>app_name</c>，选填 string(64)）。</summary>
    [JsonPropertyName("app_name")]
    public string? AppName { get; set; }

    /// <summary>网站 URL（<c>app_url</c>，选填 string(128)）。</summary>
    [JsonPropertyName("app_url")]
    public string? AppUrl { get; set; }

    /// <summary>iOS bundle id（<c>bundle_id</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bundle_id")]
    public string? BundleId { get; set; }

    /// <summary>Android 包名（<c>package_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }
}

/// <summary>
/// H5 合单下单应答（<b>仅 <c>h5_url</c> 一个字段</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不复用 <see cref="CombinePrepayResponse"/></b>：JSAPI / APP 给的是 <c>prepay_id</c>，
/// 而 H5 给的是<b>可直接跳转的支付链接 <c>h5_url</c></b>（string(512)）—— 二者不可互换。
/// </para>
/// <para>
/// <b>官方另有《H5 调起支付》指引</b>：拿到 <c>h5_url</c> 后须按官方要求在
/// <c>Referer</c> 等条件下跳转（<b>不得</b>自行改写或拼装该链接）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CombineTransactions")]
public class CombineH5PrepayResponse : WechatPayResponse
{
    /// <summary>支付跳转链接（<c>h5_url</c>，必填 string(512)）：有效期与调起要求见官方《H5 调起支付》指引。</summary>
    [JsonPropertyName("h5_url")]
    public string? H5Url { get; set; }
}
