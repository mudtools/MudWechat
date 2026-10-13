// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文件分块上传初始化响应体（<c>/cgi-bin/wedoc/file_upload_init</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <c>upload_key</c> 与 <c>file_url</c> 官方均为可选返回字段，二者以 <c>hit_exist</c> 判别：
/// 命中秒传时无需再调用分块与完成端点。SDK 侧不做本地硬拦，调用方须自行按 <c>hit_exist</c> 分支。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class InitWedocFileUploadResponse : WechatWorkResponse
{
    /// <summary>获取或设置是否命中秒传（官方字段名为 <c>hit_exist</c>，官方线格式即 JSON 布尔）。</summary>
    [JsonPropertyName("hit_exist")]
    public bool? HitExist { get; set; }

    /// <summary>获取或设置文件上传凭证（官方：<b>不命中秒传时</b>返回，作为 file_upload_part 与 file_upload_finish 的 upload_key 参数）。</summary>
    [JsonPropertyName("upload_key")]
    public string? UploadKey { get; set; }

    /// <summary>获取或设置文件 URL（官方：<b>命中秒传时</b>返回，此时上传流程即完成，无须再调用分块与完成端点）。</summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }
}
