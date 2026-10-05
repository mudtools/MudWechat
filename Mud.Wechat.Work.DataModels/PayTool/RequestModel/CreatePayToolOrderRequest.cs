// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 创建收款订单请求体（<c>/cgi-bin/paytool/open_order</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>签名三要素</b>：本端点须携带 <see cref="NonceStr"/> / <see cref="Ts"/> / <see cref="Sig"/>，
/// 签名算法见官方 <see href="https://developer.work.weixin.qq.com/document/path/98768">path 98768 签名算法</see>，
/// SDK 侧实现见 <c>WechatPayToolSignature</c>（密钥获取路径：工作台→企业微信服务商助手→工具→收银台→收银台 API 调用密钥）。
/// </para>
/// <para>
/// <b>官方权限口径</b>：服务商需有在收银台完成商户号注册（支付方式为「免支付」的订单可以不受此限制）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class CreatePayToolOrderRequest
{
    /// <summary>
    /// 获取或设置业务类型（官方必填）：<c>1</c>-普通第三方应用 / <c>2</c>-代开发应用 / <c>3</c>-行业解决方案。
    /// <para>官方必填口径：<c>product_list</c> 中与该值对应的分支才填，其余分支不填。</para>
    /// </summary>
    [JsonPropertyName("business_type")]
    public int BusinessType { get; set; }

    /// <summary>
    /// 获取或设置客户企业 corpid（不多于 64 字节）。
    /// <para>官方必填口径：<b>业务类型为「代开发应用」（2）时必填</b>；
    /// 「普通第三方应用」与「行业解决方案」可以不指定。</para>
    /// </summary>
    [JsonPropertyName("custom_corpid")]
    public string? CustomCorpid { get; set; }

    /// <summary>
    /// 获取或设置支付方式（官方必填）：<c>0</c>-客户支付 / <c>1</c>-服务商代支付 / <c>2</c>-免支付。
    /// <para>官方限制（行业解决方案分支）：订单包含应用全部为非推荐第三方应用才可免支付。</para>
    /// </summary>
    [JsonPropertyName("pay_type")]
    public int PayType { get; set; }

    /// <summary>
    /// 获取或设置银行收款回单凭证的 media_id（官方可选）。
    /// <para>官方必填口径：支付方式选择「服务商代支付」时，需上传企业已支付服务商订单费用的凭证
    /// （凭证需是银行收款回单或发票）。该 ID 即通过「服务商上传临时素材」上传文件后得到的 media_id，
    /// 支持图片（jpg/png/jpeg/bmp）、pdf；调用上传素材接口时需指定 <c>attachment_type=3</c>（专用于收银台）。</para>
    /// </summary>
    [JsonPropertyName("bank_receipt_media_id")]
    public string? BankReceiptMediaId { get; set; }

    /// <summary>
    /// 获取或设置订单创建人的 userid（官方可选）。
    /// <para>官方约束：设置的创建人需要有收银台收款的权限。设置后，「企业取消应用订单」、
    /// 「应用订单确认失败提醒」的消息会推送给创建人；<b>没有设置则默认推送给服务商所有超管</b>。</para>
    /// </summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>
    /// 获取或设置具体购买商品（官方必填，见 <see cref="PayToolProductList"/>）。
    /// </summary>
    [JsonPropertyName("product_list")]
    public PayToolProductList ProductList { get; set; } = new PayToolProductList();

    /// <summary>
    /// 获取或设置随机字符串（官方必填，长度要求在 32 字节以内）。
    /// <para>官方约束：用于保证签名不可预测及防重放攻击，<b>需保证 15 分钟内不能重复</b>，
    /// 推荐使用随机字符串生成算法。</para>
    /// </summary>
    [JsonPropertyName("nonce_str")]
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 unix 时间戳（中国时区，精确到秒，官方必填）。
    /// <para>官方约束：<b>业务系统的机器时间与腾讯的时间相差不能超过 15 分钟</b>。</para>
    /// </summary>
    [JsonPropertyName("ts")]
    public long Ts { get; set; }

    /// <summary>
    /// 获取或设置数字签名（官方必填，见 <see cref="Sig"/> 的签名算法说明）。
    /// </summary>
    [JsonPropertyName("sig")]
    public string Sig { get; set; } = string.Empty;
}
