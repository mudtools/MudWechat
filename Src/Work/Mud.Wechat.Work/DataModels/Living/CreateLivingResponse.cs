// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 创建预约直播响应体（<c>/cgi-bin/living/create</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class CreateLivingResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置直播 id。
    /// <para>可通过此 id 调用「进入直播」接口（包括小程序接口和 JS-SDK 接口），
    /// 实现主播到点后的开播操作，以及观众进入直播详情预约和观看直播。</para>
    /// </summary>
    [JsonPropertyName("livingid")]
    public string? Livingid { get; set; }
}
