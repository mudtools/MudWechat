// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 上传任务信息（官方 <c>Res.task_info</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>errcode</c>/<c>errmsg</c> 是任务级结果，不是接口级</b>：接口级错误码在外层 <see cref="Mud.Wechat.MiniProgram.DataModels.WxaResponse"/>，任务失败时本类型内的 <c>errcode</c> 才非零（0 表示成功）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_gettask.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodTaskInfo
{
    /// <summary>id（官方类型 <c>number</c>）：任务id</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>task_type（官方类型 <c>number</c>）：任务类型</summary>
    [JsonPropertyName("task_type")]
    public long? TaskType { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：任务状态</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>errcode（官方类型 <c>number</c>）：任务错误码，0表示成功，其它表示失败</summary>
    [JsonPropertyName("errcode")]
    public long? Errcode { get; set; }

    /// <summary>errmsg（官方类型 <c>string</c>）：任务错误原因</summary>
    [JsonPropertyName("errmsg")]
    public string? Errmsg { get; set; }

    /// <summary>create_time（官方类型 <c>number</c>）：创建时间，时间戳</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>finish_time（官方类型 <c>number</c>）：完成时间，时间戳</summary>
    [JsonPropertyName("finish_time")]
    public long? FinishTime { get; set; }

    /// <summary>media_id（官方类型 <c>number</c>）：媒体文件id</summary>
    [JsonPropertyName("media_id")]
    public long? MediaId { get; set; }
}
