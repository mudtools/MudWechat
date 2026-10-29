// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>智能裁剪结果条目（官方 <c>results[]</c>，裁剪区域四边界）。</summary>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpImageCropResult
{
    /// <summary>获取或设置左上角 x（官方 <c>crop_left</c>）。</summary>
    [JsonPropertyName("crop_left")]
    public int? CropLeft { get; set; }

    /// <summary>获取或设置左上角 y（官方 <c>crop_top</c>）。</summary>
    [JsonPropertyName("crop_top")]
    public int? CropTop { get; set; }

    /// <summary>获取或设置右下角 x（官方 <c>crop_right</c>）。</summary>
    [JsonPropertyName("crop_right")]
    public int? CropRight { get; set; }

    /// <summary>获取或设置右下角 y（官方 <c>crop_bottom</c>）。</summary>
    [JsonPropertyName("crop_bottom")]
    public int? CropBottom { get; set; }
}
