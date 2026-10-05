// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表各题型的额外设置容器（官方 <c>question_extend_setting</c>），
/// 按 <see cref="WedocFormItem.ReplyType"/> 题型取对应设置对象；官方未给出下拉列表与签名题型的额外设置。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormExtendSetting
{
    /// <summary>获取或设置文本题的题目设置（官方 <c>text_setting</c>，reply_type = 1）。</summary>
    [JsonPropertyName("text_setting")]
    public WedocFormTextSetting? TextSetting { get; set; }

    /// <summary>获取或设置单选题的校验设置（官方 <c>radio_setting</c>，reply_type = 2），不填则不会校验。</summary>
    [JsonPropertyName("radio_setting")]
    public WedocFormRadioSetting? RadioSetting { get; set; }

    /// <summary>获取或设置多选题的校验设置（官方 <c>checkbox_setting</c>，reply_type = 3），不填则不会校验。</summary>
    [JsonPropertyName("checkbox_setting")]
    public WedocFormCheckboxSetting? CheckboxSetting { get; set; }

    /// <summary>获取或设置位置题的设置（官方 <c>location_setting</c>，reply_type = 5）。</summary>
    [JsonPropertyName("location_setting")]
    public WedocFormLocationSetting? LocationSetting { get; set; }

    /// <summary>获取或设置图片题的设置（官方 <c>image_setting</c>，reply_type = 9）。</summary>
    [JsonPropertyName("image_setting")]
    public WedocFormImageSetting? ImageSetting { get; set; }

    /// <summary>获取或设置文件题的设置（官方 <c>file_setting</c>，reply_type = 10）。</summary>
    [JsonPropertyName("file_setting")]
    public WedocFormFileSetting? FileSetting { get; set; }

    /// <summary>获取或设置日期题的设置（官方 <c>date_setting</c>，reply_type = 11）。</summary>
    [JsonPropertyName("date_setting")]
    public WedocFormDateSetting? DateSetting { get; set; }

    /// <summary>获取或设置时间题的设置（官方 <c>time_setting</c>，reply_type = 14）。</summary>
    [JsonPropertyName("time_setting")]
    public WedocFormTimeSetting? TimeSetting { get; set; }

    /// <summary>获取或设置时长题的设置（官方 <c>duration_setting</c>，reply_type = 22）。</summary>
    [JsonPropertyName("duration_setting")]
    public WedocFormDurationSetting? DurationSetting { get; set; }

    /// <summary>获取或设置体温题的设置（官方 <c>temperature_setting</c>，reply_type = 16）。</summary>
    [JsonPropertyName("temperature_setting")]
    public WedocFormTemperatureSetting? TemperatureSetting { get; set; }

    /// <summary>获取或设置部门题的设置（官方 <c>department_setting</c>，reply_type = 18）。</summary>
    [JsonPropertyName("department_setting")]
    public WedocFormDepartmentSetting? DepartmentSetting { get; set; }

    /// <summary>获取或设置成员题的设置（官方 <c>member_setting</c>，reply_type = 19）。</summary>
    [JsonPropertyName("member_setting")]
    public WedocFormMemberSetting? MemberSetting { get; set; }
}
