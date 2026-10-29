// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「上传小程序备案媒体材料」（<c>upload_icp_media</c>）应答。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/uploadIcpMedia.html"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面自相矛盾之处</b>：返回参数表写作 <c>created_at</c>，而同一页的返回示例写作 <c>create_at</c>。
/// 本 DTO 照<b>参数表</b>（官方字段规格）取 <c>created_at</c>；若实测拿到的是示例拼法，须以逐页取证更正本键并同批更新守卫 QRC4，
/// <b>不得</b>「两个键都收」做成宽容反序列化——那会掩盖契约漂移。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpUploadIcpMediaResponse : OpenPlatformResponse
{
    /// <summary>媒体材料类型（官方 <c>type</c>，string：<c>image</c> / <c>video</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>媒体标识（官方 <c>media_id</c>，可重复用于一次备案申请的多个字段）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>创建时间（官方 <c>created_at</c>，number，UNIX 秒级时间戳）。</summary>
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
}
