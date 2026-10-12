// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>待撤回核销的券条目（官方 <c>reovke_vouchers</c> 数组元素，withdrawconsume 请求）。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsRevokeVoucherItem
{
    /// <summary>获取或设置券加密 code（官方 <c>encrypted_code</c>，选填；和券 code 二选一）。</summary>
    [JsonPropertyName("encrypted_code")]
    public string? EncryptedCode { get; set; }

    /// <summary>获取或设置券 code（官方 <c>code</c>，选填；和券加密 code 二选一）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置商品下的 sku id（官方 <c>sku_id</c>，选填；填写 code 时必填）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置撤销张数（官方 <c>cancel_count</c>，选填；官方已废弃——照录，勿在新代码中使用）。</summary>
    [JsonPropertyName("cancel_count")]
    public int? CancelCount { get; set; }
}
