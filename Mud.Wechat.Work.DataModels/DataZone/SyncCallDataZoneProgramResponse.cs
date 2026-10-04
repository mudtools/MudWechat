// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 应用同步调用专区程序响应体（<c>/cgi-bin/chatdata/sync_call_program</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class SyncCallDataZoneProgramResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置专区程序的输出结果（官方 response_data）。
    /// <para>为自定义的 JSON 字符串，要求与管理端配置的输出协议格式匹配；
    /// 如使用专区程序示例，留意 Java 版本 demo 的输出没有在 response_data 内包裹一层 output 对象。</para>
    /// </summary>
    [JsonPropertyName("response_data")]
    public string? ResponseData { get; set; }
}
