// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantStore;

/// <summary>
/// 绑定门店收款商户号请求（<b>POST /v3/merchant-store/stores/{store_id}/recipients/bind</b> 的包体；两形态共用）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：商户面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085771"/>、
/// 服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948579"/>，2026-10-14 两页逐字段比对
/// —— 服务商面只多一行选填 <c>sub_mchid</c>，其余逐字段相同 ⇒ 共用本类型。</para>
/// <para><b>与解绑支的唯一必填性差异</b>：bind 的 <c>store_recipient</c> 是<b>必填</b>数组，
/// unbind 的同名字段官方标<b>选填</b> ⇒ 两支各建请求类型，不得互换（守卫 MST-B4 锁定该分歧）。</para>
/// <para><b>绑定 ≠ 生效</b>：官方状态机有 <c>STORE_STATE_RECIPIENT_VERIFYING</c>（待收款商户号超管确认）
/// 与 <c>STORE_STATE_RECIPIENT_REFUSED</c>（被拒或超 10 天未确认）两档 ⇒ 本支 200 + 空失败列表
/// 只代表「受理成功」，最终结论须经查询支读 <c>store_state</c>。</para>
/// <para><b>一个门店最多绑定五个收款商户号</b>，且绑定主体须与品牌主体或既有收款方一致。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreRecipientBindRequest
{
    /// <summary>特约商户号（<c>sub_mchid</c>，官方标选填 string(16)）：服务商调用场景须传。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>待绑定收款商户号列表（<c>store_recipient</c>，<b>必填</b> array）：见 <see cref="MerchantStoreRecipientInfo"/>。</summary>
    [JsonPropertyName("store_recipient")]
    public List<MerchantStoreRecipientInfo>? StoreRecipient { get; set; }
}

/// <summary>
/// 解绑门店收款商户号请求（<b>POST /v3/merchant-store/stores/{store_id}/recipients/unbind</b> 的包体；两形态共用）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：商户面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085773"/>、
/// 服务商面 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4013948580"/>，2026-10-14 两页逐字段比对。</para>
/// <para><b><c>store_recipient</c> 官方标选填</b>（与 bind 的必填相对）：官方页未说明「不传」的语义
/// ⇒ SDK 侧不校验、不代填、也不把它当成「解绑全部」的简写，宿主须显式给出待解绑项。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreRecipientUnbindRequest
{
    /// <summary>特约商户号（<c>sub_mchid</c>，官方标选填 string(16)）：服务商调用场景须传。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>待解绑收款商户号列表（<c>store_recipient</c>，官方标<b>选填</b> array）：见 <see cref="MerchantStoreRecipientInfo"/>。</summary>
    [JsonPropertyName("store_recipient")]
    public List<MerchantStoreRecipientInfo>? StoreRecipient { get; set; }
}

/// <summary>
/// 绑定 / 解绑收款商户号的应答（<b>两支共用</b>，官方两页应答表逐字段相同）：只回<b>失败项</b>。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085771"/>、
/// <see href="https://pay.weixin.qq.com/doc/v3/merchant/4014085773"/>，2026-10-14 逐字段核验。</para>
/// <para><b>成功项不出现在应答里</b>：<c>failed_store_recipient</c> 是选填数组，官方注明「仅失败时返回」
/// ⇒ 「字段缺失 / 空数组」= 全部受理成功，<b>不能</b>按「没返回即没生效」处置。</para>
/// <para><b>本域两支都是 POST 且路由只差末段动词</b>（<c>/bind</c> 与 <c>/unbind</c>），
/// 与「写操作走 PATCH」的直觉相反；<c>store_id</c> 是 path 参数而非包体字段。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreRecipientOperateResponse : WechatPayResponse
{
    /// <summary>失败的收款商户号列表（<c>failed_store_recipient</c>，选填 array）：<b>仅失败项返回</b>，见 <see cref="MerchantStoreFailedRecipient"/>。</summary>
    [JsonPropertyName("failed_store_recipient")]
    public List<MerchantStoreFailedRecipient>? FailedStoreRecipient { get; set; }
}

/// <summary>
/// 绑定 / 解绑失败项（官方 <c>failed_store_recipient[]</c>）。
/// </summary>
/// <remarks>
/// <b>与请求项 <see cref="MerchantStoreRecipientInfo"/> 不是同一类型</b>：这里第二列是
/// <c>failed_reason</c>（string(1024)，官方标选填）而非 <c>company_name</c> ⇒ 复用请求项类型会静默丢掉失败原因。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantStore")]
public class MerchantStoreFailedRecipient
{
    /// <summary>收款商户号（<c>mchid</c>，必填 string(16)）：本次操作失败的门店收款商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>失败原因（<c>failed_reason</c>，选填 string(1024)）。</summary>
    [JsonPropertyName("failed_reason")]
    public string? FailedReason { get; set; }
}
