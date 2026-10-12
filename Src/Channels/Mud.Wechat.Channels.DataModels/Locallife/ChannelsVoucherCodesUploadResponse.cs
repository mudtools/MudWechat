// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>上传团购优惠 code（<c>uploadvouchercode</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherCodesUploadResponse : ChannelsResponse
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置本次上传去重后的 code 总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>获取或设置本次上传操作上传成功个数（官方 <c>success_count</c>）。</summary>
    [JsonPropertyName("success_count")]
    public int? SuccessCount { get; set; }

    /// <summary>获取或设置上传成功的 code 列表（官方 <c>success_codes</c>）。</summary>
    [JsonPropertyName("success_codes")]
    public List<string>? SuccessCodes { get; set; }

    /// <summary>获取或设置本次上传操作上传失败的 code 数（官方 <c>fail_count</c>）。</summary>
    [JsonPropertyName("fail_count")]
    public int? FailCount { get; set; }

    /// <summary>获取或设置导入失败的 code 信息（官方 <c>fail_codes</c>，修改后重试）。</summary>
    [JsonPropertyName("fail_codes")]
    public List<ChannelsVoucherCodeFailItem>? FailCodes { get; set; }
}
