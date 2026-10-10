// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 获取家校访问用户身份响应体（第三方应用，<c>/cgi-bin/service/school/getuserinfo3rd</c>，
/// 家校沟通-网页授权登录）。
/// <para>
/// 官方形态：按用户身份二选一返回 —— 学校家长返回 <see cref="Parents"/>（家长列表）；
/// 学校学生返回 <see cref="Students"/>（学生列表）；两形态均携带 <see cref="DeviceId"/>。
/// </para>
/// <para>
/// 官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、
/// 5 分钟未被使用自动过期；跳转域名须完全匹配应用的可信域名，否则返回 50001。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolAuthThirdPartySchoolUserInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置手机设备号（官方字段名即 PascalCase 的 <c>DeviceId</c>；
    /// 由企业微信在安装时随机生成，删除重装会改变，升级不受影响）。
    /// </summary>
    [JsonPropertyName("DeviceId")]
    public string? DeviceId { get; set; }

    /// <summary>
    /// 获取或设置家长列表（用户为学校的家长时返回）。
    /// </summary>
    [JsonPropertyName("parents")]
    public List<SchoolAuthThirdPartyParentItem>? Parents { get; set; }

    /// <summary>
    /// 获取或设置学生列表（用户为学校的学生时返回）。
    /// </summary>
    [JsonPropertyName("students")]
    public List<SchoolAuthThirdPartyStudentItem>? Students { get; set; }
}
