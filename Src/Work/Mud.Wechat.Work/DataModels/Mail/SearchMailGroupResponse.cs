// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 模糊搜索邮件群组响应体（<c>/cgi-bin/exmail/group/search</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class SearchMailGroupResponse : WechatWorkResponse
{
    /// <summary>获取或设置返回条数（官方 count）。</summary>
    /// <remarks>
    /// <para>
    /// 官方文档参数表将 count 标注为 string，但响应示例作数字（如 <c>2</c>），
    /// 两处形态不一致；本模型以示例为准按整数承载。
    /// </para>
    /// </remarks>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>获取或设置邮件群组列表（官方 groups；<see cref="MailGroupBrief"/>）。</summary>
    [JsonPropertyName("groups")]
    public List<MailGroupBrief>? Groups { get; set; }
}
