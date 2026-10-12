// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 永久素材的视频描述（<c>add_material</c> 的 multipart <c>description</c> 表单字段形态）。
/// </summary>
/// <remarks>
/// <para>
/// 官方表单字段 <c>description</c> 为 JSON 字符串（<c>-F description='{"title":…, "introduction":…}'</c>）；
/// 本 DTO 供调用方经 <c>MediaJsonContext</c> 序列化后作为表单 <c>description</c> 字段值传入。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：字段表将 <c>description</c>/<c>title</c>/<c>introduction</c>
/// 全部标为可选（否），但正文写「上传视频素材时需要」⇒ SDK 不做本地必填拦截，按官方错误码表达。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpMaterialDescription
{
    /// <summary>获取或设置视频素材描述标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置视频素材描述简介（官方 <c>introduction</c>）。</summary>
    [JsonPropertyName("introduction")]
    public string? Introduction { get; set; }
}
