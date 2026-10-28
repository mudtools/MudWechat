// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Tag;

/// <summary>
/// 添加企业客户标签请求体中的标签项（<c>/cgi-bin/externalcontact/add_corp_tag</c> 与
/// <c>/cgi-bin/externalcontact/add_strategy_tag</c> 共用）。
/// <para>组内标签不可同名，同名标签只会创建一个；不支持创建空标签组。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class CorpTagCreateItem
{
    /// <summary>
    /// 获取或设置添加的标签名称，最长 30 字符（官方必填；不可与同一标签组下的其他标签重名）。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置标签次序值，官方有效范围 [0, 2^32)，值大的排序靠前。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }
}
