// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

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
