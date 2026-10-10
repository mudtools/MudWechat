// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 批量外呼响应体（<c>/cgi-bin/meeting/phone/callout</c>；在拨打后立刻返回呼叫状态，可通过查询接口和回调事件获取）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class PstnBatchCalloutResponse : WechatWorkResponse
{
    /// <summary>获取或设置成功外呼的电话号码对象数组（详见 <see cref="PstnCalloutPhoneNumber"/>）。</summary>
    [JsonPropertyName("phone_numbers")]
    public List<PstnCalloutPhoneNumber>? PhoneNumbers { get; set; }

    /// <summary>获取或设置不合法的外呼电话号码对象数组（详见 <see cref="PstnPhoneNumber"/>）。</summary>
    [JsonPropertyName("invalid_phone_numbers")]
    public List<PstnPhoneNumber>? InvalidPhoneNumbers { get; set; }
}
