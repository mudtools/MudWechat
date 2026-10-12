// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>查询用户的团购优惠详情（<c>getvoucher</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<c>code</c> 与 <c>encrypted_code</c> 二者选其一；使用 <c>encrypted_code</c> 查询时不用传 <c>sku_id</c>。
/// 官方说明原文将加密 code 写作 <c>encrypted_data</c>，参数表字段名为 <c>encrypted_code</c>（矛盾照录，按参数表建模）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherRequest
{
    /// <summary>获取或设置用户订单详情页展示的二维码（官方 <c>encrypted_code</c>，选填；需进行 URL 解码，与 <c>code</c> 二选一）。</summary>
    [JsonPropertyName("encrypted_code")]
    public string? EncryptedCode { get; set; }

    /// <summary>获取或设置用户的券 code（官方 <c>code</c>，选填；查询用户的券接口返回的 code，与 <c>encrypted_code</c> 二选一）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>，选填；使用 <c>encrypted_code</c> 查询时不传）。</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; set; }
}
