// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Tags;

/// <summary>
/// 创建标签请求体（<c>/cgi-bin/tag/create</c>）。
/// </summary>
/// <remarks>创建的标签属于该应用，只有该应用的 secret 才可以增删成员；标签总数不能超过 3000 个。</remarks>
public class CreateTagRequest
{
    /// <summary>
    /// 获取或设置标签名称（≤ 32 个字，汉字或英文字母；不可与其他标签重名）。
    /// </summary>
    [JsonPropertyName("tagname")]
    public string TagName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置标签 ID（非负整型；指定时新增的标签会生成对应的标签 ID，不填则以目前最大的 ID 自增）。
    /// </summary>
    [JsonPropertyName("tagid")]
    public int? TagId { get; set; }
}
