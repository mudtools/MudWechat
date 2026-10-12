// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 上传发表内容中的图片（<c>media/uploadimg</c>，官方现名「上传发表内容中的图片」）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>POST multipart</b>（表单字段 <c>media</c>）；
/// 图片仅支持 jpg/png，大小必须在 <b>1MB 以下</b>；返回 <c>url</c>（<b>非 media_id</b>，
/// 无 media_id 生命周期），且「不占用公众号的素材库中图片数量的 100000 个的限制」。
/// </para>
/// <para>
/// <b>归属说明</b>：官方索引页把本端点同时列于「群发消息」与「永久素材」分组 ⇒ 只建模一次
/// （落素材域；群发域 XML 交叉引用，不再建第二份声明）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpUploadImageResponse : MpResponse
{
    /// <summary>获取或设置图片 URL（官方 <c>url</c>；用于图文正文，非 media_id 形态）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
