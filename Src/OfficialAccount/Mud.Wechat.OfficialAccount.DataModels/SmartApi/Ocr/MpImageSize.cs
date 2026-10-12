// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// OCR / 图像处理共用的图片尺寸（官方 <c>img_size</c>）。
/// </summary>
/// <remarks>官方字段表原文：<c>w</c> = 宽度，<c>h</c> = 高度。</remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpImageSize
{
    /// <summary>获取或设置图片宽度（官方 <c>w</c>）。</summary>
    [JsonPropertyName("w")]
    public int? Width { get; set; }

    /// <summary>获取或设置图片高度（官方 <c>h</c>）。</summary>
    [JsonPropertyName("h")]
    public int? Height { get; set; }
}
