// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>上传失败的 code 信息（官方 <c>fail_codes</c> 数组元素，uploadvouchercode 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherCodeFailItem
{
    /// <summary>获取或设置商家自定义券 code（官方 <c>code</c>，代表唯一一张券）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置对应券 code 上传失败的错误码（官方 <c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public int? ErrCode { get; set; }

    /// <summary>获取或设置上传失败的错误信息描述（官方 <c>errmsg</c>）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}
