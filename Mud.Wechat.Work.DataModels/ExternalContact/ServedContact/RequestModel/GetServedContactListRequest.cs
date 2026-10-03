// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.ServedContact;

/// <summary>
/// 获取已服务的外部联系人请求体（<c>/cgi-bin/externalcontact/contact_list</c>）。
/// <para>cursor 有有效期，请勿缓存后使用；每次请求首个分页（cursor 为空）时，
/// 返回的临时 id 和 next_cursor 都会变化，外部联系人临时 id 仅在一轮完整遍历查询中唯一。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ServedContact")]
public class GetServedContactListRequest
{
    /// <summary>
    /// 获取或设置用于分页查询的游标，由上一次调用返回，首次调用可不填（cursor 有有效期，请勿缓存后使用）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数（整型，默认 1000）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
