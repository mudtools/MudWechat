// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>获取分阶段发布详情应答（<c>POST /wxa/getgrayreleaseplan</c>）。</summary>
/// <remarks>
/// 官方文档：<c>operation/api_getgrayreleaseplan.html</c>。
/// <see cref="GrayReleasePlan"/> 为官方 <c>gray_release_plan</c> 对象（含灰度状态、时间窗、灰度比例等），
/// 字段随版本演进以原始键值对透传。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaGrayReleasePlanResponse : WxaResponse
{
    /// <summary>分阶段发布计划（<c>gray_release_plan</c>，官方原始键值对）。</summary>
    [JsonPropertyName("gray_release_plan")]
    public Dictionary<string, JsonElement>? GrayReleasePlan { get; set; }
}
