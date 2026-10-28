// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表图片题的设置（官方 <c>image_setting</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：数量限制取值范围 [1, 9]，默认 9；单文件大小上限 3000MB。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormImageSetting
{
    /// <summary>获取或设置是否仅限手机拍照（官方 <c>camera_only</c>），默认 <c>false</c>。</summary>
    [JsonPropertyName("camera_only")]
    public bool? CameraOnly { get; set; }

    /// <summary>获取或设置数量和大小限制信息（官方 <c>upload_image_limit</c>）。</summary>
    [JsonPropertyName("upload_image_limit")]
    public WedocFormUploadLimit? UploadImageLimit { get; set; }
}
