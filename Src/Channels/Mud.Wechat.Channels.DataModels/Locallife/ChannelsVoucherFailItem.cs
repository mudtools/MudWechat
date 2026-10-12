// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>无法操作的券明细（官方 <c>fail_voucher_list</c> 数组元素；consumevoucher 与 withdrawconsume 响应字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherFailItem
{
    /// <summary>获取或设置操作的券 code（官方 <c>code</c>；核销/撤销时传 code 或传了有效加密 code 时返回）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置操作的券 sku_id（官方 <c>sku_id</c>）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }

    /// <summary>获取或设置核销/撤销时传入的加密 code（官方 <c>encrypted_code</c>；加密 code 非法时返回）。</summary>
    [JsonPropertyName("encrypted_code")]
    public string? EncryptedCode { get; set; }

    /// <summary>获取或设置操作结果（官方 <c>ret_code</c>，官方类型为 string）。</summary>
    [JsonPropertyName("ret_code")]
    public string? RetCode { get; set; }

    /// <summary>获取或设置操作结果说明（官方 <c>ret_msg</c>）。</summary>
    [JsonPropertyName("ret_msg")]
    public string? RetMsg { get; set; }
}
