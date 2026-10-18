// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 订单评价拉取（<c>/billcommentsp/batchquerycomment</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>本族没有应答 DTO 是有意设计</b>：官方成功应答<b>不是 XML</b>，而是逐行以反引号开头、
/// 逗号分隔的文本表（首行为本批之后的偏移量）⇒ 收发器走原文出口 <c>SendAsync</c> 返回
/// <see cref="string"/>，失败时的 XML 信封亦原样交回由调用方判别（与 <c>/pay/downloadbill</c> 同一口径）。</para>
/// <para><b><c>sign_type</c> 默认值即契约面</b>：官方「签名类型，目前仅支持HMAC-SHA256，默认就是HMAC-SHA256」
/// ⇒ 必填栏虽写「否」，本类仍把默认值钉为 <c>HMAC-SHA256</c>（APIv2 的算法判定看字段表里有没有
/// <c>sign_type=HMAC-SHA256</c>，留空会被签名器按 MD5 计算，而本支官方<b>不支持</b> MD5 ⇒ 必然 <c>SIGNERROR</c>）。</para>
/// <para><b>时间格式是 <c>yyyyMMddHHmmss</c></b>（14 位无分隔符），与违规记录族的
/// <c>yyyy-MM-dd HH:mm:ss</c> 不同；窗口按「评论时间」计，且官方限定<b>只能拉最近三个月</b>
/// （错误码 <c>TIME_EXPIRE</c> 的解决方案原文即「请拉取90天内的数据」）。</para>
/// <para><b>需商户证书、但域名是 <c>api.mch</c></b>：本页「是否需要证书」为「需要」，
/// 证书缺失时官方返回 <c>return_code=FAIL</c> 且 <c>return_msg</c> 原文为「商户证书错误」，
/// 错误码表<b>没有</b> <c>CERT_ERROR</c> ⇒ 判错只能认这句 <c>return_msg</c>。
/// 「要证书」与「挂在证书域名上」是两个独立事实，本支属前者（与违规记录、资金提现查询同形）。</para>
/// <para><b>分页靠 <c>offset</c> 游标而非页码</b>：官方语义「从某条记录的下一条开始返回」，
/// 首批传 <c>0</c>，续拉取上一次应答<b>首行</b>的那个数字 ⇒ 循环必须以「本批条数 &lt; limit」或
/// 空批为终止条件，不能以 offset 递增猜测总条数。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2BatchQueryCommentRequest
{
    /// <summary>公众账号ID（官方必填）。</summary>
    [JsonPropertyName("appid")] public string? AppId { get; set; }

    /// <summary>微信支付分配的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：官方「目前仅支持HMAC-SHA256，默认就是HMAC-SHA256」⇒ 默认值即契约面，勿清空。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>按用户评论时间批量拉取的起始时间，格式 <c>yyyyMMddHHmmss</c>。</summary>
    [JsonPropertyName("begin_time")] public string? BeginTime { get; set; }

    /// <summary>拉取的结束时间，格式 <c>yyyyMMddHHmmss</c>；与起始时间的区间不得超过三个月。</summary>
    [JsonPropertyName("end_time")] public string? EndTime { get; set; }

    /// <summary>偏移量（官方类型 uint64）：从某条记录的下一条开始返回，首批为 <c>0</c>。</summary>
    [JsonPropertyName("offset")] public long? Offset { get; set; }

    /// <summary>一次拉取条数（官方类型 uint32）：最大 <b>200</b>。</summary>
    [JsonPropertyName("limit")] public int? Limit { get; set; }

    /// <summary>签名（由证书收发器按字段表计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}
