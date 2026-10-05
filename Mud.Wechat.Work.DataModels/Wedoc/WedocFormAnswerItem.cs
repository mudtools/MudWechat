// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表单份答案（官方 <c>answer_list</c> 元素；读取收集表答案响应体嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：匿名填写时不返回 <c>tmp_external_userid</c> 与 <c>userid</c>；
/// <c>answer_status</c> 为 3 表示答案已被统计者移除或删除。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormAnswerItem
{
    /// <summary>获取或设置答案 id（官方 <c>answer_id</c>）。</summary>
    [JsonPropertyName("answer_id")]
    public ulong? AnswerId { get; set; }

    /// <summary>获取或设置填写人名字（官方 <c>user_name</c>），匿名填写不返回。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>获取或设置答案创建时间戳（官方 <c>ctime</c>）。</summary>
    [JsonPropertyName("ctime")]
    public ulong? Ctime { get; set; }

    /// <summary>获取或设置答案修改时间戳（官方 <c>mtime</c>）。</summary>
    [JsonPropertyName("mtime")]
    public ulong? Mtime { get; set; }

    /// <summary>获取或设置该用户的答案明细（官方 <c>reply</c>）。</summary>
    [JsonPropertyName("reply")]
    public WedocFormReply? Reply { get; set; }

    /// <summary>
    /// 获取或设置答案状态（官方 <c>answer_status</c>）。
    /// 官方取值：<c>1</c> 正常、<c>3</c> 统计者移除此答案或删除。
    /// </summary>
    [JsonPropertyName("answer_status")]
    public uint? AnswerStatus { get; set; }

    /// <summary>获取或设置外部用户临时 id（官方 <c>tmp_external_userid</c>），匿名填写不返回；同一用户在不同收集表中该 id 不一致。</summary>
    [JsonPropertyName("tmp_external_userid")]
    public string? TmpExternalUserid { get; set; }

    /// <summary>获取或设置企业内成员的 id（官方 <c>userid</c>），匿名填写不返回。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }
}
