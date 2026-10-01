// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.FollowUser;

/// <summary>
/// 获取客户可建联成员响应体（<c>/cgi-bin/externalcontact/customer_acquisition_app/get_permit</c>；
/// 仅营销获客类应用可调用）。
/// </summary>
public class GetCustomerAcquisitionPermitResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置可建联成员 ID 列表。
    /// </summary>
    [JsonPropertyName("user_list")]
    public List<string>? UserList { get; set; }

    /// <summary>
    /// 获取或设置可建联部门 ID 列表（可再经「获取部门成员」接口展开为成员 ID）。
    /// </summary>
    [JsonPropertyName("department_list")]
    public List<long>? DepartmentList { get; set; }

    /// <summary>
    /// 获取或设置可建联标签 ID 列表。
    /// </summary>
    [JsonPropertyName("tag_list")]
    public List<long>? TagList { get; set; }
}
