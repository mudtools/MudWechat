// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>核销用户的团购优惠（<c>consumevoucher</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约：<c>consume_request_no</c> 为核销操作幂等串（商户自定义、小店下唯一、最长 128 字节）；
/// <c>encrypted_codes</c>（从订单详情页跳转 path 获取）与 <c>codes + sku_id</c> 二选一；
/// 次卡传多次相同值代表核销多次；多张券整体同成功或同失败；数组大小最大为 10。
/// </para>
/// <para>
/// <b>官方文档矛盾照录</b>：官方参数表字段名为 <c>consume_store_name</c>，请求示例写作 <c>store_name</c>
/// ——按参数表建模。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsConsumeVoucherRequest
{
    /// <summary>获取或设置核销操作幂等串（官方 <c>consume_request_no</c>，必填；商户自定义，小店下唯一，最长 128 字节）。</summary>
    [JsonPropertyName("consume_request_no")]
    public string ConsumeRequestNo { get; set; } = string.Empty;

    /// <summary>获取或设置加密 code 列表（官方 <c>encrypted_codes</c>，选填；与 <c>codes + sku_id</c> 二选一，最大 10 个）。</summary>
    [JsonPropertyName("encrypted_codes")]
    public List<string>? EncryptedCodes { get; set; }

    /// <summary>获取或设置用户券 code 列表（官方 <c>codes</c>，选填；与 <c>encrypted_codes</c> 二选一，最大 10 个）。</summary>
    [JsonPropertyName("codes")]
    public List<string>? Codes { get; set; }

    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>，选填；非加密 code 核销时必传）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置 C 端展示的核销门店名称（官方 <c>consume_store_name</c>，选填；仅展示，最长 128 字节；官方示例写作 store_name——矛盾照录）。</summary>
    [JsonPropertyName("consume_store_name")]
    public string? ConsumeStoreName { get; set; }

    /// <summary>获取或设置核销渠道（官方 <c>consume_channel</c>，必填，见 <see cref="ChannelsVoucherConsumeChannels"/>）。</summary>
    [JsonPropertyName("consume_channel")]
    public int ConsumeChannel { get; set; }

    /// <summary>获取或设置商家自定义门店 id（官方 <c>out_store_id</c>，选填；仅作标记）。</summary>
    [JsonPropertyName("out_store_id")]
    public string? OutStoreId { get; set; }

    /// <summary>获取或设置预约单号（官方 <c>reserve_no</c>，选填；在预约白名单内且商品开启预约时，需先预约才能核销）。</summary>
    [JsonPropertyName("reserve_no")]
    public string? ReserveNo { get; set; }
}
