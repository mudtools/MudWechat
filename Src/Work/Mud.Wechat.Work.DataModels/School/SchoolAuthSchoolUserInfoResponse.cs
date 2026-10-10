// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 获取家校访问用户身份响应体（<c>/cgi-bin/school/getuserinfo</c>，家校沟通-网页授权登录，
/// 企业自建应用与服务商代开发公共）。
/// <para>
/// 官方形态：按用户身份二选一返回 —— 学校家长返回 <see cref="ParentUserid"/>；
/// 学校学生返回 <see cref="StudentUserid"/>；两形态均携带 <see cref="DeviceId"/>
/// （注意官方字段名为 PascalCase 的 <c>DeviceId</c>）。
/// </para>
/// <para>
/// 官方业务限制：code 最大 512 字节、每次授权的 code 不同、只能使用一次、
/// 5 分钟未被使用自动过期；跳转域名须完全匹配该 access_token 对应应用的可信域名，否则返回 50001。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolAuthSchoolUserInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置手机设备号（官方字段名即 PascalCase 的 <c>DeviceId</c>；
    /// 由企业微信在安装时随机生成，删除重装会改变，升级不受影响）。
    /// </summary>
    [JsonPropertyName("DeviceId")]
    public string? DeviceId { get; set; }

    /// <summary>
    /// 获取或设置家校通讯录里家长的 userid（用户为学校的家长时返回）。
    /// </summary>
    [JsonPropertyName("parent_userid")]
    public string? ParentUserid { get; set; }

    /// <summary>
    /// 获取或设置家校通讯录里学生的 userid（用户为学校的学生时返回）。
    /// </summary>
    [JsonPropertyName("student_userid")]
    public string? StudentUserid { get; set; }
}
