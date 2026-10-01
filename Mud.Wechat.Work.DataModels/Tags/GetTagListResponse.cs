// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Tags;

/// <summary>
/// 获取标签列表响应体（<c>/cgi-bin/tag/list</c>；自建应用 / 通讯录同步助手可获取所有标签，
/// 第三方应用与代开发自建应用仅可获取自己创建的标签）。
/// </summary>
public class GetTagListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置标签列表。
    /// </summary>
    [JsonPropertyName("taglist")]
    public List<TagInfo>? TagList { get; set; } = [];
}
