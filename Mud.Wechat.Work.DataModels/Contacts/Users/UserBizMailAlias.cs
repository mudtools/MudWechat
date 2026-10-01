// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 企业邮箱别名（<c>biz_mail_alias</c>，更新成员接口）。
/// </summary>
/// <remarks>覆盖式更新：传空结构或空数组即清空别名。</remarks>
public class UserBizMailAlias
{
    /// <summary>
    /// 获取或设置别名列表（每项为 6~63 字节的有效邮箱格式且企业内唯一，最多 5 个）。
    /// </summary>
    [JsonPropertyName("item")]
    public List<string>? Item { get; set; }
}
