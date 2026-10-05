// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 创建用户专属参会链接请求体（<c>/cgi-bin/meeting/create_customer_short_url</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：该接口不支持网络研讨会（Webinar）；不同链接以 <c>customer_data</c> 进行区分。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class CreateMeetingCustomerShortUrlRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置用户专属字段（官方必填，长度不超过 256 字节）。
    /// <para>
    /// <c>customer_data</c> 需以 <c>{"ver": "1.0", "userData":"自定义字段"}</c> 的结构进行 Base64 编码；
    /// 通过用户入会、用户进入等候室等事件，或通过获取等候室成员列表的 API 可查询到该参数。
    /// </para>
    /// </summary>
    [JsonPropertyName("customer_data")]
    public string? CustomerData { get; set; }
}
