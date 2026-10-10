// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 录入打卡人员人脸信息请求体（<c>/cgi-bin/checkin/addcheckinuserface</c>；人脸信息仅用于人脸打卡）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：图片数据不超过 1M；对已有人脸的用户，传入的人脸会覆盖原有人脸，请谨慎操作。
/// 官方契约陷阱：userid 与 userface 的官方「必须」列标注为「否」（与常规必填预期不同，照抄）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class AddCheckinUserFaceRequest
{
    /// <summary>获取或设置需要录入的用户 id（官方「必须」列标注为否，照抄）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置需要录入的人脸图片数据（需要将图片数据 base64 处理后填入，非 media_id；对已录入的人脸会进行更新处理；官方「必须」列标注为否，照抄）。</summary>
    [JsonPropertyName("userface")]
    public string? Userface { get; set; }
}
