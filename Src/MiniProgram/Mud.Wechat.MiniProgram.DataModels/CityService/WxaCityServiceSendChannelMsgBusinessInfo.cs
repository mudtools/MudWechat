// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 就医助手消息业务字段（<c>business_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>不同 status 对应不同结构</b>：官方说明 <c>business_info</c> 随消息子状态变化，
/// 本类收录通用消息接口页给出的字段超集；未列出的状态专属字段须按官方 status 说明补齐。</para>
/// <para><b>敏感面</b>：<c>pat_name</c> / <c>pat_hospital_id</c> / 预约时间一律禁入日志、
/// 遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceSendChannelMsgBusinessInfo
{
    /// <summary>用户在该医院的就诊卡号（必填）。敏感字段，禁入日志。</summary>
    [JsonPropertyName("pat_hospital_id")]
    public string? PatHospitalId { get; set; }

    /// <summary>用户姓名（必填）。敏感字段，禁入日志。</summary>
    [JsonPropertyName("pat_name")]
    public string? PatName { get; set; }

    /// <summary>医生姓名（可选）；暂无具体医生时填医生级别（如主治医生）。</summary>
    [JsonPropertyName("doc_name")]
    public string? DocName { get; set; }

    /// <summary>科室名称（必填），含楼栋楼层。</summary>
    [JsonPropertyName("department_name")]
    public string? DepartmentName { get; set; }

    /// <summary>科室位置（可选）。</summary>
    [JsonPropertyName("department_location")]
    public string? DepartmentLocation { get; set; }

    /// <summary>预约时间（必填），格式如 <c>2023-06-07 10:30-11:00</c>。</summary>
    [JsonPropertyName("appointment_time")]
    public string? AppointmentTime { get; set; }

    /// <summary>就医须知（可选）。</summary>
    [JsonPropertyName("memo")]
    public string? Memo { get; set; }

    /// <summary>医院跳转页面信息（可选）。</summary>
    [JsonPropertyName("redirect_page")]
    public WxaCityServiceRedirectPage? RedirectPage { get; set; }

    /// <summary>医院适老化页面的跳转信息（可选）。</summary>
    [JsonPropertyName("elder_redirect_page")]
    public WxaCityServiceRedirectPage? ElderRedirectPage { get; set; }
}
