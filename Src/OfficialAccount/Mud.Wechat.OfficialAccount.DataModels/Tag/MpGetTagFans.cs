// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Tag;

/// <summary>
/// 获取标签下粉丝列表请求体（<c>getTagFans</c>，<c>POST /cgi-bin/user/tag/get</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>tagid</c>（标签 ID）与 <c>next_openid</c>（第一个拉取的 OPENID，不填默认从头开始拉取）。
/// </para>
/// <para>
/// <b>官方文档缺陷（勿「顺手修正」）</b>：字段表把两者「必填」列均标为「否」，但 <c>tagid</c> 语义上必须提供
/// （官方亦给 <c>45159 invalid tag id</c>），故本 SDK 按必填建模。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpGetTagFansRequest
{
    /// <summary>标签 ID。</summary>
    [JsonPropertyName("tagid")]
    public int TagId { get; set; }

    /// <summary>第一个拉取的 OPENID（不填或传空字符串表示从头开始拉取）。</summary>
    [JsonPropertyName("next_openid")]
    public string? NextOpenId { get; set; }
}

/// <summary>
/// 获取标签下粉丝列表响应（<c>getTagFans</c>）。
/// </summary>
/// <remarks>
/// 官方响应字段：<c>count</c>（本次获取的粉丝数量）/ <c>data.openid</c>（粉丝 openid 列表）/
/// <c>next_openid</c>（拉取列表最后一个用户的 openid，用于下一页）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpGetTagFansResponse : MpResponse
{
    /// <summary>本次获取的粉丝数量。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>标签下粉丝数据（失败或该页为空时缺省为 <c>null</c>）。</summary>
    [JsonPropertyName("data")]
    public MpGetTagFansData? Data { get; set; }

    /// <summary>拉取列表最后一个用户的 openid（分页游标）。</summary>
    [JsonPropertyName("next_openid")]
    public string? NextOpenId { get; set; }
}

/// <summary>标签下粉丝数据（<c>getTagFans</c> 响应的 <c>data</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpGetTagFansData
{
    /// <summary>粉丝 openid 列表。</summary>
    [JsonPropertyName("openid")]
    public List<string>? OpenId { get; set; }
}
